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

        static async Task Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
            // Console.WriteLine("Running Spotify Test");
            // await SpotifyTest.Test();
            // Console.WriteLine("Spotify Test Finished");

            FileManager.ClearOutputDirectories();

            MapRequestManager mapRequestManager = new MapRequestManager();
            HashSet<string> mapIdsDownloaded = new HashSet<string>();
            
            foreach (int playlistId in PlaylistIds)
            {
                // Download Playlist
                BPList bpList = mapRequestManager.RequestPlaylist(playlistId, out string playlistPath);
                if(bpList == null)
                    continue;
                
                Console.WriteLine("Saved Playlist: " + playlistPath);
                Console.WriteLine("Requesting " + bpList.songs.Count + " maps...");

                // Download map data and trigger async map file downloads
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
                    
                    mapRequestManager.RequestMapAsync(mapData);
                }
            }
            
            while (mapRequestManager.songsLeftToDownload > 0)
            {
                Console.WriteLine("Waiting for " + mapRequestManager.songsLeftToDownload + " songs to download");
                Thread.Sleep(750);
            }
            
            Console.WriteLine("Song and playlist download complete");
        }
    }
}