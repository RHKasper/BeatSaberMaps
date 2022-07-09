﻿using System;
using System.IO;
using System.IO.Compression;

namespace BeatSaberUnzipper
{
	public static class FileManager
	{
		public const string MapCachePath = "C:\\repos\\BeatSaberMaps\\Cache\\MapCache";
		public const string BeatsaberMapsFolder = "C:\\repos\\BeatSaberMaps\\Beat Saber_Data\\CustomLevels";
		public const string PlaylistsFolderPath = "C:\\repos\\BeatSaberMaps\\Playlists";
		
		public static void UnzipFile(string zipFilePath, bool deleteZipFile = true)
		{
			var targetDir = GetMapDirectory(zipFilePath);

			try
			{
				if(Directory.Exists(targetDir)) 
					Directory.Delete(targetDir, true);
				ZipFile.ExtractToDirectory(zipFilePath, targetDir);
			}
			catch(Exception e)
			{
				Console.WriteLine("Failed to extract " + zipFilePath);
				Console.WriteLine(e);
			}
			if(deleteZipFile)
				File.Delete(zipFilePath);
		}


		public static string GetMapDirectory(MapData mapData) => GetMapDirectory(GetZipFilePath(GetZipFileName(mapData)));

		public static string GetMapDirectory(string zipFilePath)
		{
			string targetDir = Path.Combine(BeatsaberMapsFolder, Path.GetFileNameWithoutExtension(zipFilePath));
			return targetDir;
		}

		public static string GetZipFilePath(string zipFileName)
		{
			string zipFilePath = Path.Combine(BeatsaberMapsFolder, zipFileName);
			foreach (char c in Path.GetInvalidPathChars())
				zipFileName = zipFileName.Replace(c + "", "");
			return zipFilePath;
		}
		
		public static string GetZipFilePath(MapData mapData) => GetZipFilePath(GetZipFileName(mapData));

		public static string GetZipFileName(MapData mapData)
		{
			string zipFileName = mapData.name + " - " + mapData.id + ".zip";
			foreach (char c in Path.GetInvalidFileNameChars())
				zipFileName = zipFileName.Replace(c, ' ');
			return zipFileName;
		}
		
		public static void ClearOutputDirectories()
		{
			if (Directory.Exists(BeatsaberMapsFolder))
				Directory.Delete(BeatsaberMapsFolder, true);
			Directory.CreateDirectory(BeatsaberMapsFolder);

			if (Directory.Exists(PlaylistsFolderPath))
				Directory.Delete(PlaylistsFolderPath, true);
			Directory.CreateDirectory(PlaylistsFolderPath);
		}
	}
}