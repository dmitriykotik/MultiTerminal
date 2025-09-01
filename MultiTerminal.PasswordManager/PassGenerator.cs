using MultiTerminal.Logger;
using MultiTerminal.Output;
using MultiTerminal.Input;
using System.Security.Cryptography;
using System.Text;
using MultiTerminal.Arguments;

namespace MultiTerminal.PasswordManager
{
    #region CLASS | PassGenerator
    /// <summary>
    /// Класс генератора пароля
    /// </summary>
    public class PassGenerator
    {
        #region METHOD-VOID | Run
        /// <summary>
        /// Запуск программы генератора
        /// </summary>
        /// <param name="Args">Аргументы</param>
        /// <param name="log">Логгер</param>
        public static void Run(string[] Args, Log log)
        {
            var output = new DefaultOutput();
            var input = new DefaultInput();
            Dictionary<string, string> p = DefaultArguments.ParseArgs(Args);

            int GetInt(string[] keys, int @default)
            {
                foreach (var k in keys)
                    if (p.TryGetValue(k, out var v) && int.TryParse(v, out var iv))
                        return Math.Max(1, iv);
                return @default;
            }

            bool GetBool(string[] keys, bool @default)
            {
                foreach (var k in keys)
                    if (p.TryGetValue(k, out var v) && bool.TryParse(v, out var bv))
                        return bv;
                if (keys.Length == 1 && p.ContainsKey(keys[0]) && string.IsNullOrEmpty(p[keys[0]]))
                    return true;
                return @default;
            }

            int length = GetInt(["-l", "--length"], 12);
            int count = GetInt(["-c", "--count"], 1);
            bool lower = GetBool(["--lower"], true);
            bool upper = GetBool(["--upper"], true);
            bool digits = GetBool(["--digits"], true);
            bool symbols = GetBool(["--symbols"], false);

            var sb = new StringBuilder();
            if (lower) sb.Append("abcdefghijklmnopqrstuvwxyz");
            if (upper) sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (digits) sb.Append("0123456789");
            if (symbols) sb.Append("!@#$%^&*()-_=+[]{};:,.<>?");

            if (sb.Length == 0)
            {
                log.Write(Logger.LogType.Error, "passgen: No character type selected!");
                output.WriteLine("passgen: No character type selected!", ConsoleColor.Red);
                return;
            }

            var charset = sb.ToString();
            var buf = new byte[length];
            using var rng = RandomNumberGenerator.Create();

            for (int i = 0; i < count; i++)
            {
                rng.GetBytes(buf);
                var pass = new char[length];
                for (int j = 0; j < length; j++)
                    pass[j] = charset[buf[j] % charset.Length];
                output.WriteLine("Generated password: " + new string(pass));
                log.Write(Logger.LogType.Info, "passgen: Password generated");
            }
        }
        #endregion
    }
    #endregion
}
