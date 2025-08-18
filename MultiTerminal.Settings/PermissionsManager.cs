using MultiAPI;
using System.Runtime.InteropServices;
using System.Security.Principal;

#pragma warning disable CA1416

namespace MultiTerminal.Settings
{
    #region CLASS | PermissionsManager
    /// <summary>
    /// Класс для взаимодействия с правами доступа
    /// </summary>
    public class PermissionsManager
    {
        #region METHOD-Permissions | Get
        /// <summary>
        /// Получение прав доступа терминала
        /// </summary>
        /// <returns>Права доступа</returns>
        public static Permissions Get()
        {
            if (IsWindowsPE())
                return Permissions.SuperAdmin;
            else if (IsAdmin())
                return Permissions.Admin;
            else
                return Permissions.User;
        }
        #endregion

        #region PRIVATE~METHOD-BOOL | IsAdmin
        /// <summary>
        /// Встроенный метод для получения данных о том, является ли пользователь администратором
        /// </summary>
        /// <returns>Пользователь - Администратор?</returns>
        private static bool IsAdmin()
        {
            if (OS.Get() == OS.OSTypes.Windows)
            {
                using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            else
                return geteuid() == 0;
        }
        #endregion

        #region PRIVATE~METHOD-BOOL | IsWindowsPE
        /// <summary>
        /// Встроенный метод для получения данных о том, запущен ли терминал в среде WindowsPE
        /// </summary>
        /// <returns>Терминал запущен в WindowsPE?</returns>
        private static bool IsWindowsPE()
        {
            if (OS.Get() != OS.OSTypes.Windows)
                return false;

            string? systemDrive = Environment.GetEnvironmentVariable("SystemDrive");
            if (systemDrive != null && systemDrive.Equals("X:", StringComparison.OrdinalIgnoreCase))
                return true;

            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (!Directory.Exists(Path.Combine(winDir, "System32")))
                return true;

            string startNetCmd = Path.Combine(winDir, "System32", "startnet.cmd");
            string winPeShl = Path.Combine(winDir, "System32", "winpeshl.ini");

            return File.Exists(startNetCmd) || File.Exists(winPeShl);
        }
        #endregion

        #region PRIVATE-UINT | geteid
        /// <summary>
        /// Получение euid пользователя в UNIX системе. 
        /// </summary>
        /// <returns>euid</returns>
        [DllImport("libc")]
        private static extern uint geteuid();
        #endregion
    }
    #endregion
}
