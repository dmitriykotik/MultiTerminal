using MultiTerminal.Output;
using System.Globalization;
using MultiAPI;
using System.Text.RegularExpressions;

namespace MultiTerminal.FileSystem
{
    /// <summary>
    /// Стандартный класс доступа к файловой системе
    /// </summary>
    public class Basic
    {
        private string _home;
        private string _current;
        private readonly string username;
        private readonly string hostname;

        public Basic()
        {
            username = Environment.UserName;
            hostname = Environment.MachineName;
            _home = GetHome();
            _current = _home;
        }

        public string GetUsername() => username;

        public string GetHostname() => hostname;

        public string GetHome()
        {
            if (OS.Get() == OS.OSTypes.Windows)
                return Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            else
            {
                var home = Environment.GetEnvironmentVariable("HOME");
                return !string.IsNullOrEmpty(home) ? home : "/";
            }
        }

        public string GetCurrent() => _current;

        public bool Cd(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "~")
            {
                _current = _home;
                return true;
            }

            if (path.StartsWith("~"))
                path = Path.Combine(_home, path.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            string targetPath;
            if (Path.IsPathRooted(path))
                targetPath = path;
            else
                targetPath = Path.GetFullPath(Path.Combine(_current, path));

            if (Directory.Exists(targetPath))
            {
                _current = targetPath;
                return true;
            }

            if (OS.Get() == OS.OSTypes.Windows)
            {
                if (path.Length >= 2 && path[1] == ':' && Directory.Exists(path + "\\"))
                {
                    _current = path + "\\";
                    return true;
                }
            }

            return false;
        }

        public string Pwd() => _current;

        public void Ls(string path = ".", bool showHidden = false, bool longList = false, bool human = false)
        {
            DefaultOutput output = new();
            if (!Directory.Exists(path))
            {
                output.WriteLine($"{path}: No such directory");
                return;
            }

            var entries = Directory.GetFileSystemEntries(path)
                .Select(f => new FileInfo(f));

            if (!showHidden)
                entries = entries.Where(f => !IsHidden(f));

            if (longList)
            {
                int sizeWidth = entries.Any() ? entries.Max(e => FormatSize(e.Length, human).Length) : 0;
                foreach (var entry in entries)
                    PrintLong(entry, sizeWidth, human);
            }
            else
            {
                foreach (var entry in entries)
                    PrintName(entry);
            }
        }

        public bool Mkdir(string path, bool recursive = false)
        {
            DefaultOutput output = new();
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    output.WriteLine("mkdir: missing operand");
                    return false;
                }

                string fullPath = Path.GetFullPath(path);

                if (Directory.Exists(fullPath))
                {
                    if (recursive)
                        return true;
                    else
                    {
                        output.WriteLine($"mkdir: cannot create directory '{path}': File exists");
                        return false;
                    }
                }

                if (recursive)
                    Directory.CreateDirectory(fullPath);
                else
                {
                    string? parent = Path.GetDirectoryName(fullPath);
                    if (parent == null || !Directory.Exists(parent))
                    {
                        output.WriteLine($"mkdir: cannot create directory '{path}': No such file or directory");
                        return false;
                    }

                    Directory.CreateDirectory(fullPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                output.WriteLine($"mkdir: cannot create directory '{path}': {ex.Message}");
                return false;
            }
        }

        public void Touch(string path)
        {
            try
            {
                string fullPath = Path.GetFullPath(path);

                if (File.Exists(fullPath))
                {
                    File.SetLastAccessTime(fullPath, DateTime.Now);
                    File.SetLastWriteTime(fullPath, DateTime.Now);
                }
                else 
                    File.Create(fullPath).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"touch: cannot touch '{path}': {ex.Message}");
            }
        }

