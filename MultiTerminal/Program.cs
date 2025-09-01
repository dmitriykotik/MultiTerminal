using MultiTerminal.Logger;
using static MultiTerminal.InternalVars;
using MultiTerminal.Arguments;
using MultiTerminal.Settings;

#if DEBUG
#warning Debug mode is enabled! Disable debug mode during the final build!
#endif

namespace MultiTerminal
{
    internal class Program
    {
        internal static void Main(string[] args) 
        {
            Console.Title = settingsFile.Product.Name;
            log.Write(LogType.Debug, $"[{settingsFile.Product.Name} Started]");

            if (!Check.CheckHashFiles())
                stop.Push(2);
            output.Clear();

            #if DEBUG
            DebugMode();
            #endif

            log.Write(LogType.Debug, "Checking for operating system support...");

            if (!Check.supportOs()) stop.Push(1);

            CManager.loadCommands(); 
            CManager.loadPlugins();
            
            if (args.Length <= 0) MainWithoutArgs();
            else MainWithArgs(args);
        }

        private static void MainWithArgs(string[] args)
        {
            log.Write(LogType.Debug, "Launching with args...");
            log.Write(LogType.Debug, $"string[] args = {string.Join("; ", DefaultArguments.SplitArgs(string.Join(" ", args)))}");
            #if DEBUG
            if (args[0] == "--test")
            {
                TestMode();
                return;
            }
            #endif
        }

        private static void MainWithoutArgs()
        {
            Console.CancelKeyPress += (s, e) => { output.WriteLine(); e.Cancel = true; };
            log.Write(LogType.Debug, "Launching without args...");
            output.WriteLine($"Welcome to {settingsFile.Product.Name}!", ConsoleColor.Black, ConsoleColor.Gray);

            log.Write(LogType.Debug, "Checking compatibility...");
            Check.compatibilityWarning();

            log.Write(LogType.Debug, "Writing motd message... Use variable 'motd' in settings file to change motd message, \"null\" - don't write motd message.");
            if (settingsFile.Motd != "null")
                Console.WriteLine(settingsFile.Motd);

            while (true)
            {
                var perm = PermissionsManager.Get();
                string titlePrefix = "";
                if (perm == Permissions.SuperAdmin)
                    titlePrefix = "WinPE: ";
                else if (perm == Permissions.Admin)
                    titlePrefix = "Admin: ";
                Console.Title = titlePrefix + CMDGenerator.GenPrefix();

                CMDGenerator.GenPrefix(true);
                var command = input.ReadLine();
                if (string.IsNullOrEmpty(command)) continue;

                if (!commands.ExecuteCommand(command))
                {
                    if (!Launch.LaunchProgram(command, out command))
                        Launch.CommandNotFound(DefaultArguments.SplitArgs(command)[0]);
                }
            }
        }

#if DEBUG
        private static void DebugMode()
        {
            output.WriteLine("The terminal is running in debug mode! Disable debug mode before release!", ConsoleColor.White, ConsoleColor.Red);
        }

        private static void TestMode()
        {
            Hashes hashes = new("all.hash", "cfg");
            foreach (var hash in hashes.Gen().Hashes)
            {
                output.WriteLine($"{hash.FileName} - {hash.FileHash}");
            }
            input.ReadLine();
        }
#endif
    }
}