using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMIG.OS.Utils;
using AMIG.OS.FileManagement;
using Sys = Cosmos.System;
using CosmosFileSystem=Cosmos.System.FileSystem.VFS;


namespace AMIG.OS.CommandProcessing.Commands.FileSystem
{
    
    internal class LS : ICommand
    {
        public FileSystemManager fileSystemManager;
        private User LoggedInUser;
       
        public string Description => "list files/dir";
        public string PermissionName { get; } = Permissions.ls;
        public Dictionary<string, string> Parameters => new Dictionary<string, string>
        {
            {"-help", "show help"},
        };
        public LS(FileSystemManager fileSystemManagement)
        {
            this.fileSystemManager = fileSystemManagement;
        }
        //private string CurrentDirectory => fileSystemManagement.CurrentDirectory;

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
            if (parameters.Parameters.Count == 0)
            {
                try
                {
                    var directories = CosmosFileSystem.VFSManager.GetDirectoryListing(fileSystemManager.CurrentDirectory);
                    foreach (var dir in directories)
                    {
                        Console.WriteLine($"{(dir.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory ? "[DIR]" : "[FILE]")} {dir.mName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler bei der Verzeichnisauflistung: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Insufficient arguments. Use -help to see usage.");
            }
        }

        // Show help method as defined in the custom ICommand interface
        public void ShowHelp()
        {
            Console.WriteLine(Description);
            Console.WriteLine("Usage: ls ");
            foreach (var param in Parameters)
            {
                Console.WriteLine($"  {param.Key}\t{param.Value}");
            }
        }

    }
}
