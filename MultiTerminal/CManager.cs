using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class CManager
    {
        internal static void loadCommands()
        {
            commands.RegisterCommand("help", Commands.Help);

            commands.RegisterCommand("setusercolor", (Args) => Commands.ChangeUserColor(Args, 0));
            commands.RegisterCommand("sethostcolor", (Args) => Commands.ChangeUserColor(Args, 1));
            commands.RegisterCommand("setdircolor", (Args) => Commands.ChangeUserColor(Args, 2));

            commands.RegisterCommand(["cls", "clear"], (Args) => Commands.Clear());

            commands.RegisterCommand("exit", (Args) => Commands.Exit());
        }

        internal static void loadPlugins() => plugins.LoadPlugins(commands);

        internal static void unloadPlugins() => plugins.UnloadPlugins();
    }
}
