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

                //ausstehende befehle: created, lastlogin, change pw, 

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

                case "showtime": //both
                    TimeSpan current = DateTime.Now - starttime;
                    Console.WriteLine($" Eingeloggte Zeit: {current}");
                    break;

                case "logout": //both
                    showLoginOptions.Invoke();
                    break;

                case "add": //admin
                    if(admin_true) helpers.AddUserCommand();
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                // Datei- und Verzeichnisbefehle
                case "mkdir": //admin
                    if (admin_true)
                    {
                        if (args.Length > 1)
                        {
                            string dirName = Path.Combine(currentDirectory, args[1]);
                            fileSystemManager.CreateDirectory(dirName);
                        }
                        else Console.WriteLine("Bitte geben Sie einen Verzeichnisnamen an.");    
                    }
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                case "cd": //both
                    if (args.Length > 1)
                    {
                        string newDir;

                        // Wenn der Benutzer ".." eingibt, navigiert er eine Verzeichnisebene zurück
                        if (args[1] == "..")
                        {
                            // Parent-Directory extrahieren
                            newDir = Directory.GetParent(currentDirectory)?.FullName;
                            if (newDir == null)
                            {
                                Console.WriteLine("Sie befinden sich bereits im Root-Verzeichnis.");
                            }
                            else
                            {
                                currentDirectory = newDir;
                                Console.WriteLine($"Verzeichnis gewechselt zu '{currentDirectory}'.");
                            }
                        }
                        else
                        {
                            // Andernfalls kombiniere den aktuellen Pfad mit dem neuen Unterverzeichnis
                            newDir = Path.Combine(currentDirectory, args[1]);
                            if (Directory.Exists(newDir))
                            {
                                currentDirectory = newDir;
                                Console.WriteLine($"Verzeichnis gewechselt zu '{currentDirectory}'.");
                            }
                            else
                            {
                                Console.WriteLine("Verzeichnis existiert nicht.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Bitte geben Sie ein Verzeichnis an.");
                    }
                    break;

                case "ls": //both
                    try
                    {
                        var directories = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(currentDirectory);
                        foreach (var dir in directories)
                        {
                            Console.WriteLine($"{(dir.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory ? "[DIR]" : "[FILE]")} {dir.mName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler bei der Verzeichnisauflistung: {ex.Message}");
                    }
                    break;

                case "write": //admin, fehlerbehandlung noch impllementieren
                    if (admin_true)
                    {  
                        string fileName = Path.Combine(currentDirectory, args[1]); // Kombiniere den aktuellen Pfad mit dem Dateinamen
                        Console.WriteLine("Bitte Inhalt eingeben");
                        string content = Console.ReadLine();
                        fileSystemManager.WriteToFile(fileName, content);
                    }
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                case "rm": //admin
                    if (admin_true)
                    {
                        if (args.Length > 1)
                        {
                            string filePath = Path.Combine(currentDirectory, args[1]);
                            fileSystemManager.DeleteFile(filePath);
                        }
                        else Console.WriteLine("Bitte geben Sie eine Datei an.");
                    }
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                case "rmdir": //admin
                    if (admin_true)
                    {
                        if (args.Length > 1)
                        {
                            string dirPath = Path.Combine(currentDirectory, args[1]);
                            fileSystemManager.DeleteDirectory(dirPath);
                        }
                        else Console.WriteLine("Bitte geben Sie ein Verzeichnis an.");
                    }
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                case "touch": //admin
                    if (admin_true)
                    {
                        if (args.Length > 1)
                        {
                            string filePath = Path.Combine(currentDirectory, args[1]);
                            fileSystemManager.CreateFile(filePath, "");
                        }
                        else Console.WriteLine("Bitte geben Sie einen Dateinamen an.");
                  
                    }
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                case "cat": //both
                    if (args.Length > 1)
                    {
                        string filePath = Path.Combine(currentDirectory, args[1]);
                        fileSystemManager.ReadFile(filePath,role);
                    }
                    else
                    {
                        Console.WriteLine("Bitte geben Sie eine Datei an.");
                    }
                    break;

                case "space": //both
                    var availableSpace = vfs.GetAvailableFreeSpace(@"0:\");
                    Console.WriteLine($"Verfügbarer Speicherplatz: {availableSpace} Bytes");
                    break;

                case "setperm": //admin
                    // Berechtigung für Datei oder Verzeichnis setzen
                    if (admin_true)
                    {
                        if (args.Length == 3)
                        {
                            string path = Path.Combine(currentDirectory, args[1]);
                            string permission = args[2];
                            fileSystemManager.SetPermission(path, permission);
                            Console.WriteLine($"Berechtigung '{permission}' für '{path}' gesetzt.");
                        }
                        else Console.WriteLine("Ungültige Argumente für setperm.");
                    }
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
                    break;

                case "unlock": //admin
                    if (admin_true)
                    {
                        if (args.Length == 2)
                        {
                            string path = Path.Combine(currentDirectory, args[1]);
                            fileSystemManager.UnlockFile(path);
                        }
                    else Console.WriteLine("Ungültige Argumente für unlock.");
                    
                    }
                    else Console.WriteLine("Keine Berechtigung für diesen Command");
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
