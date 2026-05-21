using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace LaParola.Services;

public class LocalizationManager
{
    public static void ApplyLanguage(string languageCode)
    {
        string lang = (languageCode ?? "en").Trim().ToLowerInvariant();
        Uri dictUri = new($"Resources/Strings.{(lang == "it" ? "it" : "en")}.xaml", UriKind.Relative);
        ResourceDictionary newDict = new() { Source = dictUri };

        Collection<ResourceDictionary> merged = Application.Current.Resources.MergedDictionaries;
        ReplaceDictionary(merged, "Resources/Strings.en.xaml", "Resources/Strings.it.xaml", newDict);
    }

    internal static void ReplaceDictionary(Collection<ResourceDictionary> merged, string matchA, string matchB, ResourceDictionary newDict)
    {
        int idx = -1;
        for (int i = 0; i < merged.Count; i++)
        {
            string src = merged[i].Source?.ToString() ?? "";
            if (src.Contains(matchA, StringComparison.OrdinalIgnoreCase) || src.Contains(matchB, StringComparison.OrdinalIgnoreCase))
            {
                idx = i;
                break;
            }
        }
        if (idx >= 0)
        {
            merged[idx] = newDict;
        }
        else
        {
            merged.Add(newDict);
        }
    }
}
