using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using AMIG.OS.FileManagement;
using AMIG.OS.CommandProcessing.Commands.UserSystem;
using AMIG.OS.Utils;
using AMIG.OS.CommandProcessing.Commands.extra;

namespace AMIG.OS.CommandProcessing
{
    public class CommandHandler
    {
        private readonly Dictionary<string, ICommand> commands;

        public CommandHandler(UserManagement userManagement, FileSystemManager fileSystemManager)
        {
            commands = new Dictionary<string, ICommand>
            {
                //{ "login", new LoginCommand(userManagement) },
                //{ "ls", new ListDirectoryCommand(fileSystem) },
                //{ "mkdir", new MakeDirectoryCommand(fileSystem) },
                { "addrole", new AddRole(userManagement) },
                { "rmrole", new RemoveRole(userManagement) },
                { "addroletouser", new AddRoleToUser(userManagement)},
                { "rmroleuser", new RemoveRoleUser(userManagement)},
                { "adios", new Adios()},
                { "logout", new Logout(userManagement)},
                { "addpermtouser", new AddPermToUser(userManagement)},
                { "rmpermuser", new RemovePermUser(userManagement)},
                { "rmpermrole", new RemovePermRole(userManagement)},
                { "addpermtorole", new AddPermToRole(userManagement)},
                { "adduser", new AddUser(userManagement) },
                { "rmuser", new RemoveUser(userManagement) },
                { "datetime", new Commands.extra.DateTime()},
                { "showall", new ShowAll(userManagement)},
                { "showme", new ShowMe(userManagement)},
                { "changename", new ChangeName(userManagement)},
                { "changepw", new ChangePW(userManagement)},
                { "clear", new Clear(userManagement)},
                // Weitere Befehle hinzufügen ...
            };
        }

        public void ProcessCommand(string input, User currentUser)
        {
            var args = input.Split(' ');
            var commandName = args[0].ToLower();

            if (commands.TryGetValue(commandName, out var command) /*&& command.CanExecute(currentUser)*/)
            {
                var parameters = ParseParameters(args.Skip(1).ToArray());
                command.Execute(parameters, currentUser);
                // Übergeben der Parameter an Execute
                // command: zusatz1 zusatz2
            }
            else if (commandName == "-help")
            {
                ShowAllCommandsHelp();
            }
            else
            {
                Console.WriteLine("Unbekannter Befehl oder fehlende Berechtigung.");
            }
        }
        private CommandParameters ParseParameters(string[] args)
        {
            var parameters = new CommandParameters();
            string currentKey = null;
            var currentValues = new HashSet<string>();

            if (args == null || args.Length == 0)
            {
                //Console.WriteLine("Keine Argumente übergeben.");
                return parameters;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(args[i]))
                {
                    //Console.WriteLine($"Leeres oder ungültiges Argument bei Index {i} übersprungen.");
                    continue; // Überspringe ungültige oder leere Argumente
                }

                //Console.WriteLine($"Verarbeite Argument {i}: {args[i]}");

                // Wenn das Argument mit einem Bindestrich beginnt, dann ist es ein Schlüssel
                if (args[i].StartsWith("-"))
                {
                    //Console.WriteLine("inside parseparam0");

                    // Speichere vorherigen Schlüssel und Werte
                    if (currentKey != null)
                    {
                        //Console.WriteLine($"Speichere Schlüssel: {currentKey}");
                        parameters.AddParameter(currentKey, string.Join(" ", currentValues));
                        currentValues.Clear();
                    }

                    // Setze den neuen Schlüssel
                    currentKey = args[i].TrimStart('-');
                    //Console.WriteLine($"Neuer Schlüssel gesetzt: {currentKey}");
                }
                else
                {
                    // Füge den aktuellen Wert der Liste hinzu
                    //Console.WriteLine($"Hinzufügen des Werts: {args[i]} zum Schlüssel: {currentKey}");
                    currentValues.Add(args[i]);
                }
            }

            // Letzten Parameter und Werte speichern
            if (currentKey != null)
            {
                //Console.WriteLine($"Speichere letzten Schlüssel: {currentKey}");
                parameters.AddParameter(currentKey, string.Join(" ", currentValues));
            }
            else
            {
                Console.WriteLine("Es wurde kein gültiger Schlüssel erkannt.");
            }

            return parameters;
        }



        private void ShowAllCommandsHelp()
        {
            Console.WriteLine("Available commands:");
            foreach (var cmd in commands)
            {
                Console.WriteLine($"\n{cmd.Key}");
                cmd.Value.ShowHelp();
            }
        }
    }
}
