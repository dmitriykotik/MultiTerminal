using MultiAPI;
using System.ComponentModel;
using Proc = System.Diagnostics.Process;

namespace MultiTerminal.Process
{
    #region CLASS | ProcessManager
    /// <summary>
    /// Класс для работы с приложениями
    /// </summary>
    public class ProcessManager
    {
        #region Private vars
        private Proc _process;
        #endregion

        #region METHOD-ProcessManager
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="File">Файл</param>
        /// <param name="Arguments">Аргументы</param>
        /// <param name="UseShellExecute">Использовать оболочку системы?</param>
        /// <param name="CreateNoWindow">Создать без окна?</param>
        public ProcessManager(string File, string? Arguments = null, bool UseShellExecute = false, bool CreateNoWindow = false)
        {
            _process = new Proc();
            _process.StartInfo.FileName = File;
            _process.StartInfo.Arguments = Arguments == null ? "" : Arguments;
            _process.StartInfo.UseShellExecute = UseShellExecute;
            _process.StartInfo.CreateNoWindow = CreateNoWindow;
        }
        #endregion

        #region METHOD-VOID | Launch
        /// <summary>
        /// Запуск приложения в обычном режиме
        /// </summary>
        /// <exception cref="Exceptions.NullField">Нулевое значение</exception>
        public bool Launch()
        {
            if (_process != null)
            {
                try
                {
                    _process.StartInfo.Verb = "";
                    _process.Start();
                    return true;
                }
                catch (Win32Exception) { return LaunchAs(); }
            }
            else
                throw new Exceptions.NullField("MultiTerminal.Process -> ProcessManager -> Launch : if (_process != null) : _process = null");
        }
        #endregion

        #region METHOD-VOID | LaunchAs
        /// <summary>
        /// Запуск приложения от имени администратора
        /// </summary>
        /// <exception cref="Exceptions.NullField">Нулевое значение</exception>
        public bool LaunchAs()
        {
            if (_process != null)
            {
                try
                {
                    var temp = _process.StartInfo.UseShellExecute;
                    _process.StartInfo.UseShellExecute = true;
                    _process.StartInfo.Verb = "runas";
                    _process.Start();
                    _process.StartInfo.UseShellExecute = temp;
                    return true;
                }
                catch (Win32Exception) { return false; }
            }
            else
                throw new Exceptions.NullField("MultiTerminal.Process -> ProcessManager -> LaunchAs : if (_process != null) : _process = null");
        }
        #endregion

        #region METHOD-VOID | Wait
        /// <summary>
        /// Ждать завершения
        /// </summary>
        /// <exception cref="Exceptions.NullField">Нулевое значение</exception>
        public void Wait()
        {
            if (_process != null)
                _process.WaitForExit();
            else
                throw new Exceptions.NullField("MultiTerminal.Process -> ProcessManager -> LaunchAs : if (_process != null) : _process = null");
        }
        #endregion

        #region METHOD-VOID | Close
        /// <summary>
        /// Завершить приложение
        /// </summary>
        /// <exception cref="Exceptions.NullField">Нулевое значение</exception>
        public void Close()
        {
            if (_process != null)
                _process.Close();
            else
                throw new Exceptions.NullField("MultiTerminal.Process -> ProcessManager -> LaunchAs : if (_process != null) : _process = null");
        }
        #endregion

        #region METHOD-VOID | Reconf
        /// <summary>
        /// Быстрая реконфигурация
        /// </summary>
        /// <param name="File">Файл</param>
        /// <param name="Arguments">Аргументы</param>
        /// <param name="UseShellExecute">Использовать оболочку системы?</param>
        /// <param name="CreateNoWindow">Создать без окна?</param>
        public void Reconf(string File, string? Arguments = null, bool UseShellExecute = false, bool CreateNoWindow = false)
        {
            _process = new Proc();
            _process.StartInfo.FileName = File;
            _process.StartInfo.Arguments = Arguments == null ? "" : Arguments;
            _process.StartInfo.UseShellExecute = UseShellExecute;
            _process.StartInfo.CreateNoWindow = CreateNoWindow;
        }
        #endregion
    }
    #endregion
}
