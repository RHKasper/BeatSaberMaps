using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
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
				try
				{
					// Get Playlist from Spotify API
					string playlistID = GetPlaylistIdFromUrl(playlistUrl);
					FullPlaylist playlist = await spotify.Playlists.Get(playlistID);
					Console.WriteLine($"Playlist: {playlist.Name}");
					
					// Get Playlist image from URL
					string playlistImagePath = await DownloadPlaylistImage(playlist);

					continue;
					
					// Generate Beatsaber BPList 
					BPList bpList = new BPList
					{
						playlistTitle = playlist.Name,
						playlistAuthor = "Spotify",
						playlistDescription = playlist.Description,
						songs = new List<Song>(),
					};

					// we need the first page
					Paging<PlaylistTrack<IPlayableItem>> page = await spotify.Playlists.GetItems(playlist.Id);
					int requestedTracks = 0;
					int foundTracks = 0;

					await foreach (var track in spotify.Paginate(page))
					{
						if (track.Track is FullTrack fullTrack)
						{
							requestedTracks++;
							SearchConfig searchConfig = new SearchConfig()
							{
								FullTrack = fullTrack,
								AcceptableDifficulties = new[] { Diff.Normal, Diff.Hard, Diff.Expert },
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
								foundTracks++;
							}
						}
					}

					bpList.RemoveDuplicates();

					Console.WriteLine($"\n{playlist.Name}: Found {foundTracks} out of {requestedTracks} ({((float)foundTracks/requestedTracks)*100}%)\n");

					string json = JsonConvert.SerializeObject(bpList);
					string playlistPath = Path.Combine(FileManager.PlaylistsCachePath, playlist.Name + ".bplist");
					await File.WriteAllTextAsync(playlistPath, json);
					generatedPlaylists.Add(bpList);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
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

		private static async Task<string> DownloadPlaylistImage(FullPlaylist playlist)
		{
			// Get image URL
			string imageUrl = playlist.Images.First().Url;

			string dir = FileManager.ImagesCachePath;
			string filename = playlist.Name + " Cover";
			
			// Request Image		
			await BeatSaverDownloader.DownloadImageAsync(dir, filename, new Uri(imageUrl));
			return Path.Combine(dir, filename);
		}
	}
}