using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class CManager
    {
        internal static void loadCommands()
        {
            commands.RegisterCommand("help", Commands.Help);

            commands.RegisterCommand("exit", (Args) => Commands.Exit());
        }

        internal static void loadPlugins() => plugins.LoadPlugins(commands);

        internal static void unloadPlugins() => plugins.UnloadPlugins();
    }
}
