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


namespace AMIG.OS.CommandProcessing.Commands.FileSystem
{

    internal class MakeDir : ICommand
    {
        private readonly FileSystemManager fileSystemManager;
        private User LoggedInUser;
        public string Description => "create a directory";
        public string PermissionName { get; } = Permissions.mkdir;
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
           {"-dir", "The directory to change to (absolute or relative)."},
           {"-help", "Show help for this command."},
        };
        public MakeDir(FileSystemManager fileSystemManagement)
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
            parameters.TryGetValue("dir", out string dir);
            if (!string.IsNullOrWhiteSpace(dir))
            {
                string dirPath = Path.Combine(fileSystemManager.CurrentDirectory, dir);

                if (!Directory.Exists(dirPath))
                {
                    fileSystemManager.CreateDirectory(dirPath);
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
            Console.WriteLine("Usage: mkdir [options] ");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }

    }
}
