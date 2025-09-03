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

            commands.RegisterCommand("passgen", Commands.PasswordGenerator);
            commands.RegisterCommand("passmgr", Commands.PasswordManager);

            commands.RegisterCommand("cd", Commands.Cd);
            commands.RegisterCommand("cd..", Commands.CdBack);
            commands.RegisterCommand("pwd", Commands.Pwd);
            commands.RegisterCommand(["ls", "dir", "list"], Commands.Ls);
            commands.RegisterCommand("mkdir", Commands.Mkdir);
            commands.RegisterCommand("touch", Commands.Touch);
            commands.RegisterCommand(["del", "delete", "rm", "remove"], Commands.Rm);
            commands.RegisterCommand("rmdir", Commands.Rmdir);
            commands.RegisterCommand(["mv", "move"], Commands.Mv);
            commands.RegisterCommand(["cp, copy"], Commands.Cp);
            commands.RegisterCommand("find", Commands.Find);
            commands.RegisterCommand("tree", Commands.Tree);

            commands.RegisterCommand("exit", Commands.Exit);
        }



        internal static void loadPlugins() => plugins.LoadPlugins(commands);

        internal static void unloadPlugins() => plugins.UnloadPlugins();
    }
}
