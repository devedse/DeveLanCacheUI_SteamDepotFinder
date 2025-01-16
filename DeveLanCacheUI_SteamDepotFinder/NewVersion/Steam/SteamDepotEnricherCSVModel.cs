namespace DeveLanCacheUI_SteamDepotFinder.NewVersion.Steam
{
    public class SteamDepotEnricherCSVModel
    {
        public required uint SteamAppId { get; init; }
        public required string SteamAppName { get; init; }
        public required uint SteamDepotId { get; init; }
    }
}
