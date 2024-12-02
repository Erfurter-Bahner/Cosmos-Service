using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMIG.OS.Utils;
using AMIG.OS.FileManagement;
using Sys = Cosmos.System;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Principal;


namespace AMIG.OS.CommandProcessing.Commands.FileSystem
{
    internal class WRITE : ICommand
    {
        private readonly FileSystemManager fileSystemManager;

        public string Description => "Write text to a file.";
        public string PermissionName { get; } = Permissions.write;
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-file", "File to write to (will be created if it does not exist)."},
            {"-text", "Text to write into the file."},
            {"-help", "Show help for this command."},
        };

        public WRITE(FileSystemManager fileSystemManager)
        {
            this.fileSystemManager = fileSystemManager;
        }

        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        public void Execute(CommandParameters parameters, User currentUser)
        {
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }

            if (parameters.TryGetValue("file", out string file) && !string.IsNullOrWhiteSpace(file))
            {
                string filePath = Path.Combine(fileSystemManager.CurrentDirectory, file);

                // Datei erstellen, falls sie nicht existiert
                if (!File.Exists(filePath))
                {
                    fileSystemManager.CreateFile(filePath, "");
                }

                if (parameters.TryGetValue("text", out string text) && !string.IsNullOrWhiteSpace(text))
                {
                   
                    // Schreibe Text in eine neue Zeile
                    fileSystemManager.WriteToFile(filePath, text);
                    ConsoleHelpers.WriteSuccess($"Text successfully written in '{filePath}'.");
                }
                else
                {
                    ConsoleHelpers.WriteError("No Text to write. Use -help to see usage.");
                }
            }
            else
            {
                ConsoleHelpers.WriteError("Insufficient arguments. Use -help to see usage.");
            }
        }


        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: write [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }
    }
}