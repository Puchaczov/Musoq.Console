using Musoq.Repository.Installer.Modules;
using Musoq.Repository.Installer.Programs;
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
            var pluginsDir = Path.Combine(context.InstalledProgramDirectoryPath, "Program", "Plugins");

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

    public class ProgramInstaller : IProgramInstaller
    {
        public void Install(IProgramInstallationContext context)
        {
            if (File.Exists(context.ImagePath))
            {
                var file = File.ReadAllBytes(context.ImagePath);
                PackerHelper.UnpackProgram(file, context.ProgramInstallationDirectoryPath);
            }
        }

        public void Uninstall(IProgramUninstallContext context)
        {
            var programPath = Path.Combine(context.InstalledProgramDirectoryPath, "Program");
            if (Directory.Exists(programPath))
                Directory.Delete(programPath, true);

            var starterPath = Path.Combine(context.InstalledProgramDirectoryPath, "Starter");
            if (Directory.Exists(starterPath))
                Directory.Delete(starterPath, true);

            var modulesInstallerPath = Path.Combine(context.InstalledProgramDirectoryPath, "ModulesInstaller");
            if (Directory.Exists(modulesInstallerPath))
                Directory.Delete(modulesInstallerPath, true);
        }

        public void Update(IProgramUpdateContext context)
        {
            var programPath = Path.Combine(context.InstalledProgramDirectoryPath, "Program");
            if (Directory.Exists(programPath))
                Directory.Delete(programPath, true);

            var starterPath = Path.Combine(context.InstalledProgramDirectoryPath, "Starter");
            if (Directory.Exists(starterPath))
                Directory.Delete(starterPath, true);

            if (File.Exists(context.ImagePath))
            {
                var file = File.ReadAllBytes(context.ImagePath);
                PackerHelper.UnpackProgram(file, context.InstalledProgramDirectoryPath);
            }
        }
    }
}
