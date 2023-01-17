using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiTerminal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length > 0) 
            {


                if (args.Length == 1)
                {
                    if (MessageBox.Show("Продолжить работу с 1-м аргументом не возможно! Желаете открыть терминал без использования аргументов?", "Ошибка аргументов", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes) 
                    {
                        mainnonterminal();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                    
                }
                else if (args.Length >= 2)
                {
                    mainargs(args);
                }
                
            }
            else
            {
                mainnonterminal();
            }
        }

        private static void mainargs(string[] arg)
        {

        }

        private static void mainnonterminal()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Write(@"USER $ ");
                string[] enter = Console.ReadLine().Split();
                switch (enter[0])
                {
                    case "plus":
                        if (enter.Length == 3)
                        {
                            Console.WriteLine(Convert.ToInt16(enter[1]) + Convert.ToInt16(enter[2]));
                        }
                        else
                        {
                            Console.WriteLine("Нельзя сложить менее 2-х или более 2-х чисел!");
                        }
                        break;
                    default:
                        if (File.Exists(enter[0]))
                        {
                            try
                            {
                                Process.Start(enter[0]);
                            }
                            catch (Exception e)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Не удаётся запустить файл, причина: {e.Message}");
                            }
                        }
                        else
                        {
                            if (enter[0] == "")
                            {

                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($@"""{enter[0]}"" не является внутренней или внешней
командой, исполняемой программой или пакетным файлом.");
                            }
                        }
                        break;
                }
            }
        }
    }
}
