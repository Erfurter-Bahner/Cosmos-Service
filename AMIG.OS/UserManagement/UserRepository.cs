using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AMIG.OS.UserSystemManagement
{
    public class UserRepository
    {
        private List<User> users = new List<User>();
        private readonly string dataFilePath = "users.json"; // Pfad zur Benutzerdaten-Datei

        public UserRepository()
        {
            LoadUsers(); // Lädt Benutzer beim Erstellen der Klasse
        }

        public void LoadUsers()
        {
            if (File.Exists(dataFilePath))
            {
                try
                {
                    string jsonData = File.ReadAllText(dataFilePath);
                    users = JsonSerializer.Deserialize<List<User>>(jsonData) ?? new List<User>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading users: {ex.Message}");
                    users = new List<User>(); // Fallback auf leere Liste bei Fehler
                }
            }
        }

        public void SaveUsers()
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dataFilePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        public User GetUserByUsername(string username)
        {
            return users.Find(u => u.Username == username);
        }

        public void AddUser(User user)
        {
            if (GetUserByUsername(user.Username) == null)
            {
                users.Add(user);
                SaveUsers(); // Speichere die Liste nach dem Hinzufügen
            }
            else
            {
                Console.WriteLine("User already exists.");
            }
        }

        public void RemoveUser(string username)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                users.Remove(user);
                SaveUsers(); // Speichere die Liste nach dem Entfernen
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        public List<User> GetAllUsers()
        {
            return users;
        }
    }
}
