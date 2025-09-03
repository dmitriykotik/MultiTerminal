using MultiTerminal.Arguments;
using MultiTerminal.PasswordManager;
using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class Commands
    {
        internal static void Help(string[] Args)
        {

        }

        internal static void Tree(string[] args)
        {
            if (args == null) args = Array.Empty<string>();

            int? maxDepth = null;
            var roots = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < args.Length; j++)
                        roots.Add(args[j]);
                    break;
                }

                if (a == "--help")
                {
                    output.WriteLine("tree: Usage: tree [-L N|--maxdepth N] [PATH...]");
                    return;
                }

                if (a.StartsWith("--maxdepth="))
                {
                    var s = a.Substring("--maxdepth=".Length);
                    if (int.TryParse(s, out var n))
                        maxDepth = n;
                    else
                    {
                        output.WriteLine($"tree: invalid maxdepth '{s}'", ConsoleColor.Red);
                        return;
                    }
                    continue;
                }

                if (a == "--maxdepth")
                {
                    if (i + 1 >= args.Length)
                    {
                        output.WriteLine("tree: option requires an argument -- 'maxdepth'", ConsoleColor.Red);
                        return;
                    }
                    if (!int.TryParse(args[++i], out var n2))
                    {
                        output.WriteLine($"tree: invalid maxdepth '{args[i]}'", ConsoleColor.Red);
                        return;
                    }
                    maxDepth = n2;
                    continue;
                }

                if (a == "-L")
                {
                    if (i + 1 >= args.Length)
                    {
                        output.WriteLine("tree: option requires an argument -- 'L'", ConsoleColor.Red);
                        return;
                    }
                    if (!int.TryParse(args[++i], out var n3))
                    {
                        output.WriteLine($"tree: invalid level '{args[i]}'", ConsoleColor.Red);
                        return;
                    }
                    maxDepth = n3;
                    continue;
                }

                if (!a.StartsWith("-") || a == "." || a == "~" || a.StartsWith("~" + Path.DirectorySeparatorChar))
                {
                    roots.Add(a);
                    continue;
                }

                output.WriteLine($"tree: unrecognized option '{a}'", ConsoleColor.Red);
                return;
            }

            if (roots.Count == 0)
                roots.Add(".");

            foreach (var r in roots)
            {
                try
                {
                    fileSystem.Tree(r, maxDepth);
                }
                catch (Exception ex)
                {
                    output.WriteLine($"tree: error processing '{r}': {ex.Message}", ConsoleColor.Red);
                }
            }
        }

        internal static void Find(string[] args)
        {
            if (args == null) args = Array.Empty<string>();

            var roots = new List<string>();
            string? namePattern = null;
            string? type = null;
            int? maxDepth = null;

            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < args.Length; j++)
                        roots.Add(args[j]);
                    break;
                }

                if (a == "--help")
                {
                    output.WriteLine("find: Usage: find [PATH...] [-name PATTERN] [-type f|d] [-maxdepth N]");
                    return;
                }

                if (a.StartsWith("--name="))
                {
                    namePattern = a.Substring("--name=".Length);
                    continue;
                }
                if (a == "--name" || a == "-name")
                {
                    if (i + 1 >= args.Length)
                    {
                        output.WriteLine("find: option requires an argument -- 'name'", ConsoleColor.Red);
                        return;
                    }
                    namePattern = args[++i];
                    continue;
                }

                if (a.StartsWith("--type="))
                {
                    type = a.Substring("--type=".Length);
                    continue;
                }
                if (a == "--type" || a == "-type")
                {
                    if (i + 1 >= args.Length)
                    {
                        output.WriteLine("find: option requires an argument -- 'type'", ConsoleColor.Red);
                        return;
                    }
                    type = args[++i];
                    continue;
                }

                if (a.StartsWith("--maxdepth="))
                {
                    var s = a.Substring("--maxdepth=".Length);
                    if (int.TryParse(s, out var n))
                        maxDepth = n;
                    else
                    {
                        output.WriteLine($"find: invalid maxdepth '{s}'", ConsoleColor.Red);
                        return;
                    }
                    continue;
                }
                if (a == "--maxdepth" || a == "-maxdepth")
                {
                    if (i + 1 >= args.Length)
                    {
                        output.WriteLine("find: option requires an argument -- 'maxdepth'", ConsoleColor.Red);
                        return;
                    }
                    if (!int.TryParse(args[++i], out var n2))
                    {
                        output.WriteLine($"find: invalid maxdepth '{args[i]}'", ConsoleColor.Red);
                        return;
                    }
                    maxDepth = n2;
                    continue;
                }

                if (!a.StartsWith("-") || a == "." || a == "~" || a.StartsWith("~" + Path.DirectorySeparatorChar))
                {
                    roots.Add(a);
                    continue;
                }

                output.WriteLine($"find: unrecognized option '{a}'", ConsoleColor.Red);
                return;
            }

            if (roots.Count == 0)
                roots.Add(".");

            foreach (var r in roots)
            {
                try
                {
                    fileSystem.Find(r, namePattern, type, maxDepth);
                }
                catch (Exception ex)
                {
                    output.WriteLine($"find: error processing '{r}': {ex.Message}", ConsoleColor.Red);
                }
            }
        }

        internal static void Cp(string[] Args)
        {
            if (Args == null || Args.Length == 0)
            {
                output.WriteLine("cp: missing operand", ConsoleColor.Red);
                return;
            }

            bool overwrite = false;
            bool recursive = false;
            var paths = new List<string>();

            string ResolveFull(string p)
            {
                if (p == "~") p = fileSystem.GetHome();
                else if (p.StartsWith("~"))
                    p = Path.Combine(fileSystem.GetHome(), p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(fileSystem.GetCurrent(), p));
            }

            for (int i = 0; i < Args.Length; i++)
            {
                string a = Args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < Args.Length; j++)
                        paths.Add(Args[j]);
                    break;
                }

                if (a.StartsWith("--"))
                {
                    if (a == "--recursive")
                        recursive = true;
                    else if (a == "--force")
                        overwrite = true;
                    else if (a == "--no-clobber")
                        overwrite = false;
                    else if (a == "--help")
                    {
                        output.WriteLine("cp: Usage: cp [-r|--recursive] [-f|--force] [-n|--no-clobber] SOURCE... DEST");
                        return;
                    }
                    else
                    {
                        output.WriteLine($"cp: unrecognized option '{a}'", ConsoleColor.Red);
                        return;
                    }
                }
                else if (a.StartsWith("-") && a.Length > 1)
                {
                    for (int k = 1; k < a.Length; k++)
                    {
                        char opt = a[k];
                        switch (opt)
                        {
                            case 'r':
                            case 'R':
                                recursive = true;
                                break;
                            case 'f':
                                overwrite = true;
                                break;
                            case 'n':
                                overwrite = false;
                                break;
                            default:
                                output.WriteLine($"cp: invalid option -- '{opt}'", ConsoleColor.Red);
                                return;
                        }
                    }
                }
                else
                    paths.Add(a);
            }

            if (paths.Count < 2)
            {
                output.WriteLine("cp: missing file operand", ConsoleColor.Red);
                return;
            }

            string dst = paths.Last();
            var srcPatterns = paths.Take(paths.Count - 1).ToList();

            foreach (var sp in srcPatterns)
            {
                try
                {
                    bool hasMatches = false;

                    if (sp.Contains('*') || sp.Contains('?'))
                    {
                        string dirPart = Path.GetDirectoryName(sp) ?? "";
                        string pattern = Path.GetFileName(sp);
                        string dirResolved = string.IsNullOrEmpty(dirPart) ? fileSystem.GetCurrent() : ResolveFull(dirPart);

                        try
                        {
                            if (Directory.Exists(dirResolved))
                            {
                                var files = Directory.GetFiles(dirResolved, pattern);
                                var dirs = Directory.GetDirectories(dirResolved, pattern);
                                if (files.Length > 0 || dirs.Length > 0) hasMatches = true;
                            }
                        }
                        catch { }

                        if (!hasMatches)
                        {
                            output.WriteLine($"cp: cannot stat '{sp}': No such file or directory", ConsoleColor.Red);
                            continue;
                        }
                    }
                    else
                    {
                        string full = ResolveFull(sp);
                        if (!File.Exists(full) && !Directory.Exists(full))
                        {
                            output.WriteLine($"cp: cannot stat '{sp}': No such file or directory", ConsoleColor.Red);
                            continue;
                        }
                    }

                    fileSystem.Cp(sp, dst, overwrite, recursive);
                }
                catch (Exception ex)
                {
                    output.WriteLine($"cp: cannot process '{sp}': {ex.Message}", ConsoleColor.Red);
                }
            }
        }

        internal static void Mv(string[] Args)
        {
            if (Args == null || Args.Length == 0)
            {
                output.WriteLine("mv: missing operand", ConsoleColor.Red);
                return;
            }

            bool overwrite = false;
            bool recursive = false;
            var paths = new List<string>();

            string ResolveFull(string p)
            {
                if (p == "~") p = fileSystem.GetHome();
                else if (p.StartsWith("~"))
                    p = Path.Combine(fileSystem.GetHome(), p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(fileSystem.GetCurrent(), p));
            }

            for (int i = 0; i < Args.Length; i++)
            {
                string a = Args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < Args.Length; j++)
                        paths.Add(Args[j]);
                    break;
                }

                if (a.StartsWith("--"))
                {
                    if (a == "--force")
                        overwrite = true;
                    else if (a == "--no-clobber")
                        overwrite = false;
                    else if (a == "--recursive")
                        recursive = true;
                    else if (a == "--help")
                    {
                        output.WriteLine("mv: Usage: mv [-f|--force] [-n|--no-clobber] [-r|--recursive] SOURCE... DEST");
                        return;
                    }
                    else
                    {
                        output.WriteLine($"mv: unrecognized option '{a}'", ConsoleColor.Red);
                        return;
                    }
                }
                else if (a.StartsWith("-") && a.Length > 1)
                {
                    for (int k = 1; k < a.Length; k++)
                    {
                        char opt = a[k];
                        switch (opt)
                        {
                            case 'f':
                                overwrite = true;
                                break;
                            case 'n':
                                overwrite = false;
                                break;
                            case 'r':
                            case 'R':
                                recursive = true;
                                break;
                            default:
                                output.WriteLine($"mv: invalid option -- '{opt}'", ConsoleColor.Red);
                                return;
                        }
                    }
                }
                else
                    paths.Add(a);
            }

            if (paths.Count < 2)
            {
                output.WriteLine("mv: missing file operand", ConsoleColor.Red);
                return;
            }

            string dst = paths.Last();
            var srcPatterns = paths.Take(paths.Count - 1).ToList();

            if (srcPatterns.Count > 1)
            {
                string dstResolved = ResolveFull(dst);
                if (!Directory.Exists(dstResolved))
                {
                    output.WriteLine($"mv: target '{dst}' is not a directory", ConsoleColor.Red);
                    return;
                }
            }

            foreach (var sp in srcPatterns)
            {
                try
                {
                    bool hasMatches = false;
                    if (sp.Contains('*') || sp.Contains('?'))
                    {
                        string dirPart = Path.GetDirectoryName(sp) ?? "";
                        string pattern = Path.GetFileName(sp);
                        string dirResolved = string.IsNullOrEmpty(dirPart) ? fileSystem.GetCurrent() : ResolveFull(dirPart);

                        try
                        {
                            if (Directory.Exists(dirResolved))
                            {
                                var files = Directory.GetFiles(dirResolved, pattern);
                                var dirs = Directory.GetDirectories(dirResolved, pattern);
                                if (files.Length > 0 || dirs.Length > 0) hasMatches = true;
                            }
                        }
                        catch { }

                        if (!hasMatches)
                        {
                            output.WriteLine($"mv: cannot stat '{sp}': No such file or directory", ConsoleColor.Red);
                            continue;
                        }
                    }
                    else
                    {
                        string full = ResolveFull(sp);
                        if (!File.Exists(full) && !Directory.Exists(full))
                        {
                            output.WriteLine($"mv: cannot stat '{sp}': No such file or directory", ConsoleColor.Red);
                            continue;
                        }
                    }

                    fileSystem.Mv(sp, dst, overwrite, recursive);
                }
                catch (Exception ex)
                {
                    output.WriteLine($"mv: cannot process '{sp}': {ex.Message}", ConsoleColor.Red);
                }
            }
        }

        internal static void Rmdir(string[] Args)
        {
            if (Args == null || Args.Length == 0)
            {
                output.WriteLine("rmdir: missing operand", ConsoleColor.Red);
                return;
            }

            bool recursive = false;
            bool force = false;
            bool parents = false;
            var paths = new List<string>();

            string ResolveFull(string p)
            {
                if (p == "~")
                    p = fileSystem.GetHome();
                else if (p.StartsWith("~"))
                    p = Path.Combine(fileSystem.GetHome(), p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(fileSystem.GetCurrent(), p));
            }

            for (int i = 0; i < Args.Length; i++)
            {
                string a = Args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < Args.Length; j++)
                        paths.Add(Args[j]);
                    break;
                }

                if (a.StartsWith("--"))
                {
                    if (a == "--recursive")
                        recursive = true;
                    else if (a == "--force")
                        force = true;
                    else if (a == "--parents")
                        parents = true;
                    else if (a == "--help")
                    {
                        output.WriteLine("rmdir: Usage: rmdir [-p|--parents] [-r|--recursive] [-f|--force] DIRECTORY...");
                        return;
                    }
                    else
                    {
                        output.WriteLine($"rmdir: unrecognized option '{a}'", ConsoleColor.Red);
                        return;
                    }
                }
                else if (a.StartsWith("-") && a.Length > 1)
                {
                    for (int k = 1; k < a.Length; k++)
                    {
                        char opt = a[k];
                        switch (opt)
                        {
                            case 'r':
                            case 'R':
                                recursive = true;
                                break;
                            case 'f':
                                force = true;
                                break;
                            case 'p':
                                parents = true;
                                break;
                            default:
                                output.WriteLine($"rmdir: invalid option -- '{opt}'", ConsoleColor.Red);
                                return;
                        }
                    }
                }
                else
                    paths.Add(a);
            }

            if (paths.Count == 0)
            {
                output.WriteLine("rmdir: missing operand", ConsoleColor.Red);
                return;
            }

            void TryRemoveParents(string startDir)
            {
                try
                {
                    string parent = Path.GetDirectoryName(startDir);
                    while (!string.IsNullOrEmpty(parent))
                    {
                        string root = Path.GetPathRoot(parent) ?? parent;
                        if (string.Equals(parent, root, StringComparison.OrdinalIgnoreCase))
                            break;

                        if (!Directory.Exists(parent))
                        {
                            parent = Path.GetDirectoryName(parent);
                            continue;
                        }

                        bool isEmpty;
                        try
                        {
                            isEmpty = !Directory.EnumerateFileSystemEntries(parent).Any();
                        }
                        catch (Exception ex)
                        {
                            if (!force)
                                output.WriteLine($"rmdir: failed to remove '{parent}': {ex.Message}", ConsoleColor.Red);
                            break;
                        }

                        if (!isEmpty)
                            break;

                        try
                        {
                            Directory.Delete(parent);
                        }
                        catch (Exception ex)
                        {
                            if (!force)
                                output.WriteLine($"rmdir: failed to remove '{parent}': {ex.Message}", ConsoleColor.Red);
                            break;
                        }

                        parent = Path.GetDirectoryName(parent);
                    }
                }
                catch { }
            }

            foreach (var p in paths)
            {
                try
                {
                    if (p.Contains('*') || p.Contains('?'))
                    {
                        string dirPart = Path.GetDirectoryName(p) ?? "";
                        string pattern = Path.GetFileName(p);

                        string dirResolved = string.IsNullOrEmpty(dirPart) ? fileSystem.GetCurrent() : ResolveFull(dirPart);

                        string[] matches = Array.Empty<string>();
                        try
                        {
                            if (Directory.Exists(dirResolved))
                                matches = Directory.GetDirectories(dirResolved, pattern);
                        }
                        catch { }

                        if (matches.Length == 0)
                        {
                            if (!force)
                                output.WriteLine($"rmdir: failed to remove '{p}': No such file or directory", ConsoleColor.Red);
                            continue;
                        }

                        foreach (var m in matches)
                        {
                            fileSystem.Rmdir(m, recursive, force);

                            if (parents)
                                TryRemoveParents(m);
                        }
                    }
                    else
                    {
                        string full = ResolveFull(p);

                        fileSystem.Rmdir(p, recursive, force);

                        if (parents)
                            TryRemoveParents(full);
                    }
                }
                catch (Exception ex)
                {
                    if (!force)
                        output.WriteLine($"rmdir: cannot process '{p}': {ex.Message}", ConsoleColor.Red);
                }
            }
        }


        internal static void Rm(string[] Args)
        {
            if (Args == null || Args.Length == 0)
            {
                output.WriteLine("rm: missing operand", ConsoleColor.Red);
                return;
            }

            bool recursive = false;
            bool force = false;
            var paths = new List<string>();

            string ResolveFull(string p)
            {
                if (p == "~")
                    p = fileSystem.GetHome();
                else if (p.StartsWith("~"))
                    p = Path.Combine(fileSystem.GetHome(), p.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(p) ? Path.GetFullPath(p) : Path.GetFullPath(Path.Combine(fileSystem.GetCurrent(), p));
            }

            for (int i = 0; i < Args.Length; i++)
            {
                string a = Args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < Args.Length; j++)
                        paths.Add(Args[j]);
                    break;
                }

                if (a.StartsWith("--"))
                {
                    if (a == "--recursive")
                        recursive = true;
                    else if (a == "--force")
                        force = true;
                    else if (a == "--help")
                    {
                        output.WriteLine("rm: Usage: rm [-f|--force] [-r|-R|--recursive] FILE...");
                        return;
                    }
                    else
                    {
                        output.WriteLine($"rm: unrecognized option '{a}'", ConsoleColor.Red);
                        return;
                    }
                }
                else if (a.StartsWith("-") && a.Length > 1)
                {
                    for (int k = 1; k < a.Length; k++)
                    {
                        char opt = a[k];
                        switch (opt)
                        {
                            case 'r':
                            case 'R':
                                recursive = true;
                                break;
                            case 'f':
                                force = true;
                                break;
                            default:
                                output.WriteLine($"rm: invalid option -- '{opt}'", ConsoleColor.Red);
                                return;
                        }
                    }
                }
                else
                {
                    paths.Add(a);
                }
            }

            if (paths.Count == 0)
            {
                output.WriteLine("rm: missing operand", ConsoleColor.Red);
                return;
            }

            foreach (var p in paths)
            {
                try
                {
                    if (p.Contains('*') || p.Contains('?'))
                    {
                        string dirPart = Path.GetDirectoryName(p) ?? "";
                        string pattern = Path.GetFileName(p);

                        string dirResolved = string.IsNullOrEmpty(dirPart) ? fileSystem.GetCurrent() : ResolveFull(dirPart);

                        bool haveMatches = false;
                        try
                        {
                            if (Directory.Exists(dirResolved))
                            {
                                var files = Directory.GetFiles(dirResolved, pattern);
                                var dirs = Directory.GetDirectories(dirResolved, pattern);
                                if (files.Length > 0 || dirs.Length > 0)
                                    haveMatches = true;
                            }
                        }
                        catch { }

                        fileSystem.Rm(p, recursive, force);
                    }
                    else
                        fileSystem.Rm(p, recursive, force);
                }
                catch (Exception ex)
                {
                    if (!force)
                        output.WriteLine($"rm: cannot process '{p}': {ex.Message}", ConsoleColor.Red);
                }
            }
        }


        internal static void Touch(string[] Args)
        {
            if (Args == null || Args.Length == 0)
            {
                output.WriteLine("touch: missing file operand", ConsoleColor.Red);
                return;
            }

            bool noCreate = false;
            var paths = new List<string>();

            string ResolveFullPath(string path)
            {
                if (path == "~")
                    path = fileSystem.GetHome();
                else if (path.StartsWith("~"))
                    path = Path.Combine(fileSystem.GetHome(), path.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                return Path.IsPathRooted(path) ? Path.GetFullPath(path) : Path.GetFullPath(Path.Combine(fileSystem.GetCurrent(), path));
            }

            for (int i = 0; i < Args.Length; i++)
            {
                string a = Args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < Args.Length; j++)
                        paths.Add(Args[j]);
                    break;
                }

                if (a.StartsWith("--"))
                {
                    if (a == "--no-create")
                        noCreate = true;
                    else if (a == "--help")
                    {
                        output.WriteLine("touch: Usage: touch [-c|--no-create] FILE...");
                        return;
                    }
                    else
                    {
                        output.WriteLine($"touch: unrecognized option '{a}'", ConsoleColor.Red);
                        return;
                    }
                }
                else if (a.StartsWith("-") && a.Length > 1)
                {
                    for (int k = 1; k < a.Length; k++)
                    {
                        char opt = a[k];
                        switch (opt)
                        {
                            case 'c':
                                noCreate = true;
                                break;
                            default:
                                output.WriteLine($"touch: invalid option -- '{opt}'", ConsoleColor.Red);
                                return;
                        }
                    }
                }
                else
                {
                    paths.Add(a);
                }
            }

            if (paths.Count == 0)
            {
                output.WriteLine("touch: missing file operand", ConsoleColor.Red);
                return;
            }

            foreach (var p in paths)
            {
                try
                {
                    string fullPath = ResolveFullPath(p);

                    if (noCreate)
                    {
                        if (File.Exists(fullPath))
                        {
                            try
                            {
                                fileSystem.Touch(p);
                            }
                            catch (Exception ex)
                            {
                                output.WriteLine($"touch: cannot touch '{p}': {ex.Message}", ConsoleColor.Red);
                                return;
                            }
                        }
                        else
                        {
                            output.WriteLine($"touch: cannot touch '{p}': No such file or directory", ConsoleColor.Red);
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            fileSystem.Touch(p);

                            if (!File.Exists(fullPath))
                            {
                                output.WriteLine($"touch: cannot touch '{p}': Failed to create or update", ConsoleColor.Red);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            output.WriteLine($"touch: cannot touch '{p}': {ex.Message}", ConsoleColor.Red);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    output.WriteLine($"touch: cannot process '{p}': {ex.Message}", ConsoleColor.Red);
                    return;
                }
            }
        }

        internal static void Mkdir(string[] Args)
        {
            if (Args == null || Args.Length == 0)
            {
                output.WriteLine("mkdir: missing operand", ConsoleColor.Red);
                return;
            }

            bool recursive = false;
            var paths = new List<string>();

            for (int i = 0; i < Args.Length; i++)
            {
                string a = Args[i];

                if (a == "--")
                {
                    for (int j = i + 1; j < Args.Length; j++)
                        paths.Add(Args[j]);
                    break;
                }

                if (a.StartsWith("--"))
                {
                    if (a == "--parents")
                        recursive = true;
                    else if (a == "--help")
                    {
                        output.WriteLine("mkdir: Usage: mkdir [-p|--parents] DIR...");
                        return;
                    }
                    else
                    {
                        output.WriteLine($"mkdir: unrecognized option '{a}'", ConsoleColor.Red);
                        return;
                    }
                }
                else if (a.StartsWith("-") && a.Length > 1)
                {
                    for (int k = 1; k < a.Length; k++)
                    {
                        char opt = a[k];
                        switch (opt)
                        {
                            case 'p':
                                recursive = true;
                                break;
                            default:
                                output.WriteLine($"mkdir: invalid option -- '{opt}'", ConsoleColor.Red);
                                return;
                        }
                    }
                }
                else
                    paths.Add(a);
            }

            if (paths.Count == 0)
            {
                output.WriteLine("mkdir: missing operand", ConsoleColor.Red);
                return;
            }

            foreach (var p in paths)
                fileSystem.Mkdir(p, recursive);
        }


        internal static void Cd(string[] Args)
        {
            if (Args.Length == 0)
                output.WriteLine(fileSystem.GetCurrent());
            else if (!fileSystem.Cd(Args[0]))
                output.WriteLine("cd: The specified path cannot be found.", ConsoleColor.Red);
        }

        internal static void CdBack(string[] Args)
        {
            if (!fileSystem.Cd(".."))
                output.WriteLine("cd: The specified path cannot be found.", ConsoleColor.Red);
        }

        internal static void Ls(string[] args)
        {
            bool longListing = false;
            bool all = false;
            bool human = false;
            bool recursive = false;
            bool reverse = false;
            bool dirStyle = false;
            bool color = true;

            var paths = new List<string>();

            string helpText =
        @"Usage: ls [options] [path...]
Options:
  -l, --long             long listing
  -a, --all              include hidden files
  -h, --human            human readable sizes (e.g. 1K 234M)
  -R, --recursive        list subdirectories recursively
  -r, --reverse          reverse order while sorting
      --dirstyle         print header/summary like Windows 'dir'
      --no-color         disable colored output
      --pattern=PAT      filter by pattern (also supported as path/*.ext)
      --help             display this help and exit";

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string a = args[i];
                    if (string.IsNullOrEmpty(a)) continue;

                    if (a == "--help" || a == "-h" && a.Length == 2 && args.Length == 1)
                    {
                        output.WriteLine(helpText);
                        return;
                    }

                    if (a.StartsWith("--"))
                    {
                        if (a == "--long") longListing = true;
                        else if (a == "--all") all = true;
                        else if (a == "--human" || a == "--human-readable") human = true;
                        else if (a == "--recursive") recursive = true;
                        else if (a == "--reverse") reverse = true;
                        else if (a == "--dirstyle") dirStyle = true;
                        else if (a == "--no-color") color = false;
                        else if (a.StartsWith("--pattern="))
                        {
                            var val = a.Substring("--pattern=".Length);
                            if (!string.IsNullOrEmpty(val)) paths.Add(val);
                        }
                        else if (a == "--help")
                        {
                            output.WriteLine(helpText);
                            return;
                        }
                        else
                        {
                            output.WriteLine($"ls: unknown option '{a}'", ConsoleColor.Red);
                            return;
                        }
                    }
                    else if (a.StartsWith("-") && a.Length > 1)
                    {
                        for (int k = 1; k < a.Length; k++)
                        {
                            char f = a[k];
                            switch (f)
                            {
                                case 'l': longListing = true; break;
                                case 'a': all = true; break;
                                case 'h': human = true; break;
                                case 'R': recursive = true; break;
                                case 'r': reverse = true; break;
                                default:
                                    output.WriteLine($"ls: invalid option -- '{f}'", ConsoleColor.Red);
                                    return;
                            }
                        }
                    }
                    else
                    {
                        paths.Add(a);
                    }
                }

                if (paths.Count == 0)
                {
                    fileSystem.List(null, longListing, all, human, recursive, reverse, "*", dirStyle, color);
                    return;
                }
                bool first = true;
                foreach (var raw in paths)
                {
                    string arg = raw.Trim();

                    if (!first)
                        output.WriteLine("");
                    first = false;

                    bool hasGlob = arg.Contains('*') || arg.Contains('?');

                    if (hasGlob)
                    {
                        string dirPart = Path.GetDirectoryName(arg);
                        string patternPart = Path.GetFileName(arg);

                        if (string.IsNullOrEmpty(dirPart))
                            dirPart = fileSystem.GetCurrent();

                        try
                        {
                            string fullDir = Path.GetFullPath(dirPart);
                            if (paths.Count > 1)
                                output.WriteLine($"{fullDir}:");
                            fileSystem.List(fullDir, longListing, all, human, recursive, reverse, patternPart, dirStyle, color);
                        }
                        catch
                        {
                            output.WriteLine($"ls: cannot access '{arg}': Invalid path", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        try
                        {
                            string candidate = Path.GetFullPath(arg);
                            if (Directory.Exists(candidate))
                            {
                                if (paths.Count > 1)
                                    output.WriteLine($"{candidate}:");
                                fileSystem.List(candidate, longListing, all, human, recursive, reverse, "*", dirStyle, color);
                            }
                            else if (File.Exists(candidate))
                            {
                                string parent = Path.GetDirectoryName(candidate) ?? fileSystem.GetCurrent();
                                string name = Path.GetFileName(candidate);
                                if (paths.Count > 1)
                                    output.WriteLine($"{parent}:");
                                fileSystem.List(parent, longListing, all, human, recursive, reverse, name, dirStyle, color);
                            }
                            else
                            {
                                string dirPart = Path.GetDirectoryName(arg);
                                string namePart = Path.GetFileName(arg);
                                if (string.IsNullOrEmpty(dirPart))
                                    dirPart = fileSystem.GetCurrent();

                                string fullDir;
                                try { fullDir = Path.GetFullPath(dirPart); }
                                catch { fullDir = fileSystem.GetCurrent(); }

                                if (paths.Count > 1)
                                    output.WriteLine($"{fullDir}:");
                                fileSystem.List(fullDir, longListing, all, human, recursive, reverse, namePart, dirStyle, color);
                            }
                        }
                        catch (Exception ex)
                        {
                            output.WriteLine($"ls: error processing '{arg}': {ex.Message}", ConsoleColor.Red);
                        }
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                output.WriteLine($"ls: unexpected error: {ex.Message}", ConsoleColor.Red);
                return;
            }
        }


        internal static void Pwd(string[] Args)
        {
            output.WriteLine(fileSystem.Pwd());
        }

        internal static void ChangeUserColor(string[] Args, int mode = 0)
        {
            string NameMethod = "user";
            switch (mode)
            {
                case 0:
                    NameMethod = "user";
                    break;
                case 1:
                    NameMethod = "host";
                    break;
                case 2:
                    NameMethod = "dir";
                    break;
                default:
                    log.Write(Logger.LogType.Error, $"setusercolor: Incorrect mode. Selected mode: {mode}");
                    output.WriteLine("setusercolor: Incorrect mode.", ConsoleColor.Red);
                    return;
            }

            if (Args.Length != 1)
            {
                log.Write(Logger.LogType.Error, $"set{NameMethod}color: Arguments error, usage: set{NameMethod}color <color>");
                output.WriteLine($"Usage: set{NameMethod}color <color>", ConsoleColor.Red);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(Args[0], true, out var color))
            {
                bool resultMethod = mode == 0 ? settings.SetUserColor(color) : mode == 1 ? settings.SetHostColor(color) : settings.SetDirColor(color);
                if (resultMethod)
                {
                    log.Write(Logger.LogType.Info, $"set{NameMethod}color: Text color changed to: {color}");
                    output.WriteLine($"set{NameMethod}color: Text color changed to: {color}", ConsoleColor.Green);
                }
                else
                {
                    log.Write(Logger.LogType.Error, $"set{NameMethod}color: Unknown error. Stack trace: MultiTerminal.Commands.ChangeUserColor -> MultiTerminal.Settings.SettingsManager.Set{string.Concat(NameMethod[0].ToString().ToUpper(), NameMethod.AsSpan(1))}Color");
                    output.WriteLine($"set{NameMethod}color: Failed to set color. Check log file. Try again.", ConsoleColor.Red);
                }
            }
            else
            {
                log.Write(Logger.LogType.Error, $"set{NameMethod}color: The specified color could not be recognized. Please try again or consult your documentation. Entered color: {Args[0]}");
                output.WriteLine($"set{NameMethod}color: The specified color could not be recognized. Please try again or consult your documentation.", ConsoleColor.Red);
            }
        }

        internal static void Exit(string[] Args)
        {
            if (Args.Length == 0)
                stop.Push(0);
            else
            {
                int value = 0;
                if (int.TryParse(Args[0], out value))
                    stop.Push(value);
                else
                {
                    output.WriteLine("exit: Incorrect exit code.", ConsoleColor.Red);
                    return;
                }
            }
        }

        internal static void Clear()
        {
            log.Write(Logger.LogType.Debug, $"clear: Console cleared");
            Console.Clear();
        }

        internal static void PasswordGenerator(string[] Args) => PassGenerator.Run(Args, log);

        internal static void PasswordManager(string[] Args)
        {
            string input;
            if (Args.Length == 0)
                input = "passmgr.exe passwords.pass cfg";
            else if (Args.Length == 1)
                input = "passmgr.exe " + DefaultArguments.SplitArgs(string.Join(" ", Args))[0];
            else if (Args.Length == 2)
                input = "passmgr.exe " + DefaultArguments.SplitArgs(string.Join(" ", Args))[0] + " " + DefaultArguments.SplitArgs(string.Join(" ", Args))[1];
            else
            {
                output.WriteLine("PassMgr: Use: passmgr [File] [Folder]", ConsoleColor.Red);
                return;
            }

            if (!Launch.LaunchProgram(input, out input))
            {
                log.Write(Logger.LogType.Error, "PassMgr: Unknown error");
                output.WriteLine("PassMgr: Unknown error, maybe file not found.", ConsoleColor.Red);
            }
        }
    }
}
