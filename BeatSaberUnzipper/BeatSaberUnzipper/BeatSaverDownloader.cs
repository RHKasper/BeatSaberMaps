using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace BeatSaberUnzipper
{
    public static class BeatSaverDownloader
    {
        public const float DownloadTimeOutDuration = 5;

        /// <summary>
        /// https://api.beatsaver.com/docs/index.html?url=./swagger.json#/OrderedMap%20%7B%20%22name%22%3A%20%22Search%22%20%7D/get_search_text__page_
        /// </summary>
        /// <returns>Hash of the chosen map, or empty if no viable map is found</returns>
        public static string SearchForSong(string title, string artist)
        {
            bool allowChroma = false, allowCinema = false, allowNoodle = false, requireCurated = false;
            
            string uri = "https://api.beatsaver.com/search/text/0?";

            // if (!allowChroma)
            //     uri += "chroma=false&";
            // if (!allowCinema)
            //     uri += "cinema=false&";
            // if (!allowNoodle)
            //     uri += "noodle=false&";

            uri += $"q={title}";
            uri += "&sortOrder=Relevance";

            try
            {
                string fileContents = Get(uri);

                //Console.WriteLine($"{title}:\n\n{fileContents}");
                SearchQuery searchQuery = JsonConvert.DeserializeObject<SearchQuery>(fileContents);

                Console.WriteLine($"{title} {artist}\n");
                var docs  = searchQuery.docs.Where(d => d.name.Contains(title) && d.name.Contains(artist));
                foreach (Doc doc in docs)
                    Console.WriteLine($"{doc.name}: {doc.versions.First().previewURL}");

                Console.WriteLine("==============================================================\n\n");
                return searchQuery.docs.First().versions.First().hash;
            }
            catch
            {
                return "";
            }
        }
        
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