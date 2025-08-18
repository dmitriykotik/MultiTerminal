namespace MultiTerminal.Settings
{
    #region CLASS | Version
    /// <summary>
    /// Класс содержащий версию
    /// </summary>
    public class Version
    {
        /// <summary>
        /// Мажор
        /// </summary>
        public int Major { get; set; }
        /// <summary>
        /// Минор
        /// </summary>
        public int Minor { get; set; }
        /// <summary>
        /// Патч
        /// </summary>
        public int Path { get; set; }
        /// <summary>
        /// Сборка
        /// </summary>
        public int Build { get; set; }
    }
    #endregion
}
