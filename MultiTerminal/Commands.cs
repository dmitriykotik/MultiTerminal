using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class Commands
    {
        internal static void Help(string[] Args)
        {

        }

        internal static void ChangeUserColor(string[] Args, int mode = 0)
        {
            string NameMethod = "user";
            switch (mode)
            {
                case 0:
                    NameMethod = "user";
                    break;
                case 1:
                    NameMethod = "host";
                    break;
                case 2:
                    NameMethod = "dir";
                    break;
                default:
                    log.Write(Logger.LogType.Error, $"setusercolor: Incorrect mode. Selected mode: {mode}");
                    output.WriteLine("setusercolor: Incorrect mode.");
                    return;
            }

            if (Args.Length != 1)
            {
                log.Write(Logger.LogType.Error, $"set{NameMethod}color: Arguments error, usage: set{NameMethod}color <color>");
                output.WriteLine($"Usage: set{NameMethod}color <color>", ConsoleColor.Red);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(Args[0], true, out var color))
            {
                bool resultMethod = mode == 0 ? settings.SetUserColor(color) : mode == 1 ? settings.SetHostColor(color) : settings.SetDirColor(color);
                if (resultMethod)
                {
                    log.Write(Logger.LogType.Info, $"set{NameMethod}color: Text color changed to: {color}");
                    output.WriteLine($"set{NameMethod}color: Text color changed to: {color}", ConsoleColor.Green);
                }
                else
                {
                    log.Write(Logger.LogType.Error, $"set{NameMethod}color: Unknown error. Stack trace: MultiTerminal.Commands.ChangeUserColor -> MultiTerminal.Settings.SettingsManager.Set{string.Concat(NameMethod[0].ToString().ToUpper(), NameMethod.AsSpan(1))}Color");
                    output.WriteLine($"set{NameMethod}color: Failed to set color. Check log file. Try again.", ConsoleColor.Red);
                }
            }
            else
            {
                log.Write(Logger.LogType.Error, $"set{NameMethod}color: The specified color could not be recognized. Please try again or consult your documentation. Entered color: {Args[0]}");
                output.WriteLine($"set{NameMethod}color: The specified color could not be recognized. Please try again or consult your documentation.", ConsoleColor.Red);
            }
        }

        internal static void Exit()
        {
            stop.Push(0);
        }

        internal static void Clear()
        {
            log.Write(Logger.LogType.Debug, $"clear: Console cleared");
            Console.Clear();
        }
    }
}
