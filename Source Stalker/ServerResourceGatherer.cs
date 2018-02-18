using KVLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Source_Stalker {
    class ServerResourceGatherer {

        public static async Task Gather(ServerStatus server) {
            if(server == null) throw new ArgumentNullException();
            if(server.rules == null) throw new ArgumentNullException();
            Dictionary<string, string> rules = server.rules.rules;

            if(!rules.TryGetValue("sv_downloadurl", out string downloadUrl)) return;

            List<string> fileList = new List<string>();

            string resUrl = $"{downloadUrl}maps/{server.info.Map}.res";

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(resUrl);

            Stream s = await response.Content.ReadAsStreamAsync();
            KeyValue resKv=ParseResFile(s);

        }

        private static KeyValue ParseResFile(Stream s) {
            using(BinaryReader r = new BinaryReader(s)) {
                string strData = new string(r.ReadChars((int)s.BytesAvailable()));
                return KVParser.ParseKeyValueText(strData);
            }
        }
    }
}
