using System.Configuration;

namespace Musoq.Console.Client.Helpers
{
    public static class Configuration
    {
        public static string Address => ConfigurationManager.AppSettings["ApiAddress"];

        public static bool DebugInfo { get; set; }

        public static bool CompileOnly { get; set; }

        public static string OutputTranslatedQuery { get; set; }
    }
}