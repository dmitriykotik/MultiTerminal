using MultiTerminal.Output;

namespace MultiTerminal.Graphics
{
    #region CLASS | DefaultGraphics
    /// <summary>
    /// Взаимодействие с графикой
    /// </summary>
    public class DefaultGraphics
    {
        #region METHOD-VOID | CreateWindow
        /// <summary>
        /// Создать окно
        /// </summary>
        /// <param name="Width">Ширина</param>
        /// <param name="Title">Заголовок окна</param>
        /// <param name="Content">Содержимое окна</param>
        /// <param name="EventException">Интерфес обработчика исключений</param>
        public static void CreateWindow(int Width, string Title, string[] Content, IException EventException)
        {
            DefaultOutput oput = new DefaultOutput();
            oput.WriteLine("+- " + Title + " ".PadRight(Width - Title.Length, '-') + "+");
            try { foreach (var line in Content) oput.WriteLine("| " + line.PadRight(Width) + " |"); } catch (Exception ex) { EventException.Start(ex); }
            oput.WriteLine("+" + new string('-', Width + 2) + "+");
        }
        #endregion

        #region METHOD-VOID | CreateWindow
        /// <summary>
        /// Создать окно
        /// </summary>
        /// <param name="Width">Ширина</param>
        /// <param name="Title">Заголовок окна</param>
        /// <param name="Content">Содержимое окна</param>
        /// <param name="MaskLineHorz">Маска горизонтальных линий</param>
        /// <param name="MaskLineVert">Маска вертикальных линий</param>
        /// <param name="MaskAngle">Маска угловых линий</param>
        /// <param name="EventException">Интерфес обработчика исключений</param>
        public static void CreateWindow(int Width, string Title, string[] Content, char MaskLineHorz, char MaskLineVert, char MaskAngle, IException EventException)
        {
            DefaultOutput oput = new DefaultOutput();
            oput.WriteLine($"{MaskAngle}{MaskLineHorz} " + Title + " ".PadRight(Width - Title.Length, MaskLineHorz) + MaskAngle);
            try { foreach (var line in Content) oput.WriteLine($"{MaskLineVert} " + line.PadRight(Width) + $" {MaskLineVert}"); } catch (Exception ex) { EventException.Start(ex); }
            oput.WriteLine(MaskAngle + new string(MaskLineHorz, Width + 2) + MaskAngle);
        }
        #endregion

        #region METHOD-VOID | CreateUniWindow
        /// <summary>
        /// Создать окно с симводами Юникода
        /// </summary>
        /// <param name="Width">Ширина</param>
        /// <param name="Title">Заголовок окна</param>
        /// <param name="Content">Содержимое окна</param>
        /// <param name="EventException">Интерфес обработчика исключений</param>
        public static void CreateUniWindow(int Width, string Title, string[] Content, IException EventException)
        {
            DefaultOutput oput = new DefaultOutput();
            oput.WriteLine("╭─ " + Title + " ".PadRight(Width - Title.Length, '─') + "╮");
            try { foreach (var line in Content) oput.WriteLine("│ " + line.PadRight(Width) + " │"); } catch (Exception ex) { EventException.Start(ex); }
            oput.WriteLine("╰" + new string('─', Width + 2) + "╯");
        }
        #endregion
    }
    #endregion
}
