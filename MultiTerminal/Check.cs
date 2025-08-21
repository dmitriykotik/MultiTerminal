using MultiAPI;
using MultiTerminal.Logger;
using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class Check
    {
        internal static bool supportOs()
        {
            log.Write(LogType.Debug, "Executing the \"supportOs\" method...");

            log.Write(LogType.Debug, "Device info: Real OS: " + GetFromOS(OS.Get()) + "; SMBIOS OS: " + GetFromOS(settingsFile.SMBIOS.OperatingSystem));
            if (!compatibility.CompatibilityMode && !settingsFile.disableWarningCompatibility)
            {
                log.Write(LogType.Warning, "Current operating system is not supported!");
                output.WriteLine(@"[WARNING] Your operating system is not supported by the terminal. Continuing to run may cause the terminal to malfunction. You can disable this warning in the terminal settings. 
Are you sure you want to continue running? (y/n)", ConsoleColor.Yellow, null);
                string? result = input.ReadLine();
                if (!string.IsNullOrEmpty(result))
                {
                    if (result.ToLower() == "y")
                        return true;
                }
                return false;
            }
            return true;
        }

        internal static void compatibilityWarning()
        {
            log.Write(LogType.Debug, "Executing the \"compatibilityWarning\" method...");
            if (!compatibility.CompatibilityMode && !settingsFile.disableWarningCompatibility)
                output.Write("[WARNING] The terminal is running in compatibility mode. Some features may not be available or may not work properly. Install the terminal on a supported device. If this is an error, edit the SMBIOS settings in the terminal to emulate a supported device.\n", ConsoleColor.Yellow, null);
        }

        internal static bool CheckHashFiles()
        {
            log.Write(LogType.Debug, "Checking hash files...");

            if (settingsFile.SkipHashCheck)
            {
                log.Write(LogType.Debug, "Pass...");
                return true;
            }

            #if DEBUG
            return true;
            #endif

            bool allConfirmed = true;

            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory))
            {
                string extension = Path.GetExtension(file);
                List<string> ignoreExt = new() { ".pdb", ".xml" };
                if (ignoreExt.Contains(extension))
                    continue;

                string fileName = Path.GetFileName(file);
                var matchingHash = hashes.Hashes.FirstOrDefault(h => Path.GetFileName(h.FileName) == fileName);

                if (matchingHash != null)
                {
                    string currentHash = FileManager.File.GetHash(file, FileManager.File.HashType.SHA256);
                    if (currentHash == matchingHash.FileHash)
                    {
                        output.WriteLine(fileName + " - Confirmed", ConsoleColor.Green, null);
                        log.Write(LogType.Info, $"File: {fileName}; Hash: {matchingHash.FileHash} - Confirmed");
                    }
                    else
                    {
                        allConfirmed = false;
                        output.WriteLine(fileName + " - Not confirmed", ConsoleColor.Red, null);
                        log.Write(LogType.Error, $"File: {fileName}; Hash: {currentHash} - Not confirmed! Hash error.");
                    }
                }
                else
                {
                    allConfirmed = false;
                    string currentHash = FileManager.File.GetHash(file, FileManager.File.HashType.SHA256);
                    output.WriteLine(fileName + " - Not confirmed", ConsoleColor.Red, null);
                    log.Write(LogType.Error, $"File: {fileName}; Hash: {currentHash} - Not confirmed! File not contained in hash file.");
                }
            }

            foreach (var hashEntry in hashes.Hashes)
            {
                string fileName = Path.GetFileName(hashEntry.FileName);
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

                if (!File.Exists(filePath))
                {
                    allConfirmed = false;
                    output.WriteLine(fileName + " - Not confirmed (missing)", ConsoleColor.Red, null);
                    log.Write(LogType.Error, $"File: {fileName} - Not confirmed! File missing in directory.");
                }
            }

            if (!allConfirmed)
            {
                log.Write(LogType.Warning, "Warning! One or more files have been modified or not found. Check the logs to find out the reason. If you downloaded mods for the terminal, make sure the creator is honest and there are no viruses. You can continue working and update the hash list. Please note that the mods you download may contain malware.");
                output.WriteLine("Warning! One or more files have been modified or not found. Check the logs to find out the reason. If you downloaded mods for the terminal, make sure the creator is honest and there are no viruses. You can continue working and update the hash list. Please note that the mods you download may contain malware. Do you want to continue working? (y/n)", ConsoleColor.Yellow, null);
                var result = input.ReadLine();
                if (result != null && result.ToLower() == "y")
                {
                    log.Write(LogType.Info, "Regenerating hash file...");
                    output.WriteLine("Regenerating hash file...", ConsoleColor.Yellow, null);
                    hashesManager.GenAndSave();
                    log.Write(LogType.Info, "Done!");
                    output.WriteLine("Done!", ConsoleColor.Green, null);
                    return true;
                }
                return false;
            }

            return true;
        }

        private static string GetFromOS(OS.OSTypes os)
        {
            switch (os)
            {
                case OS.OSTypes.Windows:
                    return "Windows (0)";
                case OS.OSTypes.Linux:
                    return "Linux (1)";
                case OS.OSTypes.MacOS:
                    return "MacOS (2)";
                case OS.OSTypes.Android:
                    return "Android (3)";
                case OS.OSTypes.Browser:
                    return "Browser (4)";
                case OS.OSTypes.FreeBSD:
                    return "FreeBSD (5)";
                case OS.OSTypes.IOS:
                    return "IOS (6)";
                case OS.OSTypes.TvOS:
                    return "TvOS (7)";
                case OS.OSTypes.WatchOS:
                    return "WatchOS (8)";
                case OS.OSTypes.Other:
                    return "Other (9)";
                default:
                    return "Other (null)";
            }
        }
    }
}
