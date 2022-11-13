using System.Diagnostics;

namespace YouTubeGrabber;

class Program
{
    private static int Main(string[] args)
    {
        if (!File.Exists("youtube-dl.exe")) return 1;
        if (!File.Exists("ffmpeg.exe")) return 1;
        
        // Prompt the user for their playlist URL
        Console.WriteLine("Enter the URL of the YouTube playlist you want to download: ");
        string playlist_url = Console.ReadLine();

        if (Directory.Exists("tracks"))
        {
            Directory.Delete("tracks", true);
        }
        Directory.CreateDirectory("tracks");
        
        Console.WriteLine("Downloading the playlist...");
        var processInfo = new ProcessStartInfo("youtube-dl.exe", "--yes-playlist -x --audio-format mp3 -o \"tracks/%(title)s.%(ext)s\" " + playlist_url);
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
        
        Console.WriteLine("Tracks downloaded successfully!");

        return 0;
    }

}