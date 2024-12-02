using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMIG.OS.Utils
{
    internal static class StartScreen
    {
        public static void DisplayLogoLoading()
        {
            Console.Clear();
            int window_width = Console.WindowWidth;
            Console.ForegroundColor = ConsoleColor.DarkRed;

            for (int i = 0; i < logo.Length; i++)
            {
                Console.Write(logo[i]);
            }
            Console.WriteLine("\nSystem is loading");
            // Ladebalken mit Klammern
            Console.Write("[");
            Console.SetCursorPosition(window_width - 1, Console.CursorTop); // Setze Cursorposition nach der rechten Klammer
            Console.Write("]");
            Console.SetCursorPosition(1, Console.CursorTop - 1);
            for (int i = 0; i < (window_width) - 3; i++)
            {
                Console.SetCursorPosition(1 + i, Console.CursorTop);
                Console.Write("||");
                Thread.Sleep(50); 
            }

            Thread.Sleep(2000);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

        }

        public static string[] logo =
        {
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            " \n\t\t\t      _____\r\n\t\t\t     /     \\\r\n\t     _______/_______\\_______\r\n\t     \\\\______AMIG.OS______//\n                                                                               ",
            "  @@@@         @@@       @@@   @@@@    @@@@@@@@@@        @@@@@@@@@@    @@@@@@@@ ",
            "  @@@@@@       @@@@     @@@@   @@@@   @@@@   @@@@@      @@@@@  @@@@@   @@@   @@ ",
            "  @@  @@@      @@@@@   @@@@@   @@@@  @@@@              @@@@      @@@@  @@@      ",
            "  @@@  @@@     @@@@@@@@@@@@@   @@@@  @@@    @@@@@@@    @@@        @@@  @@@@@@@@ ",
            "  @@@@@@@@@    @@  @@@@@  @@   @@@@  @@@@   @@@@@@@    @@@@      @@@@       @@@ ",
            "  @@@@@@@@@@   @@   @@@   @@   @@@@   @@@@    @@@@      @@@@@  @@@@@   @@   @@@ ",
            "  @@@     @@@  @@    @    @@   @@@@    @@@@@@@@@    @@   @@@@@@@@@@    @@@@@@@@ ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                ",
            "                                                                                "
        };

    }
}
