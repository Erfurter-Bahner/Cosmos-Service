using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using Sys = Cosmos.System;
using System.Security.Cryptography;
using System.Drawing;

namespace AMIG.OS
{
    public class Kernel : Sys.Kernel
    {
        private UserManagement userManagement; // Benutzerverwaltungsinstanz
         string loggedInUser; // Aktuell eingeloggter Benutzer

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
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");

            userManagement = new UserManagement(); // Benutzerverwaltung instanziieren

            // Benutzer hinzufügen (mit Passwort)
            userManagement.AddUser("User1", "Standard", "password123");
            userManagement.AddUser("User2", "Admin", "adminPass");

            // Login-Anforderung
            Console.WriteLine("Bitte melden Sie sich an.");
            string username;
            string password;

            // Überprüfen, ob der Benutzer existiert
            while (true)
            {
                Console.Write("Benutzername: ");
                username = Console.ReadLine();

                if (userManagement.UserExists(username))
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
        }

        protected override void Run()
        {
            
                Console.WriteLine("Input: ");
                var input = Console.ReadLine();
                string[] args = input.Split(' ');


            switch (args[0].ToLower())
            {
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
                        BeforeRun();
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
