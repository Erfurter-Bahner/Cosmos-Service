using System;
using System.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;

namespace AMIG.OS.FileManagement
{
    public class FileSystemManager
    {
        public void CreateFile(string path, string content)
        {
            try
            {
                using (var stream = File.Create(path))
                {
                    byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(content);
                    stream.Write(contentBytes, 0, contentBytes.Length);
                }
                Console.WriteLine($"Datei '{path}' erstellt.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Erstellen der Datei: {ex.Message}");
            }
        }
        public void ReadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string content = File.ReadAllText(path);
                    Console.WriteLine($"Inhalt der Datei '{path}':\n{content}");
                }
                else
                {
                    Console.WriteLine($"Datei '{path}' existiert nicht.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Lesen der Datei: {ex.Message}");
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
                Console.WriteLine($"Inhalt in die Datei '{path}' geschrieben.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schreiben in die Datei: {ex.Message}");
            }
        }
        public void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine($"Datei '{path}' wurde gelöscht.");
                }
                else
                {
                    Console.WriteLine($"Datei '{path}' existiert nicht.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Löschen der Datei: {ex.Message}");
            }
        }
        public void CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Console.WriteLine($"Verzeichnis '{path}' erstellt.");
                }
                else
                {
                    Console.WriteLine($"Verzeichnis '{path}' existiert bereits.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Erstellen des Verzeichnisses: {ex.Message}");
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
                        Console.WriteLine("Verzeichnis ist leer.");
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
                    Console.WriteLine($"Verzeichnis '{path}' existiert nicht.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Auflisten des Verzeichnisses: {ex.Message}");
            }
        }
        public void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"Verzeichnis '{path}' und sein Inhalt wurden gelöscht.");
                }
                else
                {
                    Console.WriteLine($"Verzeichnis '{path}' existiert nicht.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Löschen des Verzeichnisses: {ex.Message}");
            }
        }
    }
}