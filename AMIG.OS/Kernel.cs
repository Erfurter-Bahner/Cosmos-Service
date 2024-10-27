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
using AMIG.OS;


namespace AMIG.OS
{
    public class Kernel : Sys.Kernel
    {

        private static Sys.FileSystem.CosmosVFS fs;
        private static UserManagement userManagement = new UserManagement(); // Singleton für UserManagement
        public FileSystemManager fileSystemManager = new FileSystemManager();
        private string currentDirectory = @"0:\"; // Root-Verzeichnis als Startpunkt
        
        private List<string> commandHistory = new List<string>(); // Liste für Befehle
        private int historyIndex = -1; // Aktuelle Position in der Befehlsliste
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

        public static void ClearCurrentLine()
        {
            // stellt sicher, dass während der Navigation mit Pfeiltasten keine neue Zeile begonnen wird.
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        protected override void Run()
        {
            string currentInput = "";
            Console.Write("Input: "); // Eingabeaufforderung einmal setzen

            // Hauptschleife für die Eingabe
            while (true)
            {
                var key = Console.ReadKey(intercept: true); // Intercept = true, um die Eingabe nicht anzuzeigen

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); // Zeilenumbruch
                    if (!string.IsNullOrWhiteSpace(currentInput))
                    {
                        commandHistory.Add(currentInput); // Füge den Befehl zur Historie hinzu
                        historyIndex = -1; // Zurücksetzen des Index für die Historie

                        // Verarbeite den Befehl
                        string[] args = currentInput.Split(' ');

                        // Befehlsverarbeitung
                        ProcessCommand(args);

                        currentInput = ""; // Zurücksetzen der Eingabe
                        Console.Write("Input: "); // Neue Eingabeaufforderung
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (currentInput.Length > 0)
                    {
                        currentInput = currentInput.Substring(0, currentInput.Length - 1); // Entferne das letzte Zeichen
                        int cursorPos = Console.CursorLeft; // Aktuelle Cursorposition speichern

                        // Cursor eine Stelle zurücksetzen und das Zeichen löschen
                        Console.SetCursorPosition(cursorPos - 1, Console.CursorTop);
                        Console.Write(" "); // Ein Leerzeichen ausgeben, um das Zeichen zu löschen
                        Console.SetCursorPosition(cursorPos - 1, Console.CursorTop); // Cursor zurücksetzen
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    // Navigation in der Befehlsverlaufsliste nach oben
                    if (historyIndex < commandHistory.Count - 1)
                    {
                        if (historyIndex == -1) // Wenn historyIndex -1 ist, von aktueller Eingabe starten
                        {
                            historyIndex = 0; // Setze Index auf 0 für den ersten Befehl
                        }
                        else
                        {
                            historyIndex++; // Gehe zum nächsten Befehl
                        }

                        currentInput = commandHistory[commandHistory.Count - 1 - historyIndex];
                        ClearCurrentLine(); // Lösche die aktuelle Zeile
                        Console.Write(currentInput); // Zeige den aktuellen Befehl an
                        Console.SetCursorPosition(currentInput.Length, Console.CursorTop); // Setze den Cursor an das Ende der Eingabe
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    // Navigation in der Befehlsverlaufsliste nach unten
                    if (historyIndex > 0)
                    {
                        historyIndex--;
                        currentInput = commandHistory[commandHistory.Count - 1 - historyIndex];
                        ClearCurrentLine(); // Lösche die aktuelle Zeile
                        Console.Write(currentInput); // Zeige den aktuellen Befehl an
                        Console.SetCursorPosition(currentInput.Length, Console.CursorTop); // Setze den Cursor an das Ende der Eingabe
                    }
                    else if (historyIndex == 0) // Wenn wir am Anfang der Historie sind
                    {
                        historyIndex = -1; // Setze den Index auf -1 für eine leere Eingabe
                        currentInput = ""; // Leere Eingabe
                        ClearCurrentLine(); // Lösche die aktuelle Zeile
                        Console.Write("Input: "); // Eingabeaufforderung zurücksetzen
                        Console.SetCursorPosition(7, Console.CursorTop); // Setze den Cursor nach "Input: "
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    currentInput += key.KeyChar; // Zeichen zur aktuellen Eingabe hinzufügen
                    Console.Write(key.KeyChar); // Zeichen in der Konsole anzeigen
                }
            }
        }

        // Methode zur Verarbeitung der Befehle
        private void ProcessCommand(string[] args)
        {
            switch (args[0].ToLower())
            {
                case "help":
                    Console.WriteLine("for further information contact us");
                    break;

                case "adios":
                    if (args.Length == 2 && args[1].Equals("amigos"))
                    {
                        Console.WriteLine("\n\tHASTA LA VISTA");
                        Thread.Sleep(1500);
                        Sys.Power.Shutdown();
                    }
                    break;
                // userbefehle
                case "showall":
                    Console.WriteLine("Benutzerinformationen:");
                    userManagement.DisplayAllUsers(); // Benutzerinfo nur für den angemeldeten Benutzer
                    break;

                case "showme":
                    Console.WriteLine("Benutzerinformationen:");
                    userManagement.GetUserInfo(loggedInUser); // Benutzerinfo nur für den angemeldeten Benutzer
                    break;

                case "change":
                    Console.Write("Neuer Benutzername: ");
                    string newUsername = Console.ReadLine();
                    if (userManagement.ChangeUsername(loggedInUser, newUsername))
                    {
                        loggedInUser = newUsername; // Aktualisiere den aktuellen Benutzernamen
                    }
                    break;

                case "logout":
                    login();
                    break;

                case "showtime":
                    TimeSpan current = DateTime.Now - starttime;
                    Console.WriteLine($" Eingeloggte Zeit: {current}");
                    break;

                case "add":
                    AddUserCommand();
                    break;

                case "remove":
                    Console.Write("Benutzername des zu entfernenden Benutzers: ");
                    string username = Console.ReadLine();
                    userManagement.RemoveUser(username);
                    break;

                case "removeall":
                    userManagement.RemoveAllUser();
                    break;

                //datei befehle
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
                        string newDir = Path.Combine(currentDirectory, args[1]);
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

                default:
                    Console.WriteLine("Eingabe falsch.");
                    break;
            }
        }

        // Neue Methode zum Hinzufügen eines Benutzers
        private void AddUserCommand()
        {
            Console.WriteLine("Name des anzulegende Benutzers");
            string username = Console.ReadLine();

            if (userManagement.UserExists(username)){

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

                if (role!="admin" && role != "standard")
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
            string pw = GetPassword();
            string pw_2;
            do
            {
                Console.WriteLine("PW des anzulegende Benutzers wiederholen");
                pw_2 = GetPassword();
            } while (pw != pw_2);

            userManagement.AddUser(username, role, pw);
        }

    }
}


