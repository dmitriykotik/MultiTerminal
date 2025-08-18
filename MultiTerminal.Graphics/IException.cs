namespace MultiTerminal.Output
{
    #region CLASS | IException
    /// <summary>
    /// Интерфес обработчика исключений
    /// </summary>
    public interface IException
    {
        /// <summary>
        /// Если вызвано исключение...
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void Start(Exception exception);
    }
    #endregion
}