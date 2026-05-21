using System;
using System.IO;
using System.Text.Json;
using LaParola.Models;

namespace LaParola.Services;

public class SettingsService
{
    private const string SettingsFileName = "LaParola.ImpostazioniApp.json";

    public string SettingsPath { get; private set; } = "";
    private readonly JsonSerializerOptions options = new() { WriteIndented = true };

    public AppSettings Load()
    {
        SettingsPath = ResolveSettingsPath();
        try
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    return settings;
                }
            }
        }
        catch { }

        AppSettings fresh = new();
        Save(fresh);
        return fresh;
    }

    public void Save(AppSettings settings)
    {
        SettingsPath = ResolveSettingsPath();
        string json = JsonSerializer.Serialize(settings, options);

        try
        {
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            SettingsPath = GetAppDataPath();
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, json);
        }
    }

    private static string ResolveSettingsPath()
    {
        try
        {
            string appFolder = AppContext.BaseDirectory;
            string candidate = Path.Combine(appFolder, SettingsFileName);
            using (FileStream fs = new(candidate, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) { }
            return candidate;
        }
        catch
        {
            return GetAppDataPath();
        }
    }

    private static string GetAppDataPath()
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaParola");
        return Path.Combine(dir, SettingsFileName);
    }
}
