using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AMIG.OS.Utils;

namespace AMIG.OS.FileManagement
{
    public class FileSystemManager
    {
        // Das aktuelle Verzeichnis
        public string CurrentDirectory { get; set; } = @"0:\";
        private Dictionary<string, string> filePermissions = new Dictionary<string, string>();
        private const string permissionFilePath = @"0:\filePermissions.txt";

        public FileSystemManager()
        {
            LoadPermissionsFromFile();
        }

        private void LoadPermissionsFromFile()
        {
            try
            {
                if (File.Exists(permissionFilePath))
                {
                    using (var stream = File.OpenRead(permissionFilePath))
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split(',');
                            if (parts.Length == 2)
                            {
                                string filePath = parts[0];
                                string permission = parts[1];
                                filePermissions[filePath] = permission;
                            }
                        }

                    }
                    // Nach dem Laden der Berechtigungen ausgeben
                    PrintPermissions();
                    ConsoleHelpers.WriteSuccess("Permissions loaded successfully.");
                }
                else
                {
                    ConsoleHelpers.WriteError("Error: Permission file does not exist.");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Loading Permissions: {ex.Message}");
            }
        }

        public void PrintPermissions()
        {
            Console.WriteLine("Aktueller Inhalt des filePermissions Dictionaries:");

            if (filePermissions == null || filePermissions.Count == 0)
            {
                Console.WriteLine("Das Dictionary ist leer oder nicht initialisiert.");
                return;
            }

            foreach (var kvp in filePermissions)
            {
                Console.WriteLine($"Dateipfad: {kvp.Key}, Berechtigung: {kvp.Value}");
            }
        }

        public void SavePermissionsToFile()
        {
            try
            {
                Console.WriteLine($"Saving Permissions in file: {permissionFilePath}");

                // Überprüfen, ob die Datei bereits existiert und ggf. löschen
                if (File.Exists(permissionFilePath))
                {
                    File.Delete(permissionFilePath);
                }

                // Datei erstellen und Berechtigungen speichern
                using (var stream = File.Create(permissionFilePath))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var kvp in filePermissions)
                    {
                        writer.WriteLine($"{kvp.Key},{kvp.Value}");
                    }
                }

                // Überprüfen, ob die Datei erfolgreich gespeichert wurde
                if (File.Exists(permissionFilePath))
                {
                    ConsoleHelpers.WriteSuccess("File saved successfully.");
                }
                else
                {
                    ConsoleHelpers.WriteError("Error: File wasnt saved.");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Saving file: {ex.Message}");
            }
        }

        public void SetPermission(string filePath, string permission)
        {
            filePermissions[filePath] = permission;
            SavePermissionsToFile();
        }

        // Methode, um Berechtigungen für eine bestimmte Datei zu überprüfen
        public bool HasPermission(string filePath, string userRole)
        {
            // Überprüfe, ob die Datei existiert
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Die Datei wurde vom Admin gelöscht oder existiert nicht.");
                return false;
            }

            // Überprüfen, ob Berechtigungen im Dictionary geladen sind
            if (filePermissions != null && filePermissions.Count > 0)
            {
                Console.WriteLine("Aktuelle Berechtigungen im Dictionary:");
                foreach (var kvp in filePermissions)
                {
                    Console.WriteLine($"Datei: {kvp.Key}, Berechtigung: {kvp.Value}");
                }

                // Prüfen, ob die Berechtigung für den gegebenen filePath existiert
                Console.WriteLine($"der aktuelle filepath {filePath}");
                
                if (filePermissions.ContainsKey(filePath))
                {
                    string permission = filePermissions[filePath];
                    Console.WriteLine($"Berechtigung für '{filePath}': {permission}");

                    // Wenn die Datei als "locked" markiert ist, verweigern wir den Zugriff für alle außer Admins
                    if (permission.Equals("locked", StringComparison.OrdinalIgnoreCase) && !userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Die Datei ist gesperrt und kann nur vom Admin gelesen werden.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"Keine spezifischen Berechtigungen für '{filePath}' gefunden. Zugriff wird gewährt.");
                }
            }
            else
            {
                Console.WriteLine("Die Berechtigungen sind nicht geladen oder leer.");
            }

            // Zugriff gewähren, wenn keine Sperre vorhanden ist oder keine spezifischen Berechtigungen definiert sind
            return true;
        }

        public void UnlockFile(string filePath)
        {
            string fullPath = Path.GetFullPath(filePath);

            if (filePermissions.ContainsKey(fullPath))
            {
                // Entferne die Berechtigung für die Datei
                filePermissions.Remove(fullPath);
                Console.WriteLine($"Die Datei '{fullPath}' wurde entsperrt.");
                SavePermissionsToFile();
            }
            else
            {
                Console.WriteLine($"Die Datei '{fullPath}' ist nicht im Berechtigungsdictionary vorhanden.");
            }
        }

        public void CreateFile(string path, string content)
        {
            try
            {
                using (var stream = File.Create(path))
                {
                    byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(content);
                    stream.Write(contentBytes, 0, contentBytes.Length);
                }
                ConsoleHelpers.WriteSuccess($"File created: '{path}'");
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Creating file: {ex.Message}");
            }
        }

        public void ReadFile(string path)
        {
                if (File.Exists(path))
                {
                    string content = File.ReadAllText(path);
                    Console.WriteLine($"Content of file: '{path}':\n{content}");
                }
                else
                {
                ConsoleHelpers.WriteError($"Error: File doesnt exist: '{path}' ");
                }
        }

        public void WriteToFile(string path, string content)
        {
            try
            {
                // Stelle sicher, dass das Verzeichnis existiert
                string directoryPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Öffne die Datei im Anhängemodus oder erstelle sie, falls sie nicht existiert
                using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(content + Environment.NewLine); // Fügt einen Zeilenumbruch hinzu
                    stream.Write(contentBytes, 0, contentBytes.Length);
                }
                ConsoleHelpers.WriteSuccess($"Content written in file: '{path}'");
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Writing in file: {ex.Message}");
            }
        }

        public void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    ConsoleHelpers.WriteSuccess($"File deleted: '{path}'.");
                }
                else
                {
                    ConsoleHelpers.WriteError($"Error: File doesnt exist: '{path}'");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Deleting file: {ex.Message}");
            }
        }

        public void CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    ConsoleHelpers.WriteSuccess($"Directory created: '{path}' ");
                }
                else
                {
                    ConsoleHelpers.WriteError($"Error: Directory already exist: '{path}'");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Creating directory: {ex.Message}");
            }
        }

        public void ListDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var entries = Directory.GetFileSystemEntries(path);
                    if (entries.Length == 0)
                    {
                        ConsoleHelpers.WriteError("Error: Directory is empty.");
                    }
                    else
                    {
                        foreach (var entry in entries)
                        {
                            Console.WriteLine($" - {entry}");
                        }
                    }
                }
                else
                {
                    ConsoleHelpers.WriteError($"Error: Directory doesnt exist: '{path}'");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.WriteError($"Error: Listing directries: {ex.Message}");
            }
        }
        public void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    ConsoleHelpers.WriteSuccess($"Directory deleted: '{path}'");
                }
                else
                {
                    ConsoleHelpers.WriteError($"Error: Directory doesnt exist: '{path}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Deleting directory: {ex.Message}");
            }
        }
    }
}