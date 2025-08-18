using MultiTerminal.Plugin;
using MultiTerminal.Output;

namespace MultiTerminal.PluginsSystem
{
    #region CLASS | PluginLoader
    /// <summary>
    /// Загрузчик плагинов
    /// </summary>
    public class PluginLoader
    {
        private readonly string _pluginsRootPath;
        private readonly string _prefix;
        private readonly DefaultOutput _output;
        private readonly List<(IPlugin plugin, PluginLoadContext context)> _loadedPlugins = new List<(IPlugin, PluginLoadContext)>();

        #region METHOD-PluginLoader | PluginLoader
        /// <summary>
        /// Конструктор загрузчика плагинов
        /// </summary>
        /// <param name="pluginsRootPath">Корневая папка с плагинами</param>
        /// <param name="prefix">Префикс</param>
        public PluginLoader(string pluginsRootPath, string prefix = "MultiTerminal")
        {
            _pluginsRootPath = pluginsRootPath;
            _prefix = prefix;
            _output = new DefaultOutput();
        }
        #endregion

        #region METHOD-IEnumerable<IPlugin> | LoadedPlugins
        /// <summary>
        /// ПОлучает все загруженные плагины
        /// </summary>
        public IEnumerable<IPlugin> LoadedPlugins => _loadedPlugins.Select(p => p.plugin);
        #endregion

        #region METHOD-VOID | LoadPlugins
        /// <summary>
        /// Загрузка плагинов
        /// </summary>
        /// <param name="commandRegistrar">Объект, реализующий <see cref="ICommandRegistrar"/>, используемый для регистрации команд плагинов</param>
        public void LoadPlugins(ICommandRegistrar commandRegistrar)
        {
            if (!Directory.Exists(_pluginsRootPath))
            {
                Directory.CreateDirectory(_pluginsRootPath);
                _output.WriteLine($"[{_prefix}] Plugins folder not found! Folder created: {_pluginsRootPath}", ConsoleColor.Yellow, null);
                return;
            }

            foreach (var pluginDir in Directory.GetDirectories(_pluginsRootPath))
            {
                try
                {
                    string pluginName = Path.GetFileName(pluginDir);
                    var pluginDll = Directory.GetFiles(pluginDir, "*.dll")
                        .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals(pluginName, StringComparison.OrdinalIgnoreCase));

                    if (pluginDll == null)
                    {
                        _output.WriteLine($"[{_prefix}] The main plugin file was not found in the following folder: {pluginDir}", ConsoleColor.Red, null);
                        continue;
                    }
                    else
                        pluginDll = Path.GetFullPath(pluginDll);

                    var loadContext = new PluginLoadContext(pluginDll);
                    var assembly = loadContext.LoadFromAssemblyPath(pluginDll);

                    #if DEBUG
                    // Debug
                    
                    foreach (var type in assembly.GetTypes())
                    {
                        _output.WriteLine($"[{_prefix}] Type: {type.Name}", ConsoleColor.DarkCyan, null);
                        foreach (var iface in type.GetInterfaces())
                        {
                            _output.WriteLine($"[{_prefix}]   Interface: {iface.FullName}, Assembly: {iface.Assembly.FullName}", ConsoleColor.DarkCyan, null);
                        }
                    }
                    
                    // End Debug
                    #endif

                    var pluginType = assembly.GetTypes()
                        .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    if (pluginType == null)
                    {
                        _output.WriteLine($"[{_prefix}] No class implementing IPlugin found in assembly \"{pluginDll}\"", ConsoleColor.Red, null);
                        continue;
                    }

                    var pluginInstance = Activator.CreateInstance(pluginType) as IPlugin;
                    if (pluginInstance == null)
                    {
                        continue;
                    }
                    pluginInstance.OnEnable();
                    pluginInstance.RegisterCommands(commandRegistrar);

                    _loadedPlugins.Add((pluginInstance, loadContext));

                    _output.WriteLine($"[{_prefix}] Plugin loaded: {pluginInstance.Name}", ConsoleColor.Green, null);
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"[{_prefix}] Plugin error from {pluginDir}: {ex.Message}", ConsoleColor.White, ConsoleColor.Red);
                }
            }
        }
#endregion

        #region METHOD-VOID UnloadPlugins
        /// <summary>
        /// Выгрузка всех плагинов
        /// </summary>
        public void UnloadPlugins()
        {
            foreach (var (plugin, context) in _loadedPlugins)
            {
                try
                {
                    plugin.OnDisable();
                    context.Unload();
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"[{_prefix}] Error unloading plugin {plugin.Name}: {ex.Message}", ConsoleColor.White, ConsoleColor.Red);
                }
            }
            _loadedPlugins.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion
    }
#endregion
}
