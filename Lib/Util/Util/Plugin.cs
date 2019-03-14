using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using DeviceInfo = Plugin.DeviceInfo.CrossDeviceInfo;

namespace Qoden.Util
{
    public class Plugin 
    {
        public static Func<string, string, object> PluginProvider;

        private static ConcurrentDictionary<string, object> _plugins = new ConcurrentDictionary<string, object>();

        public static void AddPlugin(string key, object plugin)
        {
            _plugins.AddOrUpdate(key, plugin, (_1, _2) => plugin);
        }

        public static T Load<T>(string prefix, string name)
        {
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentException(nameof(prefix));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));

            object plugin;
            if (_plugins.TryGetValue($"{prefix}.{name}", out plugin))
            {
                return (T)plugin;
            }

            var deviceInfo = DeviceInfo.Current;
            var assemblyName = new AssemblyName($"{prefix}.{deviceInfo.Platform}");
            var assembly = Assembly.Load(assemblyName);
            var t = assembly.GetType($"{prefix}.{name}", true, true);
            return (T)Activator.CreateInstance(t);
        }
    }
}
