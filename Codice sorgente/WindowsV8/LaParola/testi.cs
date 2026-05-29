using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using static LaParola.Utilities.Funzioni;

namespace LaParola
{
    #region Confronto

    /// <summary>
    /// Una classe per confrontare due stringhe, che funziona anche con i caratteri greci.
    /// Case insensitive.
    /// </summary>
    public class ConfrontoCI : IComparer<String>
    {
        /// <summary>
        /// La funzione Compare.
        /// </summary>
        /// <param name="x">La prima stringa.</param>
        /// <param name="y">La seconda stringa.</param>
        /// <returns>Il confronto delle stringhe: -1, 0 o 1.</returns>
        public int Compare(string? x, string? y)
        {
            ArgumentNullException.ThrowIfNull(x);

            ArgumentNullException.ThrowIfNull(y);

            return String.Compare(x.Normalize(NormalizationForm.FormD), y.Normalize(NormalizationForm.FormD), StringComparison.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Una classe per confrontare due stringhe, che funziona anche con i caratteri greci.
    /// Case sensitive.
    /// </summary>
    public class ConfrontoCS : IComparer<String>
    {
        /// <summary>
        /// La funzione Compare.
        /// </summary>
        /// <param name="x">La prima stringa.</param>
        /// <param name="y">La seconda stringa.</param>
        /// <returns>Il confronto delle stringhe: -1, 0 o 1.</returns>
        public int Compare(string? x, string? y)
        {
            ArgumentNullException.ThrowIfNull(x);

            ArgumentNullException.ThrowIfNull(y);

            return String.Compare(x.Normalize(NormalizationForm.FormD), y.Normalize(NormalizationForm.FormD), StringComparison.InvariantCulture);
        }
    }

    #endregion

    #region BloccatoTipi

    /// <summary>
    /// Il modo in cui una collezione è bloccata.
    /// </summary>
    public enum BloccatoTipi
    {
        /// <summary>
        /// La collezione non è bloccata.
        /// </summary>
        Sbloccato,
        /// <summary>
        /// La collezione è bloccata, ma può essere sbloccata.
        /// </summary>
        Bloccato,
        /// <summary>
        /// La collezione è bloccata, ma non può essere sbloccata dall'utente.
        /// </summary>
        BloccatoSempre
    }

    #endregion

    #region TestoTipi

    /// <remarks>
    /// Il tipo di un certo testo.
    /// </remarks>
    public enum TestoTipi
    {
        /// <summary>
        /// Il tipo non è stato impostato.
        /// </summary>
        None,
        /// <summary>
        /// Una versione della Bibbia (o una parte).
        /// </summary>
        Bibbia,
        /// <summary>
        /// Un commentario, cioè delle note collegate a versetti o brani.
        /// </summary>
        Commentario,
        /// <summary>
        /// Un dizionario, cioè delle note collegate a temi.
        /// </summary>
        Dizionario = 4,
        /// <summary>
        /// Un libro, cioè note che hanno un ordine.
        /// </summary>
        Libro = 8
    };

    #endregion

    #region OccorrenzaParola

    /// <remarks>
    /// Per la concordanza, dà il numero del versetto o della nota e il numero della parola nella voce
    /// </remarks>
    public struct OccorrenzaParola : IComparable
    {
        private uint voce;
        /// <summary>
        /// Il numero della voce (versetto o nota) nel testo.
        /// </summary>
        public uint Voce
        {
            readonly get => voce; set => voce = value;
        }

        private ushort parola;
        /// <summary>
        /// Il numero della parola nel testo.
        /// </summary>
        public ushort Parola
        {
            readonly get => parola; set => parola = value;
        }

        /// <summary>
        /// Confronta un altro oggetto di tipo OccorrenzaParola con quello attuale.
        /// </summary>
        /// <param name="obj">L'altro oggetto di tipo OccorrenzaParola da confrontare.</param>
        /// <returns>-1 se questa parola è prima dell'altro, 0 se è uguale, 1 se è dopo.</returns>
        public readonly int CompareTo(object? obj)
        {
            if (obj != null && obj.GetType().Name == "OccorrenzaParola")
            {
                OccorrenzaParola op = (OccorrenzaParola)obj;
                if (this.voce < op.voce)
                {
                    return -1;
                }
                else if (this.voce > op.voce)
                {
                    return 1;
                }
                else
                {
                    if (this.Parola < op.Parola)
                    {
                        return -1;
                    }
                    else if (this.Parola > op.Parola)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Object not of type OccorrenzaParola", nameof(obj));
            }
        }

        #region Diversi override

        /// <summary>
        /// Restituisce se due oggetti sono uguali.
        /// </summary>
        /// <param name="obj">Un oggetto di tipo OccorrenzaParola a cui paragonare questo oggetto.</param>
        /// <returns>True se le occorrenze sono uguali.</returns>
        public override readonly bool Equals(object? obj)
        {
            if ((obj is null) || (obj is not OccorrenzaParola))
            {
                return false;
            }

            return (this.CompareTo(obj) == 0);
        }

        /// <summary>
        /// Calcola il hash code.
        /// </summary>
        /// <returns>Il hash code.</returns>
        public override readonly int GetHashCode()
        {
            return (int)(voce / 2) ^ parola;
        }

        /// <summary>
        /// Se due oggetti sono uguali.
        /// </summary>
        /// <param name="primoOggetto">Primo oggetto.</param>
        /// <param name="secondoOggetto">Secondo oggetto.</param>
        /// <returns>True se sono uguali.</returns>
        public static bool operator ==(OccorrenzaParola? primoOggetto, OccorrenzaParola? secondoOggetto)
        {
            if (primoOggetto is null)
            {
                return (secondoOggetto is null);
            }

            return primoOggetto.Equals(secondoOggetto);
        }

        /// <summary>
        /// Se due oggetti sono diversi.
        /// </summary>
        /// <param name="primoOggetto">Primo oggetto.</param>
        /// <param name="secondoOggetto">Secondo oggetto.</param>
        /// <returns>True se sono diversi.</returns>
        public static bool operator !=(OccorrenzaParola? primoOggetto, OccorrenzaParola? secondoOggetto)
        {
            return !(primoOggetto == secondoOggetto);
        }

        /// <summary>
        /// Se una parola appare prima di un'altra.
        /// </summary>
        /// <param name="primoOggetto">Prima parola.</param>
        /// <param name="secondoOggetto">Seconda parola.</param>
        /// <returns>True se la prima appare prima della seconda.</returns>
        public static bool operator <(OccorrenzaParola primoOggetto, OccorrenzaParola secondoOggetto)
        {
            return primoOggetto.CompareTo(secondoOggetto) < 0;
        }

        /// <summary>
        /// Se una parola appare dopo un'altra.
        /// </summary>
        /// <param name="primoOggetto">Prima parola.</param>
        /// <param name="secondoOggetto">Seconda parola.</param>
        /// <returns>True se la prima appare dopo la seconda.</returns>
        public static bool operator >(OccorrenzaParola primoOggetto, OccorrenzaParola secondoOggetto)
        {
            return primoOggetto.CompareTo(secondoOggetto) > 0;
        }

        #endregion
    }

    #endregion

    #region VersioneInfo

    /// <remarks>
    /// Informazione su un testo che è in un file dei dati, che può essere una versione della Bibbia
    /// oppure un commentario e/o un un dizionario e/o un libro.
    /// </remarks>
    public class VersioneInformazioni
    {
        private string versione;
        /// <summary>
        /// Il numero della versione del file (nel formato 7.13.11)
        /// </summary>
        public string Versione
        {
            get => versione; set => versione = value;
        }

        private string nomeDelFile;
        /// <summary>
        /// Il nome e percorso del file che contiene il testo.
        /// </summary>
        public string NomeDelFile
        {
            get => nomeDelFile; set => nomeDelFile = value;
        }

        private string nome;
        /// <summary>
        /// Il nome del testo.
        /// </summary>
        public string Nome
        {
            get => nome; set => nome = value;
        }

        private string abbreviazione;
        /// <summary>
        /// L'abbreviazione del testo.
        /// </summary>
        public string Abbreviazione
        {
            get => abbreviazione; set => abbreviazione = value;
        }

        private string titolo;
        /// <summary>
        /// Il titolo del testo, di solito più lungo del nome, e visualizzato solo nella finestra Informazioni su.
        /// </summary>
        public string Titolo
        {
            get => titolo; set => titolo = value;
        }

        private string autore;
        /// <summary>
        /// L'autore del testo (per una Bibbia, di solito è vuota)
        /// </summary>
        public string Autore
        {
            get => autore; set => autore = value;
        }

        private string casaEditrice;
        /// <summary>
        /// La casa editrice del testo.
        /// </summary>
        public string CasaEditrice
        {
            get => casaEditrice; set => casaEditrice = value;
        }

        private string data;
        /// <summary>
        /// La data di pubblicazione del testo.
        /// </summary>
        public string Data
        {
            get => data; set => data = value;
        }

        private string copyright;
        /// <summary>
        /// Una stringa che descrive il copyright del testo.
        /// </summary>
        public string Copyright
        {
            get => copyright; set => copyright = value;
        }

        private string isbn;
        /// <summary>
        /// Il numero ISBN del testo.
        /// </summary>
        public string Isbn
        {
            get => isbn; set => isbn = value;
        }

        private string descrizione;
        /// <summary>
        /// Una descrizione del testo. Può essere in formato RTF.
        /// </summary>
        public string Descrizione
        {
            get => descrizione; set => descrizione = value;
        }

        private string lingua;
        /// <summary>
        /// La lingua principale del testo. Deve essere un codice ISO 639-1 (2 lettere) oppure ISO 639-2 (3 lettere).
        /// Può anche essere diverse lingue separate da una riga verticale |, principale (che è la lingua quando il testo è considerato come dizionario) e secondarie,
        /// per esempio un dizionario greco-italiano avrebbe lingua el|it.
        /// </summary>
        public string Lingua
        {
            get => lingua; set => lingua = value;
        }

        private string versioneDelleNote;
        /// <summary>
        /// La versione della Bibbia a cui le note fanno riferimento. È vuoto per una Bibbia.
        /// </summary>
        public string VersioneDelleNote
        {
            get => versioneDelleNote; set => versioneDelleNote = value;
        }

        private TestoTipi tipo;
        /// <summary>
        /// Il tipo del testo.
        /// </summary>
        public TestoTipi Tipo
        {
            get => tipo; set => tipo = value;
        }

        private BloccatoTipi bloccato;
        /// <summary>
        /// Il tipo del bloccaggio di una collezione di note.
        /// </summary>
        public BloccatoTipi Bloccato
        {
            get => bloccato; set => bloccato = value;
        }

        /// <summary>
        /// Il costruttore della classe VersioneInformazioni. Valori predefiniti sono dati a tutti i membri.
        /// </summary>
        public VersioneInformazioni()
        {
            versione = "0.0.0";
            nomeDelFile = "";
            nome = "";
            abbreviazione = "";
            titolo = "";
            autore = "";
            casaEditrice = "";
            data = "";
            copyright = "";
            isbn = "";
            descrizione = "";
            lingua = "";
            versioneDelleNote = "";
            tipo = TestoTipi.None;
            bloccato = BloccatoTipi.Sbloccato;
        }

    }

    #endregion

    #region enum per il formato del testo

    /// <summary>
    /// Come visualizzare il testo proprio.
    /// </summary>
    public enum TestoVisualizzato
    {
        /// <summary>
        /// Mostrare ogni versetto su una riga diversa.
        /// </summary>
        Versetti,
        /// <summary>
        /// Mostrare il testo come paragrafi.
        /// </summary>
        Paragrafi,
        /// <summary>
        /// Non mostrare il testo.
        /// </summary>
        Nessuno
    };

    /// <summary>
    /// Come visualizzare i riferimenti nel testo.
    /// </summary>
    public enum RiferimentoTipo
    {
        /// <summary>
        /// Con due punti fra il capitolo e il versetto, per esempio 1P 5:2,6-7
        /// </summary>
        DuePunti,
        /// <summary>
        /// Con una virgola fra il capitolo e il versetto, per esempio 1P 5,2.6-7
        /// </summary>
        Virgola,
        /// <summary>
        /// Come una citazione, per esempio 1P., 5, 2.6-7:
        /// </summary>
        Citazione
    };

    /// <summary>
    /// Come visualizzare il libro nel riferimento.
    /// </summary>
    public enum RiferimentoFormato
    {
        /// <summary>
        /// Il nome intero del libro.
        /// </summary>
        Intero,
        /// <summary>
        /// L'abbreviazione del libro.
        /// </summary>
        Abbreviazione,
        /// <summary>
        /// Non mostrare nessun riferimento.
        /// </summary>
        Nessuno,
        /// <summary>
        /// Non mostrare il nome del libro.
        /// </summary>
        NessunoLibro,
        /// <summary>
        /// Usare un'abbreviazione del libro che il programma riconosce.
        /// </summary>
        AbbreviazioneRiconosciuta
    };

    /// <summary>
    /// Dove visualizzare i riferimenti nel testo.
    /// </summary>
    public enum RiferimentoPosto
    {
        /// <summary>
        /// Prima del testo del versetto, sulla stessa riga.
        /// </summary>
        PrimaStessaRiga,
        /// <summary>
        /// Prima del testo del versetto, sulla riga precedente.
        /// </summary>
        PrimaRigaDiversa,
        /// <summary>
        /// Dopo il testo del versetto.
        /// </summary>
        Dopo
    };

    #endregion

    #region FormatoTesto

    /// <remarks>
    /// Descrive il formato usato per visualizzare il testo biblico.
    /// </remarks>
    public class FormatoTesto
    {

        #region FontPredef

        private string fontNome;
        /// <summary>
        /// Il nome del font predefinito.
        /// </summary>
        public string FontNome
        {
            get => fontNome; set => fontNome = value;
        }

        private float fontDimensione;
        /// <summary>
        /// La dimensione del font predefinito.
        /// </summary>
        public float FontDimensione
        {
            get => fontDimensione; set => fontDimensione = value;
        }

        private bool fontGrassetto;
        /// <summary>
        /// Se il font predefinito è in grassetto.
        /// </summary>
        public bool FontGrassetto
        {
            get => fontGrassetto; set => fontGrassetto = value;
        }
        private bool fontCorsivo;
        /// <summary>
        /// Se il font predefinito è in corsivo.
        /// </summary>
        public bool FontCorsivo
        {
            get => fontCorsivo; set => fontCorsivo = value;
        }
        private bool fontSottolineato;
        /// <summary>
        /// Se il font predefinito è sottolineato.
        /// </summary>
        public bool FontSottolineato
        {
            get => fontSottolineato; set => fontSottolineato = value;
        }

        private System.Windows.Media.Color fontColore;
        /// <summary>
        /// Il colore del font.
        /// </summary>
        public System.Windows.Media.Color FontColore
        {
            get => fontColore; set => fontColore = value;
        }

        #endregion

        #region FontGreco

        private string fontGrecoNome;
        /// <summary>
        /// Il nome del font greco.
        /// </summary>
        public string FontGrecoNome
        {
            get => fontGrecoNome; set => fontGrecoNome = value;
        }

        private float fontGrecoDimensione;
        /// <summary>
        /// La dimensione del font greco.
        /// </summary>
        public float FontGrecoDimensione
        {
            get => fontGrecoDimensione; set => fontGrecoDimensione = value;
        }

        private bool fontGrecoGrassetto;
        /// <summary>
        /// Se il font greco è in grassetto.
        /// </summary>
        public bool FontGrecoGrassetto
        {
            get => fontGrecoGrassetto; set => fontGrecoGrassetto = value;
        }
        private bool fontGrecoCorsivo;
        /// <summary>
        /// Se il font greco è in corsivo.
        /// </summary>
        public bool FontGrecoCorsivo
        {
            get => fontGrecoCorsivo; set => fontGrecoCorsivo = value;
        }
        private bool fontGrecoSottolineato;
        /// <summary>
        /// Se il font greco è sottolineato.
        /// </summary>
        public bool FontGrecoSottolineato
        {
            get => fontGrecoSottolineato; set => fontGrecoSottolineato = value;
        }

        private System.Windows.Media.Color fontGrecoColore;
        /// <summary>
        /// Il colore del font greco.
        /// </summary>
        public System.Windows.Media.Color FontGrecoColore
        {
            get => fontGrecoColore; set => fontGrecoColore = value;
        }

        #endregion

        #region FontEbraico

        private string fontEbraicoNome;
        /// <summary>
        /// Il nome del font ebraico.
        /// </summary>
        public string FontEbraicoNome
        {
            get => fontEbraicoNome; set => fontEbraicoNome = value;
        }

        private float fontEbraicoDimensione;
        /// <summary>
        /// La dimensione del font ebraico.
        /// </summary>
        public float FontEbraicoDimensione
        {
            get => fontEbraicoDimensione; set => fontEbraicoDimensione = value;
        }

        private bool fontEbraicoGrassetto;
        /// <summary>
        /// Se il font ebraico è in grassetto.
        /// </summary>
        public bool FontEbraicoGrassetto
        {
            get => fontEbraicoGrassetto; set => fontEbraicoGrassetto = value;
        }
        private bool fontEbraicoCorsivo;
        /// <summary>
        /// Se il font ebraico è in corsivo.
        /// </summary>
        public bool FontEbraicoCorsivo
        {
            get => fontEbraicoCorsivo; set => fontEbraicoCorsivo = value;
        }
        private bool fontEbraicoSottolineato;
        /// <summary>
        /// Se il font ebraico è sottolineato.
        /// </summary>
        public bool FontEbraicoSottolineato
        {
            get => fontEbraicoSottolineato; set => fontEbraicoSottolineato = value;
        }

        private System.Windows.Media.Color fontEbraicoColore;
        /// <summary>
        /// Il colore del font ebraico.
        /// </summary>
        public System.Windows.Media.Color FontEbraicoColore
        {
            get => fontEbraicoColore; set => fontEbraicoColore = value;
        }

        #endregion

        #region FontRiferimento

        private string fontRiferimentoNome;
        /// <summary>
        /// Il nome del font usato per i riferimenti.
        /// </summary>
        public string FontRiferimentoNome
        {
            get => fontRiferimentoNome; set => fontRiferimentoNome = value;
        }

        private float fontRiferimentoDimensione;
        /// <summary>
        /// La dimensione del font usato per i riferimenti.
        /// </summary>
        public float FontRiferimentoDimensione
        {
            get => fontRiferimentoDimensione; set => fontRiferimentoDimensione = value;
        }

        private bool fontRiferimentoGrassetto;
        /// <summary>
        /// Se il font usato per i riferimenti è in grassetto.
        /// </summary>
        public bool FontRiferimentoGrassetto
        {
            get => fontRiferimentoGrassetto; set => fontRiferimentoGrassetto = value;
        }

        private bool fontRiferimentoCorsivo;
        /// <summary>
        /// Se il font usato per i riferimenti è in corsivo.
        /// </summary>
        public bool FontRiferimentoCorsivo
        {
            get => fontRiferimentoCorsivo; set => fontRiferimentoCorsivo = value;
        }

        private bool fontRiferimentoSottolineato;
        /// <summary>
        /// Se il font usato per i riferimenti è sottolineato.
        /// </summary>
        public bool FontRiferimentoSottolineato
        {
            get => fontRiferimentoSottolineato; set => fontRiferimentoSottolineato = value;
        }

        private System.Windows.Media.Color fontRiferimentoColore;
        /// <summary>
        /// Il colore del font usato per i riferimenti.
        /// </summary>
        public System.Windows.Media.Color FontRiferimentoColore
        {
            get => fontRiferimentoColore; set => fontRiferimentoColore = value;
        }

        private bool riferimentoApice;
        /// <summary>
        /// Se i riferimenti sono in apice.
        /// </summary>
        public bool RiferimentoApice
        {
            get => riferimentoApice; set => riferimentoApice = value;
        }

        private bool riferimentoContestoRicerche;
        /// <summary>
        /// Se un collegamento ipertestuale è creato per i riferimenti in una ricerca.
        /// </summary>
        public bool RiferimentoContestoRicerche
        {
            get => riferimentoContestoRicerche; set => riferimentoContestoRicerche = value;
        }

        #endregion

        #region FontRicerca

        private string fontRicercaNome;
        /// <summary>
        /// Il nome del font usato per le parole ricercate.
        /// </summary>
        public string FontRicercaNome
        {
            get => fontRicercaNome; set => fontRicercaNome = value;
        }

        private float fontRicercaDimensione;
        /// <summary>
        /// La dimensione del font usato per le parole ricercate.
        /// </summary>
        public float FontRicercaDimensione
        {
            get => fontRicercaDimensione; set => fontRicercaDimensione = value;
        }

        private bool fontRicercaGrassetto;
        /// <summary>
        /// Se il font usato per le parole ricercate è in grassetto.
        /// </summary>
        public bool FontRicercaGrassetto
        {
            get => fontRicercaGrassetto; set => fontRicercaGrassetto = value;
        }
        private bool fontRicercaCorsivo;
        /// <summary>
        /// Se il font usato per le parole ricercate è in corsivo.
        /// </summary>
        public bool FontRicercaCorsivo
        {
            get => fontRicercaCorsivo; set => fontRicercaCorsivo = value;
        }
        private bool fontRicercaSottolineato;
        /// <summary>
        /// Se il font usato per le parole ricercate è sottolineato.
        /// </summary>
        public bool FontRicercaSottolineato
        {
            get => fontRicercaSottolineato; set => fontRicercaSottolineato = value;
        }

        private System.Windows.Media.Color fontRicercaColore;
        /// <summary>
        /// Il colore del font usato per le parole ricercate.
        /// </summary>
        public System.Windows.Media.Color FontRicercaColore
        {
            get => fontRicercaColore; set => fontRicercaColore = value;
        }

        #endregion

        private bool titoliVisualizzati;
        /// <summary>
        /// Se i titoli delle sezioni sono visualizzati nel testo biblico.
        /// </summary>
        public bool TitoliVisualizzati
        {
            get => titoliVisualizzati; set => titoliVisualizzati = value;
        }

        private TestoVisualizzato testoVisualizzato;
        /// <summary>
        /// Come il testo biblico è visualizzato.
        /// </summary>
        public TestoVisualizzato TestoVisualizzato
        {
            get => testoVisualizzato; set => testoVisualizzato = value;
        }

        private RiferimentoTipo riferimentoTipo;
        /// <summary>
        /// Il tipo di riferimento da usare.
        /// </summary>
        public RiferimentoTipo RiferimentoTipo
        {
            get => riferimentoTipo; set => riferimentoTipo = value;
        }

        private RiferimentoFormato riferimentoFormato;
        /// <summary>
        /// Il formato dei riferimenti.
        /// </summary>
        public RiferimentoFormato RiferimentoFormato
        {
            get => riferimentoFormato; set => riferimentoFormato = value;
        }

        private RiferimentoPosto riferimentoPosto;
        /// <summary>
        /// La posizione dei riferimenti relativa al testo.
        /// </summary>
        public RiferimentoPosto RiferimentoPosto
        {
            get => riferimentoPosto; set => riferimentoPosto = value;
        }

        /// <summary>
        /// Il costruttore della classe FormatoTesto. Valori predefiniti sono dati a tutti i membri.
        /// </summary>
        /// <seealso cref="FormatoTesto"/>
        public FormatoTesto()
        {
            fontNome = IsRunningOnMono() ? "Times New Roman" : "Georgia";
            fontDimensione = 12;
            // false è il valore predefinito, quindi non è necessario impostarlo
            //            fontGrassetto = false;
            //            fontCorsivo = false;
            //            fontSottolineato = false;
            fontColore = System.Windows.Media.Colors.Black;

            fontGrecoNome = fontNome;
            fontGrecoDimensione = 12;
            fontGrecoColore = System.Windows.Media.Colors.Black;
            fontEbraicoNome = "Times New Roman";
            fontEbraicoDimensione = 14;
            fontEbraicoColore = System.Windows.Media.Colors.Black;

            fontRiferimentoNome = fontNome;
            fontRiferimentoDimensione = 12;
            fontRiferimentoGrassetto = true;
            //            fontRiferimentoCorsivo = false;
            //            fontRiferimentoSottolineato = false;
            //            riferimentoApice = false;
            // riferimentoContestoRicerche = false;
            fontRiferimentoColore = System.Windows.Media.Colors.Black;

            fontRicercaNome = fontNome;
            fontRicercaDimensione = 12;
            //            fontRicercaGrassetto = false;
            //            fontRicercaCorsivo = false;
            fontRicercaSottolineato = true;
            fontRicercaColore = System.Windows.Media.Colors.Black;

            titoliVisualizzati = true;
            riferimentoTipo = RiferimentoTipo.DuePunti;
            riferimentoFormato = RiferimentoFormato.Abbreviazione;
            riferimentoPosto = RiferimentoPosto.PrimaStessaRiga;
            testoVisualizzato = TestoVisualizzato.Paragrafi;
        }

        /// <summary>
        /// Copia tutte le caratteristiche di un formato ad un altro.
        /// </summary>
        /// <param name="formato">Il formato a cui copiare le caratteristiche.</param>
        public void CopiaA(FormatoTesto formato)
        {
            formato.fontNome = fontNome;
            formato.fontDimensione = fontDimensione;
            formato.fontGrassetto = fontGrassetto;
            formato.fontCorsivo = fontCorsivo;
            formato.fontSottolineato = fontSottolineato;
            formato.fontColore = fontColore;

            formato.fontGrecoNome = fontGrecoNome;
            formato.fontGrecoDimensione = fontGrecoDimensione;
            formato.fontGrecoColore = fontGrecoColore;

            formato.fontEbraicoNome = fontEbraicoNome;
            formato.fontEbraicoDimensione = fontEbraicoDimensione;
            formato.fontEbraicoColore = fontEbraicoColore;

            formato.fontRiferimentoNome = fontRiferimentoNome;
            formato.fontRiferimentoDimensione = fontRiferimentoDimensione;
            formato.fontRiferimentoGrassetto = fontRiferimentoGrassetto;
            formato.fontRiferimentoCorsivo = fontRiferimentoCorsivo;
            formato.fontRiferimentoSottolineato = fontRiferimentoSottolineato;
            formato.fontRiferimentoColore = fontRiferimentoColore;
            formato.riferimentoApice = riferimentoApice;
            formato.riferimentoContestoRicerche = riferimentoContestoRicerche;

            formato.fontRicercaNome = fontRicercaNome;
            formato.fontRicercaDimensione = fontRicercaDimensione;
            formato.fontRicercaGrassetto = fontRicercaGrassetto;
            formato.fontRicercaCorsivo = fontRicercaCorsivo;
            formato.fontRicercaSottolineato = fontRicercaSottolineato;
            formato.fontRicercaColore = fontRicercaColore;

            formato.titoliVisualizzati = titoliVisualizzati;
            formato.riferimentoTipo = riferimentoTipo;
            formato.riferimentoFormato = riferimentoFormato;
            formato.riferimentoPosto = riferimentoPosto;
            formato.testoVisualizzato = testoVisualizzato;
        }

        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }

    #endregion

    #region Abbreviazioni riconosciute

    /// <summary>
    /// Una classe che gestisce le abbreviazioni riconosciute dal libro.
    /// </summary>
    public class LibriAbbreviazioniRiconosciuteHash
    {
        private readonly Dictionary<string, byte> libriAbbreviazioniRiconosciute = [];

        /// <summary>
        /// Il costruttore della classe.
        /// </summary>
        internal LibriAbbreviazioniRiconosciuteHash()
        {
            libriAbbreviazioniRiconosciute = [];
        }

        /// <summary>
        /// Restituisce il libro che corrisponde ad una data abbreviazione.
        /// </summary>
        /// <param name="abbreviazione">L'abbreviazione di cui si vuole il libro.</param>
        /// <returns>Il numero del libro.</returns>
        public byte this[string abbreviazione]
        {
            get => libriAbbreviazioniRiconosciute[abbreviazione]; set => libriAbbreviazioniRiconosciute[abbreviazione] = value;
        }

        /// <summary>
        /// Restituisce un'abbreviazione riconosciuto di un certo libro.
        /// </summary>
        /// <param name="libro">Il numero di un libro.</param>
        /// <returns>Un'abbreviazione riconosciuta.</returns>
        public string Abbreviazione(byte libro)
        {
            string rifLibro = "";
            foreach (KeyValuePair<string, byte> k in libriAbbreviazioniRiconosciute)
            {
                if (k.Value == libro)
                {
                    rifLibro = k.Key;
                    break;
                }
            }
            return rifLibro;
        }

        /// <summary>
        /// Decide se l'abbreviazione è riconosciuta.
        /// </summary>
        /// <param name="abbreviazione">L'abbreviazione da controllare.</param>
        /// <returns>Vero se l'abbreviazione è riconosciuta.</returns>
        public bool ContainsKey(string abbreviazione)
        {
            return libriAbbreviazioniRiconosciute.ContainsKey(abbreviazione);
        }

        /// <summary>
        /// Rimuovi tutte le abbreviazioni dall'elenco.
        /// </summary>
        public void Clear()
        {
            libriAbbreviazioniRiconosciute.Clear();
        }

        /// <summary>
        /// Restituisce tutte le abbreviazioni riconosciute, ordinate per libro.
        /// </summary>
        /// <returns>Un array con 73 elementi (da 0 a 72), ogni elemento ha tutte le abbreviazioni separate da una virgola per un libro.</returns>
        public string[] AbbreviazioniPerLibro()
        {
            string[] abbreviazioniRiconoconosciute = new string[73];
            foreach (KeyValuePair<string, byte> keyValueAbbreviazioneNumero in libriAbbreviazioniRiconosciute)
            {
                abbreviazioniRiconoconosciute[keyValueAbbreviazioneNumero.Value - 1] += keyValueAbbreviazioneNumero.Key + ",";
            }

            return abbreviazioniRiconoconosciute;
        }
    }

    #endregion

    #region Exception

    /// <summary>
    /// Exception quando una richiesta è fatta per informazioni di una versione che non esiste.
    /// </summary>
    public class TextNotExistException : Exception
    {
        /// <summary>
        /// Exception quando una richiesta è fatta per informazioni di una versione che non esiste.
        /// </summary>
        public TextNotExistException()
        {
        }

        /// <summary>
        /// Exception quando una richiesta è fatta per informazioni di una versione che non esiste.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        public TextNotExistException(string messaggio)
            : base(messaggio)
        {
        }

        /// <summary>
        /// Exception quando una richiesta è fatta per informazioni di una versione che non esiste.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public TextNotExistException(string messaggio, Exception innerException)
            : base(messaggio, innerException)
        {
        }
    }

    /// <summary>
    /// Exception quando la scrittura di una collezione di note modificata dà un errore.
    /// </summary>
    public class ImpossibileScrivereModificheException : Exception
    {
        /// <summary>
        /// Exception quando la scrittura di una collezione di note modificata dà un errore.
        /// </summary>
        public ImpossibileScrivereModificheException()
        {
        }

        /// <summary>
        /// Exception quando la scrittura di una collezione di note modificata dà un errore.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        public ImpossibileScrivereModificheException(string messaggio)
            : base(messaggio)
        {
        }

        /// <summary>
        /// Exception quando la scrittura di una collezione di note modificata dà un errore.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public ImpossibileScrivereModificheException(string messaggio, Exception innerException)
            : base(messaggio, innerException)
        {
        }
    }

    /// <summary>
    /// Exception quando il file da aprire con un testo del programma non è valido.
    /// </summary>
    public class FileNonValidoException : Exception
    {
        /// <summary>
        /// Exception quando il file da aprire con un testo del programma non è valido.
        /// </summary>
        public FileNonValidoException()
        {
        }

        /// <summary>
        /// Exception quando il file da aprire con un testo del programma non è valido.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        public FileNonValidoException(string messaggio)
            : base(messaggio)
        {
        }

        /// <summary>
        /// Exception quando il file da aprire con un testo del programma non è valido.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public FileNonValidoException(string messaggio, Exception innerException)
            : base(messaggio, innerException)
        {
        }
    }

    /// <summary>
    /// Exception quando un carattere sconosciuto è trovato in un file che è analizzato per cercare le parole.
    /// </summary>
    public class CarattereSconosciutoException : Exception
    {
        /// <summary>
        /// Exception quando un carattere sconosciuto è trovato in un file che è analizzato per cercare le parole.
        /// </summary>
        public CarattereSconosciutoException()
            : base()
        {
        }

        /// <summary>
        /// Exception quando un carattere sconosciuto è trovato in un file che è analizzato per cercare le parole.
        /// </summary>
        /// <param name="testo">Il testo che contiene il carattere sconosciuto.</param>
        public CarattereSconosciutoException(string testo)
            : base(testo)
        {
        }

        /// <summary>
        /// Exception quando un carattere sconosciuto è trovato in un file che è analizzato per cercare le parole.
        /// </summary>
        /// <param name="testo">Il testo che contiene il carattere sconosciuto.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public CarattereSconosciutoException(string testo, Exception innerException)
            : base(testo, innerException)
        {
        }
    }

    /// <summary>
    /// Exception quando l'espressione da ricercare è vuota.
    /// </summary>
    public class SearchExpressionEmptyException : Exception
    {
        /// <summary>
        /// Exception quando l'espressione da ricercare è vuota.
        /// </summary>
        public SearchExpressionEmptyException()
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare è vuota.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        public SearchExpressionEmptyException(string messaggio)
            : base(messaggio)
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare è vuota.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public SearchExpressionEmptyException(string messaggio, Exception innerException)
            : base(messaggio, innerException)
        {
        }
    }

    /// <summary>
    /// Exception quando l'espressione da ricercare ha un errore di sintassi.
    /// </summary>
    public class SearchSyntaxErrorException : Exception
    {
        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore di sintassi.
        /// </summary>
        /// <param name="carattere">Il numero del carattere dove c'è l'errore.</param>
        public SearchSyntaxErrorException(string carattere)
            : base(carattere)
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore di sintassi.
        /// </summary>
        public SearchSyntaxErrorException()
            : base()
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore di sintassi.
        /// </summary>
        /// <param name="carattere">Il numero del carattere dove c'è l'errore.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public SearchSyntaxErrorException(string carattere, Exception innerException)
            : base(carattere, innerException)
        {
        }
    }

    /// <summary>
    /// Exception quando l'espressione da ricercare ha un errore nelle parentesi.
    /// </summary>
    public class SearchParenthesesException : Exception
    {
        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore nelle parentesi.
        /// </summary>
        public SearchParenthesesException()
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore nelle parentesi.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        public SearchParenthesesException(string messaggio)
            : base(messaggio)
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore nelle parentesi.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public SearchParenthesesException(string messaggio, Exception innerException)
            : base(messaggio, innerException)
        {
        }
    }

    /// <summary>
    /// Exception quando l'espressione da ricercare ha un errore nelle parentesi quadrate.
    /// </summary>
    public class SearchBracketsException : Exception
    {
        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore nelle parentesi quadrate.
        /// </summary>
        public SearchBracketsException()
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore nelle parentesi quadrate.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        public SearchBracketsException(string messaggio)
            : base(messaggio)
        {
        }

        /// <summary>
        /// Exception quando l'espressione da ricercare ha un errore nelle parentesi quadrate.
        /// </summary>
        /// <param name="messaggio">Il messaggio da dare.</param>
        /// <param name="innerException">L'exception che è la causa dell'exception attuale.</param>
        public SearchBracketsException(string messaggio, Exception innerException)
            : base(messaggio, innerException)
        {
        }
    }

    #endregion

    #region UltimaBibbia

    /// <summary>
    /// Gli argomenti dell'evento quando la Bibbia utilizzata è cambiata.
    /// </summary>
    /// <remarks>
    /// Il costruttore della classe.
    /// </remarks>
    /// <param name="bibbiaUtilizzata">La Bibbia utilizzata.</param>
    public class UltimaBibbiaEventArgs(string bibbiaUtilizzata) : EventArgs
    {
        private readonly string nuovaBibbia = bibbiaUtilizzata;
        /// <summary>
        /// La Bibbia utilizzata.
        /// </summary>
        public string NuovaBibbia
        {
            get { return nuovaBibbia; }
        }
    }

    /*
    /// <summary>
    /// Il delegate che inizia l'evento quando la Bibbia utilizzata è cambiata.
    /// </summary>
    /// <param name="sender">La classe che ha generato l'evento.</param>
    /// <param name="e">Gli argomenti dell'evento.</param>
    public delegate void UltimaBibbiaEventHandler(object sender, UltimaBibbiaEventArgs e);
    */

    #endregion

    /// <remarks>
    /// Una classe che contiene tutte le informazioni sui testi biblici trovati, e restituisce le informazioni necessarie ad altri programmi.
    /// </remarks>
    public class Texts
    {
        #region const

        /// <summary>
        /// I nomi di tutti i libri della Bibbia in inglese.
        /// </summary>
        public const string LibriNomiInglese = "|Genesis|Exodus|Leviticus|Numbers|Deuteronomy|Joshua|Judges|Ruth|1Samuel|2Samuel|1Kings|2Kings|1Chronicles|2Chronicles|Ezra|Nehemiah|Tobit|Judith|Esther|1Maccabees|2Maccabees|Job|Psalms|Proverbs|Ecclesiastes|Song of Songs|Wisdom|Sirach|Isaiah|Jeremiah|Lamentations|Baruch|Ezekiel|Daniel|Hosea|Joel|Amos|Obadiah|Jonah|Micah|Nahum|Habakkuk|Zephaniah|Haggai|Zechariah|Malachi|Matthew|Mark|Luke|John|Acts|Romans|1Corinthians|2Corinthians|Galatians|Ephesians|Philippians|Colossians|1Thessalonians|2Thessalonians|1Timothy|2Timothy|Titus|Philemon|Hebrews|James|1Peter|2Peter|1John|2John|3John|Jude|Revelation";
        /// <summary>
        /// Le abbreviazioni usate dei libri della Bibbia in inglese.
        /// </summary>
        public const string LibriAbbreviazioniUsateInglese = "|Gen|Ex|Le|Nu|De|Josh|Judg|Ru|1Sam|2Sam|1K|2K|1Chr|2Chr|Ezra|Ne|Tob|Judi|Est|1M|2M|Job|Ps|Prov|Ec|SS|Wis|Sir|Is|Jer|Lam|Bar|Ezek|Dan|Hos|Joel|Am|Ob|Jon|Mi|Na|Hab|Zep|Hag|Zec|Mal|Mat|Mar|Lu|John|Ac|Ro|1Co|2Co|Ga|Eph|Phili|Col|1Th|2Th|1Ti|2Ti|Tit|Phile|Heb|Jam|1P|2P|1J|2J|3J|Jude|Rev";
        /// <summary>
        /// Le abbreviazioni riconosciute dei libri della Bibbia in inglese.
        /// </summary>
        public const string LibriAbbreviazioniRiconosciuteInglese = "|gen,gn|ex|le,lv|nm,nu|de,dt|jos,js|jdg,jg,judg|rt,ru|1 s,1s,isam|2 s,2s,iis|1 k,1k,ik|2 k,2k,iik|1 ch,1ch,ich|2 ch,2ch,iich|ezr|ne|tb,to|jdt,jt,judi|est,et|1 m,1m,im|2 m,2m,iim|jb,job|ps|pr,pv|ec|so,ss|w|si|is|je,jr|la|b|ez|da,dn|ho|jl,joe|am|o|jon|mi|na|hab|zep|hag|zec|mal,ml|mat,mt|mar,mk,mr|lk,lu|jn,joh|ac|rm,ro|1 co,1co,ico|2 co,2co,iico|ga|ep|phi,php,pl|cl,co|1 th,1th,1ts,ith|2 th,2th,2ts,iith|1 ti,1ti,1tm,iti|2 ti,2ti,2tm,iiti|ti,tt|phile,phlm,phm,pm|he|jam,jas,jm|1 p,1p,ip|2 p,2p,iip|1 j,1j,ij|2 j,2j,iij|3 j,3j,iiij|jd,jude|re";
        /// <summary>
        /// I nomi di tutti i libri della Bibbia in italiano.
        /// </summary>
        public const string LibriNomiItaliano = "|Genesi|Esodo|Levitico|Numeri|Deuteronomio|Giosuè|Giudici|Rut|1Samuele|2Samuele|1Re|2Re|1Cronache|2Cronache|Esdra|Neemia|Tobia|Giuditta|Ester|1Maccabei|2Maccabei|Giobbe|Salmi|Proverbi|Ecclesiaste|Cantico|Sapienza|Siracide|Isaia|Geremia|Lamentazioni|Baruc|Ezechiele|Daniele|Osea|Gioele|Amos|Abdia|Giona|Michea|Naum|Abacuc|Sofonia|Aggeo|Zaccaria|Malachia|Matteo|Marco|Luca|Giovanni|Atti|Romani|1Corinzi|2Corinzi|Galati|Efesini|Filippesi|Colossesi|1Tessalonicesi|2Tessalonicesi|1Timoteo|2Timoteo|Tito|Filemone|Ebrei|Giacomo|1Pietro|2Pietro|1Giovanni|2Giovanni|3Giovanni|Giuda|Apocalisse";
        /// <summary>
        /// Le abbreviazioni usate dei libri della Bibbia in italiano.
        /// </summary>
        public const string LibriAbbreviazioniUsateItaliano = "|Gen|Eso|Le|Nu|De|Gios|Giudic|Ru|1Sam|2Sam|1Re|2Re|1Cr|2Cr|Esd|Ne|Tob|Giudit|Est|1Macc|2Macc|Giob|Sal|Prov|Ec|CC|Sap|Sir|Is|Ger|Lam|Bar|Ez|Da|Os|Gioe|Am|Abd|Gion|Mi|Na|Abac|So|Ag|Zac|Mal|Mt|Mc|Lc|Gv|At|Rm|1Cor|2Cor|Gal|Ef|Fili|Col|1Ts|2Ts|1Tm|2Tm|Tt|Fm|Eb|Giac|1P|2P|1G|2G|3G|Giuda|Ap";
        /// <summary>
        /// Le abbreviazioni riconosciute dei libri della Bibbia in italiano.
        /// </summary>
        public const string LibriAbbreviazioniRiconosciuteItaliano = "|ge,gn|eo,es|le,lv|nm,nu|de,dt|gios,gs|gdc,giudic|rt,ru|1 s,1s,isam|2 s,2s,iis|1 r,1r,ir|2 r,2r,iir|1 cr,1cr,icr|2 cr,2cr,iicr|ed,esd|ne|tb,to|giudit|est,et|1 m,1m,im|2 m,2m,iim|gb,giob|sal,sl|pr,pv|ec,q|ca,cc,ct|sap|si|is|ger,gr|la|b|ez|da,dn|o|gioe,gl|am|abd,ad|gion|mi|na|aba,ac,h|so|ag|z|mal,ml|mat,mt|mar,mc,mr|lc,lu|giov,gv|at|rm,ro|1 co,1co,ico|2 co,2co,iico|ga|ef|fili,fl|cl,co|1 te,1te,1ts,ite|2 te,2te,2ts,iite|1 ti,1ti,1tm,iti|2 ti,2ti,2tm,iiti|ti,tt|file,fm|eb|gc,gia,gm|1 p,1p,ip|2 p,2p,iip|1 g,1g,ig|2 g,2g,iig|3 g,3g,iiig|gd,giuda|ap";
        /// <summary>
        /// I nomi di tutti i libri della Bibbia in spagnolo.
        /// </summary>
        public const string LibriNomiSpagnolo = "|Génesis|Éxodo|Levítico|Números|Deuteronomio|Josué|Jueces|Rut|1Samuel|2Samuel|1Reyes|2Reyes|1Crónicas|2Crónicas|Esdras|Nehemías|Tobit|Judit|Ester|1Macabeos|2Macabeos|Job|Salmos|Proverbios|Eclesiastés|Cantares|Sabiduría|Eclesiástico|Isaías|Jeremías|Lamentaciones|Baruc|Ezequiel|Daniel|Oseas|Joel|Amós|Abdías|Jonás|Miqueas|Nahum|Habacuc|Zofonías|Hageo|Zacarías|Malaquías|Mateo|Marcos|Lucas|Juan|Hechos|Romanos|1Corintios|2Corintios|Gálatas|Efesios|Filipenses|Colosenses|1Tesalonicenses|2Tesalonicenses|1Timoteo|2Timoteo|Tito|Filemón|Hebreos|Santiago|1Pedro|2Pedro|1Juan|2Juan|3Juan|Judas|Apocalipsis";
        /// <summary>
        /// Le abbreviazioni usate dei libri della Bibbia in spagnolo.
        /// </summary>
        public const string LibriAbbreviazioniUsateSpagnolo = "|Gn|Ex|Lv|Nm|Dt|Jos|Jue|Rt|1S|2S|1R|2R|1Cr|2Cr|Esd|Neh|Tb|Jdt|Est|1M|2M|Job|Sal|Pr|Ec|Cnt|Sab|Eclo|Is|Jer|Lm|Bar|Ez|Dn|Os|Jl|Am|Abd|Jon|Mi|Nah|Hab|Sof|Hag|Zac|Mal|Mt|Mr|Lc|Jn|Hch|Rm|1Co|2Co|Gá|Ef|Fil|Col|1Ts|2Ts|1Ti|2Ti|Tit|Flm|He|Stg|1P|2P|1Jn|2Jn|3Jn|Jud|Ap";
        /// <summary>
        /// Le abbreviazioni riconosciute dei libri della Bibbia in spagnolo.
        /// </summary>
        public const string LibriAbbreviazioniRiconosciuteSpagnolo = "|gé,ge,gn|éx,ex|le,lv|nm,nu,nú|de,dt|jos,js|jue,jc|rt,ru|1 s,1s,isam|2 s,2s,iis|1 r,1r,ir|2 r,2r,iir|1 cr,1cr,icr|2 cr,2cr,iicr|esd,ed|ne,nh|tb,to|jdt,jt,judi|est,et|1 m,1m,im|2 m,2m,iim|jb,job|sal,slm|pr,pv|ec|cnt,can|sab,sb|eclo,si|is|je,jr|la,lm|bar,br|ez|da,dn|os|jl,joe|am|abd|jon,jn|mi|na,nh|hab,hb|sof,sf|hag,hg|zac,zc|mal,ml|mat,mt|mar,mr|lc,lu|jn,ju|hch,hech|rm,ro|1 co,1co,ico|2 co,2co,iico|ga,gá|ef|fil,flp|cl,col|1 ts,1ts,1tes,its|2 ts,2ts,2ts,iits,2tes|1 ti,1ti,1tm,iti|2 ti,2ti,2tm,iiti|ti,tt|flm,file,fm|he|stg,sant,snt,sg|1 p,1p,ip|2 p,2p,iip|1 j,1j,ij|2 j,2j,iij|3 j,3j,iiij|jud,jd|ap,rev,rv";

        public static readonly string[] paroleItalianeConApostrofe = ["be", "co", "com", "da", "de", "di", "die", "dov", "e", "fa", "fe", "mo", "pe", "po", "quant", "que", "rifa", "sta", "va"];
        public static readonly string[] paroleInglesiSenzaApostrofe = ["amiss", "apostates", "commandments", "fillets", "holiness", "intercessions", "jealous", "means", "peres", "prayer-fillets", "prayers", "prays", "righteous", "terms", "us", "was", "yahweh's", "yes"];

        private static readonly XmlLanguage HebrewLanguage = XmlLanguage.GetLanguage("he-IL");
        private static readonly XmlLanguage EnglishLanguage = XmlLanguage.GetLanguage("en-US");

        #endregion

        #region properties

        private FormatoTesto formato = new();
        /// <summary>
        /// Il formato del testo biblico visualizzato.
        /// </summary>
        /// <seealso cref="FormatoTesto"/>
        public FormatoTesto Formato
        {
            get => formato; set => formato = value;
        }

        #region UltimaBibbia

        /// <summary>
        /// L'handler dell'evento quando la Bibbia utilizzata è cambiata.
        /// </summary>
        public event EventHandler<UltimaBibbiaEventArgs> UltimaBibbiaEvento;

        /// <summary>
        /// Inizia l'evento quando la Bibbia utilizzata è cambiata.
        /// </summary>
        /// <param name="e">Gli argomenti dell'evento.</param>
        protected virtual void OnChangedUltimaBibbia(UltimaBibbiaEventArgs e)
        {
            // Invokes the delegates. 
            UltimaBibbiaEvento?.Invoke(this, e);
        }

        private string ultimaBibbia = "";
        /// <summary>
        /// L'ultima versione della Bibbia usata dall'utente.
        /// È usata quando il programma deve mostrare del testo in una versione qualsiasi, per esempio nelle Opzioni.
        /// </summary>
        public string UltimaBibbia
        {
            get
            {
                if (string.IsNullOrEmpty(ultimaBibbia))
                {
                    ultimaBibbia = TrovaUltimaBibbiaCompleta();
                }

                return ultimaBibbia;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ultimaBibbia = value;
                    UltimaBibbiaEventArgs e = new(ultimaBibbia);
                    OnChangedUltimaBibbia(e);
                    try
                    {
                        if (versioni[ultimaBibbia].indiceCapitolo[73] > 1000 && versioni[ultimaBibbia].capitoliInLibro[1] > 0 && versioni[ultimaBibbia].capitoliInLibro[17] > 0 && versioni[ultimaBibbia].capitoliInLibro[47] > 0)
                        {
                            ultimaBibbiaCompleta = ultimaBibbia;
                        }
                        // se non abbiamo mai trovato una Bibbia con apocrifa, anche una adesso senza va bene
                        if (string.IsNullOrEmpty(ultimaBibbiaCompleta) || versioni[ultimaBibbiaCompleta].capitoliInLibro[17] == 0)
                        {
                            if (versioni[ultimaBibbia].indiceCapitolo[73] > 1000 && versioni[ultimaBibbia].capitoliInLibro[1] > 0 && versioni[ultimaBibbia].capitoliInLibro[47] > 0)
                            {
                                ultimaBibbiaCompleta = ultimaBibbia;
                            }
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        // se ultimaBibbia non è infatti il nome di una Bibbia
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // se ultimaBibbia non contiene 73 libri
                    }
                }
            }
        }

        private string ultimaBibbiaCompleta = "";
        /// <summary>
        /// L'ultima versione della Bibbia usata dall'utente che contiene Genesi, Matteo e preferibilmente Tobia (cioè AT, apocrifa e NT).
        /// </summary>
        public string UltimaBibbiaCompleta
        {
            get
            {
                if (string.IsNullOrEmpty(ultimaBibbiaCompleta))
                {
                    ultimaBibbiaCompleta = TrovaUltimaBibbiaCompleta();
                }

                return ultimaBibbiaCompleta;
            }

            set => ultimaBibbiaCompleta = value;
        }

        private string TrovaUltimaBibbiaCompleta()
        {
            // cercare una Bibbia con sia l'AT sia il NT, e preferibilmente l'apocrifa
            string bibbiaDaRestituire = "";
            string possibileUltimaBibbia = "", ultimaBibbiaSenzaApocrifa = "";
            foreach (KeyValuePair<string, Versione> versione in versioni)
            {
                if (versione.Value.Info.Tipo == TestoTipi.Bibbia)
                {
                    if (CapitoliInLibro(1, versione.Key) > 0 && CapitoliInLibro(17, versione.Key) > 0 && CapitoliInLibro(47, versione.Key) > 0)
                    {
                        bibbiaDaRestituire = versione.Key;
                        break;
                    }
                    if (CapitoliInLibro(1, versione.Key) > 0 && CapitoliInLibro(47, versione.Key) > 0)
                    {
                        ultimaBibbiaSenzaApocrifa = versione.Key;
                    }
                    else
                    {
                        possibileUltimaBibbia = versione.Key;
                    }
                }
            }
            if (string.IsNullOrEmpty(bibbiaDaRestituire) && !string.IsNullOrEmpty(ultimaBibbiaSenzaApocrifa))
            {
                bibbiaDaRestituire = ultimaBibbiaSenzaApocrifa;
            }

            if (string.IsNullOrEmpty(bibbiaDaRestituire) && !string.IsNullOrEmpty(possibileUltimaBibbia))
            {
                bibbiaDaRestituire = possibileUltimaBibbia;
            }

            return bibbiaDaRestituire;
        }

        #endregion

        #region Libri

        internal string[] libriNomi = [];
        /// <summary>
        /// Restituisce i nomi usati dei libri della Bibbia (inclusa l'apocrifa).
        /// </summary>
        /// <param name="numeroLibro">Il numero del libro (da 1 a 73).</param>
        /// <returns>Il nome del libro.</returns>
        public string GetLibroNome(int numeroLibro)
        {
            return ((numeroLibro >= 1 && numeroLibro <= 73) ? libriNomi[numeroLibro] : "");
        }
        /// <summary>
        /// Imposta i nomi usati dei libri della Bibbia (inclusa l'apocrifa).
        /// </summary>
        /// <param name="numeroLibro">Il numero del libro (da 1 a 73).</param>
        /// <param name="nome">Il nome del libro.</param>
        public void SetLibroNome(int numeroLibro, string nome)
        {
            if (numeroLibro >= 1 && numeroLibro <= 73)
            {
                libriNomi[numeroLibro] = nome;
            }
        }

        internal string[] libriAbbreviazioniUsate = [];
        /// <summary>
        /// Restituisce le abbreviazioni usate dei libri della Bibbia (inclusa l'apocrifa).
        /// </summary>
        /// <param name="numeroLibro">Il numero del libro (da 1 a 73).</param>
        /// <returns>L'abbreviazione del libro.</returns>
        public string GetLibroAbbreviazioneUsata(int numeroLibro)
        {
            return ((numeroLibro >= 1 && numeroLibro <= 73) ? libriAbbreviazioniUsate[numeroLibro] : "");
        }
        /// <summary>
        /// Imposta le abbreviazioni usate dei libri della Bibbia (inclusa l'apocrifa).
        /// </summary>
        /// <param name="numeroLibro">Il numero del libro (da 1 a 73).</param>
        /// <param name="nome">L'abbreviazione del libro.</param>
        public void SetLibroAbbreviazioneUsata(int numeroLibro, string nome)
        {
            if (numeroLibro >= 1 && numeroLibro <= 73)
            {
                libriAbbreviazioniUsate[numeroLibro] = nome;
            }
        }

        private LibriAbbreviazioniRiconosciuteHash libriAbbreviazioniRiconosciute = new();
        /// <summary>
        /// Le abbreviazioni dei libri che il programma riconosce. 
        /// </summary>
        public LibriAbbreviazioniRiconosciuteHash LibriAbbreviazioniRiconosciute
        {
            get { return libriAbbreviazioniRiconosciute; }
            //            set { libriAbbreviazioniRiconosciute = value; }
        }

        /// <summary>
        /// Trova il numero del libro (da 1 a 73; 0 se l'abbreviazione non è stata trovata) che corrisponde ad un'abbreviazione del nome di un libro.
        /// </summary>
        /// <param name="abbreviazione">L'abbreviazione da ricercare.</param>
        /// <returns>Il numero del libro.</returns>
        public byte GetLibroNumeroDaAbbreviazione(string abbreviazione)
        {
            if (!string.IsNullOrEmpty(abbreviazione))
            {
                abbreviazione = abbreviazione.ToLower(CultureInfo.InvariantCulture);
                for (int numeroLettere = abbreviazione.Length; numeroLettere > 0; --numeroLettere)
                {
                    if (libriAbbreviazioniRiconosciute.ContainsKey(abbreviazione[..numeroLettere]))
                    {
                        return libriAbbreviazioniRiconosciute[abbreviazione[..numeroLettere]];
                    }
                }
            }
            return 0;
        }

        #endregion

        #endregion

        private Dictionary<string, Versione> versioni = [];
        private Dictionary<string, Collection<string>> indiceImmagini = [];

        private static readonly ConfrontoCI confrontoParole = new();
        private static readonly char[] separator = ['|'];

        #region costruttori

        /// <summary>
        /// The constructor of the class. It gives default values to all the members of the class, and looks for and analyses all the data files that it finds in default directory (the subdirectory LaParola of the system application data directory).
        /// If there are data files in other directories, the <see cref="AddDirectory" /> method needs to be called as well.
        /// </summary>
        public Texts()
        {
            CostruttoreComune();
            AggiungiDirectory(AppContext.BaseDirectory);
            try
            {
                AggiungiDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar);
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Il costruttore della classe. Dà valori predefiniti ai membri della classe, e cerca e analizza tutti i file dei dati che trova nella directory specificata.
        /// </summary>
        /// <param name="directory">La directory in cui cercare dei file dei dati.</param>
        /// <seealso cref="Texts"/>
        public Texts(string directory)
        {
            CostruttoreComune();
            // trovare e creare tutte le versioni disponibili
            AggiungiDirectory(directory);
        }

        private void CostruttoreComune()
        {
            formato = new FormatoTesto();
            libriNomi = LibriNomiInglese.Split('|');
            libriAbbreviazioniUsate = LibriAbbreviazioniUsateInglese.Split('|');
            libriAbbreviazioniRiconosciute = new LibriAbbreviazioniRiconosciuteHash();
            string[] abbreviazioniInglesi = LibriAbbreviazioniRiconosciuteInglese.Split('|');
            string[] abbreviazioni;
            for (byte i = 1; i <= 73; ++i)
            {
                abbreviazioni = abbreviazioniInglesi[i].Split(',');
                foreach (string abbreviazione in abbreviazioni)
                {
                    libriAbbreviazioniRiconosciute[abbreviazione] = i;
                }
            }

            versioni = [];
            indiceImmagini = [];
        }
        #endregion

        /// <summary>
        /// Il codice RTF da inserire all'inizio di ogni testo creato, con la formattazione delle opzioni.
        /// </summary>
        /// <returns>L'intestazione del codice RTF.</returns>
        public string RtfIntestazione()
        {
            string stileFont = "";
            if (formato.FontGrassetto)
            {
                stileFont += @"\b1";
            }

            if (formato.FontCorsivo)
            {
                stileFont += @"\i1";
            }

            if (formato.FontSottolineato)
            {
                stileFont += @"\u1";
            }

            //            return @"{\rtf1\ansi\ansicpg1252\deff0\deflang1040{\fonttbl{\f0\fnil\fcharset0 " + formato.FontNome + @";}{\f1\fnil\fcharset0 " + formato.FontRiferimentoNome + @";}{\f3\fnil\fcharset0 " + formato.FontGrecoNome + @";}{\f4\fnil\fcharset0 " + formato.FontEbraicoNome + @";}}"
            return @"{\rtf1\ansi\ansicpg1252\deff0\deflang1040{\fonttbl{\f0\fnil\fcharset0 " + formato.FontNome + @";}{\f1\fnil\fcharset0 " + formato.FontRiferimentoNome + @";}{\f3\fnil\fcharset0 " + formato.FontGrecoNome + @";}{\f4\fnil\fcharset0 " + formato.FontEbraicoNome + @";}}"
                + @"{\colortbl\red" + formato.FontColore.R.ToString(CultureInfo.InvariantCulture) + @"\green" + formato.FontColore.G.ToString(CultureInfo.InvariantCulture) + @"\blue" + formato.FontColore.B.ToString(CultureInfo.InvariantCulture)
                + @";\red" + formato.FontRiferimentoColore.R.ToString(CultureInfo.InvariantCulture) + @"\green" + formato.FontRiferimentoColore.G.ToString(CultureInfo.InvariantCulture) + @"\blue" + formato.FontRiferimentoColore.B.ToString(CultureInfo.InvariantCulture)
                + @";\red" + formato.FontRicercaColore.R.ToString(CultureInfo.InvariantCulture) + @"\green" + formato.FontRicercaColore.G.ToString(CultureInfo.InvariantCulture) + @"\blue" + formato.FontRicercaColore.B.ToString(CultureInfo.InvariantCulture)
                + @";\red" + formato.FontGrecoColore.R.ToString(CultureInfo.InvariantCulture) + @"\green" + formato.FontGrecoColore.G.ToString(CultureInfo.InvariantCulture) + @"\blue" + formato.FontGrecoColore.B.ToString(CultureInfo.InvariantCulture)
                + @";\red" + formato.FontEbraicoColore.R.ToString(CultureInfo.InvariantCulture) + @"\green" + formato.FontEbraicoColore.G.ToString(CultureInfo.InvariantCulture) + @"\blue" + formato.FontEbraicoColore.B.ToString(CultureInfo.InvariantCulture)
                + @";\red0\green128\blue0;\red0\green0\blue255;\red255\green0\blue0;}"
                + @"\viewkind4\uc1\pard" + stileFont + @"\cf0\f0\fs" + Convert.ToString(Convert.ToInt32(formato.FontDimensione * 2), CultureInfo.InvariantCulture) + " ";
        }

        #region aggiungere/rimuovere versioni

        /// <summary>
        /// Aggiunge tutti i testi trovati in una certa directory.
        /// </summary>
        /// <param name="directory">La cartella in cui cercare i file che contengono testi del programma.</param>
        public void AggiungiDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                directory += Path.DirectorySeparatorChar;
            }

            string[] fileTrovati;
            try
            {
                fileTrovati = Directory.GetFiles(directory, "*.laparola");
            }
            catch (UnauthorizedAccessException)
            {
                // non c'è l'autorizzazione di leggere quella directory. Saltiamola.
                return;
            }

            // byte testiInFile;

            foreach (string fileTrovato in fileTrovati)
            {
                try
                {
                    AggiungiTesto(fileTrovato, 0);
                }
                catch (FileNonValidoException)
                {
                    // se file non è valido, basta non aggiungerlo
                }

                /* non necessario mentre ogni file ha solo un testo
                FileStream fs = new FileStream(fileTrovato, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader br = new BinaryReader(fs);
                char[] c = br.ReadChars(3);
                try
                {
                    if (c[0].Equals('L') && c[1].Equals('P') && c[2].Equals('N'))
                    {
                        br.ReadByte(); // numero nomeVersione
                        br.ReadByte(); // numero nomeVersione
                        br.ReadByte(); // numero nomeVersione
                        testiInFile = br.ReadByte();
                        for (byte j = 0; j < testiInFile; ++j)
                        {
                            try
                            {
                                AggiungiTesto(fileTrovato, j);
                            }
                            catch (FileNonValidoException)
                            {
                                // se file non è valido, basta non aggiungerlo
                            }
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    // causato da un file con meno di 7 byte
                }
                 */
            }

            fileTrovati = Directory.GetFiles(directory, "*.image_link");
            XmlNode nodePrincipale, subNode;
            string fileImmagine, nome;
            foreach (string fileTrovato in fileTrovati)
            {
                try
                {
                    XmlDocument xd = new();
                    xd.Load(fileTrovato);
                    nodePrincipale = xd.SelectSingleNode("image");
                    subNode = nodePrincipale.SelectSingleNode("file");
                    fileImmagine = (subNode == null ? "" : directory + subNode.InnerText);
                    subNode = nodePrincipale.SelectSingleNode("links");
                    if (subNode != null && !string.IsNullOrEmpty(fileImmagine))
                    {
                        XmlNodeList nodeLink = subNode.SelectNodes("name");
                        foreach (XmlNode nodaLink in nodeLink)
                        {
                            nome = nodaLink.InnerText.ToLower(CultureInfo.InvariantCulture);
                            if (indiceImmagini.TryGetValue(nome, out Collection<string>? value))
                            {
                                value.Add(fileImmagine);
                            }
                            else
                            {
                                Collection<string> immaginiDellaParola =
                                [
                                    fileImmagine
                                ];
                                indiceImmagini.Add(nome, immaginiDellaParola);
                            }
                        }
                    }
                }
                catch
                {
                    // errore nell'XML, saltiamo il file
                }
            }
        }

        /// <summary>
        /// Chiude i testi, salvando eventuali note modificate.
        /// <returns>Un elenco di versioni, separate da spazi, di cui non è stato possibile salvare le modifiche.</returns>
        /// </summary>
        public string Chiudi()
        {
            string versioniNonSalvate = "";
            foreach (KeyValuePair<string, Versione> k in versioni)
            {
                try
                {
                    k.Value.Chiudi();
                }
                catch (ImpossibileScrivereModificheException)
                {
                    versioniNonSalvate += " " + k.Value.Info.Nome;
                }
            }

            return versioniNonSalvate;
        }

        /// <summary>
        /// Carica dei file dei testi informazioni sulle radici e sulle citazioni ai brani,
        /// che possono essere lette in un secondo momento dopo la creazione dell'oggetto Testi.
        /// </summary>
        public void CaricaInformazioniAddizionali()
        {
            foreach (string nomeVersione in NomiVersioni())
            {
                versioni[nomeVersione].CreaListaRadiceDiParole();
                versioni[nomeVersione].CreaListaCitazioni();
            }
        }

        /// <summary>
        /// Analizza un file dei dati e lo aggiunge all'elenco di quelli disponibili al programma.
        /// </summary>
        /// <param name="percorsoFile">Il percorso e nome del file dei dati.</param>
        /// <param name="testoInFile">Il numero (da 0) del testo nel file.</param>
        /// <returns>Il nome del testo aggiunto (stringa vuota se non è stato possibile)</returns>
        /// <exception cref="FileNonValidoException">Se c'è un errore nel file.</exception>
        public string AggiungiTesto(string percorsoFile, byte testoInFile)
        {
            Versione nuovaVersione = new(this, percorsoFile, testoInFile);
            // può dare FileNonValidoException
            string nomeTesto = nuovaVersione.Info.Nome;
            if (!string.IsNullOrEmpty(nomeTesto))
            {
                try
                {
                    versioni.Add(nomeTesto, nuovaVersione);
                }
                catch (ArgumentException) // il nome del testo già esiste
                {
                    nuovaVersione.Dispose();
                    nomeTesto = "";
                }
            }
            return nomeTesto;
        }


        /// <summary>
        /// Cancella il file che contiene un testo.
        /// </summary>
        /// <param name="nomeVersione">Il nome del testo da cancellare.</param>
        /// <exception cref="IOException">Il file del testo è ancora utilizzato (cioè è stato impossibile chiuderlo).</exception>
        /// <exception cref="UnauthorizedAccessException">All'utente non è permesso cancellare il file.</exception>
        public void CancellaTesto(string nomeVersione)
        {
            versioni[nomeVersione].Cancella();
            // se non è stato possibile cancellare, exception nella riga precedente e la versione non è rimossa dall'elenco nella prossima riga
            versioni.Remove(nomeVersione);
        }

        /// <summary>
        /// Rinomina il testo in un file.
        /// </summary>
        /// <param name="nomeVersione">Il nome del testo da rinominare.</param>
        /// <param name="nuovoNome">Il nuovo nome del testo.</param>
        public void RinominaTesto(string nomeVersione, string nuovoNome)
        {
            string fileTemp = Path.GetTempFileName();
            File.Delete(fileTemp);
            CopiaTesto(nomeVersione, nuovoNome, fileTemp);
            versioni[nuovoNome].Rimuovi();
            versioni.Remove(nuovoNome);
            string fileNome = versioni[nomeVersione].Info.NomeDelFile;
            CancellaTesto(nomeVersione);
            if (File.Exists(fileNome))
            {
                File.Delete(fileNome);
            }

            File.Move(fileTemp, fileNome);
            AggiungiTesto(fileNome, 0);
        }

        /// <summary>
        /// Copia il file che contiene un testo.
        /// </summary>
        /// <param name="nomeVersione">Il nome del testo da copiare.</param>
        /// <param name="nuovoNomeTesto">Il nome del nuovo testo.</param>
        /// <param name="nuovoNomeFile">Il nome del file a cui sarà copiato (con (1), (2), ecc. aggiunto se il file esiste già).</param>
        /// <returns>Il nome del testo che è stato creato.</returns>
        public string CopiaTesto(string nomeVersione, string nuovoNomeTesto, string nuovoNomeFile)
        {
            string nomeNonEsistente = nuovoNomeFile;
            int count = 0;
            while (File.Exists(nomeNonEsistente))
            {
                ++count;
                nomeNonEsistente = Path.GetDirectoryName(nuovoNomeFile) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(nuovoNomeFile) + " (" + count.ToString(CultureInfo.InvariantCulture) + ")" + Path.GetExtension(nuovoNomeFile);
            }

            FileStream fsRead = null, fsWrite = null;
            BinaryReader br = null;
            BinaryWriter bw = null;
            try
            {
                fsRead = new FileStream(versioni[nomeVersione].Info.NomeDelFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                br = new BinaryReader(fsRead);
                fsWrite = new FileStream(nomeNonEsistente, FileMode.Create, FileAccess.Write, FileShare.None);
                bw = new BinaryWriter(fsWrite);

                bw.Write(br.ReadBytes(6));
                byte numeroTesti = br.ReadByte();
                bw.Write(numeroTesti);
                for (int i = 1; i <= numeroTesti; ++i)
                {
                    bw.Write(br.ReadUInt32());
                }

                int posizione = (int)(br.ReadUInt32()) + nuovoNomeTesto.Length - nomeVersione.Length;
                bw.Write((UInt32)posizione);
                br.ReadString(); // il vecchio nome non ci interessa
                bw.Write(nuovoNomeTesto);
                bw.Write(br.ReadBytes((int)(fsRead.Length - fsRead.Position)));
            }
            finally
            {
                br?.Close();
                fsRead?.Close();
                bw?.Close();
                fsWrite?.Close();
            }

            AggiungiTesto(nomeNonEsistente, 0);
            return nuovoNomeTesto;
        }

        /// <summary>
        /// Tutti i file dei dati attualmente disponibili.
        /// </summary>
        /// <returns>Una collezione di stringhe con i nomi di tutte le versioni disponibili.</returns>
        public Collection<string> NomiVersioni()
        {
            List<string> nomiVersioni = [];
            foreach (Versione v in versioni.Values)
            {
                nomiVersioni.Add(v.Info.Nome);
            }

            nomiVersioni.Sort();
            return new Collection<string>(nomiVersioni);
        }

        /// <summary>
        /// Tutti i file dei dati attualmente disponibili che contengono almeno uno di certi tipi di testo.
        /// </summary>
        /// <param name="tipo">Il tipo di testo da cercare.</param>
        /// <returns>Una collezione di stringhe con i nomi di tutte le versioni del tipo giusto disponibili.</returns>
        /// <seealso cref="TestoTipi"/>
        public Collection<string> NomiVersioni(TestoTipi tipo)
        {
            return NomiVersioni(tipo, true);
        }

        /// <summary>
        /// Tutti i file dei dati attualmente disponibili che contengono certi tipi di testo.
        /// </summary>
        /// <param name="tipo">Il tipo di testo da cercare.</param>
        /// <param name="almenoUno">Se è vero (valore predefinito), almeno uno dei tipi in "tipo" deve essere presente nel testo; se è falso, tutti i tipi devono essere presenti.</param>
        /// <returns>Una collezione di stringhe con i nomi di tutte le versioni del tipo giusto disponibili.</returns>
        /// <seealso cref="TestoTipi"/>
        public Collection<string> NomiVersioni(TestoTipi tipo, bool almenoUno)
        {
            List<string> nomiVersioni = [];
            foreach (Versione v in versioni.Values)
            {
                if (almenoUno)
                {
                    if ((v.Info.Tipo & tipo) != 0)
                    {
                        nomiVersioni.Add(v.Info.Nome);
                    }
                }
                else
                {
                    if ((v.Info.Tipo & tipo) == tipo)
                    {
                        nomiVersioni.Add(v.Info.Nome);
                    }
                }
            }
            nomiVersioni.Sort();
            return new Collection<string>(nomiVersioni);
        }

        /// <summary>
        /// Informazioni su un file dei dati.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione nel file dei dati.</param>
        /// <returns>Informazioni sulla versione. Se la versione non esiste, tutti i campi delle informazioni sono vuoti.</returns>
        public VersioneInformazioni Info(string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].Info;
            }
            catch
            {
                VersioneInformazioni vi = new();
                return vi;
            }
        }

        /// <summary>
        /// Come il testo va mostrato nella finestra di visualizzazione, quando il tipo non è specificato.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione nel file dei dati.</param>
        /// <returns>Bibbia se è una Bibbia, Commentario se contiene un commentario, Dizionario se non è un commentario.</returns>
        public TestoTipi TipoPrincipaleDiTesto(string nomeVersione)
        {
            TestoTipi tipo = TestoTipi.None;
            if ((Info(nomeVersione).Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
            {
                tipo = TestoTipi.Bibbia;
            }
            else if ((Info(nomeVersione).Tipo & TestoTipi.Commentario) == TestoTipi.Commentario)
            {
                tipo = TestoTipi.Commentario;
            }
            else if ((Info(nomeVersione).Tipo & TestoTipi.Dizionario) == TestoTipi.Dizionario)
            {
                tipo = TestoTipi.Dizionario;
            }
            return tipo;
        }

        /// <summary>
        /// Cambia lo stato di una collezione di note da sola lettura a non sola lettura, o il contrario.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione nel file dei dati.</param>
        public void CambiaSolaLettura(string nomeVersione)
        {
            try
            {
                versioni[nomeVersione].CambiaSolaLettura();
            }
            catch { }
        }

        /// <summary>
        /// Trova il nome del testo che ha una certa abbreviazione.
        /// </summary>
        /// <param name="abbreviazione">L'abbreviazione da cercare.</param>
        /// <returns>Il nome della versione, o una stringa vuota se non è stata trovata.</returns>
        public string VersioneDaAbbreviazione(string abbreviazione)
        {
            string abbreviazioneLC = abbreviazione.ToUpper(CultureInfo.InvariantCulture);
            foreach (KeyValuePair<string, Versione> kvp in versioni)
            {
                if (kvp.Value.Info.Abbreviazione.ToUpper(CultureInfo.InvariantCulture) == abbreviazioneLC)
                {
                    return kvp.Key;
                }
            }
            return "";
        }

        /// <summary>
        /// Se un testo con un certo nome esiste.
        /// </summary>
        /// <param name="nomeVersione">Il nome del testo da cercare.</param>
        /// <returns>Vero se un testo esiste con il nome.</returns>
        public bool VersioneEsiste(string nomeVersione)
        {
            // tested
            foreach (String nome in versioni.Keys)
            {
                if (nome == nomeVersione)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Ricerca

        /// <summary>
        /// I versetti che contengono una certa espressione.
        /// </summary>
        /// <param name="espressione">L'espressione da ricercare.</param>
        /// <param name="brano">Il brano in cui ricercare l'espressione.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il riferimento dei versetti.</returns>
        /// <exception cref="SearchExpressionEmptyException">L'espressione da ricercare era vuota.</exception>
        /// <exception cref="SearchSyntaxErrorException">Un errore di sintassi al carattere dato dal numero dopo 'sintassi'.</exception>
        /// <exception cref="SearchParenthesesException">Un errore nelle parentesi al carattere dato dal numero dopo 'parentesi'.</exception>
        /// <exception cref="SearchBracketsException">Un errore nelle parentesi quadrate al carattere dato dal numero dopo 'quadrate'.</exception>
        public Riferimento Ricerca(string espressione, string brano, string nomeVersione)
        {
            return Ricerca(espressione, ConvertiRiferimento(brano), nomeVersione);
        }

        /// <summary>
        /// I versetti che contengono una certa espressione in tutta la Bibbia.
        /// </summary>
        /// <param name="espressione">L'espressione da ricercare.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il riferimento dei versetti.</returns>
        /// <exception cref="SearchExpressionEmptyException">L'espressione da ricercare era vuota.</exception>
        /// <exception cref="SearchSyntaxErrorException">Un errore di sintassi al carattere dato dal numero dopo 'sintassi'.</exception>
        /// <exception cref="SearchParenthesesException">Un errore nelle parentesi al carattere dato dal numero dopo 'parentesi'.</exception>
        /// <exception cref="SearchBracketsException">Un errore nelle parentesi quadrate al carattere dato dal numero dopo 'quadrate'.</exception>
        public Riferimento Ricerca(string espressione, string nomeVersione)
        {
            // tested
            Riferimento branoDaRicercare = new(); // un riferimento vuoto, usato per indicare tutta la Bibbia
            return Ricerca(espressione, branoDaRicercare, nomeVersione);
        }

        /// <summary>
        /// I versetti che contengono una certa espressione.
        /// </summary>
        /// <param name="espressione">L'espressione da ricercare.</param>
        /// <param name="riferimentoDaRicercare">Il riferimento del brano in cui ricercare l'espressione.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il riferimento dei versetti.</returns>
        /// <exception cref="SearchExpressionEmptyException">L'espressione da ricercare era vuota.</exception>
        /// <exception cref="SearchSyntaxErrorException">Un errore di sintassi al carattere dato dal numero dopo 'sintassi'.</exception>
        /// <exception cref="SearchParenthesesException">Un errore nelle parentesi al carattere dato dal numero dopo 'parentesi'.</exception>
        /// <exception cref="SearchBracketsException">Un errore nelle parentesi quadrate al carattere dato dal numero dopo 'quadrate'.</exception>
        public Riferimento Ricerca(string espressione, Riferimento riferimentoDaRicercare, string nomeVersione)
        {
            // tested
            espressione = ControllaEspressioneDaRicercare(espressione, nomeVersione);
            Riferimento versettiTrovati = TrovaOccorrenzeEspressione(espressione, riferimentoDaRicercare, false, 0, nomeVersione);
            versettiTrovati = UnisciVociRipetute(versettiTrovati);
            return versettiTrovati;
        }

        /// <summary>
        /// I versetti che contengono una certa parola in un certo brano.
        /// Questa funzione può essere usata invece di Ricerca per ricercare un numero.
        /// Restituisce un versetto due volte se la parola appare due volte.
        /// </summary>
        /// <param name="parola">La parola da ricercare.</param>
        /// <param name="brano">Il brano in cui ricercare la parola.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il riferimento dei versetti.</returns>
        public Riferimento RicercaParolaInBrano(string parola, Riferimento brano, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].RicercaParolaInBrano(parola, brano);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// I versetti che contengono una certa radice.
        /// Restituisce un versetto due volte se la radice appare due volte.
        /// </summary>
        /// <param name="radice">La radice da ricercare.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il riferimento dei versetti.</returns>
        public Riferimento RicercaRadiceInBrano(string radice, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].RicercaRadiceInBrano(radice);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// I versetti  in un certo branoche contengono una certa radice.
        /// Restituisce un versetto due volte se la radice appare due volte.
        /// </summary>
        /// <param name="radice">La radice da ricercare.</param>
        /// <param name="brano">Il brano in cui ricercare la radice.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il riferimento dei versetti.</returns>
        public Riferimento RicercaRadiceInBrano(string radice, Riferimento brano, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].RicercaRadiceInBrano(radice, brano);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        private static Riferimento UnisciVociRipetute(Riferimento riferimento)
        {
            if (riferimento.Versetti)
            {
                int nVersetti = riferimento.Brani.Count;
                for (int i = nVersetti - 1; i > 0; --i)
                {
                    if (riferimento.PrimoVersettoUguale(i - 1, i))
                    {
                        riferimento.numeroParola[i - 1].AddRange(riferimento.numeroParola[i]);
                        riferimento.Brani.RemoveAt(i);
                        riferimento.numeroParola.RemoveAt(i);
                    }
                    else
                    {
                        riferimento.numeroParola[i].Sort();
                    }
                }
                if (nVersetti > 0)
                {
                    riferimento.numeroParola[0].Sort();
                }
            }
            else
            {
                int nNote = riferimento.Note.Count;
                for (int i = nNote - 1; i > 0; --i)
                {
                    if (riferimento.Note[i - 1] == riferimento.Note[i])
                    {
                        riferimento.numeroParola[i - 1].AddRange(riferimento.numeroParola[i]);
                        riferimento.Note.RemoveAt(i);
                        riferimento.numeroParola.RemoveAt(i);
                    }
                    else
                    {
                        riferimento.numeroParola[i].Sort();
                    }
                }
                if (nNote > 0)
                {
                    riferimento.numeroParola[0].Sort();
                }
            }
            return riferimento;
        }

        private Riferimento TrovaOccorrenzeEspressione(string espressione, Riferimento branoDaRicercare, bool inFrase, int nParoleInFrase, string nomeVersione)
        {
            // se branoDaRicerca non contiene brani, tutta la Bibbia (oppure tutta la collezione di note) è ricercata
            string espressioneDaTrovare = espressione;
            espressioneDaTrovare += "\x00";
            Riferimento riferimenti = new();
            string tipoOperazione;
            while (espressioneDaTrovare != "\x00")
            {
                char primoCarattere = espressioneDaTrovare[0];
                if (primoCarattere == '~')
                {
                    primoCarattere = '0';
                    espressioneDaTrovare = "0" + espressioneDaTrovare;
                }
                if (Char.IsDigit(primoCarattere) || primoCarattere == ':')
                {
                    if (Char.IsDigit(espressioneDaTrovare[1]))
                    {
                        tipoOperazione = "prima";
                    }
                    else
                    {
                        tipoOperazione = espressioneDaTrovare[..1];
                        espressioneDaTrovare = espressioneDaTrovare[1..];
                        if (espressioneDaTrovare[0] == '~')
                        {
                            tipoOperazione += "n";
                            espressioneDaTrovare = espressioneDaTrovare[1..];
                        }
                    }
                }
                else
                {
                    if (primoCarattere == '|')
                    {
                        tipoOperazione = "oppure";
                        espressioneDaTrovare = espressioneDaTrovare[1..];
                    }
                    else
                    {
                        tipoOperazione = "prima";
                    }
                } // if ((IsNumero(cPrimoCarattere)) || cPrimoCarattere==':') else

                int i;
                Riferimento occorrenzeProssimaParola = new();
                primoCarattere = espressioneDaTrovare[0];
                if (primoCarattere == '(')
                {
                    i = 0;
                    int nParentesi = 1;
                    do
                    {
                        ++i;
                        if (espressioneDaTrovare[i] == ')')
                        {
                            --nParentesi;
                        }

                        if (espressioneDaTrovare[i] == '(')
                        {
                            ++nParentesi;
                        }
                    } while (nParentesi != 0);
                    occorrenzeProssimaParola = TrovaOccorrenzeEspressione(espressioneDaTrovare[1..i], branoDaRicercare, false, nParoleInFrase, nomeVersione);
                    espressioneDaTrovare = espressioneDaTrovare[(i + 1)..];
                }
                else if (primoCarattere == '[')
                {
                    i = espressioneDaTrovare.IndexOf(']');
                    nParoleInFrase = 0;
                    occorrenzeProssimaParola = TrovaOccorrenzeEspressione(espressioneDaTrovare[1..i], branoDaRicercare, true, nParoleInFrase, nomeVersione);
                    espressioneDaTrovare = espressioneDaTrovare[(i + 1)..];
                }
                else
                {
                    string parola = ProssimaParola(espressioneDaTrovare, 0);
                    occorrenzeProssimaParola = versioni[nomeVersione].RicercaParolaInBrano(parola, branoDaRicercare);
                    int lunghezzaExtra = (espressioneDaTrovare[0] == '<' ? 1 : 0);
                    if (lunghezzaExtra == 1 && espressioneDaTrovare.Contains('>'))
                    {
                        ++lunghezzaExtra;
                    }

                    espressioneDaTrovare = espressioneDaTrovare[(parola.Length + lunghezzaExtra)..];
                    ++nParoleInFrase;
                } // if (cPrimoCarattere=='(') else

                Riferimento occorrenzeInBrano = new();
                if (tipoOperazione == "prima")
                {
                    riferimenti = occorrenzeProssimaParola;
                }
                else
                {
                    primoCarattere = tipoOperazione[0];
                    if (Char.IsDigit(primoCarattere) || primoCarattere == ':')
                    {
                        int primoCarattereComeNumero;
                        if (primoCarattere == ':')
                        {
                            primoCarattereComeNumero = int.MaxValue / 2; // "/2" altrimenti quando si aggiunge un numero ad esso, diventa negativo
                        }
                        else
                        {
                            primoCarattereComeNumero = Convert.ToInt32(primoCarattere, CultureInfo.InvariantCulture) - 48; // '0' come carattere è ASCII 48
                        }

                        if (!inFrase || tipoOperazione.Length == 1)
                        {
                            if (riferimenti.Versetti)
                            {
                                int j = i = 1;
                                int nI = riferimenti.Count;
                                int nJ = occorrenzeProssimaParola.Count;
                                int nVersettoRiferimenti = (nI > 0 ? (int)(VersettiFinoACapitolo(riferimenti.Brani[i - 1][0], (byte)(riferimenti.Brani[i - 1][1] - 1), nomeVersione) + riferimenti.Brani[i - 1][2]) : 0);
                                int nVersettoOccorrenze = (nJ > 0 ? (int)(VersettiFinoACapitolo(occorrenzeProssimaParola.Brani[j - 1][0], (byte)(occorrenzeProssimaParola.Brani[j - 1][1] - 1), nomeVersione) + occorrenzeProssimaParola.Brani[j - 1][2]) : 0);
                                while (i <= nI && j <= nJ)
                                {
                                    if (inFrase)
                                    {
                                        if (nVersettoOccorrenze < nVersettoRiferimenti || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0]))
                                        {
                                            ++j;
                                            if (j <= nJ)
                                            {
                                                nVersettoOccorrenze = (int)(VersettiFinoACapitolo(occorrenzeProssimaParola.Brani[j - 1][0], (byte)(occorrenzeProssimaParola.Brani[j - 1][1] - 1), nomeVersione) + occorrenzeProssimaParola.Brani[j - 1][2]);
                                            }
                                        }
                                        else
                                        {
                                            if (nVersettoOccorrenze > nVersettoRiferimenti || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] > riferimenti.numeroParola[i - 1][0] + primoCarattereComeNumero + 1))
                                            {
                                                if (tipoOperazione.Length > 1)
                                                {
                                                    occorrenzeInBrano.Brani.Add(riferimenti.Brani[i - 1]);
                                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                }
                                            }
                                            else
                                            {
                                                if (tipoOperazione.Length == 1)
                                                {
                                                    occorrenzeInBrano.Brani.Add(riferimenti.Brani[i - 1]);
                                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                    occorrenzeInBrano.numeroParola[^1].Insert(0, occorrenzeProssimaParola.numeroParola[j - 1][0]);
                                                }
                                            }
                                            ++i;
                                            if (i <= nI)
                                            {
                                                nVersettoRiferimenti = (int)(VersettiFinoACapitolo(riferimenti.Brani[i - 1][0], (byte)(riferimenti.Brani[i - 1][1] - 1), nomeVersione) + riferimenti.Brani[i - 1][2]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (nVersettoOccorrenze < nVersettoRiferimenti - primoCarattereComeNumero)
                                        {
                                            ++j;
                                            if (j <= nJ)
                                            {
                                                nVersettoOccorrenze = (int)(VersettiFinoACapitolo(occorrenzeProssimaParola.Brani[j - 1][0], (byte)(occorrenzeProssimaParola.Brani[j - 1][1] - 1), nomeVersione) + occorrenzeProssimaParola.Brani[j - 1][2]);
                                            }
                                        }
                                        else
                                        {
                                            if (nVersettoOccorrenze > nVersettoRiferimenti + primoCarattereComeNumero)
                                            {
                                                if (tipoOperazione.Length > 1)
                                                {
                                                    occorrenzeInBrano.Brani.Add(riferimenti.Brani[i - 1]);
                                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                }
                                            }
                                            else
                                            {
                                                if (tipoOperazione.Length == 1)
                                                {
                                                    occorrenzeInBrano.Brani.Add(riferimenti.Brani[i - 1]);
                                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                    if (primoCarattereComeNumero == 0)
                                                    { // seconda parola nel versetto anche, quindi va sottolineata
                                                        occorrenzeInBrano.numeroParola[^1].Insert(0, occorrenzeProssimaParola.numeroParola[j - 1][0]);
                                                        while (j < nJ && occorrenzeProssimaParola.PrimoVersettoUguale(j - 1, j))
                                                        {
                                                            occorrenzeInBrano.numeroParola[^1].Add(occorrenzeProssimaParola.numeroParola[j][0]);
                                                            ++j;
                                                        }
                                                    }
                                                }
                                            }
                                            ++i;
                                            if (i <= nI)
                                            {
                                                nVersettoRiferimenti = (int)(VersettiFinoACapitolo(riferimenti.Brani[i - 1][0], (byte)(riferimenti.Brani[i - 1][1] - 1), nomeVersione) + riferimenti.Brani[i - 1][2]);
                                            }
                                        }
                                    }
                                } // while (i <= riferimenti.Count && j <= occorrenzeProssimaParola.Count)
                                if (tipoOperazione.Length > 1)
                                {
                                    while (i <= riferimenti.Count)
                                    {
                                        occorrenzeInBrano.Brani.Add(riferimenti.Brani[i - 1]);
                                        occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                        ++i;
                                    }
                                }
                                riferimenti = occorrenzeInBrano;
                            }
                            else // if (riferimenti.Versetti)
                            {
                                occorrenzeInBrano.Versetti = false;
                                int j = i = 1;
                                int nI = riferimenti.Count;
                                int nJ = occorrenzeProssimaParola.Count;
                                string notaRiferimenti = (nI > 0 ? riferimenti.Note[i - 1] : "");
                                string notaOccorrenze = (nJ > 0 ? occorrenzeProssimaParola.Note[j - 1] : "");
                                int differenzaVersetti = -1;
                                int differenzaRicercata = primoCarattereComeNumero;
                                while (i <= nI && j <= nJ)
                                {
                                    if (inFrase)
                                    {
                                        if (confrontoParole.Compare(notaOccorrenze, notaRiferimenti) < 0 || (notaOccorrenze == notaRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0]))
                                        {
                                            ++j;
                                            if (j <= nJ)
                                            {
                                                notaOccorrenze = occorrenzeProssimaParola.Note[j - 1];
                                            }
                                        }
                                        else
                                        {
                                            if (confrontoParole.Compare(notaOccorrenze, notaRiferimenti) > 0 || (notaOccorrenze == notaRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] > riferimenti.numeroParola[i - 1][0] + primoCarattereComeNumero + 1))
                                            {
                                                if (tipoOperazione.Length > 1)
                                                {
                                                    occorrenzeInBrano.Note.Add(riferimenti.Note[i - 1]);
                                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                }
                                            }
                                            else
                                            {
                                                if (tipoOperazione.Length == 1)
                                                {
                                                    occorrenzeInBrano.Note.Add(riferimenti.Note[i - 1]);
                                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                    occorrenzeInBrano.numeroParola[^1].Insert(0, occorrenzeProssimaParola.numeroParola[j - 1][0]);
                                                }
                                            }
                                            ++i;
                                            if (i <= nI)
                                            {
                                                notaRiferimenti = riferimenti.Note[i - 1];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CalcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze, ref differenzaVersetti, ref differenzaRicercata);
                                        if (differenzaVersetti < -differenzaRicercata) // cioè string.Compare(notaOccorrenze, notaRiferimenti) < 0 per due note quando una non è ad un brano
                                        {
                                            ++j;
                                            if (j <= nJ)
                                            {
                                                notaOccorrenze = occorrenzeProssimaParola.Note[j - 1];
                                            }
                                        }
                                        else
                                        {
                                            if (differenzaVersetti > differenzaRicercata) // cioè string.Compare(notaOccorrenze, notaRiferimenti) > 0 per due note quando una non è ad un brano
                                            {
                                                if (tipoOperazione.Length > 1)
                                                {
                                                    occorrenzeInBrano.Note.Add(notaRiferimenti);
                                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                }
                                            }
                                            else
                                            {
                                                if (tipoOperazione.Length == 1)
                                                {
                                                    if (occorrenzeInBrano.Note.Count > 0 && notaRiferimenti == occorrenzeInBrano.Note[^1])
                                                    {
                                                        occorrenzeInBrano.numeroParola[^1].Insert(0, riferimenti.numeroParola[i - 1][0]);
                                                    }
                                                    else
                                                    {
                                                        occorrenzeInBrano.Note.Add(notaRiferimenti);
                                                        occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                                        if (differenzaRicercata == 0) // seconda parola nel versetto anche, quindi va sottolineata
                                                        {
                                                            occorrenzeInBrano.numeroParola[^1].Insert(0, occorrenzeProssimaParola.numeroParola[j - 1][0]);
                                                        }
                                                    }
                                                    ++j;
                                                    if (j <= nJ)
                                                    {
                                                        notaOccorrenze = occorrenzeProssimaParola.Note[j - 1];
                                                        CalcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze, ref differenzaVersetti, ref differenzaRicercata);
                                                    }
                                                    while (Math.Abs(differenzaVersetti) <= differenzaRicercata && j <= nJ)
                                                    {
                                                        occorrenzeInBrano.numeroParola[^1].Insert(0, occorrenzeProssimaParola.numeroParola[j - 1][0]);
                                                        ++j;
                                                        if (j <= nJ)
                                                        {
                                                            notaOccorrenze = occorrenzeProssimaParola.Note[j - 1];
                                                            CalcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze, ref differenzaVersetti, ref differenzaRicercata);
                                                        }
                                                    }
                                                    while (i < nI && riferimenti.Note[i - 1] == riferimenti.Note[i])
                                                    {
                                                        ++i;
                                                        occorrenzeInBrano.numeroParola[^1].Insert(0, riferimenti.numeroParola[i - 1][0]);
                                                    }
                                                }
                                            }
                                            ++i;
                                            if (i <= nI)
                                            {
                                                notaRiferimenti = riferimenti.Note[i - 1];
                                            }
                                        }
                                    }
                                } // while (i <= nI && j <= nJ)
                                if (tipoOperazione.Length > 1)
                                {
                                    while (i <= riferimenti.Count)
                                    {
                                        occorrenzeInBrano.Note.Add(riferimenti.Note[i - 1]);
                                        occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                        ++i;
                                    }
                                }
                                riferimenti = occorrenzeInBrano;
                            } // if (riferimenti.Versetti) else
                        } // if (!inFrase || tipoOper.Length == 1)
                    } // if (Char.IsDigit(primoCarattere)) {
                    else
                    {
                        if (primoCarattere == 'o')
                        {
                            if (riferimenti.Versetti)
                            {
                                int j = i = 1;
                                if (riferimenti.Count > 0 && occorrenzeProssimaParola.Count > 0)
                                {
                                    UInt32 nVersettoRiferimenti = VersettiFinoACapitolo(riferimenti.Brani[i - 1][0], (byte)(riferimenti.Brani[i - 1][1] - 1), nomeVersione) + riferimenti.Brani[i - 1][2];
                                    UInt32 nVersettoOccorrenze = VersettiFinoACapitolo(occorrenzeProssimaParola.Brani[j - 1][0], (byte)(occorrenzeProssimaParola.Brani[j - 1][1] - 1), nomeVersione) + occorrenzeProssimaParola.Brani[j - 1][2];
                                    int nI = riferimenti.Count;
                                    int nJ = occorrenzeProssimaParola.Count;
                                    while (i <= nI && j <= nJ)
                                    {
                                        if (nVersettoOccorrenze < nVersettoRiferimenti || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0]))
                                        {
                                            occorrenzeInBrano.Brani.Add(occorrenzeProssimaParola.Brani[j - 1]);
                                            occorrenzeInBrano.numeroParola.Add(occorrenzeProssimaParola.numeroParola[j - 1]);
                                            ++j;
                                            if (j <= nJ)
                                            {
                                                nVersettoOccorrenze = VersettiFinoACapitolo(occorrenzeProssimaParola.Brani[j - 1][0], (byte)(occorrenzeProssimaParola.Brani[j - 1][1] - 1), nomeVersione) + occorrenzeProssimaParola.Brani[j - 1][2];
                                            }
                                        }
                                        else
                                        {
                                            occorrenzeInBrano.Brani.Add(riferimenti.Brani[i - 1]);
                                            occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                            ++i;
                                            if (i <= nI)
                                            {
                                                nVersettoRiferimenti = VersettiFinoACapitolo(riferimenti.Brani[i - 1][0], (byte)(riferimenti.Brani[i - 1][1] - 1), nomeVersione) + riferimenti.Brani[i - 1][2];
                                            }
                                        }
                                    } // while
                                }
                                while (j <= occorrenzeProssimaParola.Count)
                                {
                                    occorrenzeInBrano.Brani.Add(occorrenzeProssimaParola.Brani[j - 1]);
                                    occorrenzeInBrano.numeroParola.Add(occorrenzeProssimaParola.numeroParola[j - 1]);
                                    ++j;
                                }
                                while (i <= riferimenti.Count)
                                {
                                    occorrenzeInBrano.Brani.Add(riferimenti.Brani[i - 1]);
                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                    ++i;
                                }
                                riferimenti = occorrenzeInBrano;
                            }
                            else // collezioni di note
                            {
                                occorrenzeInBrano.Versetti = false;
                                int j = i = 1;
                                int nI = riferimenti.Count;
                                int nJ = occorrenzeProssimaParola.Count;
                                string notaRiferimenti = (nI > 0 ? riferimenti.Note[i - 1] : "");
                                string notaOccorrenze = (nJ > 0 ? occorrenzeProssimaParola.Note[j - 1] : "");
                                while (i <= nI && j <= nJ)
                                {
                                    if (confrontoParole.Compare(notaOccorrenze, notaRiferimenti) < 0 || (notaOccorrenze == notaRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0]))
                                    {
                                        occorrenzeInBrano.Note.Add(occorrenzeProssimaParola.Note[j - 1]);
                                        occorrenzeInBrano.numeroParola.Add(occorrenzeProssimaParola.numeroParola[j - 1]);
                                        ++j;
                                        if (j < nJ)
                                        {
                                            notaOccorrenze = occorrenzeProssimaParola.Note[j - 1];
                                        }
                                    }
                                    else
                                    {
                                        occorrenzeInBrano.Note.Add(riferimenti.Note[i - 1]);
                                        occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                        ++i;
                                        if (i < nI)
                                        {
                                            notaRiferimenti = riferimenti.Note[i - 1];
                                        }
                                    }
                                } // while
                                while (j <= occorrenzeProssimaParola.Count)
                                {
                                    occorrenzeInBrano.Note.Add(occorrenzeProssimaParola.Note[j - 1]);
                                    occorrenzeInBrano.numeroParola.Add(occorrenzeProssimaParola.numeroParola[j - 1]);
                                    ++j;
                                }
                                while (i <= riferimenti.Count)
                                {
                                    occorrenzeInBrano.Note.Add(riferimenti.Note[i - 1]);
                                    occorrenzeInBrano.numeroParola.Add(riferimenti.numeroParola[i - 1]);
                                    ++i;
                                }
                                riferimenti = occorrenzeInBrano;
                            }
                        }
                    } // if (Char.IsDigit(primoCarattere)) else
                }
            }

