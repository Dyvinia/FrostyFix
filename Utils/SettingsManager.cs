using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

public abstract class SettingsManager<T> where T : SettingsManager<T>, new() {
    private static readonly string filePath = GetConfigPath($"config.json");

    public static T Instance { get; private set; }

    private static string GetConfigPath(string fileName) {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, Assembly.GetEntryAssembly().GetName().Name, fileName);
    }

    public static void Load() {
        if (File.Exists(filePath))
            Instance = JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
        else
            Instance = new T();
    }

    public static void Save() {
        string json = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, json);
    }

    public static void Reset() {
        T defaultInstance = new();
        foreach (PropertyInfo property in typeof(T).GetProperties().Where(p => p.CanWrite)) {
            property.SetValue(Instance, property.GetValue(defaultInstance, null), null);
        }
    }
}