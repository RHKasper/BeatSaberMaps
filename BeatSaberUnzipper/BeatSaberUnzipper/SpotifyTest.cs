using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using Swan;

namespace BeatSaberUnzipper
{
	public static class SpotifyTest
	{
		private const string NullLabel = "NULL";
		public static async Task<List<BPList>> GenerateBeatSaberPlaylists(IEnumerable<string> spotifyPlaylistUrls)
		{
			SpotifyClient spotify = await CreateClient();
			List<BPList> generatedPlaylists = new List<BPList>();
			
			foreach (string playlistUrl in spotifyPlaylistUrls)
			{
				string playlistID = GetPlaylistIdFromUrl(playlistUrl);
				FullPlaylist playlist = await spotify.Playlists.Get(playlistID);
				
				Console.WriteLine( $"Playlist: {playlist.Name}");
				BPList bpList = new BPList
				{
					playlistTitle = playlist.Name,
					playlistAuthor = "Spotify",
					playlistDescription = playlist.Description,
					songs = new List<Song>(),
				};
				
				// we need the first page
				Paging<PlaylistTrack<IPlayableItem>> page = await spotify.Playlists.GetItems(playlist.Id);

				await foreach(var track in spotify.Paginate(page))
				{
					if (track.Track is FullTrack fullTrack)
					{
						SearchConfig searchConfig = new SearchConfig()
						{
							FullTrack = fullTrack,
							AcceptableDifficulties = new []{Diff.Normal, Diff.Hard, Diff.Expert},
						};
							
						Doc desiredMap = BeatSaverSearchFilter.SearchForTrack(searchConfig);
						string trackTitle = $"{fullTrack.Name} (by {fullTrack.Artists[0].Name})";
						string mapTitle = $"{(desiredMap == default ? NullLabel : desiredMap.name)}";
						Console.WriteLine($"{trackTitle} ========== {mapTitle}");

						if (desiredMap != null)
						{
							Version version = desiredMap.versions.First();
							bpList.songs.Add(new Song
							{
								hash = version.hash,
								key = version.key,
								songName = desiredMap.name,
							});
						}
					}
				}
				
				Console.WriteLine();

				string json = JsonConvert.SerializeObject(bpList);
				string playlistPath = Path.Combine(FileManager.PlaylistsCachePath, playlist.Name + ".bplist");
				await File.WriteAllTextAsync(playlistPath, json);
				generatedPlaylists.Add(bpList);
			}

			return generatedPlaylists;
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