using MultiTerminal.Logger;
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

        /// <summary>
        /// Конструктор менеджера паролей
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="folderPath">Путь к папке</param>
        /// <param name="Log">Логгер</param>
        public PassManager(string filePath, string folderPath, Log? Log = null)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            _filePath = folderPath + "\\" + filePath;
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
