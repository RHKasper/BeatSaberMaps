// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

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