namespace MultiTerminal.Logger
{
    #region ENUM | LogType
    /// <summary>
    /// Тип лога
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Ошибка
        /// </summary>
        Error,
        /// <summary>
        /// Информация
        /// </summary>
        Info,
        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning,
        /// <summary>
        /// Информация для разработчика
        /// </summary>
        Debug,
        /// <summary>
        /// Критическая ошибка
        /// </summary>
        CriticalError,
        /// <summary>
        /// Исключение
        /// </summary>
        Exception
    }
    #endregion
}