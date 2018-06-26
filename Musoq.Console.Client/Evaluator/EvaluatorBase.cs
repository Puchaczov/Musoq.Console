using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Musoq.Console.Client.Helpers;
using Musoq.Converter;
using Musoq.Schema;
using Musoq.Service.Client.Core;
using Musoq.Service.Client.Core.Helpers;

namespace Musoq.Console.Client.Evaluator
{
    public abstract class EvaluatorBase
    {
        protected readonly ApplicationArguments Args;

        public EvaluatorBase(ApplicationArguments args)
        {
            Args = args;
        }

        public abstract ResultTable Evaluate();

        protected string GetQuery()
        {
            return string.IsNullOrEmpty(Args.QuerySourceFile)
                ? Args.Query
                : File.ReadAllText(Args.QuerySourceFile);
        }
    }

    public class RemoteEvaluator : EvaluatorBase
    {
        public RemoteEvaluator(ApplicationArguments args) 
            : base(args)
        { }

        public override ResultTable Evaluate()
        {
            var api = new ApplicationFlowApi(string.IsNullOrEmpty(Args.Address)
                ? Configuration.Address
                : Args.Address);

            return api.RunQueryAsync(QueryContext.FromQueryText(GetQuery())).Result;
        }
    }

    public class LocalEvaluator : EvaluatorBase
    {
        public LocalEvaluator(ApplicationArguments args) 
            : base(args)
        { }

        public override ResultTable Evaluate()
        {
            var watch = new Stopwatch();
            watch.Start();

            var root = InstanceCreator.CreateTree(GetQuery());
            var plugins = LoadPlugins();
            var vm = InstanceCreator.Create(root, new DynamicSchemaProvider(plugins));
            var table = vm.Run();

            watch.Stop();

            var columns = table.Columns.Select(f => f.Name).ToArray();
            var rows = table.Select(f => f.Values).ToArray();
            var result = new ResultTable(table.Name, columns, rows, new string[0], watch.Elapsed);

            return result;
        }

        private Dictionary<string, Type> LoadPlugins()
        {
            var loadedSchemas = new Dictionary<string, Type>();

            var types = new List<Type>();

            types.AddRange(PluginsLoader.LoadDllBasedSchemas());

            foreach (var type in types)
            {
                try
                {
                    var schema = (ISchema)Activator.CreateInstance(type);
                    loadedSchemas.TryAdd($"#{schema.Name.ToLowerInvariant()}", type);
                }
                catch (Exception e)
                {
                }
            }

            return loadedSchemas;
        }
    }
}
