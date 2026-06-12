using System.Windows;

namespace LaParola.Models;

public class ViewerWindowState
{
    public string ContentId { get; set; } = "";     // es: "viewer:abcd..."
    public string Versione { get; set; } = "";   // nome testo
    public byte Libro { get; set; } = 1;
    public byte Capitolo { get; set; } = 1;
    public byte Versetto { get; set; } = 1;
    public bool IsSommarioVisibile { get; set; } = false;
    public int SincGruppo { get; set; } = 0;
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

    public int ControlloMessaggi { get; set; } = 1;
    public DateTime UltimoControlloMessaggi { get; set; } = DateTime.MinValue;
    public int UltimoMessaggioControllatoId { get; set; } = 0;

    public string RicercaTestoSelezionato { get; set; } = "";

    public List<string> MostraVersioniTutte { get; set; } = [];
    public List<string> MostraVersioniSelezionate { get; set; } = [];
    public bool MostraAlternare { get; set; } = false;
}
