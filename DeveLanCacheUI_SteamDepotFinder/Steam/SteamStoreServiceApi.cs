using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DeveLanCacheUI_SteamDepotFinder.Steam
{
    public class SteamStoreServiceApi
    {
        private readonly string _apiKey;
        private readonly ILogger<SteamStoreServiceApi> _logger;
        private readonly HttpClient _httpClient;

        public SteamStoreServiceApi(string apiKey, ILogger<SteamStoreServiceApi> logger)
        {
            _apiKey = apiKey;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Fetches all apps from the Steam Store Service API using pagination
        /// </summary>
        /// <returns>Complete list of all Steam apps with extended Store Service properties</returns>
        public async Task<List<StoreServiceApp>> GetAllAppsAsync()
        {
            var allApps = new List<StoreServiceApp>();
            uint lastAppId = 0;
            bool hasMoreResults = true;
            int pageNumber = 1;

            _logger.LogInformation("Starting to fetch apps from Steam Store Service API");

            while (hasMoreResults)
            {
                _logger.LogInformation("Fetching page {PageNumber} (last_appid: {LastAppId})", pageNumber, lastAppId);

                var url = $"https://api.steampowered.com/IStoreService/GetAppList/v1/?key={_apiKey}&max_results=50000&last_appid={lastAppId}";
                
                try
                {
                    var response = await _httpClient.GetStringAsync(url);
                    var appListResponse = JsonSerializer.Deserialize<StoreServiceAppListResponse>(response);

                    if (appListResponse?.response?.apps != null && appListResponse.response.apps.Count > 0)
                    {
                        allApps.AddRange(appListResponse.response.apps);
                        _logger.LogInformation("Fetched {Count} apps on page {PageNumber}. Total apps so far: {TotalCount}", 
                            appListResponse.response.apps.Count, pageNumber, allApps.Count);

                        hasMoreResults = appListResponse.response.have_more_results;
                        lastAppId = appListResponse.response.last_appid;
                        pageNumber++;
                    }
                    else
                    {
                        _logger.LogWarning("Received empty or null response on page {PageNumber}", pageNumber);
                        hasMoreResults = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching apps on page {PageNumber}", pageNumber);
                    throw;
                }

                // Add a small delay to avoid hitting rate limits
                await Task.Delay(100);
            }

            _logger.LogInformation("Completed fetching all apps. Total apps: {TotalCount}", allApps.Count);
            return allApps;
        }
    }
}
