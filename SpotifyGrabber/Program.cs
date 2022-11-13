using System.Diagnostics;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using YTSearch.NET;

namespace SpotifyGrabber;

class Program
{
    private static int Main(string[] args)
    {
        string client_id;
        string client_secret;

        if (!File.Exists("client.json")) return 1;
        if (!File.Exists("youtube-dl.exe")) return 1;
        if (!File.Exists("ffmpeg.exe")) return 1;
        
        // Prompt the user for their playlist URL
        Console.WriteLine("Enter the URL of the Spotify playlist you want to download: ");
        string playlist_url = Console.ReadLine();

        using (StreamReader r = new StreamReader("client.json"))
        {
            string json = r.ReadToEnd();
            // Read the json file and get the client secret and client ID
            dynamic array = JsonConvert.DeserializeObject(json);
            if (array == null) return 1;
            client_id = array.client_id;
            if (client_id == null) return 1;
            client_secret = array.client_secret;
            if (client_secret == null) return 1;
        }
        
        var GrabberSpotifyConfig = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(new ClientCredentialsAuthenticator(client_id, client_secret));

        var GrabberSpotify = new SpotifyClient(GrabberSpotifyConfig);
        
        // Get the playlist ID from the URL
        string playlist_id = playlist_url.Substring(playlist_url.LastIndexOf('/') + 1);
        
        // Get the playlist
        var playlist = GrabberSpotify.Playlists.Get(playlist_id).Result;
        Console.WriteLine("Playlist found: " + playlist.Name);
        
        // Get all the tracks instead of just the first 100
        var tracks = GrabberSpotify.PaginateAll(playlist.Tracks);

        // Create a new file
        string path = playlist.Name + ".txt";
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (Directory.Exists(playlist.Name))
        {
            Directory.Delete(playlist.Name, true);
        }
        Directory.CreateDirectory(playlist.Name);
        
        foreach (PlaylistTrack<IPlayableItem> item in tracks.Result)
        {
            string query = "Never Gonna Give You Up - Rick Astley";
            string ytUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            if (item.Track is FullTrack track)
            {
                query = track.Name + " - " + track.Artists[0].Name;
                Console.WriteLine("Searching for: " + query);
                ytUrl = AsyncYouTubeMusicSearch(query).GetAwaiter().GetResult();
            }
            if (item.Track is FullEpisode episode)
            {
                query = episode.Name + " - " + episode.Show.Name;
                ytUrl = AsyncYouTubeMusicSearch(query).GetAwaiter().GetResult();
            }
            Console.WriteLine("Downloading: " + ytUrl);
            var processInfo = new ProcessStartInfo("youtube-dl.exe", "-x --audio-format mp3 -o \"" + playlist.Name + "/" + query + ".%(ext)s\" " + ytUrl);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            var process = Process.Start(processInfo);

#if DEBUG
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("Output >> " + e.Data);
            process.BeginOutputReadLine();
#endif
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("Error >> " + e.Data);
            process.BeginErrorReadLine();

            process.WaitForExit();

#if DEBUG
            Console.WriteLine("Exit code: {0}", process.ExitCode);
#endif
            process.Close();
        }
        
        Console.WriteLine("Tracks downloaded successfully!");

        return 0;
    }
    
    public static async Task<string> AsyncYouTubeMusicSearch(string query)
    {
        var GrabberYouTubeClient = new YouTubeSearchClient();
        var results = await GrabberYouTubeClient.SearchYoutubeAsync(query);
        return results.Results.First().Url;
    }

}