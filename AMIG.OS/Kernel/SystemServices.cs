
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.Linq;
using AMIG.OS.Utils;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.FileManagement;
using AMIG.OS.CommandProcessing;

namespace AMIG.OS.Kernel
{
    public class SystemServices
    {
        private List<string> commandHistory = new List<string>(); // Liste für Befehle
        private int historyIndex = -1; // Aktuelle Position in der Befehlsliste
        CommandHandler commandHandler;
        UserManagement userManagement;
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
            var currentInput = "";
            int cursorPosInInput = 0;
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
                        commandHandler.ProcessCommand(currentInput, userManagement.loginManager.LoggedInUser);

                        // Reset input and cursor position
                        currentInput = "";
                        cursorPosInInput = 0;
                        Console.Write(Helper.preInput);
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
                        Console.Write(Helper.preInput + currentInput);
                        Console.SetCursorPosition(cursorPosInInput + Helper.preInput.Length, Console.CursorTop); // Adjust cursor
                    }
                }

                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorPosInInput > 0)
                    {
                        cursorPosInInput--; // Move cursor left within the input
                        Console.SetCursorPosition(cursorPosInInput + Helper.preInput.Length, Console.CursorTop);
                    }
                }

                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (cursorPosInInput < currentInput.Length)
                    {
                        cursorPosInInput++; // Move cursor right within the input
                        Console.SetCursorPosition(cursorPosInInput + Helper.preInput.Length, Console.CursorTop);
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
                        Console.Write(Helper.preInput);
                        Console.Write(currentInput); // Zeige den aktuellen Befehl an
                        cursorPosInInput = (currentInput.Length);
                        Console.SetCursorPosition(currentInput.Length + Helper.preInput.Length, Console.CursorTop); // Setze den Cursor an das Ende der Eingabe
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
                        Console.Write(Helper.preInput);
                        Console.Write(currentInput); // Zeige den aktuellen Befehl an
                        cursorPosInInput = (currentInput.Length);
                        Console.SetCursorPosition(currentInput.Length + Helper.preInput.Length, Console.CursorTop); // Setze den Cursor an das Ende der Eingabe
                    }
                    else if (historyIndex == 0) // Wenn wir am Anfang der Historie sind
                    {
                        historyIndex = -1; // Setze den Index auf -1 für eine leere Eingabe
                        currentInput = ""; // Leere Eingabe
                        ClearCurrentLine(); // Lösche die aktuelle Zeile
                        Console.Write(Helper.preInput); // Eingabeaufforderung zurücksetzen
                        cursorPosInInput = (currentInput.Length);
                        Console.SetCursorPosition(Helper.preInput.Length, Console.CursorTop); // Setze den Cursor nach "Input: "
                    }
                }

                else if (!char.IsControl(key.KeyChar))
                {
                    // Insert character at current cursor position within input
                    currentInput = currentInput.Insert(cursorPosInInput, key.KeyChar.ToString());
                    cursorPosInInput++; // Move cursor position forward

                    // Clear line, reprint, and reposition cursor
                    ClearCurrentLine();
                    Console.Write(Helper.preInput + currentInput);
                    Console.SetCursorPosition(cursorPosInInput + Helper.preInput.Length, Console.CursorTop);
                }
            }
        }
    }
}