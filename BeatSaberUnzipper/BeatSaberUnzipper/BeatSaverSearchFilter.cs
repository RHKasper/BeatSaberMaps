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
			Console.WriteLine($"\n\nSearch results for: {fullTrack.Name} ({string.Join(", ", fullTrack.Artists.Select(a=>a.Name))})");

			for (int i = searchQuery.docs.Count - 1; i >= 0; i--)
			{
				Doc doc = searchQuery.docs[i];
				Version version = doc.GetLatestVersion();
				
				// Remove maps that don't have the desired difficulties
				if (!version.diffs.Any(diff => searchConfig.AcceptableDifficulties.Any(a => a == diff.difficulty)))
					searchQuery.docs.Remove(doc);
				
				// Remove maps with low ratings and many downvotes
				else if (doc.stats.score <= searchConfig.MinRatingForSmallMaps || (doc.stats.score <= searchConfig.MinRating && doc.stats.downvotes > searchConfig.MaxDownvotes))
					searchQuery.docs.Remove(doc);
				
				// Remove maps with tons of parity errors in Hard+ difficulties
				else if (version.diffs.Where(d=> d.difficulty != Diff.Normal && d.difficulty != Diff.Easy).Any(d => d.paritySummary.errors > searchConfig.MaxParityErrors))
					searchQuery.docs.Remove(doc);
				
				// Remove maps that don't contain the song name or the artist name
				else if (!ContainsSongOrArtistName(doc, fullTrack))
					searchQuery.docs.Remove(doc);
			}

			searchQuery.docs.Sort(((doc, doc1) => Math.Sign(doc1.stats.score - doc.stats.score)));
			
			foreach (Doc doc in searchQuery.docs)
			{
				Version version = doc.GetLatestVersion();
				Console.WriteLine($"{doc.name} =====  Score: {doc.stats.score}; Downvotes: {doc.stats.downvotes}; DiffStats: [{string.Join(',', version.diffs.Select(d=>d.difficulty))}]");
			}
			
			return searchQuery.docs.First();
		}

		private static bool ContainsSongOrArtistName(Doc doc, FullTrack fullTrack)
		{
			string[] trackNameWords = FilterToJustAlphaNumerics(fullTrack.Name).Split(' ');
			string[] artistNameWords = FilterToJustAlphaNumerics(fullTrack.Artists.First().Name).Split(' ');

			int trackNameWordsFound = FindWordsInMapName(trackNameWords, doc);
			Console.WriteLine($"Found {trackNameWordsFound} out of {trackNameWords.Length} track name words");
			
			int artistNameWordsFound = FindWordsInMapName(artistNameWords, doc);
			Console.WriteLine($"Found {artistNameWordsFound} out of {artistNameWords.Length} artist name words");

			bool containsTrackName = trackNameWordsFound > .6f * trackNameWords.Length;
			bool containsArtistName = artistNameWordsFound > .6f * artistNameWords.Length;

			return containsArtistName || containsTrackName;
		}

		private static string FilterToJustAlphaNumerics(string str)
		{
			string result = "";
			foreach (char c in str)
			{
				if (char.IsLetterOrDigit(c))
					result += c;
				else
					result += ' ';
			}

			return result;
		}

		private static int FindWordsInMapName(string[] words, Doc doc)
		{
			int wordsFound = 0;
			foreach (string word in words)
			{
				if (doc.name.Contains(word, StringComparison.CurrentCultureIgnoreCase))
				{
					wordsFound++;
					//Console.WriteLine($"Found \"{word}\" in \"{doc.name}\"");
				}
			}

			return wordsFound;
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