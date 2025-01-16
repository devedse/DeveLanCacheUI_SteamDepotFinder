using DeveLanCacheUI_SteamDepotFinder.NewVersion.Steam;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveLanCacheUI_SteamDepotFinder.NewVersion
{
    public class SteamDepotObtainer
    {
        private readonly Steam3Session _steam3Session;
        private readonly AppInfoHandler _appInfoHandler;
        private readonly ILogger<SteamDepotObtainer> _logger;

        public SteamDepotObtainer(Steam3Session steam3Session, AppInfoHandler appInfoHandler, ILogger<SteamDepotObtainer> logger)
        {
            _steam3Session = steam3Session;
            _appInfoHandler = appInfoHandler;
            _logger = logger;
        }

        public async Task GoObtainDepotsAndWriteToFile()
        {
            _logger.LogInformation("Ensuring apps are loaded...");
            await _appInfoHandler.EnsureAppsAreLoaded();
            _logger.LogInformation("Ensuring apps are loaded completed");

            var picsChangesResult = await _steam3Session.SteamAppsApi.PICSGetChangesSince().ToTask();
            var currentChangeNumber = picsChangesResult.CurrentChangeNumber;
            var _currentChangeNumber = currentChangeNumber;
            var changedApps = (await _appInfoHandler.RetrieveAllAppIds2()).Select(t => t.appid).ToList();

            _logger.LogInformation($"Changelist 0 -> {_currentChangeNumber} ({changedApps.Count} apps)");

            _logger.LogInformation($"Processing everything in bulk...");

            var superList = new List<DepotInformationThing>();

            for (var i = 0; i < changedApps.Count; i += 1000)
            {
                _logger.LogInformation($"Processing {i} -> {i + 1000} / {changedApps.Count}");

                var currentBatch = changedApps.Skip(i).Take(1000).ToList();
                var appInfos = await _appInfoHandler.BulkLoadAppInfoAsync(currentBatch);

                foreach (var depot in appInfos)
                {
                    superList.Add(new DepotInformationThing(depot.SteamAppId, string.IsNullOrWhiteSpace(depot.SteamAppName) ? "unknown" : depot.SteamAppName, depot.SteamDepotId));
                }
            }

            _logger.LogInformation("Obtaining depots completed, total depots: " + superList.Count);

            _logger.LogInformation("Filtering duplicate depots...");

            var superListFiltered = superList.DistinctBy(t => new { t.DepotId, t.AppId }).OrderBy(t => t.AppId).ThenBy(t => t.DepotId).ThenBy(t => t.AppName).ToList();

            _logger.LogInformation("Filtering duplicate depots completed, total depots: " + superListFiltered.Count);

            _logger.LogInformation("Writing depots to file...");

            var sb = new StringBuilder();
            foreach (var depot in superListFiltered)
            {
                sb.AppendLine(depot.ToCsvString());
            }

            var depots = sb.ToString();
            Directory.CreateDirectory("output");
            var outputPath = Path.Combine("output", "app-depot-output-cleaned.csv");
            File.WriteAllText(outputPath, depots);
        }
    }
}
