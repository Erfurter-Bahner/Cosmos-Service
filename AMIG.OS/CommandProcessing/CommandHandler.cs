﻿using System;
using AMIG.OS.UserSystemManagement;
using AMIG.OS.FileManagement;
using AMIG.OS.Utils;
using System.IO;
using Sys = Cosmos.System;
using System.Threading;
using System.Data;
using System.Security;


namespace AMIG.OS.CommandProcessing
{
    public class CommandHandler
    {
        private readonly UserManagement userManagement;
        private readonly FileSystemManager fileSystemManager;
        private readonly Helpers helpers;
        private DateTime starttime;
        private string currentDirectory = @"0:\"; // Root-Verzeichnis als Startpunkt
        private readonly Action showLoginOptions;
        private readonly Sys.FileSystem.CosmosVFS vfs; //damit freier speicherplatz angezeigt werden kann

        public CommandHandler
            (UserManagement userMgmt, 
            FileSystemManager fsManager, 
            Action showLoginOptionsDelegate,
            Helpers _helpers,
            Sys.FileSystem.CosmosVFS vfs)
        {
            userManagement = userMgmt;
            fileSystemManager = fsManager;
            showLoginOptions = showLoginOptionsDelegate; // Delegate speichern
            helpers = _helpers;
            this.vfs = vfs; 
        }

        public void SetStartTime(DateTime loginTime)
        {
            starttime = loginTime;
        }

        public void ProcessCommand(string input, string loggedInUser)
        {
            var args = input.Split(' ');
            var currentUser = userManagement.GetUser(loggedInUser);
            bool admin_true = userManagement.GetUserRole(loggedInUser).ToLower() == "admin";
            string role = userManagement.GetUserRole(loggedInUser).ToLower();
            switch (args[0].ToLower())
            {
                case "userperm":
                    // Überprüfen, ob die erforderlichen Argumente übergeben wurden
                    if (args.Length >= 3) // args[0] ist der Befehl, args[1] der Benutzername, args[2] die Berechtigung, args[3] der Wert
                    {
                        string targetUsername = args[1]; // Benutzername, dessen Berechtigung gesetzt werden soll
                        string permission = args[2]; // Die Berechtigung, die gesetzt werden soll
                        string value = args[3]; // Der Wert, der gesetzt werden soll (z.B. "true" oder "false")

                        // Berechtigungen setzen
                        userManagement.SetUserPermission(loggedInUser, targetUsername, permission, value);
                    }
                    else
                    {
                        Console.WriteLine("Ungültige Eingabe. Verwenden Sie: userperm <Benutzername> <Berechtigung> <Wert>");
                    }
                    break;


                case "datetime":
                    Console.WriteLine(DateTime.Now);
                    break;

                case "adios":
                    helpers.AdiosHelper(args);
                    break;

                // Benutzerbefehle
                case "showall": //Admin
                    if (currentUser.HasPermission("ShowAllUsers")) // Berechtigungsprüfung
                    {
                        helpers.ShowAllHelper(true); // Admin hat die Berechtigung
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung für diesen Befehl.");
                    }
                    break;

                case "showme": //Both
                    helpers.ShowMeHelper(loggedInUser);
                    break;

                case "remove": //Admin
                    if (currentUser.HasPermission("RemoveUser")) // Berechtigungsprüfung
                    {
                        helpers.RemoveHelper(true); // Admin hat die Berechtigung
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung für diesen Befehl.");
                    }
                    break;

                case "removeall": //Admin
                    if (currentUser.HasPermission("RemoveAllUsers")) // Berechtigungsprüfung
                    {
                        userManagement.RemoveAllUser();
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung für diesen Befehl.");
                    }
                    break;

                case "changename": // Both
                    if (currentUser.HasPermission("ChangeName")) // Berechtigungsprüfung
                    {
                        helpers.ChangeNameHelper(loggedInUser);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um den Namen zu ändern.");
                    }
                    break;

                case "changepw": //both
                    if (currentUser.HasPermission("ChangePassword")) // Berechtigungsprüfung
                    {
                        helpers.ChangePasswortHelper(loggedInUser);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um das Passwort zu ändern.");
                    }
                    break;

                case "showtime": //both
                    TimeSpan current = DateTime.Now - starttime;
                    Console.WriteLine($" Eingeloggte Zeit: {current}");
                    break;

                case "logout": //both
                    showLoginOptions.Invoke();
                    break;

                case "add": //admin
                    if (currentUser.HasPermission("CreateUser")) // Berechtigungsprüfung
                    {
                        helpers.addHelper(true); // Admin hat die Berechtigung
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um einen Benutzer hinzuzufügen.");
                    }
                    break;

                // Datei- und Verzeichnisbefehle
                case "mkdir": //admin
                    if (currentUser.HasPermission("CreateDirectory")) // Berechtigungsprüfung
                    {
                        helpers.mkdirHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um ein Verzeichnis zu erstellen.");
                    }
                    break;

                case "cd": //both             
                    helpers.cdHelper(args, ref currentDirectory);
                    break;

                case "ls": //both                    
                    helpers.lsHelper(args, currentDirectory);
                    break;

                case "write": //admin
                    if (currentUser.HasPermission("WriteToFile")) // Berechtigungsprüfung
                    {
                        helpers.writeHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um in die Datei zu schreiben.");
                    }
                    break;

                case "rm": //admin
                    if (currentUser.HasPermission("RemoveFile")) // Berechtigungsprüfung
                    {
                        helpers.rmHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um die Datei zu entfernen.");
                    }
                    break;

                case "rmdir": //admin
                    if (currentUser.HasPermission("RemoveDirectory")) // Berechtigungsprüfung
                    {
                        helpers.rmdirHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um das Verzeichnis zu entfernen.");
                    }
                    break;

                case "touch": //admin
                    if (currentUser.HasPermission("CreateFile")) // Berechtigungsprüfung
                    {
                        helpers.touchHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um die Datei zu erstellen.");
                    }
                    break;

                case "cat": //both
                    helpers.catHelper(args, currentDirectory, role);
                    break;

                case "space": //both
                    var availableSpace = vfs.GetAvailableFreeSpace(@"0:\");
                    Console.WriteLine($"Verfügbarer Speicherplatz: {availableSpace} Bytes");
                    break;

                case "setperm": //admin
                    if (currentUser.HasPermission("SetUserPermission")) // Berechtigungsprüfung
                    {
                        helpers.setpermHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um Berechtigungen zu setzen.");
                    }
                    break;

                case "unlock": //admin
                    if (currentUser.HasPermission("UnlockUser")) // Berechtigungsprüfung
                    {
                        helpers.unlockHelper(true, args, currentDirectory);
                    }
                    else
                    {
                        Console.WriteLine("Keine Berechtigung, um den Benutzer zu entsperren.");
                    }
                    break;

                // Beispiel für andere Befehle
                case "help": //both
                    Console.WriteLine("Available commands: showall, adduser, removeuser, mkdir, rmdir, touch, rm, cat.");
                    break;

                default: //both
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
