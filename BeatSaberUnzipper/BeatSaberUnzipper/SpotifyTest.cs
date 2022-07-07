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
			var spotify = new SpotifyClient("BQCakW_XZiFZ_sDdtKmUUEYVmBV53q792cw9Rznbu7KooPooYOoHsvVj-eBwX3d4qYEGNLd8FAq6xG701T2Z77SP0S_hqfj_7dCchcLjj9HXNuUZtgLPtgOhbSiRJtj_vzRjZR7y_Lc3J_Gc01x8rQYq5dloKRMNaxF2hg2HJ5D4pI_nVozcSxoDlNsC");
			FullTrack testTrack = await spotify.Tracks.Get("0q7oMII7kWTj1ZSX6GT6LU");
			Console.WriteLine(testTrack.Name + "\n");
			
			PrivateUser user = await spotify.UserProfile.Current();
			Paging<SimplePlaylist> playlistPage = await spotify.Playlists.GetUsers(user.Id);
			IList<SimplePlaylist> allPlaylists = await spotify.PaginateAll(playlistPage);
			
			foreach (var playlist in allPlaylists)
			{
				Paging<PlaylistTrack<IPlayableItem>> trackPage = await spotify.Playlists.GetItems(playlist.Id);
				IList<PlaylistTrack<IPlayableItem>> allTracks = await spotify.PaginateAll(trackPage);s
				
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
				
				//Console.WriteLine(message);
			}
		}
	}
}