namespace MultiTerminal.Output
{
    #region CLASS | DefaultOutput
    /// <summary>
    /// Стандартные методы вывода
    /// </summary>
    public class DefaultOutput : IOutput
    {
        #region METHOD-VOID | Write
        /// <summary>
        /// Вывод строки
        /// </summary>
        /// <param name="content">Содержимое</param>
        public void Write(string? content = null) =>
            InternalWrite(content, Console.Write);
        #endregion

        #region METHOD-VOID | WriteLine
        /// <summary>
        /// Вывод строки с отступом строки
        /// </summary>
        /// <param name="content">Содержимое</param>
        public void WriteLine(string? content = null) =>
            InternalWrite(content, Console.WriteLine);
        #endregion

        #region METHOD-VOID | Write
        /// <summary>
        /// Вывод строки с указанием цвета
        /// </summary>
        /// <param name="content">Содержимое</param>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        public void Write(string? content, ConsoleColor? fg = null, ConsoleColor? bg = null) =>
            InternalWrite(content, Console.Write, fg, bg);
        #endregion

        #region METHOD-VOID | WriteLine
        /// <summary>
        /// Вывод строки с отступом строки и с указанием цвета
        /// </summary>
        /// <param name="content">Содержимое</param>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        public void WriteLine(string? content, ConsoleColor? fg = null, ConsoleColor? bg = null) =>
            InternalWrite(content, Console.WriteLine, fg, bg);
        #endregion

        #region METHOD-VOID | Clear
        /// <summary>
        /// Очистка экрана с возможностью указания цвета
        /// </summary>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        public void Clear(ConsoleColor? fg = null, ConsoleColor? bg = null)
        {
            if (fg.HasValue) Console.ForegroundColor = fg.Value;
            if (bg.HasValue) Console.BackgroundColor = bg.Value;

            Console.Clear();

            Console.ResetColor();
        }
        #endregion

        #region PRIVATE~METHOD-VOID | InternalWrite
        /// <summary>
        /// Общий метод для вывода текста
        /// </summary>
        /// <param name="content">Содержимое</param>
        /// <param name="writer">Функция вывода</param>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        private void InternalWrite(string? content, Action<string?> writer, ConsoleColor? fg = null, ConsoleColor? bg = null)
        {
            if (fg.HasValue) Console.ForegroundColor = fg.Value;
            if (bg.HasValue) Console.BackgroundColor = bg.Value;

            writer(content);

            Console.ResetColor();
        }
        #endregion
    }
    #endregion
}
