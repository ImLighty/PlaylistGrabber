
# PlaylistGrabber

Tools that might be useful to download a huge playlist to enjoy it while being offline.
This needs still a lot of work since I want to optimize it for bigger playlists (like my friends' one which has 385 songs in it).




## Installation

At the moment, both releases are Windows only, but if you want to make it working on Linux, you can make a pull request for it.

1. [Download](https://github.com/ImLighty/PlaylistGrabber/releases/latest) the binaries for the respective playlist
2. Download both [ffmpeg.exe](https://www.gyan.dev/ffmpeg/builds/ffmpeg-git-essentials.7z) and [youtube-dl.exe](https://youtube-dl.org/downloads/latest/youtube-dl.exe) and put them in the same directory
   where the program is located
3. **SPOTIFY ONLY:** Create an application in the Spotify [Developers Dashboard](https://developer.spotify.com/dashboard/applications),
   then create a `client.json` file which should look like this:
   ```json
   {
     "client_id": "<your spotify client id here>",
     "client_secret": "<your spotify client secret here>"
   }
   ```
4. Launch the program and input the link of the playlist you want to download.
5. Done!
## FAQ

#### Why do I have to use my own Spotify credentials?

Because Spotify might have some API limitations when it comes to use that.

#### Why is it slow to download my 1K+ songs playlist?

You see... I have to figure out how to split the work within multiple threads/processes,
so it will be faster compared to the current method I'm using.

