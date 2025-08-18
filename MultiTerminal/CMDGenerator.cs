using MultiTerminal.FileSystem;

namespace MultiTerminal
{
    internal class CMDGenerator
    {
        internal static string genPrefix()
        {
            Basic basic = new();
            string displayDir;
            if (basic.GetCurrent() == basic.GetHome())
                displayDir = "~";
            else if (basic.GetCurrent().StartsWith(basic.GetHome() + Path.DirectorySeparatorChar))
                displayDir = "~" + basic.GetCurrent().Substring(basic.GetHome().Length);
            else
                displayDir = basic.GetCurrent();

            return $"{basic.GetUsername()}@{basic.GetHostname()}:{displayDir}$ ";
        }
    }
}
