using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class Commands
    {
        internal static void Help(string[] Args)
        {

        }

        internal static void ChangeUserNameColor(string colorArg)
        {
            if (Enum.TryParse<UserNameColor>(colorArg, true, out var color))
            {
                CMDGenerator.currentUserNameColor = color;
                output.WriteLine($"User name color changed to {color}");
            }
            else
            {
                output.WriteLine("Unknown color. Available colors: Red, Green, Blue, Yellow, Cyan, Magenta, Default");
            }
        }

        internal static void Exit()
        {
            stop.Push(0);
        }
    }
}
