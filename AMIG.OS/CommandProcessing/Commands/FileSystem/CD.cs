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

        public string Description => "Change directory.";
        public string PermissionName { get; } = Permissions.cd; // Permission für 'cd' Befehl
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-dir", "Directory to change to (absolute or relative)."},
            {"-help", "Show help."},
        };

        public CD(FileSystemManager fileSystemManager)
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

            if (!parameters.TryGetValue("dir", out string dir) || string.IsNullOrWhiteSpace(dir))
            {
                Console.WriteLine("Bitte geben Sie ein gültiges Verzeichnis an. Verwenden Sie -help für Details.");
                return;
            }

            // Sonderfall: Wechsel ins übergeordnete Verzeichnis
            if (dir == "..")
            {
                // Verwende Directory.GetParent
                var parent = Directory.GetParent(fileSystemManager.CurrentDirectory);
                if (parent == null) // Kein übergeordnetes Verzeichnis vorhanden (Root)
                {
                    Console.WriteLine("Sie befinden sich bereits im Root-Verzeichnis.");
                    return;
                }

                fileSystemManager.CurrentDirectory = parent.FullName;
                Console.WriteLine($"Verzeichnis gewechselt zu '{fileSystemManager.CurrentDirectory}'.");
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
                Console.WriteLine($"Verzeichnis gewechselt zu '{fileSystemManager.CurrentDirectory}'.");
            }
            else
            {
                Console.WriteLine($"Verzeichnis '{newPath}' existiert nicht.");
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: cd -dir <directory>");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }
    }
}


