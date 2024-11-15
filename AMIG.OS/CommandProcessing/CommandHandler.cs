using AMIG.OS.UserSystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMIG.OS.FileManagement;
using AMIG.OS.CommandProcessing.Commands.UserSystem;
using AMIG.OS.Utils;
using AMIG.OS.CommandProcessing.Commands.extra;

namespace AMIG.OS.CommandProcessing
{
    internal class CommandHandler
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
                { "adios", new Adios()}
                // Weitere Befehle hinzufügen ...
            };
        }

        public void ProcessCommand(string input, User currentUser)
        {
            var args = input.Split(' ');
            var commandName = args[0].ToLower();

            if (commands.TryGetValue(commandName, out var command) && command.CanExecute(currentUser))
            {
                command.Execute(args.Skip(1).ToArray(), currentUser); // Übergeben der Parameter an Execute
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
