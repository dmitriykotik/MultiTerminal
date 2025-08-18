namespace MultiTerminal.Input
{
    #region INTERFACE | IInput
    /// <summary>
    /// Интерфейс ввода
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Ввод строки
        /// </summary>
        /// <returns>Строка</returns>
        public string? ReadLine();

        /// <summary>
        /// Ввод символа/кнопки
        /// </summary>
        /// <param name="intercept">Отображение нажатого символа</param>
        /// <returns>Символ</returns>
        public ConsoleKeyInfo ReadKey(bool intercept = false);
    }
    #endregion
}
