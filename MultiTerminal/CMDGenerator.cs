using MultiTerminal.FileSystem;

namespace MultiTerminal
{
    internal enum UserNameColor
    {
        White,
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta
    }

    internal class CMDGenerator
    {
        public static UserNameColor currentUserNameColor = UserNameColor.Red;
        internal static ConsoleColor GetUserNameColor()
        {
            return currentUserNameColor switch
            {
                UserNameColor.Red => ConsoleColor.Red,
                UserNameColor.Green => ConsoleColor.Green,
                UserNameColor.Blue => ConsoleColor.Blue,
                UserNameColor.Yellow => ConsoleColor.Yellow,
                UserNameColor.Cyan => ConsoleColor.Cyan,
                UserNameColor.Magenta => ConsoleColor.Magenta,
                _ => Console.ForegroundColor
            };
        }

        internal static (string userName, string hostName, string displayDir) genPrefix()
        {
            Basic basic = new();
            string displayDir;
            if (basic.GetCurrent() == basic.GetHome())
                displayDir = "~";
            else if (basic.GetCurrent().StartsWith(basic.GetHome() + Path.DirectorySeparatorChar))
                displayDir = "~" + basic.GetCurrent().Substring(basic.GetHome().Length);
            else
                displayDir = basic.GetCurrent();

            return (basic.GetUsername(), basic.GetHostname(), displayDir);
        }
    }
}
