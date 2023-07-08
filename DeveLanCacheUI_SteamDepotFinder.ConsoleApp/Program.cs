namespace DeveLanCacheUI_SteamDepotFinder.ConsoleApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Finding Depots...");

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
                }

            }
        }
    }
}