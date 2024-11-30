using System;
using System.Collections.Generic;
using AMIG.OS.Utils;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.CommandProcessing;

namespace AMIG.OS.Kernel
{
    public class SystemServices
    {
        private List<string> commandHistory = new List<string>(); // Liste f�r Befehle
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
            // stellt sicher, dass w�hrend der Navigation mit Pfeiltasten keine neue Zeile begonnen wird.
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
                cursorPosition = 0; // Reset Cursorposition f�r jede neue Eingabe
                while (true)
                {
                    var key = Console.ReadKey(intercept: true); // Eingabeerkennung f�r Pfeiltasten und Backspace
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

                                // Setze die Farbe f�r die Eingabe
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

                        case ConsoleKey.UpArrow:
                            if (commandHistory.Count > 0) // Nur wenn die Historie Eintr�ge hat
                            {
                                if (historyIndex > 0) // Bewege den Index nur, wenn er > 0 ist
                                {
                                    historyIndex--;
                                }
                                currentInput = commandHistory[historyIndex]; // Hole den Befehl aus der Historie
                                cursorPosition = currentInput.Length; // Setze den Cursor ans Ende des Befehls
                                ClearCurrentLine();

                                // Schreibe Pr�fix und den aktuellen Befehl
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Helper.preInput);
                                Console.ResetColor();
                                Console.Write(currentInput);
                            }
                            break;


                        case ConsoleKey.DownArrow:
                            if (commandHistory.Count > 0) // Nur wenn die Historie Eintr�ge hat
                            {
                                if (historyIndex < commandHistory.Count - 1) // Wenn wir nicht am Ende der Liste sind
                                {
                                    historyIndex++;
                                    currentInput = commandHistory[historyIndex]; // Hole den n�chsten Befehl
                                }
                                //else // Wenn wir am Ende sind, leere den Input
                                //{
                                //    currentInput = "";
                                //}

                                cursorPosition = currentInput.Length; // Setze den Cursor ans Ende
                                ClearCurrentLine();

                                // Schreibe Pr�fix und den aktuellen Befehl (oder leere Zeile)
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(Helper.preInput);
                                Console.ResetColor();
                                Console.Write(currentInput);
                            }
                            break;


                        default:
                            if (key.KeyChar > 31) // F�r alle druckbaren Zeichen
                            {
                                // Berechne die maximale erlaubte Eingabel�nge basierend auf der Konsolenbreite
                                int maxInputLength = Console.WindowWidth - Helper.preInput.Length - 1;

                                if (currentInput.Length < maxInputLength) // Eingabe nur zulassen, wenn Platz vorhanden
                                {
                                    currentInput = currentInput.Insert(cursorPosition, key.KeyChar.ToString());
                                    cursorPosition++; // Cursor nach rechts verschieben
                                    ClearCurrentLine();

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(Helper.preInput); // Schreibe das Prefix
                                    Console.ResetColor();

                                    // Teile den aktuellen Input in Hauptbefehl und Argumente
                                    string[] inputParts = currentInput.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                                    string commandPart = inputParts.Length > 0 ? inputParts[0] : "";
                                    string argumentPart = inputParts.Length > 1 ? inputParts[1] : "";

                                    // Pr�fen, ob der Befehl g�ltig ist
                                    if (!string.IsNullOrWhiteSpace(commandPart) &&
                                        commandPart != "extra" &&
                                        (Helper.SpecialCommands.Contains(commandPart.Trim()) || userManagement.loginManager.LoggedInUser.HasPermission(commandPart.Trim())))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green; // G�ltiger Befehl: Gr�n
                                    }
                                    else if(commandPart=="-help")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow; 
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red; // Ung�ltig: Rot
                                    }

                                    Console.Write(commandPart);

                                    // Schreibe die Argumente (ohne Pr�fung)
                                    Console.ResetColor();
                                    if (!string.IsNullOrEmpty(argumentPart))
                                    {
                                        Console.Write(" " + argumentPart);
                                    }

                                    // Setze den Cursor an die richtige Position
                                    Console.SetCursorPosition(Helper.preInput.Length + cursorPosition, Console.CursorTop);
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
