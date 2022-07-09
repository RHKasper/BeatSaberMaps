using System;
using System.IO;

namespace BeatSaberUnzipper
{
	public class MapRequestManager
	{
		private readonly string _mapCachePath;

		public MapRequestManager(string mapCachePath)
		{
			_mapCachePath = mapCachePath;
		}

		/// <summary>
		/// Checks the mapCache to see if the desired map is already present. If it is, <see cref="OnRequestComplete"/>
		/// gets triggered immediately. Otherwise, trigger a download request for the given song and call <see cref="OnRequestComplete"/> when it completes
		/// </summary>
		public void RequestMapAsync(MapData mapData, Action<string> OnRequestComplete)
		{
			string mapDirectory = FileManager.GetMapDirectory(mapData);
			
			if(Directory.Exists(mapDirectory))
				OnRequestComplete.Invoke(mapDirectory);
			else
			{
				string zipFilePath = FileManager.GetZipFilePath(mapData);
				BeatSaverDownloader.DownloadZipFile(mapData.GetLatestVersion().downloadURL, zipFilePath, OnRequestComplete);
			}
		}
	}
}