using static SteamKit2.Internal.CContentBuilder_CommitAppBuild_Request;

namespace DeveLanCacheUI_SteamDepotFinder.NewVersion.Steam
{
    public record DepotInformationThing(uint AppId, string? AppName, uint DepotId)
    {
        public string ToCsvString()
        {
            return $"{AppId};{AppName?.Replace(";", ":") ?? ""};{DepotId}";
        }
    }
}