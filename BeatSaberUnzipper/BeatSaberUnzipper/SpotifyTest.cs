using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace BeatSaberUnzipper
{
	class SpotifyTest
	{ 
		public static async Task Test()
		{
			var spotify = new SpotifyClient("BQBHec-CCCspN_LxH53wNOu9KVPUZ2uIYb06Vu0rkyNtw1vPG4G-jqRvaPDJ5ipSoTcVaial0DCREFi5mHWDnSd7Dxi7jOq8RpQtAHdLD11CnYjRv1BjUCL9wMZvYsbD08HTKm1TAQ61pbCG-HjhRNL7ZeJ2HsdHoWFoDa-UiwitFME");
			FullTrack testTrack = await spotify.Tracks.Get("0q7oMII7kWTj1ZSX6GT6LU");
			Console.WriteLine(testTrack.Name + "\n");
			
			PrivateUser user = await spotify.UserProfile.Current();
			Paging<SimplePlaylist> playlistPage = await spotify.Playlists.GetUsers(user.Id);
			IList<SimplePlaylist> allPlaylists = await spotify.PaginateAll(playlistPage);
			
			foreach (var playlist in allPlaylists)
			{
				Paging<PlaylistTrack<IPlayableItem>> trackPage = await spotify.Playlists.GetItems(playlist.Id);
				IList<PlaylistTrack<IPlayableItem>> allTracks = await spotify.PaginateAll(trackPage);
				
				string message = $"{playlist.Name} ({allTracks.Count} Tracks)\n";
				
				foreach (PlaylistTrack<IPlayableItem> track in allTracks)
				{
					if (track.Track is FullTrack fullTrack)
						message += fullTrack.Name + "\n";
				}
				
				Console.WriteLine(message);
			}
		}
	}
}