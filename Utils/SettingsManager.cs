using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

public abstract class SettingsManager<T> where T : SettingsManager<T>, new() {
    public static T Instance { get; private set; }

    public static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Assembly.GetEntryAssembly().GetName().Name,
        $"config.json"
        );

    public static void Load() {
        try {
            Instance = JsonSerializer.Deserialize<T>(File.ReadAllText(ConfigPath));
        }
        catch {
            Instance = new T();
        }
    }

    public static void Save() {
        string json = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
        File.WriteAllText(ConfigPath, json);
    }

    public static void Reset() {
        T defaultInstance = new();
        foreach (PropertyInfo property in typeof(T).GetProperties().Where(p => p.CanWrite)) {
            property.SetValue(Instance, property.GetValue(defaultInstance, null), null);
        }
    }
}