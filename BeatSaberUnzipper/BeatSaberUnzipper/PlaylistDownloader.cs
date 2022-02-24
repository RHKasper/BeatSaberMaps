using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace BeatSaberUnzipper
{
    public static class PlaylistDownloader
    {
        public static BPList DownloadBPList(int id, out string fileContents)
        {
            string uri = "https://api.beatsaver.com/playlists/id/" + id + "/download";
            fileContents = Get(uri);
            return JsonConvert.DeserializeObject<BPList>(fileContents);
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