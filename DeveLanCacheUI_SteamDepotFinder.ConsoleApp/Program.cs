namespace DeveLanCacheUI_SteamDepotFinder.ConsoleApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"DeveLanCacheUI_SteamDepotFinder version: {typeof(Program).Assembly.GetName().Version}");

            Console.WriteLine("Finding Depots...");

            int retries = 0;

            while (true)
            {
                try
                {
                    var qrCodeLoginner = new QrCodeDingLogin();
                    await qrCodeLoginner.GoLogin();
                    break;
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine("Timeout, retrying...");
                    retries++;
                }

            }

            Console.WriteLine($"Application completed with {retries} retries");

            //For some reason threads remain running
            Environment.Exit(0);
        }
    }
}