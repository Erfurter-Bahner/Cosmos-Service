﻿using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMIG.OS.Utils;
using AMIG.OS.FileManagement;
using Sys = Cosmos.System;
using System.IO;


namespace AMIG.OS.CommandProcessing.Commands.FileSystem
{

    internal class RemoveFile : ICommand
    {
        private readonly FileSystemManager fileSystemManager;
        private User LoggedInUser;
        public string Description => "remove file";
        public string PermissionName { get; } = Permissions.rmfile;
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-file", "file to remove"},
           {"-help", "Show help for this command."},
        };
        public RemoveFile(FileSystemManager fileSystemManagement)
        {
            this.fileSystemManager = fileSystemManagement;
        }
        // Implementing CanExecute as defined in the custom ICommand interface
        public bool CanExecute(User currentUser)
        {
            return currentUser.HasPermission(PermissionName);
        }

        // Implementing Execute as defined in the custom ICommand interface
        public void Execute(CommandParameters parameters, User currentUser)
        {
            if (parameters.TryGetValue("help", out _))
            {
                ShowHelp();
                return;
            }
            parameters.TryGetValue("file", out string file);
            
            if (!string.IsNullOrWhiteSpace(file))
            {
                string filePath = Path.Combine(fileSystemManager.CurrentDirectory, file);

                if (File.Exists(filePath))
                {
                    fileSystemManager.DeleteFile(filePath);
                }

            }
            else
            {
                ConsoleHelpers.WriteError("Insufficient arguments. Use -help to see usage.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: rmfile [options] ");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"{param.Key}\t{param.Value}");
            }
        }

    }
}
