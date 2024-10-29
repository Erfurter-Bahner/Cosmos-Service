using System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.FileManagement;
using System.IO;
using Sys = Cosmos.System;

namespace AMIG.OS.CommandProcessing
{
    public class CommandHandler
    {
        private readonly UserManagement userManagement;
        private readonly FileSystemManager fileSystemManager;
        private string currentDirectory = @"0:\"; // Root-Verzeichnis als Startpunkt
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
                case "removeall":
                    userManagement.RemoveAllUser();
                    break;

                // Datei- und Verzeichnisbefehle
                case "mkdir":
                    if (args.Length > 1)
                    {
                        string dirName = Path.Combine(currentDirectory, args[1]);
                        fileSystemManager.CreateDirectory(dirName);
                    }
                    else
                    {
                        Console.WriteLine("Bitte geben Sie einen Verzeichnisnamen an.");
                    }
                    break;

                case "cd":
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


                case "ls":
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

                case "write":
                    string fileName = Path.Combine(currentDirectory, args[1]); // Kombiniere den aktuellen Pfad mit dem Dateinamen
                    Console.WriteLine("Bitte Inhalt eingeben");
                    string content = Console.ReadLine();
                    fileSystemManager.WriteToFile(fileName, content);

                    break;

                case "rm":
                    if (args.Length > 1)
                    {
                        string filePath = Path.Combine(currentDirectory, args[1]);
                        fileSystemManager.DeleteFile(filePath);
                    }
                    else
                    {
                        Console.WriteLine("Bitte geben Sie eine Datei an.");
                    }
                    break;

                case "rmdir":
                    if (args.Length > 1)
                    {
                        string dirPath = Path.Combine(currentDirectory, args[1]);
                        fileSystemManager.DeleteDirectory(dirPath);
                    }
                    else
                    {
                        Console.WriteLine("Bitte geben Sie ein Verzeichnis an.");
                    }
                    break;

                case "touch":
                    if (args.Length > 1)
                    {
                        string filePath = Path.Combine(currentDirectory, args[1]);
                        fileSystemManager.CreateFile(filePath, "");
                    }
                    else
                    {
                        Console.WriteLine("Bitte geben Sie einen Dateinamen an.");
                    }
                    break;

                case "cat":
                    if (args.Length > 1)
                    {
                        string filePath = Path.Combine(currentDirectory, args[1]);
                        fileSystemManager.ReadFile(filePath);
                    }
                    else
                    {
                        Console.WriteLine("Bitte geben Sie eine Datei an.");
                    }
                    break;

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
