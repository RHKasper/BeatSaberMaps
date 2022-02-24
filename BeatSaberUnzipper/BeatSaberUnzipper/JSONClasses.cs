// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

using System;
using System.Collections.Generic;

public class CustomData
{
    public string syncURL { get; set; }
}

public class Song
{
    public string key { get; set; }
    public string hash { get; set; }
    public string songName { get; set; }
}

public class BPList
{
    public string playlistTitle { get; set; }
    public string playlistAuthor { get; set; }
    public string playlistDescription { get; set; }
    public string image { get; set; }
    public CustomData customData { get; set; }
    public List<Song> songs { get; set; }
}




//========= Song Data =================================================================================================

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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

    public class MapData
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
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime lastPublishedAt { get; set; }
    }

