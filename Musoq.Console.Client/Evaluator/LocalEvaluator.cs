using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Musoq.Console.Client.Helpers;
using Musoq.Converter;
using Musoq.Converter.Build;
using Musoq.Converter.Exceptions;
using Musoq.Evaluator;
using Musoq.Plugins;
using Musoq.Schema;
using Musoq.Service.Client.Core;
using Environment = Musoq.Plugins.Environment;

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

            var query = GetQuery();
            var plugins = LoadPlugins();

            if (Configuration.DebugInfo)
            {
                System.Console.WriteLine($"Loaded plugins ({plugins.Count}):");
                foreach (var plugin in plugins)
                {
                    System.Console.WriteLine($"Assembly {plugin.Value.FullName}");
                }
            }

            var tempDir = Path.Combine(Path.GetTempPath(), "Musoq", "Compiled");

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            byte[] queryHash;
            using (var hashCreator = new MD5CryptoServiceProvider())
            {
                queryHash = hashCreator.ComputeHash(Encoding.UTF8.GetBytes(query));
            }

            var queryHashString = BitConverter.ToString(queryHash).Replace("-", "");

            var dllPath = Path.Combine(tempDir, $"{queryHashString}.dll");
            var pdbPath = Path.Combine(tempDir, $"{queryHashString}.pdb");

            var schemaProvider = new DynamicSchemaProvider(plugins);

            Assembly assembly;
            if (!File.Exists(dllPath) || Configuration.CompileOnly)
            {
                var items = new BuildItems
                {
                    SchemaProvider = schemaProvider,
                    RawQuery = query
                };

                new Environment().SetValue(Constants.NetStandardDllEnvironmentName, EnvironmentUtils.GetOrCreateEnvironmentVariable());

                var throwAfterSave = false;
                CompilationException exc = null;
                try
                {
                    BuildChain chain = new CreateTree(
                        new TransformTree(
                            new TurnQueryIntoRunnableCode(null)));

                    chain.Build(items);
                }
                catch (CompilationException ce)
                {
                    throwAfterSave = true;
                    exc = ce;
                }

                if (items.DllFile?.Length > 0)
                {
                    using (var writer = new BinaryWriter(File.OpenWrite(dllPath)))
                    {
                        writer.Write(items.DllFile);
                    }
                }

                if (items.PdbFile?.Length > 0)
                {
                    using (var writer = new BinaryWriter(File.OpenWrite(pdbPath)))
                    {
                        writer.Write(items.PdbFile);
                    }
                }

                if (!string.IsNullOrEmpty(Configuration.OutputTranslatedQuery))
                {
                    var path = Configuration.OutputTranslatedQuery;
                    var fileInfo = new FileInfo(path);

                    if (fileInfo.Directory == null || !fileInfo.Directory.Exists)
                        throw new DirectoryNotFoundException(fileInfo.DirectoryName);

                    var builder = new StringBuilder();
                    using (var writer = new StringWriter(builder))
                    {
                        items.Compilation?.SyntaxTrees.ElementAt(0).GetRoot().WriteTo(writer);
                    }

                    using (var file = new StreamWriter(File.OpenWrite(path)))
                    {
                        file.Write(builder.ToString());
                    }
                }

                if (throwAfterSave)
                    throw exc;

                assembly = Assembly.Load(items.DllFile, items.PdbFile);
            }
            else
            {
                assembly = Assembly.LoadFile(dllPath);
            }

            if(Configuration.CompileOnly || !string.IsNullOrEmpty(Configuration.OutputTranslatedQuery))
                return new ResultTable(string.Empty, new string[0], new object[0][], new string[0], TimeSpan.Zero);

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
                    if (Configuration.DebugInfo)
                        System.Console.WriteLine(e);
                }
            }

            return loadedSchemas;
        }
    }
}