namespace BeatSaberUnzipper
{
    public static class JSONClassExtensions
    {
        public static Version GetLatestVersion(this MapData map)
        {
            Version latest = null;
            foreach (Version version in map.versions)
            {
                if (latest == null || version.createdAt > latest.createdAt)
                    latest = version;
            }

            return latest;
        }
    }
}