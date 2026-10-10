using LaParola.Models;
using System;
using System.IO;
using System.Text.Json;

namespace LaParola.Services
{
    public class SettingsService
    {
        private const string SettingsFileName = "LaParola.ImpostazioniApp.json";
        private static string? _cachedSettingsPath;

        /// <summary>
        /// Gets the resolved settings file path. Evaluates environment once and caches result.
        /// </summary>
        public static string SettingsPath => _cachedSettingsPath ??= DetermineSettingsPath();

        private readonly JsonSerializerOptions options = new() { WriteIndented = true };

        public AppSettings Load()
        {
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
            string json = JsonSerializer.Serialize(settings, options);

            try
            {
                string? dir = Path.GetDirectoryName(SettingsPath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Fallback to AppData if saving fails, updating the cache
                _cachedSettingsPath = GetAppDataPath();
                Directory.CreateDirectory(Path.GetDirectoryName(_cachedSettingsPath)!);
                File.WriteAllText(_cachedSettingsPath, json);
            }
        }

        private static string DetermineSettingsPath()
        {
            string appFolder = AppContext.BaseDirectory;

            // Is AppContext.BaseDirectory writable? for example, portable application in a user-writable folder
            if (IsDirectoryWritable(appFolder))
            {
                return Path.Combine(appFolder, SettingsFileName);
            }

            // Default fallback (e.g. Program Files or read-only directory)
            return GetAppDataPath();
        }

        public static string GetAppDataPath()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaParola");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, SettingsFileName);
        }

        private static bool IsDirectoryWritable(string dirPath)
        {
            try
            {
                string testFile = Path.Combine(dirPath, $".write_test_{Guid.NewGuid():N}");
                using (File.Create(testFile, 1, FileOptions.DeleteOnClose)) { }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}