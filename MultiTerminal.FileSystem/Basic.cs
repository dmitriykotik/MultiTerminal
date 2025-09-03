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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="longListing"></param>
        /// <param name="all"></param>
        /// <param name="human"></param>
        /// <param name="recursive"></param>
        /// <param name="reverse"></param>
        /// <param name="pattern"></param>
        /// <param name="dirStyle"></param>
        /// <param name="color"></param>
        public void List(string? path = null,
                            bool longListing = false,
                            bool all = false,
                            bool human = false,
                            bool recursive = false,
                            bool reverse = false,
                            string pattern = "*",
                            bool dirStyle = false,
                            bool color = true)
        {
            DefaultOutput output = new();
            string dirPath = Path.GetFullPath(path ?? _current);

            if (!Directory.Exists(dirPath))
            {
                output.WriteLine($"ls: cannot access '{dirPath}': No such directory", ConsoleColor.Red);
                return;
            }

            try
            {
                if (dirStyle)
                    output.WriteLine($" Directory of {dirPath}");

                string[] entries;
                try { entries = Directory.GetFileSystemEntries(dirPath, pattern).OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase).ToArray(); }
                catch { entries = Array.Empty<string>(); }

                if (!all)
                    entries = entries.Where(e => !Path.GetFileName(e).StartsWith(".") && (File.GetAttributes(e) & FileAttributes.Hidden) == 0).ToArray();

                if (reverse)
                    entries = entries.Reverse().ToArray();

                int sizeWidth = 0;
                if (longListing)
                {
                    foreach (var e in entries)
                    {
                        if (Directory.Exists(e))
                            sizeWidth = Math.Max(sizeWidth, "<DIR>".Length);
                        else
                        {
                            try
                            {
                                long len = new FileInfo(e).Length;
                                string s = FormatSize(len, human);
                                sizeWidth = Math.Max(sizeWidth, s.Length);
                            }
                            catch { }
                        }
                    }
                }

                long totalFiles = 0;
                long totalBytes = 0;
                int totalDirs = 0;

                foreach (var e in entries)
                {
                    bool isDir = Directory.Exists(e);
                    string name = Path.GetFileName(e);

                    if (longListing)
                    {
                        FileAttributes attrs;
                        try { attrs = File.GetAttributes(e); }
                        catch { attrs = 0; }

                        bool canWrite = (attrs & FileAttributes.ReadOnly) == 0;
                        string perms = (isDir ? "d" : "-") + (canWrite ? "rw" : "r-") + "x------";

                        int links = 1;
                        string owner = "-";
                        string group = "-";

                        string sizeStr;
                        if (isDir)
                            sizeStr = "<DIR>".PadLeft(sizeWidth);
                        else
                        {
                            try
                            {
                                long len = new FileInfo(e).Length;
                                totalBytes += len;
                                sizeStr = FormatSize(len, human).PadLeft(sizeWidth);
                            }
                            catch { sizeStr = "0".PadLeft(sizeWidth); }
                        }

                        string mtime;
                        try { mtime = File.GetLastWriteTime(e).ToString("MMM dd HH:mm", CultureInfo.InvariantCulture); }
                        catch { mtime = "???"; }

                        output.Write($"{perms} {links,3} {owner,8} {group,8} {sizeStr} {mtime} ");
                        if (color)
                        {
                            var col = GetColorForEntry(e);
                            output.Write(name, col, null);
                        }
                        else
                            output.Write(name);
                        output.WriteLine("");
                    }
                    else
                    {
                        if (color)
                        {
                            var col = GetColorForEntry(e);
                            output.Write(name, col, null);
                        }
                        else
                            output.Write(name);
                        output.Write("  ");
                    }

                    if (isDir) totalDirs++; else totalFiles++;
                }

                if (!longListing)
                    output.WriteLine("");

                if (dirStyle)
                {
                    output.WriteLine("");
                    output.WriteLine($" {totalFiles} File(s) {FormatSize(totalBytes, false)} bytes");
                    output.WriteLine($" {totalDirs} Dir(s)");
                }

                if (recursive)
                {
                    foreach (var d in entries.Where(e => Directory.Exists(e)))
                    {
                        output.WriteLine("");
                        List(d, longListing, all, human, true, reverse, pattern, dirStyle: true, color: color);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                output.WriteLine($"ls: cannot open directory '{dirPath}': Permission denied", ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                output.WriteLine($"ls: error reading '{dirPath}': {ex.Message}", ConsoleColor.Red);
            }
        }

        public bool Mkdir(string path, bool recursive = false)
        {
            DefaultOutput output = new();
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    output.WriteLine("mkdir: missing operand", ConsoleColor.Red);
                    return false;
                }

                if (path == "~")
                    path = _home;
                else if (path.StartsWith("~"))
                    path = Path.Combine(_home, path.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                string fullPath;
                if (Path.IsPathRooted(path))
                    fullPath = Path.GetFullPath(path);
                else
                    fullPath = Path.GetFullPath(Path.Combine(_current, path));

                if (Directory.Exists(fullPath))
                {
                    if (recursive)
                        return true;
                    else
                    {
                        output.WriteLine($"mkdir: cannot create directory '{path}': File exists", ConsoleColor.Red);
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
                        output.WriteLine($"mkdir: cannot create directory '{path}': No such file or directory", ConsoleColor.Red);
                        return false;
                    }

                    Directory.CreateDirectory(fullPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                output.WriteLine($"mkdir: cannot create directory '{path}': {ex.Message}", ConsoleColor.Red);
                return false;
            }
        }


        public void Touch(string path)
        {
            DefaultOutput output = new();
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    output.WriteLine("touch: missing file operand", ConsoleColor.Red);
                    return;
                }

                if (path == "~")
                    path = _home;
                else if (path.StartsWith("~"))
                    path = Path.Combine(_home, path.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                string fullPath = Path.IsPathRooted(path) ? Path.GetFullPath(path) : Path.GetFullPath(Path.Combine(_current, path));

                string? parent = Path.GetDirectoryName(fullPath);
                if (parent == null || !Directory.Exists(parent))
                {
                    output.WriteLine($"touch: cannot touch '{path}': No such file or directory", ConsoleColor.Red);
                    return;
                }

                if (File.Exists(fullPath))
                {
                    File.SetLastAccessTime(fullPath, DateTime.Now);
                    File.SetLastWriteTime(fullPath, DateTime.Now);
                }
                else
                {
                    using (File.Create(fullPath)) { }
                }
            }
            catch (Exception ex)
            {
                output.WriteLine($"touch: cannot touch '{path}': {ex.Message}", ConsoleColor.Red);
            }
        }


        public void Rm(string path, bool recursive = false, bool force = false)
        {
            DefaultOutput output = new();

            if (string.IsNullOrWhiteSpace(path))
            {
                if (!force) output.WriteLine("rm: missing operand", ConsoleColor.Red);
                return;
            }

            try
            {
                List<string> targets = new();

                string ResolveToFull(string p)
                {
                    if (p == "~")
                        p = _home;
                    else if (p.StartsWith("~"))
                        p = Path.Combine(_home, p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                    return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(_current, p));
                }

                if (path.Contains('*') || path.Contains('?'))
                {
                    string dirPart = Path.GetDirectoryName(path) ?? "";
                    string pattern = Path.GetFileName(path);

                    string dirResolved;
                    if (string.IsNullOrEmpty(dirPart))
                        dirResolved = _current;
                    else
                        dirResolved = ResolveToFull(dirPart);

                    try
                    {
                        if (Directory.Exists(dirResolved))
                        {
                            try
                            {
                                var files = Directory.GetFiles(dirResolved, pattern);
                                var dirs = Directory.GetDirectories(dirResolved, pattern);

                                targets.AddRange(files);
                                targets.AddRange(dirs);
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                else 
                    targets.Add(ResolveToFull(path));

                if (targets.Count == 0)
                {
                    if (!force)
                        output.WriteLine($"rm: cannot remove '{path}': No such file or directory", ConsoleColor.Red);
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
                                    output.WriteLine($"rm: cannot remove '{target}': Is a directory", ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            if (!force)
                                output.WriteLine($"rm: cannot remove '{target}': No such file or directory", ConsoleColor.Red);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (!force)
                            output.WriteLine($"rm: cannot remove '{target}': Permission denied", ConsoleColor.Red);
                    }
                    catch (IOException ex)
                    {
                        if (!force)
                            output.WriteLine($"rm: cannot remove '{target}': {ex.Message}", ConsoleColor.Red);
                    }
                    catch (Exception ex)
                    {
                        if (!force)
                            output.WriteLine($"rm: cannot remove '{target}': {ex.Message}", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!force)
                    output.WriteLine($"rm: error: {ex.Message}", ConsoleColor.Red);
            }
        }

        public void Rmdir(string path, bool recursive = false, bool force = false)
        {
            DefaultOutput output = new();

            if (string.IsNullOrWhiteSpace(path))
            {
                if (!force) output.WriteLine("rmdir: missing operand", ConsoleColor.Red);
                return;
            }
            
            string ResolveToFull(string p)
            {
                if (p == "~")
                    p = _home;
                else if (p.StartsWith("~"))
                    p = Path.Combine(_home, p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(_current, p));
            }

            try
            {
                var targets = new List<string>();

                if (path.Contains('*') || path.Contains('?'))
                {
                    string dirPart = Path.GetDirectoryName(path) ?? "";
                    string pattern = Path.GetFileName(path);

                    string dirResolved = string.IsNullOrEmpty(dirPart) ? _current : ResolveToFull(dirPart);

                    try
                    {
                        if (Directory.Exists(dirResolved))
                        {
                            var dirs = Directory.GetDirectories(dirResolved, pattern);
                            targets.AddRange(dirs);
                        }
                    }
                    catch { }
                }
                else
                    targets.Add(ResolveToFull(path));

                if (targets.Count == 0)
                {
                    if (!force)
                        output.WriteLine($"rmdir: failed to remove '{path}': No such file or directory", ConsoleColor.Red);
                    return;
                }

                foreach (var target in targets)
                {
                    try
                    {
                        if (Directory.Exists(target))
                        {
                            if (!recursive)
                            {
                                bool isEmpty;
                                try
                                {
                                    isEmpty = !Directory.EnumerateFileSystemEntries(target).Any();
                                }
                                catch (Exception ex)
                                {
                                    if (!force)
                                        output.WriteLine($"rmdir: failed to remove '{target}': {ex.Message}", ConsoleColor.Red);
                                    continue;
                                }

                                if (!isEmpty)
                                {
                                    if (!force)
                                        output.WriteLine($"rmdir: failed to remove '{target}': Directory not empty", ConsoleColor.Red);
                                    continue;
                                }

                                Directory.Delete(target);
                            }
                            else
                                Directory.Delete(target, true);
                        }
                        else if (File.Exists(target))
                        {
                            if (!force)
                                output.WriteLine($"rmdir: failed to remove '{target}': Not a directory", ConsoleColor.Red);
                        }
                        else
                        {
                            if (!force)
                                output.WriteLine($"rmdir: failed to remove '{target}': No such file or directory", ConsoleColor.Red);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (!force)
                            output.WriteLine($"rmdir: failed to remove '{target}': Permission denied", ConsoleColor.Red);
                    }
                    catch (IOException ex)
                    {
                        if (!force)
                            output.WriteLine($"rmdir: failed to remove '{target}': {ex.Message}", ConsoleColor.Red);
                    }
                    catch (Exception ex)
                    {
                        if (!force)
                            output.WriteLine($"rmdir: failed to remove '{target}': {ex.Message}", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!force)
                    output.WriteLine($"rmdir: error: {ex.Message}", ConsoleColor.Red);
            }
        }


        public void Mv(string srcPattern, string dst, bool overwrite = false, bool recursive = false)
        {
            DefaultOutput output = new();

            string ResolveFull(string p)
            {
                if (p == "~") p = _home;
                else if (p.StartsWith("~"))
                    p = Path.Combine(_home, p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(_current, p));
            }

            try
            {
                List<string> sources = new();

                if (srcPattern.Contains('*') || srcPattern.Contains('?'))
                {
                    string dirPart = Path.GetDirectoryName(srcPattern) ?? "";
                    string pattern = Path.GetFileName(srcPattern);

                    string dirResolved = string.IsNullOrEmpty(dirPart) ? _current : ResolveFull(dirPart);

                    try
                    {
                        if (Directory.Exists(dirResolved))
                        {
                            var files = Directory.GetFiles(dirResolved, pattern);
                            var dirs = Directory.GetDirectories(dirResolved, pattern);
                            sources.AddRange(files);
                            sources.AddRange(dirs);
                        }
                    }
                    catch { }
                }
                else
                    sources.Add(ResolveFull(srcPattern));

                if (sources.Count == 0)
                {
                    output.WriteLine($"mv: cannot stat '{srcPattern}': No such file or directory", ConsoleColor.Red);
                    return;
                }

                string dstResolved = ResolveFull(dst);

                bool dstIsDir = Directory.Exists(dstResolved) || (sources.Count > 1);
                string dstDir = dstIsDir ? dstResolved : Path.GetDirectoryName(dstResolved) ?? dstResolved;

                if (dstIsDir && !Directory.Exists(dstResolved))
                {
                    output.WriteLine($"mv: target '{dst}' is not a directory", ConsoleColor.Red);
                    return;
                }

                foreach (var src in sources)
                {
                    try
                    {
                        string name = Path.GetFileName(src);
                        string targetPath = dstIsDir ? Path.Combine(dstResolved, name) : dstResolved;

                        if (File.Exists(src))
                        {
                            if (File.Exists(targetPath) && !overwrite)
                            {
                                output.WriteLine($"mv: cannot overwrite '{targetPath}': File exists", ConsoleColor.Red);
                                continue;
                            }
                            if (File.Exists(targetPath))
                                File.Delete(targetPath);

                            string? parent = Path.GetDirectoryName(targetPath);
                            if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                            {
                                output.WriteLine($"mv: cannot move '{src}': No such directory '{parent}'", ConsoleColor.Red);
                                continue;
                            }

                            File.Move(src, targetPath);
                        }
                        else if (Directory.Exists(src))
                        {
                            if (!recursive)
                            {
                                output.WriteLine($"mv: cannot move '{src}': Is a directory", ConsoleColor.Red);
                                continue;
                            }

                            if (Directory.Exists(targetPath) && Directory.EnumerateFileSystemEntries(targetPath).Any() && !overwrite)
                            {
                                output.WriteLine($"mv: cannot overwrite directory '{targetPath}'", ConsoleColor.Red);
                                continue;
                            }
                            if (Directory.Exists(targetPath))
                                Directory.Delete(targetPath, true);

                            string? parent = Path.GetDirectoryName(targetPath);
                            if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent) && !dstIsDir)
                            {
                                output.WriteLine($"mv: cannot move '{src}': No such directory '{parent}'", ConsoleColor.Red);
                                continue;
                            }

                            Directory.Move(src, targetPath);
                        }
                        else
                            output.WriteLine($"mv: cannot stat '{src}': No such file or directory", ConsoleColor.Red);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        output.WriteLine($"mv: cannot move '{src}': Permission denied", ConsoleColor.Red);
                    }
                    catch (IOException ex)
                    {
                        output.WriteLine($"mv: cannot move '{src}': {ex.Message}", ConsoleColor.Red);
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine($"mv: cannot move '{src}': {ex.Message}", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                output.WriteLine($"mv: error: {ex.Message}", ConsoleColor.Red);
            }
        }

        public void Cp(string srcPattern, string dst, bool overwrite = false, bool recursive = false)
        {
            DefaultOutput output = new();

            string ResolveFull(string p)
            {
                if (p == "~") p = _home;
                else if (p.StartsWith("~"))
                    p = Path.Combine(_home, p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(_current, p));
            }

            try
            {
                List<string> sources = new();

                if (srcPattern.Contains('*') || srcPattern.Contains('?'))
                {
                    string dirPart = Path.GetDirectoryName(srcPattern) ?? "";
                    string pattern = Path.GetFileName(srcPattern);

                    string dirResolved = string.IsNullOrEmpty(dirPart) ? _current : ResolveFull(dirPart);

                    try
                    {
                        if (Directory.Exists(dirResolved))
                        {
                            var files = Directory.GetFiles(dirResolved, pattern);
                            var dirs = Directory.GetDirectories(dirResolved, pattern);
                            sources.AddRange(files);
                            sources.AddRange(dirs);
                        }
                    }
                    catch { }
                }
                else
                    sources.Add(ResolveFull(srcPattern));

                if (sources.Count == 0)
                {
                    output.WriteLine($"cp: cannot stat '{srcPattern}': No such file or directory", ConsoleColor.Red);
                    return;
                }

                string dstResolved = ResolveFull(dst);
                bool dstIsDir = Directory.Exists(dstResolved) || (sources.Count > 1);

                if (dstIsDir && !Directory.Exists(dstResolved))
                {
                    try { Directory.CreateDirectory(dstResolved); }
                    catch (Exception ex)
                    {
                        output.WriteLine($"cp: cannot create directory '{dst}': {ex.Message}", ConsoleColor.Red);
                        return;
                    }
                }

                foreach (var src in sources)
                {
                    try
                    {
                        string name = Path.GetFileName(src);
                        string targetPath = dstIsDir ? Path.Combine(dstResolved, name) : dstResolved;

                        if (File.Exists(src))
                        {
                            if (File.Exists(targetPath) && !overwrite)
                            {
                                output.WriteLine($"cp: cannot overwrite '{targetPath}': File exists", ConsoleColor.Red);
                                continue;
                            }

                            string? parent = Path.GetDirectoryName(targetPath);
                            if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                            {
                                try { Directory.CreateDirectory(parent); }
                                catch (Exception ex)
                                {
                                    output.WriteLine($"cp: cannot copy '{src}': {ex.Message}", ConsoleColor.Red);
                                    continue;
                                }
                            }

                            File.Copy(src, targetPath, overwrite);
                        }
                        else if (Directory.Exists(src))
                        {
                            if (!recursive)
                            {
                                output.WriteLine($"cp: omitting directory '{src}' (use -r to copy directories)", ConsoleColor.Red);
                                continue;
                            }

                            CopyDirectory(src, targetPath, overwrite);
                        }
                        else
                            output.WriteLine($"cp: cannot stat '{src}': No such file or directory", ConsoleColor.Red);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        output.WriteLine($"cp: cannot copy '{src}': Permission denied", ConsoleColor.Red);
                    }
                    catch (IOException ex)
                    {
                        output.WriteLine($"cp: cannot copy '{src}': {ex.Message}", ConsoleColor.Red);
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine($"cp: cannot copy '{src}': {ex.Message}", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                output.WriteLine($"cp: error: {ex.Message}", ConsoleColor.Red);
            }
        }

        public void Find(string root = ".", string? namePattern = null, string? type = null, int? maxDepth = null)
        {
            DefaultOutput output = new();

            string ResolveFull(string p)
            {
                if (p == "~")
                    p = _home;
                else if (p.StartsWith("~"))
                    p = Path.Combine(_home, p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(_current, p));
            }

            try
            {
                string resolvedRoot = ResolveFull(root);

                if (!Directory.Exists(resolvedRoot))
                {
                    output.WriteLine($"find: '{root}': No such directory", ConsoleColor.Red);
                    return;
                }

                FindStreamInner(resolvedRoot, namePattern, type, 0, maxDepth, output);
            }
            catch (Exception ex)
            {
                output.WriteLine($"find: error: {ex.Message}", ConsoleColor.Red);
            }
        }

        public void Tree(string root = ".", int? maxDepth = null)
        {
            DefaultOutput output = new();

            string ResolveFull(string p)
            {
                if (p == "~")
                    p = _home;
                else if (p.StartsWith("~"))
                    p = Path.Combine(_home, p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(_current, p));
            }

            try
            {
                string resolvedRoot = ResolveFull(root);

                if (!Directory.Exists(resolvedRoot))
                {
                    output.WriteLine($"tree: cannot access '{root}': No such directory", ConsoleColor.Red);
                    return;
                }

                PrintTree(resolvedRoot, "", true, 0, maxDepth);
            }
            catch (Exception ex)
            {
                output.WriteLine($"tree: error: {ex.Message}", ConsoleColor.Red);
            }
        }


        #region Private Methods
        private ConsoleColor GetColorForEntry(string path)
        {
            try
            {
                if (Directory.Exists(path)) return ConsoleColor.Blue;

                var attrs = File.GetAttributes(path);
                if ((attrs & FileAttributes.Hidden) != 0) return ConsoleColor.DarkGray;

                var ext = Path.GetExtension(path).ToLowerInvariant();
                string[] execExt = { ".exe", ".bat", ".cmd", ".sh", ".ps1", ".com" };
                if (execExt.Contains(ext)) return ConsoleColor.Green;

                string[] docExt = { ".txt", ".md", ".log", ".json", ".xml", ".csv" };
                if (docExt.Contains(ext)) return ConsoleColor.Cyan;

                return ConsoleColor.Gray;
            }
            catch
            {
                return ConsoleColor.Gray;
            }
        }

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

        private static void FindStreamInner(string dir, string? namePattern, string? type, int depth, int? maxDepth, DefaultOutput output)
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
                        output.WriteLine(entry);
                }

                if (isDir)
                    FindStreamInner(entry, namePattern, type, depth + 1, maxDepth, output);
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
