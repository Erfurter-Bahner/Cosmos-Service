using System;
using System.Collections.Generic;
using System.IO;

namespace AMIG.OS.UserSystemManagement
{
    public class UserRepository
    {
        public Dictionary<string, User> users = new Dictionary<string, User>();
        private readonly string dataFilePath = @"0:\users.txt"; // Pfad zur Benutzerdaten-Datei


        public UserRepository()
        {
            InitializeTestUsers();
            LoadUsers(); // Lädt Benutzer beim Erstellen der Klasse
        }

        public void SaveUsers()
        {
            try
            {
                // Überprüfe, ob die Datei bereits existiert und lösche sie ggf.
                if (File.Exists(dataFilePath))
                {
                    File.Delete(dataFilePath);
                }

                // Erstellen und Schreiben in die Datei
                using (var stream = File.Create(dataFilePath))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var user in users)
                    {
                        var userInfo = user.Value;
                        writer.WriteLine($"{user.Key},{userInfo.PasswordHash},{userInfo.Role}");
                    }
                }
                Console.WriteLine("Benutzerdaten erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Benutzer: {ex.Message}");
            }
        }

        //fileio
        public void LoadUsers()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    using (var stream = File.OpenRead(dataFilePath))
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split(',');
                            if (parts.Length == 3)
                            {
                                string username = parts[0];
                                string password = parts[1];
                                string role = parts[2];
                                
                                users[username] = new User(username,password,role );
                            }
                        }
                    }
                    Console.WriteLine("Benutzerdaten erfolgreich geladen.");
                }
                else
                {
                    Console.WriteLine("Benutzerdaten-Datei existiert nicht.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Benutzer: {ex.Message}");
            }
        }
        public void InitializeTestUsers()
        {
            if (!File.Exists(dataFilePath))  // Prüfen, ob die Datei existiert
            {
                Console.WriteLine("Erstelle Testbenutzer...");

                // Testbenutzer hinzufügen
                users.Add("User1", new User("User1", "password123", "Standard"));
                users.Add("User2", new User("User2", "adminPass", "Admin"));

                // Speichern der Benutzer in die Datei
                SaveUsers();
            }
            else
            {
                Console.WriteLine("Benutzerdaten-Datei existiert bereits. Überspringe Testbenutzer-Erstellung.");
            }
        }

        public User GetUserByUsername(string username)
        {
            users.TryGetValue(username, out User user);
            return user;
        }

        public void AddUser(User user)
        {
            if (!users.ContainsKey(user.Username))
            {
                users[user.Username] = user;
                SaveUsers(); // Speichere die Liste nach dem Hinzufügen
            }
            else
            {
                Console.WriteLine("User already exists.");
            }
        }

        public void RemoveUser(string username)
        {
            if (users.Remove(username))
            {
                SaveUsers(); // Speichere die Liste nach dem Entfernen
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        public void RemoveAllUsers()
        {
            users.Clear();
        }


        public Dictionary<string, User> GetAllUsers()
        {
            return users;
        }
    }
}
