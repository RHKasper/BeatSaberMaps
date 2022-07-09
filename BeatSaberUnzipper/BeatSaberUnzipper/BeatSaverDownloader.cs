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
        public static Doc SearchForSong(string title, string artist)
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
                SearchQuery searchQuery = JsonConvert.DeserializeObject<SearchQuery>(fileContents);
                var selectedMapDoc = GetBestMap(title, artist, searchQuery);
                return selectedMapDoc;
            }
            catch
            {
                return null;
            }
        }

        public static Doc GetBestMap(string title, string artist, SearchQuery searchQuery)
        {
            return searchQuery.docs.First();
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

        /// <summary>
        /// download a zip file from <see cref="uri"/> to <see cref="outFilePath"/>.
        /// </summary>
        /// <param name="uri">Where the file is downloaded from</param>
        /// <param name="outFilePath">Where the file is saved on the local drive</param>
        /// <param name="onDownloadFinished">Action gets passed outFilePath (.zip file)</param>
        public static void DownloadZipFile(string uri, string outFilePath, Action<string> onDownloadFinished)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadFileAsync(new Uri(uri),outFilePath);
            webClient.DownloadFileCompleted += (_, _) => onDownloadFinished(outFilePath);
        }

        public static void GetMapData(string uri, Action<MapData> mapData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadDataAsync(new Uri(uri));
            
            webClient.DownloadDataCompleted += (_, args) =>
            {
                JsonConvert.DeserializeObject<MapData>(System.Text.Encoding.Default.GetString(args.Result));
            };
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

        public static void GetAsync(string uri, Action<string> callback)
        {
            
        }
    }
}