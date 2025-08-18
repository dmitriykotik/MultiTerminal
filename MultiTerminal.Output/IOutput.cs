namespace MultiTerminal.Output
{
    #region INTERFACE | IOutput
    /// <summary>
    /// Интерфейс для работы с выводом
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Вывод строки
        /// </summary>
        /// <param name="content">Содержимое</param>
        void Write(string? content = null);

        /// <summary>
        /// Вывод строки с отступом строки
        /// </summary>
        /// <param name="content">Содержимое</param>
        void WriteLine(string? content = null);

        /// <summary>
        /// Вывод строки с указанием цвета
        /// </summary>
        /// <param name="content">Содержимое</param>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        void Write(string? content, ConsoleColor? fg = null, ConsoleColor? bg = null);

        /// <summary>
        /// Вывод строки с отступом строки и с указанием цвета
        /// </summary>
        /// <param name="content">Содержимое</param>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        void WriteLine(string? content, ConsoleColor? fg = null, ConsoleColor? bg = null);

        /// <summary>
        /// Очистка экрана с возможностью указания цвета
        /// </summary>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        void Clear(ConsoleColor? fg = null, ConsoleColor? bg = null);
    }
    #endregion
}
