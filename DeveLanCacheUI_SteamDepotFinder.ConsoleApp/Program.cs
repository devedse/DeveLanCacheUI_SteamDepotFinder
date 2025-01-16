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

            var steamSession = new Steam3Session(loggerFactory.CreateLogger<Steam3Session>());
            steamSession.LoginToSteam();
            var appInfoHandler = new AppInfoHandler(steamSession, loggerFactory.CreateLogger<AppInfoHandler>());

            var steamDepotObtainer = new SteamDepotObtainer(steamSession, appInfoHandler, loggerFactory.CreateLogger<SteamDepotObtainer>());

            await steamDepotObtainer.GoObtainDepotsAndWriteToFile();



            //For some reason threads remain running
            Environment.Exit(0);
        }
    }
}