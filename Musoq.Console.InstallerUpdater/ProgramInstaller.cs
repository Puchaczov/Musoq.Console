using Musoq.Repository.Installer.Programs;
using Musoq.Repository.Package;
using System.IO;

namespace Musoq.Console.InstallerUpdater
{
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
