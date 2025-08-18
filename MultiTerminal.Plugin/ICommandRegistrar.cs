namespace MultiTerminal.Plugin
{
    #region INTERFACE | ICommandRegistrar
    /// <summary>
    /// Интерфейс регистрации команд
    /// </summary>
    public interface ICommandRegistrar
    {
        #region METHOD-VOID | RegisterCommand
        /// <summary>
        /// Регистрация команды
        /// </summary>
        /// <param name="commandName">Имя команды</param>
        /// <param name="handler">Действие, выполняемое при вызове команды</param>
        void RegisterCommand(string commandName, Action<string[]> handler);
        #endregion
    }
    #endregion
}
