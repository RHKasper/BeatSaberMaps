using System;
using System.Linq;
using BeatSaberUnzipper.MapEvaluation;
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
		public static Doc SearchForTrack(SearchConfig searchConfig)
		{
			string title = searchConfig.FullTrack.Name;
			string firstArtist = searchConfig.FullTrack.Artists.First().Name;
            
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
				var selectedMapDoc = GetBestMap(searchConfig.FullTrack, searchQuery, searchConfig);
				return selectedMapDoc;
			}
			catch
			{
				return null;
			}
		}
		
		public static Doc GetBestMap(FullTrack fullTrack, SearchQuery searchQuery, SearchConfig searchConfig)
		{
			for (int i = searchQuery.docs.Count - 1; i >= 0; i--)
			{
				Doc doc = searchQuery.docs[i];
				Version version = doc.GetLatestVersion();
				
				if (!version.HasAnyOfRequestedDifficulties(searchConfig.AcceptableDifficulties))
					searchQuery.docs.Remove(doc);
				else if (doc.IsPoorlyRatedBigMap() || doc.IsPoorlyRatedSmallMap())
					searchQuery.docs.Remove(doc);
				else if (version.HasTooManyParityErrors())
					searchQuery.docs.Remove(doc);
				else if (!doc.ContainsArtistName(fullTrack))
					searchQuery.docs.Remove(doc);
			}

			return searchQuery.docs.Most(doc => doc.ScoreOverall(fullTrack));
		}
	}

	public class SearchConfig
	{
		public FullTrack FullTrack;
		public string[] AcceptableDifficulties;
		
		/// <summary>
		/// a track is excluded if it has less than <see cref="MinRating"/> AND fewer than <see cref="MaxDownvotes"/> downvotes
		/// </summary>
		public float MinRating = .8f; //0-1

		public float MinRatingForSmallMaps = .7f;
		public int MaxDownvotes = 5;

		public int MaxParityErrors = 20;
	}
}