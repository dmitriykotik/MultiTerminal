namespace MultiTerminal.Settings
{
    #region ENUM-Permissions
    /// <summary>
    /// Права доступа
    /// </summary>
    public enum Permissions
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        User,
        /// <summary>
        /// Администратор (root)
        /// </summary>
        Admin,
        /// <summary>
        /// Супер-администратор (WinPE) (Создано только для WinPE!)
        /// </summary>
        SuperAdmin
    }
    #endregion
}
