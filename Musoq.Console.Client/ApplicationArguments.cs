using CommandLine;

namespace Musoq.Console.Client
{
    public class ApplicationArguments
    {
        [Option('q', HelpText = "Put the query here!")]
        public string Query { get; set; }

        [Option("use-service", HelpText = "Use service to evaluate the query.")]
        public bool UseService { get; set; }

        [Option("addr", HelpText = "Set different address for particular query.", Required = false)]
        public string Address { get; set; }

        [Option("qs", HelpText = "Use the query from file.", Required = false)]
        public string QuerySourceFile { get; set; }

        [Option("sd", HelpText = "Save response to file.")]
        public string QueryScoreFile { get; set; }

        [Option("debugInfo", HelpText = "Will provide additional infos about the internals.")]
        public bool DebugInfo { get; set; }

        [Option("compileOnly", HelpText = "Compiles the query without running it.")]
        public bool CompileOnly { get; set; }

        [Option("outputTranslatedQuery", HelpText = "Generates the .CS file that is direct equivalent for SQL query.")]
        public string OutputTranslatedQuery { get; set; }
    }
}