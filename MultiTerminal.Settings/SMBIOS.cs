using MultiAPI;

namespace MultiTerminal.Settings
{
    #region CLASS | SMBIOS
    /// <summary>
    /// Класс содержащий SMBIOS
    /// </summary>
    public class SMBIOS
    {
        /// <summary>
        /// SMBIOS включён?
        /// </summary>
        public bool EnableSMBIOS { get; set; }
        /// <summary>
        /// Операционная система
        /// </summary>
        public OS.OSTypes OperatingSystem { get; set; }
    }
    #endregion
}
