#region Libraries
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

/*
 -=* NOTE #1 *=-
Project: MultiTerminal
Version: V1.0 ALPHA
Credits: Dmitryy Nekrash
Rules for using the source code: 
| For the sake of fair use of source code, we sincerely ask that you indicate 
| in the authorship that you are using our source code: "Used MT_Source by Dmitryy Nekrash". 
| If you don't want a lot of text, then specify the name Dmitryy Nekrash in the developers. 
| It is also forbidden to create terminal modifications and embed viruses in them. You can 
| create modifications for MultiTerminal or create your own programmes for MultiTerminal. 
| In your programmes for MultiTermianl you do not need to specify the author of MultiPlayer, 
| as it is your programme. Also in your MultiTerminal modifications you can indicate your 
| authorship with our authorship. Thaaaaaaaaaaaaaaaaaaaaaaaaaaaaanks for understanding :3
(C) MultiPlayer 2019-2023
-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
Проект: MultiTerminal
Версия: V1.0 ALPHA
Разработчики: Dmitryy Nekrash
Правила использования исходного кода:
| В целях честного использования исходном кода, мы искренне просим, чтобы вы указывали в авторах,
| что вы используете наш исходный код: "Используется MT_Source by Dmitryy Nekrash". 
| Если вы не хотите много текста, то укажите в разработчиках имя Dmitryy Nekrash. 
| Также запрещено создавать модификации терминала и встраивать в них вирусы. 
| Вы можете создавать модификации MultiTerminal или создавать свои программы под MultiTerminal. 
| В своих программах для MultiTermianl не требуется указывать автора MultiPlayer, так как это 
| ваша программа. Также в своих модификациях MultiTerminal вы можете указать своё авторство с 
| указанием нашего авторства. Спасииииибооо за понимание :3
(C) MultiPlayer 2019-2023
*/

namespace MultiTerminal
{
    internal class Program
    {

        /* Переменные для обозначения, что это за терминал */
        public static string Product = "MultiTerminal (For XP)";
        public static string Version = "1.0.0.0";
        public static string Company = "MultiPlayer";
        public static string CompanyOtherLanguage = "МультиПлеер";
        public static string Revision = "ALPHA";

        /* Системные переменные */
        public static bool StandartModeWhile = true;
        /*
        public static string lastestDir = "https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update"; // Сетевой путь до папки в котором находятся необходимые папки
        public static string lastestVersion = "lastest_Version_mt.txt"; // Файл с последней версией терминала находящийся на сервере (В файле хранится текст, Например: "1.0.0.0")
        public static string lastestVersionUpdater = "lastest_Version_Updater.txt"; // Файл с последней версией обновлятора находящийся на сервере (В файле хранится текст, Например: "1.0")
        public static string lastestTerminal = "lastest_release_mt.mpkg_update";
        public static string lastestUpdater = "lastest_updater.mpkg_update";*/
        /* END */

