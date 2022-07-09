using System;
using System.IO;
using System.IO.Compression;

namespace BeatSaberUnzipper
{
	public static class FileManager
	{
		public const string DownloadsFolder = "C:\\Users\\fires\\Downloads";
		public const string BeatsaberMapsFolder = "C:\\repos\\BeatSaberMaps\\Beat Saber_Data\\CustomLevels";
		public const string PlaylistsFolderPath = "C:\\repos\\BeatSaberMaps\\Playlists";
		
		public static void UnzipFile(string zipFilePath, bool deleteZipFile = true)
		{
			string targetDir = Path.Combine(FileManager.BeatsaberMapsFolder, Path.GetFileNameWithoutExtension(zipFilePath));

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
		
		public static string GetZipFilePath(string zipFileName)
		{
			string zipFilePath = Path.Combine(FileManager.BeatsaberMapsFolder, zipFileName);
			foreach (char c in Path.GetInvalidPathChars())
				zipFileName = zipFileName.Replace(c + "", "");
			return zipFilePath;
		}

		public static string GetZipFileName(MapData mapData)
		{
			string zipFileName = mapData.name + " - " + mapData.id + ".zip";
			foreach (char c in Path.GetInvalidFileNameChars())
				zipFileName = zipFileName.Replace(c, ' ');
			return zipFileName;
		}
		
		public static void ClearOutputDirectories()
		{
			if (Directory.Exists(FileManager.BeatsaberMapsFolder))
				Directory.Delete(FileManager.BeatsaberMapsFolder, true);
			Directory.CreateDirectory(FileManager.BeatsaberMapsFolder);

			if (Directory.Exists(FileManager.PlaylistsFolderPath))
				Directory.Delete(FileManager.PlaylistsFolderPath, true);
			Directory.CreateDirectory(FileManager.PlaylistsFolderPath);
		}
	}
}