using AvalonDock.Layout;
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

    public static void RefreshToolTitles(LayoutRoot layout)
    {
        if (layout == null) return;

        // 1. Refresh side-pane tools (Anchorables)
        foreach (LayoutAnchorable anchorable in layout.Descendents().OfType<LayoutAnchorable>())
        {
            switch (anchorable.ContentId)
            {
                case "tool.search":
                    anchorable.Title = (string)(Application.Current.TryFindResource("RicercaTitolo") ?? "Search");
                    break;
                case "tool.textgen":
                    anchorable.Title = (string)(Application.Current.TryFindResource("MostraTitolo") ?? "Show Passage");
                    break;
                case "tool.creachiave":
                    anchorable.Title = (string)(Application.Current.TryFindResource("CreaChiaveTitolo") ?? "Create Concordance");
                    break;
                case "tool.converter":
                    anchorable.Title = (string)(Application.Current.TryFindResource("MisureTitolo") ?? "Measures Converter");
                    break;
            }
        }

        // 2. Refresh main-workspace tools (Documents)
        foreach (LayoutDocument doc in layout.Descendents().OfType<LayoutDocument>())
        {
            if (doc.ContentId == "tool.options")
            {
                doc.Title = (string)(Application.Current.TryFindResource("OpzioniTitolo") ?? "Options");
            }
            // Viewers and Editor windows are completely ignored here because 
            // their ContentIds won't match "tool.options"
        }
    }
}
