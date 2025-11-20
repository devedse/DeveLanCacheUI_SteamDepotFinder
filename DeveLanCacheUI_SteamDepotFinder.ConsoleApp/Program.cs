using DeveLanCacheUI_SteamDepotFinder.NewVersion;
using DeveLanCacheUI_SteamDepotFinder.NewVersion.Steam;
using Microsoft.Extensions.Logging;

namespace DeveLanCacheUI_SteamDepotFinder.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"DeveLanCacheUI_SteamDepotFinder version: {typeof(Program).Assembly.GetName().Version}");

            // Parse command-line arguments
            string? steamApiKey = null;

            if (args.Length > 0)
            {
                if (args[0] == "--help" || args[0] == "-h")
                {
                    ShowHelp();
                    return;
                }
                steamApiKey = args[0];
            }

            // Fallback to environment variable if no argument provided
            if (string.IsNullOrWhiteSpace(steamApiKey))
            {
                steamApiKey = Environment.GetEnvironmentVariable("STEAM_API_KEY");
            }

            if (string.IsNullOrWhiteSpace(steamApiKey))
            {
                Console.WriteLine("Steam API key is required!");
                Console.WriteLine();
                ShowHelp();
                Environment.Exit(1);
                return;
            }

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("DeveLanCacheUI_SteamDepotFinder", LogLevel.Information)
                    .AddConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Starting processing...");

            logger.LogInformation("Using Steam Store Service API with provided API key");

            // Create the Steam Store Service API instance
            var storeServiceApi = new DeveLanCacheUI_SteamDepotFinder.Steam.SteamStoreServiceApi(
                steamApiKey,
                loggerFactory.CreateLogger<DeveLanCacheUI_SteamDepotFinder.Steam.SteamStoreServiceApi>());

            var steamSession = new Steam3Session(loggerFactory.CreateLogger<Steam3Session>());
            steamSession.LoginToSteam();
            var appInfoHandler = new AppInfoHandler(steamSession, loggerFactory.CreateLogger<AppInfoHandler>(), storeServiceApi);

            var steamDepotObtainer = new SteamDepotObtainer(steamSession, appInfoHandler, loggerFactory.CreateLogger<SteamDepotObtainer>());

            await steamDepotObtainer.GoObtainDepotsAndWriteToFile();



            //For some reason threads remain running
            Environment.Exit(0);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: DeveLanCacheUI_SteamDepotFinder.ConsoleApp [STEAM_API_KEY]");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  STEAM_API_KEY    Steam API key for Store Service API (REQUIRED)");
            Console.WriteLine("                   Can also be set via STEAM_API_KEY environment variable");
            Console.WriteLine("                   Automatically saves app list to output/steam-applist.json");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -h, --help       Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  DeveLanCacheUI_SteamDepotFinder.ConsoleApp YOUR_API_KEY");
            Console.WriteLine("  set STEAM_API_KEY=YOUR_API_KEY && DeveLanCacheUI_SteamDepotFinder.ConsoleApp");
            Console.WriteLine();
            Console.WriteLine("Get your API key at: https://steamcommunity.com/dev/apikey");
        }
    }
}