using System;
using System.IO;
using System.Text.Json;

namespace DeveLanCacheUI_SteamDepotFinder.Steam
{
    public static class SteamApi
    {
        private static readonly Lazy<Dictionary<int, App>> _steamAppDict = new Lazy<Dictionary<int, App>>(() => SteamApiData.applist.apps.ToDictionary(t => t.appid, t => t));
        private static readonly Lazy<SteamApiData> _steamApiData = new Lazy<SteamApiData>(LoadSteamApiData);

        public static SteamApiData SteamApiData => _steamApiData.Value;
        public static Dictionary<int, App> SteamAppDict => _steamAppDict.Value;

        private static SteamApiData LoadSteamApiData()
        {
            string path = Path.Combine("Steam", "SteamData.json");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<SteamApiData>(json);
            }
            else
            {
                throw new FileNotFoundException($"File not found: {path}");
            }
        }
    }
}
