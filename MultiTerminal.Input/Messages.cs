using MultiTerminal.Output;
using MultiTerminal.Settings;

namespace MultiTerminal.Input
{
    #region CLASS | Messages
    /// <summary>
    /// Сообщения
    /// </summary>
    public class Messages
    {
        #region METHOD-BOOL | Question
        /// <summary>
        /// Вопрос
        /// </summary>
        /// <param name="Content">Содержимое</param>
        /// <param name="enableWaitAnswer">Ожидание ответа (y или n)</param>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        /// <returns>Дан ли положительный ответ?</returns>
        public static bool Question(string Content, bool enableWaitAnswer = false, ConsoleColor? fg = null, ConsoleColor? bg = null)
        {
            if (fg.HasValue) Console.ForegroundColor = fg.Value;
            if (bg.HasValue) Console.BackgroundColor = bg.Value;

            DefaultOutput oput = new DefaultOutput();
            oput.WriteLine(Content + " (y/n)", Console.ForegroundColor, Console.BackgroundColor);

            DefaultInput iput = new DefaultInput();

            do
            {
                oput.Write("> ");
                var result = iput.ReadLine();

                if (!string.IsNullOrEmpty(result))
                {
                    if (result.ToLower() == "y")
                        return true;
                    return false;
                }
            }
            while (enableWaitAnswer);

            return false;
        }
        #endregion

        #region METHOD-BOOL | SuperAdmin
        /// <summary>
        /// Доступ к супер админу
        /// </summary>
        /// <param name="settingsFile">Файл настроек</param>
        /// <returns>Дан доступ?</returns>
        public static bool SuperAdmin(SettingsFile settingsFile)
        {

            return true;
        }
        #endregion
    }
    #endregion
}