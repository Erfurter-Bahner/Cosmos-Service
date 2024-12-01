using System;
using System.Collections.Generic;
using AMIG.OS.Utils;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.CommandProcessing;


namespace AMIG.OS.Kernel
{
    public class SystemServices
    {
        
        CommandHandler commandHandler;
        UserManagement userManagement;
        private int cursorPosition = 0; // Position des Cursors
        private int historyIndex = -1; // Aktuelle Position in der Befehlsliste

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
                                CommandHistory.commandHistory.Add(currentInput);
                                ClearCurrentLine();
                                commandHandler.ProcessCommand(currentInput.Trim(), userManagement.loginManager.LoggedInUser);
                                historyIndex = CommandHistory.commandHistory.Count;
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
                            if (CommandHistory.commandHistory.Count > 0) // Nur wenn die Historie Eintr�ge hat
                            {
                                if (historyIndex > 0) // Bewege den Index nur, wenn er > 0 ist
                                {
                                    historyIndex--;
                                }
                                currentInput = CommandHistory.commandHistory[historyIndex]; // Hole den Befehl aus der Historie
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
                            if (CommandHistory.commandHistory.Count > 0) // Nur wenn die Historie Eintr�ge hat
                            {
                                if (historyIndex < CommandHistory.commandHistory.Count - 1) // Wenn wir nicht am Ende der Liste sind
                                {
                                    historyIndex++;
                                    currentInput = CommandHistory.commandHistory[historyIndex]; // Hole den n�chsten Befehl
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
                                int maxInputLength = Console.WindowWidth - Helper.preInput.Length - 1;

                                if (currentInput.Length < maxInputLength)
                                {
                                    currentInput = currentInput.Insert(cursorPosition, key.KeyChar.ToString());
                                    cursorPosition++;
                                    ClearCurrentLine();

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(Helper.preInput);
                                    Console.ResetColor();
                                    Console.Write(currentInput);
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
