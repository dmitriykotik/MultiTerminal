using System.Text.Json;
using MultiAPI;

namespace MultiTerminal.Settings
{
    #region CLASS | SettingsManager
    /// <summary>
    /// Класс управления настройками конфиг. файла
    /// </summary>
    public class SettingsManager
    {
        #region Private vars
        private string _path;
        private SettingsFile? _file;
        #endregion

        #region METHOD-Settings-Manager
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="settingsFile">Файл конфигурации</param>
        public SettingsManager(string settingsFile)
        {
            _path = settingsFile;
            Get();
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="settingsFile">Файл конфигурации</param>
        /// <param name="settingsDir">Папка конфигурации</param>
        public SettingsManager(string settingsFile, string settingsDir)
        {
            if (!FileManager.Directory.Exists(settingsDir))
                FileManager.Directory.Create(settingsDir);
            _path = settingsDir + "\\" + settingsFile;
            Get();
        }
        #endregion

        #region PRIVATE~METHOD-VOID | Create
        /// <summary>
        /// Создание файла кофнигурации
        /// </summary>
        private void Create()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            FileManager.File.WriteAllText(_path, JsonSerializer.Serialize(sampleConfig, options));
        }
        #endregion

        #region METHOD-SettingsFile | Get
        /// <summary>
        /// Получение содержимого конфигурационного файла
        /// </summary>
        /// <returns>Содержимое конфиг. файла</returns>
        public SettingsFile Get()
        {
            if (!FileManager.File.Exists(_path))
                Create();

            var content = FileManager.File.ReadAllText(_path);

            if (!string.IsNullOrEmpty(content))
                _file = JsonSerializer.Deserialize<SettingsFile>(content);

            if (_file == null)
                _file = sampleConfig;

            return _file;
        }
        #endregion

        #region METHOD-VOID | Save
        /// <summary>
        /// Сохранение конфигурации
        /// </summary>
        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            FileManager.File.WriteAllText(_path, JsonSerializer.Serialize(_file, options));
        }
        #endregion

        #region METHOD-BOOL | AddPath
        /// <summary>
        /// Добавить папку в переменную пути
        /// </summary>
        /// <param name="folder">Существующая папка</param>
        /// <returns>true - Если удалось загрузить папку, иначе false</returns>
        public bool AddPath(string folder)
        {
            if (!FileManager.Directory.Exists(folder))
                return false;

            if (_file != null && !_file.Paths.Contains(folder))
                _file.Paths.Add(folder);
            else
                return false;
            Save();
            return true;
        }
        #endregion

        #region METHOD-BOOL | DelPath
        /// <summary>
        /// Удалить папку из переменной пути
        /// </summary>
        /// <param name="folder">Папка в переменной</param>
        /// <returns>true - Если удалось удалить папку из переменноой, иначе false</returns>
        public bool DelPath(string folder)
        {
            if (_file != null && _file.Paths.Contains(folder))
                _file.Paths.Remove(folder);
            else
                return false;
            Save();
            return true;
        }
        #endregion

        #region METHOD-BOOL | ContainsFilePath
        /// <summary>
        /// Проверка на существование файла в папках переменной
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true - Если папка существует, иначе false</returns>
        public bool ContainsFilePath(string file)
        {
            if (_file == null)
                return false;

            foreach (var dir in _file.Paths)
            {
                if (FileManager.Directory.Exists(dir))
                {
                    if (FileManager.File.Exists(dir + "\\" + file))
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region METHOD-BOOL | ContainsPath
        /// <summary>
        /// Проверка на существование папки в переменной
        /// </summary>
        /// <param name="folder">Папка</param>
        /// <returns>true - Если папка существует в переменной, иначе false</returns>
        public bool ContainsPath(string folder)
        {
            if (_file != null && _file.Paths.Contains(folder))
                return true;
            return false;
        }
        #endregion

        #region METHOD-STRING? | GetFilePath
        /// <summary>
        /// Получение полного пути по файлу из папки в переменной
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Строка, содержащая полный путь до файла, в противном случае значение null</returns>
        public string? GetFilePath(string file)
        {
            if (!ContainsFilePath(file) || _file == null)
                return null;

            foreach (var dir in _file.Paths)
            {
                if (FileManager.Directory.Exists(dir))
                {
                    if (FileManager.File.Exists(file))
                        return Path.GetFullPath(dir + "\\" + file);
                }
            }
            return null;
        }
        #endregion

        #region METHOD-VOID | Update
        /// <summary>
        /// Обновлнение конфигурационного файла
        /// </summary>
        private void Update()
        {

        }
        #endregion

        #region METHOD-VOID | Delete
        /// <summary>
        /// Удаление конфигурационного файла
        /// </summary>
        private void Delete()
        {

        }
        #endregion

        #region SettingsFile | sampleConfig
        /// <summary>
        /// Временный конфиг. файл
        /// </summary>
        private SettingsFile sampleConfig = new()
        {
            Product = new()
            {
                Name = "MultiTerminal",
                Version = new()
                {
                    Major = 0,
                    Minor = 1,
                    Build = 1,
                    Path = 50
                }
            },
            disableWarningCompatibility = false,
            ignoreNewVersions = false,
            SMBIOS = new()
            {
                EnableSMBIOS = false,
                OperatingSystem = OS.OSTypes.Other
            },
            Paths = new()
            {
                "C:\\"
            },
            Motd = "null",
            SkipHashCheck = false
        };
        #endregion
    }
    #endregion
}
