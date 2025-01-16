using System.Runtime.Serialization;

namespace DeveLanCacheUI_SteamDepotFinder.NewVersion.Steam.Exceptions
{
    public class SteamConnectionException : Exception
    {
        public SteamConnectionException()
        {

        }

        public SteamConnectionException(string message) : base(message)
        {

        }

        public SteamConnectionException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}