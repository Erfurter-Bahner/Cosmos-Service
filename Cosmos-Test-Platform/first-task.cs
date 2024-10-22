using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;

namespace Cosmos_Test_Platform
{
    public class Kernel : Sys.Kernel
    {
        Memory mem = new Memory();
        DateTime start;
        protected override void BeforeRun()
        {
            Console.WriteLine("\n\t\t\t _____\r\n\t\t\t/     \\\r\n\t_______/_______\\_______\r\n\t\\\\______AMIG.OS______//\n");
            start = DateTime.Now;
        }

        protected override void Run()
        {
            Console.Write(">> ");
            var input = Console.ReadLine();
            string[] args = input.Split(' ');

            switch (args[0])
            {
                case "help":
                    {
                        Console.WriteLine("phillip stinkt");
                        break;
                    }

                case "time":
                    {
                        getSysTime();
                        break;
                    }

                case "write":
                    {
                        int args_lenght = args.Length - 1;

                        if (args_lenght <= 1 || args_lenght > 2)
                        {
                            Console.WriteLine("index wert");
                        }

                        else if (uint.Parse(args[1]) > 16 || byte.Parse(args[2]) > 255)
                        {
                            Console.WriteLine("index muss zwischen 0 und 16 liegen, byte zwischen 0 und 255");
                        }

                        else mem.Schreiben(uint.Parse(args[1]), byte.Parse(args[2]));

                        Console.WriteLine(args_lenght);
                        break;
                    }

                case "read":
                    {
                        int a = mem.Lesen(uint.Parse(args[1]));
                        Console.WriteLine(a);
                        break;
                    }

                case "adios":
                    {
                        if (args.GetLength(0)==2 && args[1].Equals("amigos"))
                        {
                            Console.WriteLine("~~~\nHASTA LA VISTA\n~~~");
                            Thread.Sleep(1500);
                            Sys.Power.Shutdown();
                        }
                        break;
                    }

                case "adios2":
                    {
                        if (args.GetLength(0) == 2)
                        {
                            if (args[1].Equals("amigos"))
                            {
                                Console.WriteLine("~~~\nHASTA LA VISTA\n~~~");
                                Thread.Sleep(1500);
                                Sys.Power.Shutdown();
                            }
                            else
                            {
                                fehler(0, args, 1);
                            }
                        }
                        else if (args.GetLength(0) > 2)
                        {
                            fehler(2, args, 1);
                        }
                        else
                        {
                            fehler(1, args, 1);
                        }
                        break;
                    }

                default:
                    Console.WriteLine("eingabe falsch");
                    break;
            }
        }
        private void getSysTime()
        {
            TimeSpan runtime = DateTime.Now - start;
            Console.WriteLine("running for: " + runtime.TotalSeconds + " seconds");

        }

        private void fehler(int i, string []arr, int a)
        {
            Thread.Sleep(2000);
            switch (i)
            {
                case 0:
                    {
                        Console.WriteLine("unknown argument:" + arr[a]);
                        break;
                    }
                case 1:
                    {
                        Console.WriteLine("missing argument after: " + arr[a-1]);
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("too many arguments, expected arguments: "+(a+1));
                        break;
                    }
                case 3:
                    {
                        break;
                    }
                case 4:
                    {
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Fehler :D");
                        break;
                    }
            }
        }
    }

    class Memory
    {
        Cosmos.Core.ManagedMemoryBlock newBlock = new Cosmos.Core.ManagedMemoryBlock(16); //16 Speicheradressen 
        public void Schreiben(uint index, byte value)
        {
            newBlock.Write8(index, value);
        }
        public ushort Lesen(uint index)
        {
            ushort val = 0;
            val = newBlock.Read16(index);
            return (ushort)(val & 0xFF);
        }
    }
 
   public abstract class ProgrammClass //ohne Zugriffsmodifizierer standardmäßig internal
    {
        public int PID { get; private set; }
        public string ProgName { get; private set; }
        public int MemoryIndex { get; private set; }

        public ProgrammClass(int _PID, string _ProgName, int _MemoryIndex)
        {
            PID=_PID; 
            ProgName=_ProgName; 
            MemoryIndex=_MemoryIndex;
        }

    }
}
