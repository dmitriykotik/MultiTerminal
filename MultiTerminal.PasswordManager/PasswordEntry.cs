namespace MultiTerminal.PasswordManager
{
    /// <summary>
    /// Класс элемента менеджера паролей
    /// </summary>
    public class PasswordEntry
    {
        /// <summary>
        /// Сайт
        /// </summary>
        public required string Site { get; set; }
        /// <summary>
        /// Логин
        /// </summary>
        public required string Login { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public required string Password { get; set; }

    }
}
