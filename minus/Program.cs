using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (Process.GetProcessesByName("multiterminal").Length > 0)
            {
                if (args.Length == 2)
                {
                    try
                    {
                        int sum = Convert.ToInt32(args[0]) - Convert.ToInt32(args[1]);
                        Console.WriteLine(sum);
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (args.Length < 2)
                {
                    Console.WriteLine("Не достаточно аргументов");
                    Environment.Exit(0);
                }
                else if (args.Length > 2)
                {
                    Console.WriteLine("Перебор аргументов");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unknown Error");
                    Environment.Exit(0);
                }
            }
            else
            {
                if (args.Length == 2)
                {
                    try
                    {
                        int sum = Convert.ToInt32(args[0]) - Convert.ToInt32(args[1]);
                        Console.WriteLine(sum);
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (args.Length < 2)
                {
                    Console.WriteLine("Не достаточно аргументов");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else if (args.Length > 2)
                {
                    Console.WriteLine("Перебор аргументов");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unknown Error");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }



        }
    
    }
}
