using MultiTerminal.Arguments;
using MultiTerminal.Logger;
using MultiTerminal.Process;
using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class Launch
    {
        internal static bool LaunchProgram(string input, out string _output)
        {
            string _input = input;
            bool RunAs = FindLocalFlag.Find(_input, "runas", out _input);
            bool IgnoreSysPath = FindLocalFlag.Find(_input, "ignorepath", out _input);
            bool UseOnlyLocal = FindLocalFlag.Find(_input, "onlylocal", out _input);

            _output = _input;

            string RunAsAdmin = "Launching error, run terminal as Administrator";
            string[] Args = DefaultArguments.SplitArgs(_input);

            ProcessManager p;

            if (File.Exists(Args[0]))
                p = new(Args[0], string.Join(" ", DefaultArguments.RFArg(Args)));
            else if (File.Exists(Args[0] + ".exe"))
                p = new(Args[0] + ".exe", string.Join(" ", DefaultArguments.RFArg(Args)));
            else if (settings.ContainsFilePath(Args[0], !IgnoreSysPath) && !UseOnlyLocal)
            {
                var filepath = settings.GetFilePath(Args[0], !IgnoreSysPath);
                if (filepath != null)
                    p = new(filepath, string.Join(" ", DefaultArguments.RFArg(Args)));
                else
                {
                    log.Write(LogType.Error, Args[0] + ": File not exists in path file.");
                    return false;
                }
            }
            else
            {
                log.Write(LogType.Error, Args[0] + ": File not exists");
                return false;
            }

            try
            {
                if (!RunAs)
                {
                    if (!p.Launch())
                    {
                        log.Write(LogType.Error, $"{Args[0]}: {RunAsAdmin}");
                        return false;
                    }
                    p.Wait();
                    return true;
                }
                else
                {
                    if (!p.LaunchAs())
                    {
                        log.Write(LogType.Error, $"{Args[0]}: {RunAsAdmin}");
                        return false;
                    }
                    p.Wait();
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Write(LogType.Error, $"{Args[0]}: EXCEPTION EXECUTED: {ex.Message}");
                return false;
            }
        }

        internal static void CommandNotFound(string Program)
        {
            output.WriteLine($@"'{Program}' is not recognized as an internal or external command,
operable program or batch file.", ConsoleColor.Red, null);
        }
    }
}
