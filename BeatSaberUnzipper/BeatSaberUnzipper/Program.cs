using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace BeatSaberUnzipper
{
    class Program
    {
        private const string DownloadsFolder = "C:\\Users\\fires\\Downloads";
        private const string BeatsaberMapsFolder = "C:\\repos\\BeatSaberMaps\\Beat Saber_Data\\CustomLevels";
        private const string PlaylistsFolderPath = "C:\\repos\\BeatSaberMaps\\Playlists";
        private static readonly int[] PlaylistIds = {3210, 2363, 2364, 3209};
        private static int songsLeftToDownload = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            if(Directory.Exists(BeatsaberMapsFolder))
                Directory.Delete(BeatsaberMapsFolder, true);
            Directory.CreateDirectory(BeatsaberMapsFolder);
            
            if(Directory.Exists(PlaylistsFolderPath))
                Directory.Delete(PlaylistsFolderPath, true);
            Directory.CreateDirectory(PlaylistsFolderPath);

            songsLeftToDownload = 0;

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
                string path = Path.Combine(PlaylistsFolderPath, bpList.playlistTitle + ".bplist");
                File.WriteAllText(path, fileContents);
                Console.WriteLine("Saved Playlist: " + path);
                Console.WriteLine("Queueing " + bpList.songs.Count + " for download...");
                
                // Download songs
                foreach (Song song in bpList.songs)
                {
                    MapData mapData = BeatSaverDownloader.GetMapData(song);
                    if (mapData == null)
                    {
                        Console.WriteLine($"Downloading {song.songName} song data failed");
                        continue;
                    }

                    if (mapIdsDownloaded.Contains(mapData.id))
                        continue;
                    else
                        mapIdsDownloaded.Add(mapData.id);
                
                    string zipFileName = mapData.name + " - " + mapData.id + ".zip";
                    foreach (char c in Path.GetInvalidFileNameChars()) 
                        zipFileName = zipFileName.Replace(c, ' ');
                
                    string zipFilePath = Path.Combine(BeatsaberMapsFolder, zipFileName);
                    foreach (char c in Path.GetInvalidPathChars()) 
                        zipFileName = zipFileName.Replace(c+"", "");

                    songsLeftToDownload++;
                    BeatSaverDownloader.DownloadZipFile(mapData.GetLatestVersion().downloadURL, zipFilePath, OnSongFinishedDownloading);
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
            UnzipFile(path);
            songsLeftToDownload--;
        }
        
        private static void UnzipFile(string zipFilePath, bool deleteZipFile = true)
        {
            string targetDir = Path.Combine(BeatsaberMapsFolder, Path.GetFileNameWithoutExtension(zipFilePath));

            try
            {
                if(Directory.Exists(targetDir))
                {
                    Directory.Delete(targetDir, true);
                    //Console.WriteLine("Cleared " + targetDir);
                }
                        
                ZipFile.ExtractToDirectory(zipFilePath, targetDir);
                //Console.WriteLine("Extracted " + zipFilePath + " to " + targetDir);
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to extract " + zipFilePath);
                Console.WriteLine(e);
            }
            if(deleteZipFile)
                File.Delete(zipFilePath);
        }
    }
}