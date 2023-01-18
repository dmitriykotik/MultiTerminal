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
using System.Net.Mail;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Xml.Linq;
using MultiINI;

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
            if (File.Exists("themes.ini"))
            {

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@"Продолжить работу в терминале нельзя, так как отсутствует файл темы.");
            }
            var MyIni = new INI("themes.ini");
            WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();


            //Console.WriteLine(@"MultiPlayer MultiTerminal [V0.1 DEV]");
            //Console.WriteLine("(C) Корпорация MultiPlayer (MultiPlayer Corporation). Все права защищены.");
            Console.WriteLine(MyIni.Read("welcomeTextOne", "main"));
            Console.WriteLine(MyIni.Read("welcomeTextTwo", "main"));

            Console.Title = MyIni.Read("startTitle", "startTitleConsole");
            
            while (true)
            {
                Console.ResetColor();
                if (MyIni.Read("enableCustomTextEnter", "enterConsole") == "0")
                {
                    Console.Write($@"{Environment.UserName}@Terminal $ ");
                }
                else
                {
                    Console.Write(MyIni.Read("customTextEnter", "enterConsole"));
                }
                string[] enter = Console.ReadLine().Split();
                switch (enter[0])
                {
                    case "app":
                        if (enter.Length == 4)
                        {
                            if (enter[1] == "music")
                            {
                                if (enter[2] == "play")
                                {
                                    try
                                    {
                                        wplayer.URL = enter[3];
                                        wplayer.controls.play();

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                
                            }
                        }
                        else if (enter.Length == 3) 
                        {
                            if (enter[1] == "music")
                            {
                                if (enter[2] == "stop")
                                {
                                    try
                                    {
                                        wplayer.controls.stop();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                        }
                        break;

                    case "smtp":
                        smtp();
                        break;
                    case "help":
                        help();
                        break;
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
                                ProcessStartInfo startinfo = new ProcessStartInfo(enter[0]);
                                startinfo.CreateNoWindow = false;
                                startinfo.UseShellExecute = false;
                                Process p = Process.Start(startinfo);
                                p.WaitForExit();
                            }
                            catch (Exception e)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"{MyIni.Read("errorStartFile", "errors")} {e.Message}");
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
                                Console.WriteLine($@"""{enter[0]}"" {MyIni.Read("unknownCommand1", "errors")}");
                                Console.WriteLine($@"{MyIni.Read("unknownCommand2", "errors")}");
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
            var MyIni = new INI("themes.ini");
            try
            {
                ProcessStartInfo startinfo = new ProcessStartInfo("plus.exe", $"{arg1} {arg2}");
                startinfo.CreateNoWindow = false;
                startinfo.UseShellExecute = false;
                Process p = Process.Start(startinfo);
                p.WaitForExit();
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (File.Exists("plus.exe"))
                {
                    Console.WriteLine($"{MyIni.Read("PLUSandMINUSandCLOCKprogrammFoundStartError", "errors")} {ex.Message}");
                }
                else {
                    Console.WriteLine($"{MyIni.Read("PLUSandMINUSandCLOCKprogrammUnFound", "errors")} {ex.Message}");
                }
            }
        }
        //Метод удавления

        private static void minus(string arg1, string arg2)
        {
            var MyIni = new INI("themes.ini");
            try
            {
                ProcessStartInfo startinfo = new ProcessStartInfo("minus.exe", $"{arg1} {arg2}");
                startinfo.CreateNoWindow = false;
                startinfo.UseShellExecute = false;
                Process p = Process.Start(startinfo);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (File.Exists("minus.exe"))
                {
                    Console.WriteLine($"{MyIni.Read("PLUSandMINUSandCLOCKprogrammFoundStartError", "errors")} {ex.Message}");
                }
                else
                {
                    Console.WriteLine($"{MyIni.Read("PLUSandMINUSandCLOCKprogrammUnFound", "errors")} {ex.Message}");
                }
            }
        }

        //Метод часов
        private static void clock()
        {
            var MyIni = new INI("themes.ini");
            try
            {
                ProcessStartInfo startinfo = new ProcessStartInfo("clock.exe");
                startinfo.CreateNoWindow = false;
                startinfo.UseShellExecute = false;
                Process p = Process.Start(startinfo);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (File.Exists("clock.exe"))
                {
                    Console.WriteLine($"{MyIni.Read("PLUSandMINUSandCLOCKprogrammUnFound", "errors")} {ex.Message}");
                }
                else
                {
                    Console.WriteLine($"{MyIni.Read("PLUSandMINUSandCLOCKprogrammUnFound", "errors")} {ex.Message}");
                }
            }
        }

        private static void smtp()
        {
            var MyIni = new INI("themes.ini");
            try
            {
                Console.Write("Введите имя: ");
                string name = Console.ReadLine();
                Console.WriteLine("");
                Console.Write("Введите почту на которую вы отправляете сообщение: ");
                string toemail = Console.ReadLine();
                Console.WriteLine("");
                Console.Write("Введите тему: ");
                string subj = Console.ReadLine();
                Console.WriteLine("");
                Console.Write("Введите текст или html код: ");
                string tex = Console.ReadLine();
                MailAddress from = new MailAddress("multiplayercorporation@gmail.com", name + " - MultiMail");
                MailAddress to = new MailAddress(toemail);
                MailMessage m = new MailMessage(from, to);
                m.Subject = subj;
                m.Body = tex + "<br> <br> Отправлено из MultiTerminal <br> (C) Copyright MultiPlayer 2019-2023 <br> https://multiplayercorporation.mya5.ru";
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("multiplayercorporation@gmail.com", "pbmaikdrcczrvweq");
                smtp.EnableSsl = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Попытка отправить сообщение...");
                smtp.Send(m);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Успешно!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                MessageBox.Show($"Не удаётся отправить сообщение! Причина: {ex.Message}", "Ошибка!");
            }
        }

        private static void help()
        {
            var MyIni = new INI("themes.ini");
            Console.WriteLine(@"Справка команд | HELP:
 clock - Часы
 plus (arg1) (arg2) - Сумма чисел
 minus (arg1) (arg2) - Разность чисел
 help - Справка
 echo (text) - Повтор текста за вами
 smtp - Отправка сообщений на почту");
        }

        
        
    }
}
