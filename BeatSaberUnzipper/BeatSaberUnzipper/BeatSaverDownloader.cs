﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace BeatSaberUnzipper
{
    public static class BeatSaverDownloader
    {
        public const float DownloadTimeOutDuration = 5;


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

        public static void GetMapData(string uri, Action<MapData> callback)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            webClient.DownloadDataAsync(new Uri(uri));
            
            webClient.DownloadDataCompleted += (_, args) =>
            {
                MapData mapData =  JsonConvert.DeserializeObject<MapData>(System.Text.Encoding.Default.GetString(args.Result));
                callback.Invoke(mapData);
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

        public static async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri)
        {
            using var httpClient = new HttpClient();

            // Get the file extension
            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);

            // Create file path and ensure directory exists
            var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
            Directory.CreateDirectory(directoryPath);

            if (Path.HasExtension(path) == false)
            {
                Console.WriteLine($"{path} doesn't have a file extension. Assuming it's a .png file.");
                path += ".png";
            }

            // Download the image and write to the file
            var imageBytes = await httpClient.GetByteArrayAsync(uri);
            await File.WriteAllBytesAsync(path, imageBytes);
        }
    }
}