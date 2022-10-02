using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DyviniaUtils {
    public abstract class SettingsManager<T> where T : SettingsManager<T>, new() {
        public static T Settings { get; private set; }

        public static string FilePath {
            get {
                if (typeof(T).GetCustomAttribute<GlobalConfigAttribute>() != null) {
                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Assembly.GetEntryAssembly().GetName().Name,
                        "config.json");
                }
                else {
                    return Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "config.json");
                }
            }
        }

        public static void Load() {
            try {
                Settings = JsonSerializer.Deserialize<T>(File.ReadAllText(FilePath));
            }
            catch {
                Settings = new T();
            }
        }

        public static void Save() {
            string json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, json);
        }

        public static void Reset() {
            T defaultSettings = new();
            foreach (PropertyInfo property in typeof(T).GetProperties().Where(p => p.CanWrite)) {
                property.SetValue(Settings, property.GetValue(defaultSettings, null), null);
            }
        }
    }

    /// <summary>
    /// Saves Config file to AppData
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GlobalConfigAttribute : Attribute { }
}