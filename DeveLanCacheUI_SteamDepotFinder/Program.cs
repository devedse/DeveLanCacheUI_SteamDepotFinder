namespace DeveLanCacheUI_SteamDepotFinder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var qrCodeLoginner = new QrCodeDingLogin();
            qrCodeLoginner.GoLogin();
        }
    }
}