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
            log.Write(LogType.Debug, "Launching without args...");
            output.WriteLine($"Welcome to {settingsFile.Product.Name}!", ConsoleColor.Black, ConsoleColor.Gray);

            log.Write(LogType.Debug, "Checking compatibility...");
            Check.compatibilityWarning();

            log.Write(LogType.Debug, "Writing motd message... Use variable 'motd' in settings file to change motd message, \"null\" - don't write motd message.");
            if (settingsFile.Motd != "null")
                Console.WriteLine(settingsFile.Motd);

            while (true)
            {
                var prefix = CMDGenerator.genPrefix();
                var perm = PermissionsManager.Get();
                string titlePrefix = "";
                if (perm == Permissions.SuperAdmin)
                    titlePrefix = "WinPE: ";
                else if (perm == Permissions.Admin)
                    titlePrefix = "Admin: ";
                Console.Title = titlePrefix + prefix;

                output.Write(prefix);
                var command = input.ReadLine();
                if (string.IsNullOrEmpty(command)) continue;

                if (!commands.ExecuteCommand(command))
                {
                    CommandNotFound(DefaultArguments.SplitArgs(command));
                }
            }
        }

        private static void CommandNotFound(string[] input)
        {
            output.WriteLine($@"'{input[0]}' is not recognized as an internal or external command,
operable program or batch file.", ConsoleColor.Red, null);
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
































/*
 * Нет, тут нет полезной информации. Вообще, сейчас на дворе 15.07.2025.
 * Мне 16, на улице лето, а я мижу дома и пишу код. И нет, это не потому,
 * что мне это нравится, а потому, что мне нечего делать. У меня нет ни
 * друзей, ни знакомых, с которыми я бы мог провести своё время вне дома.
 * Я пишу код только, чтобы провести хотя бы как то своё время, но я
 * понял, что мне это становится менее интересно. В сети столько
 * пафосников, которые пишут: "Ух ебать, я лучший кодер, я фигачу самый
 * лучший код", а в итоге их обсирают. Я не пишу, что я хороший кодер,
 * но и не пишу, что плохой, хоть так и есть. Этим заниматься нет
 * никакого смысла, когда нет никакого фидбэка, даже плохих отзывов.
 * Я не опредилися кем хочу стать, я пересмотрел кучу вариантов и
 * нигде не нашёл себя лучше, чем в коде, но эта ниша занята, да
 * и нейронки потихоньку занимают её тоже. Я ничему так и не научился.
 * Достаточно мне взглянуть в окно на реальный мир... Так и хочется
 * выйти погулять, но скучно. Думаю, что MultiTerminal будет моим
 * последним проектом. Я ничего не добился, хах... Друзей нет, зато
 * есть комплексы, вау. Круто... Я столько думал о суициде, и как будто
 * это лучший вариант из возможных в моей жизне. Не думаю, что кто-то
 * даже увидит этот комментарий или вообще в принципе проект, так что
 * пускай это тут останется. Я неудачник. Я понимаю, что код - это не
 * дневник, но кто мне запретит? Этот год был самым худшим, ужасный, 
 * начиная депрессией, заканчивая выгоранием и мыслями о суициде.
 * Мне это не нравится. Если ты это читаешь, то просто смирись с тем,
 * что я тут написал. У меня всё.
 */