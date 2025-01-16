namespace DeveLanCacheUI_SteamDepotFinder.NewVersion.Steam.Exceptions
{
    public class SteamLoginException : Exception
    {
        public SteamLoginException()
        {

        }

        public SteamLoginException(string message) : base(message)
        {

        }

        public SteamLoginException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}