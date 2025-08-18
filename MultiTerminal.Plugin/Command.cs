namespace MultiTerminal.Plugin
{
    #region CLASS | Command
    /// <summary>
    /// Представляет команду с именем и обработчиком выполнения
    /// </summary>
    public class Command
    {
        #region STRING | Name
        /// <summary>
        /// Имя команды
        /// </summary>
        public string Name { get; }
        #endregion

        #region Action<string[]> | Handler
        /// <summary>
        /// Делегат, указывающий обработчик команды. Принимает массив аргументов
        /// </summary>
        public Action<string[]> Handler { get; }
        #endregion

        #region METHOD-Command | Command
        /// <summary>
        /// Создаёт экземпляр команды с заданным именем и обработчиком
        /// </summary>
        /// <param name="name">Имя команды</param>
        /// <param name="handler">Обработчик выполнения команды, принимающий аргументы</param>
        public Command(string name, Action<string[]> handler)
        {
            Name = name;
            Handler = handler;
        }
        #endregion
    }
    #endregion
}
