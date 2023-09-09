using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTerminal
{
    internal class s_app
    {
        public void start(string[] input, string version)
        {
            if (input.Length >= 2)
            {
                switch (input[1].ToLower())
                {
                    case "update":
                        if (File.Exists(Environment.CurrentDirectory + "\\updater.exe"))
                        {
                            try
                            {
                                ProcessStartInfo info = new ProcessStartInfo();
                                info.Verb = "runas";
                                info.FileName = Environment.CurrentDirectory + "\\updater.exe";
                                info.Arguments = version;
                                Process.Start(info);
                                break;
                            }
                            catch (Exception e)
                            {
                                SystemProgams.SendError(e.Message);
                                break;
                            }
                        }
                        else
                        {
                            SystemProgams.SendError("Файл центра обновления MultiTerminal не обнаружен! Попробуйте вручную запустить программу updater.exe.");
                            break;
                        }

                    case "packages":
                        if (input.Length >= 3)
                        {
                            switch (input[2].ToLower())
                            {
                                case "list":
                                    DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\Packages");
                                    if (directoryInfo.Exists)
                                    {
                                        foreach (var item in directoryInfo.GetFileSystemInfos())
                                        {
                                            Console.WriteLine($"{item.Name}");
                                        }
                                    }
                                    else
                                    {
                                        SystemProgams.SendWarning("У вас нет пакетов!");
                                    }
                                    break;
                                case "remove":
                                    if (input.Length >= 4)
                                    {
                                        switch (input[3].ToLower())
                                        {
                                            case "all":
                                                SystemProgams.SendWarning("Вы уверенны, что хотите удалить ВСЕ ПАКЕТЫ из MultiTerminal? [Y/N] Это не обратимый процесс!!!");
                                                Console.Write("> ");
                                                if (Console.ReadLine().ToLower() == "y")
                                                {
                                                    try
                                                    {
                                                        if (Directory.Exists(Environment.CurrentDirectory + "\\Packages"))
                                                        {
                                                            Directory.Delete(Environment.CurrentDirectory + "\\Packages", true);
                                                            SystemProgams.SendInfo("Пакеты удалены!");
                                                        }
                                                        else
                                                        {
                                                            SystemPrograms.SendInfo("Пакеты уже были удалены!");
                                                        }
                                                    }catch(Exception e)
                                                    {
                                                        SystemProgams.SendError(e.Message);
                                                        break;
                                                    }
                                                }
                                                break;
                                            default:
                                                if (input.Length >= 4)
                                                {
                                                    if (String.IsNullOrEmpty(input[3]))
                                                    {
                                                        break;
                                                    }
                                                    try
                                                    {
                                                        Directory.Delete(Environment.CurrentDirectory + $"\\Packages\\{input[3]}");
                                                    }catch (Exception e)
                                                    {
                                                        SystemProgams.SendError(e.Message);
                                                        break;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;

                                case "install":
                                    if (input.Length >= 4)
                                    {
                                        try
                                        {
                                            if (File.Exists(input[3]))
                                            {
                                                if (Directory.Exists(Environment.CurrentDirectory + "\\Packages")) { }
                                                else { Directory.CreateDirectory(Environment.CurrentDirectory + "\\Packages"); }
                                                
                                                if (Path.GetExtension(input[3]) == ".mpkg")
                                                {
                                                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\tmp_packages");
                                                    ZipFile.ExtractToDirectory(input[3], Environment.CurrentDirectory + "\\tmp_packages");
                                                    if (File.Exists(Environment.CurrentDirectory + "\\tmp_packages\\package.data") && File.Exists(Environment.CurrentDirectory + "\\tmp_packages\\main.exe")) {
                                                        Directory.CreateDirectory(Environment.CurrentDirectory + "\\Packages\\" + File.ReadAllText(Environment.CurrentDirectory + "\\tmp_packages\\package.data"));
                                                        ZipFile.ExtractToDirectory(input[3], Environment.CurrentDirectory + "\\Packages\\" + File.ReadAllText(Environment.CurrentDirectory + "\\tmp_packages\\package.data"));
                                                        SystemProgams.SendInfo($"Пакет \"{File.ReadAllText(Environment.CurrentDirectory + "\\tmp_packages\\package.data")}\" установлен! Для его использования, используйте команду \"{File.ReadAllText(Environment.CurrentDirectory + "\\tmp_packages\\package.data")}\"");
                                                        Directory.Delete(Environment.CurrentDirectory + "\\tmp_packages", true);                                                        
                                                    }
                                                    else
                                                    {
                                                        SystemPrograms.SendError("APP: PACKAGES-INSTALL: К сожалению пакет повреждён :(");
                                                        Directory.Delete(Environment.CurrentDirectory + "\\tmp_packages", true);
                                                    }

                                                }
                                                else
                                                {

                                                    SystemProgams.SendError("APP: PACKAGES-INSTALL: Данный формат файла не поддерживается! Скачайте пакет в формате .mpkg");
                                                }
                                            }
                                            else
                                            {
                                                SystemProgams.SendError("APP: PACKAGES-INSTALL: Пакет не найден!");
                                            }
                                        }catch (Exception e)
                                        {
                                            SystemPrograms.SendError(e.Message);
                                        }
                                    }
                                    else
                                    {
                                        SystemProgams.SendError("APP: PACKAGES-INSTALL: Не достаточно аргументов!");
                                    }
                                    break;

                                default:
                                    if (String.IsNullOrEmpty(input[2]))
                                    {
                                        break;
                                    }
                                    SystemProgams.SendError($"APP: PACKAGES: Команда \"{input[2]}\" в секторе \"PACKAGES\" не обнаружена!");
                                    break;
                            }
                            break;
                        }
                        else
                        {
                            if (values.strings.debug) SystemProgams.SendError($"APP: PACKAGES: Не достаточно аргументов! [app>packages ! \"if (input.Length >= 3)\" => returned \"FALSE\"]");
                            else SystemProgams.SendError($"APP: PACKAGES: Не достаточно аргументов!");
                            break;
                        }
                    default:
                        if (String.IsNullOrEmpty(input[1])) 
                        {
                            break;
                        }
                        SystemPrograms.SendError($"APP: Команда {input[1]} не обнаружена!");
                        break;


                }
            }
            else
            {
                if (values.strings.debug) SystemProgams.SendError("APP: Запустить пустоту не возможно? [app ! \"if (input.Length >= 2)\" => returned \"FALSE\"]");
                else SystemProgams.SendError("APP: Запустить пустоту не возможно?");
                if (values.strings.notifyHelp == 0)
                {
                    SystemProgams.SendInfo("Возможно, вы не правильно указали аргументы. Попробуйте использовать \"APP HELP\" (для вывода справки по APP) или \"HELP\" (для вывода справки по терминалу)");
                    ++values.strings.notifyHelp;
                }
                else
                {
                    values.strings.notifyHelp = 0;
                }
            }
        }
    }

    internal static class SystemPrograms
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
    }
}
