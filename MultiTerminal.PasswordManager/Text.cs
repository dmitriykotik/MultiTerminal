using MultiTerminal.Input;
using MultiTerminal.Output;
using MultiTerminal.PasswordManager;

public partial class Text
{
    public static void RunPasswordManagerAscii(string filePath)
    {
        var output = new DefaultOutput();
        var input = new DefaultInput();

        // Создадим менеджер (Logger передаем null — можно заменить).
        var pm = new PassManager(filePath, null);

        // Вспомогательные лямбды
        void WriteLine(string text = "") => output.WriteLine(text);
        void Write(string text) => output.Write(text);
        string Read() => input.ReadLine() ?? string.Empty;

        // Маленькие рисовалки для ASCII-интерфейса
        void DrawHeader(string title)
        {
            WriteLine(new string('═', 60));
            WriteLine($"═  {title.PadRight(56)}═");
            WriteLine(new string('═', 60));
        }

        void DrawFooter() => WriteLine(new string('═', 60));

        // --- Авторизация / инициализация ---
        string masterPassword = string.Empty;

        if (!File.Exists(filePath))
        {
            WriteLine("Файл паролей не найден.");
            Write("Создать новый файл? (y/n): ");
            var ans = Read().Trim().ToLower();
            if (ans != "y" && ans != "yes")
            {
                WriteLine("Отмена. Выход.");
                return;
            }

            // Создаём новый мастер-пароль (с подтверждением)
            while (true)
            {
                Write("Введите мастер-пароль: ");
                var p1 = Read();
                Write("Подтвердите мастер-пароль: ");
                var p2 = Read();
                if (p1 == p2 && !string.IsNullOrEmpty(p1))
                {
                    masterPassword = p1;
                    try
                    {
                        pm.Initialize(masterPassword);
                        WriteLine("Новый файл создан и инициализирован.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Ошибка инициализации: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    WriteLine("Пароли не совпадают или пусты. Попробуйте снова.");
                }
            }
        }
        else
        {
            // Пытаемся загрузить (максимум 3 попытки)
            bool loaded = false;
            for (int attempt = 1; attempt <= 3 && !loaded; attempt++)
            {
                Write($"Введите мастер-пароль (попытка {attempt}/3): ");
                var p = Read();
                masterPassword = p;
                try
                {
                    if (pm.TryLoad(masterPassword))
                    {
                        loaded = true;
                        WriteLine("Файл успешно загружен.");
                        break;
                    }
                    else
                    {
                        WriteLine("Не удалось загрузить файл: неверный пароль или повреждён файл.");
                    }
                }
                catch (Exception ex)
                {
                    WriteLine("Ошибка при загрузке: " + ex.Message);
                }
            }

            if (!loaded)
            {
                WriteLine("Не удалось загрузить файл. Завершение.");
                return;
            }
        }

        // --- Основной цикл меню ---
        while (true)
        {
            Console.Clear();

            var entries = pm.GetAllEntries() ?? new List<PasswordEntry>();

            DrawHeader("Password Manager — MultiTerminal");

            WriteLine($"Записей: {entries.Count}");
            WriteLine(new string('-', 60));
            if (entries.Count == 0)
            {
                WriteLine("[Пусто] Нажмите 'a' чтобы добавить запись.");
            }
            else
            {
                // Вывод списка: индекс, site, login и замаскированный пароль
                for (int i = 0; i < entries.Count; i++)
                {
                    var e = entries[i];
                    string masked = new string('•', Math.Max(4, e.Password?.Length ?? 4));
                    WriteLine($"{i + 1,3}. {Truncate(e.Site, 30),-30} | {Truncate(e.Login, 15),-15} | {masked}");
                }
            }
            WriteLine(new string('-', 60));
            WriteLine("Команды: (a)dd  (v)iew  (e)dit  (d)elete  (r)efresh  (q)uit");
            Write("Выберите команду: ");
            var cmd = Read().Trim().ToLower();

            if (string.IsNullOrEmpty(cmd)) continue;

            if (cmd == "q" || cmd == "quit")
            {
                // Корректно завершаем: очищаем и выходим
                try
                {
                    pm.Close();
                }
                catch { /* ignore */ }
                WriteLine("Завершено. Данные в памяти очищены.");
                break;
            }
            else if (cmd == "r" || cmd == "refresh")
            {
                // Попробуем повторно загрузить из файла (на случай внешних изменений)
                try
                {
                    if (pm.TryLoad(masterPassword))
                        WriteLine("Обновлено из файла.");
                    else
                        WriteLine("Не удалось обновить: неверный мастер-пароль или файл изменён.");
                }
                catch (Exception ex)
                {
                    WriteLine("Ошибка refresh: " + ex.Message);
                }
                Pause();
            }
            else if (cmd == "a" || cmd == "add")
            {
                WriteLine("--- Добавление новой записи ---");
                Write("Site: ");
                var site = Read().Trim();
                if (string.IsNullOrEmpty(site))
                {
                    WriteLine("Отмена: site не может быть пустым.");
                    Pause(); continue;
                }

                Write("Login: ");
                var login = Read().Trim();

                Write("Password: ");
                var password = Read(); // input.ReadLine без маски

                var entry = new PasswordEntry { Site = site, Login = login, Password = password };
                try
                {
                    pm.AddEntry(entry);
                    WriteLine("Запись добавлена и сохранена.");
                }
                catch (Exception ex)
                {
                    WriteLine("Ошибка при добавлении: " + ex.Message);
                }
                Pause();
            }
            else if (cmd == "v" || cmd == "view")
            {
                if (entries.Count == 0) { WriteLine("Нет записей."); Pause(); continue; }
                Write("Введите номер записи для просмотра: ");
                if (!int.TryParse(Read(), out int idx) || idx < 1 || idx > entries.Count)
                {
                    WriteLine("Неверный номер."); Pause(); continue;
                }
                var e = entries[idx - 1];
                DrawHeader($"Просмотр записи #{idx}");
                WriteLine($"Site    : {e.Site}");
                WriteLine($"Login   : {e.Login}");
                WriteLine($"Password: {e.Password}");
                DrawFooter();
                Pause();
            }
            else if (cmd == "e" || cmd == "edit")
            {
                if (entries.Count == 0) { WriteLine("Нет записей."); Pause(); continue; }
                Write("Введите номер записи для редактирования: ");
                if (!int.TryParse(Read(), out int idx) || idx < 1 || idx > entries.Count)
                {
                    WriteLine("Неверный номер."); Pause(); continue;
                }
                var old = entries[idx - 1];

                WriteLine("Оставьте поле пустым чтобы сохранить текущее значение.");
                Write($"Site [{old.Site}]: ");
                var site = Read();
                if (string.IsNullOrEmpty(site)) site = old.Site;

                Write($"Login [{old.Login}]: ");
                var login = Read();
                if (string.IsNullOrEmpty(login)) login = old.Login;

                Write($"Password [{MaskForPrompt(old.Password)}]: ");
                var password = Read();
                if (string.IsNullOrEmpty(password)) password = old.Password;

                var updated = new PasswordEntry { Site = site, Login = login, Password = password };

                try
                {
                    pm.UpdateEntry(old, updated);
                    WriteLine("Запись обновлена и сохранена.");
                }
                catch (Exception ex)
                {
                    WriteLine("Ошибка при обновлении: " + ex.Message);
                }
                Pause();
            }
            else if (cmd == "d" || cmd == "delete")
            {
                if (entries.Count == 0) { WriteLine("Нет записей."); Pause(); continue; }
                Write("Введите номер записи для удаления: ");
                if (!int.TryParse(Read(), out int idx) || idx < 1 || idx > entries.Count)
                {
                    WriteLine("Неверный номер."); Pause(); continue;
                }
                var e = entries[idx - 1];
                WriteLine($"Удалить запись #{idx}: {e.Site} / {e.Login}? (y/n): ");
                var confirm = Read().Trim().ToLower();
                if (confirm == "y" || confirm == "yes")
                {
                    try
                    {
                        pm.RemoveEntry(e);
                        WriteLine("Запись удалена.");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Ошибка при удалении: " + ex.Message);
                    }
                }
                else
                {
                    WriteLine("Удаление отменено.");
                }
                Pause();
            }
            else
            {
                WriteLine("Неизвестная команда.");
                Pause();
            }
        } // end while

        // --- локальные вспомогательные методы ---
        void Pause()
        {
            WriteLine("");
            Write("Нажмите Enter чтобы продолжить...");
            Read();
        }

        static string Truncate(string s, int max)
        {
            if (s == null) return string.Empty;
            return s.Length <= max ? s : s.Substring(0, max - 1) + "…";
        }

        static string MaskForPrompt(string s)
        {
            if (string.IsNullOrEmpty(s)) return "(пусто)";
            return new string('•', Math.Min(6, s.Length));
        }
    }
}