            return riferimenti;
        }

        private void CalcolaDifferenzeDelleNote(int primoCarattereComeNumero, string notaRiferimenti, string notaOccorrenze, ref int differenzaVersetti, ref int differenzaRicercata)
        {
            differenzaVersetti = -1;
            differenzaRicercata = primoCarattereComeNumero;
            if (notaOccorrenze.StartsWith('#') && notaRiferimenti.StartsWith('#'))
            {
                try
                {
                    differenzaVersetti = (int)(VersettiFinoACapitolo(Convert.ToByte(notaOccorrenze.Substring(1, 2), CultureInfo.InvariantCulture), Convert.ToByte(notaOccorrenze.Substring(3, 3), CultureInfo.InvariantCulture))
                        + Convert.ToInt32(notaOccorrenze.Substring(6, 3), CultureInfo.InvariantCulture)
                        - VersettiFinoACapitolo(Convert.ToByte(notaRiferimenti.Substring(1, 2), CultureInfo.InvariantCulture), Convert.ToByte(notaRiferimenti.Substring(3, 3), CultureInfo.InvariantCulture))
                        - Convert.ToInt32(notaRiferimenti.Substring(6, 3), CultureInfo.InvariantCulture));
                }
                catch { }
            }
            else
            {
                differenzaVersetti = confrontoParole.Compare(notaOccorrenze, notaRiferimenti);
                differenzaRicercata = 0;
            }
        }