        static void Main(string[] args)
        {
            
            WebClient client = new WebClient();
            Console.ResetColor();
            Console.Title = $"{Environment.UserName}@{Environment.MachineName}";
            Console.WriteLine($@"{Company} {Product} [Version {Version}/{Revision}]
(c) Корпорация {CompanyOtherLanguage}, 2019.
ВНИМАНИЕ! Терминал предназначен для Windows XP, поэтому некоторые функции могут не работать.");
            /*
            try
            {
                if (client.DownloadString(lastestDir + "/" + lastestVersion) == Version) { }
                else
                {
                    try
                    {
                        if (SystemProgams.TestConnection())
                        {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($@"
[WARNING] Вышло новое обновление! [Текущая версия: {Version}] [Новая версия: {client.DownloadString(lastestDir + "/" + lastestVersion)}]");
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.WriteLine("[INFO] Для обновления напишите команду: app update");
                            Console.ResetColor();
                        }
                        else
                        {
                            SystemPrograms.SendWarning("Нет подключения к интернету чтобы проверить обновление!");
                        }
                    }
                    catch (Exception ex)
                    {
                        SystemProgams.SendWarning($"Не удалось проверить обновление! [{ex.Message}]");
                    }
                }
            }catch (Exception ex)
            {
                SystemPrograms.SendError(ex.Message);
            }*/
            Console.WriteLine("");
            if (args.Length == 0) StandartMode();
            
        }

        static void StandartMode()
        {
            while (StandartModeWhile)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Environment.UserName + "@" + Environment.MachineName);
                Console.ResetColor();
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("~");
                Console.ResetColor();
                Console.Write("# ");
                string[] input = Console.ReadLine().Split(' ');
                Console.ResetColor();
                if (input.Length == 0)
                {
                    Console.WriteLine("");
                }
                switch (input[0].ToLower())
                {
                    case "terminal":
                        StandartPrograms.terminal();
                        break;
                    case "clear":
                        StandartPrograms.clear();
                        break;
                    case "ls":
                        StandartPrograms.ls(input);
                        break;
                    case "dir":
                        StandartPrograms.ls(input);
                        break;
                    case "arg":
                        string result = string.Join(" ", input, 1, input.Length - 1);
                        Console.WriteLine(result);
                        break;
                    case "app":
                        StandartPrograms.app(input);
                        break;
                    case "debug":
                        values.strings.debug = true;
                        break;

                    default:
                        if (String.IsNullOrEmpty(input[0])){}
                        else
                        {
                            if (input[0].ToLower() == "updater")
                            {
                                SystemProgams.SendWarning("К сожалению встроенный обновлятор для MultiTerminal (For XP) временно отсутствует :(");
                                break;
                                /*
                                if (input.Length == 1)
                                {
                                    SystemProgams.SendError("Извините, но вы не можете запустить эту программу, пожалуйста, используйте команду: \"app update\". Если вы хотите использовать программу в ручном режиме, то используйте эту команду: \"updater +manual_mode\". Если у вас нет разрешения UAC, то используйте команду: \"updater +manual_mode -uac\".");
                                    break;
                                }else if (input.Length == 2)
                                {
                                    if (input[1].ToLower() == "+manual_mode")
                                    {
                                        ProcessStartInfo processStartInfo = new ProcessStartInfo();
                                        processStartInfo.FileName = "updater.exe";
                                        processStartInfo.Verb = "runas";
                                        try
                                        {
                                            Process.Start(processStartInfo);
                                        }catch (Exception ex)
                                        {
                                            SystemProgams.SendError(ex.Message);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        SystemProgams.SendError("Неизвестный аргумент!");
                                        break;
                                    }
                                }else if (input.Length == 3)
                                {
                                    if (input[1].ToLower() == "+manual_mode")
                                    {
                                        if (input[2].ToLower() == "-uac")
                                        {
                                            if (File.Exists(Environment.CurrentDirectory + "\\updater_nuac.exe"))
                                            {
                                                try
                                                {
                                                    Process.Start(Environment.CurrentDirectory + "\\updater_nuac.exe");
                                                    break;
                                                }catch (Exception ex)
                                                {
                                                    SystemPrograms.SendError(ex.Message);
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                SystemProgams.SendError("Файл Updater_nuac.exe не обнаружен!");
                                                break;
                                            }
                                            

                                        }
                                        ProcessStartInfo processStartInfo = new ProcessStartInfo();
                                        processStartInfo.FileName = "updater.exe";
                                        processStartInfo.Verb = "runas";
                                        try
                                        {
                                            Process.Start(processStartInfo);
                                        }
                                        catch (Exception ex)
                                        {
                                            SystemProgams.SendError(ex.Message);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        SystemProgams.SendError("Неизвестный аргумент!");
                                        break;
                                    }
                                }
                                */
                            }
                            if (File.Exists(input[0].ToLower() + ".exe"))
                            {
                                ProcessStartInfo startinfo = new ProcessStartInfo(input[0] + ".exe", string.Join(" ", input, 1, input.Length - 1));
                                startinfo.CreateNoWindow = false;
                                startinfo.UseShellExecute = false;
                                SystemProgams.StartProgram(startinfo);
                            }
                            else if (File.Exists(input[0].ToLower()))
                            {
                                ProcessStartInfo startinfo = new ProcessStartInfo(input[0], string.Join(" ", input, 1, input.Length - 1));
                                startinfo.CreateNoWindow = false;
                                startinfo.UseShellExecute = false;
                                SystemProgams.StartProgram(startinfo);


                            }
                            else if (Directory.Exists(Environment.CurrentDirectory + "\\Packages\\" + input[0]))
                            {
                                ProcessStartInfo startinfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\Packages\\" + input[0] + "\\main.exe", string.Join(" ", input, 1, input.Length - 1));
                                startinfo.CreateNoWindow = false;
                                startinfo.UseShellExecute = false;
                                SystemProgams.StartProgram(startinfo);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($@"""{input[0]}"" не является внутренней или внешней
командой, исполняемой программой или пакетным файлом.");
                                Console.ResetColor();
                            }
                        }
                        break;
                }
                
            }
            
        }
    }





    internal static class StandartPrograms
    {
        public static void terminal()
        {
            s_terminal a = new s_terminal();
            a.start();
        }

        public static void clear() 
        {
            s_clear a = new s_clear();
            a.start();
        }

        public static void ls(string[] ss)
        {
            s_ls a = new s_ls();
            a.start(ss);
        }

        public static void app(string[] input)
        {
            s_app a = new s_app();
            a.start(input, Program.Version);
        }
    }





    internal static class SystemProgams
    {
        public static void SendError(string text)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[ERROR] {text}");
            Console.ResetColor();
        }

        public static void SendWarning(string text)
        {
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[WARNING] {text}");
            Console.ResetColor();
        }

        public static void SendInfo(string text)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[INFO] {text}");
            Console.ResetColor();
        }
        public static bool TestConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send("google.com", 1000);
                    return result.Status == IPStatus.Success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void StartProgram(ProcessStartInfo startinfo)
        {
            try
            {
                Process p = Process.Start(startinfo);
                p.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 740)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    SendError(ex.Message);
                    Console.BackgroundColor = ConsoleColor.Blue;
                    SendInfo("Запуск терминала с правами администратора");
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    SendWarning("Будет запущено новое окно терминала с правами администратора!");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    
                    ProcessStartInfo si = new ProcessStartInfo(Environment.CurrentDirectory + "\\" + System.AppDomain.CurrentDomain.FriendlyName);
                    si.CreateNoWindow = false;
                    si.UseShellExecute = true;
                    si.Verb = "runas";
                    try
                    {
                        Process s = Process.Start(si);
                        Environment.Exit(0);
                    }
                    catch (Win32Exception e)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.White;
                        SendError($"(Win32Exception){e.NativeErrorCode}/{e.Message}");
                        Console.ResetColor();
                    }
                    catch (Exception ee)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.White;
                        SendError($"{ee.Message}");
                        Console.ResetColor();
                    }

                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    SendError($"(Win32Exception){ex.NativeErrorCode}/{ex.Message}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                SendError($"{ex.Message}");
                Console.ResetColor();
            }
        }
    }

}
