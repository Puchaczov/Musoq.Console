using System.Configuration;

namespace Musoq.Console.Client.Helpers
{
    public static class Configuration
    {
        public static string Address => ConfigurationManager.AppSettings["ApiAddress"];
    }
}