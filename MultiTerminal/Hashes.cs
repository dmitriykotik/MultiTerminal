using MultiAPI;
using System.Text.Json;

namespace MultiTerminal
{
    internal class Hashes
    {
        private string _path;
        private HashFile? _file;

        internal Hashes(string file)
        {
            _path = file;
            Get();
        }

        internal Hashes(string file, string folder)
        {
            if (!FileManager.Directory.Exists(folder))
                FileManager.Directory.Create(folder);
            _path = folder + "\\" + file;
            Get();
        }

        private void Create()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            FileManager.File.WriteAllText(_path, JsonSerializer.Serialize(Gen(), options));
        }

        internal HashFile Get()
        {
            if (!FileManager.File.Exists(_path))
                Create();

            var content = FileManager.File.ReadAllText(_path);

            if (!string.IsNullOrEmpty(content))
                _file = JsonSerializer.Deserialize<HashFile>(content);

            if (_file == null)
                _file = Gen();

            return _file;
        }

        internal HashFile Gen()
        {
            HashFile tempHashFile = new() { Hashes = new() { } };
            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory))
            {
                string extensions = Path.GetExtension(file);
                List<string> ignoreExt = new() { ".pdb", ".xml" };
                if (ignoreExt.Contains(extensions))
                    continue;
                tempHashFile.Hashes.Add(new() { FileName = file, FileHash = FileManager.File.GetHash(file, FileManager.File.HashType.SHA256) });
            }

            _file = tempHashFile;
            return tempHashFile;
        }

        internal HashFile GenAndSave()
        {
            HashFile tempHashFile = new() { Hashes = new() { } };
            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory))
            {
                string extensions = Path.GetExtension(file);
                List<string> ignoreExt = new() { ".pdb", ".xml" };
                if (ignoreExt.Contains(extensions))
                    continue;
                tempHashFile.Hashes.Add(new() { FileName = file, FileHash = FileManager.File.GetHash(file, FileManager.File.HashType.SHA256) });
            }

            FileManager.File.Delete(_path);
            FileManager.File.WriteAllText(_path, JsonSerializer.Serialize(tempHashFile, new JsonSerializerOptions { WriteIndented = true }));

            _file = tempHashFile;
            return tempHashFile;
        }

        internal void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            FileManager.File.WriteAllText(_path, JsonSerializer.Serialize(_file, options));
        }
    }

    internal class Hash
    {
        public required string FileName { get; set; }

        public required string FileHash { get; set; }
    }

    internal class HashFile
    {
        public required List<Hash> Hashes { get; set; }
    }
}
