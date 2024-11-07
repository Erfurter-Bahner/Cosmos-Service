﻿using AMIG.OS.Utils;
using AMIG.OS.FileManagement;
using System;
using System.IO;

namespace AMIG.OS.UserSystemManagement
{
    // Zentraler Dienst für Benutzerverwaltung, Rollen- und Berechtigungsmanagement
    public class UserManagement
    {
        private readonly UserRepository userRepository;  // Verwaltet alle Benutzerinformationen
        private readonly AuthenticationService authService;  // Zuständig für Authentifizierung
        private readonly RoleRepository roleRepository;  // Speichert Rollen und Berechtigungen
        private static FileSystemManager fileSystemManager = new FileSystemManager();

        public UserManagement()
        {
            // Initialisiere Repositorys und den Authentifizierungsdienst
            roleRepository = new RoleRepository();
            userRepository = new UserRepository(roleRepository);
            authService = new AuthenticationService(userRepository);
        }

        // Führt die Benutzeranmeldung durch
        public bool Login(string username, string password)
        {
            return authService.Login(username, password);
        }

        // Registriert einen neuen Benutzer mit Benutzernamen, Passwort und zugewiesener Rolle
        public bool Register(string username, string password, string role)
        {
            return authService.Register(username, password, role);
        }

        // Entfernt einen Benutzer aus dem System
        public void RemoveUser(string username)
        {
            userRepository.RemoveUser(username);
        }

        // Fügt einen Benutzer mit einer Rolle und einem Passwort hinzu, falls er noch nicht existiert
        public void AddUserWithRoleAndPassword(string username, string password, string role)
        {
            if (!authService.UserExists(username))
            {
                var user = new User(username, password, isHashed: true); // Neues Benutzerobjekt mit Passwort-Hash erstellen
                userRepository.AddUser(user); // Benutzer zum Repository hinzufügen
                Console.WriteLine($"Benutzer {username} mit der Rolle {role} hinzugefügt.");
            }
            else
            {
                Console.WriteLine("Benutzername existiert bereits.");
            }
        }

        // Entfernt alle Benutzer aus dem System
        public void RemoveAllUser()
        {
            userRepository.RemoveAllUsers();
            Console.WriteLine("Alle Benutzer wurden entfernt.");
        }

        // Gibt Informationen aller Benutzer in der Konsole aus
        public void DisplayAllUsers()
        {
            var users = userRepository.GetAllUsers();
            foreach (var user in users.Values)
            {
                Console.WriteLine($"Username: {user.Username}, " +
                    $"PW: {user.PasswordHash}, " +
                    $"Role: {string.Join(", ", user.Roles)}, " +
                    $"Erstellt am: {user.CreatedAt}, " +
                    $"Letzter Login: {user.LastLogin}");
            }
        }

        // Ändert das Passwort eines Benutzers, sofern das alte Passwort korrekt ist
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return userRepository.ChangePassword(username, oldPassword, newPassword);
        }

        // Gibt das Benutzerobjekt anhand des Benutzernamens zurück, falls vorhanden
        public User GetUser(string username)
        {
            if (userRepository.users.ContainsKey(username))
            {
                return userRepository.users[username];
            }
            else
            {
                Console.WriteLine($"Benutzer '{username}' existiert nicht.");
                return null;
            }
        }

        // Überprüft, ob ein Benutzer im System existiert
        public bool UserExists(string username)
        {
            return authService.UserExists(username);
        }
    }
}
