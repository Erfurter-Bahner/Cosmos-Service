using System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.FileManagement;

namespace AMIG.OS.CommandProcessing
{
    public class CommandHandler
    {
        private readonly UserManagement userManagement;
        private readonly FileSystemManager fileSystemManager;

        public CommandHandler(UserManagement userMgmt, FileSystemManager fsManager)
        {
            userManagement = userMgmt;
            fileSystemManager = fsManager;
        }

        public void ProcessCommand(string input, string loggedInUser)
        {
            var args = input.Split(' ');

            switch (args[0].ToLower())
            {
                // Benutzerbefehle
                case "showall":
                    userManagement.DisplayAllUsers();
                    break;

                // Datei- und Verzeichnisbefehle
                case "mkdir":
                case "rmdir":
                case "touch":
                case "rm":
                //case "cat":
                //    fileSystemManager.ExecuteCommand(args);
                //    break;

                // Beispiel für andere Befehle
                case "help":
                    Console.WriteLine("Available commands: showall, adduser, removeuser, mkdir, rmdir, touch, rm, cat.");
                    break;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
