using Musoq.Repository.Installer.Modules;
using Musoq.Repository.Package;
using System;
using System.IO;

namespace Musoq.Console.InstallerUpdater
{
    public class ModuleInstaller : IModuleInstaller
    {
        public void Install(IModuleInstallationContext context)
        {
            var pluginsDir = Path.Combine(context.InstalledProgramDirectoryPath, "Program", "Plugins");

            if (!Directory.Exists(pluginsDir))
                Directory.CreateDirectory(pluginsDir);

            var file = File.ReadAllBytes(context.ImagePath);
            PackerHelper.UnpackModule(file, pluginsDir);
        }

        public void Uninstall(IModuleUninstallContext context)
        {
            var pluginsDir = Path.Combine(context.InstalledProgramDirectoryPath, "Program", "Plugins", context.Name);

            if (Directory.Exists(pluginsDir))
                Directory.Delete(pluginsDir, true);
        }

        public void Update(IModuleUpdateContext context)
        {
            var pluginsDir = Path.Combine(context.InstalledProgramDirectoryPath, "Program", "Plugins");

            if (!Directory.Exists(pluginsDir))
                Directory.CreateDirectory(pluginsDir);

            var file = File.ReadAllBytes(context.ImagePath);
            PackerHelper.UnpackModule(file, pluginsDir);
        }
    }
}
