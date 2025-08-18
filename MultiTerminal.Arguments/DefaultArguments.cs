using MultiAPI;
using System.Text;

namespace MultiTerminal.Arguments
{
    #region CLASS | DefaultArguments
    /// <summary>
    /// Стандартная работа с аргументами
    /// </summary>
    public class DefaultArguments
    {
        #region METHOD-STRING[] | RFArg
        /// <summary>
        /// Убрать первый аргумент
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <returns>Массив аргументов без первого аргумента</returns>
        /// <exception cref="Exceptions.NullField">Нулевое поле</exception>
        public static string[] RFArg(string args)
        {
            if (string.IsNullOrEmpty(args)) throw new Exceptions.NullField("MultiTerminal.Arguments.DefaultArguments.RFArg -> string args");

            return SplitArgs(args).Skip(1).ToArray();
        }

        /// <summary>
        /// Убрать первый аргумент (Из массива)
        /// </summary>
        /// <param name="args">Массив с аргументами</param>
        /// <returns>Массив аргументов без первого аргумента</returns>
        /// /// <exception cref="Exceptions.NullField">Нулевое поле</exception>
        public static string[] RFArg(string[] args)
        {
            if (args.Length <= 0) throw new Exceptions.NullField("MultiTerminal.Arguments.DefaultArguments.RFArg -> string[] args");

            return args.Skip(1).ToArray();
        }
        #endregion

        #region METHOD-STRING[] | SplitArgs
        /// <summary>
        /// Разбить аргументную строку на массив с аргументами
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <returns>Массив с аргументами</returns>
        /// <exception cref="Exceptions.NullField">Нулевое поле</exception>
        public static string[] SplitArgs(string args)
        {
            if (string.IsNullOrEmpty(args)) throw new Exceptions.NullField("MultiTerminal.Arguments.DefaultArguments.SplitArgs -> string args");

            List<string> _args = new List<string>();
            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < args.Length; i++)
            {
                char c = args[i];

                if (c == '\"') inQuotes = !inQuotes;
                else if (c == ' ' && !inQuotes)
                {
                    if (currentArg.Length > 0)
                    {
                        _args.Add(currentArg.ToString());
                        currentArg.Clear();
                    }
                }
                else currentArg.Append(c);
            }

            if (currentArg.Length > 0) _args.Add(currentArg.ToString());

            return _args.ToArray();
        }
        #endregion

        #region METHOD-DICTIONARY | ParseArgs
        /// <summary>
        /// Парсинг аргументов (Разделяет аргументную строку на словарь аргументов (разделяет аргументы по параметрам (- и --)))
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <returns>Словарь из аргументов</returns>
        /// <exception cref="Exceptions.NullField">Нулевое поле</exception>
        public static Dictionary<string, string> ParseArgs(string args)
        {
            if (string.IsNullOrEmpty(args)) throw new Exceptions.NullField("MultiTerminal.Arguments.DefaultArguments.ParseArgs -> string args");

            string[] _args = SplitArgs(args);

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (_args[i].StartsWith("-"))
                {
                    string key = _args[i];
                    string value = "";

                    if (i + 1 < _args.Length)
                    {
                        if (!_args[i + 1].StartsWith("-") || !_args[i + 1].StartsWith("--")) value = _args[i + 1];
                    }

                    parameters.Add(key, value);
                }
            }

            return parameters;
        }

        /// <summary>
        /// Парсинг аргументов (Разделяет массив аргументов на словарь с аргументами (разделяет аргументы по параметрам (- и --)))
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <returns>Словарь с аргументами</returns>
        public static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    string key = args[i];
                    string value = "";

                    if (i + 1 < args.Length)
                    {
                        if (!args[i + 1].StartsWith("-") || !args[i + 1].StartsWith("--")) value = args[i + 1];
                    }

                    parameters.Add(key, value);
                }
            }

            return parameters;
        }
        #endregion
    }
    #endregion
}