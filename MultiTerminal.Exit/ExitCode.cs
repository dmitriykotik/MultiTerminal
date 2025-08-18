namespace MultiTerminal.ExitFunc
{
    #region CLASS | ExitCode
    /// <summary>
    /// Информация о кодах
    /// </summary>
    public class ExitCode
    {
        /// <summary>
        /// Код выхода
        /// </summary>
        public int ErrorCode;

        /// <summary>
        /// Информация о коде выхода
        /// </summary>
        public string? Information;

        /// <summary>
        /// Процесс завершается из-за ошибки?
        /// </summary>
        public bool CriticalError;
    }
    #endregion
}
