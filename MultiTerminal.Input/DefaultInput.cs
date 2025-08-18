namespace MultiTerminal.Input
{
    #region CLASS | DefaultInput
    /// <summary>
    /// Стандартные методы ввода
    /// </summary>
    public class DefaultInput : IInput
    {
        #region METHOD-STRING | ReadLine
        /// <summary>
        /// Ввод строки
        /// </summary>
        /// <returns>Строка введённая пользователем</returns>
        public string? ReadLine() => Console.ReadLine();
        #endregion

        #region METHOD-CONSOLEKEYINFO | ReadKey
        /// <summary>
        /// Ввод символа
        /// </summary>
        /// <returns>Информация о введённом символе</returns>
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
        #endregion
    }
    #endregion
}