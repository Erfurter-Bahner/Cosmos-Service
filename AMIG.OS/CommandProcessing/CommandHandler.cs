using System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.FileManagement;
using AMIG.OS.Utils;
using System.IO;
using Sys = Cosmos.System;
using System.Threading;
using System.Data;


namespace AMIG.OS.CommandProcessing
{
    public class CommandHandler
    {
        private readonly UserManagement userManagement;
        private readonly FileSystemManager fileSystemManager;
        private readonly Helpers helpers;
        private DateTime starttime;
        private string currentDirectory = @"0:\"; // Root-Verzeichnis als Startpunkt
        private readonly Action showLoginOptions;
        private readonly Sys.FileSystem.CosmosVFS vfs; //damit freier speicherplatz angezeigt werden kann

        public CommandHandler
            (UserManagement userMgmt, 
            FileSystemManager fsManager, 
            Action showLoginOptionsDelegate,
            Helpers _helpers,
            Sys.FileSystem.CosmosVFS vfs)
        {
            userManagement = userMgmt;
            fileSystemManager = fsManager;
            showLoginOptions = showLoginOptionsDelegate; // Delegate speichern
            helpers = _helpers;
            this.vfs = vfs; 
        }

        public void SetStartTime(DateTime loginTime)
        {
            starttime = loginTime;
        }

        public void ProcessCommand(string input, string loggedInUser)
        {
            var args = input.Split(' ');

            bool admin_true = userManagement.GetUserRole(loggedInUser).ToLower() == "admin";
            string role = userManagement.GetUserRole(loggedInUser).ToLower();
            switch (args[0].ToLower())
            {
                //ausstehende befehle: created, lastlogin
                case "datetime":
                    DateTime now=DateTime.Now;
                    Console.WriteLine(now);
                    break;

                case "adios":
                    helpers.AdiosHelper(args);
                    break;

                // Benutzerbefehle
                case "showall": //Admin
                    helpers.ShowAllHelper(admin_true);
                    break;

                case "showme": //Both
                    helpers.ShowMeHelper(loggedInUser);
                    break;

                case "remove": //Admin
                    helpers.RemoveHelper(admin_true);
                    break;

                case "removeall": //Admin
                    if(admin_true) userManagement.RemoveAllUser();
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                case "changename": // Both
                    helpers.ChangeNameHelper(loggedInUser);
                    break;

                case "changepw": //both
                    helpers.ChangePasswortHelper(loggedInUser);
                    break;

                case "showtime": //both
                    TimeSpan current = DateTime.Now - starttime;
                    Console.WriteLine($" Eingeloggte Zeit: {current}");
                    break;

                case "logout": //both
                    showLoginOptions.Invoke();
                    break;

                case "add": //admin
                    helpers.addHelper(admin_true);
                    break;

                // Datei- und Verzeichnisbefehle
                case "mkdir": //admin
                    helpers.mkdirHelper(admin_true, args, currentDirectory);
                    break;

                case "cd": //both             
                    helpers.cdHelper(args, ref currentDirectory);
                    break;

                case "ls": //both                    
                    helpers.lsHelper(args, currentDirectory);
                    break;

                case "write": //admin, fehlerbehandlung noch impllementieren
                    helpers.writeHelper(admin_true, args, currentDirectory);
                    break;

                case "rm": //admin
                    helpers.rmHelper(admin_true, args, currentDirectory);
                    break;

                case "rmdir": //admin
                    helpers.rmdirHelper(admin_true, args, currentDirectory);
                    break;

                case "touch": //admin
                    helpers.touchHelper(admin_true, args, currentDirectory);
                    break;

                case "cat": //both
                    helpers.catHelper(args, currentDirectory, role);
                    break;

                case "space": //both
                    var availableSpace = vfs.GetAvailableFreeSpace(@"0:\");
                    Console.WriteLine($"Verfügbarer Speicherplatz: {availableSpace} Bytes");
                    break;

                case "setperm": //admin
                    helpers.setpermHelper(admin_true, args, currentDirectory);
                    break;

                case "unlock": //admin
                    helpers.unlockHelper(admin_true, args, currentDirectory);
                    break;

                // Beispiel für andere Befehle
                case "help": //both
                    Console.WriteLine("Available commands: showall, adduser, removeuser, mkdir, rmdir, touch, rm, cat.");
                    break;

                default: //both
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
