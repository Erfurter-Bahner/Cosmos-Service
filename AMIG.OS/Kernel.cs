using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;
using System.Security.Cryptography;
using System.Drawing;
using System.Data;
using Cosmos.System;
using Console = System.Console;
using System.IO;
using System.Text.Json;


namespace AMIG.OS
{
    public class Kernel : Sys.Kernel
    {
        private static UserManagement userManagement = new UserManagement(); // Singleton für UserManagement
        private static Sys.FileSystem.CosmosVFS fs;

        string loggedInUser; // Aktuell eingeloggter Benutzer
        DateTime starttime;
        public static string GetPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true); // true = Zeichen wird nicht angezeigt

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    // Ein Zeichen aus der Passworteingabe löschen
                    password = password.Substring(0, password.Length - 1);

                    // Cursor eine Stelle zurücksetzen, das Sternchen überschreiben und Cursor wieder zurücksetzen
                    int cursorPos = Console.CursorLeft;
                    Console.SetCursorPosition(cursorPos - 1, Console.CursorTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(cursorPos - 1, Console.CursorTop);
                }
                else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    // Zeichen zum Passwort hinzufügen und Sternchen anzeigen
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter); // Schleife läuft, bis Enter gedrückt wird

            Console.WriteLine(); // Zeilenumbruch nach der Eingabe
            return password;
        }

        protected override void BeforeRun()
        {
            // Registrierung des virtuellen Dateisystems
            fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            // Initialisiere die Benutzerverwaltung
            userManagement.LoadUsers(); // Lade die Benutzer beim Start

            Console.WriteLine("Benutzerverwaltung ist bereit.");

            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            Console.WriteLine("\n\t\t\t _____\r\n\t\t\t/     \\\r\n\t_______/_______\\_______\r\n\t\\\\______AMIG.OS______//\n");

            login();
        }

         public void login()
        {
            // Login-Auforderung
            Console.WriteLine("Bitte melden Sie sich an.");
            string username;
            string password;

            // Überprüfen, ob der Benutzer existiert
            while (true)
            {
                Console.Write("Benutzername: ");
                username = Console.ReadLine();

                if (!string.IsNullOrEmpty(username) && userManagement.UserExists(username))
                {
                    break; // Abbrechen, wenn der Benutzer existiert
                }
                else
                {
                    Console.WriteLine("Benutzername falsch, bitte versuchen Sie es erneut.");
                }
            }

            // Endlosschleife zur Passwortabfrage
            while (true) // Unendliche Schleife für die Passwortabfrage
            {
                Console.Write("Passwort: ");
                password = GetPassword(); // Ruft das Passwort mit Sternchen-Eingabe ab

                // Überprüfung des Passworts
                if (userManagement.VerifyPassword(username, password))
                {
                    loggedInUser = username; // Speichere den eingeloggenen Benutzer
                    Console.WriteLine("Login erfolgreich!");
                    break; // Bei erfolgreichem Login Schleife verlassen
                }
                else
                {
                    Console.WriteLine("Login fehlgeschlagen, bitte versuchen Sie es erneut.");
                }
            }
            starttime = DateTime.Now;

        }
        
        protected override void Run()
        {
            Console.WriteLine("Input: ");
            var input = Console.ReadLine();
            string[] args = input.Split(' ');

            switch (args[0].ToLower())
            {
                case "help":
                    {
                        Console.WriteLine("for further information contact us");
                        break;
                    }

                case "adios":
                    {
                        if (args.GetLength(0) == 2 && args[1].Equals("amigos"))
                        {
                            Console.WriteLine("\n\tHASTA LA VISTA");
                            Thread.Sleep(1500);
                            Sys.Power.Shutdown();
                        }
                        break;
                    }

                case "showall":
                    {
                        Console.WriteLine("Benutzerinformationen:");
                        userManagement.DisplayAllUsers(); // Benutzerinfo nur für den angemeldeten Benutzer
                        break;
                    }

                case "showme":
                    {
                        Console.WriteLine("Benutzerinformationen:");
                        userManagement.GetUserInfo(loggedInUser); // Benutzerinfo nur für den angemeldeten Benutzer
                        break;
                    }

                case "change":
                    {
                        Console.Write("Neuer Benutzername: ");
                        string newUsername = Console.ReadLine();

                        // Ändere den Benutzernamen
                        if (userManagement.ChangeUsername(loggedInUser, newUsername))
                        {
                            loggedInUser = newUsername; // Aktualisiere den aktuellen Benutzernamen
                        }

                        break;
                    }
                case "logout": 
                    {
                        login();
                        break;
                    }

                case "showtime":
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan current = now - starttime;
                        Console.WriteLine($" Eingeloggte Zeit:{current}");
                        break;
                    }

                case "add":
                    {
                        string username;
                        string role;
                        string pw;
                        string pw_2;

                        Console.WriteLine("Name des anzulegende Benutzers");
                        username = Console.ReadLine();
                        Console.WriteLine("Rolle des anzulegende Benutzers"); //
                        role = Console.ReadLine();

                        //implementation doppelt eingeben
                        Console.WriteLine("PW des anzulegende Benutzers");
                        pw = GetPassword();
                        do{
                        Console.WriteLine("PW des anzulegende Benutzers wiederholen");
                        pw_2 = GetPassword();
                        } while (pw != pw_2);

                        userManagement.AddUser(username, role, pw);
                        break;
                    }

                default:
                    {
                        Console.WriteLine("Eingabe falsch.");
                        break;
                    }
            }
        }
    }
}
