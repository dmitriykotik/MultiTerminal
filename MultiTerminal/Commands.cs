using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class Commands
    {
        internal static void Help(string[] Args)
        {

        }

        internal static void Exit()
        {
            stop.Push(0);
        }
    }
}
