using Musoq.Repository.Installer.Starter;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Musoq.Console.Starter
{
    public class ConsoleStarter : IProcessStarter
    {
        public Process Create(StarterInfo info)
        {
            var args = new StringBuilder();

            foreach (var arg in info.Arguments)
            {
                args.Append(arg.GetStringRepresentation());
                args.Append(" ");
            }

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "dotnet",
                    Arguments = $"{Path.Combine(info.ExecutableFileDirectory.FullName, "Program", info.EntryPoint)} {args}",
                    WorkingDirectory = Path.Combine(info.ExecutableFileDirectory.FullName, "Program"),
                    UseShellExecute = false
                }
            };
        }
    }
}
