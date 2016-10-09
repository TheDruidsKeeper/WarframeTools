using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;

namespace WarframeTools {
	class Exporter {
		public static List<string> GetFiles() {
			var json = File.ReadAllText(ConfigurationManager.AppSettings["DownloadUrlsJson"]);
			var downloadUrls = JsonConvert.DeserializeObject<DownloadUrls>(json);
			var downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "JsonExports");
			if (!Directory.Exists(downloadFolder))
				Directory.CreateDirectory(downloadFolder);

			var downloadedFiles = AlreadyDownloadFiles(downloadFolder);
			if (downloadedFiles.Count == downloadUrls.Urls.Count)
				return downloadedFiles;
			else
				return DownloadFiles(downloadFolder, downloadUrls);
		}

		private static List<string> DownloadFiles(string downloadFolder, DownloadUrls downloadUrls) {
			var concurrentFilesList = new ConcurrentBag<string>();
			Parallel.ForEach(downloadUrls.Urls, (url) => {
				var webRequest = (HttpWebRequest)WebRequest.Create(url);
				webRequest.Method = "POST";
				using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
				using (var responseStream = webResponse.GetResponseStream())
				using (var streamReader = new StreamReader(responseStream)) {
					//var fileName = webRequest.Headers[HttpRequestHeader.]
					var fileName = Path.GetFileName(url);
					var filePath = Path.Combine(downloadFolder, fileName);
					string fileData = streamReader.ReadToEnd();
					File.WriteAllText(filePath, fileData);
					concurrentFilesList.Add(filePath);
				}
			});
			return concurrentFilesList.ToList();
		}

		private static List<string> AlreadyDownloadFiles(string downloadFolder) {
			return Directory.EnumerateFiles(downloadFolder, "*.json").ToList();
		}

		class DownloadUrls {
			public List<string> Urls { get; set; }
		}
	}
}
