using MultiTerminal.Input;
using MultiTerminal.Logger;
using MultiTerminal.Output;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

#pragma warning disable CS8618
namespace MultiTerminal.PasswordManager
{
    #region CLASS | PassManager
    /// <summary>
    /// Класс менеджера паролей
    /// </summary>
    public class PassManager
    {
        #region Private Vars
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int IvSize = 16;
        private const int Iterations = 10000;
        private readonly string _filePath;

        private byte[] _salt;
        private byte[] _passwordHash;
        private byte[] _key;
        private byte[] _iv;

        private List<PasswordEntry> _entries = [];

        private readonly Log? Logger;
        #endregion

        #region METHOD-PassManager | PassManager
        /// <summary>
        /// Конструктор менеджера паролей
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="Log">Логгер</param>
        public PassManager(string filePath, Log? Log = null)
        {
            _filePath = filePath;
            Logger = Log;
        }
        #endregion

        #region METHOD-VOID | Initialize
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="masterPassword">Мастер-пароль</param>
        public void Initialize(string masterPassword)
        {
            Logger?.Write(LogType.Debug, "Password Manager: Initializing new file...");
            _salt = GenerateRandomBytes(SaltSize);
            _passwordHash = HashPassword(masterPassword, _salt);

            _key = DeriveKey(masterPassword, _salt);
            _iv = GenerateRandomBytes(IvSize);

            _entries = new List<PasswordEntry>();

            SaveToFile();
        }
        #endregion

        #region METHOD-VOID | TryLoad
        /// <summary>
        /// Попытка загрузки менеджера паролей
        /// </summary>
        /// <param name="masterPassword">Мастер-пароль</param>
        /// <returns></returns>
        public bool TryLoad(string masterPassword)
        {
            Logger?.Write(LogType.Debug, "Password Manager: Trying to retrieve data...");

            if (!File.Exists(_filePath))
            {
                Logger?.Write(LogType.Error, "Password Manager: The password file does not exist!");
                return false;
            }

            byte[] fileData = File.ReadAllBytes(_filePath);

            _salt = new byte[SaltSize];
            _iv = new byte[IvSize];

            Array.Copy(fileData, 0, _salt, 0, SaltSize);
            Array.Copy(fileData, SaltSize, _iv, 0, IvSize);

            int encryptedDataLength = fileData.Length - SaltSize - IvSize;
            byte[] encryptedData = new byte[encryptedDataLength];
            Array.Copy(fileData, SaltSize + IvSize, encryptedData, 0, encryptedDataLength);

            _key = DeriveKey(masterPassword, _salt);

            string decryptedJson;
            try
            {
                decryptedJson = DecryptString(encryptedData, _key, _iv);
            }
            catch (Exception ex)
            {
                Logger?.Write(LogType.Error, "Password Manager: " + ex.Message);
                return false;
            }

            var container = JsonSerializer.Deserialize<PasswordFile>(decryptedJson);

            if (container == null || container.PasswordHash == null)
                return false;

            if (!CompareBytes(container.PasswordHash, HashPassword(masterPassword, _salt)))
                return false;

            _passwordHash = container.PasswordHash;
            _entries = container.Entries ?? new List<PasswordEntry>();

            return true;
        }
        #endregion

        #region METHOD-LIST | Run
        /// <summary>
        /// Получение всех элементов
        /// </summary>
        /// <returns></returns>
        public List<PasswordEntry> GetAllEntries() => _entries;
        #endregion

        #region METHOD-VOID | AddEntry
        /// <summary>
        /// Добавление элемента
        /// </summary>
        /// <param name="entry">Элемент</param>
        public void AddEntry(PasswordEntry entry)
        {
            Logger?.Write(LogType.Debug, "Password Manager: Adding new item...");
            _entries.Add(entry);
            SaveToFile();
        }
        #endregion

        #region METHOD-VOID | RemoveEntry
        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="entry">Элемент</param>
        public void RemoveEntry(PasswordEntry entry)
        {
            Logger?.Write(LogType.Debug, "Password Manager: Removing item...");
            _entries.Remove(entry);
            SaveToFile();
        }
        #endregion

        #region METHOD-VOID | UpdateEntry
        /// <summary>
        /// Обновление элемента
        /// </summary>
        /// <param name="oldEntry">Старый элемент</param>
        /// <param name="newEntry">Новый элемент</param>
        public void UpdateEntry(PasswordEntry oldEntry, PasswordEntry newEntry)
        {
            Logger?.Write(LogType.Debug, "Password Manager: Updating item...");
            int index = _entries.IndexOf(oldEntry);
            if (index != -1)
            {
                _entries[index] = newEntry;
                SaveToFile();
            }
        }
        #endregion

