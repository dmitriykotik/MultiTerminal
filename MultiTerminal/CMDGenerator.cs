using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class CMDGenerator
    {
        internal static string GenPrefix(bool WriteToConsole = false)
        {
            string displayDir;
            if (fileSystem.GetCurrent() == fileSystem.GetHome())
                displayDir = "~";
            else if (fileSystem.GetCurrent().StartsWith(fileSystem.GetHome() + Path.DirectorySeparatorChar))
                displayDir = "~" + fileSystem.GetCurrent().Substring(fileSystem.GetHome().Length);
            else
                displayDir = fileSystem.GetCurrent();

            if (WriteToConsole)
            {
                output.Write(fileSystem.GetUsername(), settingsFile.UserColor);
                output.Write("@");
                output.Write(fileSystem.GetHostname(), settingsFile.HostColor);
                output.Write(":");
                output.Write(displayDir, settingsFile.DirColor);
                output.Write("$ ");
            }

            return $"{fileSystem.GetUsername()}@{fileSystem.GetHostname()}:{displayDir}$";
        }
    }
}
