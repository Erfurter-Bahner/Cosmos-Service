using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using AMIG.OS.FileManagement;
using AMIG.OS.CommandProcessing.Commands.UserSystem;
using AMIG.OS.Utils;
using AMIG.OS.CommandProcessing.Commands.Extra;
using AMIG.OS.CommandProcessing.Commands.FileSystem;

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
                { "addrole", new AddRole(userManagement) }, //addrole
                { "rmrole", new RemoveRole(userManagement) }, //rmrole
                { "addroletouser", new AddRoleToUser(userManagement)},//addroletouser
                { "rmroleuser", new RemoveRoleUser(userManagement)}, //rmroleuser               
                { "addpermtouser", new AddPermToUser(userManagement)},//addpermtouser
                { "rmpermuser", new RemovePermUser(userManagement)},//rmpermuser
                { "rmpermrole", new RemovePermRole(userManagement)},//
                { "addpermtorole", new AddPermToRole(userManagement)},
                { "adduser", new AddUser(userManagement) },//
                { "rmuser", new RemoveUser(userManagement) },//              
                { "showall", new ShowAll(userManagement)},//
                { "showme", new ShowMe(userManagement)},//
                { "showuser", new ShowSpecificUser(userManagement)},//
                { "changename", new ChangeName(userManagement)},//
                { "changepw", new ChangePW(userManagement)},//
                //extra
                { "adios", new Adios(userManagement)}, //extra
                { "logout", new Logout(userManagement)}, //extra
                { "datetime", new Commands.Extra.DateTime()}, //extra
                { "clear", new Clear(userManagement)}, //extra
                // Weitere Befehle hinzufügen ...
                { "ls", new LS(fileSystemManager)},
                { "cat", new CAT(fileSystemManager)},
                { "touch", new TOUCH(fileSystemManager)},
                { "cd", new CD(fileSystemManager)},
                { "write", new WRITE(fileSystemManager)},
                { "rmfile", new RemoveFile(fileSystemManager)},
                { "rmdir", new RemoveDir(fileSystemManager)},
                { "mkdir", new MakeDir(fileSystemManager)},
            };
        }

        public void ProcessCommand(string input, User currentUser)
        {
            var args = input.Split(' ');
            var commandName = args[0].ToLower();

            if (commands.TryGetValue(commandName, out var command) && command.CanExecute(currentUser))
            {
                var parameters = ParseParameters(args.Skip(1).ToArray());
                command.Execute(parameters, currentUser);
                // Übergeben der Parameter an Execute
                // command: zusatz1 zusatz2
            }
            else if (commandName == "-help")
            {
                Console.WriteLine("Commands: Use command -help for more information");
                CommandHelp.ShowAllCommandsHelp();
            }
            else
            {
                ConsoleHelpers.WriteError("Unknown command or missing permission.");
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
                ConsoleHelpers.WriteError("No valid key detected.");
            }

            return parameters;
        }
        //private void ShowAllCommandsHelp()
        //{
        //    Console.WriteLine("Available commands:");
        //    foreach (var cmd in commands)
        //    {
        //        Console.WriteLine($"\n{cmd.Key}");
        //        cmd.Value.ShowHelp();
        //    }
        //}
    }
}
