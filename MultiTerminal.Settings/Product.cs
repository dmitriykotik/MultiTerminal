namespace MultiTerminal.Settings
{
    #region CLASS | Product
    /// <summary>
    /// Класс содержащий информацию о продукте
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Наименование продукта
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// Версия продукта
        /// </summary>
        public required Version Version { get; set; }
    }
    #endregion
}
