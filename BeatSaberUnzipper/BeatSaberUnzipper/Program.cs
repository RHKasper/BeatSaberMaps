using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeatSaberUnzipper
{
    class Program
    {
        private static readonly int[] PlaylistIds =
        {
            BeatSaverPlaylists.Favorites,
            BeatSaverPlaylists.Aspirational,
            BeatSaverPlaylists.SolidMaps,
            BeatSaverPlaylists.TryOuts,
            BeatSaverPlaylists.LadyLove,
        };
        private static readonly string[] SpotifyPlaylistUrls =
        {
            SpotifyPlaylists.ProgressiveHouseMix,
            SpotifyPlaylists.MelodicDubstepMix,
            SpotifyPlaylists.FutureBassMix,
            SpotifyPlaylists.DancePopMix,
            SpotifyPlaylists.PopMix,
            SpotifyPlaylists.HypeEdmMix,
            SpotifyPlaylists.PopEdmMix,
            SpotifyPlaylists.AllLikes,
            SpotifyPlaylists.RunningEdmMix,
            SpotifyPlaylists.ChillEdmMix,
            SpotifyPlaylists.EdmBangers,
            SpotifyPlaylists.EpicMix
        };
        
        
        static async Task Main(string[] args)
        {
            Stopwatch overallTimer = Stopwatch.StartNew();
            MapRequestManager mapRequestManager = new MapRequestManager();
            FileManager.ClearPlaylistsCache();
            FileManager.ClearImagesCache();
            
            await GenerateBsPlaylistsFromSpotify(mapRequestManager);
            DownloadBeatSaverPlaylists(mapRequestManager);

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
            var playlists = await SpotifyTest.GenerateBeatSaberPlaylists(SpotifyPlaylistUrls);
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
            // Download beatsaver playlists
            foreach (int playlistId in PlaylistIds)
            {
                // Download Playlist
                BPList bpList = mapRequestManager.RequestPlaylist(playlistId, out string playlistPath);
                if (bpList == null)
                    continue;
                
                Console.WriteLine("\n\nSaved Playlist: " + playlistPath);
                Console.WriteLine("Requesting " + bpList.songs.Count + " maps...");

                // Download map data and trigger async map file downloads
                foreach (Song song in bpList.songs)
                    mapRequestManager.RequestMapDataAsync(song);
            }
        }
    }
}