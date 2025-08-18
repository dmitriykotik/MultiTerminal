namespace MultiTerminal.Logger
{
    #region CLASS | SLogger
    /// <summary>
    /// Класс данных
    /// </summary>
    public class SLogger
    {
        /// <summary>
        /// Временная метка
        /// </summary>
        public DateTime Timestamp;
        /// <summary>
        /// Тип логирования
        /// </summary>
        public LogType LogType;
        /// <summary>
        /// Сообщение
        /// </summary>
        public string? Message;
    }
    #endregion
}
