namespace MultiTerminal.Settings
{
    #region CLASS | SettingsFile
    /// <summary>
    /// Класс содержащий конфигурационный файл
    /// </summary>
    public class SettingsFile
    {
        /// <summary>
        /// Продукт
        /// </summary>
        public required Product Product { get; set; }
        /// <summary>
        /// Отключить предупреждения о несовместимости
        /// </summary>
        public bool disableWarningCompatibility { get; set; }
        /// <summary>
        /// Игнорировать новые версии
        /// </summary>
        public bool ignoreNewVersions { get; set; }
        /// <summary>
        /// SMBIOS
        /// </summary>
        public required SMBIOS SMBIOS { get; set; }
        /// <summary>
        /// Paths
        /// </summary>
        public required List<string> Paths { get; set; } 
        /// <summary>
        /// Сообщение при входе
        /// </summary>
        public required string Motd { get; set; }
        /// <summary>
        /// Пропуск проверки хэшей
        /// </summary>
        public bool SkipHashCheck { get; set; }
    }
    #endregion
}
