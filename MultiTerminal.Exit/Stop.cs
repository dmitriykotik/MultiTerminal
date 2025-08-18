using Microsoft.ML.Transforms;
using MultiTerminal.Output;

namespace MultiTerminal.ExitFunc
{
    #region CLASS | Stop
    /// <summary>
    /// Класс помогает завершить программу
    /// </summary>
    public class Stop
    {
        #region Vars
        private List<ExitCode> _errorCodes;
        private Logger.Log _log;
        #endregion

        #region METHOD-Stop | Stop
        /// <summary>
        /// Конструктор класса Stop
        /// </summary>
        /// <param name="errorCodes">Лист с кодами ошибок</param>
        /// <param name="log">Логгер</param>
        public Stop(List<ExitCode> errorCodes, Logger.Log log)
        {
            _errorCodes = errorCodes;
            _log = log;
        }
        #endregion

        #region METHOD-VOID | Push
        /// <summary>
        /// Завершение процесса
        /// </summary>
        /// <param name="errorCode">Код ошибки</param>
        public void Push(int errorCode)
        {
            DefaultOutput output = new();
            if (Contains(errorCode))
            {
                var code = GetByCode(errorCode);
                if (code != null)
                {
                    if (!code.CriticalError) 
                        _log.Write(Logger.LogType.Info, $"The process was completed. Error code: {code.ErrorCode}. Brief information: {code.Information}.");
                    else
                        _log.Write(Logger.LogType.CriticalError, $"The process was completed. Error code: {code.ErrorCode}. Brief information: {code.Information}.");
                }
                else
                    _log.Write(Logger.LogType.Info, $"The process was completed. Error code: {errorCode}.");
            }
            else
                _log.Write(Logger.LogType.Info, $"The process was completed. Error code: {errorCode}.");
            output.WriteLine("THE TERMINAL WAS STOPPED! ERROR CODE: " + errorCode, ConsoleColor.White, ConsoleColor.Red);
            Environment.Exit(errorCode);
        }
        #endregion

        #region PRIVATE~METHOD-BOOL | Contains
        private bool Contains(int errorCode) => _errorCodes.Any(c => c.ErrorCode == errorCode);
        #endregion

        #region PRIVATE~METHOD-ExitCode | GetByCode
        private ExitCode? GetByCode(int errorCode) => _errorCodes.FirstOrDefault(c => c.ErrorCode == errorCode);
        #endregion
    }
    #endregion
}
