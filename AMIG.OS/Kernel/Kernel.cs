using System;
using Sys = Cosmos.System;
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
        

        protected override void BeforeRun()
        {                    
            StartScreen.DisplayLogoLoading();

            // Initialisiere das Dateisystem und registriere VFS
            fs1 = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs1);

            userManagement = new UserManagement();
            fileSystemManager = new FileSystemManager();
            // Initialisiere CommandHandler mit Abhängigkeiten
            commandHandler = new CommandHandler(userManagement, fileSystemManager);
            systemServices = new SystemServices(commandHandler, userManagement);

            Console.Clear();
            var available_space = fs1.GetAvailableFreeSpace(@"0:\");
            Console.WriteLine("available free space: " + available_space/1024/1024 +"MB");

            var fs_type = fs1.GetFileSystemType(@"0:\");
            Console.WriteLine("file system type: " + fs_type);
            Console.WriteLine(System.DateTime.Now);
            userManagement.loginManager.ShowLoginOptions();
        }

        protected override void Run()
        {
            Console.ForegroundColor=ConsoleColor.Green;
            Console.Write(Helper.preInput);
            Console.ResetColor();
            systemServices.inputs();
        }
    }
}
