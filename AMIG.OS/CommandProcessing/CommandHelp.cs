using System.Collections.Generic;
using System;

public class CommandHelp
{
    public static readonly List<string> AllPermissions = new List<string>
    {
        "addrole",
        "rmrole",
        "addroletouser",
        "rmroleuser",
        "addpermtouser",
        "rmpermuser",
        "rmpermrole",
        "addpermtorole",
        "adduser",
        "rmuser",
        "showall",
        "showme",
        "changename",
        "changepw",
        "logout",
        "adios",
        "clear",
        "datetime",
        "ls",
        "cat",
        "touch",
        "cd",
        "write",
        "rmfile",
        "rmdir",
        "showuser",
        "mkdir",
    };

    public static void ShowAllCommandsHelp()
    {
        const int columnWidth = 20; // Maximale Breite pro Spalte
        const int columns = 2;     // Anzahl der Spalten

        int rows = (int)Math.Ceiling((double)AllPermissions.Count / columns);

        Console.WriteLine("Available Commands:");
        for (int row = 0; row < rows; row++)
        {
            string line = "";
            for (int col = 0; col < columns; col++)
            {
                int index = row + col * rows;
                if (index < AllPermissions.Count)
                {
                    line += AllPermissions[index].PadRight(columnWidth);
                }
            }
            Console.WriteLine(line);
        }
    }
}
