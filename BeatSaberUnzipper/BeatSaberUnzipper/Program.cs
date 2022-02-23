﻿using System;
using System.IO;
using System.IO.Compression;

namespace BeatSaberUnzipper
{
    class Program
    {
        private const string DownloadsFolder = "C:\\Users\\fires\\Downloads";
        private const string BeatsaberMapsFolder = "C:\\repos\\BeatSaberMaps\\Beat Saber_Data\\CustomLevels";
        private static readonly int[] PlaylistIds = {1918, 2363, 2364};

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            foreach (int playlistId in PlaylistIds)
            {
                Console.WriteLine(SongDownloader.GetPlaylist(playlistId));
            }
            
            Console.WriteLine("Song download complete");
            Console.ReadLine();




            var files = Directory.GetFiles(DownloadsFolder, "* (*).zip");

            foreach (string s in files)
            {
                Console.WriteLine(s);
            }
            
            Console.WriteLine("\nDo you want to unzip these files to " + BeatsaberMapsFolder + "? (Y/N)");
            var input = Console.ReadLine();
            
            if (input != null && input.ToLower().StartsWith('y'))
            {
                Console.WriteLine("Okay! Now unzipping files to " + BeatsaberMapsFolder);
                foreach (string file in files)
                {
                    string targetDir = Path.Combine(BeatsaberMapsFolder, Path.GetFileNameWithoutExtension(file));

                    try
                    {
                        if(Directory.Exists(targetDir))
                        {
                            Directory.Delete(targetDir, true);
                            Console.WriteLine("Cleared " + targetDir);
                        }
                        
                        ZipFile.ExtractToDirectory(file, targetDir);
                        Console.WriteLine("Extracted " + file + " to " + targetDir);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Failed to extract " + file);
                        Console.WriteLine(e);
                    }
                }
                Console.WriteLine("Finished.");
            }
        }
    }
}