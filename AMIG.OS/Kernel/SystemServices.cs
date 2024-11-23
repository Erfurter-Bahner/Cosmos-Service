using System;
using System.Collections.Generic;
using AMIG.OS.Utils;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.CommandProcessing;

namespace AMIG.OS.Kernel
{
    public class SystemServices
    {
        private List<string> commandHistory = new List<string>(); // Liste für Befehle
        private int historyIndex = -1; // Aktuelle Position in der Befehlsliste
        CommandHandler commandHandler;
        UserManagement userManagement;
        private int cursorPosition = 0; // Position des Cursors

        public SystemServices(CommandHandler commandHandler, UserManagement userManagement)
        {
            this.userManagement = userManagement;
            this.commandHandler = commandHandler;
        }

        public void ClearCurrentLine()
        {
            // stellt sicher, dass während der Navigation mit Pfeiltasten keine neue Zeile begonnen wird.
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public void inputs()
        {
            while (true)
            {
                var currentInput = "";
                cursorPosition = 0; // Reset Cursorposition für jede neue Eingabe
                while (true)
                {
                    var key = Console.ReadKey(intercept: true); // Eingabeerkennung für Pfeiltasten und Backspace
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            Console.WriteLine();
                            if (!string.IsNullOrWhiteSpace(currentInput))
                            {
                                commandHistory.Add(currentInput);
                                ClearCurrentLine();
                                commandHandler.ProcessCommand(currentInput.Trim(), userManagement.loginManager.LoggedInUser);
                                historyIndex = commandHistory.Count;
                            }
                            break;

                        case ConsoleKey.Backspace:
                            if (currentInput.Length > 0 && cursorPosition > 0)
                            {
                                currentInput = currentInput.Substring(0, cursorPosition - 1) + currentInput.Substring(cursorPosition);
                                cursorPosition--; // Cursor nach links verschieben
                                ClearCurrentLine();

                                // Setze die Farbe für die Eingabe
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Helper.preInput); // Schreibe das Prefix
                                Console.ResetColor();

                                // Schreibe den eigentlichen Input in der Standardfarbe
                                Console.Write(currentInput);

                                // Setze den Cursor an die korrekte Position
                                Console.SetCursorPosition(Helper.preInput.Length + cursorPosition, Console.CursorTop);
                            }
                            break;


                        case ConsoleKey.LeftArrow:
                            if (cursorPosition > 0)
                            {
                                cursorPosition--; // Cursor nach links bewegen
                                Console.SetCursorPosition(Helper.preInput.Length + cursorPosition, Console.CursorTop);
                            }
                            break;

                        case ConsoleKey.RightArrow:
                            if (cursorPosition < currentInput.Length)
                            {
                                cursorPosition++; // Cursor nach rechts bewegen
                                Console.SetCursorPosition(Helper.preInput.Length + cursorPosition, Console.CursorTop);
                            }
                            break;

                        case ConsoleKey.UpArrow: // schreibt letzten command 
                            if (historyIndex > 0)
                            {
                                historyIndex--;
                                currentInput = commandHistory[historyIndex]; // Fetch command from history
                                cursorPosition = currentInput.Length; // Setze den Cursor ans Ende des Befehls
                                ClearCurrentLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Helper.preInput); // Schreibe das Prefix
                                Console.ResetColor();
                                Console.Write(currentInput);
                            }
                            break;

                        case ConsoleKey.DownArrow: // schreibt nächsten command
                            if (historyIndex < commandHistory.Count - 1)
                            {
                                historyIndex++;
                                currentInput = commandHistory[historyIndex]; // Fetch next command from history
                                cursorPosition = currentInput.Length; // Setze den Cursor ans Ende des Befehls
                                ClearCurrentLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Helper.preInput); // Schreibe das Prefix
                                Console.ResetColor();
                                Console.Write(currentInput);
                            }
                            else
                            {
                                currentInput = ""; // Keine weiteren Befehle, Eingabe leeren
                                cursorPosition = 0;
                                ClearCurrentLine();
                            }
                            break;

                        default:
                            if (key.KeyChar > 31) // Für alle Zeichen, die gedrückt werden (außer Steuerzeichen)
                            {
                                // Berechne die maximale erlaubte Eingabelänge basierend auf der Konsolenbreite
                                int maxInputLength = Console.WindowWidth - Helper.preInput.Length - 1;

                                if (currentInput.Length < maxInputLength) // Eingabe nur zulassen, wenn Platz vorhanden
                                {
                                    currentInput = currentInput.Insert(cursorPosition, key.KeyChar.ToString());
                                    cursorPosition++; // Cursor nach rechts verschieben
                                    ClearCurrentLine();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(Helper.preInput); // Schreibe das Prefix
                                    Console.ResetColor();
                                    Console.Write(currentInput);
                                    Console.SetCursorPosition(Helper.preInput.Length + cursorPosition, Console.CursorTop); // Setze den Cursor an die richtige Position
                                }
                            }
                            break;
                    }

                    if (key.Key == ConsoleKey.Enter) // Endet die Schleife nach einem "Enter"
                    {
                        ClearCurrentLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(Helper.preInput); // Schreibe das Prefix
                        Console.ResetColor();
                        break;
                    }
                }
            }
        }
    }
}
