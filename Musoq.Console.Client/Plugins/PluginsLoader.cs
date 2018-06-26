using System;
using System.Linq;
using Musoq.Schema;

namespace Musoq.Console.Client
{
    public static class PluginsLoader
    {
        private static Type[] _plugins;

        public static Type[] LoadDllBasedSchemas()
        {
            if (_plugins != null)
                return _plugins;

            var assemblies = PluginsHelper.GetReferencingAssemblies("Plugins");
            var assemblyTypes = assemblies.SelectMany(assembly => assembly.GetTypes());

            var interfaceType = typeof(ISchema);

            _plugins = assemblyTypes
                .Where(type => interfaceType.IsAssignableFrom(type) && type.HasParameterlessConstructor()).ToArray();

            return _plugins;
        }

        private static bool HasParameterlessConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}