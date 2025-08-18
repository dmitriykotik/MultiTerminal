using MultiAPI;
using MultiTerminal.Logger;
using MultiTerminal.Output;
using MultiTerminal.Input;
using MultiTerminal.ExitFunc;
using MultiTerminal.Settings;
using MultiTerminal.PluginsSystem;

namespace MultiTerminal
{
    #region CLASS | InternalVars
    internal class InternalVars
    {
        #region Settings
        internal static SettingsManager settings = new SettingsManager("terminal.cfg", "cfg");
        internal static SettingsFile settingsFile = settings.Get();
        #endregion

        #region Log Var
        internal static Log log = new Log("mt.log", "logs");
        #endregion

        #region In/Out
        internal static DefaultInput input = new DefaultInput();
        internal static DefaultOutput output = new DefaultOutput();
        #endregion

        #region Compatibility
        internal static OS.OSTypes[] Supported = { OS.OSTypes.Windows, OS.OSTypes.Linux };

        internal static CompatibilityManager cmgr = new CompatibilityManager(Supported, settingsFile.SMBIOS);

        internal static Compatibility compatibility = cmgr.Get();
        #endregion

        #region Exit Terminal
        internal static readonly List<ExitCode> exitCodes = new List<ExitCode>
        {
            new ExitCode { ErrorCode = 0, Information = "The shutdown was performed by the user", CriticalError = false },
            new ExitCode { ErrorCode = 1, Information = "The current operating system is not supported. The exit was performed by the user after a warning", CriticalError = false },
            new ExitCode { ErrorCode = 2, Information = "One or more files were not verified by hash verification. The user exited after a warning.", CriticalError = false }
        };

        internal static Stop stop = new Stop(exitCodes, log);
        #endregion

        #region Command Manager
        internal static CommandManager commands = new();
        internal static PluginLoader plugins = new("plugins");
        #endregion

        #region Hashes
        internal static Hashes hashesManager = new("all.hash", "cfg");
        internal static HashFile hashes = hashesManager.Get();
        #endregion
    }
    #endregion
}