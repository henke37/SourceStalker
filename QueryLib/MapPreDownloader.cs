using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using HenkesUtils.Steam;
using ICSharpCode.SharpZipLib.BZip2;
using System.Threading.Tasks;

namespace Source_Stalker {
	public class MapPreDownloader {

		static Dictionary<int, string> installPaths=new Dictionary<int, string>();

		private ServerStatus status;
		private readonly string fastDLRoot;

		public MapPreDownloader(ServerStatus status, string fastDLRoot) {
			this.status = status;
			this.fastDLRoot = fastDLRoot;
		}

		public Task ReadyServerAsync() {
			string mapName = status.info.Map;
			var nowT=ReadyMapAsync(mapName);

			string nextMap=status.rules["nextlevel"];
			if(status.rules.TryGetCVar("sm_nextmap",out string smNextMap)) {
				nextMap = smNextMap;
			}

			Task nextT;
			if(nextMap!="") {
				nextT = ReadyMapAsync(nextMap);
			} else {
				nextT = Task.CompletedTask;
			}

			return Task.WhenAll(nextT, nowT);
		}

		private async Task ReadyMapAsync(string mapName) {
			string installPath = getInstallPath(status.info.Id);

			string mapDownloadFolder = $@"{installPath}\{status.info.Folder}\download\maps\";
			string mapStockFolder = $@"{installPath}\{status.info.Folder}\maps\";

			string downloadedPath = mapDownloadFolder + mapName + ".bsp";
			if(File.Exists(downloadedPath)) return;
			if(File.Exists(mapStockFolder + mapName)) return;

			await DownloadFile($"maps/{mapName}.bsp", downloadedPath);
		}

		private async Task DownloadFile(string fileNameAndPath, string downloadedPath) {
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
				if(httpResponse.Headers.TryGetValues("Last-Modified", out var dates)) {
					var enumerator=dates.GetEnumerator();
					enumerator.MoveNext();
					var time=DateTime.Parse(enumerator.Current);
					File.SetLastWriteTimeUtc(downloadedPath, time);
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
