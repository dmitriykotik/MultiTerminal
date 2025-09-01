using MultiAPI;

namespace MultiTerminal.Logger
{
    #region CLASS | Log
    /// <summary>
    /// Логгер
    /// </summary>
    public class Log
    {
        private string _File;

        #region METHOD-Log | Log
        /// <summary>
        /// Создать логгер
        /// </summary>
        /// <param name="File">Лог файл</param>
        /// <param name="enablePrefix">Включить префикс даты перед названием файла?</param>
        public Log(string File, bool enablePrefix = true)
        {
            string prefix = "";
            if (enablePrefix)
                prefix = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-";
            _File = prefix + File;
        }

        /// <summary>
        /// Создать логгер
        /// </summary>
        /// <param name="File">Лог файл</param>
        /// <param name="Folder">Папка с логами</param>
        /// <param name="enablePrefix">Включить префикс даты перед названием файла?</param>
        public Log(string File, string Folder, bool enablePrefix = true)
        {
            if (!FileManager.Directory.Exists(Folder))
                FileManager.Directory.Create(Folder);

            string prefix = "";
            if (enablePrefix)
                prefix = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-";

            _File = Folder + "\\" + prefix + File;
        }
        #endregion

        #region PRIVATE~METHOD-STRING | GetString
        /// <summary>
        /// Создание строки
        /// </summary>
        /// <param name="TimeStamp">Временная метка</param>
        /// <param name="LogType">Тип логирования</param>
        /// <param name="Message">Сообщение</param>
        /// <returns>Строка</returns>
        private static string GetString(DateTime TimeStamp, LogType LogType, string? Message) => $"[{TimeStamp:dd-MM-yyyy HH:mm:ss}] [{GetStringType(LogType)}] {Message}";
        #endregion

        #region PRIVATE~METHOD-STRING | GetStringType
        /// <summary>
        /// Получение строки типа логирования
        /// </summary>
        /// <param name="LogType">Тип логирования</param>
        /// <returns>Строка</returns>
        private static string GetStringType(LogType LogType)
        {
            switch (LogType)
            {
                case LogType.Error:
                    return "Error";
                case LogType.Info:
                    return "Info";
                case LogType.Warning:
                    return "Warning";
                case LogType.Debug:
                    return "Debug";
                case LogType.CriticalError:
                    return "CRITICAL ERROR";
                case LogType.Exception:
                    return "Exception";
                default:
                    return "Unknown";
            }
        }
        #endregion

        #region METHOD-VOID | WriteData
        /// <summary>
        /// Записать данные в лог файл
        /// </summary>
        /// <param name="Data">Данные</param>
        public void WriteData(SLogger Data) => FileManager.File.AppendAllText(_File, "\n" + GetString(Data.Timestamp, Data.LogType, Data.Message));
        #endregion

        #region METHOD-VOID | Write
        /// <summary>
        /// Создать конструкцию данных и записать её в лог файл
        /// </summary>
        /// <param name="LogType">Тип логгирования</param>
        /// <param name="Message">Сообщение</param>
        public void Write(LogType LogType, string Message) => FileManager.File.AppendAllText(_File, "\n" + GetString(DateTime.Now, LogType, Message));
        #endregion

        #region METHOD-VOID | WriteUTC
        /// <summary>
        /// Создать конструкцию данных, с указанием времени в формате UTC, и записать её в лог файл
        /// </summary>
        /// <param name="LogType">Тип логгирования</param>
        /// <param name="Message">Сообщение</param>
        public void WriteUTC(LogType LogType, string Message) => FileManager.File.AppendAllText(_File, "\n" + GetString(DateTime.Now.ToUniversalTime(), LogType, Message));
        #endregion
    }
    #endregion
}
