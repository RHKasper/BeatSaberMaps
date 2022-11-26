using System.Linq;

namespace BeatSaberUnzipper
{
	public static class MapFilters
	{
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


	}
}