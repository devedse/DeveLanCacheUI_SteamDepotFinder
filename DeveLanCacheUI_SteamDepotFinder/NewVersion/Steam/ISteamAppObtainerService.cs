using DeveLanCacheUI_SteamDepotFinder.Steam;

namespace DeveLanCacheUI_SteamDepotFinder.NewVersion.Steam
{
    public interface ISteamAppObtainerService
    {
        App? GetSteamAppById(uint? steamAppId);
    }
}
