using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;

namespace updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                while (true)
                {
                    Console.WriteLine(@"Ручное управление MultiTerminal.
Выберите опцию:
1. Удалить текущую и установить новую версию MultiTerminal
2. Установить прошивку из файла
3. Скачать определённую версию официальной прошивки
4. Выйти
");
                    Console.Write("> ");
                    string result = Console.ReadLine();
                    switch (result)
                    {
                        case "1":
                            if (TestConnection()) { }
                            else
                            {
                                SendError("Операция отменена: Нет подключения к интернету!");
                                break;
                            }
                            SendWarning("Вы уверенны, что хотите удалить текущую версию терминала? (y/n) [Ваши установленные пакеты в папке Packages не будут удалены!]");
                            Console.Write("> ");
                            if (Console.ReadLine() == "y")
                            {
                                string processName = "MultiTerminal";
                                Process[] processes = Process.GetProcessesByName(processName);
                                foreach (Process process in processes)
                                {
                                    try
                                    {
                                        process.Kill();
                                    }
                                    catch (Exception ex)
                                    {
                                        SendError(ex.Message);
                                        break;
                                    }
                                }
                                WebClient cl = new WebClient();
                                if (TestConnection())
                                {

                                    try
                                    {
                                        string directoryPath = Environment.CurrentDirectory;
                                        string[] exceptionFileNames = { AppDomain.CurrentDomain.FriendlyName, "Packages", "updater_nuac.exe" };

                                        DirectoryInfo directory = new DirectoryInfo(directoryPath);
                                        FileInfo[] files = directory.GetFiles();
                                        DirectoryInfo[] subDirectories = directory.GetDirectories();

                                        foreach (FileInfo file in files)
                                        {
                                            if (Array.IndexOf(exceptionFileNames, file.Name) == -1)
                                            {
                                                file.Delete();
                                            }
                                        }

                                        foreach (DirectoryInfo subDirectory in subDirectories)
                                        {
                                            if (Array.IndexOf(exceptionFileNames, subDirectory.Name) == -1)
                                            {
                                                subDirectory.Delete(true);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        SendError(ex.Message);
                                        break;
                                    }
                                }
                                else
                                {
                                    SendError("Операция отменена: Нет подключения к интернету!");
                                    break;
                                }

                                SendInfo("Текущая версия терминала удалена!");
                                SendInfo("Скачивание прошивки...");
                                if (TestConnection()) { }
                                else
                                {
                                    SendError("Операция отменена: Нет подключения к интернету!");
                                    break;
                                }
                                try
                                {
                                    cl.DownloadFile("https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/lastest_release_mt.mpkg_update", $"{Environment.CurrentDirectory}\\lastest_release_mt.mpkg_update");
                                }
                                catch (Exception exa)
                                {
                                    SendError(exa.Message);
                                    break;
                                }
                                if (File.Exists(Environment.CurrentDirectory + "\\lastest_release_mt.mpkg_update"))
                                {
                                    try
                                    {
                                        ZipFile.ExtractToDirectory(Environment.CurrentDirectory + "\\lastest_release_mt.mpkg_update", Environment.CurrentDirectory);
                                        SendInfo("Прошивка установлена!");
                                    }
                                    catch (Exception exb)
                                    {
                                        SendError(exb.Message);
                                    }
                                }
                                SendInfo("Удаление файла прошивки...");
                                if (File.Exists(Environment.CurrentDirectory + "\\lastest_release_mt.mpkg_update"))
                                {
                                    try
                                    {
                                        File.Delete("lastest_release_mt.mpkg_update");
                                        SendInfo("Файл прошивки удалён!");
                                    }
                                    catch (Exception ex)
                                    {
                                        SendError(ex.Message);
                                    }
                                }
                                else
                                {
                                    SendWarning("Файл прошивки не обнаружен!");
                                }
                                SendInfo("Обновление завершено!");
                            }
                            else
                            {
                                SendError("Операция отменена: Пользователь отменил операцию");
                                break;
                            }

                            break;

                        case "2":
                            SendWarning("Вы уверены, что хотите вручную установить прошивку? (y/n)");
                            Console.Write("> ");
                            if (Console.ReadLine() == "y")
                            {
                                SendInfo("Введите полный путь к прошивке (Указывайте без пробелов!): ");
                                Console.Write("> ");
                                string res = Console.ReadLine();
                                if (File.Exists(res))
                                {
                                    if (Path.GetExtension(res) == ".mpkg_install")
                                    {
                                        SendInfo("Прошивка обнаружена!");
                                    }
                                    else if (Path.GetExtension(res) == ".mpkg_update")
                                    {
                                        SendWarning("Прошивка является файлом обновления! Вы уверенны, что хотите использовать файл обновления? (y/n)");
                                        Console.Write("> ");
                                        string resa = Console.ReadLine();
                                        if (resa == "y")
                                        {

                                        }
                                        else
                                        {
                                            SendError("Операция отменена: Пользователь отменил операцию");
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        SendError("Файл не является форматом прошивки для MultiTerminal!");
                                        break;
                                    }
                                }
                                else
                                {
                                    SendError("Прошивка не обнаружена!");
                                    break;
                                }
                                SendWarning($"Вы уверенны, что хотите установить прошивку по пути {res}? (y/n) [Ваши установленные пакеты в папке Packages не будут удалены!]");
                                Console.Write("> ");
                                string resb = Console.ReadLine();
                                if (resb == "y")
                                {
                                    string processName = "MultiTerminal";
                                    Process[] processes = Process.GetProcessesByName(processName);
                                    foreach (Process process in processes)
                                    {
                                        try
                                        {
                                            process.Kill();
                                        }
                                        catch (Exception ex)
                                        {
                                            SendError(ex.Message);
                                            break;
                                        }
                                    }
                                    SendInfo("Удаление текущей прошивки...");

                                }
                                else
                                {
                                    SendError("Операция отменена: Пользователь отменил операцию");
                                    break;
                                }
                                if (File.Exists(res))
                                {
                                    try
                                    {
                                        string directoryPath = Environment.CurrentDirectory;
                                        string[] exceptionFileNames = { AppDomain.CurrentDomain.FriendlyName, Path.GetFileName(res), "Packages", "updater_nuac.exe" };

                                        DirectoryInfo directory = new DirectoryInfo(directoryPath);
                                        FileInfo[] files = directory.GetFiles();
                                        DirectoryInfo[] subDirectories = directory.GetDirectories();

                                        foreach (FileInfo file in files)
                                        {
                                            if (Array.IndexOf(exceptionFileNames, file.Name) == -1)
                                            {
                                                file.Delete();
                                            }
                                        }

                                        foreach (DirectoryInfo subDirectory in subDirectories)
                                        {
                                            if (Array.IndexOf(exceptionFileNames, subDirectory.Name) == -1)
                                            {
                                                subDirectory.Delete(true);
                                            }
                                        }
                                        SendInfo("Текущая прошивка удалена!");
                                    }
                                    catch (Exception ex)
                                    {
                                        SendError(ex.Message);
                                        break;
                                    }
                                }
                                else
                                {
                                    SendError("Файл прошивки не обнаружен!");
                                }
                                SendInfo("Распаковка прошивки...");
                                if (File.Exists(res))
                                {
                                    try
                                    {
                                        ZipFile.ExtractToDirectory(res, Environment.CurrentDirectory);
                                        SendInfo("Прошивка установлена!");
                                    }
                                    catch (Exception exb)
                                    {
                                        SendError(exb.Message);
                                    }
                                }
                                else
                                {
                                    SendError("Файл прошивки не обнаружен!");
                                }
                            }
                            else
                            {
                                SendError("Операция отменена: Пользователь отменил операцию");
                                break;
                            }
                            break;

                        case "3":
                            WebClient client = new WebClient();
                            if (TestConnection())
                            {

                            }
                            else
                            {
                                SendError("Нет подключения к интернету!");
                                break;
                            }
                            try
                            {
                                SendInfo("Список версий:");
                                Console.WriteLine(client.DownloadString("https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/listAllVersions.txt"));
                            }
                            catch (Exception ex)
                            {
                                SendError(ex.Message);
                            }
                            SendInfo("Введите нужную вам версию (Например: 1.0.0.0):");
                            string res1 = Console.ReadLine();
                            SendInfo("Введите нужную ревизию к этой версии (Например: alpha): ");
                            string res2 = Console.ReadLine();

                            try
                            {
                                SendInfo("Скачивание файла установщика прошивки...");
                                if (File.Exists(Environment.CurrentDirectory + $"\\{res1}_{res2}_mt.mpkg_install"))
                                {
                                    client.DownloadFile($"https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/{res1}_{res2}_mt.mpkg_install", $"{Environment.CurrentDirectory}\\{res1}_{res2}_mt2.mpkg_install");
                                    SendInfo($"Файл прошивки сохранён на пути: {Environment.CurrentDirectory}\\{res1}_{res2}_mt.mpkg_install");
                                }
                                else
                                {
                                    client.DownloadFile($"https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/{res1}_{res2}_mt.mpkg_install", $"{Environment.CurrentDirectory}\\{res1}_{res2}_mt.mpkg_install");
                                    SendInfo($"Файл прошивки сохранён на пути: {Environment.CurrentDirectory}\\{res1}_{res2}_mt.mpkg_install");
                                }
                            }
                            catch (Exception ex)
                            {
                                SendError(ex.Message);
                                break;
                            }
                            break;

                        case "4":
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine(@"
Хмм, вам нужно ввести значение от 1 до 4
");
                            break;
                    }
                }
            }
            else
            {
                bool s2 = true;
                while (s2)
                {
                    WebClient cli = new WebClient();
                    Console.WriteLine("Добро пожаловать в центр обновлений MultiTerminal!");
                    if (TestConnection())
                    {
                        if (args[0] == cli.DownloadString("https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/lastest_Version_mt.txt"))
                        {
                            Console.Write("Текущая версия: ");
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(args[0]);
                            Console.WriteLine();
                            Console.ResetColor();
                            Console.Write("Последняя версия: ");
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(cli.DownloadString("https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/lastest_Version_mt.txt"));
                            Console.ResetColor();
                            Console.WriteLine();
                            SendInfo("Обновление не требуется!");
                            Console.WriteLine(@"
Опции:
1. Выйти в режим ручного управления
2. Выйти
");
                            Console.Write("> ");
                            string result = Console.ReadLine();
                            if (result == "1")
                            {
                                try
                                {
                                    Process.Start(Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName);
                                    Environment.Exit(0);
                                }catch (Exception ex) 
                                {
                                    SendError(ex.Message);
                                }
                            }
                            else
                            {
                                Environment.Exit(0);
                            }
                        }
                        else
                        {
                            Console.Write("Текущая версия: ");
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(args[0]);
                            Console.WriteLine();
                            Console.ResetColor();
                            Console.Write("Последняя версия: ");
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(cli.DownloadString("https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/lastest_Version_mt.txt"));
                            Console.ResetColor();
                            Console.WriteLine();
                            Console.WriteLine(@"
Опции:
1. Установить обновление
2. Выйти в режим ручного управления
3. Выйти
");
                        }
                        Console.Write("> ");
                        string resu = Console.ReadLine();
                        if (resu == "1") 
                        {
                            if (TestConnection()) { }
                            else
                            {
                                SendError("Операция отменена: Нет подключения к интернету!");
                                break;
                            }
                            SendWarning("Вы уверенны, что хотите удалить текущую версию терминала? (y/n) [Ваши установленные пакеты в папке Packages не будут удалены!]");
                            Console.Write("> ");
                            if (Console.ReadLine() == "y")
                            {
                                string processName = "MultiTerminal";
                                Process[] processes = Process.GetProcessesByName(processName);
                                foreach (Process process in processes)
                                {
                                    try
                                    {
                                        process.Kill();
                                    }
                                    catch (Exception ex)
                                    {
                                        SendError(ex.Message);
                                        break;
                                    }
                                }
                                WebClient cl = new WebClient();
                                if (TestConnection())
                                {

                                    try
                                    {
                                        string directoryPath = Environment.CurrentDirectory;
                                        string[] exceptionFileNames = { AppDomain.CurrentDomain.FriendlyName, "Packages" };

                                        DirectoryInfo directory = new DirectoryInfo(directoryPath);
                                        FileInfo[] files = directory.GetFiles();
                                        DirectoryInfo[] subDirectories = directory.GetDirectories();

                                        foreach (FileInfo file in files)
                                        {
                                            if (Array.IndexOf(exceptionFileNames, file.Name) == -1)
                                            {
                                                file.Delete();
                                            }
                                        }

                                        foreach (DirectoryInfo subDirectory in subDirectories)
                                        {
                                            if (Array.IndexOf(exceptionFileNames, subDirectory.Name) == -1)
                                            {
                                                subDirectory.Delete(true);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        SendError(ex.Message);
                                        break;
                                    }
                                }
                                else
                                {
                                    SendError("Операция отменена: Нет подключения к интернету!");
                                    break;
                                }

                                SendInfo("Текущая версия терминала удалена!");
                                SendInfo("Скачивание прошивки...");
                                if (TestConnection()) { }
                                else
                                {
                                    SendError("Операция отменена: Нет подключения к интернету!");
                                    break;
                                }
                                try
                                {
                                    cl.DownloadFile("https://raw.githubusercontent.com/dmitriykotik/MultiTerminal/master/update/lastest_release_mt.mpkg_update", $"{Environment.CurrentDirectory}\\lastest_release_mt.mpkg_update");
                                }
                                catch (Exception exa)
                                {
                                    SendError(exa.Message);
                                    break;
                                }
                                if (File.Exists(Environment.CurrentDirectory + "\\lastest_release_mt.mpkg_update"))
                                {
                                    try
                                    {
                                        ZipFile.ExtractToDirectory(Environment.CurrentDirectory + "\\lastest_release_mt.mpkg_update", Environment.CurrentDirectory);
                                        SendInfo("Прошивка установлена!");
                                    }
                                    catch (Exception exb)
                                    {
                                        SendError(exb.Message);
                                    }
                                }
                                SendInfo("Удаление файла прошивки...");
                                if (File.Exists(Environment.CurrentDirectory + "\\lastest_release_mt.mpkg_update"))
                                {
                                    try
                                    {
                                        File.Delete("lastest_release_mt.mpkg_update");
                                        SendInfo("Файл прошивки удалён!");
                                    }
                                    catch (Exception ex)
                                    {
                                        SendError(ex.Message);
                                    }
                                }
                                else
                                {
                                    SendWarning("Файл прошивки не обнаружен!");
                                }
                                SendInfo("Обновление завершено!");
                                s2 = false;
                                Environment.Exit(0);
                            }
                            else
                            {
                                SendError("Операция отменена: Пользователь отменил операцию");
                                break;
                            }
                        }
                        else if (resu == "2")
                        {
                            try
                            {
                                Process.Start(Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName);
                                Environment.Exit(0);
                            }
                            catch (Exception ex)
                            {
                                SendError(ex.Message);
                            }
                        }
                        else
                        {
                            Environment.Exit(0);
                        }

                    }
                    else
                    {
                        SendWarning("Нет подключения к интернету!");
                    }
                }
            }
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
