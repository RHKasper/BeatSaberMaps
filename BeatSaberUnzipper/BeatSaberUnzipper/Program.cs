using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BeatSaberUnzipper
{
    class Program
    {
        private static readonly Dictionary<string, string> PlaylistURLs = new()
        {
            { "Alphabeat PixelTerror", "https://bsaber.com/PlaylistAPI/21-07-01_pixel-terror-pack_alphabeat.bplist" }
        };

        private static readonly Dictionary<string, int> PlaylistIds = new()
        {
            { "Favorites", 7903 },
            { "Aspirational", 7038 },
            { "Ajr NeoTheater", 171573 },
            { "Skillet", 85217 },
        };

        private static readonly Dictionary<string, int> UserPlaylistIds = new()
        {
            {"Teuflum", 68740},
            {"ZaneSaber", 4284220},
            {"TheCzar1994", 4285984},
            {"NixieKorten - Electroswing mapper ", 4286374},
            {"Halcyon12", 14808},
            {"Revelate", 2768},
        };
        
        private static readonly Dictionary<string,string> SpotifyPlaylistUrls = new()
        {
            {"All Likes", "https://open.spotify.com/playlist/5Zi1NzMK91ImLODGEWHNqS?si=bcde847d6766403f"},
            {"Robert's Epic Mix", "https://open.spotify.com/playlist/6QTGBDOzjxnkfQ9X6OmWIY?si=9d82829900c84462"},
            {"Robert's Pop Mix", "https://open.spotify.com/playlist/7vSZ2b4591qCZhi8bn2xkn?si=4a570e22013747fa"},
            {"Robert's Edm Mix", "https://open.spotify.com/playlist/5tY88XWKaUArUISNkY697j?si=3afb235dac594189"},
            {"Robert's Rock Mix", "https://open.spotify.com/playlist/43jMBPZVR5cdD7Cw1gZF8j?si=9a990d1647eb4cde"},
        };
        
        
        static async Task Main(string[] args)
        {
            Stopwatch overallTimer = Stopwatch.StartNew();
            MapRequestManager mapRequestManager = new MapRequestManager();
            FileManager.ClearPlaylistsCache();
            FileManager.ClearImagesCache();
            
            await GenerateBsPlaylistsFromSpotify(mapRequestManager);
            DownloadBeatSaverUserPlaylists(mapRequestManager);
            DownloadBeatSaverPlaylists(mapRequestManager);
            DownloadWebPlaylists(mapRequestManager);

            Stopwatch timer = Stopwatch.StartNew();

            while (timer.Elapsed.TotalSeconds < 10 && (mapRequestManager.mapDataLeftToDownload >0 || mapRequestManager.zipFilesLeftToDownload > 0))
            {
                Console.WriteLine($"Waiting for {mapRequestManager.mapDataLeftToDownload} map data requests and {mapRequestManager.zipFilesLeftToDownload} zip file requests");
                Thread.Sleep(750);
            }

            mapRequestManager.PreventDownloads = true;
            Console.WriteLine("Song and playlist download complete\n");
            
            Console.WriteLine("Generating output folders from caches");
            FileManager.ClearOutputDirectories();
            FileManager.ExportPlaylists();
            FileManager.ExportMaps(mapRequestManager.mapFoldersToOutput);
            
            Console.WriteLine($"Finished Map and Playlist export. Total program runtime: {overallTimer.Elapsed.TotalSeconds} seconds");
        }
        
        private static async Task GenerateBsPlaylistsFromSpotify(MapRequestManager mapRequestManager)
        {
            Console.WriteLine("Generating spotify playlists...\n");
            var playlists = await SpotifyTest.GenerateBeatSaberPlaylists(SpotifyPlaylistUrls.Values);
            Console.WriteLine("Spotify Playlist Generation Complete.\n\n");

            foreach (BPList bpList in playlists)
            {
                // Download map data and trigger async map file downloads
                foreach (Song song in bpList.songs) 
                    mapRequestManager.RequestMapDataAsync(song);
            }
        }
        
        private static void DownloadBeatSaverPlaylists(MapRequestManager mapRequestManager)
        {
            Console.WriteLine("Downloading BeatSaver playlists...");

            foreach (int playlistId in PlaylistIds.Values)
            {
                BPList bpList = mapRequestManager.RequestPlaylist(playlistId, out string playlistPath);
                RequestMaps(mapRequestManager, bpList, playlistPath);
            }
        }
        
        private static void DownloadBeatSaverUserPlaylists(MapRequestManager mapRequestManager)
        {
            Console.WriteLine("Downloading BeatSaver User playlists...");

            foreach (int userId in UserPlaylistIds.Values)
            {
                BPList bpList = mapRequestManager.RequestUserPlaylist(userId, out string playlistPath);
                RequestMaps(mapRequestManager, bpList, playlistPath);
            }
        }

        private static void DownloadWebPlaylists(MapRequestManager mapRequestManager)
        {
            Console.WriteLine("Downloading Web URL Playlists...");
            
            foreach (string url in PlaylistURLs.Values)
            {
                BPList bpList = mapRequestManager.RequestPlaylist(url, out string playlistPath);
                RequestMaps(mapRequestManager, bpList, playlistPath);
            }
        }

        private static void RequestMaps(MapRequestManager mapRequestManager, BPList bpList, string playlistPath)
        {
            if (bpList == null)
                return;

            Console.WriteLine("\n\nSaved User Playlist: " + playlistPath);
            Console.WriteLine("Requesting " + bpList.songs.Count + " maps...");

            // Download map data and trigger async map file downloads
            foreach (Song song in bpList.songs)
                mapRequestManager.RequestMapDataAsync(song);
        }
    }
}