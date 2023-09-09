using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace addons_XP_MusicBox
{
    internal class Program
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


        //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        static WMPLib.WindowsMediaPlayer w = new WMPLib.WindowsMediaPlayer();
        static void Main(string[] args)
        {
            Console.WriteLine("Разрешаете ли вы очистку окна консоли? (y/n)");
            if (Console.ReadLine().ToLower() == "y")
            {
                if (args.Length == 0)
                {
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine(@"Выберите действие:
1. Воспроизвести музыкальный файл
2. Остановить музыкальный файл
3. Приостановить музыкальный файл
4. Продолжить воспроизведение музыки
5. Выйти");
                        string res = Console.ReadLine();
                        switch (res)
                        {
                            case "1":
                                Console.WriteLine("Пожалуйста, введите полный путь к музыкальному файлу:");
                                try
                                {
                                    w.URL = Console.ReadLine();
                                    w.controls.play();
                                }
                                catch (Exception ex)
                                {
                                    SendError(ex.Message);
                                }
                                break;
                            case "2":
                                try
                                {
                                    w.controls.stop();
                                }
                                catch (Exception ex)
                                {
                                    SendError(ex.Message);
                                }
                                break;
                            case "3":
                                try
                                {
                                    w.controls.pause();
                                }
                                catch (Exception ex)
                                {
                                    SendError(ex.Message);
                                }
                                break;
                            case "4":
                                try
                                {
                                    w.controls.play();
                                }
                                catch (Exception ex)
                                {
                                    SendError(ex.Message);
                                }
                                break;
                            case "5":
                                Environment.Exit(0);
                                break;
                            default:
                                break;

                        }
                    }
                }
                else
                {
                    try
                    {
                        w.URL = args.ToString();
                        w.controls.play();
                    }
                    catch (Exception ex)
                    {
                        SendError(ex.Message);
                    }
                }
            }
            if (args.Length == 0)
            {
                while (true)
                {
                    Console.WriteLine(@"Выберите действие:
1. Воспроизвести музыкальный файл
2. Остановить музыкальный файл
3. Приостановить музыкальный файл
4. Продолжить воспроизведение музыки
5. Выйти");
                    string res = Console.ReadLine();
                    switch (res)
                    {
                        case "1":
                            Console.WriteLine("Пожалуйста, введите полный путь к музыкальному файлу:");
                            try
                            {
                                w.URL = Console.ReadLine();
                                w.controls.play();
                            }
                            catch (Exception ex)
                            {
                                SendError(ex.Message);
                            }
                            break;
                        case "2":
                            try
                            {
                                w.controls.stop();
                            }
                            catch (Exception ex)
                            {
                                SendError(ex.Message);
                            }
                            break;
                        case "3":
                            try
                            {
                                w.controls.pause();
                            }
                            catch (Exception ex)
                            {
                                SendError(ex.Message);
                            }
                            break;
                        case "4":
                            try
                            {
                                w.controls.play();
                            }
                            catch (Exception ex)
                            {
                                SendError(ex.Message);
                            }
                            break;
                        case "5":
                            Environment.Exit(0);
                            break;
                        default:
                            break;

                    }
                }
            }
            else
            {
                try
                {
                    w.URL = args.ToString();
                    w.controls.play();
                }
                catch (Exception ex)
                {
                    SendError(ex.Message);
                }
            }
        }
    }
}
