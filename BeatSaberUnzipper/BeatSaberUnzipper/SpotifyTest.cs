using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Swan;

namespace BeatSaberUnzipper
{
	class SpotifyTest
	{ 
		public static async Task Test()
		{
			var config = SpotifyClientConfig.CreateDefault();

			var request = new ClientCredentialsRequest("1d303e20e9c8498f95c8d39f244b143d", "45648a39b896462594915ed2d7d48714");
			var response = await new OAuthClient(config).RequestToken(request);
			
			var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
			
			FullTrack testTrack = await spotify.Tracks.Get("0q7oMII7kWTj1ZSX6GT6LU");
			Console.WriteLine(testTrack.Name + "\n");
			
			Paging<SimplePlaylist> playlistPage = await spotify.Playlists.GetUsers("1275790494");
			IList<SimplePlaylist> allPlaylists = await spotify.PaginateAll(playlistPage);
			
			foreach (var playlist in allPlaylists)
			{
				Console.WriteLine("Playlist: " + playlist.Name);
				
				Paging<PlaylistTrack<IPlayableItem>> trackPage = await spotify.Playlists.GetItems(playlist.Id);
				IList<PlaylistTrack<IPlayableItem>> allTracks = await spotify.PaginateAll(trackPage);
				
				string message = $"{playlist.Name} ({allTracks.Count} Tracks)\n";
				
				foreach (PlaylistTrack<IPlayableItem> track in allTracks)
				{
					if (track.Track is FullTrack fullTrack)
					{
						message += fullTrack.Name + "\n";
						string desiredMapHash = BeatSaverDownloader.SearchForSong(fullTrack.Name, fullTrack.Artists.First().Name);
						if(desiredMapHash == default)
							Console.WriteLine($"Search for {fullTrack.Name} by {fullTrack.Artists.Humanize()} failed.");
					}
				}
				
				Console.WriteLine(message);
			}
		}
	}
}