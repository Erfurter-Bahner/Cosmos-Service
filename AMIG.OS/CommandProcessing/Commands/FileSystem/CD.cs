using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using AMIG.OS.Utils;
using AMIG.OS.FileManagement;
using System.IO;

namespace AMIG.OS.CommandProcessing.Commands.FileSystem
{
    internal class CD : ICommand
    {
        private readonly FileSystemManager fileSystemManager;

        public string Description => "Change the current directory.";
        public string PermissionName { get; } = Permissions.cd; // Permission für 'cd' Befehl
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-dir", "The directory to change to (absolute or relative)."},
            {"-help", "Show help for this command."},
        };

        public CD(FileSystemManager fileSystemManager)
        {
            this.fileSystemManager = fileSystemManager ?? throw new ArgumentNullException(nameof(fileSystemManager));
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

            if (!parameters.TryGetValue("dir", out string dir) || string.IsNullOrWhiteSpace(dir))
            {
                ConsoleHelpers.WriteError("Please provide a valid directory. Use '-help' for usage details.");
                return;
            }

            try
            {
                // Sonderfall: Wechsel ins übergeordnete Verzeichnis
                if (dir == "..")
                {
                    var parent = Directory.GetParent(fileSystemManager.CurrentDirectory);
                    if (parent == null)
                    {
                        ConsoleHelpers.WriteError("You are already in the root directory.");
                        return;
                    }

                    fileSystemManager.CurrentDirectory = parent.FullName;
                    ConsoleHelpers.WriteSuccess($"Changed directory to '{fileSystemManager.CurrentDirectory}'.");
                    return;
                }

                // Prüfe, ob der Pfad relativ oder absolut ist
                string newPath = Path.IsPathRooted(dir)
                    ? dir
                    : Path.Combine(fileSystemManager.CurrentDirectory, dir);

                // Verzeichnis existiert prüfen
                if (Directory.Exists(newPath))
                {
                    fileSystemManager.CurrentDirectory = newPath;
                    ConsoleHelpers.WriteSuccess($"Changed directory to '{fileSystemManager.CurrentDirectory}'.");
                }
                else
                {
                    ConsoleHelpers.WriteError($"The directory '{newPath}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"An error occurred while changing the directory: {ex.Message}");
            }
        }

        public void ShowHelp()
        {
            ConsoleHelpers.WriteSuccess(Description);
            Console.WriteLine("Usage: cd [options]");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }

        }
    }
}
