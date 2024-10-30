using System;
using System.Collections.Generic;
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
        private  static UserManagement userManagement = new UserManagement();
        private  static FileSystemManager fileSystemManager = new FileSystemManager();
        private static Helpers helpers = new Helpers(userManagement);
        private  CommandHandler commandHandler;
        private List<string> commandHistory = new List<string>(); // Liste für Befehle
        private int historyIndex = -1; // Aktuelle Position in der Befehlsliste
        private string loggedInUser;
        DateTime starttime;

        protected override void BeforeRun()
        {
            
            // Initialisiere das Dateisystem und registriere VFS
            fs1 = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs1);
            //var available_space = fs.getavailablefreespace(@"0:\");
            //console.writeline("available free space: " + available_space);

            //var fs_type = fs.getfilesystemtype(@"0:\");
            //console.writeline("file system type: " + fs_type);

            // Initialisiere CommandHandler mit Abhängigkeiten
            commandHandler = new CommandHandler(userManagement, 
                                                fileSystemManager,
                                                ShowLoginOptions,
                                                helpers);

            Console.WriteLine("Cosmos booted successfully.");
            Console.WriteLine("\n\t\t\t _____\r\n\t\t\t/     \\\r\n\t_______/_______\\_______\r\n\t\\\\______AMIG.OS______//\n");

            ShowLoginOptions();
        }

        public void ShowLoginOptions()
        {
            userManagement.DisplayAllUsers(); // nur zum testen
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Select an option: ");
            var option = Console.ReadLine();

            if (option == "1")
            {
                Login();
            }
            else if (option == "2")
            {
                Register();
                ShowLoginOptions(); // Nach der Registrierung erneut Login/Registrierung anzeigen
            }
            else
            {
                Console.WriteLine("Invalid option, try again.");
                ShowLoginOptions();
            }
        }

        private void Login()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = ConsoleHelpers.GetPassword();

            
            if (userManagement.Login(username, password))
            {
                loggedInUser = username;
                commandHandler.SetStartTime(DateTime.Now); // Startzeit setzen
                Console.WriteLine("Login successful!");
                // Systemstart fortsetzen
            }
            else
            {
                Console.WriteLine("Invalid credentials. Try again.");
                ShowLoginOptions();
            }
            
        }
        private void Register()
        {
            Console.Write("Choose a username: ");
            var username = Console.ReadLine();
            Console.Write("Choose a password: ");
            var password = ConsoleHelpers.GetPassword();

            Console.Write("Choose a role (Admin or Standard): ");
            var roleInput = Console.ReadLine();

            /*
            string role;
            if (!Enum.TryParse(roleInput, true, out role) || !Enum.IsDefined(typeof(UserRole), role))
            {
                Console.WriteLine("Invalid role. Defaulting to Standard.");
                role = UserRole.Standard;
            }
            */
            
            if (userManagement.Register(username, password, roleInput))
            {
                Console.WriteLine("Registration successful! Please log in.");
            }
            else
            {
                Console.WriteLine("Registration failed. Username may already exist.");
            }
            
        }
        //mussin utils oder so
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
            Console.Write("Input: ");
            var currentInput = "";
            int cursorPosInInput = 0; // Track cursor position in the current input string

            while (true)
            {
                var key = Console.ReadKey(intercept: true); // Intercept = true to avoid showing the key automatically

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); // Line break
                    if (!string.IsNullOrWhiteSpace(currentInput))
                    {
                        commandHistory.Add(currentInput); // Add the command to history
                        historyIndex = -1; // Reset history index

                        // Process command
                        commandHandler.ProcessCommand(currentInput, loggedInUser);

                        // Reset input and cursor position
                        currentInput = "";
                        cursorPosInInput = 0;
                        Console.Write("Input: ");
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (cursorPosInInput > 0)
                    {
                        // Remove character from currentInput at cursor position
                        currentInput = currentInput.Remove(cursorPosInInput - 1, 1);
                        cursorPosInInput--; // Move cursor position back

                        // Clear the line and re-write the input with updated cursor position
                        ClearCurrentLine();
                        Console.Write("Input: " + currentInput);
                        Console.SetCursorPosition(cursorPosInInput + 7, Console.CursorTop); // Adjust cursor
                    }
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorPosInInput > 0)
                    {
                        cursorPosInInput--; // Move cursor left within the input
                        Console.SetCursorPosition(cursorPosInInput + 7, Console.CursorTop);
                    }
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (cursorPosInInput < currentInput.Length)
                    {
                        cursorPosInInput++; // Move cursor right within the input
                        Console.SetCursorPosition(cursorPosInInput + 7, Console.CursorTop);
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
                    // Insert character at current cursor position within input
                    currentInput = currentInput.Insert(cursorPosInInput, key.KeyChar.ToString());
                    cursorPosInInput++; // Move cursor position forward

                    // Clear line, reprint, and reposition cursor
                    ClearCurrentLine();
                    Console.Write("Input: " + currentInput);
                    Console.SetCursorPosition(cursorPosInInput + 7, Console.CursorTop);
                }
            }
        }
    }
}
