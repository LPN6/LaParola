using System.Windows;

namespace LaParola.Models;

public class ViewerWindowState
{
    public string ContentId { get; set; } = "";     // es: "viewer:abcd..."
    public string DisplayName { get; set; } = "";   // placeholder (nome testo)
    public string VerseRef { get; set; } = "";      // placeholder (riferimento)
}

public enum ThemeState
{
    Light, Dark, System
}

public class AppSettings
{
    public ThemeState ThemeMode { get; set; } = ThemeState.System;
    public string Language { get; set; } = "";

    // Layout AvalonDock serializzato(XML come stringa)
    public string? DockLayoutXml { get; set; }

    // Stato “viewer” da ricreare (placeholder)
    public List<ViewerWindowState> ViewerWindows { get; set; } = [];

    public double? WindowTop { get; set; }
    public double? WindowLeft { get; set; }
    public double? WindowWidth { get; set; }
    public double? WindowHeight { get; set; }
    public WindowState? WindowState { get; set; }

    public FormatoTesto Formato { get; set; } = new FormatoTesto();

    public string RicercaTestoSelezionato { get; set; } = "";

    public List<string> MostraVersioniTutte { get; set; } = [];
    public List<string> MostraVersioniSelezionate { get; set; } = [];
    public bool MostraAlternare { get; set; } = false;
}