        private string ControllaEspressioneDaRicercare(string espressione, string nomeVersione)
        {
            espressione = espressione.Trim();
            if (string.IsNullOrEmpty(espressione))
            {
                throw new SearchExpressionEmptyException();
            }

            int nParentesiSinistra = 0, nParentesiDestra = 0, nParentesiQuadrateSinistra = 0;
            int erroreSintassi = -1;
            bool erroreParentesi = false, erroreParentesiQuadrate = false;
            char a, b, c;

            espressione = espressione.ToLower(CultureInfo.CurrentCulture);

            if (Array.IndexOf(SplitString(versioni[nomeVersione].Info.Lingua.ToLower(CultureInfo.InvariantCulture), '|'), "it") >= 0)
            {
                int p = 0;
                while (espressione.IndexOf('\'', p + 1) > -1)
                {
                    p = espressione.IndexOf('\'', p + 1);
                    if (p < espressione.Length - 1 && (IsLettera(espressione[p + 1]) || espressione[p + 1] == '*' || espressione[p + 1] == '?'))
                    {
                        espressione = espressione.Insert(p + 1, " ");
                    }
                }
            }

            espressione = espressione.Replace(' ', ' '); // il primo spazio è il carattere xA0 (spazio unificatore), il secondo x20 (spazio normale)
            espressione = espressione.Replace('^', '~');
            espressione = espressione.Replace('!', '|');
            char prossimaParentesiQuadrate = '[';
            while (espressione.Contains('"'))
            {
                espressione = espressione[..espressione.IndexOf('"')] + prossimaParentesiQuadrate + espressione[(espressione.IndexOf('"') + 1)..];
                prossimaParentesiQuadrate = (prossimaParentesiQuadrate == '[' ? ']' : '[');
            }

            for (int i = 0; i < espressione.Length - 1; ++i)
            {
                c = espressione[i];
                if (c == ' ' && i > 1)
                {
                    a = espressione[i - 1];
                    b = espressione[i + 1];
                    if ((a < 'a' && a != '\'' && a != '-' && a != ']' && a != ')' && !(IsLettera(a) || a == '*' || a == '?')) || a == '~' || a == '|' || a == ':' || a == '<' || a == '>' || (b < 'a' && b != '\'' && b != '-' && b != '(' && b != '[' && !(IsLettera(b) || b == '*' || b == '?')) || b == '~' || b == '|' || b == ':' || b == '<' || a == '>')
                    {
                        espressione = espressione.Remove(i, 1);
                        --i;
                    }
                }
            }

            for (int i = 0; i < espressione.Length; ++i)
            {
                c = espressione[i];
                if (i == 0)
                {
                    if (c == '(')
                    {
                        ++nParentesiSinistra;
                    }
                    else if (c == '[')
                    {
                        ++nParentesiQuadrateSinistra;
                    }
                    else if (c == '<')
                    {
                        int nuovoI = espressione.IndexOf('>', i);
                        if (nuovoI > i)
                        {
                            i = nuovoI;
                        }
                        else
                        {
                            erroreSintassi = i;
                        }
                    }
                    else if (!(IsLettera(c) || c == '\'' || c == '/' || c == '\\' || c == '*' || c == '?'))
                    {
                        erroreSintassi = i;
                    }
                }
                else
                {
                    a = espressione[i - 1];
                    if (c == ' ')
                    {
                        espressione = espressione.Remove(i, 1);
                        espressione = espressione.Insert(i, "0");
                    }
                    else if (c == '-' || c == '\'')
                    {
                        if (!(IsLettera(a) || a == '*' || a == '?'))
                        {
                            erroreSintassi = i;
                        }
                    }
                    else if (c == '/' || c == '\\')
                    {
                        if (a == '/' || a == '\\' || a == '<')
                        {
                            erroreSintassi = i;
                        }
                        else
                        {
                            if (a != '|' && a != ':' && a != '~' && (!Char.IsDigit(a)) && a != '(' && a != '[')
                            {
                                espressione = espressione.Insert(i, "0");
                                ++i;
                            }
                        }
                    }
                    else if (c == '(')
                    {
                        ++nParentesiSinistra;
                        if (nParentesiQuadrateSinistra > 0 && nParentesiSinistra > 1)
                        {
                            erroreParentesiQuadrate = true;
                        }

                        if (a == '/' || a == '\\' || a == ':' || a == '<')
                        {
                            erroreSintassi = i;
                        }
                        else
                        {
                            if (a != '|' && a != '~' && a != '[' && (!Char.IsDigit(a)))
                            {
                                espressione = espressione.Insert(i, "0");
                                ++i;
                            }
                        }
                    }
                    else if (c == ')')
                    {
                        ++nParentesiDestra;
                        if (nParentesiDestra > nParentesiSinistra)
                        {
                            erroreParentesi = true;
                        }

                        if ((a >= '/' && a <= ':') || a == '|' || a == '~' || a == '\\' || a == '<')
                        {
                            erroreSintassi = i;
                        }
                    }
                    else if (c == '[')
                    {
                        ++nParentesiQuadrateSinistra;
                        if (nParentesiQuadrateSinistra > 1)
                        {
                            erroreParentesiQuadrate = true;
                        }

                        if (a == '/' || a == '\\' || a == ':' || a == '<')
                        {
                            erroreSintassi = i;
                        }
                        else
                        {
                            if (a != '|' && a != '~' && (!Char.IsDigit(a)))
                            {
                                espressione = espressione.Insert(i, "0");
                                ++i;
                            }
                        }
                    }
                    else if (c == ']')
                    {
                        nParentesiQuadrateSinistra--;
                        if (nParentesiQuadrateSinistra < 0)
                        {
                            erroreParentesiQuadrate = true;
                        }

                        if (nParentesiDestra - nParentesiSinistra < 0)
                        {
                            erroreParentesi = true;
                        }

                        if (Char.IsDigit(a) || a == '/' || a == ':' || a == '|' || a == '~' || a == '\\' || a == '<')
                        {
                            erroreSintassi = i;
                        }
                    }
                    else if (c == '|' || Char.IsDigit(c))
                    {
                        if (a != ')' && a != ']' && a != '<' && a != '>' && !(IsLettera(a) || a == '*' || a == '?'))
                        {
                            erroreSintassi = i;
                        }

                        if (nParentesiQuadrateSinistra == 1 && c == '|')
                        {
                            b = 'a';
                            int j;
                            for (j = i + 1; b != ']' && (!Char.IsDigit(b)) && b != ':' && j < espressione.Length; ++j)
                            {
                                b = espressione[j];
                            }

                            espressione = espressione.Insert(j - 1, ")");
                            b = 'a';
                            for (j = i - 1; b != '[' && (!Char.IsDigit(b)) && b != ':' && j >= 0; --j)
                            {
                                b = espressione[j];
                            }

                            espressione = espressione.Insert(j + 2, "(");
                            ++i;
                            ++nParentesiSinistra;
                        }
                    }
                    else if (c == ':')
                    {
                        if ((a != ')' && a != '>' && !(IsLettera(a) || a == '*' || a == '?')) || nParentesiQuadrateSinistra == 0)
                        {
                            erroreSintassi = i;
                        }
                    }
                    else if (c == '~')
                    {
                        if (a == '<' || a == '(' || a == '[' || a == ':' || a == '/' || a == '|' || a == '~' || a == '\\' || (nParentesiQuadrateSinistra == 1 && nParentesiSinistra > 0))
                        {
                            erroreSintassi = i;
                        }
                        else
                        {
                            if (a == ')' || a > ']' || IsLettera(a) || a == '*' || a == '?')
                            {
                                espressione = espressione.Insert(i, "0");
                                ++i;
                            }
                        }
                        if (nParentesiQuadrateSinistra == 1 && (IsLettera(espressione[i + 1]) || espressione[i + 1] == '*' || espressione[i + 1] == '?' || espressione[i + 1] == '/' || espressione[i + 1] == '\\'))
                        {
                            espressione = espressione.Insert(i + 1, "(");
                            int j;
                            b = 'a';
                            for (j = i + 2; b != ']' && (!Char.IsDigit(b)) && b != ':' && j < espressione.Length; ++j)
                            {
                                b = espressione[j];
                            }

                            espressione = espressione.Insert(j, ")");
                        }
                    }
                    else if (c == '>')
                    {
                    }
                    else if (c == '<')
                    {
                        if (a == ')' || a == ']' || a == '>' || IsLettera(a) || a == '*' || a == '?')
                        {
                            espressione = espressione.Insert(i, "0");
                            ++i;
                        }
                        int nuovoI = espressione.IndexOf('>', i);
                        if (nuovoI > i)
                        {
                            i = nuovoI;
                        }
                        else
                        {
                            erroreSintassi = i;
                        }
                    }
                    else if (IsLettera(c) || c == '*' || c == '?' || c == '<')
                    {	// lettera (senza o con accento)
                        if (a == ')' || a == ']' || a == '>')
                        {
                            espressione = espressione.Insert(i, "0");
                            ++i;
                        }
                    }
                    else    // carattere non riconosciuto
                    {
                        erroreSintassi = i;
                    }
                } // if (i == 0) - else
            } // for (int i = 0; i < espressione.Length; ++i)

            a = espressione[^1];
            if (!(a == ')' || a == ']' || a == '-' || a == '\'' || IsLettera(a) || a == '*' || a == '?' || a == '>'))
            {
                erroreSintassi = espressione.Length - 1;
            }

            if (nParentesiSinistra != nParentesiDestra)
            {
                erroreParentesi = true;
            }

            if (nParentesiQuadrateSinistra == 1)
            {
                erroreParentesiQuadrate = true;
            }

            if (erroreParentesiQuadrate)
            {
                erroreParentesi = false; // indicare solo uno degli errori
            }

            if (erroreSintassi >= 0)
            {
                throw new SearchSyntaxErrorException(erroreSintassi.ToString(CultureInfo.CurrentCulture));
            }

            if (erroreParentesi)
            {
                throw new SearchParenthesesException();
            }

            if (erroreParentesiQuadrate)
            {
                throw new SearchBracketsException();
            }

            return espressione;
        }

