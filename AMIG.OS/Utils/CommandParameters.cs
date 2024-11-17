﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMIG.OS.Utils
{
    public class CommandParameters
    {
        public Dictionary<string, string> Parameters = new Dictionary<string, string>();/*(StringComparer.OrdinalIgnoreCase)*/ 

        public CommandParameters()
        {
            
        }

        public void AddParameter(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine("Fehler: Der Schlüssel darf nicht leer sein.");
                return;
            }

            if (!Parameters.ContainsKey(key))
            {
                Parameters[key] = value;
            }
            else
            {
                Console.WriteLine($"Warnung: Der Schlüssel '{key}' existiert bereits. Der Wert wird überschrieben.");
                Parameters[key] = value; // Falls Überschreiben gewünscht
                                         // Alternativ:
                                         // Parameters[key] += " " + value; // Falls Werte angehängt werden sollen
            }
        }


        public bool TryGetValue(string key, out string value)
        {
            foreach (var param in Parameters)
            {
                Console.WriteLine($"Schlüssel: {param.Key}, Wert: {param.Value}");
            }
            return Parameters.TryGetValue(key, out value);
        }

    }
}
