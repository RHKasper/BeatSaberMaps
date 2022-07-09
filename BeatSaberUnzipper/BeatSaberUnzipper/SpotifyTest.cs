using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Swan;

namespace BeatSaberUnzipper
{
	public static class SpotifyTest
	{
		private const string NullLabel = "NULL";
		public static async Task GenerateBeatSaberPlaylists(IEnumerable<string> spotifyPlaylistUrls)
		{
			SpotifyClient spotify = await CreateClient();
			
			foreach (string playlistUrl in spotifyPlaylistUrls)
			{
				string playlistID = GetPlaylistIdFromUrl(playlistUrl);
				FullPlaylist playlist = await spotify.Playlists.Get(playlistID);
				
				Console.WriteLine( $"{playlist.Name}");
				
				Paging<PlaylistTrack<IPlayableItem>> trackPage = await spotify.Playlists.GetItems(playlist.Id);
				// TODO: don't paginate all at once
				IList<PlaylistTrack<IPlayableItem>> allTracks = await spotify.PaginateAll(trackPage);
				
				Console.WriteLine($"{allTracks.Count} tracks");

				foreach (PlaylistTrack<IPlayableItem> track in allTracks)
				{
					if (track.Track is FullTrack fullTrack)
					{
						Doc desiredMap = BeatSaverDownloader.SearchForTrack(fullTrack);
						string trackTitle = $"{fullTrack.Name} (by {fullTrack.Artists[0].Name})";
						string mapTitle = $"{(desiredMap == default ? NullLabel : desiredMap.name)}";
						Console.WriteLine($"{trackTitle} ========== {mapTitle}");
					}
				}

				Console.WriteLine();
			}
		}

		private static async Task<SpotifyClient> CreateClient()
		{
			var config = SpotifyClientConfig.CreateDefault();
			var request = new ClientCredentialsRequest("1d303e20e9c8498f95c8d39f244b143d", "45648a39b896462594915ed2d7d48714");
			var response = await new OAuthClient(config).RequestToken(request);
			var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
			return spotify;
		}

		private static string GetPlaylistIdFromUrl(string url) => Path.GetFileName(url).Split('?')[0];
	}
}