namespace MultiTerminal.Plugin
{
    #region INTERFACE | IPlugin
    /// <summary>
    /// Базовый интерфейс представляющий плагин
    /// </summary>
    public interface IPlugin
    {
        #region STRING | Name
        /// <summary>
        /// Имя плагина
        /// </summary>
        string Name { get; }
        #endregion

        #region METHOD-VOID | OnEnable
        /// <summary>
        /// Событие, выполняемое при загрузки плагина
        /// </summary>
        void OnEnable();
        #endregion

        #region METHOD-VOID | OnDisable
        /// <summary>
        /// Событие, выполняемое при выгрузки плагина
        /// </summary>
        void OnDisable();
        #endregion

        #region METHOD-VOID | RegisterCommands
        /// <summary>
        /// Регистратор команд
        /// </summary>
        /// <param name="registrar">Интерфейс команд</param>
        void RegisterCommands(ICommandRegistrar registrar);
        #endregion
    }
    #endregion
}
