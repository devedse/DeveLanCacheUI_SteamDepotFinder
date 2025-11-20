namespace DeveLanCacheUI_SteamDepotFinder.Steam
{
    /// <summary>
    /// Model for the new Steam Store Service API response
    /// </summary>
    public class StoreServiceAppListResponse
    {
        public StoreServiceResponseData response { get; set; } = new();
    }

    public class StoreServiceResponseData
    {
        public List<StoreServiceApp> apps { get; set; } = new();
        public bool have_more_results { get; set; }
        public uint last_appid { get; set; }
    }

    /// <summary>
    /// App model for the Store Service API with extended properties
    /// </summary>
    public class StoreServiceApp
    {
        public uint appid { get; set; }
        public string name { get; set; } = string.Empty;
        public long? last_modified { get; set; }
        public long? price_change_number { get; set; }

        /// <summary>
        /// Converts to the standard App model
        /// </summary>
        public App ToApp()
        {
            return new App
            {
                appid = appid,
                name = name
            };
        }
    }
}
