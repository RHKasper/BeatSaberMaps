using System;
using System.Collections.Generic;

namespace BeatSaberUnzipper
{
   // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Owner
    {
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string type { get; set; }
    }

    public class Playlist
    {
        public int playlistId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string playlistImage { get; set; }
        public bool @public { get; set; }
        public Owner owner { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime songsChangedAt { get; set; }
    }

    public class Uploader
    {
        public int id { get; set; }
        public string name { get; set; }
        public string hash { get; set; }
        public string avatar { get; set; }
        public string type { get; set; }
    }

    public class Metadata
    {
        public double bpm { get; set; }
        public int duration { get; set; }
        public string songName { get; set; }
        public string songSubName { get; set; }
        public string songAuthorName { get; set; }
        public string levelAuthorName { get; set; }
    }

    public class Stats
    {
        public int plays { get; set; }
        public int downloads { get; set; }
        public int upvotes { get; set; }
        public int downvotes { get; set; }
        public double score { get; set; }
    }

    public class ParitySummary
    {
        public int errors { get; set; }
        public int warns { get; set; }
        public int resets { get; set; }
    }

    public class Diff
    {
        public double njs { get; set; }
        public double offset { get; set; }
        public int notes { get; set; }
        public int bombs { get; set; }
        public int obstacles { get; set; }
        public double nps { get; set; }
        public double length { get; set; }
        public string characteristic { get; set; }
        public string difficulty { get; set; }
        public int events { get; set; }
        public bool chroma { get; set; }
        public bool me { get; set; }
        public bool ne { get; set; }
        public bool cinema { get; set; }
        public double seconds { get; set; }
        public ParitySummary paritySummary { get; set; }
        public double? stars { get; set; }
    }

    public class Version
    {
        public string hash { get; set; }
        public string key { get; set; }
        public string state { get; set; }
        public DateTime createdAt { get; set; }
        public int sageScore { get; set; }
        public List<Diff> diffs { get; set; }
        public string downloadURL { get; set; }
        public string coverURL { get; set; }
        public string previewURL { get; set; }
    }

    public class Curator
    {
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string type { get; set; }
        public string hash { get; set; }
    }

    public class Map2
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Uploader uploader { get; set; }
        public Metadata metadata { get; set; }
        public Stats stats { get; set; }
        public DateTime uploaded { get; set; }
        public bool automapper { get; set; }
        public bool ranked { get; set; }
        public bool qualified { get; set; }
        public List<Version> versions { get; set; }
        public Curator curator { get; set; }
        public DateTime curatedAt { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime lastPublishedAt { get; set; }
        public List<string> tags { get; set; }
    }

    public class Map
    {
        public Map map { get; set; }
        public double order { get; set; }
    }

    public class PlaylistPage
    {
        public Playlist playlist { get; set; }
        public List<Map> maps { get; set; }
    }


}