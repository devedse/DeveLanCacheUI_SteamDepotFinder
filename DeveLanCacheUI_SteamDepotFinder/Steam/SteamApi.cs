using System.Text.Json;

namespace DeveLanCacheUI_SteamDepotFinder.Steam
{
    public static class SteamApi
    {
        private static readonly Lazy<Dictionary<uint, App>> _steamAppDict = new Lazy<Dictionary<uint, App>>(() =>
        {
            //.ToDictionary(t => t.appid, t => t)
            //Previously we had the code mentioned above but this sometimes resulted in a duplicate appid (still weird but whatever this should solve it)

            var allSteamApps = SteamApiData.applist.apps;

            var outputDict = new Dictionary<uint, App>();

            foreach (var app in allSteamApps)
            {
                if (outputDict.ContainsKey(app.appid))
                {
                    Console.WriteLine($"Found duplicate appid: {app.appid} with name: {app.name}");
                }
                else
                {
                    outputDict.Add(app.appid, app);
                }
            }

            return outputDict;
        });
        private static readonly Lazy<SteamApiData> _steamApiData = new Lazy<SteamApiData>(LoadSteamApiData);

        public static SteamApiData SteamApiData => _steamApiData.Value;
        public static Dictionary<uint, App> SteamAppDict => _steamAppDict.Value;

        private static SteamApiData LoadSteamApiData()
        {
            var subDir = "Steam";
            string path = Path.Combine(subDir, "SteamData.json");
            if (File.Exists(path))
            {
                Console.WriteLine($"Found {path} so reading apps from that file.");
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<SteamApiData>(json);
            }
            else
            {
                Console.WriteLine($"Could not find {path}, so obtaining new SteamApi Data...");
                using var c = new HttpClient();
                var result = c.GetAsync("https://api.steampowered.com/ISteamApps/GetAppList/v2/").Result;
                var resultString = result.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Writing result to file. First 1000 chars: {resultString.Substring(0, 1000)}");

                if (!Directory.Exists(subDir))
                {
                    Directory.CreateDirectory(subDir);
                }

                File.WriteAllText(path, resultString);
                return JsonSerializer.Deserialize<SteamApiData>(resultString);
            }
        }
    }
}
