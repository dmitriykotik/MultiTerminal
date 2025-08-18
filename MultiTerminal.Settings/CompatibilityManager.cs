using MultiAPI;

namespace MultiTerminal.Settings
{
    #region CLASS | CompatibilityManager
    /// <summary>
    /// Класс упрвления совместимости 
    /// </summary>
    public class CompatibilityManager
    {
        #region Private vars
        private SMBIOS _smbios;
        private OS.OSTypes[] _supportedOS;
        #endregion

        #region METHOD-CompatibilityManager | CompatibilityManager
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="supportedOS">Поддерживаемые операционные системы</param>
        /// <param name="smbios">SMBIOS</param>
        public CompatibilityManager(OS.OSTypes[] supportedOS, SMBIOS smbios) 
        { 
            _supportedOS = supportedOS;
            _smbios = smbios; 
        }
        #endregion

        #region METHOD-Compatibility | Get
        /// <summary>
        /// Получение совместимости
        /// </summary>
        /// <returns>Compatibility</returns>
        public Compatibility Get()
        {
            if (!_smbios.EnableSMBIOS)
                return new() { CompatibilityMode = OS.isSupported(_supportedOS) };

            var contains = _supportedOS.Contains(_smbios.OperatingSystem);
            return new() { CompatibilityMode = contains ? contains : false };
        }
        #endregion

        #region METHOD-OSTypes | GetOS
        /// <summary>
        /// Получения операционной системы с учётом SMBIOS
        /// </summary>
        /// <returns>Операционная система</returns>
        public OS.OSTypes GetOS()
        {
            if (!_smbios.EnableSMBIOS)
                return OS.Get();
            return _smbios.OperatingSystem;
        }
        #endregion

        #region METHOD-VOID | Run
        /// <summary>
        /// Выполнение функций с обработкой совместимости
        /// </summary>
        /// <param name="compatibility">Совместимый метод</param>
        /// <param name="notCompatibility">Не совместимый метод</param>
        public void Run(Action compatibility, Action notCompatibility)
        {
            if (Get().CompatibilityMode)
                compatibility.Invoke();
            else
                notCompatibility.Invoke();
        }
        #endregion
    }
    #endregion
}
