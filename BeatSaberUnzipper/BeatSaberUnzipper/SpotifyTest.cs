using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace BeatSaberUnzipper
{
	class SpotifyTest
	{ 
		public static async Task Test()
		{
			var spotify = new SpotifyClient("BQBHec-CCCspN_LxH53wNOu9KVPUZ2uIYb06Vu0rkyNtw1vPG4G-jqRvaPDJ5ipSoTcVaial0DCREFi5mHWDnSd7Dxi7jOq8RpQtAHdLD11CnYjRv1BjUCL9wMZvYsbD08HTKm1TAQ61pbCG-HjhRNL7ZeJ2HsdHoWFoDa-UiwitFME");
			FullTrack track = await spotify.Tracks.Get("0q7oMII7kWTj1ZSX6GT6LU");
			Console.WriteLine(track.Name);
			
			PrivateUser user = await spotify.UserProfile.Current();

			Paging<SimplePlaylist> playlists = await spotify.Playlists.GetUsers(user.Id);
			foreach (var playlist in playlists.Items)
			{
				Console.WriteLine(playlist.Name);
			}
		}
	}
}