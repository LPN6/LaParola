using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Themes;
using LaParola.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using LaParola.Utilities;
using System.Windows.Documents;
using LaParola.DocumentViews;
using LaParola.ToolViews;

namespace LaParola.Services;

public class ThemeManager
{
    private bool _hooked;
    private UserPreferenceChangedEventHandler? _handler;

    public static void ApplyTheme(ThemeState themeMode)
    {
        bool isDark = IsDark(themeMode);
        Uri dictUri = new($"Themes/{(isDark ? "Dark" : "Light")}.xaml", UriKind.Relative);
        ResourceDictionary newDict = new() { Source = dictUri };
        Collection<ResourceDictionary> merged = Application.Current.Resources.MergedDictionaries;
        LocalizationManager.ReplaceDictionary(merged, "Themes/Light.xaml", "Themes/Dark.xaml", newDict);
        SetDarkTitleBar(isDark);
    }

    public static void ApplyDockTheme(DockingManager dock, ThemeState themeMode)
    {
        bool isDark = IsDark(themeMode);
        dock.Theme = isDark ? new ArcDarkTheme() : new ArcLightTheme();

        LayoutRoot? root = dock.Layout;
        if (root == null)
        {
            return;
        }

        List<LayoutDocument> docs = [.. root.Descendents()
                       .OfType<LayoutDocument>()
                       .Where(d => (d.ContentId ?? "").StartsWith("doc.editor."))];

        Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
        foreach (LayoutDocument doc in docs)
        {
            if (doc.Content is EditorDocumentView fd)
            {
                bool oldDirty = fd.IsDirty;
                string oldTitle = fd.ParentDocument?.Title ?? "LPNqwe3141#";
                RtfColorTransformer.ApplyThemeToDocument(fd.FlowDocument, isDark, fg);
                if (fd.ParentDocument != null && oldTitle != "LPNqwe3141#")
                {
                    fd.ParentDocument.Title = oldTitle;
                }
                fd.IsDirty = oldDirty;
            }
        }

        LayoutAnchorable? anchorable = dock.Layout?
            .Descendents()
            .OfType<LayoutAnchorable>()
            .FirstOrDefault(a => a.ContentId == "tool.options");

        if (anchorable != null)
        {
            if (anchorable.Content is OptionsToolView toolView)
            {
                toolView.UpdateFontColor(isDark);
            }
        }
    }

    public void HookSystemThemeChanges(DockingManager dock, ThemeState themeMode)
    {
        UnhookSystemThemeChanges();
        if (themeMode != ThemeState.System)
        {
            return;
        }

        _handler = (_, __) =>
        {
            AppSettings settings = App.Settings.Load();
            ApplyTheme(settings.ThemeMode);
            ApplyDockTheme(dock, settings.ThemeMode);
        };

        SystemEvents.UserPreferenceChanged += _handler;
        _hooked = true;
    }

    public void UnhookSystemThemeChanges()
    {
        if (_hooked && _handler != null)
        {
            SystemEvents.UserPreferenceChanged -= _handler;
            _hooked = false;
            _handler = null;
        }
    }

    public static bool IsDark(ThemeState themeMode)
    {
        return ((themeMode == ThemeState.Dark) ||
               (themeMode == ThemeState.System && IsSystemInDarkMode()));
    }

    internal static bool IsSystemInDarkMode()
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key?.GetValue("AppsUseLightTheme") is int light)
            {
                return light == 0;
            }
        }
        catch { }
        return false;
    }

    public static void SetDarkTitleBar(Window? win, bool enabled)
    {
        IntPtr hwnd = IntPtr.Zero;
        if (win is not null)
            hwnd = new WindowInteropHelper(win).Handle;
        if (hwnd == IntPtr.Zero) return;

        // DWMWA_USE_IMMERSIVE_DARK_MODE = 20 (newer Win10/Win11),
        // some builds use 19; many implementations try both.
        int useDark = enabled ? 1 : 0;

        _ = DwmSetWindowAttribute(hwnd, 20, ref useDark, sizeof(int));
        // optional compatibility fallback:
        _ = DwmSetWindowAttribute(hwnd, 19, ref useDark, sizeof(int));
    }

    public static void SetDarkTitleBar(bool enabled)
    {
        SetDarkTitleBar(Application.Current?.MainWindow, enabled);
    }

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

}
