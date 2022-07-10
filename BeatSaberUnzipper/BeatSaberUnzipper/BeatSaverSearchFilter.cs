using System;
using System.Linq;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using Swan;

namespace BeatSaberUnzipper
{
	public static class BeatSaverSearchFilter
	{
		/// <summary>
		/// https://api.beatsaver.com/docs/index.html?url=./swagger.json#/OrderedMap%20%7B%20%22name%22%3A%20%22Search%22%20%7D/get_search_text__page_
		/// </summary>
		/// <returns>Hash of the chosen map, or empty if no viable map is found</returns>
		public static Doc SearchForTrack(FullTrack fullTrack)
		{
			string title = fullTrack.Name;
			string firstArtist = fullTrack.Artists.First().Name;
            
			bool allowChroma = false, allowCinema = false, allowNoodle = false, requireCurated = false;
            
			string uri = "https://api.beatsaver.com/search/text/0?";

			// if (!allowChroma)
			//     uri += "chroma=false&";
			// if (!allowCinema)
			//     uri += "cinema=false&";
			// if (!allowNoodle)
			//     uri += "noodle=false&";

			uri += $"q={title} {firstArtist}";
			uri += "&sortOrder=Relevance";

			try
			{
				string fileContents = BeatSaverDownloader.Get(uri);
				SearchQuery searchQuery = JsonConvert.DeserializeObject<SearchQuery>(fileContents);
				var selectedMapDoc = GetBestMap(fullTrack, searchQuery);
				return selectedMapDoc;
			}
			catch
			{
				return null;
			}
		}
		
		public static Doc GetBestMap(FullTrack fullTrack, SearchQuery searchQuery)
		{
			Console.WriteLine($"\n\nSearch results for: {fullTrack.Name} ({string.Join(", ", fullTrack.Artists.Select(a=>a.Name))})\n");

			foreach (Doc doc in searchQuery.docs)
			{
				Console.WriteLine($"{doc.name} =====  Score: {doc.stats.score}");
			}
			
			return searchQuery.docs.First();
		}
	}
}