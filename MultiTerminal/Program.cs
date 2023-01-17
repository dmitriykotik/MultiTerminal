using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.MemoryMappedFiles;
using Microsoft.Win32;

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
            Console.WriteLine(@"MultiPlayer MultiTerminal [V0.1 DEV]
(C) Корпорация MultiPlayer (MultiPlayer Corporation). Все права защищены.           
");
            
            while (true)
            {
                Console.ResetColor();
                Console.Write($@"{Environment.UserName}@Terminal $ ");
                string[] enter = Console.ReadLine().Split();
                switch (enter[0])
                {
                    case "echo":
                        enter[0] = "";
                        Console.WriteLine(string.Join(" ", enter));
                        break;

                    case "clock":
                        clock();
                        break;

                    case "plus":
                        if (enter.Length == 3)
                        {
                            plus(enter[1], enter[2]);
                        }
                        else
                        {
                            int len = enter.Length - 1;
                            Console.WriteLine($"[MT | Plus | Pre-Start] Перебор/Нехватает чисел для запуска программы Plus! Указанно аргументов: {len}");
                            Console.WriteLine("[MT | Plus | Pre-Start | Help > plus_args] plus (arg 1) (arg 2)");
                        }
                        break;
                    case "minus":
                        if (enter.Length == 3)
                        {
                            minus(enter[1], enter[2]);
                        }
                        else
                        {
                            int len = enter.Length - 1;
                            Console.WriteLine($"[MT | Minus | Pre-Start] Перебор/Нехватает чисел для запуска программы Plus! Указанно аргументов: {len}");
                            Console.WriteLine("[MT | Minus | Pre-Start | Help > minus_args] minus (arg 1) (arg 2)");
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





        //Основные методы команд

        //Методы прибавления
        private static void plus(string arg1, string arg2)
        {
            ProcessStartInfo startinfo = new ProcessStartInfo("plus.exe", $"{arg1} {arg2}");
            startinfo.CreateNoWindow = false;
            startinfo.UseShellExecute = false;
            Process p = Process.Start(startinfo);
            p.WaitForExit();
        }
        //Метод удавления

        private static void minus(string arg1, string arg2)
        {
            ProcessStartInfo startinfo = new ProcessStartInfo("minus.exe", $"{arg1} {arg2}");
            startinfo.CreateNoWindow = false;
            startinfo.UseShellExecute = false;
            Process p = Process.Start(startinfo);
            p.WaitForExit();
        }

        //Метод часов
        private static void clock()
        {
            ProcessStartInfo startinfo = new ProcessStartInfo("clock.exe");
            startinfo.CreateNoWindow = false;
            startinfo.UseShellExecute = false;
            Process p = Process.Start(startinfo);
            p.WaitForExit();
        }

        
        
    }
}
