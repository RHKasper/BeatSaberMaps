using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeatSaberUnzipper
{
    class Program
    {
        private static readonly int[] PlaylistIds = {3210, 2363, 2364, 3209};
        private static int songsLeftToDownload = 0;

        static async Task Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
            // Console.WriteLine("Running Spotify Test");
            // await SpotifyTest.Test();
            // Console.WriteLine("Spotify Test Finished");

            FileManager.ClearOutputDirectories();

            songsLeftToDownload = 0;

            MapRequestManager mapRequestManager = new MapRequestManager(FileManager.MapCachePath);
            HashSet<string> mapIdsDownloaded = new HashSet<string>();
            
            foreach (int playlistId in PlaylistIds)
            {
                //download playlist file
                BPList bpList = BeatSaverDownloader.GetBpList(playlistId, out string fileContents);
                if (bpList == null)
                {
                    Console.WriteLine($"playlist {playlistId} could not be downloaded or read");
                    continue;
                }
            
                // save playlist file contents
                string path = Path.Combine(FileManager.PlaylistsFolderPath, bpList.playlistTitle + ".bplist");
                File.WriteAllText(path, fileContents);
                Console.WriteLine("Saved Playlist: " + path);
                Console.WriteLine("Queueing " + bpList.songs.Count + " for download...");
                
                // Download songs
                foreach (Song song in bpList.songs)
                {
                    MapData mapData = BeatSaverDownloader.GetMapData(song);
                    if (mapData == null)
                    {
                        Console.WriteLine($"Downloading {song.songName} map data failed");
                        continue;
                    }
            
                    if (mapIdsDownloaded.Contains(mapData.id))
                        continue;
                    else
                        mapIdsDownloaded.Add(mapData.id);

                    songsLeftToDownload++;
                    mapRequestManager.RequestMapAsync(mapData, OnSongFinishedDownloading);
                }
            }
            
            while (songsLeftToDownload != 0)
            {
                Console.WriteLine("Waiting for " + songsLeftToDownload + " songs to download");
                Thread.Sleep(750);
            }
            
            Console.WriteLine("Song and playlist download complete");
        }

        private static void OnSongFinishedDownloading(string path)
        {
            FileManager.UnzipFile(path);
            songsLeftToDownload--;
        }
    }
}