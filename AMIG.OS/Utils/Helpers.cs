using AMIG.OS.UserSystemManagement;
using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using AMIG.OS.FileManagement;
using System.IO;
namespace AMIG.OS.Utils
{
    public class Helpers
    {
        private readonly UserManagement userManagement;
        private readonly FileSystemManager fileSystemManager;
        public Helpers(UserManagement userManagement, FileSystemManager fileSystemManager)
        {
            this.userManagement = userManagement;
            this.fileSystemManager = fileSystemManager;
        }

        public String preInput = "Input: ";
        // Neue Methode zum Hinzufügen eines Benutzers
        public void addHelper(bool admin_true)
        {
            if (admin_true)
            {
                Console.WriteLine("Name des anzulegende Benutzers");
                string username = Console.ReadLine();

                if (userManagement.UserExists(username))
                {
                    do
                    {
                        Console.WriteLine("Benutzer existiert bereits");
                        Console.WriteLine("Name des anzulegende Benutzers");
                        username = Console.ReadLine();
                    }
                    while (userManagement.UserExists(username));
                }

                Console.WriteLine("Rolle des anzulegende Benutzers: Standard oder Admin");
                string role = Console.ReadLine().ToLower();

                if (role != "admin" && role != "standard")
                {
                    do
                    {
                        Console.WriteLine("Rolle des anzulegende Benutzers: Standard oder Admin");
                        role = Console.ReadLine().ToLower();
                    }
                    while (role != "admin" && role != "standard");

                }

                // Passwortabfrage
                Console.WriteLine("Passwort des anzulegende Benutzers");
                string pw = ConsoleHelpers.GetPassword();
                string pw_2;
                do
                {
                    Console.WriteLine("PW des anzulegende Benutzers wiederholen");
                    pw_2 = ConsoleHelpers.GetPassword();
                } while (pw != pw_2);

                userManagement.AddUserWithRoleAndPassword(username, pw, role);
            }
            else Console.WriteLine("Keine Berechtigung für diesen Command"); 
        }

        public void AdiosHelper()
        {
                Console.Clear();
                Console.WriteLine("\n\tHASTA LA VISTA");
                Thread.Sleep(1500);
                Sys.Power.Shutdown();
        }

        public void ShowAllHelper(bool admin_true)
        {
            if (admin_true) userManagement.DisplayAllUsers();
            else Console.WriteLine("Keine Berechtigung für diesen Command");
        }

        public void ShowMeHelper(string loggedInUser)
        {
            Console.WriteLine("Benutzerinformationen:");
            userManagement.DisplayUser(loggedInUser); // Benutzerinfo nur für den angemeldeten Benutzer
        }

        public void RemoveHelper(bool admin_true)
        {
            if (admin_true)
            {
                Console.Write("Benutzername des zu entfernenden Benutzers: ");
                string username = Console.ReadLine();
                userManagement.RemoveUser(username);
            }
            else Console.WriteLine("Keine Berechtigung für diesen Command");
        }

        public void ChangeNameHelper(string loggedInUser)
        {
            string newUsername;
            string entscheidung;

            do
            {
                Console.Write("Neuer Benutzername: ");
                newUsername = Console.ReadLine();

                do
                {
                    Console.Write("Bestätigen: y/n ");
                    entscheidung = Console.ReadLine().ToLower();

                    if (entscheidung == "n")
                    {
                        Console.WriteLine("Benutzername nicht bestätigt. Bitte erneut eingeben.");
                        break;  // Schleife verlassen, um neuen Benutzernamen einzugeben
                    }
                    else if (entscheidung != "y" && entscheidung != "n")
                    {
                        Console.WriteLine("Ungültige Eingabe: y/n ");
                    }

                } while (entscheidung != "y" && entscheidung != "n");

            } while (entscheidung != "y");

            //if (userManagement.ChangeUsername(loggedInUser, newUsername))
            //{
            //    loggedInUser = newUsername; // Aktualisiere den aktuellen Benutzernamen
            //}
        }

        public void ChangePasswortHelper(string loggedInUser)
        {

            Console.Write("Altes Passwort: ");
            string oldPassword =ConsoleHelpers.GetPassword() ;

            Console.Write("Neues Passwort: ");
            string newPassword = ConsoleHelpers.GetPassword();
            userManagement.ChangePassword(loggedInUser, oldPassword, newPassword);
        }

        public void mkdirHelper(bool admin_true, string[]args, string currentDirectory)
        {
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
        }

        public void cdHelper(string[] args, ref string currentDirectory) 
        {
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
        }

        public void lsHelper( string[] args, string currentDirectory)
        {
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
        }

        public void writeHelper(bool admin_true, string[] args, string currentDirectory)
        {
            if (admin_true)
            {
                string fileName = Path.Combine(currentDirectory, args[1]); // Kombiniere den aktuellen Pfad mit dem Dateinamen
                Console.WriteLine("Bitte Inhalt eingeben");
                string content = Console.ReadLine();
                fileSystemManager.WriteToFile(fileName, content);
            }
            else Console.WriteLine("Keine Berechtigung für diesen Command");
        }

        public void rmHelper(bool admin_true, string[] args, string currentDirectory)
        {
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
        }

        public void rmdirHelper(bool admin_true, string[] args, string currentDirectory)
        {
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
        }

        public void touchHelper(bool admin_true, string[] args, string currentDirectory)
        {
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
        }

        public void catHelper(string[] args, string currentDirectory, string role)
        {
            if (args.Length > 1)
            {
                string filePath = Path.Combine(currentDirectory, args[1]);
                fileSystemManager.ReadFile(filePath, role);
            }
            else Console.WriteLine("Bitte geben Sie eine Datei an.");
        }

        public void setpermHelper(bool admin_true, string[] args, string currentDirectory)
        {
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
        }

        public void unlockHelper(bool admin_true, string[] args, string currentDirectory)
        {
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
        }

        public void error1() {
                Console.WriteLine("Missing or unknown argument");
            }
        //hier kommen noch die anderen befehle

    }
}
