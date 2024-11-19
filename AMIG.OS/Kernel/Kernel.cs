using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.Linq;
using AMIG.OS.Utils;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.FileManagement;
using AMIG.OS.CommandProcessing;

namespace AMIG.OS.Kernel
{
    public class Kernel : Sys.Kernel
    {
        private Sys.FileSystem.CosmosVFS fs1;
        private FileSystemManager fileSystemManager;
        private UserManagement userManagement;
        private CommandHandler commandHandler;
        private SystemServices systemServices;
        DateTime starttime;

        protected override void BeforeRun()
        { 
            // Initialisiere das Dateisystem und registriere VFS
            fs1 = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs1);
            var available_space = fs1.GetAvailableFreeSpace(@"0:\");
            Console.WriteLine("available free space: " + available_space);

            var fs_type = fs1.GetFileSystemType(@"0:\");
            Console.WriteLine("file system type: " + fs_type);

            userManagement = new UserManagement();
            fileSystemManager = new FileSystemManager();

            // Initialisiere CommandHandler mit Abhängigkeiten
            commandHandler = new CommandHandler(userManagement, fileSystemManager);
            systemServices = new SystemServices(commandHandler, userManagement);

            Console.Clear();
            Console.WriteLine("\n\t\t\t _____\r\n\t\t\t/     \\\r\n\t_______/_______\\_______\r\n\t\\\\______AMIG.OS______//\n");           
            userManagement.loginManager.ShowLoginOptions();
        }

        protected override void Run()
        {
            Console.Write(Helper.preInput);
            systemServices.inputs();
        }
    }
}