        #region METHOD-VOID | Close
        /// <summary>
        /// Закрытие менеджера паролей
        /// </summary>
        public void Close()
        {
            Logger?.Write(LogType.Debug, "Password Manager: Destroying temporary data and ending session...");
            if (_key != null)
            {
                Array.Clear(_key, 0, _key.Length);
            #pragma warning disable CS8625
                _key = null;
            }

            if (_salt != null)
            {
                Array.Clear(_salt, 0, _salt.Length);
                _salt = null;
            }

            if (_passwordHash != null)
            {
                Array.Clear(_passwordHash, 0, _passwordHash.Length);
                _passwordHash = null;
            }

            if (_iv != null)
            {
                Array.Clear(_iv, 0, _iv.Length);
                _iv = null;
            }

            _entries?.Clear();
            _entries = null;
            #pragma warning restore CS8625

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion

        #region METHOD-VOID | Run
        /// <summary>
        /// Запуск программы менеджера паролей
        /// </summary>
        public void Run()
        {
            var output = new DefaultOutput();
            var input = new DefaultInput();

            void WriteLine(string text = "") => output.WriteLine(text);
            void Write(string text) => output.Write(text);
            string Read() => input.ReadLine() ?? string.Empty;

            void DrawHeader(string title)
            {
                WriteLine(new string('═', 60));
                WriteLine($"═  {title.PadRight(56)}═");
                WriteLine(new string('═', 60));
            }

            void DrawFooter() => WriteLine(new string('═', 60));

            string masterPassword = string.Empty;

            if (!File.Exists(_filePath))
            {
                WriteLine("PassMgr: Password file not found.");

                while (true)
                {
                    Write("PassMgr: Enter master password: ");
                    var p1 = Read();
                    Write("PassMgr: Confirm Master Password: ");
                    var p2 = Read();
                    if (p1 == p2 && !string.IsNullOrEmpty(p1))
                    {
                        masterPassword = p1;
                        try
                        {
                            Initialize(masterPassword);
                            WriteLine("PassMgr: The new file has been created and initialized.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            WriteLine("PassMgr: Initialization error: " + ex.Message);
                            return;
                        }
                    }
                    else
                    {
                        WriteLine("PassMgr: Passwords do not match or are empty. Try again.");
                    }
                }
            }
            else
            {
                bool loaded = false;
                for (int attempt = 1; attempt <= 3 && !loaded; attempt++)
                {
                    Write($"PassMgr: Enter master password (attempt {attempt}/3): ");
                    var p = Read();
                    masterPassword = p;
                    try
                    {
                        if (TryLoad(masterPassword))
                        {
                            loaded = true;
                            WriteLine("PassMgr: The file has been loaded successfully.");
                            break;
                        }
                        else
                        {
                            WriteLine("PassMgr: Failed to load file: incorrect password or file is corrupted.");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine("PassMgr: Error loading: " + ex.Message);
                    }
                }

                if (!loaded)
                {
                    WriteLine("PassMgr: Failed to load file. Terminating.");
                    return;
                }
            }

            while (true)
            {
                output.Clear();

                var entries = GetAllEntries() ?? new List<PasswordEntry>();

                DrawHeader("Password Manager — MultiTerminal");

                WriteLine($"Entries: {entries.Count}");
                WriteLine(new string('-', 60));
                if (entries.Count == 0)
                {
                    WriteLine("[Empty] Press 'a' to add an entry.");
                }
                else
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        var e = entries[i];
                        string masked = new string('•', Math.Max(4, e.Password?.Length ?? 4));
                        WriteLine($"{i + 1,3}. {Truncate(e.Site, 30),-30} | {Truncate(e.Login, 15),-15} | {masked}");
                    }
                }
                WriteLine(new string('-', 60));
                WriteLine("(a)dd  (v)iew  (e)dit  (d)elete  (r)efresh  (q)uit");
                Write("> ");
                var cmd = Read().Trim().ToLower();

                if (string.IsNullOrEmpty(cmd)) continue;

                if (cmd == "q" || cmd == "quit")
                {
                    try
                    {
                        Close();
                    }
                    catch { }
                    WriteLine("Completed. Data in memory cleared.");
                    break;
                }
                else if (cmd == "r" || cmd == "refresh")
                {
                    try
                    {
                        if (TryLoad(masterPassword))
                            WriteLine("Updated from file.");
                        else
                            WriteLine("Update failed: incorrect master password or file has been modified.");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Error refresh: " + ex.Message);
                    }
                    Pause();
                }
                else if (cmd == "a" || cmd == "add")
                {
                    WriteLine("--- Adding a new entry ---");
                    Write("Site: ");
                    var site = Read().Trim();
                    if (string.IsNullOrEmpty(site))
                    {
                        WriteLine("Cancel: site cannot be empty.");
                        Pause(); continue;
                    }

                    Write("Login: ");
                    var login = Read().Trim();

                    Write("Password: ");
                    var password = Read();

                    var entry = new PasswordEntry { Site = site, Login = login, Password = password };
                    try
                    {
                        AddEntry(entry);
                        WriteLine("The entry has been added and saved.");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Error while adding: " + ex.Message);
                    }
                    Pause();
                }
                else if (cmd == "v" || cmd == "view")
                {
                    if (entries.Count == 0) { WriteLine("No entries."); Pause(); continue; }
                    Write("Enter the entry number to view: ");
                    if (!int.TryParse(Read(), out int idx) || idx < 1 || idx > entries.Count)
                    {
                        WriteLine("Invalid number."); Pause(); continue;
                    }
                    var e = entries[idx - 1];
                    DrawHeader($"View entry #{idx}");
                    WriteLine($"Site    : {e.Site}");
                    WriteLine($"Login   : {e.Login}");
                    WriteLine($"Password: {e.Password}");
                    DrawFooter();
                    Pause();
                }
                else if (cmd == "e" || cmd == "edit")
                {
                    if (entries.Count == 0) { WriteLine("No entries."); Pause(); continue; }
                    Write("Enter the entry number to edit: ");
                    if (!int.TryParse(Read(), out int idx) || idx < 1 || idx > entries.Count)
                    {
                        WriteLine("Invalid number."); Pause(); continue;
                    }
                    var old = entries[idx - 1];

                    WriteLine("Leave the field blank to keep the current value.");
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
                        UpdateEntry(old, updated);
                        WriteLine("The entry has been updated and saved.");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Error while updating: " + ex.Message);
                    }
                    Pause();
                }
                else if (cmd == "d" || cmd == "delete")
                {
                    if (entries.Count == 0) { WriteLine("No entries."); Pause(); continue; }
                    Write("Enter the record number to delete: ");
                    if (!int.TryParse(Read(), out int idx) || idx < 1 || idx > entries.Count)
                    {
                        WriteLine("Invalid number."); Pause(); continue;
                    }
                    var e = entries[idx - 1];
                    WriteLine($"Delete entry #{idx}: {e.Site} / {e.Login}? (y/n): ");
                    var confirm = Read().Trim().ToLower();
                    if (confirm == "y" || confirm == "yes")
                    {
                        try
                        {
                            RemoveEntry(e);
                            WriteLine("The entry has been deleted.");
                        }
                        catch (Exception ex)
                        {
                            WriteLine("Error while deleting: " + ex.Message);
                        }
                    }
                    else
                    {
                        WriteLine("Deletion cancelled.");
                    }
                    Pause();
                }
                else
                {
                    WriteLine("Unknown command.");
                    Pause();
                }
            }

            void Pause()
            {
                WriteLine("");
                Write("Press Enter to continue...");
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
        #endregion

        #region Private Methods
        private void SaveToFile()
        {
            var container = new PasswordFile
            {
                PasswordHash = _passwordHash,
                Entries = _entries
            };

            string json = JsonSerializer.Serialize(container);
            byte[] encryptedData = EncryptString(json, _key, _iv);

            using var fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
            fs.Write(_salt, 0, _salt.Length);
            fs.Write(_iv, 0, _iv.Length);
            fs.Write(encryptedData, 0, encryptedData.Length);
        }

        private static byte[] GenerateRandomBytes(int size)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return bytes;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using var sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] salted = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, salted, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, salted, salt.Length, passwordBytes.Length);
            return sha256.ComputeHash(salted);
        }

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }

        private static byte[] EncryptString(string plainText, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            using var encryptor = aes.CreateEncryptor();

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static string DecryptString(byte[] cipherText, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();

            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static bool CompareBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }
        #endregion
    }
    #endregion
}
