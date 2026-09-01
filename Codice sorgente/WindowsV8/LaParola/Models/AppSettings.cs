using LaParola.ToolViews;
using System.Windows;
using System.Windows.Media;

namespace LaParola.Models;

public class ViewerWindowState
{
    public string ContentId { get; set; } = "";     // es: "viewer:abcd..."
    public string Versione { get; set; } = "";   // nome testo
    public bool VersettoMostrato { get; set; } = true;
    public byte Libro { get; set; } = 1;
    public byte Capitolo { get; set; } = 1;
    public byte Versetto { get; set; } = 1;
    public string Titolo { get; set; } = "";
    public bool IsSommarioVisibile { get; set; } = false;
    public int SincGruppo { get; set; } = 0;
    public int Zoom { get; set; } = 100;
}

public enum ThemeState
{
    Light, Dark, System
}

public class AppSettings
{
    public ThemeState ThemeMode { get; set; } = ThemeState.System;
    public string Lingua { get; set; } = "";

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

    public string VoceSintesiVocale { get; set; } = "";
    public bool VoceDelTesto { get; set; } = true;
    public double VelocitaVoce { get; set; } = 1.0;
    public int VolumeVoce { get; set; } = 100;
    public bool VoceEvidenzia { get; set; } = false;
    public Color VoceEvidenziaColore { get; set; } = Colors.Red;

    public string UltimaBibbia { get; set; } = "";
    public string UltimaBibbiaCompleta { get; set; } = "";
    public string UltimaCartellaImmagini { get; set; } = "";
    public string UltimaCartellaImportare { get; set; } = "";
    public string UltimaCartellaImportarePDF { get; set; } = "";
    public string UltimaCartellaImportareRtf { get; set; } = "";

    public bool EditorChiudere { get; set; } = false;
    public bool IpertestoTooltip { get; set; } = true;
    public bool IpertestoDizionario { get; set; } = true;
    public int ControlloMessaggi { get; set; } = 1;
    public DateTime UltimoControlloMessaggi { get; set; } = DateTime.MinValue;
    public int UltimoMessaggioControllatoId { get; set; } = 0;
    public string BibbiaPreferita1 { get; set; } = "";
    public string BibbiaPreferita2 { get; set; } = "";
    public string BibbiaPreferita3 { get; set; } = "";

    public string DizionarioInglese { get; set; } = "";
    public string DizionarioItaliano { get; set; } = "";
    public string DizionarioGreco { get; set; } = "";
    public string DizionarioEbraico { get; set; } = "";
    public string DizionarioLatino { get; set; } = "";

    public LibraryToolState LibraryState { get; set; } = new LibraryToolState();

    public string RicercaTestoSelezionato { get; set; } = "";

    public List<string> MostraVersioniTutte { get; set; } = [];
    public List<string> MostraVersioniSelezionate { get; set; } = [];
    public bool MostraAlternare { get; set; } = false;

    public string CreaChiaveVersioneSelezionata { get; set; } = "";
    public int CreaChiaveNumeroMinimo { get; set; } = 1;
    public bool CreaChiaveConRiferimenti { get; set; } = true;
    public TipoChiave CreaChiaveTipo { get; set; } = TipoChiave.Parole;
    public OrdineChiave CreaChiaveOrdine { get; set; } = OrdineChiave.Alfabetico;
    public bool CreaChiaveEscludiParole { get; set; } = true;
    public string CreaChiaveParoleDaEscludere { get; set; } = "*";

    // Reference search
    public bool ReferenceSearchTocVisible { get; set; } = true;

    public HashSet<string> TestiNascosti { get; set; } = [];
}
