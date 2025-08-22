using MultiTerminal.FileSystem;
using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class CMDGenerator
    {
        internal static string GenPrefix(bool WriteToConsole = false)
        {
            Basic basic = new();
            string displayDir;
            if (basic.GetCurrent() == basic.GetHome())
                displayDir = "~";
            else if (basic.GetCurrent().StartsWith(basic.GetHome() + Path.DirectorySeparatorChar))
                displayDir = "~" + basic.GetCurrent().Substring(basic.GetHome().Length);
            else
                displayDir = basic.GetCurrent();

            if (WriteToConsole)
            {
                output.Write(basic.GetUsername(), settingsFile.UserColor);
                output.Write("@");
                output.Write(basic.GetHostname(), settingsFile.HostColor);
                output.Write(":");
                output.Write(displayDir, settingsFile.DirColor);
                output.Write("$ ");
            }

            return $"{basic.GetUsername()}@{basic.GetHostname()}:{displayDir}$";
        }
    }
}
