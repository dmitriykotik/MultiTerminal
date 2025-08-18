using System.Security;
using MultiTerminal.Output;

namespace MultiTerminal.Input
{
    #region CLASS | Password
    /// <summary>
    /// Утилитный класс для безопасного ввода пароля с опциональной маской символов.
    /// </summary>
    public static class Password
    {
        #region METHOD-SecureString | Read
        /// <summary>
        /// Принимает ввод пароля пользователем.
        /// При указании maskChar ввод отображается этим символом, иначе - скрывается полностью.
        /// </summary>
        /// <param name="maskChar">Символ маскировки. Если null, ввод не отображается</param>
        /// <returns>Введённый пароль</returns>
        public static SecureString Read(char? maskChar = '*')
        {
            DefaultOutput oput = new DefaultOutput();
            DefaultInput iput = new DefaultInput();
            var securePwd = new SecureString();
            ConsoleKeyInfo key;
            var output = maskChar.HasValue ? oput.Write : (Action<string>)(_ => { });

            while (true)
            {
                key = iput.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        oput.WriteLine();
                        securePwd.MakeReadOnly();
                        return securePwd;

                    case ConsoleKey.Backspace:
                        if (securePwd.Length > 0)
                        {
                            securePwd.RemoveAt(securePwd.Length - 1);
                            oput.Write("\b \b");
                        }
                        break;

                    case ConsoleKey.Escape:
                        while (securePwd.Length > 0)
                        {
                            securePwd.RemoveAt(0);
                            oput.Write("\b \b");
                        }
                        break;

                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            securePwd.AppendChar(key.KeyChar);
                            if (maskChar.HasValue)
                                output(maskChar.Value.ToString());
                        }
                        break;
                }
            }
        }
        #endregion

        #region METHOD-STRING | ToPlainText
        /// <summary>
        /// Получает строковое представление безопасного пароля.
        /// </summary>
        /// <param name="securePwd">SecureString с введенным паролем</param>
        /// <returns>Пароль в виде строки</returns>
        public static string ToPlainText(SecureString securePwd)
        {
            if (securePwd == null) throw new ArgumentNullException(nameof(securePwd));
            IntPtr bstr = IntPtr.Zero;
            try
            {
                bstr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(securePwd);
                return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                if (bstr != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(bstr);
            }
        }
        #endregion
    }
    #endregion
}
