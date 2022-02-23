using System;
using System.IO;
using System.Net;

namespace BeatSaberUnzipper
{
    public static class SongDownloader
    {
        public static string GetPlaylist(int id, int pageNumber = 0)
        {
            string uri = "https://api.beatsaver.com/playlists/id/" + id + "/" + pageNumber;
            return Get(uri);
        }
        
        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}