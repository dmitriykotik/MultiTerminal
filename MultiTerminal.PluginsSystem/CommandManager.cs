using MultiTerminal.Plugin;
using MultiTerminal.Output;
using MultiTerminal.Arguments;
using MultiAPI;

namespace MultiTerminal.PluginsSystem
{
    #region CLASS | CommandManager
    /// <summary>
    /// Управление командами
    /// </summary>
    public class CommandManager : ICommandRegistrar
    {
        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);

        #region METHOD-VOID | RegisterCommand
        /// <summary>
        /// Регистрация команды
        /// </summary>
        /// <param name="commandName">Имя команды</param>
        /// <param name="handler">Обработчик</param>
        public void RegisterCommand(string commandName, Action<string[]> handler)
        {
            var output = new DefaultOutput();
            if (!_commands.ContainsKey(commandName))
            {
                _commands[commandName] = new Command(commandName, handler);
            }
            else
                output.WriteLine($"The command \"{commandName}\" is already registered.", ConsoleColor.Red, null);
        }
        #endregion

        #region METHOD-VOID | RegisterCommand
        /// <summary>
        /// Регистрация алиасов
        /// </summary>
        /// <param name="aliases">Имя команды и её алиасов</param>
        /// <param name="handler">Обработчик</param>
        public void RegisterCommand(string[] aliases, Action<string[]> handler)
        {
            var output = new DefaultOutput();
            foreach (var commandName in aliases)
            {
                if (!_commands.ContainsKey(commandName))
                {
                    _commands[commandName] = new Command(commandName, handler);
                }
                else
                    output.WriteLine($"The command \"{commandName}\" is already registered.", ConsoleColor.Red, null);
            }
        }
        #endregion

        #region METHOD-BOOL | ExecuteCommand
        /// <summary>
        /// Выполнение команд
        /// </summary>
        /// <param name="input">Входные данные</param>
        /// <returns>true - Если комманда существует, иначе false</returns>
        public bool ExecuteCommand(string input)
        {
            var output = new DefaultOutput();
            if (string.IsNullOrWhiteSpace(input)) return false;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmdName = parts[0];
            string[] args;
            try
            {
                args = DefaultArguments.RFArg(input);
            }
            catch (Exceptions.NullField)
            {
                args = [ "" ];
            }

            if (_commands.TryGetValue(cmdName, out var command))
            {
                try
                {
                    command.Handler(args);
                }
                catch (Exception ex)
                {
                    output.WriteLine($"Error executing command \"{cmdName}\": {ex.Message}", ConsoleColor.White, ConsoleColor.Red);
                }
                return true;
            }

            return false;
        }
        #endregion
    }
    #endregion
}
