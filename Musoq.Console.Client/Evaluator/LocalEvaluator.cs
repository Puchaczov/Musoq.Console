using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Musoq.Converter;
using Musoq.Evaluator;
using Musoq.Schema;
using Musoq.Service.Client.Core;

namespace Musoq.Console.Client.Evaluator
{
    public class LocalEvaluator : EvaluatorBase
    {
        public LocalEvaluator(ApplicationArguments args) 
            : base(args)
        { }

        public override ResultTable Evaluate()
        {
            var watch = new Stopwatch();
            watch.Start();

            var plugins = LoadPlugins();
            var schemaProvider = new DynamicSchemaProvider(plugins);

            var tempDir = Path.Combine(Path.GetTempPath(), "Musoq", "Compiled");

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            byte[] queryHash;
            using (var hashCreator = new MD5CryptoServiceProvider())
            {
                queryHash = hashCreator.ComputeHash(Encoding.UTF8.GetBytes(GetQuery()));
            }

            var queryHashString = BitConverter.ToString(queryHash).Replace("-", "");

            var dllPath = Path.Combine(tempDir, $"{queryHashString}.dll");
            var pdbPath = Path.Combine(tempDir, $"{queryHashString}.pdb");

            Assembly assembly;
            if (!File.Exists(dllPath))
            {
                var arrays = InstanceCreator.CompileForStore(GetQuery(), schemaProvider);

                using (var writer = new BinaryWriter(File.OpenWrite(dllPath)))
                {
                    writer.Write(arrays.DllFile);
                }

                using (var writer = new BinaryWriter(File.OpenWrite(pdbPath)))
                {
                    writer.Write(arrays.PdbFile);
                }

                assembly = Assembly.Load(arrays.DllFile, arrays.PdbFile);
            }
            else
            {
                assembly = Assembly.LoadFile(dllPath);
            }

            var runnableType = assembly.GetTypes().Single(type => type.FullName.ToLowerInvariant().Contains("query"));

            var instance = (IRunnable) Activator.CreateInstance(runnableType);
            instance.Provider = schemaProvider;

            var compiledQuery = new CompiledQuery(instance);
            var table = compiledQuery.Run();

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