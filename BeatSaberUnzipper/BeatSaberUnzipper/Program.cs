using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BeatSaberUnzipper
{
    class Program
    {
        private static readonly int[] PlaylistIds = {3210, 2363, 2364, 3209, 7038};
        private static readonly string[] SpotifyPlaylistUrls =
        {
            SpotifyPlaylists.AllLikes,
            SpotifyPlaylists.LongtermFavorites, 
            SpotifyPlaylists.JeanPossibleBeatsabers,
            SpotifyPlaylists.EdmForJean,
            SpotifyPlaylists.ThisIsZedd,
            SpotifyPlaylists.BeatsaberBarbara,
            SpotifyPlaylists.ThisIsIllenium,
        };
        
        
        static async Task Main(string[] args)
        {
            Stopwatch overallTimer = Stopwatch.StartNew();
            MapRequestManager mapRequestManager = new MapRequestManager();
            FileManager.ClearPlaylistsCache();
            
            await GenerateBsPlaylistsFromSpotify(mapRequestManager);
            //DownloadBeatSaverPlaylists(mapRequestManager);

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