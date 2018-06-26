using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Musoq.Console.Client
{
    public static class PluginsHelper
    {
        private static IEnumerable<FileInfo> GetFilesFromPluginsFolder(string pluginsFolder, string searchPattern)
        {
            var assembly = Assembly.GetEntryAssembly();
            var fileInfo = new FileInfo(assembly.Location);

            if (fileInfo.Directory == null)
                return new List<FileInfo>();

            var thisDir = fileInfo.Directory.FullName;
            var pluginsDir = new DirectoryInfo(Path.Combine(thisDir, pluginsFolder));

            if (!pluginsDir.Exists)
                pluginsDir.Create();

            return pluginsDir
                .GetDirectories()
                .SelectMany(sm => sm
                    .GetFiles(searchPattern));
        }

        public static IEnumerable<Assembly> GetReferencingAssemblies(string pluginsFolder)
        {
            var files = GetFilesFromPluginsFolder(pluginsFolder, "*.dll").ToArray();

            var assemblies = new List<Assembly>();
            foreach(var file in files)
            {
                AssemblyLoader loader = new AssemblyLoader(file.Directory.FullName);
                var assembly = loader.LoadFromAssemblyName(AssemblyName.GetAssemblyName(file.FullName));
                assemblies.Add(assembly);
            }

            return assemblies;
        }
    }
}