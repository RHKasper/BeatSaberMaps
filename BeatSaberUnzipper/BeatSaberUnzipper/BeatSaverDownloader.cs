using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace BeatSaberUnzipper
{
    public static class BeatSaverDownloader
    {
        public const float DownloadTimeOutDuration = 5;

        public static MapData GetMapData(Song song) => GetMapData(song.hash);
        
        public static MapData GetMapData(string hash)
        {
            try
            {
                string uri = "https://api.beatsaver.com/maps/hash/" + hash;
                string fileContents = Get(uri);
                return JsonConvert.DeserializeObject<MapData>(fileContents);
            }
            catch (TimeoutException)
            {
                return null;
            }
        }
        
        public static BPList GetBpList(int id, out string fileContents)
        {
            try
            {
                string uri = "https://api.beatsaver.com/playlists/id/" + id + "/download";
                fileContents = Get(uri);
                return JsonConvert.DeserializeObject<BPList>(fileContents);
            }
            catch (TimeoutException)
            {
                fileContents = default;
                return null;
            }
        }

        public static void DownloadZipFile(string uri, string outFilePath, Action<string> onDownloadFinished)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadFileAsync(new Uri(uri),outFilePath);
            webClient.DownloadFileCompleted += (_, _) => onDownloadFinished(outFilePath);

        }
        
        /// <exception cref="TimeoutException"></exception>
        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadDataAsync(new Uri(uri));

            string result = default;
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            webClient.DownloadDataCompleted += (sender, args) =>
            {
                result = System.Text.Encoding.Default.GetString(args.Result);
            };

            TimeSpan duration = TimeSpan.FromSeconds(DownloadTimeOutDuration);
            while (result == default && stopwatch.Elapsed < duration) 
                Thread.Sleep(100);

            if (result == default && stopwatch.Elapsed >= duration)
                throw new TimeoutException($"Web Request for {uri} took more than {DownloadTimeOutDuration} second to complete");
            
            return result;
        }
    }
}