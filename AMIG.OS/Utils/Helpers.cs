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

        // Neue Methode zum Hinzufügen eines Benutzers
        public void AddUserCommand()
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

        public void AdiosHelper(string[] args)
        {
            if (args.Length == 2 && args[1].Equals("amigos"))
            {
                Console.WriteLine("\n\tHASTA LA VISTA");
                Thread.Sleep(1500);
                Sys.Power.Shutdown();
            }
        }

        public void ShowAllHelper(bool admin_true)
        {
            if (admin_true) userManagement.DisplayAllUsers();
            else Console.WriteLine("Keine Berechtigung für diesen Command");
        }

        public void ShowMeHelper(string loggedInUser)
        {
            Console.WriteLine("Benutzerinformationen:");
            userManagement.GetUserInfo(loggedInUser); // Benutzerinfo nur für den angemeldeten Benutzer
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

            if (userManagement.ChangeUsername(loggedInUser, newUsername))
            {
                loggedInUser = newUsername; // Aktualisiere den aktuellen Benutzernamen
            }
        }

        public void ChangePasswortHelper(string loggedInUser)
        {

            Console.Write("Altes Passwort: ");
            string oldPassword = Console.ReadLine();

            Console.Write("Neues Passwort: ");
            string newPassword = Console.ReadLine();
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

        public void cdHelper(bool admin_true, string[] args, string currentDirectory)
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
        public void lsHelper(bool admin_true, string[] args, string currentDirectory)
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



        //hier kommen noch die anderen befehle

    }
}
