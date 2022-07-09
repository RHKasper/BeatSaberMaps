using System;
using System.IO;

namespace BeatSaberUnzipper
{
	public class MapRequestManager
	{
		public int songsLeftToDownload { get; private set; }= 0;
		
		public MapRequestManager()
		{
			if (Directory.Exists(FileManager.MapCachePath) == false)
				Directory.CreateDirectory(FileManager.MapCachePath);
			if (Directory.Exists(FileManager.PlaylistsCachePath) == false)
				Directory.CreateDirectory(FileManager.PlaylistsCachePath);
		}

		/// <summary>
		/// Checks the mapCache to see if the desired map is already present. If it is, <see cref="OnRequestComplete"/>
		/// gets triggered immediately. Otherwise, trigger a download request for the given song and call <see cref="OnRequestComplete"/> when it completes.
		/// </summary>
		/// <param name="mapData">The map being requested</param>
		/// <param name="OnRequestComplete"> What should be done with the downloaded and unzipped directory</param>
		public void RequestMapAsync(MapData mapData, Action<string> OnRequestComplete)
		{
			string mapDirectory = FileManager.GetMapDirectory(mapData);
			
			if(Directory.Exists(mapDirectory))
				OnRequestComplete.Invoke(mapDirectory);
			else
			{
				string zipFilePath = FileManager.GetZipFilePath(mapData);
				Console.WriteLine($"Downloading {mapData.name}");
				songsLeftToDownload++;
				BeatSaverDownloader.DownloadZipFile(mapData.GetLatestVersion().downloadURL, zipFilePath, zipPath => OnDownloadFinished(zipPath, OnRequestComplete));
			}
		}

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

		private void OnDownloadFinished(string zipFilePath, Action<string> onRequestComplete)
		{
			FileManager.UnzipFile(zipFilePath, out string unzipDir);
			songsLeftToDownload--;
			onRequestComplete(unzipDir);
		}
	}
}