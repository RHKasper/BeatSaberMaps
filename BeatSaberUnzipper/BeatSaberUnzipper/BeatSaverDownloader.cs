using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace BeatSaberUnzipper
{
    public static class BeatSaverDownloader
    {

        public static MapData GetMapData(Song song) => GetMapData(song.hash);
        public static MapData GetMapData(string hash)
        {
            string uri = "https://api.beatsaver.com/maps/hash/" + hash;
            string fileContents = Get(uri);
            return JsonConvert.DeserializeObject<MapData>(fileContents);
        }
        
        public static BPList GetBpList(int id, out string fileContents)
        {
            string uri = "https://api.beatsaver.com/playlists/id/" + id + "/download";
            fileContents = Get(uri);
            return JsonConvert.DeserializeObject<BPList>(fileContents);
        }

        public static void DownloadZipFile(string uri, string outFilePath, Action<string> onDownloadFinished)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadFileAsync(new Uri(uri),outFilePath);
            webClient.DownloadFileCompleted += (_, _) => onDownloadFinished(outFilePath);

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