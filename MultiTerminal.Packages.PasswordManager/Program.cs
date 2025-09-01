using MultiTerminal.Output;
using MultiTerminal.Input;
using MultiTerminal.Logger;
using MultiTerminal.PasswordManager;
using MultiTerminal.Arguments;
using IniParser;

namespace MultiTerminal.Packages.PasswordManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var output = new DefaultOutput();
            var input = new DefaultInput();

            var log = new Log("passmgr.log", "logs");

            if (args.Length <= 0)
            {
                log.Write(LogType.Error, "PassMgr: Args error: Null args");
                output.WriteLine("PassMgr: Use: passmgr (File) [Folder]", ConsoleColor.Red);
                return;
            }

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
            string[] Args = DefaultArguments.SplitArgs(string.Join(" ", args));
            PassManager pmgr;

            string filePath;

            if (Args.Length == 1)
            {
                pmgr = new(Args[0], log);
                filePath = Args[0];
            }
            else if (Args.Length == 2)
            {
                pmgr = new(Args[0], Args[1], log);
                filePath = Args[1] + "\\" + Args[0];
            }
            else
            {
                log.Write(LogType.Error, "PassMgr: Args error: " + string.Join(" ", Args));
                output.WriteLine("PassMgr: Use: passmgr (File) [Folder]", ConsoleColor.Red);
                return;
            }

            if (!File.Exists(filePath))
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
                            pmgr.Initialize(masterPassword);
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
                        if (pmgr.TryLoad(masterPassword))
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

                var entries = pmgr.GetAllEntries() ?? new List<PasswordEntry>();

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
                        pmgr.Close();
                    }
                    catch { }
                    WriteLine("Completed. Data in memory cleared.");
                    break;
                }
                else if (cmd == "r" || cmd == "refresh")
                {
                    try
                    {
                        if (pmgr.TryLoad(masterPassword))
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
                        pmgr.AddEntry(entry);
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
                        pmgr.UpdateEntry(old, updated);
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
                            pmgr.RemoveEntry(e);
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
    }
}