        public void Rm(string path, bool recursive = false, bool force = false)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                if (!force) Console.WriteLine("rm: missing operand");
                return;
            }

            try
            {
                string baseDir = Directory.GetCurrentDirectory();

                List<string> targets = new();

                if (path.Contains('*') || path.Contains('?'))
                {
                    string dir = Path.GetDirectoryName(path) ?? baseDir;
                    string pattern = Path.GetFileName(path);

                    var files = Directory.GetFiles(dir, pattern);
                    var dirs = Directory.GetDirectories(dir, pattern);

                    targets.AddRange(files);
                    targets.AddRange(dirs);
                }
                else
                {
                    targets.Add(Path.GetFullPath(path));
                }

                if (targets.Count == 0)
                {
                    if (!force)
                        Console.WriteLine($"rm: cannot remove '{path}': No such file or directory");
                    return;
                }

                foreach (var target in targets)
                {
                    try
                    {
                        if (File.Exists(target))
                            File.Delete(target);
                        else if (Directory.Exists(target))
                        {
                            if (recursive)
                                Directory.Delete(target, true);
                            else
                            {
                                if (!force)
                                    Console.WriteLine($"rm: cannot remove '{target}': Is a directory");
                            }
                        }
                        else if (!force)
                            Console.WriteLine($"rm: cannot remove '{target}': No such file or directory");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (!force)
                            Console.WriteLine($"rm: cannot remove '{target}': Permission denied");
                    }
                    catch (IOException ex)
                    {
                        if (!force)
                            Console.WriteLine($"rm: cannot remove '{target}': {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        if (!force)
                            Console.WriteLine($"rm: cannot remove '{target}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!force)
                    Console.WriteLine($"rm: error: {ex.Message}");
            }
        }

        public void Mv(string srcPattern, string dst, bool overwrite = false, bool recursive = false)
        {
            string baseDir = Directory.GetCurrentDirectory();
            List<string> sources = new();

            if (srcPattern.Contains("*") || srcPattern.Contains("?"))
            {
                string dir = Path.GetDirectoryName(srcPattern) ?? baseDir;
                string pattern = Path.GetFileName(srcPattern);
                sources.AddRange(Directory.GetFiles(dir, pattern));
                sources.AddRange(Directory.GetDirectories(dir, pattern));
            }
            else
            {
                sources.Add(Path.GetFullPath(srcPattern));
            }

            if (sources.Count == 0)
            {
                Console.WriteLine($"mv: cannot stat '{srcPattern}': No such file or directory");
                return;
            }

            bool dstIsDir = Directory.Exists(dst) || (sources.Count > 1);
            string dstDir = dstIsDir ? dst : Path.GetDirectoryName(dst)!;

            if (dstIsDir && !Directory.Exists(dst))
            {
                Console.WriteLine($"mv: target '{dst}' is not a directory");
                return;
            }

            foreach (var src in sources)
            {
                try
                {
                    string name = Path.GetFileName(src);
                    string targetPath = dstIsDir ? Path.Combine(dst, name) : dst;

                    if (File.Exists(src))
                    {
                        if (File.Exists(targetPath) && !overwrite)
                        {
                            Console.WriteLine($"mv: cannot overwrite '{targetPath}': File exists");
                            continue;
                        }
                        if (File.Exists(targetPath))
                            File.Delete(targetPath);
                        File.Move(src, targetPath);
                    }
                    else if (Directory.Exists(src))
                    {
                        if (!recursive)
                        {
                            Console.WriteLine($"mv: cannot move '{src}': Is a directory");
                            continue;
                        }
                        if (Directory.Exists(targetPath) && Directory.EnumerateFileSystemEntries(targetPath).Any() && !overwrite)
                        {
                            Console.WriteLine($"mv: cannot overwrite directory '{targetPath}'");
                            continue;
                        }
                        if (Directory.Exists(targetPath))
                            Directory.Delete(targetPath, true);
                        Directory.Move(src, targetPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"mv: cannot move '{src}': {ex.Message}");
                }
            }
        }

        public void Cp(string srcPattern, string dst, bool overwrite = false, bool recursive = false)
        {
            string baseDir = Directory.GetCurrentDirectory();
            List<string> sources = new();

            if (srcPattern.Contains("*") || srcPattern.Contains("?"))
            {
                string dir = Path.GetDirectoryName(srcPattern) ?? baseDir;
                string pattern = Path.GetFileName(srcPattern);
                sources.AddRange(Directory.GetFiles(dir, pattern));
                sources.AddRange(Directory.GetDirectories(dir, pattern));
            }
            else
                sources.Add(Path.GetFullPath(srcPattern));

            if (sources.Count == 0)
            {
                Console.WriteLine($"cp: cannot stat '{srcPattern}': No such file or directory");
                return;
            }

            bool dstIsDir = Directory.Exists(dst) || (sources.Count > 1);
            if (dstIsDir && !Directory.Exists(dst))
                Directory.CreateDirectory(dst);

            foreach (var src in sources)
            {
                try
                {
                    string name = Path.GetFileName(src);
                    string targetPath = dstIsDir ? Path.Combine(dst, name) : dst;

                    if (File.Exists(src))
                    {
                        if (File.Exists(targetPath) && !overwrite)
                        {
                            Console.WriteLine($"cp: cannot overwrite '{targetPath}': File exists");
                            continue;
                        }
                        File.Copy(src, targetPath, overwrite);
                    }
                    else if (Directory.Exists(src))
                    {
                        if (!recursive)
                        {
                            Console.WriteLine($"cp: omitting directory '{src}' (use -r to copy directories)");
                            continue;
                        }
                        CopyDirectory(src, targetPath, overwrite);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"cp: cannot copy '{src}': {ex.Message}");
                }
            }
        }

        public IEnumerable<string> Find(string root = ".", string? namePattern = null, string? type = null, int? maxDepth = null)
        {
            var results = new List<string>();
            FindInner(Path.GetFullPath(root), namePattern, type, 0, maxDepth, results);
            return results;
        }

        public void Tree(string root = ".", int? maxDepth = null) => PrintTree(Path.GetFullPath(root), "", true, 0, maxDepth);


        #region Private Methods
        private void CopyDirectory(string sourceDir, string destDir, bool overwrite)
        {
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite);
            }
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                CopyDirectory(dir, Path.Combine(destDir, Path.GetFileName(dir)), overwrite);
            }
        }

        private void PrintLong(FileInfo file, int sizeWidth, bool human)
        {
            DefaultOutput output = new();
            string perms = GetPermissions(file);
            int links = 1;
            string owner = "-";
            string group = "-";
            string size = FormatSize(file.Length, human).PadLeft(sizeWidth);
            string mtime = file.LastWriteTime.ToString("MMM dd HH:mm", CultureInfo.InvariantCulture);

            output.WriteLine($"{perms} {links,3} {owner,8} {group,8} {size} {mtime} {file.Name}");
        }

        private void PrintName(FileInfo file)
        {
            DefaultOutput output = new();
            if ((file.Attributes & FileAttributes.Directory) != 0)
                output.Write(file.Name, ConsoleColor.Blue, null);
            else
                output.Write(file.Name);
            output.Write("  ");
        }

        private string FormatSize(long size, bool human)
        {
            if (!human) return size.ToString();
            string[] symbols = { "", "K", "M", "G", "T" };
            double readable = size;
            int idx = 0;
            while (readable >= 1024 && idx < symbols.Length - 1)
            {
                readable /= 1024;
                idx++;
            }
            return $"{readable:0.##}{symbols[idx]}";
        }

        private string GetPermissions(FileInfo file)
        {
            bool isDir = (file.Attributes & FileAttributes.Directory) != 0;
            bool canWrite = (file.Attributes & FileAttributes.ReadOnly) == 0;
            return (isDir ? "d" : "-") +
                   (file.Exists ? "r" : "-") +
                   (canWrite ? "w" : "-") + "x------";
        }

        private bool IsHidden(FileInfo file)
        {
            return file.Name.StartsWith(".") ||
                   (file.Attributes & FileAttributes.Hidden) != 0;
        }

        private static void FindInner(string dir, string? namePattern, string? type, int depth, int? maxDepth, List<string> results)
        {
            if (maxDepth.HasValue && depth > maxDepth) return;

            IEnumerable<string>? entries;
            try
            {
                entries = Directory.EnumerateFileSystemEntries(dir);
            }
            catch
            {
                return;
            }

            foreach (var entry in entries)
            {
                bool isDir = Directory.Exists(entry);
                bool isFile = File.Exists(entry);

                if ((type == null) ||
                    (type == "f" && isFile) ||
                    (type == "d" && isDir))
                {
                    if (namePattern == null || WildcardMatch(Path.GetFileName(entry), namePattern))
                        results.Add(entry);
                }
                if (isDir)
                    FindInner(entry, namePattern, type, depth + 1, maxDepth, results);
            }
        }

        private static bool WildcardMatch(string str, string pattern)
        {
            var regex = "^" + Regex.Escape(pattern)
                                  .Replace("\\*", ".*")
                                  .Replace("\\?", ".") + "$";
            return Regex.IsMatch(str, regex, RegexOptions.IgnoreCase);
        }

        private static void PrintTree(string dir, string indent, bool last, int depth, int? maxDepth)
        {
            DefaultOutput output = new();
            if (maxDepth.HasValue && depth > maxDepth)
                return;

            string prefix = indent + (depth == 0 ? "" : (last ? "└── " : "├── "));
            output.WriteLine($"{prefix}{Path.GetFileName(dir)}{(Directory.Exists(dir) ? "/" : "")}");

            if (!Directory.Exists(dir)) return;

            var entries = Directory.GetFileSystemEntries(dir).OrderBy(x => x).ToArray();
            for (int i = 0; i < entries.Length; i++)
            {
                bool isLast = i == entries.Length - 1;
                if (Directory.Exists(entries[i]))
                    PrintTree(entries[i], indent + (depth == 0 ? "" : (last ? "    " : "│   ")), isLast, depth + 1, maxDepth);
                else
                    output.WriteLine($"{indent}{(depth == 0 ? "" : (last ? "    " : "│   "))}{(isLast ? "└── " : "├── ")}{Path.GetFileName(entries[i])}");
            }
        }
        #endregion
    }
}
