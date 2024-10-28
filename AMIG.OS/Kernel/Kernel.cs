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
        private UserManagement userManagement = new UserManagement();
        private FileSystemManager fileSystemManager = new FileSystemManager();
        private CommandHandler commandHandler;

        private string loggedInUser;
        DateTime starttime;

        protected override void BeforeRun()
        {
            // Initialisiere das Dateisystem und registriere VFS
            var fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            // Initialisiere CommandHandler mit Abhängigkeiten
            commandHandler = new CommandHandler(userManagement, fileSystemManager);

            Console.WriteLine("Cosmos booted successfully.");
            Console.WriteLine("\n\t\t\t _____\r\n\t\t\t/     \\\r\n\t_______/_______\\_______\r\n\t\\\\______AMIG.OS______//\n");

            //ShowLoginOptions();
        }

        private void ShowLoginOptions()
        {
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

            UserRole role;
            if (!Enum.TryParse(roleInput, true, out role) || !Enum.IsDefined(typeof(UserRole), role))
            {
                Console.WriteLine("Invalid role. Defaulting to Standard.");
                role = UserRole.Standard;
            }

            if (userManagement.Register(username, password, role))
            {
                Console.WriteLine("Registration successful! Please log in.");
            }
            else
            {
                Console.WriteLine("Registration failed. Username may already exist.");
            }
        }

        protected override void Run()
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            commandHandler.ProcessCommand(input, loggedInUser);

            //string currentInput = "";
            //Console.Write("Input: ");

            //while (true)
            //{
            //    var key = Console.ReadKey(intercept: true);

            //    if (key.Key == ConsoleKey.Enter)
            //    {
            //        Console.WriteLine();
            //        if (!string.IsNullOrWhiteSpace(currentInput))
            //        {
            //            commandHistory.Add(currentInput);
            //            historyIndex = -1;

            //            string[] args = currentInput.Split(' ');
            //            ProcessCommand(args);

            //            currentInput = "";
            //            Console.Write("Input: ");
            //        }
            //    }
            //    else if (key.Key == ConsoleKey.Backspace)
            //    {
            //        if (currentInput.Length > 0)
            //        {
            //            currentInput = currentInput[..^1];
            //            ConsoleHelpers.ClearCurrentLine();
            //            Console.Write(currentInput);
            //        }
            //    }
            //    else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow)
            //    {
            //        currentInput = CommandHistory.NavigateHistory(commandHistory, key.Key, ref historyIndex);
            //        ConsoleHelpers.ClearCurrentLine();
            //        Console.Write(currentInput);
            //    }
            //    else if (!char.IsControl(key.KeyChar))
            //    {
            //        currentInput += key.KeyChar;
            //        Console.Write(key.KeyChar);
            //    }
            //}
        }

        //private void ProcessCommand(string[] args)
        //{
        //    switch (args[0].ToLower())
        //    {
        //        case "help":
        //            Console.WriteLine("Use 'help' for more information.");
        //            break;
        //        case "adios":
        //            if (args.Length > 1 && args[1].Equals("amigos"))
        //            {
        //                Console.WriteLine("\n\tHASTA LA VISTA");
        //                Sys.Power.Shutdown();
        //            }
        //            break;
        //        case "showall":
        //        case "showme":
        //            userManagement.HandleUserCommands(args, loggedInUser);
        //            break;
        //        case "mkdir":
        //        case "cd":
        //        case "ls":
        //        case "write":
        //        case "rm":
        //        case "rmdir":
        //        case "touch":
        //        case "cat":
        //            fileSystemManager.HandleFileCommands(args, currentDirectory);
        //            break;
        //        default:
        //            Console.WriteLine("Unbekannter Befehl.");
        //            break;
        //    }
        //}
    }
}
