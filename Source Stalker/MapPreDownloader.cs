﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using HenkesUtils.Steam;
using ICSharpCode.SharpZipLib.BZip2;
using System.Threading.Tasks;

namespace Source_Stalker {
	class MapPreDownloader {

		static Dictionary<int, string> installPaths=new Dictionary<int, string>();

		public async Task<bool> ReadyMapAsync(ServerStatus.A2S_INFO_Response info, string fastDLRoot) {
			string installPath = getInstallPath(info.ID);

			string mapDownloadFolder = $@"{installPath}\{info.Folder}\download\maps\";
			string mapStockFolder = $@"{installPath}\{info.Folder}\maps\";

			string downloadedPath = mapDownloadFolder + info.Map;
			if(File.Exists(downloadedPath)) return true;
			if(File.Exists(mapStockFolder + info.Map)) return true;

			await DownloadFile(fastDLRoot, $"maps/{info.Map}.bsp", downloadedPath);

			return false;
		}

		private static async Task DownloadFile(string fastDLRoot, string fileNameAndPath, string downloadedPath) {
			string downloadURL = $"{fastDLRoot}/{fileNameAndPath}";
			string bz2URL = $"{downloadURL}.bz2";

			var client = new HttpClient();

			var httpResponse = await client.GetAsync(bz2URL);
			bool bz2Used = true;
			if(!httpResponse.IsSuccessStatusCode) {
				httpResponse = await client.GetAsync(downloadURL);
				bz2Used = false;
			}
			httpResponse.EnsureSuccessStatusCode();

			Stream readStream = null;
			try {
				readStream = await httpResponse.Content.ReadAsStreamAsync();

				if(bz2Used) {
					readStream = new BZip2InputStream(readStream);
				}

				using(var writeStream = File.OpenWrite(downloadedPath)) {
					await readStream.CopyToAsync(writeStream);
				}
			} finally {
				readStream.Dispose();
			}
		}

		private static string getInstallPath(int appId) {
			string path;
			if(installPaths.TryGetValue(appId, out path)) return path;
			path=SteamHelper.GetInstallPathForApp(appId);
			installPaths.Add(appId, path);
			return path;
		}
	}
}