        private static string ProssimaParola(string fraseRicercata, int inizio)
        {
            int j = 0;
            StringBuilder prossimaParola = new("");
            String frase = fraseRicercata[inizio..] + " "; // con " ", la riga c = sFraseRicercata[iInizio+j] funziona anche quando passa oltre la fine di sFraseRicercata
            char c = frase[0];
            if (c == '<')
            {
                int p = frase.IndexOf('>');
                return (p > 0 ? frase[1..p] : "");
            }
            else if (Char.IsDigit(c))
            {
                while (Char.IsDigit(c))
                {
                    prossimaParola.Append(c);
                    ++j;
                    c = frase[j];
                }
            }
            else
            {
                while (IsLettera(c) || c == '-' || c == '\'' || c == '*' || c == '?' || c == '/' || c == '\\')
                {
                    prossimaParola.Append(c);
                    ++j;
                    c = frase[j];
                }
            }

            return prossimaParola.ToString();
        }

        #endregion

        #region TestoBrano

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, []);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, string nomeVersione)
        {
            return await TestoBranoAsync(riferimento, nomeVersione, [], null, null);
        }

        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, string nomeVersione)
        {
            return await FlowDocumentBranoAsync(riferimento, nomeVersione, [], null, null);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="paroleRicercate">Tutte le parole che vanno sottolineate nel testo visualizzato.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, string nomeVersione, Riferimento paroleRicercate)
        {
            return await TestoBranoAsync(riferimento, nomeVersione, [], paroleRicercate, null, null);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(string riferimento, Collection<string> listaVersioni)
        {
            return await TestoBranoAsync(ConvertiRiferimento(riferimento), listaVersioni);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(string riferimento, string nomeVersione)
        {
            return await TestoBranoAsync(riferimento, nomeVersione, []);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(string riferimento, string nomeVersione, Collection<string> collezioniDaVisualizzare)
        {
            return await TestoBranoAsync(ConvertiRiferimento(riferimento), nomeVersione, collezioniDaVisualizzare);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="paroleRicercate">Tutte le parole che vanno sottolineate nel testo visualizzato.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(string riferimento, string nomeVersione, Collection<string> collezioniDaVisualizzare, Riferimento paroleRicercate)
        {
            return await TestoBranoAsync(ConvertiRiferimento(riferimento), nomeVersione, collezioniDaVisualizzare, paroleRicercate, null, null);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="paroleRicercate">Tutte le parole che vanno sottolineate nel testo visualizzato.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, string nomeVersione, Collection<string> collezioniDaVisualizzare, Riferimento paroleRicercate)
        {
            Collection<string> versioni =
            [
                nomeVersione
            ];
            return await TestoBranoAsync(riferimento, versioni, collezioniDaVisualizzare, paroleRicercate, null, null);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, string nomeVersione, Collection<string> collezioniDaVisualizzare)
        {
            return await TestoBranoAsync(riferimento, nomeVersione, collezioniDaVisualizzare, new Riferimento(), null, null);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, null, null);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(string riferimento, Collection<string> listaVersioni, BackgroundWorker worker, DoWorkEventArgs e)
        {
            return await TestoBranoAsync(ConvertiRiferimento(riferimento), listaVersioni, worker, e);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una lista di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, BackgroundWorker worker, DoWorkEventArgs e)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, [], worker, e);
        }

        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, bool alternare)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, [], alternare, null, null);
        }

        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, bool alternare)
        {
            return await FlowDocumentBranoAsync(riferimento, listaVersioni, [], alternare, null, null);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una lista di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="alternare">Se tutti i testi sono mostrati per ogni versetto, l'uno dopo l'altro (invece di fare tutti i testi l'uno dopo l'altro).</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, bool alternare, BackgroundWorker? worker, DoWorkEventArgs e)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, [], alternare, worker, e);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, string nomeVersione, BackgroundWorker worker, DoWorkEventArgs e)
        {
            return await TestoBranoAsync(riferimento, nomeVersione, [], worker, e);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, string nomeVersione, Collection<string> collezioniDaVisualizzare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            Collection<string> versioni =
            [
                nomeVersione
            ];
            return await TestoBranoAsync(riferimento, versioni, collezioniDaVisualizzare, worker, e);
        }

        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, string nomeVersione, Collection<string> collezioniDaVisualizzare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            Collection<string> versioni =
            [
                nomeVersione
            ];
            return await FlowDocumentBranoAsync(riferimento, versioni, collezioniDaVisualizzare, worker, e);
        }
        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="paroleRicercate">Tutte le parole che vanno sottolineate nel testo visualizzato.</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, string nomeVersione, Collection<string> collezioniDaVisualizzare, Riferimento paroleRicercate, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            Collection<string> versioni =
            [
                nomeVersione
            ];
            return await TestoBranoAsync(riferimento, versioni, collezioniDaVisualizzare, paroleRicercate, worker, e);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, new Riferimento(), worker, e);
        }

        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await FlowDocumentBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, new Riferimento(), worker, e);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="alternare">Se tutti i testi sono mostrati per ogni versetto, l'uno dopo l'altro (invece di fare tutti i testi l'uno dopo l'altro).</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, bool alternare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, new Riferimento(), alternare, worker, e);
        }

        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, bool alternare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await FlowDocumentBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, new Riferimento(), alternare, worker, e);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="paroleRicercate">Tutte le parole che vanno sottolineate nel testo visualizzato.</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, Riferimento paroleRicercate, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, paroleRicercate, false, worker, e);
        }

        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, Riferimento paroleRicercate, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await FlowDocumentBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, true, paroleRicercate, false, worker, e);
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="paroleRicercate">Tutte le parole che vanno sottolineate nel testo visualizzato.</param>
        /// <param name="alternare">Se tutti i testi sono mostrati per ogni versetto, l'uno dopo l'altro (invece di fare tutti i testi l'uno dopo l'altro).</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        public async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, Riferimento paroleRicercate, bool alternare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await TestoBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, true, paroleRicercate, alternare, worker, e);
        }
        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, Riferimento paroleRicercate, bool alternare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            return await FlowDocumentBranoAsync(riferimento, listaVersioni, collezioniDaVisualizzare, true, paroleRicercate, alternare, worker, e);
        }

        public async Task<FlowDocument> FlowDocumentBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, bool conNomiVersioni, Riferimento paroleRicercate, bool alternare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            FlowDocument fd;
            if (alternare)
            {
                byte cap0, cap1, vers0, vers1, maxCapitoloInTuttiTesti, maxVersettoInTuttiTesti;

                StringBuilder titoloVersetto = new(RtfIntestazione().Length + 40);
                int lunghezzaIntestazione = RtfIntestazione().Length;
                String testoVersetto;
                //string titoloVersettoInizio = RtfIntestazione() + @"{\v " + RichTextBoxEx.InizioRiferimento;
                string titoloVersettoInizio = @"{\v " + RichTextBoxEx.InizioRiferimento;
                string libStringa, capStringa, versStringa;
                RiferimentoFormato rfVecchio = Formato.RiferimentoFormato;
                List<string> stringheRtf = await Task.Run(async () =>
                {
                    List<string> stringheRtf = [];
                    StringBuilder stringaRtf = new(RtfIntestazione());
                    byte[] riferimentoArray = new byte[6];
                    Riferimento rif = new Riferimento(riferimentoArray);
                    foreach (byte[] branoInRiferimento in riferimento.Brani)
                    {
                        for (byte lib = branoInRiferimento[0]; lib <= branoInRiferimento[3]; ++lib)
                        {
                            libStringa = Numeri2Stringhe[lib];

                            if (lib == branoInRiferimento[0])
                            {
                                cap0 = branoInRiferimento[1];
                            }
                            else
                            {
                                cap0 = 1;
                            }

                            maxCapitoloInTuttiTesti = 0;
                            foreach (string versioneDaControllare in listaVersioni)
                            {
                                if (Info(versioneDaControllare).Tipo == TestoTipi.Bibbia && CapitoliInLibro(lib, versioneDaControllare) > maxCapitoloInTuttiTesti)
                                {
                                    maxCapitoloInTuttiTesti = CapitoliInLibro(lib, versioneDaControllare);
                                }
                            }

                            if (maxCapitoloInTuttiTesti == 0)
                            {
                                maxCapitoloInTuttiTesti = CapitoliInLibro(lib, UltimaBibbia);
                            }

                            if (lib == branoInRiferimento[3])
                            {
                                cap1 = branoInRiferimento[4];
                            }
                            else
                            {
                                cap1 = maxCapitoloInTuttiTesti;
                            }
                            if (cap1 > maxCapitoloInTuttiTesti)
                            {
                                cap1 = maxCapitoloInTuttiTesti;
                            }

                            for (byte cap = cap0; cap <= cap1; ++cap)
                            {
                                capStringa = Numeri3Stringhe[cap];
                                //capStringa = "00" + cap.ToString(CultureInfo.InvariantCulture);
                                //capStringa = capStringa[^3..];

                                if (lib == branoInRiferimento[0] && cap == branoInRiferimento[1])
                                {
                                    vers0 = branoInRiferimento[2];
                                }
                                else
                                {
                                    vers0 = 1;
                                }

                                maxVersettoInTuttiTesti = 0;
                                foreach (string versioneDaControllare in listaVersioni)
                                {
                                    if (Info(versioneDaControllare).Tipo == TestoTipi.Bibbia && VersettiInCapitolo(lib, cap, versioneDaControllare) > maxVersettoInTuttiTesti)
                                    {
                                        maxVersettoInTuttiTesti = VersettiInCapitolo(lib, cap, versioneDaControllare);
                                    }
                                }

                                if (maxVersettoInTuttiTesti == 0)
                                {
                                    maxVersettoInTuttiTesti = VersettiInCapitolo(lib, cap, UltimaBibbia);
                                }

                                if (lib == branoInRiferimento[3] && cap == branoInRiferimento[4])
                                {
                                    vers1 = branoInRiferimento[5];
                                }
                                else
                                {
                                    vers1 = maxVersettoInTuttiTesti;
                                }

                                if (vers1 > maxVersettoInTuttiTesti)
                                {
                                    vers1 = maxVersettoInTuttiTesti;
                                }

                                for (byte vers = vers0; vers <= vers1; ++vers)
                                {
                                    versStringa = Numeri3Stringhe[vers];
                                    riferimentoArray[0] = lib;
                                    riferimentoArray[1] = cap;
                                    riferimentoArray[2] = vers;
                                    riferimentoArray[3] = lib;
                                    riferimentoArray[4] = cap;
                                    riferimentoArray[5] = vers;
                                    stringaRtf.Append(titoloVersettoInizio);
                                    stringaRtf.Append(libStringa).Append(capStringa).Append(versStringa);
                                    stringaRtf.Append(@"}\fs28\b ").Append(ConvertiRiferimentoDa3ByteATesto(riferimentoArray, Formato.RiferimentoFormato)).Append(@"\b0\par\ql\par ");
                                    Formato.RiferimentoFormato = RiferimentoFormato.Nessuno;
                                    rif.Rimuovi(0);
                                    rif.AggiungiBrano(riferimentoArray);
                                    testoVersetto = await TestoBranoAsync(rif, listaVersioni, collezioniDaVisualizzare, false, paroleRicercate, false, null, e); // null per worker, così non è aggiornato per ogni versetto
                                    stringaRtf.Append(testoVersetto[lunghezzaIntestazione..^1]).Append(@"\par\ql\par}");
                                    Formato.RiferimentoFormato = rfVecchio;
                                }
                            }
                        }
                        // TODO2 worker?.ReportProgress(-listaVersioni.Count - collezioniDaVisualizzare.Count, e);
                    }
                    stringheRtf.Add(stringaRtf.ToString());
                    return stringheRtf;
                }).ConfigureAwait(false);
                fd = await MergeManyRtfAsync(stringheRtf);
            }
            else
            { // else non alternare
                try
                {
                    List<Riferimento> noteDaVisualizzare = [];
                    if (listaVersioni.Count > 0)
                    {
                        foreach (string collezione in collezioniDaVisualizzare)
                        {
                            noteDaVisualizzare.Add(versioni[collezione].ElencaNoteInBrano(riferimento));
                        }
                    }

                    bool bibbiaTrovata = false;
                    // TODO2 worker?.ReportProgress(-1, e);
                    if (listaVersioni.Count == 0)
                    { // non c'è una versione della Bibbia, solo note
                        List<string> stringheRtf = await Task.Run(async () =>
                        {
                            List<string> stringheRtf = [];
                            string testoInCollezione;
                            for (int i = 0; i < collezioniDaVisualizzare.Count; ++i)
                            {
                                try
                                {
                                    Riferimento noteInCollezione = new();
                                    if (riferimento.Versetti)
                                    {
                                        noteInCollezione = versioni[collezioniDaVisualizzare[i]].ElencaNoteInBrano(riferimento);
                                    }
                                    else
                                    {
                                        noteInCollezione = riferimento;
                                    }

                                    if (noteInCollezione.Count > 0)
                                    {
                                        if (conNomiVersioni)
                                        {
                                            stringheRtf.Add(RtfIntestazione() + @"\fs28\b " + collezioniDaVisualizzare[i] + @"\par}");
                                        }

                                        testoInCollezione = await versioni[collezioniDaVisualizzare[i]].TestoBrano(noteInCollezione, [], [], conNomiVersioni, worker, e);
                                        if (i != collezioniDaVisualizzare.Count - 1)
                                        {
                                            testoInCollezione = testoInCollezione[..^1] + @"\par\ql\par}";
                                        }
                                        stringheRtf.Add(testoInCollezione);
                                    }
                                }
                                catch { }
                            }
                            return stringheRtf;
                        }).ConfigureAwait(false);
                        fd = await MergeManyRtfAsync(stringheRtf);
                    }
                    else if (listaVersioni.Count == 1 && ((versioni[listaVersioni[0]].Info.Tipo & TestoTipi.Bibbia) != TestoTipi.Bibbia))
                    {
                        // quando una collezione di note, il testo è già RTF completo
                        fd = new FlowDocument();
                        using MemoryStream ms = new(Encoding.ASCII.GetBytes(await versioni[listaVersioni[0]].TestoBrano(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, paroleRicercate)));
                        ms.Position = 0;

                        TextRange range = new(fd.ContentStart, fd.ContentEnd);
                        range.Load(ms, DataFormats.Rtf);
                    }
                    else
                    {
                        List<string> stringheRtf = await Task.Run(async () =>
                         {
                             List<string> stringheRtf = [];
                             StringBuilder stringaRtf = new(RtfIntestazione());
                             string testoInVersione;
                             int lunghezzaIntestazione = stringaRtf.Length;
                             for (int i = 0; i < listaVersioni.Count; ++i)
                             {
                                 try
                                 {
                                     if (conNomiVersioni && listaVersioni.Count > 1)
                                     {
                                         stringaRtf.Append(@"{\b1").Append(listaVersioni[i]).Append(@"}\par\ql\par");
                                     }
                                     testoInVersione = await versioni[listaVersioni[i]].TestoBranoAsync(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, conNomiVersioni, paroleRicercate, worker, e);
                                     stringaRtf.Append(testoInVersione[lunghezzaIntestazione..^1]);
                                     if (i < listaVersioni.Count - 1)
                                     {
                                         stringaRtf.Append(@"\par\ql\par");
                                     }
                                     if (!bibbiaTrovata && versioni[listaVersioni[i]].Info.Tipo == TestoTipi.Bibbia)
                                     {
                                         UltimaBibbia = listaVersioni[i];
                                         bibbiaTrovata = true;
                                     }
                                 }
                                 catch { } // il nome della versione non era riconosciuto
                             }
                             stringheRtf.Add(stringaRtf + @"}");
                             return stringheRtf;
                         }).ConfigureAwait(false);
                        fd = await MergeManyRtfAsync(stringheRtf);
                    }
                }
                catch (KeyNotFoundException)
                {
                    throw new TextNotExistException();
                }

            }
            return fd;
        }

        /// <summary>
        /// Il testo biblico di un brano.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano desiderato.</param>
        /// <param name="listaVersioni">Una collezione di stringhe con i nomi delle versioni di cui mostrare il testo.</param>
        /// <param name="collezioniDaVisualizzare">Una collezione delle collezioni di note che devono essere visualizzate insieme con il testo.</param>
        /// <param name="conNomiVersioni">Se aggiungi i nomi dei testi al risultato.</param>
        /// <param name="paroleRicercate">Tutte le parole che vanno sottolineate nel testo visualizzato.</param>
        /// <param name="alternare">Se tutti i testi sono mostrati per ogni versetto, l'uno dopo l'altro (invece di fare tutti i testi l'uno dopo l'altro).</param>
        /// <param name="worker">Il thread in cui il testo è creato.</param>
        /// <param name="e">Gli argomenti del thread.</param>
        /// <returns>Il testo biblico.</returns>
        private async Task<string> TestoBranoAsync(Riferimento riferimento, Collection<string> listaVersioni, Collection<string> collezioniDaVisualizzare, bool conNomiVersioni, Riferimento paroleRicercate, bool alternare, BackgroundWorker? worker, DoWorkEventArgs? e)
        {
            string brano;
            if (alternare)
            {
                byte cap0, cap1, vers0, vers1, maxCapitoloInTuttiTesti, maxVersettoInTuttiTesti;
                StringBuilder titoloVersetto = new(RtfIntestazione().Length + 40);
                string titoloVersettoInizio = RtfIntestazione() + @"{\v " + RichTextBoxEx.InizioRiferimento;
                string testoVersetto;
                string libStringa, capStringa, versStringa;
                byte[] riferimentoArray = new byte[6];
                RiferimentoFormato rfVecchio = Formato.RiferimentoFormato;
                List<string> stringheRtf = await Task.Run(async () =>
                {
                    List<string> stringheRtf = [];

                    foreach (byte[] branoInRiferimento in riferimento.Brani)
                    {
                        for (byte lib = branoInRiferimento[0]; lib <= branoInRiferimento[3]; ++lib)
                        {
                            libStringa = Numeri2Stringhe[lib];
                            //libStringa = "0" + lib.ToString(CultureInfo.InvariantCulture);
                            //libStringa = libStringa[^2..];

                            if (lib == branoInRiferimento[0])
                            {
                                cap0 = branoInRiferimento[1];
                            }
                            else
                            {
                                cap0 = 1;
                            }

                            maxCapitoloInTuttiTesti = 0;
                            foreach (string versioneDaControllare in listaVersioni)
                            {
                                if (Info(versioneDaControllare).Tipo == TestoTipi.Bibbia && CapitoliInLibro(lib, versioneDaControllare) > maxCapitoloInTuttiTesti)
                                {
                                    maxCapitoloInTuttiTesti = CapitoliInLibro(lib, versioneDaControllare);
                                }
                            }

                            if (maxCapitoloInTuttiTesti == 0)
                            {
                                maxCapitoloInTuttiTesti = CapitoliInLibro(lib, UltimaBibbia);
                            }

                            if (lib == branoInRiferimento[3])
                            {
                                cap1 = branoInRiferimento[4];
                            }
                            else
                            {
                                cap1 = maxCapitoloInTuttiTesti;
                            }
                            if (cap1 > maxCapitoloInTuttiTesti)
                            {
                                cap1 = maxCapitoloInTuttiTesti;
                            }

                            for (byte cap = cap0; cap <= cap1; ++cap)
                            {
                                capStringa = Numeri3Stringhe[cap];
                                //capStringa = "00" + cap.ToString(CultureInfo.InvariantCulture);
                                //capStringa = capStringa[^3..];

                                if (lib == branoInRiferimento[0] && cap == branoInRiferimento[1])
                                {
                                    vers0 = branoInRiferimento[2];
                                }
                                else
                                {
                                    vers0 = 1;
                                }

                                maxVersettoInTuttiTesti = 0;
                                foreach (string versioneDaControllare in listaVersioni)
                                {
                                    if (Info(versioneDaControllare).Tipo == TestoTipi.Bibbia && VersettiInCapitolo(lib, cap, versioneDaControllare) > maxVersettoInTuttiTesti)
                                    {
                                        maxVersettoInTuttiTesti = VersettiInCapitolo(lib, cap, versioneDaControllare);
                                    }
                                }

                                if (maxVersettoInTuttiTesti == 0)
                                {
                                    maxVersettoInTuttiTesti = VersettiInCapitolo(lib, cap, UltimaBibbia);
                                }

                                if (lib == branoInRiferimento[3] && cap == branoInRiferimento[4])
                                {
                                    vers1 = branoInRiferimento[5];
                                }
                                else
                                {
                                    vers1 = maxVersettoInTuttiTesti;
                                }

                                if (vers1 > maxVersettoInTuttiTesti)
                                {
                                    vers1 = maxVersettoInTuttiTesti;
                                }

                                for (byte vers = vers0; vers <= vers1; ++vers)
                                {
                                    versStringa = Numeri3Stringhe[vers];
                                    //versStringa = "00" + vers.ToString(CultureInfo.InvariantCulture);
                                    //versStringa = versStringa[^3..];
                                    riferimentoArray[0] = lib;
                                    riferimentoArray[1] = cap;
                                    riferimentoArray[2] = vers;
                                    riferimentoArray[3] = lib;
                                    riferimentoArray[4] = cap;
                                    riferimentoArray[5] = vers;
                                    titoloVersetto.Remove(0, titoloVersetto.Length);
                                    titoloVersetto.Append(titoloVersettoInizio);
                                    titoloVersetto.Append(libStringa).Append(capStringa).Append(versStringa);
                                    titoloVersetto.Append(@"}\fs28\b ").Append(ConvertiRiferimentoDa3ByteATesto(riferimentoArray, Formato.RiferimentoFormato)).Append(@"\par}");
                                    stringheRtf.Add(titoloVersetto.ToString());
                                    Formato.RiferimentoFormato = RiferimentoFormato.Nessuno;
                                    testoVersetto = await TestoBranoAsync(new Riferimento(riferimentoArray), listaVersioni, collezioniDaVisualizzare, false, paroleRicercate, false, null, e); // null per worker, così non è aggiornato per ogni versetto
                                    stringheRtf.Add(testoVersetto[..^1] + @"\par}");
                                    //stringheRtf.Add(await TestoBranoAsync(new Riferimento(riferimentoArray), listaVersioni, collezioniDaVisualizzare, false, paroleRicercate, false, null, e)); // null per worker, così non è aggiornato per ogni versetto
                                    //stringheRtf.Add(RtfIntestazione() + @"\par}");
                                    Formato.RiferimentoFormato = rfVecchio;
                                }
                            }
                        }
                        // TODO2 worker?.ReportProgress(-listaVersioni.Count - collezioniDaVisualizzare.Count, e);
                    }
                    return stringheRtf;
                }).ConfigureAwait(false);
                brano = await ToRtfStringAsync(await MergeManyRtfAsync(stringheRtf));
            }
            else
            { // else non alternare
                try
                {
                    List<Riferimento> noteDaVisualizzare = [];
                    if (listaVersioni.Count > 0)
                    {
                        foreach (string collezione in collezioniDaVisualizzare)
                        {
                            noteDaVisualizzare.Add(versioni[collezione].ElencaNoteInBrano(riferimento));
                        }
                    }

                    bool bibbiaTrovata = false;
                    // TODO2 worker?.ReportProgress(-1, e);
                    if (listaVersioni.Count == 0)
                    { // non c'è una versione della Bibbia, solo note
                        List<string> stringheRtf = await Task.Run(async () =>
                        {
                            List<string> stringheRtf = [];
                            string testoInCollezione;
                            for (int i = 0; i < collezioniDaVisualizzare.Count; ++i)
                            {
                                try
                                {
                                    Riferimento noteInCollezione = new();
                                    if (riferimento.Versetti)
                                    {
                                        noteInCollezione = versioni[collezioniDaVisualizzare[i]].ElencaNoteInBrano(riferimento);
                                    }
                                    else
                                    {
                                        noteInCollezione = riferimento;
                                    }

                                    if (noteInCollezione.Count > 0)
                                    {
                                        if (conNomiVersioni)
                                        {
                                            stringheRtf.Add(RtfIntestazione() + @"\fs28\b " + collezioniDaVisualizzare[i] + @"\par}");
                                        }

                                        testoInCollezione = await versioni[collezioniDaVisualizzare[i]].TestoBrano(noteInCollezione, [], [], conNomiVersioni, worker, e);
                                        if (i != collezioniDaVisualizzare.Count - 1)
                                        {
                                            testoInCollezione = testoInCollezione[..^1] + @"\par\ql\par}";
                                        }
                                        stringheRtf.Add(testoInCollezione);
                                    }
                                }
                                catch { }
                            }
                            return stringheRtf;
                        }).ConfigureAwait(false);

                        brano = await ToRtfStringAsync(await MergeManyRtfAsync(stringheRtf));
                    }
                    else if (listaVersioni.Count == 1 && ((versioni[listaVersioni[0]].Info.Tipo & TestoTipi.Bibbia) != TestoTipi.Bibbia))
                    {
                        // quando una collezione di note, il testo è già RTF completo
                        brano = await versioni[listaVersioni[0]].TestoBrano(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, paroleRicercate);
                    }
                    else
                    {
                        List<string> stringheRtf = await Task.Run(async () =>
                        {
                            List<string> stringheRtf = [];
                            string testoPerVersione;
                            StringBuilder stringaRtf = new(RtfIntestazione());
                            int lunghezzaIntestazione = stringaRtf.Length;
                            for (int i = 0; i < listaVersioni.Count; ++i)
                            {
                                try
                                {
                                    if (conNomiVersioni && listaVersioni.Count > 1)
                                    {
                                        stringaRtf.Append(@"{\b1").Append(listaVersioni[i]).Append(@"}\par\ql\par");
                                    }
                                    testoPerVersione = await versioni[listaVersioni[i]].TestoBranoAsync(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, conNomiVersioni, paroleRicercate, worker, e);
                                    stringaRtf.Append(testoPerVersione[lunghezzaIntestazione..^1]);
                                    if (i < listaVersioni.Count - 1)
                                    {
                                        stringaRtf.Append(@"\par\ql\par");
                                    }

                                    if (!bibbiaTrovata && versioni[listaVersioni[i]].Info.Tipo == TestoTipi.Bibbia)
                                    {
                                        UltimaBibbia = listaVersioni[i];
                                        bibbiaTrovata = true;
                                    }
                                }
                                catch { } // il nome della versione non era riconosciuto
                            }
                            while (stringaRtf.Length > 0 && char.IsWhiteSpace(stringaRtf[^1]))
                            {
                                stringaRtf.Length--;
                            }

                            if (stringaRtf.Length >= 4 &&
                                stringaRtf[^4] == '\\' &&
                                stringaRtf[^3] == 'p' &&
                                stringaRtf[^2] == 'a' &&
                                stringaRtf[^1] == 'r')
                            {
                                stringaRtf.Length -= 4;
                            }

                            stringheRtf.Add(stringaRtf + "}");
                            return stringheRtf;
                        }).ConfigureAwait(false);
                        brano = stringheRtf[0];
                        //brano = await ToRtfStringAsync(await MergeManyRtfAsync(stringheRtf));
                    }
                }
                catch (KeyNotFoundException)
                {
                    throw new TextNotExistException();
                }

            }
            if (brano.EndsWith(@"\par\par }", StringComparison.Ordinal))
            {
                brano = brano[..^10] + "}";
            }
            else if (brano.EndsWith("\\pard\\par\r\n}\r\n", StringComparison.Ordinal))
            {
                brano = brano[..^14] + "\\pard}\r\n";
            }
            else if (brano.EndsWith("\\pard\\i0\\f0\\par\r\n}\r\n", StringComparison.Ordinal))
            {
                brano = brano[..^20] + "\\pard\\i0\\f0}\r\n";
            }
            else if (brano.EndsWith("\\pard\\f0\\par\r\n}\r\n", StringComparison.Ordinal))
            {
                brano = brano[..^17] + "\\pard\\f0}\r\n";
            }
            else if (brano.EndsWith("\\pard\\cf0\\par\r\n}\r\n", StringComparison.Ordinal))
            {
                brano = brano[..^18] + "\\pard\\cf0}\r\n";
            }
            return brano;
        }

        public void SetFontRtb(RichTextBoxEx rtb)
        {
            TextRange range = new(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            // FontFamily (può fallire se nome non valido)
            try
            {
                range.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(Formato.FontNome));
            }
            catch (Exception)
            {
                // fallback: lascia il font corrente oppure imposta un default
                // (in WPF di solito FontFamily non “esplode” spesso, ma se il nome è errato meglio gestirlo)
            }

            // FontSize
            range.ApplyPropertyValue(TextElement.FontSizeProperty, (double)Formato.FontDimensione);

            // Bold/Normal
            range.ApplyPropertyValue(
                TextElement.FontWeightProperty,
                Formato.FontGrassetto ? FontWeights.Bold : FontWeights.Normal
            ); // esempio d’uso con FontWeightProperty [1](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.textrange.applypropertyvalue?view=windowsdesktop-10.0)

            // Italic/Normal
            range.ApplyPropertyValue(
                TextElement.FontStyleProperty,
                Formato.FontCorsivo ? FontStyles.Italic : FontStyles.Normal
            );

            // Underline (si applica come TextDecorations sull’Inline)
            range.ApplyPropertyValue(
                Inline.TextDecorationsProperty,
                Formato.FontSottolineato ? TextDecorations.Underline : null
            ); // underline via TextDecorations è il meccanismo WPF [2](https://sne04.blogspot.com/2008/12/using-wpfc-richtextbox.html)[3](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/how-to-create-a-text-decoration)

        }

        /// <summary>
        /// Solo il testo di un versetto come è memorizzato nel file.
        /// </summary>
        /// <param name="libro">Il numero del libro nel riferimento del versetto desiderato.</param>
        /// <param name="capitolo">Il numero del capitolo nel riferimento del versetto desiderato.</param>
        /// <param name="versetto">Il numero del versetto nel riferimento del versetto desiderato.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il testo del versetto come è memorizzato nel file.</returns>
        public string TestoVersettoRaw(byte libro, byte capitolo, byte versetto, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].TestoVersettoRaw(libro, capitolo, versetto);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        #endregion

        #region Funzioni per il numero di capitoli e versetti

        /// <summary>
        /// Il numero di capitoli in un libro in una versione.
        /// </summary>
        /// <param name="libro">Il numero del libro (da 1 a 73).</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero di capitoli.</returns>
        /// <exception cref="KeyNotFoundException">Se il nome della versione non esiste.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
        public byte CapitoliInLibro(byte libro, string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].capitoliInLibro[libro];
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Il numero di capitoli in tutti i libri fino ad un certo libro in una versione.
        /// </summary>
        /// <param name="libro">Il numero del libro (da 1 a 73).</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero di capitoli.</returns>
        /// <exception cref="KeyNotFoundException">Se il nome della versione non esiste.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
        public UInt16 CapitoliFinoALibro(byte libro, string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].indiceLibro[libro];
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Il numero di versetti in un capitolo in un libro in una versione.
        /// </summary>
        /// <param name="libro">Il numero del libro (da 1 a 73).</param>
        /// <param name="capitolo">Il numero del capitolo.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero di versetti.</returns>
        /// <exception cref="KeyNotFoundException">Se il nome della versione non esiste.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
        public byte VersettiInCapitolo(byte libro, byte capitolo, string nomeVersione)
        {
            // tested
            try
            {
                if (versioni[nomeVersione].capitoliInLibro[libro] == 0)
                {
                    return 0;
                }
                else
                {
                    return versioni[nomeVersione].versettiInCapitolo[versioni[nomeVersione].indiceLibro[libro - 1] + capitolo];
                }
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Il numero di versetti in tutti i capitoli fino ad un certo capitolo in una versione.
        /// </summary>
        /// <param name="libro">Il numero del libro (da 1 a 73).</param>
        /// <param name="capitolo">Il numero del capitolo.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero di versetti.</returns>
        /// <exception cref="KeyNotFoundException">Se il nome della versione non esiste.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
        public UInt32 VersettiFinoACapitolo(byte libro, byte capitolo, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].indiceCapitolo[versioni[nomeVersione].indiceLibro[libro - 1] + capitolo];
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Il numero di versetti in tutti i capitoli fino ad un certo capitolo nell'ultima versione completa usata.
        /// </summary>
        /// <param name="libro">Il numero del libro (da 1 a 73).</param>
        /// <param name="capitolo">Il numero del capitolo.</param>
        /// <returns>Il numero di versetti.</returns>
        /// <exception cref="KeyNotFoundException">Se non c'è stata un'ultima versione.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
        public UInt32 VersettiFinoACapitolo(byte libro, byte capitolo)
        {
            return versioni[UltimaBibbiaCompleta].indiceCapitolo[versioni[UltimaBibbiaCompleta].indiceLibro[libro - 1] + capitolo];
        }

        /// <summary>
        /// Il numero di un libro in cui è un certo capitolo della Bibbia (contando da 1 a circa 1300).
        /// </summary>
        /// <param name="capitolo">Il capitolo da cercare (1-50 in Genesi, 51-90 in Esodo, ecc.).</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero del libro.</returns>
        public byte LibroDiCapitolo(int capitolo, string nomeVersione)
        {
            // tested
            if (capitolo < 1)
            {
                capitolo = 1;
            }

            byte libro = 0;
            do
            {
                libro++;
            }
            while (libro <= 73 && CapitoliFinoALibro(libro, nomeVersione) < capitolo);

            return libro;
        }

        /// <summary>
        /// Il riferimento di un versetto, secondo il suo posto nell'ordine della Bibbia (contando da 1 a circa 31000).
        /// </summary>
        /// <param name="versetto">Il versetto da cercare (1-31 in Genesi 1, 32-56 in Genesi 2, ecc.).</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il riferimento del versetto.</returns>
        public Riferimento RiferimentoDiVersetto(int versetto, string nomeVersione)
        {
            if (versetto < 1)
            {
                versetto = 1;
            }

            byte libro = 0;
            do
            {
                libro++;
            }
            while (libro <= 73 && VersettiFinoACapitolo(libro, CapitoliInLibro(libro, nomeVersione), nomeVersione) < versetto);

            byte capitolo = 0;
            do
            {
                capitolo++;
            }
            while (VersettiFinoACapitolo(libro, capitolo, nomeVersione) < versetto);

            return new Riferimento(libro, capitolo, Convert.ToByte(versetto - VersettiFinoACapitolo(libro, capitolo, nomeVersione) + VersettiInCapitolo(libro, capitolo, nomeVersione)));
            // versetto - VersettiFinoACapitolo(libro, capitolo, nomeVersione) + VersettiInCapitolo(libro, capitolo, nomeVersione)
            // invece di versetto - VersettiFinoACapitolo(libro-1, capitolo, nomeVersione) 
            // perché libro-1 non è possibile quando libro è di tipo byte
        }

        /// <summary>
        /// Il riferimento di un capitolo, secondo il suo posto nell'ordine della Bibbia (contando da 1 a circa 1200).
        /// </summary>
        /// <param name="capitolo">Il capitolo da cercare (1-50 in Genesi, 51-91 in Esodo, ecc.).</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il riferimento del capitolo.</returns>
        public Riferimento RiferimentoDiCapitolo(int capitolo, string nomeVersione)
        {
            if (capitolo < 1)
            {
                capitolo = 1;
            }

            byte libro = 0;
            do
            {
                libro++;
            }
            while (libro <= 73 && CapitoliFinoALibro(libro, nomeVersione) < capitolo);
            byte numeroCapitolo = Convert.ToByte(capitolo - CapitoliFinoALibro(libro, nomeVersione) + CapitoliInLibro(libro, nomeVersione));
            // capitolo - CapitoliFinoALibro(libro, nomeVersione) + CapitoliInLibro(libro, nomeVersione)
            // invece di capitolo - CapitoliFinoALibro(libro-1, nomeVersione) 
            // perché libro-1 non è possibile quando libro è di tipo byte
            return new Riferimento([libro, numeroCapitolo, 1, libro, numeroCapitolo, VersettiInCapitolo(libro, numeroCapitolo, nomeVersione)]);
        }

        #endregion

        #region Funzioni per i riferimenti

        /// <summary>
        /// Converte un riferimento in una versione allo schema standard di riferimenti del programma.
        /// </summary>
        /// <param name="riferimento">Il riferimento da convertire.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il riferimento nello schema standard.</returns>
        public Riferimento ConvertiAStandard(Riferimento riferimento, string nomeVersione)
        {
            // tested
            foreach (byte[] branoDaConvertire in riferimento.Brani)
            {
                bool inizioConvertito = false;
                bool fineConvertita = false;
                try
                {
                    foreach (Int16[] rifDiversi in versioni[nomeVersione].riferimentiDiversi)
                    {
                        if (!inizioConvertito && branoDaConvertire[0] == rifDiversi[3] && branoDaConvertire[1] == rifDiversi[4] && (branoDaConvertire[2] == rifDiversi[5] || rifDiversi[5] <= 0))
                        {
                            branoDaConvertire[0] = (byte)rifDiversi[0];
                            branoDaConvertire[1] = (byte)rifDiversi[1];
                            inizioConvertito = true;
                            if (rifDiversi[5] > 0)
                            {
                                branoDaConvertire[2] = (byte)rifDiversi[2];
                            }
                            else if (rifDiversi[5] == 0)
                            { // fare la stessa cosa a tutti i versetti nel capitolo: cambiare il capitolo e/o sottrarre un numero da ogni versetto
                                if (rifDiversi[2] < 0)
                                {
                                    branoDaConvertire[2] = (byte)(branoDaConvertire[2] + rifDiversi[2]);
                                }
                            }
                            else // <0 ==> bisogna aggiungere il numero di versetti
                            {
                                branoDaConvertire[2] = (byte)(branoDaConvertire[2] - rifDiversi[5]);
                            }
                        }
                        if (!fineConvertita && branoDaConvertire[3] == rifDiversi[3] && branoDaConvertire[4] == rifDiversi[4] && (branoDaConvertire[5] == rifDiversi[5] || rifDiversi[5] <= 0))
                        {
                            branoDaConvertire[3] = (byte)rifDiversi[0];
                            branoDaConvertire[4] = (byte)rifDiversi[1];
                            fineConvertita = true;
                            if (rifDiversi[5] > 0)
                            {
                                branoDaConvertire[5] = (byte)rifDiversi[2];
                            }
                            else
                            {
                                if (branoDaConvertire[5] != 255)
                                {
                                    if (rifDiversi[5] == 0)
                                    {
                                        if (rifDiversi[2] < 0)
                                        {
                                            branoDaConvertire[5] = (byte)(branoDaConvertire[5] + rifDiversi[5]);
                                        }
                                    }
                                    else
                                    {
                                        branoDaConvertire[5] = (byte)(branoDaConvertire[5] - rifDiversi[5]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (KeyNotFoundException)
                { // "nomeVersione" non esiste; non cambiamo il riferimento
                    return riferimento;
                }
            }
            riferimento.DaTradurre = true;
            return riferimento;
        }

        /// <summary>
        /// Converte un riferimento nello schema standard di riferimenti del programma al riferimento in una versione della Bibbia.
        /// </summary>
        /// <param name="riferimento">Il riferimento da convertire.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il riferimento nello schema della versione.</returns>
        public Riferimento ConvertiDaStandard(Riferimento riferimento, string nomeVersione)
        {
            // tested
            foreach (byte[] branoDaConvertire in riferimento.Brani)
            {
                bool inizioConvertito = false;
                bool fineConvertita = false;
                try
                {
                    foreach (Int16[] rifDiversi in versioni[nomeVersione].riferimentiDiversi)
                    {
                        if (!inizioConvertito && branoDaConvertire[0] == rifDiversi[0] && branoDaConvertire[1] == rifDiversi[1] && (branoDaConvertire[2] == rifDiversi[2] || rifDiversi[2] <= 0))
                        {
                            branoDaConvertire[0] = (byte)rifDiversi[3];
                            branoDaConvertire[1] = (byte)rifDiversi[4];
                            inizioConvertito = true;
                            if (rifDiversi[2] > 0)
                            {
                                branoDaConvertire[2] = (byte)rifDiversi[5];
                            }
                            else if (rifDiversi[2] == 0) // fare la stessa cosa a tutti i versetti nel capitolo: cambiare il capitolo e/o sottrarre un numero da ogni versetto
                            {
                                if (rifDiversi[5] < 0)
                                {
                                    branoDaConvertire[2] = (byte)(branoDaConvertire[2] + rifDiversi[5]);
                                }
                            }
                            else // <0 ==> bisogna aggiungere il numero di versetti
                            {
                                branoDaConvertire[2] = (byte)(branoDaConvertire[2] - rifDiversi[2]);
                            }
                        }
                        if (!fineConvertita && branoDaConvertire[3] == rifDiversi[0] && branoDaConvertire[4] == rifDiversi[1] && (branoDaConvertire[5] == rifDiversi[2] || rifDiversi[2] <= 0))
                        {
                            branoDaConvertire[3] = (byte)rifDiversi[3];
                            branoDaConvertire[4] = (byte)rifDiversi[4];
                            fineConvertita = true;
                            if (rifDiversi[2] > 0)
                            {
                                branoDaConvertire[5] = (byte)rifDiversi[5];
                            }
                            else
                            {
                                if (branoDaConvertire[5] != 255)
                                {
                                    if (rifDiversi[2] == 0)
                                    {
                                        if (rifDiversi[5] < 0)
                                        {
                                            branoDaConvertire[5] = (byte)(branoDaConvertire[5] + rifDiversi[5]);
                                        }
                                    }
                                    else
                                    {
                                        branoDaConvertire[5] = (byte)(branoDaConvertire[5] - rifDiversi[2]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (KeyNotFoundException)
                { // "nomeVersione" non esiste; non cambiamo il riferimento
                    return riferimento;
                }
            }
            riferimento.DaTradurre = false;
            return riferimento;
        }

        /// <summary>
        /// Converte un riferimento nel formato "1 28:14; 4 24:17" a "Genesi 28:14; Numeri 24:17".
        /// </summary>
        /// <param name="riferimentoDaConvertire">Il riferimento da convertire</param>
        /// <returns>Il riferimento convertito</returns>
        public string ConvertiRiferimentoDa3Numeri(string riferimentoDaConvertire)
        {
            // tested
            StringBuilder riferimentoConvertito = new("");
            if (!string.IsNullOrEmpty(riferimentoDaConvertire))
            {
                riferimentoDaConvertire = ";" + riferimentoDaConvertire + ";";
                riferimentoDaConvertire = riferimentoDaConvertire.Replace("; ", ";");
                riferimentoDaConvertire = riferimentoDaConvertire[1..];
                while (!string.IsNullOrEmpty(riferimentoDaConvertire))
                {
                    int posizioneSpazio = riferimentoDaConvertire.IndexOf(' ');
                    int posizionePuntoVirgola = riferimentoDaConvertire.IndexOf(';');
                    if (posizionePuntoVirgola == -1)
                    {
                        riferimentoDaConvertire = "";
                    }
                    else
                    {
                        if (posizioneSpazio >= 0)
                        {
                            try
                            {
                                riferimentoConvertito.Append(libriNomi[Convert.ToInt32(riferimentoDaConvertire[..posizioneSpazio], CultureInfo.InvariantCulture)] + riferimentoDaConvertire[posizioneSpazio..posizionePuntoVirgola]).Append("; ");
                            }
                            catch
                            {
                                // se c'è un errore nel formato (per esempio la prima parte di riferimentoDaConvertire non è un numero), saltiamo quella parte
                            }
                        }
                        else // c'è solo il numero del libro
                        {
                            try
                            {
                                riferimentoConvertito.Append(libriNomi[Convert.ToInt32(riferimentoDaConvertire[..posizionePuntoVirgola], CultureInfo.InvariantCulture)]).Append("; ");
                            }
                            catch
                            {
                                // se c'è un errore nel formato (per esempio la prima parte di riferimentoDaConvertire non è un numero), saltiamo quella parte
                            }
                        }
                        riferimentoDaConvertire = riferimentoDaConvertire[(posizionePuntoVirgola + 1)..];
                    }
                }
            }
            string riferimentoStringa = riferimentoConvertito.ToString().Trim();
            if (riferimentoStringa.EndsWith(';'))
            {
                riferimentoStringa = riferimentoStringa[..^1];
            }

            return riferimentoStringa;
        }

        #region NormalizzaRiferimento

        /// <summary>
        /// Converte un riferimento in formato testuale ad uno più bello. Usa le abbreviazioni dei libri.
        /// </summary>
        /// <param name="riferimento">Il riferimento da convertire.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(string riferimento)
        {
            // tested
            return NormalizzaRiferimento(riferimento, RiferimentoFormato.Abbreviazione);
        }

        /// <summary>
        /// Converte un riferimento in formato testuale ad uno più bello.
        /// </summary>
        /// <param name="riferimento">Il riferimento da convertire.</param>
        /// <param name="formatoDelRiferimento">Il formato del riferimento da visualizzare.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(string riferimento, RiferimentoFormato formatoDelRiferimento)
        {
            // tested
            return NormalizzaRiferimento(ConvertiRiferimento(riferimento), formatoDelRiferimento);
        }

        /// <summary>
        /// Converte un riferimento nel formato del programma ad uno più bello.
        /// </summary>
        /// <param name="riferimento">Il riferimento da convertire.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(Riferimento riferimento)
        {
            // tested
            return NormalizzaRiferimento(riferimento, RiferimentoFormato.Abbreviazione);
        }

        /// <summary>
        /// Converte un riferimento nel formato del programma ad uno più bello.
        /// </summary>
        /// <param name="riferimento">Il riferimento da convertire.</param>
        /// <param name="formatoDelRiferimento">Il formato del riferimento da visualizzare.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(Riferimento riferimento, RiferimentoFormato formatoDelRiferimento)
        {
            // tested
            string riferimentoNormalizzato = "";
            string[] separatori = SeparatoriNeiRiferimenti();

            if (formatoDelRiferimento != RiferimentoFormato.Nessuno && riferimento.Versetti)
            { // se è un riferimento con note, restituisce niente
                string riferimentoTestuale;
                UInt16 sLibroVecchio = 0;
                UInt16 sCapitoloVecchio = 0;
                int nRiferimenti = riferimento.Count;
                for (int i = 0; i < nRiferimenti; ++i)
                {
                    riferimentoTestuale = ConvertiRiferimentoDa3ByteATesto(riferimento.Brani[i], formatoDelRiferimento);
                    if (riferimentoTestuale.EndsWith(':')) // se RifTipo==RIFTIPO_CITAZIONE
                    {
                        riferimentoTestuale = riferimentoTestuale[..^1];
                    }

                    if (!string.IsNullOrEmpty(riferimentoNormalizzato))
                    {
                        if (riferimento.Brani[i][0] == sLibroVecchio && riferimento.Brani[i][1] == sCapitoloVecchio && riferimento.Brani[i][0] == riferimento.Brani[i][3] && riferimento.Brani[i][1] == riferimento.Brani[i][4])
                        {
                            riferimentoTestuale = riferimentoTestuale[(riferimentoTestuale.IndexOf(' ') + 1)..];
                            riferimentoTestuale = riferimentoTestuale[(riferimentoTestuale.IndexOf(separatori[1], StringComparison.Ordinal) + 1)..];
                            riferimentoNormalizzato += separatori[2];
                        }
                        else
                        {
                            riferimentoNormalizzato += "; ";
                            if (riferimento.Brani[i][0] == sLibroVecchio && riferimento.Brani[i][0] == riferimento.Brani[i][3])
                            {
                                riferimentoTestuale = riferimentoTestuale[(riferimentoTestuale.IndexOf(' ') + 1)..];
                            }
                        }
                    }
                    riferimentoNormalizzato += riferimentoTestuale;
                    sLibroVecchio = 0;
                    if (riferimento.Brani[i][0] == riferimento.Brani[i][3])
                    {
                        sLibroVecchio = riferimento.Brani[i][3];
                    }

                    sCapitoloVecchio = 0;
                    if (riferimento.Brani[i][0] == riferimento.Brani[i][3] && riferimento.Brani[i][1] == riferimento.Brani[i][4])
                    {
                        sCapitoloVecchio = riferimento.Brani[i][4];
                    }
                }
            }

            if (formato.RiferimentoTipo == RiferimentoTipo.Citazione && !string.IsNullOrEmpty(riferimentoNormalizzato))
            {
                riferimentoNormalizzato += ":";
            }

            return riferimentoNormalizzato;
        }

        /// <summary>
        /// Converte un riferimento di un brano (libro, capitolo, versetto) ad un formato più bello.
        /// </summary>
        /// <param name="libroInizio">Il numero del libro dell'inizio del brano.</param>
        /// <param name="capitoloInizio">Il capitolo dell'inizio del brano.</param>
        /// <param name="versettoInizio">Il versetto dell'inizio del brano.</param>
        /// <param name="libroFine">Il numero del libro della fine del brano.</param>
        /// <param name="capitoloFine">Il capitolo della fine del brano.</param>
        /// <param name="versettoFine">Il versetto della fine del brano.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(byte libroInizio, byte capitoloInizio, byte versettoInizio, byte libroFine, byte capitoloFine, byte versettoFine)
        {
            return NormalizzaRiferimento(new Riferimento([libroInizio, capitoloInizio, versettoInizio, libroFine, capitoloFine, versettoFine]));
        }

        /// <summary>
        /// Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
        /// </summary>
        /// <param name="libro">Il numero del libro.</param>
        /// <param name="capitolo">Il capitolo.</param>
        /// <param name="versetto">Il versetto.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(byte libro, byte capitolo, byte versetto)
        {
            // tested
            return NormalizzaRiferimento(new Riferimento(libro, capitolo, versetto));
        }

        /// <summary>
        /// Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
        /// </summary>
        /// <param name="libro">Il numero del libro.</param>
        /// <param name="capitolo">Il capitolo.</param>
        /// <param name="versetto">Il versetto.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(int libro, int capitolo, int versetto)
        {
            // tested
            return NormalizzaRiferimento(Convert.ToByte(libro), Convert.ToByte(capitolo), Convert.ToByte(versetto));
        }

        /// <summary>
        /// Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
        /// </summary>
        /// <param name="libro">Il numero del libro, come stringa.</param>
        /// <param name="capitolo">Il capitolo, come stringa.</param>
        /// <param name="versetto">Il versetto, come stringa.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(string libro, string capitolo, string versetto)
        {
            try
            {
                return NormalizzaRiferimento(Convert.ToByte(libro, CultureInfo.InvariantCulture), Convert.ToByte(capitolo, CultureInfo.InvariantCulture), Convert.ToByte(versetto, CultureInfo.InvariantCulture));
            }
            catch (FormatException)
            {
                return "";
            }
            catch (OverflowException)
            {
                return "";
            }
        }

        /// <summary>
        /// Converte un riferimento ad un brano (libro, capitolo, versetto) ad un formato più bello.
        /// </summary>
        /// <param name="libroInizio">Il numero del libro dell'inizio del brano, come stringa.</param>
        /// <param name="capitoloInizio">Il capitolo dell'inizio del brano, come stringa.</param>
        /// <param name="versettoInizio">Il versetto dell'inizio del brano, come stringa.</param>
        /// <param name="libroFine">Il numero del libro della fine del brano, come stringa.</param>
        /// <param name="capitoloFine">Il capitolo della fine del brano, come stringa.</param>
        /// <param name="versettoFine">Il versetto della fine del brano, come stringa.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimento(string libroInizio, string capitoloInizio, string versettoInizio, string libroFine, string capitoloFine, string versettoFine)
        {
            try
            {
                return NormalizzaRiferimento(Convert.ToByte(libroInizio, CultureInfo.InvariantCulture), Convert.ToByte(capitoloInizio, CultureInfo.InvariantCulture), Convert.ToByte(versettoInizio, CultureInfo.InvariantCulture), Convert.ToByte(libroFine, CultureInfo.InvariantCulture), Convert.ToByte(capitoloFine, CultureInfo.InvariantCulture), Convert.ToByte(versettoFine, CultureInfo.InvariantCulture));
            }
            catch (FormatException)
            {
                return "";
            }
            catch (OverflowException)
            {
                return "";
            }
        }

        #endregion

        /// <summary>
        /// Converte un segnalibro ad un formato testuale più bello.
        /// </summary>
        /// <param name="segnalibro">Il riferimento del segnalibro.</param>
        /// <returns>Il riferimento convertito.</returns>
        public string NormalizzaRiferimentoSegnalibro(string segnalibro)
        {
            if (string.IsNullOrEmpty(segnalibro))
            {
                return "";
            }

            StringBuilder riferimento = new("");
            char[] spazio = [' '];
            string[] brani = SplitString(segnalibro, ';');
            foreach (string brano in brani)
            {
                string[] numeri = SplitString(brano, spazio);
                if (numeri.Length >= 6)
                {
                    riferimento.Append(NormalizzaRiferimento(numeri[0], numeri[1], numeri[2], numeri[3], numeri[4], numeri[5])).Append(';');
                }
                else if (numeri.Length >= 3)
                {
                    riferimento.Append(NormalizzaRiferimento(numeri[0], numeri[1], numeri[2])).Append(';');
                }
            }
            string riferimentoNormalizzato = riferimento.ToString();
            if (riferimentoNormalizzato.EndsWith(';'))
            {
                riferimentoNormalizzato = riferimentoNormalizzato[..^1];
            }

            return riferimentoNormalizzato;
        }

        /// <summary>
        /// Converti un riferimento testuale al formato usato dal programma.
        /// </summary>
        /// <param name="riferimento">Il riferimento da convertire.</param>
        /// <returns>Il riferimento nel formato usato dal programma.</returns>
        /// <seealso cref="Riferimento"/>
        public Riferimento ConvertiRiferimento(string riferimento)
        {
            Riferimento nuovoRiferimento = new();
            if (string.IsNullOrEmpty(riferimento))
            {
                return nuovoRiferimento;
            }

            riferimento = riferimento.Trim().ToLower(CultureInfo.CurrentCulture);
            if (riferimento.StartsWith('\\') && riferimento.Contains(' ')) // a volte il link inizia con \f0 ...
            {
                riferimento = riferimento[(riferimento.IndexOf(' ') + 1)..];
            }

            if (string.IsNullOrEmpty(riferimento))
            {
                return nuovoRiferimento;
            }
            // cancellare eventuali spazi dopo punteggiatura o un numero (per esempio 2 re)
            for (int i = riferimento.Length - 1; i >= 1; --i)
            {
                if (riferimento[i] == ' ' && (riferimento[i - 1] == ':' || riferimento[i - 1] == ',' || riferimento[i - 1] == '.' || riferimento[i - 1] == ';' || riferimento[i - 1] == '-' || char.IsDigit(riferimento[i - 1])))
                {
                    riferimento = riferimento.Remove(i, 1);
                }
            }
            // cancellare eventuali punti o virgole dopo il nome di un libro (virgole succede con RIFTIPO_CITAZIONE)
            for (int i = riferimento.Length - 1; i >= 1; --i)
            {
                if ((riferimento[i] == '.') && (Char.IsLetter(riferimento[i - 1])))
                {
                    riferimento = riferimento.Remove(i, 1);
                }
                else
                {
                    if ((riferimento[i] == ',') && (Char.IsLetter(riferimento[i - 1])))
                    {
                        if (i == riferimento.Length - 1 || (Char.IsDigit(riferimento[i + 1])) && (i == riferimento.Length - 2 || !Char.IsLetter(riferimento[i + 2]))) // non nel caso di mr,gv o mr,3g ma sì nel caso di mr,3,4
                        {
                            riferimento = riferimento.Remove(i, 1);
                        }
                    }
                }
            }
            // cancellare eventuali due punti alla fine o prima di punteggiatura (possibile con RIFTIPO_CITAZIONE)
            for (int i = riferimento.Length - 1; i >= 1; --i)
            {
                if (riferimento[i] == ':' && (i == riferimento.Length - 1 || (riferimento[i + 1] == ';' || riferimento[i + 1] == ',' || riferimento[i + 1] == '.')))
                {
                    riferimento = riferimento.Remove(i, 1);
                }
            }

            if ((formato.RiferimentoTipo == RiferimentoTipo.Virgola || formato.RiferimentoTipo == RiferimentoTipo.Citazione) && (riferimento.IndexOf(':') < 0 || riferimento.IndexOf(':') >= riferimento.Length - 2))
            {
                riferimento = riferimento.Replace(",", ":");
                riferimento = riferimento.Replace(".", ",");
                while (riferimento.Contains(';'))
                {
                    int dopoDivisore = riferimento.IndexOf(';') + 1; // controlla situazioni come Is 7,1-10;12 che viene tradotto in modo diverso
                    while (dopoDivisore <= riferimento.Length - 1 && ((Char.IsDigit(riferimento[dopoDivisore])) || riferimento[dopoDivisore] == ' '))
                    {
                        ++dopoDivisore;
                    }

                    if (dopoDivisore > riferimento.Length - 1 || (riferimento[dopoDivisore] != ':' && riferimento[dopoDivisore] != '.' && (!Char.IsLetter(riferimento[dopoDivisore]))))
                    {
                        riferimento = riferimento[1..dopoDivisore] + ":1-200" + riferimento[dopoDivisore..];
                    }

                    riferimento = riferimento.Replace(";", ",");
                }
            }

            int punteggiature, capitolo = 0;
            bool trattinoVecchio = true, trattino = false, versettoMancante = false;
            String riferimentoDaAnalizzare, libroNome = "";
            byte[] riferimentoBrano = [0, 0, 0, 0, 0, 0, 0, 0];
            byte[] riferimentoBranoPrecedente = [0, 0, 0, 0, 0, 0, 0, 0];
            byte[] riferimentoBrano4Byte = [0, 0, 0, 0, 0, 0, 0, 0];
            do
            {
                // troviamo il riferimento del primo brano, cioè fino alla prima punteggiatura
                punteggiature = riferimento.IndexOf(',');
                if (punteggiature < 0 || (riferimento.IndexOf(';') < punteggiature && riferimento.Contains(';')))
                {
                    punteggiature = riferimento.IndexOf(';');
                }

                if (punteggiature < 0 || (riferimento.IndexOf('-') < punteggiature && riferimento.Contains('-')))
                {
                    punteggiature = riferimento.IndexOf('-');
                    if (punteggiature >= 0)
                    {
                        trattino = true;
                    }
                }
                if (punteggiature >= 0)
                {
                    riferimentoDaAnalizzare = riferimento[..punteggiature]; // il riferimento del primo brano
                    riferimento = riferimento[(punteggiature + 1)..].Trim(); // il resto del riferimento, che analizzeremo più tardi
                }
                else
                {
                    riferimentoDaAnalizzare = riferimento;
                    riferimento = "";
                }
                riferimentoBrano = ConvertiRiferimentoDaTestoA4Byte(riferimentoDaAnalizzare, trattinoVecchio); // il primo brano, in formatto a 4 byte
                if (riferimentoBrano[0] == 0 && !string.IsNullOrEmpty(riferimentoDaAnalizzare) && (!Char.IsLetter(riferimentoDaAnalizzare[0])))
                {
                    if (!riferimentoDaAnalizzare.Contains(':') && !riferimentoDaAnalizzare.Contains('.') && !versettoMancante)
                    {
                        riferimentoDaAnalizzare = capitolo.ToString(CultureInfo.CurrentCulture) + ":" + riferimentoDaAnalizzare;
                    }

                    riferimentoDaAnalizzare = libroNome + riferimentoDaAnalizzare;
                    riferimentoBrano = ConvertiRiferimentoDaTestoA4Byte(riferimentoDaAnalizzare, trattinoVecchio);
                }
                versettoMancante = false;
                if (riferimentoBrano[0] > 0)
                {
                    riferimentoBrano4Byte = riferimentoBrano;
                    if (!riferimentoDaAnalizzare.Contains(':') && !riferimentoDaAnalizzare.Contains('.'))
                    {
                        versettoMancante = true;
                        if (trattino)
                        {
                            if (!string.IsNullOrEmpty(riferimento) && (!Char.IsLetter(riferimento[0])) && (riferimento.Length == 1 || (!Char.IsLetter(riferimento[1]))))
                            {
                                riferimento = libriAbbreviazioniUsate[riferimentoBrano4Byte[0]] + riferimento;
                            }
                        }
                        else
                        {
                            if (trattinoVecchio)
                            {
                                trattino = true;
                                riferimento = riferimentoDaAnalizzare + ";" + riferimento;
                            }
                        }
                    }
                    libroNome = libriAbbreviazioniUsate[riferimentoBrano4Byte[0]];
                    capitolo = riferimentoBrano4Byte[1];
                }
                if (!trattinoVecchio)
                {
                    riferimentoBrano[4] = riferimentoBrano[0];
                    riferimentoBrano[5] = riferimentoBrano[1];
                    riferimentoBrano[6] = riferimentoBrano[2];
                    riferimentoBrano[7] = riferimentoBrano[3];
                    riferimentoBrano[0] = riferimentoBranoPrecedente[0];
                    riferimentoBrano[1] = riferimentoBranoPrecedente[1];
                    riferimentoBrano[2] = riferimentoBranoPrecedente[2];
                    riferimentoBrano[3] = riferimentoBranoPrecedente[3];
                    trattinoVecchio = true;
                }
                else
                {
                    if (trattino)
                    {
                        trattinoVecchio = false;
                        trattino = false;
                        riferimentoBranoPrecedente[0] = riferimentoBrano[0];
                        riferimentoBranoPrecedente[1] = riferimentoBrano[1];
                        riferimentoBranoPrecedente[2] = riferimentoBrano[2];
                        riferimentoBranoPrecedente[3] = riferimentoBrano[3];
                    }
                    else
                    {
                        riferimentoBrano[4] = riferimentoBrano[0];
                        riferimentoBrano[5] = riferimentoBrano[1];
                        riferimentoBrano[6] = riferimentoBrano[2];
                        riferimentoBrano[7] = riferimentoBrano[3];
                    }
                }
                if (riferimentoBrano[0] > 0 && riferimentoBrano[4] > 0)
                {
                    nuovoRiferimento.AggiungiBrano8Byte(riferimentoBrano);
                }
            }
            while (!string.IsNullOrEmpty(riferimento));
            return nuovoRiferimento;
        }

        /// <summary>
        /// Trova tutti i riferimenti in una stringa.
        /// </summary>
        /// <param name="stringaDaAnalizzare">La stringa in cui cercare i riferimenti.</param>
        /// <returns>I riferimenti trovati, nel formato usato dal programma.</returns>
        public Riferimento ConvertiRiferimenti(string stringaDaAnalizzare)
        {
            string riferimentoTrovato = "";
            if (!string.IsNullOrEmpty(stringaDaAnalizzare))
            {
                char[] numeri = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
                int indice = stringaDaAnalizzare.IndexOfAny(numeri, 1);
                int primaLetteraDopo, primaLetteraPrima;
                while (indice > 0)
                {
                    primaLetteraDopo = indice + 1;
                    while (primaLetteraDopo < stringaDaAnalizzare.Length && !Char.IsLetter(stringaDaAnalizzare[primaLetteraDopo]))
                    {
                        ++primaLetteraDopo;
                    }

                    primaLetteraPrima = indice - 1;
                    //                    while (primaLetteraPrima > 0 && !Char.IsLetter(s[primaLetteraPrima]))
                    if (Char.IsWhiteSpace(stringaDaAnalizzare[primaLetteraPrima]))
                    {
                        while (primaLetteraPrima > 0 && Char.IsWhiteSpace(stringaDaAnalizzare[primaLetteraPrima]))
                        {
                            --primaLetteraPrima;
                        }
                        // adesso andiamo all'inizio di questa parola
                        while (primaLetteraPrima > 0 && Char.IsLetter(stringaDaAnalizzare[primaLetteraPrima - 1]))
                        {
                            --primaLetteraPrima;
                        }
                        // aggiustiamo per 1Giovanni eccetera
                        if (primaLetteraPrima > 0 && (stringaDaAnalizzare[primaLetteraPrima - 1] >= '1' && stringaDaAnalizzare[primaLetteraPrima - 1] <= '3'))
                        {
                            --primaLetteraPrima;
                        }

                        if (primaLetteraPrima > 1 && char.IsWhiteSpace(stringaDaAnalizzare[primaLetteraPrima - 1]) && (stringaDaAnalizzare[primaLetteraPrima - 2] >= '1' && stringaDaAnalizzare[primaLetteraPrima - 2] <= '3'))
                        {
                            primaLetteraPrima -= 2;
                        }

                        riferimentoTrovato += stringaDaAnalizzare[primaLetteraPrima..primaLetteraDopo] + ";";
                    }
                    indice = (primaLetteraDopo == stringaDaAnalizzare.Length ? -1 : stringaDaAnalizzare.IndexOfAny(numeri, primaLetteraDopo));
                }
            }
            Riferimento riferimento = ConvertiRiferimento(riferimentoTrovato);
            for (int i = riferimento.Count - 1; i >= 0; --i)
            {
                if (riferimento.Brani[i][4] == 255 && riferimento.Brani[i][5] == 255)
                {
                    riferimento.Rimuovi(i);
                }
            }
            return riferimento;
        }

        /// <summary>
        /// Converte il titolo di una nota che inizia con # ad una stringa con il riferimento in formato leggibile.
        /// </summary>
        /// <param name="notaDaConvertire">Il titolo di una nota.</param>
        /// <returns>Un riferimento come una stringa.</returns>
        public string ConvertiTitoloNotaARiferimento(string notaDaConvertire)
        {
            // tested
            // vedi anche Riferimento.ComeNota per l'altra direzione
            if (string.IsNullOrEmpty(notaDaConvertire))
            {
                return "";
            }

            string[] separatori = SeparatoriNeiRiferimenti();
            StringBuilder riferimento = new("");

            string[] note = SplitString(notaDaConvertire, '#');
            foreach (string nota in note)
            {
                try
                {
                    if (!string.IsNullOrEmpty(riferimento.ToString()))
                    {
                        riferimento.Append(';');
                    }
                    // nota non ha # all'inizio qui
                    byte libro1 = Convert.ToByte(nota[..2], CultureInfo.InvariantCulture);
                    riferimento.Append(libriAbbreviazioniUsate[libro1]);
                    int capitolo1 = Convert.ToInt32(nota.Substring(2, 3), CultureInfo.InvariantCulture);
                    int versetto1 = Convert.ToInt32(nota.Substring(5, 3), CultureInfo.InvariantCulture);
                    int numeroParola1 = Convert.ToInt32(nota.Substring(8, 4), CultureInfo.InvariantCulture);
                    byte capitoliInLibro1 = CapitoliInLibro(libro1, UltimaBibbiaCompleta);
                    if (capitolo1 > 0)
                    {
                        riferimento.Append(separatori[0]);
                        if (capitoliInLibro1 != 1)
                        {
                            riferimento.Append(capitolo1);
                        }

                        if (versetto1 > 0)
                        {
                            if (capitoliInLibro1 != 1)
                            {
                                riferimento.Append(separatori[1]);
                            }

                            riferimento.Append(versetto1);
                            if (numeroParola1 > 0)
                            {
                                riferimento.Append('/').Append(numeroParola1);
                            }
                        }
                    }

                    if (nota[..12] != nota.Substring(13, 12))
                    {
                        riferimento.Append('-');
                        byte libro2 = Convert.ToByte(nota.Substring(13, 2), CultureInfo.InvariantCulture);
                        int capitolo2 = Convert.ToInt32(nota.Substring(15, 3), CultureInfo.InvariantCulture);
                        int versetto2 = Convert.ToInt32(nota.Substring(18, 3), CultureInfo.InvariantCulture);
                        int numeroParola2 = Convert.ToInt32(nota.Substring(21, 4), CultureInfo.InvariantCulture);
                        byte capitoliInLibro2 = CapitoliInLibro(libro2, UltimaBibbiaCompleta);
                        if (libro2 != libro1)
                        {
                            riferimento.Append(libriAbbreviazioniUsate[libro2]);
                            if (capitolo2 > 0)
                            {
                                riferimento.Append(separatori[0]);
                                if (capitoliInLibro2 != 1)
                                {
                                    riferimento.Append(capitolo2);
                                }

                                if (versetto2 > 0)
                                {
                                    if (capitoliInLibro1 != 1)
                                    {
                                        riferimento.Append(separatori[1]);
                                    }

                                    riferimento.Append(versetto2);
                                    if (numeroParola2 > 0)
                                    {
                                        riferimento.Append('/').Append(numeroParola2);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (capitolo2 != capitolo1)
                            {
                                if (capitolo2 > 0)
                                {
                                    riferimento.Append(capitolo2);
                                    if (versetto2 > 0)
                                    {
                                        riferimento.Append(separatori[1]).Append(versetto2);
                                        if (numeroParola2 > 0)
                                        {
                                            riferimento.Append('/').Append(numeroParola2);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (versetto2 != versetto1 || numeroParola2 > 0)
                                { // aggiungi il numero del versetto se c'è la parola, altrimenti c'è un riferimento ambiguo come Gen 1:2/3-4 invece di Gen 1:2/3-2/4.
                                    if (versetto2 > 0)
                                    {
                                        riferimento.Append(versetto2);
                                        if (numeroParola2 > 0)
                                        {
                                            riferimento.Append('/').Append(numeroParola2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // se c'è un errore nel formato, saltiamo
                }
            }
            return riferimento.ToString();
        }

        private byte[] ConvertiRiferimentoDaTestoA4Byte(string riferimentoTestuale, bool primaDelTrattino)
        {
            // convertire a 4 interi un riferimento di un versetto+parola
            // se primaDelTrattino = false, il riferimento va dopo il trattino
            byte[] riferimentoRestituito = [0, 0, 0, 0, 0, 0, 0, 0];
            int primaNonLettera = -1;
            String riferimento = riferimentoTestuale.ToLower(CultureInfo.CurrentCulture).Trim();
            if (string.IsNullOrEmpty(riferimento))
            {
                return riferimentoRestituito;
            }

            String nomeLibro = "";
            if (riferimento[0] >= '1' && riferimento[0] <= '3')
            {
                nomeLibro = riferimento[..1];
                riferimento = riferimento[1..].Trim();
            }

            do
            {
                ++primaNonLettera;
            }
            while (primaNonLettera < riferimento.Length - 1 && Char.IsLetter(riferimento[primaNonLettera]));

            String riferimentoRimanente = "";
            byte capitolo = 0, versetto = 0, parola = 0;
            if (primaNonLettera == riferimento.Length - 1 && Char.IsLetter(riferimento[^1]))
            {
                nomeLibro += riferimento;
            }
            else
            {
                nomeLibro += riferimento[..primaNonLettera];
                riferimentoRimanente = riferimento[primaNonLettera..].Trim();
                StringBuilder capitoloNumerico = new("");
                for (int j = 0; j < riferimentoRimanente.Length && Char.IsDigit(riferimentoRimanente[j]); ++j)
                {
                    capitoloNumerico.Append(riferimentoRimanente.AsSpan(j, 1));
                }

                try
                {
                    capitolo = Convert.ToByte(capitoloNumerico.ToString(), CultureInfo.InvariantCulture);
                }
                catch (OverflowException)
                {
                    capitolo = 0;
                }
                catch (ArgumentException)
                {
                    capitolo = 0;
                }
                catch (FormatException)
                {
                    capitolo = 0;
                }
            }

            if (!string.IsNullOrEmpty(riferimentoRimanente))
            {
                int posDivisoreCapitoloVersetto = riferimentoRimanente.IndexOf(':');
                if (posDivisoreCapitoloVersetto == -1 || (riferimentoRimanente.IndexOf('.') < posDivisoreCapitoloVersetto && riferimentoRimanente.Contains('.')))
                {
                    posDivisoreCapitoloVersetto = riferimentoRimanente.IndexOf('.');
                }

                if ((formato.RiferimentoTipo == RiferimentoTipo.Virgola || formato.RiferimentoTipo == RiferimentoTipo.Citazione) && (posDivisoreCapitoloVersetto == -1 || (riferimentoRimanente.IndexOf(',') < posDivisoreCapitoloVersetto && riferimentoRimanente.Contains(','))))
                {
                    posDivisoreCapitoloVersetto = riferimentoRimanente.IndexOf(',');
                }

                if (posDivisoreCapitoloVersetto >= 0)
                {
                    riferimentoRimanente = riferimentoRimanente[(posDivisoreCapitoloVersetto + 1)..].Trim();
                }
                else
                {
                    riferimentoRimanente = "";
                }

                StringBuilder versettoNumerico = new("");
                for (int j = 0; j < riferimentoRimanente.Length && Char.IsDigit(riferimentoRimanente[j]); ++j)
                {
                    versettoNumerico.Append(riferimentoRimanente.AsSpan(j, 1));
                }

                try
                {
                    versetto = Convert.ToByte(versettoNumerico.ToString(), CultureInfo.InvariantCulture);
                }
                catch (OverflowException)
                {
                    versetto = 0;
                }
                catch (ArgumentException)
                {
                    versetto = 0;
                }
                catch (FormatException)
                {
                    versetto = 0;
                }
            }

            // trovare eventuale parola dopo /
            if (!string.IsNullOrEmpty(riferimentoRimanente))
            {
                int posDivisoreVersettoParola = riferimentoRimanente.IndexOf('/');
                if (posDivisoreVersettoParola >= 0)
                {
                    riferimentoRimanente = riferimentoRimanente[(posDivisoreVersettoParola + 1)..].Trim();
                }
                else
                {
                    riferimentoRimanente = "";
                }

                StringBuilder parolaNumerico = new("");
                for (int j = 0; j < riferimentoRimanente.Length && Char.IsDigit(riferimentoRimanente[j]); ++j)
                {
                    parolaNumerico.Append(riferimentoRimanente.AsSpan(j, 1));
                }

                try
                {
                    parola = Convert.ToByte(parolaNumerico.ToString(), CultureInfo.InvariantCulture);
                }
                catch (OverflowException)
                {
                    parola = 0;
                }
                catch (ArgumentException)
                {
                    parola = 0;
                }
                catch (FormatException)
                {
                    parola = 0;
                }
            }

            byte libro = GetLibroNumeroDaAbbreviazione(nomeLibro);

            if (libro > 0)
            {
                riferimentoRestituito[0] = libro;
                if ((libro == 38 || libro == 64 || libro == 70 || libro == 71 || libro == 72) && versetto == 0)
                {
                    versetto = capitolo;
                    capitolo = 1;
                }
                if (capitolo == 0)
                {
                    if (primaDelTrattino)
                    {
                        riferimentoRestituito[1] = 1;
                        riferimentoRestituito[2] = 1;
                    }
                    else
                    {
                        riferimentoRestituito[1] = 255;
                        riferimentoRestituito[2] = 255;
                    }
                } // if (iCapitolo==0)
                else
                {
                    riferimentoRestituito[1] = capitolo;
                    if (versetto == 0)
                    {
                        if (primaDelTrattino)
                        {
                            versetto = 1;
                        }
                        else
                        {
                            versetto = 255;
                        }
                    }
                    riferimentoRestituito[2] = versetto;
                }
            } // if (!string.IsNullOrEmpty(rifOut))

            riferimentoRestituito[3] = parola;

            return riferimentoRestituito;
        }

        private string ConvertiRiferimentoDa3ByteATesto(byte[] rif, RiferimentoFormato rf)
        {
            if (rf == RiferimentoFormato.Nessuno)
            {
                return "";
            }

            String riferimentoTestuale = "";
            byte libro1 = rif[0];
            byte capitolo1 = rif[1];
            byte versetto1 = rif[2];
            byte libro2 = rif[3];
            byte capitolo2 = rif[4];
            byte versetto2 = rif[5];

            string dopoLibro = (formato.RiferimentoTipo == RiferimentoTipo.Citazione ? "., " : " ");
            if (rf == RiferimentoFormato.Intero)
            {
                riferimentoTestuale = libriNomi[libro1] + dopoLibro;
            }
            else if (rf == RiferimentoFormato.Abbreviazione)
            {
                riferimentoTestuale = libriAbbreviazioniUsate[libro1] + dopoLibro;
            }
            else if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta)
            {
                riferimentoTestuale = libriAbbreviazioniRiconosciute.Abbreviazione(libro1);
                if (riferimentoTestuale.IndexOf(',') < 0)
                {
                    riferimentoTestuale += dopoLibro;
                }
                else
                {
                    riferimentoTestuale = riferimentoTestuale[..riferimentoTestuale.IndexOf(',')] + dopoLibro;
                }
            }

            string[] separatori = SeparatoriNeiRiferimenti();
            StringBuilder rifSB = new(riferimentoTestuale);

            if (capitolo1 == 1 && capitolo2 == 255)
            {
                if (libro1 == libro2)
                { // Gv
                    //rifSB += "";
                }
                else
                { // Gv-At
                    rifSB.Append('-');
                    if (rf == RiferimentoFormato.Intero)
                    {
                        rifSB.Append(libriNomi[libro2]);
                    }
                    else if (rf == RiferimentoFormato.Abbreviazione)
                    {
                        rifSB.Append(libriAbbreviazioniUsate[libro2]);
                    }
                    else if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta)
                    {
                        string s = libriAbbreviazioniRiconosciute.Abbreviazione(libro2);
                        rifSB.Append(s[..s.IndexOf(',')]);
                    }
                }
            }
            else
            {
                if (versetto1 == 1 && versetto2 == 255)
                {
                    if (libro1 == 38 || libro1 == 64 || libro1 == 70 || libro1 == 71 || libro1 == 72)
                    {
                        //rifSB += "";
                    }
                    else
                    {
                        rifSB.Append(ByteStringhe[capitolo1]);
                    }

                    if (libro1 == libro2)
                    {
                        if (capitolo1 == capitolo2)
                        { // Gv 4
                            //rifSB += "";
                        }
                        else // Gv 4-5
                        {
                            rifSB.Append('-').Append(ByteStringhe[capitolo2]);
                        }
                    }
                    else
                    { // Gv 4-At 3
                        rifSB.Append('-');
                        if (rf == RiferimentoFormato.Intero)
                        {
                            rifSB.Append(libriNomi[libro2]).Append(dopoLibro);
                        }
                        else if (rf == RiferimentoFormato.Abbreviazione)
                        {
                            rifSB.Append(libriAbbreviazioniUsate[libro2]).Append(dopoLibro);
                        }
                        else if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta)
                        {
                            string s = libriAbbreviazioniRiconosciute.Abbreviazione(libro2);
                            rifSB.Append(s[..s.IndexOf(',')]).Append(dopoLibro);
                        }
                        if (libro2 == 38 || libro2 == 64 || libro2 == 70 || libro2 == 71 || libro2 == 72)
                        {
                            //rifSB += "";
                        }
                        else
                        {
                            rifSB.Append(ByteStringhe[capitolo2]);
                        }
                    }
                }
                else
                {
                    if (libro1 == 38 || libro1 == 64 || libro1 == 70 || libro1 == 71 || libro1 == 72)
                    {
                        rifSB.Append(ByteStringhe[versetto1]);
                    }
                    else
                    {
                        rifSB.Append(ByteStringhe[capitolo1]).Append(separatori[1]).Append(ByteStringhe[versetto1]);
                    }

                    if (libro1 == libro2)
                    {
                        if (capitolo1 == capitolo2)
                        {
                            if (versetto1 != versetto2)
                            {
                                rifSB.Append('-').Append(ByteStringhe[versetto2]);
                            }
                        }
                        else
                        {
                            rifSB.Append('-').Append(ByteStringhe[capitolo2]).Append(separatori[1]).Append(ByteStringhe[versetto2]);
                        }
                    }
                    else
                    {
                        rifSB.Append('-');
                        if (rf == RiferimentoFormato.Intero)
                        {
                            rifSB.Append(libriNomi[libro2]).Append(dopoLibro);
                        }
                        else if (rf == RiferimentoFormato.Abbreviazione)
                        {
                            rifSB.Append(libriAbbreviazioniUsate[libro2]).Append(dopoLibro);
                        }
                        else if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta)
                        {
                            string s = libriAbbreviazioniRiconosciute.Abbreviazione(libro2);
                            rifSB.Append(s[..s.IndexOf(',')]).Append(dopoLibro);
                        }
                        if (libro2 == 38 || libro2 == 64 || libro2 == 70 || libro2 == 71 || libro2 == 72)
                        {
                            rifSB.Append(ByteStringhe[versetto2]);
                        }
                        else
                        {
                            rifSB.Append(ByteStringhe[capitolo2]).Append(separatori[1]).Append(ByteStringhe[versetto2]);
                        }
                    }
                }
            }

            if (formato.RiferimentoTipo == RiferimentoTipo.Citazione)
            {
                rifSB.Append(':');
            }

            return rifSB.ToString().Trim().Replace(" -", "-");
        }

        /// <summary>
        /// I caratteri da mettere nei riferimenti, secondo le opzioni.
        /// </summary>
        /// <returns>Un array di tre stringhe: la prima è fra il libro e il capitolo, la seconda fra il capitolo e il versetto, la terza fra due versetti.</returns>
        public string[] SeparatoriNeiRiferimenti()
        {
            string[] separatori = new string[3];
            switch (formato.RiferimentoTipo)
            {
                case RiferimentoTipo.Virgola:
                    separatori[0] = " ";
                    separatori[1] = ",";
                    separatori[2] = ".";
                    break;
                case RiferimentoTipo.Citazione:
                    separatori[0] = ((formato.RiferimentoFormato == RiferimentoFormato.Abbreviazione) ? "., " : ", ");
                    separatori[1] = ", ";
                    separatori[2] = ".";
                    break;
                default: // DuePunti o valori illegali
                    separatori[0] = " ";
                    separatori[1] = ":";
                    separatori[2] = ",";
                    break;
            }
            return separatori;
        }

        #endregion

        #region Funzioni per le parole

        /// <summary>
        /// Tutte le parole che appaiono in un testo.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Un array di stringhe con tutte le parole.</returns>
        public string[] Parole(string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].Parole;
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Se la versione ha delle radici.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Vero se esistono la radici.</returns>
        public bool EsistonoRadici(string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].EsistonoRadici();
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Tutte le radici che appaiono in un testo.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Un array di stringhe con tutte le radici.</returns>
        public string[] Radici(string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].Radici;
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// La radice di una certa parola in un testo.
        /// </summary>
        /// <param name="parola">La parola di cui si vuole la radice.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>La radice della parola.</returns>
        public string RadiceDiParola(string parola, string nomeVersione)
        {
            // tested
            if (nomeVersione == null)
            {
                return parola;
            }
            else
            {
                try
                {
                    return versioni[nomeVersione].RadiceDiParola(parola);
                }
                catch (KeyNotFoundException)
                {
                    throw new TextNotExistException();
                }
            }
        }

        /// <summary>
        /// Tutte le parole di una certa radice che appaiono in un testo.
        /// </summary>
        /// <param name="radice">La radice di cui si vogliono le parole.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Una collezione di stringhe con le parole.</returns>
        public Collection<string> ParoleDiRadice(string radice, string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].ParoleDiRadice(radice);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Restituisce quante volte una parola appare in una versione.
        /// </summary>
        /// <param name="parola">La parola da ricercare.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero di volte.</returns>
        public int NumeroVolteParola(string parola, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].NumeroVolteParola(parola);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Restituisce quante volte una radice appare in una versione.
        /// </summary>
        /// <param name="radice">La radice da ricercare.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero di volte.</returns>
        public int NumeroVolteRadice(string radice, string nomeVersione)
        {
            // tested
            try
            {
                return versioni[nomeVersione].NumeroVolteRadice(radice);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Aggiunge delle radici ad un testo.
        /// </summary>
        /// <param name="elencoRadici">Un array di tutte le radici nel testo.</param>
        /// <param name="radiceStringaDiParole">La radice di ogni parola nel testo.</param>
        /// <param name="nomeVersione">Il nome della testo.</param>
        public void AggiungiRadiciAllaVersione(string[] elencoRadici, string[] radiceStringaDiParole, string nomeVersione)
        {
            try
            {
                versioni[nomeVersione].AggiungiRadiciAllaVersione(elencoRadici, radiceStringaDiParole);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Restituisce tutte le apparenze di tutte le parole.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns></returns>
        public byte[] GetApparenzeParole(string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].GetApparenzeParole();
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        #endregion

        #region Funzioni per le note

        #region Get/Set NotaTesto

        /// <summary>
        /// Trova una nota con un certo titolo.
        /// </summary>
        /// <param name="titolo">Il titolo da cercare.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il numero della nota se esiste una nota con quel titolo, altrimenti un numero negativo.</returns>
        public int GetNumeroNotaTitolo(string titolo, String nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].GetNumeroNotaTitolo(titolo);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Il testo di una nota con un certo titolo; può essere in formato RTF o testo normale.
        /// </summary>
        /// <param name="titolo">Il titolo della nota.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il testo della nota.</returns>
        public string GetNotaTesto(string titolo, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].GetNotaTestoTitolo(titolo);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Il testo di tutte le note su un certo titolo.
        /// </summary>
        /// <param name="titolo">Il titolo della nota.</param>
        /// <returns>Il testo delle note.</returns>
        public string GetTutteLeNote(string titolo)
        {
            return GetTutteLeNote(titolo, "");
        }

        /// <summary>
        /// Il testo di tutte le note su un certo titolo.
        /// </summary>
        /// <param name="titolo">Il titolo della nota.</param>
        /// <param name="radice">Il radice del titolo, da cercare se il titolo non ha una nota.</param>
        /// <returns>Il testo delle note.</returns>
        public string GetTutteLeNote(string titolo, string radice)
        {
            // non è necessario fare qualcosa di simile con un riferimento, perché si può usare Testo
            RichTextBoxEx rtb = new();
            string testo, versione;
            Collection<string> collezioniDaVisualizzare = NomiVersioni(TestoTipi.Dizionario);
            for (int i = 0; i < collezioniDaVisualizzare.Count; ++i)
            {
                versione = collezioniDaVisualizzare[i];
                testo = GetNotaTesto(titolo, versione);
                if (string.IsNullOrEmpty(testo) && !string.IsNullOrEmpty(radice))
                {
                    testo = GetNotaTesto(radice, versione);
                }

                if (!string.IsNullOrEmpty(testo))
                {
                    rtb.AggiungiRtf(RtfIntestazione() + @"\fs28\b " + collezioniDaVisualizzare[i] + @"\par}");
                    try
                    {
                        rtb.AggiungiRtf(testo);
                    }
                    catch
                    {
                        rtb.AppendText(testo);
                    }
                }
            }
            testo = rtb.Rtf;
            if (testo.EndsWith("\\par\r\n}\r\n", StringComparison.Ordinal))
            {
                testo = testo[..^9] + "\r\n}\r\n";
            }

            return testo;
        }

        /// <summary>
        /// Il testo di una nota con un certo riferimento; può essere in formato RTF o testo normale.
        /// </summary>
        /// <param name="riferimento">Il riferimento della nota.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il testo della nota.</returns>
        public string GetNotaTesto(Riferimento riferimento, string nomeVersione)
        {
            return riferimento == null ? "" : GetNotaTesto(riferimento.ComeNotaTuttoRiferimento(), nomeVersione);
        }

        /// <summary>
        /// Cambia il testo di una nota con un certo titolo; può essere in formato RTF o testo normale.
        /// </summary>
        /// <param name="testo">Il nuovo testo della nota.</param>
        /// <param name="titolo">Il titolo della nota.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        public void SetNotaTesto(string testo, string titolo, string nomeVersione)
        {
            try
            {
                versioni[nomeVersione].SetNotaTesto(testo, titolo);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Cambia il testo di una nota con un certo riferimento; può essere in formato RTF o testo normale.
        /// </summary>
        /// <param name="testo">Il nuovo testo della nota.</param>
        /// <param name="riferimento">Il riferimento della nota.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        public void SetNotaTesto(string testo, Riferimento riferimento, string nomeVersione)
        {
            if (riferimento != null)
            {
                SetNotaTesto(testo, riferimento.ComeNotaTuttoRiferimento(), nomeVersione);
            }
        }

        #endregion

        /// <summary>
        /// Restituisce un elenco di tutte le note.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Una collezione con i nomi di tutte le note.</returns>
        public Collection<string> Note(string nomeVersione)
        {
            try
            {
                List<string> note = [.. versioni[nomeVersione].NoteTitoli];
                return new Collection<string>(note);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Restituisce un elenco di tutte le note con un titolo.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Una collezione di tutti i titoli.</returns>
        public Collection<string> NoteConTitolo(string nomeVersione)
        {
            try
            {
                Collection<string> note = [];
                int numeroNote = versioni[nomeVersione].NoteTitoli.Count;
                for (int i = 0; i < numeroNote; ++i)
                {
                    if (!versioni[nomeVersione].NoteTitoli[i].StartsWith('#'))
                    {
                        note.Add(versioni[nomeVersione].NoteTitoli[i]);
                    }
                }

                return note;
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Restituisce un elenco di tutte le note, con quelle ordinate all'inizio dell'elenco.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <param name="conNoteSuBrani">Se include anche i commenti nell'elenco, oppure solo quelli del dizionario su un tema.</param>
        /// <returns>Una collezione con i nomi di tutte le note.</returns>
        public Collection<string> NotePrimaOrdinate(string nomeVersione, bool conNoteSuBrani)
        {
            Collection<string> titoli = [];
            if (!String.IsNullOrEmpty(nomeVersione))
            {
                Collection<string> noteInOrdine = GetNoteInOrdine(nomeVersione);
                List<string> note = [.. Note(nomeVersione)];
                ConfrontoCI confronto = new();
                note.Sort(confronto);

                // aggiungere prima le note in ordine, poi le altre note in ordine alfabetico
                int indiceNota;
                string notaSenzaTab;
                char[] trimTab = ['\t'];

                foreach (string nota in noteInOrdine)
                {
                    if (!string.IsNullOrEmpty(nota))
                    {
                        notaSenzaTab = nota.TrimStart(trimTab); // possono essere note dalle note in ordine, ma senza l'indentazione (indicata da una tabulazione) rimossa
                        if (!string.IsNullOrEmpty(notaSenzaTab) && (conNoteSuBrani || !notaSenzaTab.StartsWith('#')))
                        {
                            titoli.Add(notaSenzaTab);
                            indiceNota = note.BinarySearch(notaSenzaTab, confronto);
                            if (indiceNota > -1)
                            {
                                note.RemoveAt(indiceNota);
                            }
                        }
                    }
                }

                foreach (string nota in note)
                {
                    if (!string.IsNullOrEmpty(nota) && (conNoteSuBrani || !nota.StartsWith('#')))
                    {
                        titoli.Add(nota);
                    }
                }
            }
            return titoli;
        }

        /// <summary>
        /// Restituisce un elenco di tutte le note in ordine.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Il primo elemento è l'indice, gli altri l'ordine delle note.</returns>
        public Collection<string> GetNoteInOrdine(string nomeVersione)
        {
            try
            {
                List<string> note = [.. versioni[nomeVersione].noteInOrdine];
                return new Collection<string>(note);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Impone l'ordine delle note in una collezione.
        /// </summary>
        /// <param name="noteInOrdine">Una collezione: il primo elemento è l'indice, gli altri l'ordine delle note.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        public void SetNoteInOrdine(Collection<string> noteInOrdine, string nomeVersione)
        {
            try
            {
                versioni[nomeVersione].SetNoteInOrdine(noteInOrdine);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Un elenco di tutte le note che contengono un certo riferimento.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano da cercare.</param>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <returns>Un riferimento con tutte le note.</returns>
        public Riferimento ElencaNoteInBrano(Riferimento riferimento, string nomeVersione)
        {
            return riferimento == null ? new Riferimento(false) : versioni[nomeVersione].ElencaNoteInBrano(riferimento);
        }

        /// <summary>
        /// Restituisce la nota precedente e la nota successiva ad una data nota.
        /// </summary>
        /// <param name="nomeVersione">Il nome della versione.</param>
        /// <param name="titolo">Il titolo della nota.</param>
        /// <returns>Un array con due stringhe, con i titoli delle note precedente e successiva.</returns>
        public string[] NotePrecedenteSuccessiva(string nomeVersione, string titolo)
        {
            Collection<string> ordine = GetNoteInOrdine(nomeVersione);
            for (int i = 0; i < ordine.Count; ++i)
            {
                ordine[i] = ordine[i].TrimStart();
            }

            if (ordine.Count < 2)
            {
                return ["", ""];
            }

            ordine.RemoveAt(0);
            int indice = ordine.IndexOf(titolo);

            int indicePrecedente = indice - 1;
            while (indicePrecedente >= 0)
            {
                if (!string.IsNullOrEmpty(GetNotaTesto(ordine[indicePrecedente], nomeVersione)))
                {
                    break;
                }

                --indicePrecedente;
            }

            int indiceSuccessivo = (indice >= 0 ? indice + 1 : ordine.Count);
            while (indiceSuccessivo < ordine.Count)
            {
                if (!string.IsNullOrEmpty(GetNotaTesto(ordine[indiceSuccessivo], nomeVersione)))
                {
                    break;
                }

                ++indiceSuccessivo;
            }
            return [((indicePrecedente >= 0) ? ordine[indicePrecedente] : ""), ((indiceSuccessivo < ordine.Count) ? ordine[indiceSuccessivo] : "")];
        }

        /// <summary>
        /// Scrive una collezione di note ad un file del programma.
        /// </summary>
        /// <param name="bw">Un binary writer dove i dati saranno scritti.</param>
        /// <param name="posizioneInizioDati">La posizione nel file in cui i dati iniziano.</param>
        /// <param name="noteTitolo">I titoli delle note</param>
        /// <param name="noteTesto">I testi delle note (formato RTF o testo normale)</param>
        /// <returns>Due interi senza segno, con la posizione dell'inizio dei titoli e la posizione dell'inizio dell'indice delle note, sempre relativo a pInizioDati</returns>
        public static UInt32[] ScriviNote(BinaryWriter bw, UInt32 posizioneInizioDati, string[] noteTitolo, string[] noteTesto)
        {
            ArgumentNullException.ThrowIfNull(bw);

            ArgumentNullException.ThrowIfNull(noteTitolo);

            ArgumentNullException.ThrowIfNull(noteTesto);

            UInt32[] indici = new UInt32[2];

            int numeroNote = noteTitolo.Length;
            StringBuilder titoliNote = new("");
            UInt32[] posizioniNote = new UInt32[numeroNote];
            for (int i = 0; i < numeroNote; ++i)
            {
                if (!string.IsNullOrEmpty(noteTitolo[i]))
                {
                    titoliNote.Append(noteTitolo[i]).Append('|');
                    posizioniNote[i] = (UInt32)(bw.Seek(0, SeekOrigin.Current));
                    bw.Write(noteTesto[i]);
                }
            }

            indici[0] = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - posizioneInizioDati; // diventa inizioTestoIndiceLC
            bw.Write(titoliNote.ToString());
            indici[1] = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - posizioneInizioDati; // diventa inizioTestoIndice
            for (int i = 0; i < numeroNote; ++i)
            {
                if (!string.IsNullOrEmpty(noteTitolo[i]))
                {
                    bw.Write(posizioniNote[i] - posizioniNote[0]);
                }
            }

            return indici;
        }

        /// <summary>
        /// Scrivi un indice dei riferimenti citati in una collezione alla posizione attuale del file.
        /// </summary>
        /// <param name="bw">Un binary writer dove i dati saranno scritti.</param>
        /// <param name="noteTesto">I testi delle note (formato RTF o testo normale)</param>
        /// <returns>Falso se non c'erano riferimenti citati nella collezione.</returns>
        public bool ScriviRiferimentiCitati(BinaryWriter bw, string[] noteTesto)
        {
            int posizione1 = (int)(bw.Seek(0, SeekOrigin.Current));
            List<Riferimento> riferimenti = [];
            UInt32 numeroCitazioni = 0;
            bw.Write(numeroCitazioni); // il valore vero sarà scritto più avanti in questa routine
            for (UInt32 i = 0; i < noteTesto.Length; ++i)
            {
                riferimenti = TrovaRiferimentiInVoce(noteTesto[i]);
                for (int j = 0; j < riferimenti.Count; ++j)
                {
                    bw.Write(riferimenti[j].Brani[0]);
                    bw.Write(i);
                    ++numeroCitazioni;
                }
            }
            if (numeroCitazioni > 0)
            {
                int posizione2 = (int)(bw.Seek(0, SeekOrigin.Current));
                bw.Seek(posizione1, SeekOrigin.Begin);
                bw.Write(numeroCitazioni);
                bw.Seek(posizione2, SeekOrigin.Begin);
            }
            return (numeroCitazioni > 0);
        }

        /// <summary>
        /// Analizza un testo per aggiungere le sue parole ad una corcordanza.
        /// </summary>
        /// <param name="testo">Il testo da analizzare.</param>
        /// <param name="numeroVoce">Il numero del testo nella versione della Bibbia o nella collezione di note.</param>
        /// <param name="chiave">La chiave a cui aggiungere le parole.</param>
        /// <param name="lingua">Le lingue (separate da una riga verticale |) delle parole (necessaria per decidere la fine di una parola con apostrofe).</param>
        public static SortedDictionary<string, List<OccorrenzaParola>> TrovaParoleInVoce(string testo, UInt32 numeroVoce, SortedDictionary<string, List<OccorrenzaParola>> chiave, string lingua)
        {
            string[] lingue = lingua.ToLower(CultureInfo.InvariantCulture).Split(separator, StringSplitOptions.RemoveEmptyEntries);

            while (testo.Contains(RichTextBoxEx.InizioLink.ToString()))
            {
                testo = testo.Remove(testo.IndexOf(RichTextBoxEx.InizioLink.ToString(), StringComparison.Ordinal), 1);
            }

            int invisibileInizio = testo.IndexOf(RichTextBoxEx.FineLink1.ToString(), StringComparison.Ordinal);
            while (invisibileInizio >= 1)
            {
                int invisibileFine = testo.IndexOf(RichTextBoxEx.FineLink2.ToString(), invisibileInizio, StringComparison.Ordinal);
                if (invisibileFine >= 0)
                {
                    testo = testo.Remove(invisibileInizio, invisibileFine - invisibileInizio + 1);
                    invisibileInizio = testo.IndexOf(RichTextBoxEx.FineLink1.ToString(), StringComparison.Ordinal);
                }
                else
                {
                    invisibileInizio = -1; // problema con i link; basta uscire e non analizzarli più
                }
            }
            testo = testo.Replace("’", "'");
            testo = testo.Replace(@"\rquote ", "'");

            OccorrenzaParola vp = new()
            {
                Voce = numeroVoce
            };
            UInt16 nParola = 0;
            int nCaratteri = testo.Length;
            string parola = "", linguaDaUsare, linguaPrincipale = (lingue.Length >= 1 ? lingue[0] : "");
            bool analizzaParola;
            bool dizionarioGreco = (linguaPrincipale == "el" && lingue.Length >= 2);
            bool dizionarioEbraico = (linguaPrincipale.StartsWith("he") && lingue.Length >= 2);
            char c;

            for (int i = 0; i < nCaratteri; ++i)
            {
                c = testo[i];
                if (IsLetteraONumero(c) || c == '') // ACI 154
                {
                    parola += c;
                }
                else if (Char.IsPunctuation(c) || Char.IsWhiteSpace(c) || Char.IsSymbol(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.Format || c <= RichTextBoxEx.FineLinkFile || c == '' || c == '' || c == '' || c == '') // ASCII 144, 145 e 151, 154
                {
                    analizzaParola = true;
                    if (c == '\'' || c == '' || c == '') // ACII 145 e 146
                    {
                        // in un dizionario greco-altra lingua, dobbiamo scegliere la lingua giusta
                        linguaDaUsare = linguaPrincipale;
                        if (dizionarioGreco && i > 0 && !IsLetteraGreca(testo[i - 1]))
                        {
                            linguaDaUsare = lingue[1];
                        }
                        else if (dizionarioEbraico && i > 0 && !IsLetteraEbraica(testo[i - 1]))
                        {
                            linguaDaUsare = lingue[1];
                        }

                        if (linguaDaUsare.Length > 2)
                        {
                            linguaDaUsare = linguaDaUsare[..2];
                        }

                        switch (linguaDaUsare)
                        {
                            case "en":
                                if ((i == 0 || !IsLetteraONumero(testo[i - 1]))
                                    && ((i < nCaratteri - 1 && (testo[i + 1] == 't' || testo[i + 1] == 'T') && (i == nCaratteri - 2 || !IsLetteraONumero(testo[i + 2])))
                                      || (i < nCaratteri - 3 && testo.Substring(i + 1, 3).ToLower(CultureInfo.InvariantCulture) == "tis" && (i == nCaratteri - 4 || !IsLetteraONumero(testo[i + 4])))
                                      || (i < nCaratteri - 4 && testo.Substring(i + 1, 4).ToLower(CultureInfo.InvariantCulture) == "twas" && (i == nCaratteri - 5 || !IsLetteraONumero(testo[i + 5])))))
                                {
                                    parola += c;
                                    analizzaParola = false;
                                }
                                else if (i >= 2)
                                {
                                    if (i < nCaratteri - 1 &&
                                        (IsLetteraONumero(testo[i - 1])
                                            && char.IsLetter(testo[i + 1])
                                            && (i == testo.Length - 2 || !IsLetteraONumero(testo[i + 2]))))
                                    {
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                    else if (dizionarioEbraico && i < nCaratteri - 1 && (char.IsLetter(testo[i - 1]) && testo[i + 1] == '-'))
                                    { // per il dizionario Strong's Hebrew, che ha pronunce come eh'-sheth
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                    else if (testo.Substring(i - 2, 2).ToLowerInvariant() == "ba" && i < nCaratteri - 1 && char.IsLetter(testo[i + 1]))
                                    { // per la Literal Standard Version
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                    else if ((testo[i - 1] == 's' || testo[i - 1] == 'S')
                                        && (i == nCaratteri - 1 || !char.IsPunctuation(testo[i + 1]))
                                        && Array.BinarySearch(paroleInglesiSenzaApostrofe, parola, confrontoParole) < 0)
                                    {
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                    else if (IsLetteraGreca(testo[i - 1]) && (i == nCaratteri - 1 || char.IsPunctuation(testo[i + 1]) || char.IsWhiteSpace(testo[i + 1])))
                                    {
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                    else if (i < nCaratteri - 2
                                        && IsLetteraONumero(testo[i - 1]) && (i == testo.Length - 3 || !IsLetteraONumero(testo[i + 3]))
                                   && (testo.Substring(i + 1, 2) == "en" || testo.Substring(i + 1, 2) == "er" || testo.Substring(i + 1, 2) == "ll" || testo.Substring(i + 1, 2) == "lt" || testo.Substring(i + 1, 2) == "ry" || testo.Substring(i + 1, 2) == "st" || testo.Substring(i + 1, 2) == "ve"))
                                    {
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                    else if (i < nCaratteri - 4
                                   && IsLetteraONumero(testo[i - 1]) && (i == testo.Length - 5 || !IsLetteraONumero(testo[i + 5]))
                                   && (testo.Substring(i + 1, 4) == "ring"))
                                    {
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                }
                                break;
                            case "it":
                                if (i > 0 && i < nCaratteri - 1)
                                {
                                    if ((IsLetteraONumero(testo[i - 1]) && (IsLetteraONumero(testo[i + 1]) || testo[i + 1] == '\'' || testo[i + 1] == '«' || testo[i + 1] == '“' || testo[i + 1] == ']')) || (Array.BinarySearch(paroleItalianeConApostrofe, parola, confrontoParole) >= 0))
                                    {
                                        // per esempio l'uomo, l''Italica'
                                        parola += c;
                                    }
                                }
                                break;
                            case "el":
                                if (i > 0)
                                {
                                    if (IsLetteraGreca(testo[i - 1]))
                                    {
                                        parola += c;
                                    }
                                    else if (i < nCaratteri - 1 && char.IsLetter(testo[i - 1]) && char.IsLetter(testo[i + 1]))
                                    {
                                        parola += c;
                                        analizzaParola = false;
                                    }
                                    else if (char.IsLetter(testo[i - 1]) && (i == nCaratteri - 1 || (i < nCaratteri - 1 && !char.IsLetter(testo[i + 1]))))
                                    { // parola che finisce con apostrofe in greco trasliterato
                                        parola += c;
                                    }
                                }
                                break;
                            case "": // interlineare
                            case "he-t": // usato nell'ebraico traslitterato
                                parola += c;
                                break;
                        }
                    }
                    if ((c == '[' || c == ']'))
                    {
                        if (i > 0 && i < nCaratteri - 1)
                        {
                            if (IsLettera(testo[i - 1]) && IsLettera(testo[i + 1]))
                            {
                                // parentesi quadrate in mezzo ad una parola
                                analizzaParola = false;
                            }
                        }
                    }
                    if (c == '-' || c == '') // ASCII 45 e 151
                    {
                        if (i > 0 && i < nCaratteri - 1)
                        {
                            if (IsLettera(testo[i - 1]) && IsLettera(testo[i + 1])  // per esempio Eben-Ezer
                                || (dizionarioEbraico && testo[i - 1] == '\'' && char.IsLetter(testo[i + 1]))) // per esempio eh'-sheth in Strong's Hebrew
                            {
                                parola += '-'; // ASCII 45
                                analizzaParola = false;
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(parola) && analizzaParola)
                    {
                        ++nParola;
                        vp.Parola = nParola;
                        parola = parola.ToLower(CultureInfo.InvariantCulture);
                        if (!chiave.TryGetValue(parola, out List<OccorrenzaParola>? value))
                        {
                            value = [];
                            chiave.Add(parola, value);
                        }

                        value.Add(vp);
                        parola = "";
                    }
                }
                else
                {
                    throw new CarattereSconosciutoException("Carattere sconosciuto in " + testo);
                }
            }

            if (!String.IsNullOrEmpty(parola))
            {
                ++nParola;
                vp.Parola = nParola;
                parola = parola.ToLower(CultureInfo.InvariantCulture);
                if (!chiave.TryGetValue(parola, out List<OccorrenzaParola>? value))
                {
                    value = [];
                    chiave.Add(parola, value);
                }

                value.Add(vp);
                parola = "";
            }

            // il processo può essere lungo (per tutte le parole) per una collezione grande

            Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new Action(delegate { })
                        );

            return chiave;
        }

        /// <summary>
        /// Trova tutte le citazioni a riferimenti in una nota.
        /// </summary>
        /// <param name="testo">Il testo della nota da analizzare.</param>
        /// <returns>Una lista con tutti i brani.</returns>
        private List<Riferimento> TrovaRiferimentiInVoce(string testo)
        {
            List<Riferimento> riferimenti = [];
            Riferimento riferimentoLink = new();

            int posizione = testo.IndexOf(RichTextBoxEx.InizioLink.ToString(), StringComparison.Ordinal);
            int posizioneLink;
            while (posizione >= 0)
            {
                try
                {
                    posizioneLink = testo.IndexOf(RichTextBoxEx.FineLink1.ToString(), posizione, StringComparison.Ordinal);
                    if (testo[posizioneLink + 1] == RichTextBoxEx.FineLinkBrano)
                    {
                        riferimentoLink = ConvertiRiferimento(ConvertiTitoloNotaARiferimento(testo.Substring(posizioneLink + 2, testo.IndexOf(RichTextBoxEx.FineLink2.ToString(), posizioneLink, StringComparison.Ordinal) - posizioneLink - 2)));
                        for (int i = 0; i < riferimentoLink.Count; ++i)
                        {
                            riferimenti.Add(new Riferimento(riferimentoLink.Brani[i]));
                        }
                    }
                }
                catch
                {
                    // errore nel formato del link; lo saltiamo
                }
                posizione = testo.IndexOf(RichTextBoxEx.InizioLink.ToString(), posizione + 1, StringComparison.Ordinal);
            }

            // quando un file RTF con riferimento è salvato, i caratteri per indicare i riferimenti
            // vengono convertiti, quindi dobbiamo cercare anche loro
            string inizioLink = @"\'0" + ((int)RichTextBoxEx.InizioLink).ToString(CultureInfo.InvariantCulture);
            string fineLink1 = @"\'0" + ((int)RichTextBoxEx.FineLink1).ToString(CultureInfo.InvariantCulture);
            string fineLink2 = @"\'0" + ((int)RichTextBoxEx.FineLink2).ToString(CultureInfo.InvariantCulture);
            string fineLinkBrano = @"\'0" + ((int)RichTextBoxEx.FineLinkBrano).ToString(CultureInfo.InvariantCulture);
            posizione = testo.IndexOf(inizioLink, StringComparison.Ordinal);
            while (posizione >= 0)
            {
                try
                {
                    posizioneLink = testo.IndexOf(fineLink1, posizione, StringComparison.Ordinal);
                    if (testo.Substring(posizioneLink + 4, 4) == fineLinkBrano)
                    {
                        riferimentoLink = ConvertiRiferimento(ConvertiTitoloNotaARiferimento(testo.Substring(posizioneLink + 8, testo.IndexOf(fineLink2, posizioneLink, StringComparison.Ordinal) - posizioneLink - 8)));
                        for (int i = 0; i < riferimentoLink.Count; ++i)
                        {
                            riferimenti.Add(new Riferimento(riferimentoLink.Brani[i]));
                        }
                    }
                }
                catch
                {
                    // errore nel formato del link; lo saltiamo
                }
                posizione = testo.IndexOf(inizioLink, posizione + 1, StringComparison.Ordinal);
            }

            return riferimenti;
        }

        /// <summary>
        /// Indica se una collezione di note è stata modificata.
        /// </summary>
        /// <param name="nomeVersione">La collezione da controllare.</param>
        /// <returns></returns>
        public bool CollezioneModificata(string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].NoteModificate;
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Indica se almeno una collezione di note è stata modificata.
        /// </summary>
        /// <returns>Se una collezione di note è stata modificata.</returns>
        public bool NoteModificate()
        {
            foreach (Versione versione in versioni.Values)
            {
                if (versione.NoteModificate)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Restituisce se esistono citazioni a brani della Bibbia in una collezione di note.
        /// </summary>
        /// <param name="nomeVersione">La collezione in cui cercare le citazioni.</param>
        /// <returns>Se ci sono citazioni nella collezione.</returns>
        public Boolean EsistonoCitazioni(string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].EsistonoCitazioni();
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Dove un brano è menzionato in una collezione di note.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano da cercare.</param>
        /// <param name="nomeVersione">La collezione in cui cercare il riferimento.</param>
        /// <returns>Un elenco di tutte le note che contengono un riferimento al brano.</returns>
        public Riferimento Citazioni(string riferimento, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].Citazioni(ConvertiRiferimento(riferimento));
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Dove un brano è menzionato in una collezione di note.
        /// </summary>
        /// <param name="riferimento">Il riferimento del brano da cercare.</param>
        /// <param name="nomeVersione">La collezione in cui cercare il riferimento.</param>
        /// <returns>Un elenco di tutte le note che contengono un riferimento al brano.</returns>
        public Riferimento Citazioni(Riferimento riferimento, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].Citazioni(riferimento);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        #endregion

        #region Funzioni per le immagini

        /// <summary>
        /// Trova i file grafici che contengono un nome.
        /// </summary>
        /// <param name="nome">Il nome da ricercare.</param>
        /// <returns>Una collezione con i nomi di tutti i file grafici che contengono il nome.</returns>
        public Collection<string> Immagini(string nome)
        {
            try
            {
                return indiceImmagini[nome];
            }
            catch (KeyNotFoundException)
            {
                return [];
            }
            catch (ArgumentNullException)
            {
                return [];
            }
        }

        #endregion

        /// <summary>
        /// Se un brano o delle note esistono in una certa versione.
        /// </summary>
        /// <param name="riferimento">Il brano o elenco di note da controllare.</param>
        /// <param name="nomeVersione">La versione in cui cercare il brano o note.</param>
        /// <returns>Vero se il brano o nota esiste.</returns>
        public bool EsisteBrano(Riferimento riferimento, string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].EsisteBrano(riferimento);
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Una lista di tutte le radici diverse in una certa versione.
        /// </summary>
        /// <param name="nomeVersione">La versione di cui restituire le radici diverse.</param>
        /// <returns>Una lista di stringhe, con il versetto o numero della note, poi il numero della parola, poi la radice diverse, separati dal carattere |.</returns>
        public Collection<string> GetRadiciDiverse(string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].GetRadiciDiverse();
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Una lista di tutti i riferimenti diversi dai riferimenti standard in una certa versione.
        /// </summary>
        /// <param name="nomeVersione">La versione di cui restituire i riferimenti diversi.</param>
        /// <returns>Una lista di stringhe, con sei numeri separati dal carattere |.</returns>
        public Collection<string> GetRiferimentiDiversi(string nomeVersione)
        {
            try
            {
                Collection<string> listaRiferimentiDiversi = [];
                foreach (Int16[] riferimentoDiverso in versioni[nomeVersione].riferimentiDiversi)
                {
                    listaRiferimentiDiversi.Add(new StringBuilder().Append(riferimentoDiverso[0]).Append('|').Append(riferimentoDiverso[1]).Append('|').Append(riferimentoDiverso[2]).Append('|').Append(riferimentoDiverso[3]).Append('|').Append(riferimentoDiverso[4]).Append('|').Append(riferimentoDiverso[5]).ToString());
                }
                //            listaRiferimentiDiversi.Add(riferimentoDiverso[0] + "|" + riferimentoDiverso[1] + "|" + riferimentoDiverso[2] + "|" + riferimentoDiverso[3] + "|" + riferimentoDiverso[4] + "|" + riferimentoDiverso[5]);
                return listaRiferimentiDiversi;
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Una lista di tutti i riferimenti citati nelle note.
        /// </summary>
        /// <param name="nomeVersione">La versione di cui restituire i riferimenti citati.</param>
        /// <returns>Una lista di stringhe, con sette numeri separati dal carattere |.</returns>
        public Collection<string> GetRiferimentiCitati(string nomeVersione)
        {
            try
            {
                return versioni[nomeVersione].GetRiferimentiCitati();
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        /// <summary>
        /// Una lista di tutte le parole in una versione con le loro radici.
        /// </summary>
        /// <param name="nomeVersione">La versione di cui restituire le parole e le loro radici.</param>
        /// <returns>Una lista di stringhe, con le parole, poi =, poi la radice di ogni parola.</returns>
        public Collection<string> GetParoleRadici(string nomeVersione)
        {
            // tested
            try
            {
                Collection<string> listaParoleRadici = [];
                foreach (string parola in versioni[nomeVersione].Parole)
                {
                    listaParoleRadici.Add(parola + "=" + versioni[nomeVersione].RadiceDiParola(parola));
                }

                return listaParoleRadici;
            }
            catch (KeyNotFoundException)
            {
                throw new TextNotExistException();
            }
        }

        public async Task<FlowDocument> MergeManyRtfAsync(IEnumerable<string> rtfs, bool impostaFormato = false)
        {
            // Materialize on the current thread; still thread-neutral.
            List<string> sourceRtfs = rtfs?
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList() ?? [];

            // Build the FlowDocument on the UI dispatcher so the returned doc
            // belongs to the UI thread and can be used by RichTextBox/DockingHost.
            return await Application.Current.Dispatcher.InvokeAsync(
                () => BuildMergedFlowDocument(sourceRtfs, impostaFormato),
                DispatcherPriority.Normal);
        }

        private FlowDocument BuildMergedFlowDocument(IReadOnlyList<string> rtfs, bool impostaFormato)
        {
            FlowDocument finalDoc = new()
            {
                FlowDirection = FlowDirection.LeftToRight,
                TextAlignment = TextAlignment.Left
            };

            bool first = true;

            foreach (string originalRtf in rtfs)
            {
                TextAlignment sourceAlignment = GetAlignment(originalRtf);
                FlowDocument tempDoc = LoadRtfToFlowDocumentOnUiThread(originalRtf);
                NormalizeLoadedBlocks(tempDoc, null);

                if (!first)
                {
                    finalDoc.Blocks.Add(new Paragraph(new Run("")));
                }

                // Move blocks into final doc
                while (tempDoc.Blocks.FirstBlock != null)
                {
                    Block block = tempDoc.Blocks.FirstBlock;
                    tempDoc.Blocks.Remove(block);
                    finalDoc.Blocks.Add(block);
                }

                first = false;
            }

            // Final safety pass on the merged document so the displayed result is correct.
            NormalizeLoadedBlocks(finalDoc, null);

            if (impostaFormato)
            {
                ApplyGlobalFormatting(finalDoc);
            }

            //CollapseConsecutiveBlankParagraphs(finalDoc);
            return finalDoc;
        }

        private static FlowDocument LoadRtfToFlowDocumentOnUiThread(string rtf)
        {
            FlowDocument doc = new();
            TextRange range = new(doc.ContentStart, doc.ContentEnd);

            string converted = ConvertiUnicodeInRtf(rtf);

            using MemoryStream ms = new(Encoding.UTF8.GetBytes(converted));
            range.Load(ms, DataFormats.Rtf);

            return doc;
        }

        private static void NormalizeLoadedBlocks(FlowDocument doc, TextAlignment? fallbackAlignment)
        {
            List<Block> blocks = [.. doc.Blocks.Cast<Block>()];

            foreach (Block block in blocks)
            {
                if (block is Paragraph p)
                {
                    TextAlignment effectiveAlignment =
    p.TextAlignment != TextAlignment.Left ||
    fallbackAlignment == null
        ? p.TextAlignment
        : fallbackAlignment.Value;

                    TextAlignment alignment =
    p.TextAlignment;

                    // Optional fallback only if really needed
                    if (alignment == TextAlignment.Left &&
                        fallbackAlignment != null)
                    {
                        alignment = fallbackAlignment.Value;
                    }

                    NormalizeParagraphForMixedBidi(p, alignment);
                }
                else if (block is Section s)
                {
                    NormalizeSection(s, fallbackAlignment ?? TextAlignment.Left);
                }
            }
        }

        private static void NormalizeSection(Section section, TextAlignment fallbackAlignment)
        {
            List<Block> blocks = [.. section.Blocks.Cast<Block>()];

            foreach (Block block in blocks)
            {
                if (block is Paragraph p)
                {
                    NormalizeParagraphForMixedBidi(p, fallbackAlignment);
                }
                else if (block is Section nested)
                {
                    NormalizeSection(nested, fallbackAlignment);
                }
            }
        }

        private static void NormalizeParagraphForMixedBidi(Paragraph p, TextAlignment fallbackAlignment)
        {
            bool hasHebrew = ParagraphContainsHebrew(p);

            // Keep paragraph LTR if that is what your editor/host visually needs,
            // but align Hebrew paragraphs to the right.
            p.FlowDirection = FlowDirection.LeftToRight;
            p.TextAlignment = hasHebrew ? TextAlignment.Right : fallbackAlignment;

            if (!hasHebrew)
                return;

            // Snapshot current inlines BEFORE modifying them
            List<Inline> originalInlines = [.. p.Inlines];

            // Move everything into one outer RTL span so sibling ordering is RTL
            Span outerRtl = new()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Language = HebrewLanguage
            };

            // Clear paragraph inlines safely
            p.Inlines.Clear();

            foreach (Inline inline in originalInlines)
            {
                outerRtl.Inlines.Add(inline);
            }

            p.Inlines.Add(outerRtl);

            NormalizeMixedInlines(outerRtl.Inlines);
        }

        private static void NormalizeMixedInlines(InlineCollection inlines)
        {
            List<Inline> snapshot = [.. inlines];

            foreach (Inline inline in snapshot)
            {
                switch (inline)
                {
                    case Run run:
                        NormalizeRunDirection(run);
                        break;

                    case Span span:
                        NormalizeSpanDirection(span);
                        break;
                }
            }
        }

        private static void NormalizeSpanDirection(Span span)
        {
            string text = new TextRange(span.ContentStart, span.ContentEnd).Text ?? "";

            if (ContainsHebrew(text))
            {
                span.FlowDirection = FlowDirection.RightToLeft;
                span.Language = HebrewLanguage;
            }
            else if (ContainsLatinOrDigits(text))
            {
                span.FlowDirection = FlowDirection.LeftToRight;
                span.Language = EnglishLanguage;
            }

            List<Inline> children = [.. span.Inlines];

            foreach (Inline child in children)
            {
                switch (child)
                {
                    case Run run:
                        NormalizeRunDirection(run);
                        break;

                    case Span nested:
                        NormalizeSpanDirection(nested);
                        break;
                }
            }
        }

        private static void NormalizeRunDirection(Run run)
        {
            string text = run.Text ?? "";

            if (text == "&nbsp;")
            {
                run.Text = "\u00A0";
                text = run.Text;
            }

            if (ContainsHebrew(text))
            {
                if (run.FlowDirection != FlowDirection.RightToLeft)
                    run.FlowDirection = FlowDirection.RightToLeft;
                if (run.Language != HebrewLanguage)
                    run.Language = HebrewLanguage;
            }
            else if (ContainsLatinOrDigits(text))
            {
                if (run.FlowDirection != FlowDirection.LeftToRight)
                    run.FlowDirection = FlowDirection.LeftToRight;
                if (run.Language != EnglishLanguage)
                    run.Language = EnglishLanguage;
            }
        }

        private static bool ContainsHebrew(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            foreach (char c in text)
            {
                if ((c >= '\u0590' && c <= '\u05FF') ||
                    (c >= '\uFB1D' && c <= '\uFB4F'))
                    return true;
            }

            return false;
        }

        private static bool ParagraphContainsHebrew(Paragraph p)
        {
            foreach (Inline inline in p.Inlines)
            {
                if (InlineContainsHebrew(inline))
                    return true;
            }

            return false;
        }

        private static bool InlineContainsHebrew(Inline inline)
        {
            switch (inline)
            {
                case Run run:
                    return ContainsHebrew(run.Text);

                case Span span:
                    foreach (Inline child in span.Inlines)
                    {
                        if (InlineContainsHebrew(child))
                            return true;
                    }
                    break;
            }
            return false;
        }

        private static bool ContainsLatinOrDigits(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            foreach (char c in text)
            {
                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9'))
                    return true;
            }

            return false;
        }

        private void ApplyGlobalFormatting(FlowDocument doc)
        {
            TextRange finalRange = new(doc.ContentStart, doc.ContentEnd);

            try
            {
                finalRange.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(Formato.FontNome));
            }
            catch
            {
                // ignore invalid font name
            }

            finalRange.ApplyPropertyValue(TextElement.FontSizeProperty, (double)Formato.FontDimensione);
            finalRange.ApplyPropertyValue(
                TextElement.FontWeightProperty,
                Formato.FontGrassetto ? FontWeights.Bold : FontWeights.Normal);

            finalRange.ApplyPropertyValue(
                TextElement.FontStyleProperty,
                Formato.FontCorsivo ? FontStyles.Italic : FontStyles.Normal);

            finalRange.ApplyPropertyValue(
                Inline.TextDecorationsProperty,
                Formato.FontSottolineato ? TextDecorations.Underline : null);
        }


        private static TextAlignment GetAlignment(string rtf)
        {
            if (rtf.Contains(@"\qr"))
                return TextAlignment.Right;

            if (rtf.Contains(@"\qc"))
                return TextAlignment.Center;

            if (rtf.Contains(@"\qj"))
                return TextAlignment.Justify;

            return TextAlignment.Left;
        }

        private static void NormalizeBlock(Block block, TextAlignment fallbackAlignment)
        {
            if (block is Paragraph p)
            {
                NormalizeParagraph(p, fallbackAlignment);
            }
            else if (block is Section s)
            {
                foreach (Block child in s.Blocks)
                    NormalizeBlock(child, fallbackAlignment);
            }
        }

        private static void NormalizeParagraph(Paragraph p, TextAlignment fallbackAlignment)
        {
            string text = new TextRange(p.ContentStart, p.ContentEnd).Text;
            bool hasHebrew = ContainsHebrew(text);

            if (hasHebrew)
            {
                p.FlowDirection = FlowDirection.RightToLeft;
                p.TextAlignment = TextAlignment.Right;
                p.Language = HebrewLanguage;
            }
            else
            {
                p.FlowDirection = FlowDirection.LeftToRight;
                p.TextAlignment = fallbackAlignment;
            }

            NormalizeInlines(p.Inlines, hasHebrew);
        }

        private static void NormalizeInlines(InlineCollection inlines, bool paragraphIsRtl)
        {
            foreach (Inline inline in inlines.ToList())
            {
                switch (inline)
                {
                    case Run run:
                        NormalizeRun(run, paragraphIsRtl);
                        break;

                    case Span span:
                        NormalizeSpan(span, paragraphIsRtl);
                        break;
                }
            }
        }

        private static void NormalizeSpan(Span span, bool paragraphIsRtl)
        {
            string text = new TextRange(span.ContentStart, span.ContentEnd).Text ?? "";

            // If the span wraps a single-direction fragment, force it explicitly
            if (ContainsHebrew(text))
            {
                span.FlowDirection = FlowDirection.RightToLeft;
                span.Language = HebrewLanguage;
            }
            else if (ContainsLatinOrDigits(text) || text.Contains("&nbsp;"))
            {
                span.FlowDirection = FlowDirection.LeftToRight;
                span.Language = EnglishLanguage;
            }
            else
            {
                // neutral content: let it inherit from paragraph
                span.FlowDirection = paragraphIsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            }

            foreach (Inline child in span.Inlines.ToList())
            {
                switch (child)
                {
                    case Run run:
                        NormalizeRun(run, paragraphIsRtl);
                        break;
                    case Span nested:
                        NormalizeSpan(nested, paragraphIsRtl);
                        break;
                }
            }
        }

        private static void NormalizeRun(Run run, bool paragraphIsRtl)
        {
            string text = run.Text ?? "";

            if (text == "&nbsp;")
            {
                run.Text = "\u00A0";
                text = run.Text;
            }

            if (ContainsHebrew(text))
            {
                if (run.FlowDirection != FlowDirection.RightToLeft)
                    run.FlowDirection = FlowDirection.RightToLeft;
                if (run.Language != HebrewLanguage)
                    run.Language = HebrewLanguage;
            }
            else if (ContainsLatinOrDigits(text))
            {
                // Verse labels / refs like "Gen 1:1", "2", etc.
                if (run.FlowDirection != FlowDirection.LeftToRight)
                    run.FlowDirection = FlowDirection.LeftToRight;
                if (run.Language != EnglishLanguage)
                    run.Language = EnglishLanguage;
            }
            else
            {
                // neutral whitespace/punctuation inherits paragraph direction
                if (run.FlowDirection != (paragraphIsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight))
                    run.FlowDirection = paragraphIsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            }
        }

        public static void CollapseConsecutiveBlankParagraphs(FlowDocument doc)
        {
            if (doc == null) return;
            CollapseInBlockCollection(doc.Blocks);
        }

        private static void CollapseInBlockCollection(BlockCollection blocks)
        {
            // Snapshot first, because we may remove from the live BlockCollection.
            List<Block> snapshot = [.. blocks.Cast<Block>()];

            bool previousWasBlankParagraph = false;

            foreach (Block block in snapshot)
            {
                switch (block)
                {
                    case Paragraph p:
                        {
                            bool isBlank = IsBlankParagraph(p);

                            if (isBlank)
                            {
                                if (previousWasBlankParagraph)
                                {
                                    blocks.Remove(p);   // keep only the first blank paragraph
                                }
                                else
                                {
                                    previousWasBlankParagraph = true;
                                }
                            }
                            else
                            {
                                previousWasBlankParagraph = false;
                            }

                            break;
                        }

                    case Section s:
                        {
                            CollapseInBlockCollection(s.Blocks);
                            previousWasBlankParagraph = false; // treat nested container as boundary
                            break;
                        }

                    case List list:
                        {
                            foreach (ListItem item in list.ListItems.Cast<ListItem>().ToList())
                            {
                                CollapseInBlockCollection(item.Blocks);
                            }

                            previousWasBlankParagraph = false; // boundary
                            break;
                        }

                    case Table table:
                        {
                            foreach (TableRowGroup rg in table.RowGroups.Cast<TableRowGroup>().ToList())
                            {
                                foreach (TableRow row in rg.Rows.Cast<TableRow>().ToList())
                                {
                                    foreach (TableCell cell in row.Cells.Cast<TableCell>().ToList())
                                    {
                                        CollapseInBlockCollection(cell.Blocks);
                                    }
                                }
                            }

                            previousWasBlankParagraph = false; // boundary
                            break;
                        }

                    default:
                        {
                            previousWasBlankParagraph = false;
                            break;
                        }
                }
            }

            // remove trailing blank paragraphs at the end of this block collection
            TrimTrailingBlankParagraphs(blocks);
        }

        private static void TrimTrailingBlankParagraphs(BlockCollection blocks)
        {
            // Cammina all’indietro finché l’ultimo blocco è un paragrafo vuoto.
            // La doc MS mostra che puoi rimuovere l’ultimo blocco con Blocks.Remove(Blocks.LastBlock)
            while (blocks.LastBlock is Paragraph p && IsBlankParagraph(p))
            {
                blocks.Remove(p);
            }
        }

        private static bool IsBlankParagraph(Paragraph p)
        {
            // TextRange.Text gives the plain text content of the paragraph.
            string text = new TextRange(p.ContentStart, p.ContentEnd).Text;

            if (string.IsNullOrEmpty(text))
                return true;

            // Consider normal whitespace + NBSP as blank
            text = text
                .Replace("\u00A0", " ") // NBSP
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");

            return string.IsNullOrWhiteSpace(text);
        }
        public static Task<string> ToRtfStringAsync(FlowDocument doc)
        {
            // Esegui sul Dispatcher del documento (thread-safe per WPF)
            return doc.Dispatcher.InvokeAsync(
                () => ToRtfString(doc),
                DispatcherPriority.Normal
            ).Task;
        }

        private static string ToRtfString(FlowDocument doc)
        {
            TextRange range = new(doc.ContentStart, doc.ContentEnd);

            using MemoryStream ms = new();
            range.Save(ms, DataFormats.Rtf); // supporta Rtf, Text, Xaml, XamlPackage 

            // RTF è tipicamente ASCII-safe (Unicode viene emesso come \uNNNN?)
            return Encoding.ASCII.GetString(ms.ToArray());
        }

        private static Task<byte[]> ToRtfBytesAsync(FlowDocument doc)
        {
            return doc.Dispatcher.InvokeAsync(
                () => ToRtfBytes(doc),
                DispatcherPriority.Normal
            ).Task;
        }

        private static byte[] ToRtfBytes(FlowDocument doc)
        {
            TextRange range = new(doc.ContentStart, doc.ContentEnd);
            using MemoryStream ms = new();
            range.Save(ms, DataFormats.Rtf); // 
            return ms.ToArray();
        }

        private static readonly string[] Numeri3Stringhe =
    [.. Enumerable.Range(0, 256).Select(i => i.ToString("000", CultureInfo.InvariantCulture))];

        private static readonly string[] Numeri2Stringhe =
    [.. Enumerable.Range(0, 256).Select(i => i.ToString("00", CultureInfo.InvariantCulture))];

        private static readonly string[] ByteStringhe =
    [.. Enumerable.Range(0, 256).Select(i => i.ToString(CultureInfo.CurrentCulture))];
    }
}
