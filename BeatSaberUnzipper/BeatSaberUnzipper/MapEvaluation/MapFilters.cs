using System.Linq;
using SpotifyAPI.Web;

namespace BeatSaberUnzipper.MapEvaluation
{
	public static class MapFilters
	{
		public static bool FailsAnyQualityFilter(this Doc d)
		{
			Version v = d.GetLatestVersion();
			return d.IsPoorlyRated() || v.HasTooManyParityErrors() || v.NpsIsTooHigh();
		}
		
		public static bool HasTooManyParityErrors(this Version v, int maxParityErrors = 5)
		{
			foreach (Diff diff in v.diffs)
			{
				if(diff.difficulty != Diff.Normal && diff.difficulty != Diff.Easy)
					if (diff.paritySummary.errors > maxParityErrors)
						return true;
			}
			return false;
		}

		public static bool NpsIsTooHigh(this Version version, float maxNps = 6.5f)
		{
			return version.diffs.All(d => d.nps > maxNps);
		}

		public static bool IsPoorlyRated(this Doc doc)
		{
			bool hasManyRatings = doc.stats.upvotes + doc.stats.downvotes > 50;

			if (hasManyRatings)
				return doc.stats.score < .8f;
			else
				return doc.stats.score < .7f;
		}
		
		public static bool IsPoorlyRatedBigMap(this Doc doc, float minRating = .8f, int minDownvotes = 5)
		{
			
			return doc.stats.downvotes > minDownvotes && doc.stats.score < minRating;
		}

		public static bool IsPoorlyRatedSmallMap(this Doc doc, float minRating = .7f, int maxDownvotes = 5)
		{
			return doc.stats.downvotes <= maxDownvotes && doc.stats.score < minRating;
		}

		public static bool HasAnyOfRequestedDifficulties(this Version v, string[] desiredDiffs)
		{
			foreach (string desiredDiff in desiredDiffs)
			{
				if (v.diffs.Any(d=> d.difficulty == desiredDiff))
					return true;
			}

			return false;
		}

		public static bool ContainsArtistName(this Doc doc, FullTrack fullTrack)
		{
			string[] artistNameWords = MapEvalUtils.FilterToJustAlphaNumerics(fullTrack.Artists.First().Name).Split(' ');
			int artistNameWordsFound = doc.FindWordsInMapName(artistNameWords);
			bool containsArtistName = artistNameWordsFound > .7f * artistNameWords.Length;

			return containsArtistName;
		}
	}
}