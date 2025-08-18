using System.Reflection;
using System.Runtime.Loader;

#pragma warning disable CS8600

namespace MultiTerminal.PluginsSystem
{
    #region CLASS | PluginLoadContext
    /// <summary>
    /// Загрузчик плагинов (Контекст)
    /// </summary>
    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        #region METHOD-PluginLoadContext | PluginLoadContext
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="PluginLoadContext"/> для загрузки плагина из указанного пути.
        /// </summary>
        /// <param name="pluginPath">Путь к корневой папке с плагинами</param>
        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }
        #endregion

        #region PROTECTED~METHOD-ASSEMBLY | Load
        /// <summary>
        /// Загрузка управляемой сборки
        /// </summary>
        /// <param name="assemblyName">Имя загружаемой сборки</param>
        /// <returns>Загруженная сборка</returns>
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == "MultiTerminal.Plugin")
                return Assembly.Load(assemblyName);

            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            return null;
        }
        #endregion

        #region PROTECTED~METHOD-IntPtr | LoadUnmanagedDll
        /// <summary>
        /// Загрузка неуправляемой сборки
        /// </summary>
        /// <param name="unmanagedDllName">Указатель на незагруженную библиотеку</param>
        /// <returns>Указатель на загруженное библиотеку</returns>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }
            return IntPtr.Zero;
        }
        #endregion
    }
    #endregion
}
