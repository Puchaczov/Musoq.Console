using System;
using System.Data;
using System.Linq;
using System.Reflection;
using CommandLine;
using Musoq.Console.Client.Evaluator;
using Musoq.Console.Client.Helpers;

namespace Musoq.Console.Client
{
    public static class Program
    {
        private static int Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            return CommandLine.Parser.Default.ParseArguments<ApplicationArguments>(args)
                .MapResult(
                    ProcessArguments,
                    _ => 1);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.FullName == args.Name)
                    return assembly;

            return null;
        }

        private static int ProcessArguments(ApplicationArguments appArgs)
        {
            Configuration.DebugInfo = appArgs.DebugInfo;

            EvaluatorBase evaluator;

            if (appArgs.UseService)
                evaluator = new RemoteEvaluator(appArgs);
            else
                evaluator = new LocalEvaluator(appArgs);

            var result = evaluator.Evaluate();

            var dt = new DataTable(result.Name);

            dt.Columns.AddRange(result.Columns.Select(f => new DataColumn(f)).ToArray());

            foreach (var item in result.Rows)
                dt.Rows.Add(item);

            Printer printer;

            if (string.IsNullOrEmpty(appArgs.QueryScoreFile))
                printer = new ConsolePrinter(dt, result.ComputationTime);
            else
                printer = new CsvPrinter(dt, appArgs.QueryScoreFile);

            printer.Print();

            return 0;
        }
    }
}