using System;
using System.Collections.Generic;
using System.IO;

namespace BeatSaberUnzipper
{
	public class MapRequestManager
	{
		public int songsLeftToDownload { get; private set; }
		private HashSet<string> mapsRequested = new();

		public MapRequestManager()
		{
			if (Directory.Exists(FileManager.MapCachePath) == false)
				Directory.CreateDirectory(FileManager.MapCachePath);
			if (Directory.Exists(FileManager.PlaylistsCachePath) == false)
				Directory.CreateDirectory(FileManager.PlaylistsCachePath);
		}

		/// <summary>
		/// Checks the mapCache to see if the desired map is already present. If it is, do nothing. Otherwise, trigger
		/// a download request for the given song and call <see cref="OnMapDownloadFinished"/> when it completes.
		/// </summary>
		/// <param name="mapData">The map being requested</param>
		public void RequestMapAsync(MapData mapData)
		{
			string mapDirectory = FileManager.GetMapDirectory(mapData);
			string zipFilePath = FileManager.GetZipFilePath(mapData);

			// Don't bother downloading if we already have this map, or have already requested it.
			if (mapsRequested.Contains(mapData.id))
			{
				Console.WriteLine($"Already received a map request for : {mapData.name}");
				return;
			}
			
			if (Directory.Exists(mapDirectory))
			{
				Console.WriteLine($"Discovered existing map folder: {mapData.name}");
				return;
			}
			
			// Clean up old .zip files that likely have issues
			if (File.Exists(zipFilePath))
			{
				Console.WriteLine($"Deleting .zip file from a previous run that got interrupted or failed to unzip: {mapData.name}");
				File.Delete(zipFilePath);
			}
			
			// Track new request and trigger .zip file download
			mapsRequested.Add(mapData.id);
			Console.WriteLine($"Downloading {mapData.name}");
			songsLeftToDownload++;
			BeatSaverDownloader.DownloadZipFile(mapData.GetLatestVersion().downloadURL, zipFilePath, OnMapDownloadFinished);
		}

		/// <summary>
		/// Download playlist file synchronously
		/// </summary>
		/// <returns></returns>
		public BPList RequestPlaylist(int playlistId, out string filePath)
		{
			//download playlist file
			BPList bpList = BeatSaverDownloader.GetBpList(playlistId, out string fileContents);
			if (bpList == null)
			{
				Console.WriteLine($"playlist {playlistId} could not be downloaded or read");
				filePath = default;
				return null;
			}
            
			// save playlist file contents
			filePath = FileManager.GetPlaylistFilePath(bpList);
			File.WriteAllText(filePath, fileContents);
			return bpList;
		}

		private void OnMapDownloadFinished(string zipFilePath)
		{
			FileManager.UnzipFile(zipFilePath, out string unzipDir);
			songsLeftToDownload--;
		}
	}
}