using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace Musoq.Console.Client
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        private readonly string _folderPath;
        private readonly static string MainFolderPath;

        public AssemblyLoader(string folderPath)
        {
            _folderPath = folderPath;
        }

        static AssemblyLoader()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            MainFolderPath = new FileInfo(executingAssembly.Location).DirectoryName;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var deps = DependencyContext.Default;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (res.Count > 0)
            {
                return Assembly.Load(new AssemblyName(res.First().Name));
            }
            else
            {
                var apiApplicationFileInfo = new FileInfo($"{_folderPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");
                var mainFolderDllCandidate = new FileInfo(Path.Combine(MainFolderPath, $"{assemblyName.Name}.dll"));
                if (File.Exists(apiApplicationFileInfo.FullName))
                {
                    var asl = new AssemblyLoader(apiApplicationFileInfo.DirectoryName);
                    return asl.LoadFromAssemblyPath(apiApplicationFileInfo.FullName);
                }
                else if (File.Exists(mainFolderDllCandidate.FullName))
                {
                    var asl = new AssemblyLoader(mainFolderDllCandidate.DirectoryName);
                    return asl.LoadFromAssemblyPath(mainFolderDllCandidate.FullName);
                }
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.FullName == assemblyName.FullName)
                    return assembly;

            return Assembly.Load(assemblyName);
        }
    }
}