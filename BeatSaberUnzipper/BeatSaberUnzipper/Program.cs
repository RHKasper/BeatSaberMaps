﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            
            int mapDataCountRequested = 0;
            int mapDataCountReceived = 0;
            MapRequestManager mapRequestManager = new MapRequestManager();

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
                    mapRequestManager.RequestMapDataAsync(song);
            }
            
            Stopwatch timer = Stopwatch.StartNew();

            while ((timer.Elapsed.TotalSeconds < 10 && mapRequestManager.mapDataLeftToDownload >0 ) || mapRequestManager.zipFilesLeftToDownload > 0)
            {
                Console.WriteLine($"Waiting for {mapRequestManager.mapDataLeftToDownload} map data requests and {mapRequestManager.zipFilesLeftToDownload} zip file requests");
                Thread.Sleep(750);
            }
            
            Console.WriteLine("Song and playlist download complete");
        }
    }
}