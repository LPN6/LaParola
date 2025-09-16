//
//  Testi.swift
//  LaParola
//
//  Created by admin on 13/02/24.
//

import Foundation
import SwiftUI

/*
 
 {
 #region Confronto
 
 /// <summary>
 /// Una classe per confrontare due stringhe, che funziona anche con i caratteri greci.
 /// Case insensitive.
 /// </summary>
 public struct ConfrontoCI : IComparer<String>
 {
 /// <summary>
 /// La funzione Compare.
 /// </summary>
 /// <param name="x">La prima stringa.</param>
 /// <param name="y">La seconda stringa.</param>
 /// <returns>Il confronto delle stringhe: -1, 0 o 1.</returns>
 public int Compare(string x, string y)
 {
 if (x == null)
 throw new ArgumentNullException("x");
 if (y == null)
 throw new ArgumentNullException("y");
 
 
 return String.Compare(x.Normalize(NormalizationForm.FormD), y.Normalize(NormalizationForm.FormD), StringComparison.InvariantCultureIgnoreCase);
 
 }
 }
 
 /// <summary>
 /// Una classe per confrontare due stringhe, che funziona anche con i caratteri greci.
 /// Case sensitive.
 /// </summary>
 public struct ConfrontoCS : IComparer<String>
 {
 /// <summary>
 /// La funzione Compare.
 /// </summary>
 /// <param name="x">La prima stringa.</param>
 /// <param name="y">La seconda stringa.</param>
 /// <returns>Il confronto delle stringhe: -1, 0 o 1.</returns>
 public int Compare(string x, string y)
 {
 if (x == null)
 throw new ArgumentNullException("x");
 if (y == null)
 throw new ArgumentNullException("y");
 
 return String.Compare(x.Normalize(NormalizationForm.FormD), y.Normalize(NormalizationForm.FormD), StringComparison.InvariantCulture);
 
 }
 }
 
 */
//#region Abbreviazioni riconosciute

/// <summary>
/// Una classe che gestisce le abbreviazioni riconosciute dal libro.
/// </summary>
public struct LibriAbbreviazioniRiconosciuteHash
{
    var libriAbbreviazioniRiconosciute = [String: UInt8]()
    
    /// <summary>
    /// Il costruttore della classe.
    /// </summary>
    init()
    {
        //libriAbbreviazioniRiconosciute = new Dictionary<string, byte>();
    }
    
    public mutating func add(_ abb:String, _ index:UInt8) {
        libriAbbreviazioniRiconosciute[abb] = index;
    }
    
    /// <summary>
    /// Restituisce un'abbreviazione riconosciuto di un certo libro.
    /// </summary>
    /// <param name="libro">Il numero di un libro.</param>
    /// <returns>Un'abbreviazione riconosciuta.</returns>
    public func abbreviazione(_ libro:UInt8) -> String
    {
        var rifLibro = "";
        for (abb, nLibro) in libriAbbreviazioniRiconosciute
        {
            if (nLibro == libro)
            {
                rifLibro = abb;
                break;
            }
        }
        return rifLibro;
    }
    
    public func libroDiAbbreviazione(_ abb:String) -> UInt8 {
        return libriAbbreviazioniRiconosciute[abb, default:0]
    }
    
    /// <summary>
    /// Decide se l'abbreviazione è riconosciuta.
    /// </summary>
    /// <param name="abbreviazione">L'abbreviazione da controllare.</param>
    /// <returns>Vero se l'abbreviazione è riconosciuta.</returns>
    public func containsKey(_ abbreviazione:String) -> Bool
    {
        return libriAbbreviazioniRiconosciute.keys.contains(abbreviazione)
    }
    
    /// <summary>
    /// Rimuovi tutte le abbreviazioni dall'elenco.
    /// </summary>
    public mutating func clear()
    {
        libriAbbreviazioniRiconosciute.removeAll();
    }
    
    /// <summary>
    /// Restituisce tutte le abbreviazioni riconosciute, ordinate per libro.
    /// </summary>
    /// <returns>Un array con 73 elementi (da 0 a 72), ogni elemento ha tutte le abbreviazioni separate da una virgola per un libro.</returns>
    public func abbreviazioniPerLibro() -> [String]
    {
        var abbreviazioniRiconoconosciute = [String](repeating:"", count:73)
        for (abb, nLibro) in libriAbbreviazioniRiconosciute {
            abbreviazioniRiconoconosciute[Int(nLibro) - 1] += abb + ","
        }
        return abbreviazioniRiconoconosciute;
    }
}

//#region Exception

/// <summary>
/// Exception quando una richiesta è fatta per informazioni di una versione che non esiste.
/// </summary>
public struct TextNotExistException : Error
{
    
}

/// <summary>
/// Exception quando il file da aprire con un testo del programma non è valido.
/// </summary>
public enum FileNonValidoException : Error
{
    case fileNonEsiste
    case fileNonValido
}

enum SearchException: Error {
    case SearchExpressionEmpty
    case SearchSyntaxError(Int)
    case SearchParentheses
    case SearchBrackets
}
/*
 
 //#region UltimaBibbia
 // UltimaBibbia evento (vedi blog.scottlogic.com/2015/02/05/swift-events.html)
 /// <summary>
 /// Gli argomenti dell'evento quando la Bibbia utilizzata è cambiata.
 /// </summary>
 public class UltimaBibbiaEventArgs : EventArgs
 {
 private string nuovaBibbia;
 /// <summary>
 /// La Bibbia utilizzata.
 /// </summary>
 public string NuovaBibbia
 {
 get { return nuovaBibbia; }
 }
 
 /// <summary>
 /// Il costruttore della classe.
 /// </summary>
 /// <param name="bibbiaUtilizzata">La Bibbia utilizzata.</param>
 public UltimaBibbiaEventArgs(string bibbiaUtilizzata)
 {
 nuovaBibbia = bibbiaUtilizzata;
 }
 }
 // UltimaBibbia evento (vedi blog.scottlogic.com/2015/02/05/swift-events.html)
 /*
  /// <summary>
  /// Il delegate che inizia l'evento quando la Bibbia utilizzata è cambiata.
  /// </summary>
  /// <param name="sender">La classe che ha generato l'evento.</param>
  /// <param name="e">Gli argomenti dell'evento.</param>
  public delegate void UltimaBibbiaEventHandler(object sender, UltimaBibbiaEventArgs e);
  */
 
 #endregion
 
 */
/// <remarks>
/// Una classe che contiene tutte le informazioni sui testi biblici trovati, e restituisce le informazioni necessarie ad altri programmi.
/// </remarks>
public class Texts
{
    let fileManager = FileManager.default
    //#region const
    
    /// <summary>
    /// Le otto cifre che seguono danno il riferimento del versetto che segue.
    /// </summary>
    public let InizioRiferimento = String(UnicodeScalar(1))
    /// <summary>
    /// Il carattere inserito per indicare l'inizio di un link ipertestuale.
    /// </summary>
    public let InizioLink = String(UnicodeScalar(2))
    /// <summary>
    /// Il carattere inserito per indicare l'inizio della parte finale un link ipertestuale.
    /// </summary>
    public let FineLink1 = String(UnicodeScalar(3))
    /// <summary>
    /// Il carattere inserito per indicare la fine della parte finale un link ipertestuale.
    /// </summary>
    public let FineLink2 = String(UnicodeScalar(4))
    /// <summary>
    /// Il carattere inserito per indicare la fine di un link ipertestuale ad un brano.
    /// </summary>
    public let FineLinkBrano = String(UnicodeScalar(5))
    /// <summary>
    /// Il carattere inserito per indicare la fine di un link ipertestuale ad una nota.
    /// </summary>
    public let FineLinkNota = String(UnicodeScalar(6))
    /// <summary>
    /// Il carattere inserito per indicare la fine di un link ipertestuale ad un file.
    /// </summary>
    public let FineLinkFile = String(UnicodeScalar(7))
    /// <summary>
    /// Il carattere inserito per indicare l'inizio di una parola ricercata.
    /// </summary>
    public let ParolaRicercata = String(UnicodeScalar(14))
    
    /// <summary>
    /// I nomi di tutti i libri della Bibbia in inglese.
    /// </summary>
    let LibriNomiInglese = "|Genesis|Exodus|Leviticus|Numbers|Deuteronomy|Joshua|Judges|Ruth|1Samuel|2Samuel|1Kings|2Kings|1Chronicles|2Chronicles|Ezra|Nehemiah|Tobit|Judith|Esther|1Maccabees|2Maccabees|Job|Psalms|Proverbs|Ecclesiastes|Song of Songs|Wisdom|Sirach|Isaiah|Jeremiah|Lamentations|Baruch|Ezekiel|Daniel|Hosea|Joel|Amos|Obadiah|Jonah|Micah|Nahum|Habakkuk|Zephaniah|Haggai|Zechariah|Malachi|Matthew|Mark|Luke|John|Acts|Romans|1Corinthians|2Corinthians|Galatians|Ephesians|Philippians|Colossians|1Thessalonians|2Thessalonians|1Timothy|2Timothy|Titus|Philemon|Hebrews|James|1Peter|2Peter|1John|2John|3John|Jude|Revelation";
    /// <summary>
    /// Le abbreviazioni usate dei libri della Bibbia in inglese.
    /// </summary>
    let LibriAbbreviazioniUsateInglese = "|Gen|Ex|Le|Nu|De|Josh|Judg|Ru|1Sam|2Sam|1K|2K|1Chr|2Chr|Ezra|Ne|Tob|Judi|Est|1M|2M|Job|Ps|Prov|Ec|SS|Wis|Sir|Is|Jer|Lam|Bar|Ezek|Dan|Hos|Joel|Am|Ob|Jon|Mi|Na|Hab|Zep|Hag|Zec|Mal|Mat|Mar|Lu|John|Ac|Ro|1Co|2Co|Ga|Eph|Phili|Col|1Th|2Th|1Ti|2Ti|Tit|Phile|Heb|Jam|1P|2P|1J|2J|3J|Jude|Rev";
    /// <summary>
    /// Le abbreviazioni riconosciute dei libri della Bibbia in inglese.
    /// </summary>
    let LibriAbbreviazioniRiconosciuteInglese = "|gen,gn|ex|le,lv|nm,nu|de,dt|jos,js|jdg,jg,judg|rt,ru|1s,1 s,isam|2s,2 s,iis|1k,1 k,ik|2k,2 k,iik|1ch,1 ch,ich|2ch,2 ch,iich|ezr|ne|tb,to|jdt,jt,judi|est,et|1m,1 m,im|2m,2 m,iim|jb,job|ps|pr,pv|ec|so,ss|w|si|is|je,jr|la|b|ez|da,dn|ho|jl,joe|am|o|jon|mi|na|hab|zep|hag|zec|mal,ml|mat,mt|mar,mk,mr|lk,lu|jn,joh|ac|rm,ro|1co,1 co,ico|2co,2 co,iico|ga|ep|phi,php,pl|cl,co|1th,1 th,1ts,ith|2th,2 th,2ts,iith|1ti,1 ti,1tm,iti|2ti,2 ti,2tm,iiti|ti,tt|phile,phlm,phm,pm|he|jam,jas,jm|1p,1 p,ip|2p,2 p,iip|1j,1 j,ij|2j,2 j,iij|3j,3 j,iiij|jd,jude|re";
    /// <summary>
    /// I nomi di tutti i libri della Bibbia in italiano.
    /// </summary>
    let LibriNomiItaliano = "|Genesi|Esodo|Levitico|Numeri|Deuteronomio|Giosuè|Giudici|Rut|1Samuele|2Samuele|1Re|2Re|1Cronache|2Cronache|Esdra|Neemia|Tobia|Giuditta|Ester|1Maccabei|2Maccabei|Giobbe|Salmi|Proverbi|Ecclesiaste|Cantico|Sapienza|Siracide|Isaia|Geremia|Lamentazioni|Baruc|Ezechiele|Daniele|Osea|Gioele|Amos|Abdia|Giona|Michea|Naum|Abacuc|Sofonia|Aggeo|Zaccaria|Malachia|Matteo|Marco|Luca|Giovanni|Atti|Romani|1Corinzi|2Corinzi|Galati|Efesini|Filippesi|Colossesi|1Tessalonicesi|2Tessalonicesi|1Timoteo|2Timoteo|Tito|Filemone|Ebrei|Giacomo|1Pietro|2Pietro|1Giovanni|2Giovanni|3Giovanni|Giuda|Apocalisse";
    /// <summary>
    /// Le abbreviazioni usate dei libri della Bibbia in italiano.
    /// </summary>
    let LibriAbbreviazioniUsateItaliano = "|Gen|Eso|Le|Nu|De|Gios|Giudic|Ru|1Sam|2Sam|1Re|2Re|1Cr|2Cr|Esd|Ne|Tob|Giudit|Est|1Macc|2Macc|Giob|Sal|Prov|Ec|CC|Sap|Sir|Is|Ger|Lam|Bar|Ez|Da|Os|Gioe|Am|Abd|Gion|Mi|Na|Abac|So|Ag|Zac|Mal|Mt|Mc|Lc|Gv|At|Rm|1Cor|2Cor|Gal|Ef|Fili|Col|1Ts|2Ts|1Tm|2Tm|Tt|Fm|Eb|Giac|1P|2P|1G|2G|3G|Giuda|Ap";
    /// <summary>
    /// Le abbreviazioni riconosciute dei libri della Bibbia in italiano.
    /// </summary>
    let LibriAbbreviazioniRiconosciuteItaliano = "|ge,gn|eo,es|le,lv|nm,nu|de,dt|gios,gs|gdc,giudic|rt,ru|1s,1 s,isam|2s,2 s,iis|1r,1 r,ir|2r,2 r,iir|1cr,1 cr,icr|2cr,2 cr,iicr|ed,esd|ne|tb,to|giudit|est,et|1m,1 m,im|2m,2 m,iim|gb,giob|sal,sl|pr,pv|ec,q|ca,cc,ct|sap|si|is|ger,gr|la|b|ez|da,dn|o|gioe,gl|am|abd,ad|gion|mi|na|aba,ac,h|so|ag|z|mal,ml|mat,mt|mar,mc,mr|lc,lu|giov,gv|at|rm,ro|1co,1 co,ico|2co,2 co,iico|ga|ef|fili,fl|cl,co|1te,1 te,1ts,ite|2te,2 te,2ts,iite|1ti,1 ti,1tm,iti|2ti,2 ti,2tm,iiti|ti,tt|file,fm|eb|gc,gia,gm|1p,1 p,ip|2p,2 p,iip|1g,1 g,ig|2g,2 g,iig|3g,3 g,iiig|gd,giuda|ap";
    /// <summary>
    /// I nomi di tutti i libri della Bibbia in spagnolo.
    /// </summary>
    let LibriNomiSpagnolo = "|Génesis|Éxodo|Levítico|Números|Deuteronomio|Josué|Jueces|Rut|1Samuel|2Samuel|1Reyes|2Reyes|1Crónicas|2Crónicas|Esdras|Nehemías|Tobit|Judit|Ester|1Macabeos|2Macabeos|Job|Salmos|Proverbios|Eclesiastés|Cantares|Sabiduría|Eclesiástico|Isaías|Jeremías|Lamentaciones|Baruc|Ezequiel|Daniel|Oseas|Joel|Amós|Abdías|Jonás|Miqueas|Nahum|Habacuc|Zofonías|Hageo|Zacarías|Malaquías|Mateo|Marcos|Lucas|Juan|Hechos|Romanos|1Corintios|2Corintios|Gálatas|Efesios|Filipenses|Colosenses|1Tesalonicenses|2Tesalonicenses|1Timoteo|2Timoteo|Tito|Filemón|Hebreos|Santiago|1Pedro|2Pedro|1Juan|2Juan|3Juan|Judas|Apocalipsis";
    /// <summary>
    /// Le abbreviazioni usate dei libri della Bibbia in spagnolo.
    /// </summary>
    let LibriAbbreviazioniUsateSpagnolo = "|Gn|Ex|Lv|Nm|Dt|Jos|Jue|Rt|1S|2S|1R|2R|1Cr|2Cr|Esd|Neh|Tb|Jdt|Est|1M|2M|Job|Sal|Pr|Ec|Cnt|Sab|Eclo|Is|Jer|Lm|Bar|Ez|Dn|Os|Jl|Am|Abd|Jon|Mi|Nah|Hab|Sof|Hag|Zac|Mal|Mt|Mr|Lc|Jn|Hch|Rm|1Co|2Co|Gá|Ef|Fil|Col|1Ts|2Ts|1Ti|2Ti|Tit|Flm|He|Stg|1P|2P|1Jn|2Jn|3Jn|Jud|Ap";
    /// <summary>
    /// Le abbreviazioni riconosciute dei libri della Bibbia in spagnolo.
    /// </summary>
    let LibriAbbreviazioniRiconosciuteSpagnolo = "|gé,ge,gn|éx,ex|le,lv|nm,nu,nú|de,dt|jos,js|jue,jc|rt,ru|1s,1 s,isam|2s,2 s,iis|1r,1 r,ir|2r,2 r,iir|1cr,1 cr,icr|2cr,2 cr,iicr|esd,ed|ne,nh|tb,to|jdt,jt,judi|est,et|1m,1 m,im|2m,2 m,iim|jb,job|sal,slm|pr,pv|ec|cnt,can|sab,sb|eclo,si|is|je,jr|la,lm|bar,br|ez|da,dn|os|jl,joe|am|abd|jon,jn|mi|na,nh|hab,hb|sof,sf|hag,hg|zac,zc|mal,ml|mat,mt|mar,mr|lc,lu|jn,ju|hch,hech|rm,ro|1co,1 co,ico|2co,2 co,iico|ga,gá|ef|fil,flp|cl,col|1ts,1 ts,1tes,its|2ts,2 ts,2ts,iits,2tes|1ti,1 ti,1tm,iti|2ti,2 ti,2tm,iiti|ti,tt|flm,file,fm|he|stg,sant,snt,sg|1p,1 p,ip|2p,2 p,iip|1j,1 j,ij|2j,2 j,iij|3j,3 j,iiij|jud,jd|ap,rev,rv";
    
    let paroleItalianeConApostrofe = [ "be", "co", "com", "da", "di", "die", "dov", "e", "fa", "fe", "mo", "pe", "po", "quant", "que", "rifa", "sta", "va" ];
    let paroleInglesiSenzaApostrofe = [ "amiss", "apostates", "commandments", "fillets", "holiness", "intercessions", "means", "prayer-fillets", "prayers", "prays", "righteous", "terms", "us", "was", "yes" ];
    
    //#region properties
    
    var formato:FormatoTesto
    
    var versioni = [String : Versione]()
    
    /*
     private Dictionary<string, Collection<string>> indiceImmagini;
     
     //#region UltimaBibbia
     
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
     if (UltimaBibbiaEvento != null)
     {
     // Invokes the delegates.
     UltimaBibbiaEvento(this, e);
     }
     }
     
     */
    
    private var _ultimaBibbia:String = ""
    /// <summary>
    /// L'ultima versione della Bibbia usata dall'utente.
    /// È usata quando il programma deve mostrare del testo in una versione qualsiasi, per esempio nelle Opzioni.
    /// </summary>
    var UltimaBibbia:String
    {
        get
        {
            if (_ultimaBibbia.isEmpty) {
                _ultimaBibbia = trovaUltimaBibbiaCompleta();
            }
            return _ultimaBibbia;
        }
        set
        {
            if (!(newValue.isEmpty))
            {
                _ultimaBibbia = newValue;
                // UltimaBibbia evento (vedi blog.scottlogic.com/2015/02/05/swift-events.html)
                //UltimaBibbiaEventArgs e = new UltimaBibbiaEventArgs(_ultimaBibbia);
                //OnChangedUltimaBibbia(e);
                
                if (versioni[_ultimaBibbia]?.indiceCapitolo.count ?? 0 > 50) { // altrimenti non è un'intera Bibbia
                    if (versioni[_ultimaBibbia]?.indiceCapitolo[73] ?? 0 > 1000 && versioni[_ultimaBibbia]?.capitoliInLibro[1] ?? 0 > 0 && versioni[_ultimaBibbia]?.capitoliInLibro[17] ?? 0 > 0 && versioni[_ultimaBibbia]?.capitoliInLibro[47] ?? 0 > 0) {
                        _ultimaBibbiaCompleta = _ultimaBibbia;
                    }
                    // se non abbiamo mai trovato una Bibbia con apocrifa, anche una adesso senza va bene
                    if (_ultimaBibbiaCompleta.isEmpty || versioni[_ultimaBibbiaCompleta]?.capitoliInLibro[17] == 0) {
                        if (versioni[_ultimaBibbia]?.indiceCapitolo[73] ?? 0 > 1000 && versioni[_ultimaBibbia]?.capitoliInLibro[1] ?? 0 > 0 && versioni[_ultimaBibbia]?.capitoliInLibro[47] ?? 0 > 0) {
                            _ultimaBibbiaCompleta = _ultimaBibbia;
                        }
                    }
                }
            }
        }
    }
    
    private var _ultimaBibbiaCompleta:String = ""
    /// <summary>
    /// L'ultima versione della Bibbia usata dall'utente che contiene Genesi, Matteo e preferibilmente Tobia (cioè AT, apocrifa e NT).
    /// </summary>
    var ultimaBibbiaCompleta:String
    {
        get
        {
            if (_ultimaBibbiaCompleta.isEmpty) {
                _ultimaBibbiaCompleta = trovaUltimaBibbiaCompleta();
            }
            return _ultimaBibbiaCompleta;
        }
        set { _ultimaBibbiaCompleta = newValue; }
    }
    
    func trovaUltimaBibbiaCompleta() -> String
    {
        // cercare una Bibbia con sia l'AT sia il NT, e preferibilmente l'apocrifa
        var bibbiaDaRestituire = "";
        var possibileUltimaBibbia = "", ultimaBibbiaSenzaApocrifa = "";
        for (nome, versione) in versioni
        {
            if (versione.info.tipo == TestoTipi.Bibbia)
            {
                if (capitoliInLibro(1, nome) > 0 && capitoliInLibro(17, nome) > 0 && capitoliInLibro(47, nome) > 0)
                {
                    bibbiaDaRestituire = nome;
                    break;
                }
                if (capitoliInLibro(1, nome) > 0 && capitoliInLibro(47, nome) > 0) {
                    ultimaBibbiaSenzaApocrifa = nome;
                }
                else {
                    possibileUltimaBibbia = nome;
                }
            }
        }
        if (bibbiaDaRestituire.isEmpty && !ultimaBibbiaSenzaApocrifa.isEmpty) {
            bibbiaDaRestituire = ultimaBibbiaSenzaApocrifa;
        }
        if (bibbiaDaRestituire.isEmpty && !possibileUltimaBibbia.isEmpty) {
            bibbiaDaRestituire = possibileUltimaBibbia;
        }
        return bibbiaDaRestituire;
    }
    
    //#region Libri
    
    var libriAbbreviazioniRiconosciute = LibriAbbreviazioniRiconosciuteHash()
    
    /// <summary>
    /// Trova il numero del libro (da 1 a 73; 0 se l'abbreviazione non è stata trovata) che corrisponde ad un'abbreviazione del nome di un libro.
    /// </summary>
    /// <param name="abbreviazione">L'abbreviazione da ricercare.</param>
    /// <returns>Il numero del libro.</returns>
    func getLibroNumeroDaAbbreviazione(_ abbreviazione:String) -> UInt8
    {
        var abb = abbreviazione
        if (abb != "")
        {
            abb = abb.lowercased()
            let abbcount = abb.count
            for numeroLettere in (1...abbcount).reversed()
            {
                if (libriAbbreviazioniRiconosciute.containsKey(abb[0..<numeroLettere]))
                {
                    return libriAbbreviazioniRiconosciute.libroDiAbbreviazione(abb[0..<numeroLettere]);
                }
            }
        }
        return 0;
    }
    
    //#region costruttori
    
    /// <summary>
    /// The constructor of the class. It gives default values to all the members of the class, and looks for and analyses all the data files that it finds in default directory (the subdirectory LaParola of the system application data directory).
    /// If there are data files in other directories, the <see cref="AddDirectory" /> method needs to be called as well.
    /// </summary>
    init(_ f:FormatoTesto)
    {
        formato = f
        
        if formato.libriNomi.count == 0 {
            formato.libriNomi = LibriNomiItaliano.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
        }
        if formato.libriAbbreviazioniUsate.count == 0 {
            formato.libriAbbreviazioniUsate = LibriAbbreviazioniUsateItaliano.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
        }
        if formato.libriAbbreviazioniRiconosciute.count == 0 {
            formato.libriAbbreviazioniRiconosciute = LibriAbbreviazioniRiconosciuteItaliano.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
        }
        //libriAbbreviazioniRiconosciute = new LibriAbbreviazioniRiconosciuteHash();
        //let abbreviazioniItaliane:[String] = LibriAbbreviazioniRiconosciuteItaliano.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
        /*var abbreviazioni:[String] = []
         for i in (UInt8)(1)...73
         {
         abbreviazioni = formato.libriAbbreviazioniRiconosciute[Int(i)].split(separator:",").map{String($0)}
         for abbreviazione in abbreviazioni
         {
         libriAbbreviazioniRiconosciute.add(abbreviazione, i)
         }
         }*/
        creaAbbreviazioniHash()
    }
    
    func creaAbbreviazioniHash() {
        libriAbbreviazioniRiconosciute.clear()
        var abbreviazioni:[String] = []
        for i in (UInt8)(1)...73
        {
            abbreviazioni = formato.libriAbbreviazioniRiconosciute[Int(i)].split(separator:",").map{String($0)}
            for abbreviazione in abbreviazioni
            {
                libriAbbreviazioniRiconosciute.add(abbreviazione, i)
            }
        }
    }
    
    /// <summary>
    /// Il codice RTF da inserire all'inizio di ogni testo creato, con la formattazione delle opzioni.
    /// </summary>
    /// <returns>L'intestazione del codice RTF.</returns>
    func RtfIntestazione() -> String
    {
        var stileFont = "";
        if (formato.fontGrassetto) {
            stileFont += "\\b1";
        }
        if (formato.fontCorsivo) {
            stileFont += "\\i1";
        }
        if (formato.fontSottolineato) {
            stileFont += "\\u1";
        }
        
        /*
         let s = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040{\\fonttbl{\\f0\\fnil\\fcharset0 " + formato.fontNome + ";}{\\f1\\fnil\\fcharset0 " + formato.fontRiferimentoNome + ";}{\\f3\\fnil\\fcharset0 " + formato.fontGrecoNome + ";}{\\f4\\fnil\\fcharset0 " + formato.fontEbraicoNome + ";}}"
         + "{\\colortbl\\red255\\green255\\blue255;"
         + ";\\red0\\green128\\blue0;\\red0\\green0\\blue255;\\red255\\green0\\blue0;}"
         + "\\viewkind4\\uc1\\pard" + stileFont + "\\cf0\\f0\\fs" + String(lround(formato.fontDimensione * 2)) + " ";
         */
        
        /* colortbl was this
         + "{\\colortbl\\red" + round255(formato.fontColore.resolve(in: EnvironmentValues()).red) + "\\green" + round255(formato.fontColore.resolve(in: EnvironmentValues()).green) + "\\blue" + round255(formato.fontColore.resolve(in: EnvironmentValues()).blue)
         + ";\\red" + round255(formato.fontRiferimentoColore.resolve(in: EnvironmentValues()).red) + "\\green" + round255(formato.fontRiferimentoColore.resolve(in: EnvironmentValues()).green) + "\\blue" + round255(formato.fontRiferimentoColore.resolve(in: EnvironmentValues()).blue)
         + ";\\red" + round255(formato.fontRicercaColore.resolve(in: EnvironmentValues()).red) + "\\green" + round255(formato.fontRicercaColore.resolve(in: EnvironmentValues()).green) + "\\blue" + round255(formato.fontRicercaColore.resolve(in: EnvironmentValues()).blue)
         + ";\\red" + round255(formato.fontGrecoColore.resolve(in: EnvironmentValues()).red) + "\\green" + round255(formato.fontGrecoColore.resolve(in: EnvironmentValues()).green) + "\\blue" + round255(formato.fontGrecoColore.resolve(in: EnvironmentValues()).blue)
         + ";\\red" + round255(formato.fontEbraicoColore.resolve(in: EnvironmentValues()).red) + "\\green" + round255(formato.fontEbraicoColore.resolve(in: EnvironmentValues()).green) + "\\blue" + round255(formato.fontEbraicoColore.resolve(in: EnvironmentValues()).blue)
         */
        
        return "";
    }
    
    func round255(_ color:Float) -> String {
        return String(Int(round(color*255)))
    }
    
    //#region aggiungere/rimuovere versioni
    func aggiungiDirectory(_ directory:URL) {
        if !directory.path().isEmpty {
            aggiungiDirectory(directory.path())
        }
    }
    /// <summary>
    /// Aggiunge tutti i testi trovati in una certa directory.
    /// </summary>
    /// <param name="directory">La cartella in cui cercare i file che contengono testi del programma.</param>
    func aggiungiDirectory(_ directory:String)
    {
        if !fileManager.fileExists(atPath:directory) {
            return;
        }
        
        var fileTrovati:[String] = []
        do
        {
            try fileTrovati = fileManager.contentsOfDirectory(atPath: directory)//(directory, "*.laparola");
        }
        catch
        {
            // non c'è l'autorizzazione di leggere quella directory. Saltiamola.
            return;
        }
        
        for fileTrovato in fileTrovati
        {
            if fileTrovato.hasSuffix(".laparola") {
                aggiungiTesto(directory+(directory.hasSuffix("/") ? "" : "/")+fileTrovato);
            }
        }
        
        /*fileTrovati = Directory.GetFiles(directory, "*.image_link");
         XmlNode nodePrincipale, subNode;
         string fileImmagine, nome;
         foreach (string fileTrovato in fileTrovati)
         {
         try
         {
         XmlDocument xd = new XmlDocument();
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
         if (indiceImmagini.ContainsKey(nome))
         indiceImmagini[nome].Add(fileImmagine);
         else
         {
         Collection<string> immaginiDellaParola = new Collection<string>
         {
         fileImmagine
         };
         indiceImmagini.Add(nome, immaginiDellaParola);
         }
         }
         }
         }
         catch
         {
         // errore nell'XML, saltiamo il file
         }
         }*/
    }
    
    /// <summary>
    /// Chiude i testi, salvando eventuali note modificate.
    /// <returns>Un elenco di versioni, separate da spazi, di cui non è stato possibile salvare le modifiche.</returns>
    /// </summary>
    deinit
    //func Chiudi() -> String
    {
        //var versioniNonSalvate = "";
        
        for (_, v) in versioni
        {
            v.chiudi();
        }
        
        //return versioniNonSalvate;
    }
    /*
     
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
     
     */
    /// <summary>
    /// Analizza un file dei dati e lo aggiunge all'elenco di quelli disponibili al programma.
    /// </summary>
    /// <param name="percorsoFile">Il percorso e nome del file dei dati.</param>
    /// <returns>Il nome del testo aggiunto (stringa vuota se non è stato possibile)</returns>
    func aggiungiTesto(_ percorsoFile:String) //-> String
    {
        var percorso:String = percorsoFile.removingPercentEncoding ?? percorsoFile
        if percorso.hasPrefix("file://") {
            percorso = percorso.remove(0, 7)
        }
        
        let nuovaVersione:Versione
        do {
            nuovaVersione = try Versione(self, percorso);
        }
        catch { // error nel file; non facciamo niente
            //nuovaVersione.Chiudi()
            return
        }
        var nomeTesto = nuovaVersione.info.nome;
        
        if (nomeTesto != "")
        {
            if versioni.keys.contains(nomeTesto) {
                // if exists already, don't add it but Close it and nil it
                nuovaVersione.chiudi()
                nomeTesto = ""
            }
            else {
                versioni[nomeTesto] = nuovaVersione
            }
        }
        else {
            nuovaVersione.chiudi()
        }
        //return nomeTesto;
    }
    
    /// <summary>
    /// Cancella il file che contiene un testo.
    /// </summary>
    /// <param name="nomeVersione">Il nome del testo da cancellare.</param>
    func cancellaTesto(_ nomeVersione:String) -> Bool
    {
        let successo = versioni[nomeVersione]?.cancella();
        if (successo ?? false) {
            versioni.removeValue(forKey:nomeVersione);
        }
        return successo ?? false
    }
    
    func rimuoviTesto(_ nomeVersione:String)
    {
        versioni[nomeVersione]?.chiudi();
        versioni.removeValue(forKey:nomeVersione);
    }
    
    /*
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
     File.Delete(fileNome);
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
     bw.Write(br.ReadUInt32());
     
     int posizione = (int)(br.ReadUInt32()) + nuovoNomeTesto.Length - nomeVersione.Length;
     bw.Write((UInt32)posizione);
     br.ReadString(); // il vecchio nome non ci interessa
     bw.Write(nuovoNomeTesto);
     bw.Write(br.ReadBytes((int)(fsRead.Length - fsRead.Position)));
     }
     finally
     {
     if (br != null)
     br.Close();
     if (fsRead != null)
     fsRead.Close();
     if (bw != null)
     bw.Close();
     if (fsWrite != null)
     fsWrite.Close();
     }
     
     AggiungiTesto(nomeNonEsistente, 0);
     return nuovoNomeTesto;
     }
     
     */
    /// <summary>
    /// Tutti i file dei dati attualmente disponibili.
    /// </summary>
    /// <returns>Una collezione di stringhe con i nomi di tutte le versioni disponibili.</returns>
    func nomiVersioni() -> [String]
    {
        var nomiVersioni = [String]()
        for (_, v) in versioni {
            nomiVersioni.append(v.info.nome);
        }
        nomiVersioni.sort();
        return nomiVersioni
    }
    
    /// <summary>
    /// Tutti i file dei dati attualmente disponibili che contengono almeno uno di certi tipi di testo.
    /// </summary>
    /// <param name="tipo">Il tipo di testo da cercare.</param>
    /// <returns>Una collezione di stringhe con i nomi di tutte le versioni del tipo giusto disponibili.</returns>
    /// <seealso cref="TestoTipi"/>
    func nomiVersioni(_ tipo:TestoTipi) -> [String]
    {
        return nomiVersioni(tipo, true);
    }
    
    /// <summary>
    /// Tutti i file dei dati attualmente disponibili che contengono certi tipi di testo.
    /// </summary>
    /// <param name="tipo">Il tipo di testo da cercare.</param>
    /// <param name="almenoUno">Se è vero (valore predefinito), almeno uno dei tipi in "tipo" deve essere presente nel testo; se è falso, tutti i tipi devono essere presenti.</param>
    /// <returns>Una collezione di stringhe con i nomi di tutte le versioni del tipo giusto disponibili.</returns>
    /// <seealso cref="TestoTipi"/>
    func nomiVersioni(_ tipo:TestoTipi, _ almenoUno:Bool) -> [String]
    {
        var nomiVersioni = [String]()
        for (_, v) in versioni
        {
            if (almenoUno)
            {
                if ((v.info.tipo.rawValue & tipo.rawValue) != 0) {
                    nomiVersioni.append(v.info.nome);
                }
            }
            else
            {
                if ((v.info.tipo.rawValue & tipo.rawValue) == tipo.rawValue) {
                    nomiVersioni.append(v.info.nome)
                }
            }
        }
        nomiVersioni.sort();
        return nomiVersioni;
    }
    
    /// <summary>
    /// Informazioni su un file dei dati.
    /// </summary>
    /// <param name="nomeVersione">Il nome della versione nel file dei dati.</param>
    /// <returns>Informazioni sulla versione. Se la versione non esiste, tutti i campi delle informazioni sono vuoti.</returns>
    func info(_ nomeVersione:String) -> VersioneInformazioni
    {
        return versioni[nomeVersione]?.info ?? VersioneInformazioni()
    }
    
    /// <summary>
    /// Come il testo va mostrato nella finestra di visualizzazione, quando il tipo non è specificato.
    /// </summary>
    /// <param name="nomeVersione">Il nome della versione nel file dei dati.</param>
    /// <returns>Bibbia se è una Bibbia, Commentario se contiene un commentario, Dizionario se non è un commentario.</returns>
    func tipoPrincipaleDiTesto(_ nomeVersione:String) -> TestoTipi
    {
        var tipo = TestoTipi.None
        if ((info(nomeVersione).tipo.rawValue & TestoTipi.Bibbia.rawValue) == TestoTipi.Bibbia.rawValue)
        {
            tipo = TestoTipi.Bibbia;
        }
        else if ((info(nomeVersione).tipo.rawValue & TestoTipi.Commentario.rawValue) == TestoTipi.Commentario.rawValue)
        {
            tipo = TestoTipi.Commentario;
        }
        else if ((info(nomeVersione).tipo.rawValue & TestoTipi.Dizionario.rawValue) == TestoTipi.Dizionario.rawValue)
        {
            tipo = TestoTipi.Dizionario;
        }
        return tipo;
    }
    
    /*
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
     return kvp.Key;
     }
     return "";
     }
     */
    /// <summary>
    /// Se un testo con un certo nome esiste.
    /// </summary>
    /// <param name="nomeVersione">Il nome del testo da cercare.</param>
    /// <returns>Vero se un testo esiste con il nome.</returns>
    public func versioneEsiste(_ nomeVersione:String) -> Bool
    {
        for (nome, _) in versioni {
            if (nome == nomeVersione) {
                return true;
            }
        }
        return false;
    }
    
    //#region Ricerca
    
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
    public func ricerca(_ espressione:String, _ brano:String = "", _ nomeVersione:String) throws -> Riferimento
    {
        do {
            return try ricerca(espressione, convertiRiferimento(brano), nomeVersione);
        }
        catch let error {
            throw error
        }
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
    public func ricerca(_ espressione:String, _ riferimentoDaRicercare:Riferimento, _ nomeVersione:String) throws -> Riferimento
    {
        do {
            let espressione = try controllaEspressioneDaRicercare(espressione, nomeVersione);
            let versettiTrovati:Riferimento = trovaOccorrenzeEspressione(espressione, riferimentoDaRicercare, false, 0, nomeVersione);
            return unisciVociRipetute(versettiTrovati);
        }
        catch let error {
            throw error
        }
    }
    
    /*
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
     throw TextNotExistException
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
     throw TextNotExistException
     }
     }
     
     */
    
    private func unisciVociRipetute(_ riferimento:Riferimento) -> Riferimento
    {
        var rif = riferimento
        if (rif.versetti)
        {
            let nVersetti = rif.brani.count;
            for i in stride(from:(nVersetti-1), to:0, by:-1) {
                if (rif.primoVersettoUguale(i - 1, i))
                {
                    rif.numeroParola[i - 1].append(contentsOf:rif.numeroParola[i]);
                    rif.brani.remove(at:i);
                    rif.numeroParola.remove(at:i);
                }
                else {
                    rif.numeroParola[i].sort();
                }
            }
            if (nVersetti > 0) {
                rif.numeroParola[0].sort();
            }
        }
        else
        {
            let nNote = rif.note.count;
            for i in stride(from:(nNote-1), to:0, by:-1) {
                if (rif.note[i - 1] == rif.note[i])
                {
                    rif.numeroParola[i - 1].append(contentsOf:rif.numeroParola[i]);
                    rif.note.remove(at:i);
                    rif.numeroParola.remove(at:i);
                }
                else {
                    rif.numeroParola[i].sort();
                }
            }
            if (nNote > 0) {
                rif.numeroParola[0].sort();
            }
        }
        return rif;
    }
    
    private func trovaOccorrenzeEspressione(_ espressione:String, _ branoDaRicercare:Riferimento, _ inFrase:Bool, _ numeroParoleInFrase:Int, _ nomeVersione:String) -> Riferimento
    {
        var nParoleInFrase = numeroParoleInFrase
        // se branoDaRicerca non contiene brani, tutta la Bibbia (oppure tutta la collezione di note) è ricercata
        var espressioneDaTrovare = espressione;
        let char0:String = String(UnicodeScalar(0))
        espressioneDaTrovare += char0
        var riferimenti = Riferimento();
        var tipoOperazione = ""
        while (espressioneDaTrovare != char0)
        {
            var primoCarattere = espressioneDaTrovare[0];
            if (primoCarattere == "~")
            {
                primoCarattere = "0";
                espressioneDaTrovare = "0" + espressioneDaTrovare;
            }
            if (Character(primoCarattere).isWholeNumber || primoCarattere == ":")
            {
                if (Character(espressioneDaTrovare[1]).isWholeNumber) {
                    tipoOperazione = "prima";
                }
                else
                {
                    tipoOperazione = espressioneDaTrovare[0..<1];
                    espressioneDaTrovare = espressioneDaTrovare.remove(0, 1);
                    if (espressioneDaTrovare[0] == "~")
                    {
                        tipoOperazione += "n";
                        espressioneDaTrovare = espressioneDaTrovare.remove(0, 1);
                    }
                }
            }
            else
            {
                if (primoCarattere == "|")
                {
                    tipoOperazione = "oppure";
                    espressioneDaTrovare = espressioneDaTrovare.remove(0, 1);
                }
                else {
                    tipoOperazione = "prima";
                }
            } // if ((IsNumero(cPrimoCarattere)) || cPrimoCarattere==':') else
            
            var i:Int;
            var occorrenzeProssimaParola = Riferimento();
            primoCarattere = espressioneDaTrovare[0];
            if (primoCarattere == "(")
            {
                i = 0;
                var nParentesi = 1;
                repeat
                {
                    i += 1;
                    if (espressioneDaTrovare[i] == ")") {
                        nParentesi -= 1
                    }
                    if (espressioneDaTrovare[i] == "(") {
                        nParentesi += 1
                    }
                } while (nParentesi != 0);
                occorrenzeProssimaParola = trovaOccorrenzeEspressione(espressioneDaTrovare[1..<i], branoDaRicercare, false, nParoleInFrase, nomeVersione);
                espressioneDaTrovare = espressioneDaTrovare.remove(0, i + 1);
            }
            else if (primoCarattere == "[")
            {
                i = espressioneDaTrovare.indexOf("]");
                nParoleInFrase = 0;
                occorrenzeProssimaParola = trovaOccorrenzeEspressione(espressioneDaTrovare[1..<i], branoDaRicercare, true, nParoleInFrase, nomeVersione);
                espressioneDaTrovare = espressioneDaTrovare.remove(0, i + 1);
            }
            else
            {
                let parola = prossimaParola(espressioneDaTrovare, 0);
                occorrenzeProssimaParola = versioni[nomeVersione]?.ricercaParolaInBrano(parola, branoDaRicercare) ?? Riferimento();
                var lunghezzaExtra = (espressioneDaTrovare[0] == "<" ? 1 : 0);
                if (lunghezzaExtra == 1 && espressioneDaTrovare.indexOf(">") >= 0) {
                    lunghezzaExtra += 1
                }
                espressioneDaTrovare = espressioneDaTrovare.remove(0, parola.count + lunghezzaExtra);
                nParoleInFrase += 1
            } // if (cPrimoCarattere=='(') else
            
            var occorrenzeInBrano = Riferimento();
            if (tipoOperazione == "prima") {
                riferimenti = occorrenzeProssimaParola;
            }
            else
            {
                primoCarattere = tipoOperazione[0];
                if (Character(primoCarattere).isWholeNumber || primoCarattere == ":")
                {
                    var primoCarattereComeNumero = 0
                    if (primoCarattere == ":") {
                        primoCarattereComeNumero = Int.max / 2; // "/2" altrimenti quando si aggiunge un numero ad esso, diventa negativo
                    }
                    else {
                        primoCarattereComeNumero = Int(primoCarattere) ?? 0
                    }
                    if (!inFrase || tipoOperazione.count == 1)
                    {
                        if (riferimenti.versetti)
                        {
                            var j = 1
                            var i = 1;
                            let nI = riferimenti.count();
                            let nJ = occorrenzeProssimaParola.count();
                            var nVersettoRiferimenti = (nI > 0 ? versettiFinoACapitolo(riferimenti.brani[i - 1][0], riferimenti.brani[i - 1][1] - 1, nomeVersione) + Int(riferimenti.brani[i - 1][2]) : 0);
                            var nVersettoOccorrenze = (nJ > 0 ? versettiFinoACapitolo(occorrenzeProssimaParola.brani[j - 1][0], occorrenzeProssimaParola.brani[j - 1][1] - 1, nomeVersione) + Int(occorrenzeProssimaParola.brani[j - 1][2]) : 0);
                            while ((i <= nI) && (j <= nJ))
                            {
                                if (inFrase)
                                {
                                    if (nVersettoOccorrenze < nVersettoRiferimenti || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0]))
                                    {
                                        j += 1
                                        if (j <= nJ) {
                                            nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.brani[j - 1][0], occorrenzeProssimaParola.brani[j - 1][1] - 1, nomeVersione) + Int(occorrenzeProssimaParola.brani[j - 1][2])
                                        }
                                    }
                                    else
                                    {
                                        if (nVersettoOccorrenze > nVersettoRiferimenti || (nVersettoOccorrenze == nVersettoRiferimenti && (occorrenzeProssimaParola.numeroParola[j - 1][0] > Int(riferimenti.numeroParola[i - 1][0]) + primoCarattereComeNumero + 1)))
                                        {
                                            if (tipoOperazione.count > 1)
                                            {
                                                occorrenzeInBrano.brani.append(riferimenti.brani[i - 1]);
                                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                            }
                                        }
                                        else
                                        {
                                            if (tipoOperazione.count == 1)
                                            {
                                                occorrenzeInBrano.brani.append(riferimenti.brani[i - 1]);
                                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                                occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].insert(occorrenzeProssimaParola.numeroParola[j - 1][0], at:0);
                                            }
                                        }
                                        i += 1
                                        if (i <= nI) {
                                            nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.brani[i - 1][0], riferimenti.brani[i - 1][1] - 1, nomeVersione) + Int(riferimenti.brani[i - 1][2]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (nVersettoOccorrenze < nVersettoRiferimenti - primoCarattereComeNumero)
                                    {
                                        j += 1
                                        if (j <= nJ) {
                                            nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.brani[j - 1][0], occorrenzeProssimaParola.brani[j - 1][1] - 1, nomeVersione) + Int(occorrenzeProssimaParola.brani[j - 1][2]);
                                        }
                                    }
                                    else
                                    {
                                        if (nVersettoOccorrenze > nVersettoRiferimenti + primoCarattereComeNumero)
                                        {
                                            if (tipoOperazione.count > 1)
                                            {
                                                occorrenzeInBrano.brani.append(riferimenti.brani[i - 1]);
                                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                            }
                                        }
                                        else
                                        {
                                            if (tipoOperazione.count == 1)
                                            {
                                                occorrenzeInBrano.brani.append(riferimenti.brani[i - 1]);
                                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                                if (primoCarattereComeNumero == 0)
                                                { // seconda parola nel versetto anche, quindi va sottolineata
                                                    occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].insert(occorrenzeProssimaParola.numeroParola[j - 1][0], at:0);
                                                    while (j < nJ && occorrenzeProssimaParola.primoVersettoUguale(j - 1, j))
                                                    {
                                                        occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].append(occorrenzeProssimaParola.numeroParola[j][0]);
                                                        j += 1
                                                    }
                                                }
                                            }
                                        }
                                        i += 1
                                        if (i <= nI) {
                                            nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.brani[i - 1][0], riferimenti.brani[i - 1][1] - 1, nomeVersione) + Int(riferimenti.brani[i - 1][2]);
                                        }
                                    }
                                }
                            } // while (i <= riferimenti.Count && j <= occorrenzeProssimaParola.Count)
                            if (tipoOperazione.count > 1)
                            {
                                while (i <= riferimenti.count())
                                {
                                    occorrenzeInBrano.brani.append(riferimenti.brani[i - 1]);
                                    occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                    i += 1
                                }
                            }
                            riferimenti = occorrenzeInBrano;
                        }
                        else // if (riferimenti.Versetti)
                        {
                            occorrenzeInBrano.versetti = false;
                            var j = 1, i = 1;
                            let nI = riferimenti.count();
                            let nJ = occorrenzeProssimaParola.count();
                            var notaRiferimenti = (nI > 0 ? riferimenti.note[i - 1] : "");
                            var notaOccorrenze = (nJ > 0 ? occorrenzeProssimaParola.note[j - 1] : "");
                            var differenzaVersetti = -1;
                            var differenzaRicercata = primoCarattereComeNumero;
                            while (i <= nI && j <= nJ)
                            {
                                if (inFrase)
                                { // TOD2 per dizionari questo string compare funziona anche con lettere greche? ci sono funzioni alternative e options da usare
                                    // nella documentazione di nsstring / compare
                                    if (notaOccorrenze.compare(notaRiferimenti) == .orderedAscending || (notaOccorrenze == notaRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0]))
                                    {
                                        j += 1
                                        if (j <= nJ) {
                                            notaOccorrenze = occorrenzeProssimaParola.note[j - 1];
                                        }
                                    }
                                    else
                                    {
                                        if (notaOccorrenze.compare(notaRiferimenti) == .orderedDescending || (notaOccorrenze == notaRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] > Int(riferimenti.numeroParola[i - 1][0]) + primoCarattereComeNumero + 1))
                                        {
                                            if (tipoOperazione.count > 1)
                                            {
                                                occorrenzeInBrano.note.append(riferimenti.note[i - 1]);
                                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                            }
                                        }
                                        else
                                        {
                                            if (tipoOperazione.count == 1)
                                            {
                                                occorrenzeInBrano.note.append(riferimenti.note[i - 1]);
                                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                                occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].insert(occorrenzeProssimaParola.numeroParola[j - 1][0], at:0);
                                            }
                                        }
                                        i += 1
                                        if (i <= nI) {
                                            notaRiferimenti = riferimenti.note[i - 1];
                                        }
                                    }
                                }
                                else
                                {
                                    let differenza = calcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze);
                                    differenzaVersetti = differenza.differenzaVersetti
                                    differenzaRicercata = differenza.differenzaRicercata
                                    if (differenzaVersetti < -differenzaRicercata) // cioè string.Compare(notaOccorrenze, notaRiferimenti) < 0 per due note quando una non è ad un brano
                                    {
                                        j += 1
                                        if (j <= nJ) {
                                            notaOccorrenze = occorrenzeProssimaParola.note[j - 1];
                                        }
                                    }
                                    else
                                    {
                                        if (differenzaVersetti > differenzaRicercata) // cioè string.Compare(notaOccorrenze, notaRiferimenti) > 0 per due note quando una non è ad un brano
                                        {
                                            if (tipoOperazione.count > 1)
                                            {
                                                occorrenzeInBrano.note.append(notaRiferimenti);
                                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                            }
                                        }
                                        else
                                        {
                                            if (tipoOperazione.count == 1)
                                            {
                                                if (occorrenzeInBrano.note.count > 0 && notaRiferimenti == occorrenzeInBrano.note[occorrenzeInBrano.note.count - 1])
                                                {
                                                    occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].insert(riferimenti.numeroParola[i - 1][0], at:0);
                                                }
                                                else
                                                {
                                                    occorrenzeInBrano.note.append(notaRiferimenti);
                                                    occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                                    if (differenzaRicercata == 0) { // seconda parola nel versetto anche, quindi va sottolineata
                                                        occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].insert(occorrenzeProssimaParola.numeroParola[j - 1][0], at:0);
                                                    }
                                                }
                                                j += 1
                                                if (j <= nJ)
                                                {
                                                    notaOccorrenze = occorrenzeProssimaParola.note[j - 1];
                                                    let differenza = calcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze);
                                                    differenzaVersetti = differenza.differenzaVersetti
                                                    differenzaRicercata = differenza.differenzaRicercata
                                                }
                                                while (abs(differenzaVersetti) <= differenzaRicercata && j <= nJ)
                                                {
                                                    occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].insert(occorrenzeProssimaParola.numeroParola[j - 1][0], at:0);
                                                    j += 1
                                                    if (j <= nJ)
                                                    {
                                                        notaOccorrenze = occorrenzeProssimaParola.note[j - 1];
                                                        let differenza = calcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze);
                                                        differenzaVersetti = differenza.differenzaVersetti
                                                        differenzaRicercata = differenza.differenzaRicercata
                                                    }
                                                }
                                                while (i < nI && riferimenti.note[i - 1] == riferimenti.note[i])
                                                {
                                                    i += 1
                                                    occorrenzeInBrano.numeroParola[occorrenzeInBrano.numeroParola.count - 1].insert(riferimenti.numeroParola[i - 1][0], at:0);
                                                }
                                            }
                                        }
                                        i += 1
                                        if (i <= nI) {
                                            notaRiferimenti = riferimenti.note[i - 1];
                                        }
                                    }
                                }
                            } // while (i <= nI && j <= nJ)
                            if (tipoOperazione.count > 1)
                            {
                                while (i <= riferimenti.count())
                                {
                                    occorrenzeInBrano.note.append(riferimenti.note[i - 1]);
                                    occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                    i += 1
                                }
                            }
                            riferimenti = occorrenzeInBrano;
                        } // if (riferimenti.Versetti) else
                    } // if (!inFrase || tipoOper.Length == 1)
                } // if (Character(primoCarattere).isWholeNumber) {
                else
                {
                    if (primoCarattere == "o")
                    {
                        if (riferimenti.versetti)
                        {
                            var j = 1, i = 1;
                            if (riferimenti.count() > 0 && occorrenzeProssimaParola.count() > 0)
                            {
                                var nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.brani[i - 1][0], riferimenti.brani[i - 1][1] - 1, nomeVersione) + Int(riferimenti.brani[i - 1][2]);
                                var nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.brani[j - 1][0], occorrenzeProssimaParola.brani[j - 1][1] - 1, nomeVersione) + Int(occorrenzeProssimaParola.brani[j - 1][2]);
                                let nI = riferimenti.count();
                                let nJ = occorrenzeProssimaParola.count();
                                while (i <= nI && j <= nJ)
                                {
                                    if (nVersettoOccorrenze < nVersettoRiferimenti || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0]))
                                    {
                                        occorrenzeInBrano.brani.append(occorrenzeProssimaParola.brani[j - 1]);
                                        occorrenzeInBrano.numeroParola.append(occorrenzeProssimaParola.numeroParola[j - 1]);
                                        j += 1
                                        if (j <= nJ) {
                                            nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.brani[j - 1][0], occorrenzeProssimaParola.brani[j - 1][1] - 1, nomeVersione) + Int(occorrenzeProssimaParola.brani[j - 1][2]);
                                        }
                                    }
                                    else
                                    {
                                        occorrenzeInBrano.brani.append(riferimenti.brani[i - 1]);
                                        occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                        i += 1
                                        if (i <= nI) {
                                            nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.brani[i - 1][0], riferimenti.brani[i - 1][1] - 1, nomeVersione) + Int(riferimenti.brani[i - 1][2]);
                                        }
                                    }
                                } // while
                            }
                            while (j <= occorrenzeProssimaParola.count())
                            {
                                occorrenzeInBrano.brani.append(occorrenzeProssimaParola.brani[j - 1]);
                                occorrenzeInBrano.numeroParola.append(occorrenzeProssimaParola.numeroParola[j - 1]);
                                j += 1
                            }
                            while (i <= riferimenti.count())
                            {
                                occorrenzeInBrano.brani.append(riferimenti.brani[i - 1]);
                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                i += 1
                            }
                            riferimenti = occorrenzeInBrano;
                        }
                        else // collezioni di note
                        {
                            occorrenzeInBrano.versetti = false;
                            var j = 1, i = 1;
                            let nI = riferimenti.count();
                            let nJ = occorrenzeProssimaParola.count();
                            var notaRiferimenti = (nI > 0 ? riferimenti.note[i - 1] : "");
                            var notaOccorrenze = (nJ > 0 ? occorrenzeProssimaParola.note[j - 1] : "");
                            while (i <= nI && j <= nJ)
                            {
                                if (notaOccorrenze.compare(notaRiferimenti) == .orderedAscending) || (notaOccorrenze == notaRiferimenti && occorrenzeProssimaParola.numeroParola[j - 1][0] < riferimenti.numeroParola[i - 1][0])
                                {
                                    occorrenzeInBrano.note.append(occorrenzeProssimaParola.note[j - 1]);
                                    occorrenzeInBrano.numeroParola.append(occorrenzeProssimaParola.numeroParola[j - 1]);
                                    j += 1
                                    if (j < nJ) {
                                        notaOccorrenze = occorrenzeProssimaParola.note[j - 1];
                                    }
                                }
                                else
                                {
                                    occorrenzeInBrano.note.append(riferimenti.note[i - 1]);
                                    occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                    i += 1
                                    if (i < nI) {
                                        notaRiferimenti = riferimenti.note[i - 1];
                                    }
                                }
                            } // while
                            while (j <= occorrenzeProssimaParola.count())
                            {
                                occorrenzeInBrano.note.append(occorrenzeProssimaParola.note[j - 1]);
                                occorrenzeInBrano.numeroParola.append(occorrenzeProssimaParola.numeroParola[j - 1]);
                                j += 1
                            }
                            while (i <= riferimenti.count())
                            {
                                occorrenzeInBrano.note.append(riferimenti.note[i - 1]);
                                occorrenzeInBrano.numeroParola.append(riferimenti.numeroParola[i - 1]);
                                i += 1
                            }
                            riferimenti = occorrenzeInBrano;
                        }
                    }
                } // if (Character(primoCarattere).isWholeNumber) else
            }
        }
        
        return riferimenti;
    }
    
    private func calcolaDifferenzeDelleNote(_ primoCarattereComeNumero:Int, _ notaRiferimenti:String, _ notaOccorrenze:String) -> (differenzaVersetti:Int, differenzaRicercata:Int)
    {
        var differenzaVersetti = -1;
        var differenzaRicercata = primoCarattereComeNumero;
        if (notaOccorrenze.hasPrefix("#") && notaRiferimenti.hasPrefix("#")) {
            differenzaVersetti = versettiFinoACapitolo(UInt8(notaOccorrenze[1..<3]) ?? 0, UInt8(notaOccorrenze[3..<6]) ?? 0)
            + (Int(notaOccorrenze[6..<9]) ?? 0)
            - versettiFinoACapitolo(UInt8(notaRiferimenti[1..<3]) ?? 0, UInt8(notaRiferimenti[3..<6]) ?? 0)
            - (Int(notaRiferimenti[6..<9]) ?? 0);
        }
        
        else
        {
            switch notaOccorrenze.compare(notaOccorrenze) {
            case .orderedAscending:
                differenzaVersetti = -1
            case .orderedSame:
                differenzaVersetti = 0
            case .orderedDescending:
                differenzaVersetti = 1
            }
            differenzaRicercata = 0;
        }
        return (differenzaVersetti, differenzaRicercata)
    }
    
    private func controllaEspressioneDaRicercare(_ espressionedaRicercare:String, _ nomeVersione:String) throws -> String
    {
        var espressione = espressionedaRicercare.trimmingCharacters(in: .whitespacesAndNewlines)
        if (espressione.isEmpty) {
            throw SearchException.SearchExpressionEmpty
        }
        var nParentesiSinistra = 0, nParentesiDestra = 0, nParentesiQuadrateSinistra = 0;
        var erroreSintassi = -1;
        var erroreParentesi = false, erroreParentesiQuadrate = false;
        var a:String, b:String, c:String;
        
        espressione = espressione.lowercased();
        
        if (versioni[nomeVersione]?.info.lingua.lowercased().split(separator:"|", omittingEmptySubsequences: true).map{String($0)}.firstIndex(of: "it") ?? -1 >= 0)
        {
            var p = 0;
            while (espressione.indexOf("'", p + 1) > -1)
            {
                p = espressione.indexOf("'", p + 1);
                if (p < espressione.count - 1 && (Character(espressione[p + 1]).isLetter || espressione[p + 1] == "*" || espressione[p + 1] == "?")) {
                    espressione.insert(p + 1, " ");
                }
            }
        }
        
        espressione = espressione.replacingOccurrences(of:" ", with:" "); // il primo spazio è il carattere xA0 (spazio unificatore), il secondo x20 (spazio normale)
        espressione = espressione.replacingOccurrences(of:"^", with:"~");
        espressione = espressione.replacingOccurrences(of:"!", with:"|");
        var prossimaParentesiQuadrate = "[";
        while (espressione.indexOf("\"") >= 0)
        {
            espressione = espressione[0..<(espressione.indexOf("\""))] + prossimaParentesiQuadrate + espressione[(espressione.indexOf("\"") + 1)...];
            prossimaParentesiQuadrate = (prossimaParentesiQuadrate == "[" ? "]" : "[");
        }
        
        var i = 0
        while i < espressione.count-1 {
            c = espressione[i];
            if (c == " " && i > 1)
            {
                a = espressione[i - 1];
                b = espressione[i + 1];
                if ((a < "a" && a != "'" && a != "-" && a != "]" && a != ")" && !(Character(a).isLetter || a == "*" || a == "?")) || a == "~" || a == "|" || a == ":" || a == "<" || a == ">" || (b < "a" && b != "'" && b != "-" && b != "(" && b != "[" && !(Character(b).isLetter || b == "*" || b == "?")) || b == "~" || b == "|" || b == ":" || b == "<" || a == ">")
                {
                    espressione = espressione.remove(i, 1);
                    i -= 1
                }
            }
            i += 1
        }
        
        i = 0
        while i < espressione.count {
            c = espressione[i];
            if (i == 0)
            {
                if (c == "(") {
                    nParentesiSinistra += 1
                }
                else if (c == "[") {
                    nParentesiQuadrateSinistra += 1
                }
                else if (c == "<")
                {
                    let nuovoI = espressione.indexOf(">", i);
                    if (nuovoI > i) {
                        i = nuovoI;
                    }
                    else {
                        erroreSintassi = i;
                    }
                }
                else if (!(Character(c).isLetter || c == "'" || c == "/" || c == "\\" || c == "*" || c == "?")) {
                    erroreSintassi = i;
                }
            }
            else {
                a = espressione[i - 1];
                if (c == " ") {
                    espressione = espressione.remove(i, 1);
                    espressione.insert(i, "0");
                }
                else if (c == "-" || c == "'") {
                    if (!(Character(a).isLetter || a == "*" || a == "?")) {
                        erroreSintassi = i;
                    }
                }
                else if (c == "/" || c == "\\")
                {
                    if (a == "/" || a == "\\" || a == "<") {
                        erroreSintassi = i;
                    }
                    else
                    {
                        if (a != "|" && a != ":" && a != "~" && (!Character(a).isWholeNumber) && a != "(" && a != "[")
                        {
                            espressione.insert(i, "0");
                            i += 1;
                        }
                    }
                }
                else if (c == "(")
                {
                    nParentesiSinistra += 1;
                    if (nParentesiQuadrateSinistra > 0 && nParentesiSinistra > 1) {
                        erroreParentesiQuadrate = true;
                    }
                    if (a == "/" || a == "\\" || a == ":" || a == "<") {
                        erroreSintassi = i;
                    }
                    else
                    {
                        if (a != "|" && a != "~" && a != "[" && (!Character(a).isWholeNumber))
                        {
                            espressione.insert(i, "0");
                            i += 1
                        }
                    }
                }
                else if (c == ")")
                {
                    nParentesiDestra += 1
                    if (nParentesiDestra > nParentesiSinistra) {
                        erroreParentesi = true;
                    }
                    if ((a >= "/" && a <= ":") || a == "|" || a == "~" || a == "\\" || a == "<") {
                        erroreSintassi = i;
                    }
                }
                else if (c == "[")
                {
                    nParentesiQuadrateSinistra += 1
                    if (nParentesiQuadrateSinistra > 1) {
                        erroreParentesiQuadrate = true;
                    }
                    if (a == "/" || a == "\\" || a == ":" || a == "<") {
                        erroreSintassi = i;
                    }
                    else
                    {
                        if (a != "|" && a != "~" && (!Character(a).isWholeNumber))
                        {
                            espressione.insert(i, "0");
                            i += 1
                        }
                    }
                }
                else if (c == "]")
                {
                    nParentesiQuadrateSinistra -= 1
                    if (nParentesiQuadrateSinistra < 0) {
                        erroreParentesiQuadrate = true;
                    }
                    if (nParentesiDestra - nParentesiSinistra < 0) {
                        erroreParentesi = true;
                    }
                    if (Character(a).isWholeNumber || a == "/" || a == ":" || a == "|" || a == "~" || a == "\\" || a == "<") {
                        erroreSintassi = i;
                    }
                }
                else if (c == "|" || Character(c).isWholeNumber)
                {
                    if (a != ")" && a != "]" && a != "<" && a != ">" && !(Character(a).isLetter || a == "*" || a == "?")) {
                        erroreSintassi = i;
                    }
                    if (nParentesiQuadrateSinistra == 1 && c == "|")
                    {
                        b = "a";
                        var j = i + 1
                        while (b != "]" && (!Character(b).isWholeNumber) && b != ":" && j < espressione.count) {
                            b = espressione[j];
                            j += 1
                        }
                        espressione.insert(j - 1, ")");
                        b = "a";
                        j = i - 1
                        while (b != "[" && (!Character(b).isWholeNumber) && b != ":" && j >= 0) {
                            b = espressione[j];
                            j -= 1
                        }
                        espressione.insert(j + 2, "(");
                        i += 1
                        nParentesiSinistra += 1
                    }
                }
                else if (c == ":")
                {
                    if ((a != ")" && a != ">" && !(Character(a).isLetter || a == "*" || a == "?")) || nParentesiQuadrateSinistra == 0)  {
                        erroreSintassi = i;
                    }
                }
                else if (c == "~")
                {
                    if (a == "<" || a == "(" || a == "[" || a == ":" || a == "/" || a == "|" || a == "~" || a == "\\" || (nParentesiQuadrateSinistra == 1 && nParentesiSinistra > 0)) {
                        erroreSintassi = i;
                    }
                    else
                    {
                        if (a == ")" || a > "]" || Character(a).isLetter || a == "*" || a == "?") {
                            espressione.insert(i, "0");
                            i += 1
                        }
                    }
                    if (nParentesiQuadrateSinistra == 1 && (Character(espressione[i + 1]).isLetter || espressione[i + 1] == "*" || espressione[i + 1] == "?" || espressione[i + 1] == "/" || espressione[i + 1] == "\\"))
                    {
                        espressione.insert(i + 1, "(");
                        var j = i + 2
                        b = "a";
                        while (b != "]" && (!Character(b).isWholeNumber) && b != ":" && j < espressione.count) {
                            b = espressione[j];
                            j += 1
                        }
                        espressione.insert(j, ")");
                    }
                }
                else if (c == ">")
                {
                }
                else if (c == "<") {
                    if (a == ")" || a == "]" || a == ">" || Character(a).isLetter || a == "*" || a == "?") {
                        espressione.insert(i, "0");
                        i += 1
                    }
                    let nuovoI = espressione.indexOf(">", i);
                    if (nuovoI > i) {
                        i = nuovoI;
                    }
                    else {
                        erroreSintassi = i;
                    }
                }
                else if (Character(c).isLetter || c == "*" || c == "?" || c == "<")
                {    // lettera (senza o con accento)
                    if (a == ")" || a == "]" || a == ">")
                    {
                        espressione.insert(i, "0");
                        i += 1
                    }
                }
                else {   // carattere non riconosciuto
                    erroreSintassi = i;
                }
            } // if (i == 0) - else
            i += 1
        } // for (int i = 0; i < espressione.Length; ++i)
        
        a = espressione[espressione.count - 1];
        if (!(a == ")" || a == "]" || a == "-" || a == "'" || Character(a).isLetter || a == "*" || a == "?" || a == ">")) {
            erroreSintassi = espressione.count - 1;
        }
        
        if (nParentesiSinistra != nParentesiDestra) {
            erroreParentesi = true;
        }
        if (nParentesiQuadrateSinistra == 1) {
            erroreParentesiQuadrate = true;
        }
        if (erroreParentesiQuadrate) {
            erroreParentesi = false; // indicare solo uno degli errori
        }
        if (erroreSintassi >= 0) {
            throw SearchException.SearchSyntaxError(erroreSintassi)
        }
        if (erroreParentesi) {
            throw SearchException.SearchParentheses
        }
        if (erroreParentesiQuadrate) {
            throw SearchException.SearchBrackets
        }
        
        return espressione;
    }
    
    private func prossimaParola(_ fraseRicercata:String, _ inizio:Int) -> String
    {
        var j = 0;
        var prossimaParola = ""
        let frase = fraseRicercata[inizio...] + " "; // con " ", la riga c = sFraseRicercata[iInizio+j] funziona anche quando passa oltre la fine di sFraseRicercata
        var c = frase[0];
        var p:Int
        if (c == "<")
        {
            p = frase.indexOf(">");
            return (p > 0 ? frase[1..<p] : "");
        }
        else if (Character(c).isWholeNumber)
        {
            while (Character(c).isWholeNumber)
            {
                prossimaParola += c
                j += 1
                c = frase[j];
            }
        }
        else
        {
            while ((Character(c).isLetter) || c == "-" || c == "'" || c == "*" || c == "?" || c == "/" || c == "\\")
            {
                prossimaParola += c
                j += 1
                c = frase[j];
            }
        }
        
        return prossimaParola;
    }
    
    func testoDaRiferimento(_ rif:Riferimento, _ nomeVersione:String) -> String {
        // lpnb://#220260070000-220260070000
        let versioneDaUsare = (info(nomeVersione).tipo == TestoTipi.Bibbia) ? nomeVersione : (UserDefaults.standard.string(forKey: "versionepreferita") ?? "")
        var linkProssimoCapitolo = ""
        if rif.count() == 1 {
            var libro = rif.brani[0][3]
            var capitolo = rif.brani[0][4]
            if (capitolo < 250) { // capitolo potrebbe essere 255, poi +1 errore perché UInt8
                capitolo += 1
            }
            if capitolo > capitoliInLibro(libro, versioneDaUsare) {
                capitolo = 1
                libro += 1
                while libro < 74 && capitoliInLibro(libro, versioneDaUsare) == 0 {
                    libro += 1
                }
            }
            if libro < 74 {
                var libroString = "00" + String(libro);
                libroString = libroString[(libroString.count - 2)...]
                var capitoloString = "00" + String(capitolo);
                capitoloString = capitoloString[(capitoloString.count - 3)...]
                linkProssimoCapitolo = "<a href=\"lpnb://#"+libroString+capitoloString+"0010000_"+libroString+capitoloString+"2550000?ip=1\" id=\"prossimocapitolo\"></a>" // _ invece di - per stessa finestra
            }
        }
        var stringDaRestituire = testoBrano(rif, nomeVersione)
        if stringDaRestituire.hasSuffix("</body></html>") {
            stringDaRestituire.insert(stringDaRestituire.indexOf("</body></html>"), linkProssimoCapitolo)
        }
        return stringDaRestituire
    }
    
    func testoRicerca(_ riferimento:String, _ nomeVersione:String) -> String
    {
        if riferimento.isEmpty {
            return String(localized: "L'espressione da ricercare era vuota.")
        }
        let c2 = Character(riferimento[riferimento.count-1])
        let c1 = Character(riferimento[0])
        if c2.isNumber || c1=="1" || c1=="2" || c1=="3" {
            let rif = convertiRiferimento(riferimento)
            return testoDaRiferimento(rif, nomeVersione)
        }
        else {
            do {
                let rif = try ricerca(riferimento, "", nomeVersione)
                if rif.count() == 0 {
                    return String(localized: "Non sono stati trovati versetti che contengono questa espressione da ricercare.")
                }
                return testoBrano(rif, nomeVersione)
            } catch SearchException.SearchExpressionEmpty {
                return String(localized: "L'espressione da ricercare era vuota.")
            } catch SearchException.SearchParentheses {
                return String(localized: "Le parentesi non corrispondono nell'espressione da ricercare.")
            } catch SearchException.SearchBrackets {
                return String(localized: "Le parentesi quadrate non corrispondono nell'espressione da ricercare.")
            } catch SearchException.SearchSyntaxError(let car) {
                return String(localized: "Errore di sintasi nell'espressione da ricercare approssimativamente al carattere numero ") + "\(car+1)."
            } catch {
                return String(localized: "Errore sconosciuto nella ricerca")
            }
        }
    }
    
    //#region TestoBrano
    
    
    func testoBrano(_ riferimento:String, _ nomeVersione:String, _ collezioniDaVisualizzare:[String] = [], _ conNomiVersioni:Bool = true, _ paroleRicercate:Riferimento = Riferimento(), _ alternare:Bool = false) -> String
    {
        return testoBrano(convertiRiferimento(riferimento), nomeVersione, collezioniDaVisualizzare, conNomiVersioni, paroleRicercate, alternare);
    }
    
    func testoBrano(_ riferimento:String, _ listaVersioni:[String], _ collezioniDaVisualizzare:[String] = [], _ conNomiVersioni:Bool = true, _ paroleRicercate:Riferimento = Riferimento(), _ alternare:Bool = false) -> String
    {
        return testoBrano(convertiRiferimento(riferimento), listaVersioni, collezioniDaVisualizzare, conNomiVersioni, paroleRicercate, alternare);
    }
    
    func testoBrano(_ riferimento:Riferimento, _ nomeVersione:String, _ collezioniDaVisualizzare:[String] = [], _ conNomiVersioni:Bool = true, _ paroleRicercate:Riferimento = Riferimento(), _ alternare:Bool = false) -> String
    {
        let versioni:[String] = [nomeVersione]
        return testoBrano(riferimento, versioni, collezioniDaVisualizzare, conNomiVersioni, paroleRicercate, alternare);
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
    /// <returns>Il testo biblico.</returns>
    func testoBrano(_ riferimento:Riferimento, _ listaVersioni:[String], _ collezioniDaVisualizzare:[String], _ conNomiVersioni:Bool = true, _ paroleRicercate:Riferimento = Riferimento(), _ alternare:Bool = false) -> String
    {
        if riferimento.count() == 0 {
            return String(localized: "Il riferimento digitato non è valido.")
        }
        if (alternare)
        {
            var cap0:UInt8; var cap1:UInt8; var vers0:UInt8; var vers1:UInt8;
            var maxCapitoloInTuttiTesti:UInt8; var maxVersettoInTuttiTesti:UInt8;
            
            var testoDelBrano = ""
            
            var libStringa:String; var capStringa:String; var versStringa:String
            let rfVecchio = formato.riferimentoFormato;
            for branoInRiferimento in riferimento.brani
            {
                for lib in branoInRiferimento[0]...branoInRiferimento[3]
                {
                    libStringa = "0" + String(lib);
                    libStringa = libStringa[(libStringa.count - 2)...];
                    
                    if (lib == branoInRiferimento[0]) {
                        cap0 = branoInRiferimento[1];
                    }
                    else {
                        cap0 = 1;
                    }
                    maxCapitoloInTuttiTesti = 0;
                    for versioneDaControllare in listaVersioni {
                        if (info(versioneDaControllare).tipo == TestoTipi.Bibbia && capitoliInLibro(lib, versioneDaControllare) > maxCapitoloInTuttiTesti) {
                            maxCapitoloInTuttiTesti = capitoliInLibro(lib, versioneDaControllare);
                        }
                    }
                    if (maxCapitoloInTuttiTesti == 0) {
                        maxCapitoloInTuttiTesti = capitoliInLibro(lib, UltimaBibbia); // UltimaBibbia forse non è mai impostata, ma non uso "alternare" in queste versioni dell'app
                    }
                    if (lib == branoInRiferimento[3]) {
                        cap1 = branoInRiferimento[4];
                    }
                    else {
                        cap1 = maxCapitoloInTuttiTesti;
                    }
                    if (cap1 > maxCapitoloInTuttiTesti) {
                        cap1 = maxCapitoloInTuttiTesti;
                    }
                    
                    for cap in cap0...cap1
                    {
                        capStringa = "00" + String(cap);
                        capStringa = capStringa[(capStringa.count - 3)...];
                        
                        if (lib == branoInRiferimento[0] && cap == branoInRiferimento[1]) {
                            vers0 = branoInRiferimento[2];
                        }
                        else {
                            vers0 = 1;
                        }
                        maxVersettoInTuttiTesti = 0;
                        for versioneDaControllare in listaVersioni {
                            if (info(versioneDaControllare).tipo == TestoTipi.Bibbia && versettiInCapitolo(lib, cap, versioneDaControllare) > maxVersettoInTuttiTesti) {
                                maxVersettoInTuttiTesti = versettiInCapitolo(lib, cap, versioneDaControllare);
                            }
                        }
                        if (maxVersettoInTuttiTesti == 0) {
                            maxVersettoInTuttiTesti = versettiInCapitolo(lib, cap, UltimaBibbia); // UltimaBibbia forse non è mai impostata, ma non uso "alternare" in queste versioni dell'app
                        }
                        if (lib == branoInRiferimento[3] && cap == branoInRiferimento[4]) {
                            vers1 = branoInRiferimento[5];
                        }
                        else {
                            vers1 = maxVersettoInTuttiTesti;
                        }
                        if (vers1 > maxVersettoInTuttiTesti) {
                            vers1 = maxVersettoInTuttiTesti;
                        }
                        
                        for vers in vers0...vers1
                        {
                            versStringa = "00" + String(vers);
                            versStringa = versStringa[(versStringa.count - 3)...];
                            
                            let riferimentoArray:[UInt8] = [ lib, cap, vers, lib, cap, vers ]
                            
                            testoDelBrano.append(convertiRiferimentoDa3ByteATesto(riferimentoArray, formato.riferimentoFormato))
                            testoDelBrano.append("\r\n");
                            formato.riferimentoFormato = RiferimentoFormato.Nessuno;
                            testoDelBrano.append(testoBrano(Riferimento(riferimentoArray), listaVersioni, collezioniDaVisualizzare, false, paroleRicercate, false))
                            testoDelBrano.append("\r\n");
                            
                            formato.riferimentoFormato = rfVecchio;
                        }
                    }
                }
            }
            
            var testoDaRestituire = testoDelBrano.replacingOccurrences(of:"\r\n ", with:"\r\n");
            while (testoDaRestituire.hasSuffix("\r\n")) {
                testoDaRestituire = testoDaRestituire.remove(testoDaRestituire.count - 2, 2);
            }
            return testoDaRestituire;
        }
        
        // else non alternare
        var noteDaVisualizzare:[Riferimento] = []
        if (listaVersioni.count > 0)
        {
            for collezione in collezioniDaVisualizzare {
                if (versioni[collezione] != nil) {
                    noteDaVisualizzare.append(versioni[collezione]?.elencaNoteInBrano(riferimento) ?? Riferimento());
                }
            }
        }
        
        var brano = "";
        var bibbiaTrovata = false;
        
        if (listaVersioni.count == 0)
        { // non c'è una versione della Bibbia, solo note
            var testoBrano = ""
            
            for i in stride(from:0, to:collezioniDaVisualizzare.count, by:1) {
                var noteInCollezione = Riferimento()
                if (riferimento.versetti) {
                    noteInCollezione = versioni[collezioniDaVisualizzare[i]]?.elencaNoteInBrano(riferimento) ?? Riferimento();
                }
                else {
                    noteInCollezione = riferimento;
                }
                if (noteInCollezione.count() > 0) {
                    if (conNomiVersioni) {
                        testoBrano.append(collezioniDaVisualizzare[i])
                        testoBrano.append("\r\n");
                    }
                    testoBrano.append(versioni[collezioniDaVisualizzare[i]]?.testoBrano(noteInCollezione, [], [], conNomiVersioni, Riferimento()) ?? "");
                    
                    if (i != collezioniDaVisualizzare.count - 1) {
                        testoBrano.append("\r\n");
                    }
                }
            }
            
            brano = testoBrano;
            
        }
        else if (listaVersioni.count == 1 && (((versioni[listaVersioni[0]]?.info.tipo.rawValue ?? 0) & TestoTipi.Bibbia.rawValue) != TestoTipi.Bibbia.rawValue))
        {
            // quando una collezione di note, il testo è già RTF completo
            
            brano = versioni[listaVersioni[0]]?.testoBrano(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, true,  paroleRicercate) ?? "";
            
        }
        else {
            var testoBrano = ""
            
            for i in stride(from:0, to:listaVersioni.count, by:1) {
                if (listaVersioni.count > 1 && conNomiVersioni) {
                    testoBrano.append(listaVersioni[i])
                    testoBrano.append("\r\n\r\n");
                }
                testoBrano.append(versioni[listaVersioni[i]]?.testoBrano(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, conNomiVersioni, paroleRicercate) ?? "");
                
                // una riga vuota dopo ogni nomeVersione (tranne l'ultima)
                if (i < listaVersioni.count - 1) {
                    if (testoBrano.hasSuffix("\r\n")) {
                        testoBrano.append("\r\n");
                    }
                    else {
                        testoBrano.append("\r\n\r\n");
                    }
                }
                
                if (!bibbiaTrovata && versioni[listaVersioni[i]]?.info.tipo == TestoTipi.Bibbia)
                {
                    UltimaBibbia = listaVersioni[i];
                    bibbiaTrovata = true;
                }
            }
            brano = testoBrano;
        }
        
        brano = brano.replacingOccurrences(of:"\r\n ", with:"\r\n");
        while (brano.hasSuffix("\r\n")) {
            brano = brano.remove(brano.count - 2, 2);
        }
        
        return brano;
    }
    
    /*
     
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
     throw TextNotExistException
     }
     
     }
     
     */
    
    //#region Funzioni per il numero di capitoli e versetti
    
    /// <summary>
    /// Il numero di capitoli in un libro in una versione.
    /// </summary>
    /// <param name="libro">Il numero del libro (da 1 a 73).</param>
    /// <param name="nomeVersione">Il nome della versione.</param>
    /// <returns>Il numero di capitoli.</returns>
    /// <exception cref="KeyNotFoundException">Se il nome della versione non esiste.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
    func capitoliInLibro(_ libro:UInt8, _ nomeVersione:String) -> UInt8
    {
        return capitoliInLibro(Int(libro), nomeVersione);
    }
    
    func capitoliInLibro(_ libro:Int, _ nomeVersione:String) -> UInt8
    {
        return versioni[nomeVersione]?.capitoliInLibro[libro] ?? 0;
    }
    
    /// <summary>
    /// Il numero di capitoli in tutti i libri fino ad un certo libro in una versione.
    /// </summary>
    /// <param name="libro">Il numero del libro (da 1 a 73).</param>
    /// <param name="nomeVersione">Il nome della versione.</param>
    /// <returns>Il numero di capitoli.</returns>
    /// <exception cref="KeyNotFoundException">Se il nome della versione non esiste.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
    
    public func capitoliFinoALibro(_ libro:UInt8, _ nomeVersione:String) -> UInt16
    {
        return versioni[nomeVersione]?.indiceLibro[Int(libro)] ?? 0;
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
    func versettiInCapitolo(_ libro:UInt8, _ capitolo:UInt8, _ nomeVersione:String) -> UInt8
    {
        return versettiInCapitolo(Int(libro), Int(capitolo), nomeVersione)
    }
    
    func versettiInCapitolo(_ libro:Int, _ capitolo:Int, _ nomeVersione:String) -> UInt8 {
        let cInL : Int = Int(versioni[nomeVersione]?.capitoliInLibro[libro] ?? 0);
        if (cInL == 0) {
            return 0;
        }
        var capitoloDaUsare = capitolo;
        if (cInL < capitolo) {
            capitoloDaUsare = cInL;
        }
            return versioni[nomeVersione]?.versettiInCapitolo[Int(versioni[nomeVersione]?.indiceLibro[libro - 1] ?? 0) + capitoloDaUsare] ?? 0;
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
    public func versettiFinoACapitolo(_ libro:UInt8, _ capitolo:UInt8, _ nomeVersione:String) -> Int // was UInt32
    {
        let t:Int = Int(versioni[nomeVersione]?.indiceLibro[Int(libro) - 1] ?? 0)
        return Int(versioni[nomeVersione]?.indiceCapitolo[t + Int(capitolo)] ?? 0);
    }
    
    /// <summary>
    /// Il numero di versetti in tutti i capitoli fino ad un certo capitolo nell'ultima versione completa usata.
    /// </summary>
    /// <param name="libro">Il numero del libro (da 1 a 73).</param>
    /// <param name="capitolo">Il numero del capitolo.</param>
    /// <returns>Il numero di versetti.</returns>
    /// <exception cref="KeyNotFoundException">Se non c'è stata un'ultima versione.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Se libro non è da 1 a 73.</exception>
    public func versettiFinoACapitolo(_ libro:UInt8, _ capitolo:UInt8) -> Int
    {
        let t:Int = Int(versioni[ultimaBibbiaCompleta]?.indiceLibro[Int(libro) - 1] ?? 0)
        return Int(versioni[ultimaBibbiaCompleta]?.indiceCapitolo[t + Int(capitolo)] ?? 0)
    }
    
    /// <summary>
    /// Il numero di un libro in cui è un certo capitolo della Bibbia (contando da 1 a circa 1300).
    /// </summary>
    /// <param name="capitolo">Il capitolo da cercare (1-50 in Genesi, 51-90 in Esodo, ecc.).</param>
    /// <param name="nomeVersione">Il nome della versione.</param>
    /// <returns>Il numero del libro.</returns>
    public func libroDiCapitolo(_ capitoloIn:Int, _ nomeVersione:String) -> UInt8
    {
        var capitolo = capitoloIn
        if (capitolo < 1) {
            capitolo = 1;
        }
        var libro:UInt8 = 0;
        repeat {
            libro += 1;
        }
        while (libro <= 73 && capitoliFinoALibro(libro, nomeVersione) < capitolo);
        
        return libro;
    }
    
    /// <summary>
    /// Il riferimento di un versetto, secondo il suo posto nell'ordine della Bibbia (contando da 1 a circa 31000).
    /// </summary>
    /// <param name="versetto">Il versetto da cercare (1-31 in Genesi 1, 32-56 in Genesi 2, ecc.).</param>
    /// <param name="nomeVersione">Il nome della versione.</param>
    /// <returns>Il riferimento del versetto.</returns>
    public func riferimentoDiVersetto(_ versettoIn:Int, _ nomeVersione:String) -> Riferimento
    {
        var versetto = versettoIn
        if (versetto < 1) {
            versetto = 1;
        }
        var libro:UInt8 = 0;
        repeat {
            libro += 1;
        }
        while (libro <= 73 && versettiFinoACapitolo(libro, capitoliInLibro(libro, nomeVersione), nomeVersione) < versetto);
        
        var capitolo:UInt8 = 0;
        repeat {
            capitolo += 1;
        }
        while (versettiFinoACapitolo(libro, capitolo, nomeVersione) < versetto);
        
        return Riferimento(libro, capitolo, UInt8(versetto - versettiFinoACapitolo(libro, capitolo, nomeVersione) + Int(versettiInCapitolo(libro, capitolo, nomeVersione))));
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
    public func riferimentoDiCapitolo(_ capitoloIn:Int, _ nomeVersione:String) -> Riferimento
    {
        var capitolo = capitoloIn
        if (capitolo < 1) {
            capitolo = 1;
        }
        var libro:UInt8 = 0;
        repeat {
            libro += 1;
        }
        while (libro <= 73 && capitoliFinoALibro(libro, nomeVersione) < capitolo);
        let numeroCapitolo = UInt8(capitolo - Int(capitoliFinoALibro(libro, nomeVersione)) + Int(capitoliInLibro(libro, nomeVersione)));
        // capitolo - CapitoliFinoALibro(libro, nomeVersione) + capitoliInLibro(libro, nomeVersione)
        // invece di capitolo - CapitoliFinoALibro(libro-1, nomeVersione)
        // perché libro-1 non è possibile quando libro è di tipo byte
        return Riferimento([ libro, numeroCapitolo, 1, libro, numeroCapitolo, versettiInCapitolo(libro, numeroCapitolo, nomeVersione) ]);
    }
    
    //#region Funzioni per i riferimenti
    
    /// <summary>
    /// Converte un riferimento in una versione allo schema standard di riferimenti del programma.
    /// </summary>
    /// <param name="riferimento">Il riferimento da convertire.</param>
    /// <param name="nomeVersione">Il nome della versione.</param>
    /// <returns>Il riferimento nello schema standard.</returns>
    public func convertiAStandard(_ rif:Riferimento, _ nomeVersione:String) -> Riferimento
    {
        var riferimento = Riferimento(rif);
        for i in stride(from:0, to:riferimento.brani.count, by:1) {
            var branoConvertito = riferimento.brani[i]
            var inizioConvertito = false;
            var fineConvertita = false;
            let riferimentiDiversi = versioni[nomeVersione]?.riferimentiDiversi ?? []
            for rifDiversi in riferimentiDiversi {
                if (!inizioConvertito && branoConvertito[0] == rifDiversi[3] && branoConvertito[1] == rifDiversi[4] && (branoConvertito[2] == rifDiversi[5] || rifDiversi[5] <= 0)) {
                    branoConvertito[0] = UInt8(rifDiversi[0]);
                    branoConvertito[1] = UInt8(rifDiversi[1]);
                    inizioConvertito = true;
                    if (rifDiversi[5] > 0) {
                        branoConvertito[2] = UInt8(rifDiversi[2])
                    }
                    else if (rifDiversi[5] == 0)
                    { // fare la stessa cosa a tutti i versetti nel capitolo: cambiare il capitolo e/o sottrarre un numero da ogni versetto
                        if (rifDiversi[2] < 0) {
                            branoConvertito[2] = branoConvertito[2] + UInt8(rifDiversi[2]);
                        }
                    }
                    else { // <0 ==> bisogna aggiungere il numero di versetti
                        branoConvertito[2] = branoConvertito[2] - UInt8(rifDiversi[5]);
                    }
                }
                if (!fineConvertita && branoConvertito[3] == rifDiversi[3] && branoConvertito[4] == rifDiversi[4] && (branoConvertito[5] == rifDiversi[5] || rifDiversi[5] <= 0)) {
                    branoConvertito[3] = UInt8(rifDiversi[0])
                    branoConvertito[4] = UInt8(rifDiversi[1])
                    fineConvertita = true;
                    if (rifDiversi[5] > 0) {
                        branoConvertito[5] = UInt8(rifDiversi[2])
                    }
                    else {
                        if (branoConvertito[5] != 255) {
                            if (rifDiversi[5] == 0) {
                                if (rifDiversi[2] < 0) {
                                    branoConvertito[5] = branoConvertito[5] + UInt8(rifDiversi[5]);
                                }
                            }
                            else {
                                branoConvertito[5] = branoConvertito[5] - UInt8(rifDiversi[5]);
                            }
                        }
                    }
                }
            }
            riferimento.brani[i] = branoConvertito
        }
        riferimento.daTradurre = true;
        return riferimento;
    }
    
    /// <summary>
    /// Converte un riferimento nello schema standard di riferimenti del programma al riferimento in una versione della Bibbia.
    /// </summary>
    /// <param name="riferimento">Il riferimento da convertire.</param>
    /// <param name="nomeVersione">Il nome della versione.</param>
    /// <returns>Il riferimento nello schema della versione.</returns>
    public func convertiDaStandard(_ rif:Riferimento, _ nomeVersione:String) -> Riferimento
    {
        var riferimento = Riferimento(rif);
        for i in stride(from:0, to:riferimento.brani.count, by:1) {
            var branoConvertito = riferimento.brani[i]
            var inizioConvertito = false;
            var fineConvertita = false;
            
            let riferimentiDiversi = versioni[nomeVersione]?.riferimentiDiversi ?? []
            for rifDiversi in riferimentiDiversi {
                if (!inizioConvertito && branoConvertito[0] == rifDiversi[0] && branoConvertito[1] == rifDiversi[1] && (branoConvertito[2] == rifDiversi[2] || rifDiversi[2] <= 0)) {
                    branoConvertito[0] = UInt8(rifDiversi[3])
                    branoConvertito[1] = UInt8(rifDiversi[4]);
                    inizioConvertito = true;
                    if (rifDiversi[2] > 0) {
                        branoConvertito[2] = UInt8(rifDiversi[5])
                    }
                    else if (rifDiversi[2] == 0) // fare la stessa cosa a tutti i versetti nel capitolo: cambiare il capitolo e/o sottrarre un numero da ogni versetto
                    {
                        if (rifDiversi[5] < 0) {
                            branoConvertito[2] = branoConvertito[2] + UInt8(rifDiversi[5])
                        }
                    }
                    else { // <0 ==> bisogna aggiungere il numero di versetti
                        branoConvertito[2] = branoConvertito[2] - UInt8(rifDiversi[2])
                    }
                }
                if (!fineConvertita && branoConvertito[3] == rifDiversi[0] && branoConvertito[4] == rifDiversi[1] && (branoConvertito[5] == rifDiversi[2] || rifDiversi[2] <= 0)) {
                    branoConvertito[3] = UInt8(rifDiversi[3])
                    branoConvertito[4] = UInt8(rifDiversi[4])
                    fineConvertita = true;
                    if (rifDiversi[2] > 0) {
                        branoConvertito[5] = UInt8(rifDiversi[5])
                    }
                    else {
                        if (branoConvertito[5] != 255) {
                            if (rifDiversi[2] == 0) {
                                if (rifDiversi[5] < 0) {
                                    branoConvertito[5] = branoConvertito[5] + UInt8(rifDiversi[5]);
                                }
                            }
                            else {
                                branoConvertito[5] = branoConvertito[5] - UInt8(rifDiversi[2]);
                            }
                        }
                    }
                }
            }
            riferimento.brani[i] = branoConvertito
        }
        riferimento.daTradurre = false;
        return riferimento;
    }
    
    /// <summary>
    /// Converte un riferimento nel formato "1 28:14; 4 24:17" a "Genesi 28:14; Numeri 24:17".
    /// </summary>
    /// <param name="riferimentoDaConvertire">Il riferimento da convertire</param>
    /// <returns>Il riferimento convertito</returns>
    public func convertiRiferimentoDa3Numeri(_ riferimento:String) -> String
    {
        var riferimentoDaConvertire = riferimento
        var riferimentoConvertito = ""
        if (!riferimentoDaConvertire.isEmpty) {
            riferimentoDaConvertire = ";" + riferimentoDaConvertire + ";";
            riferimentoDaConvertire = riferimentoDaConvertire.replacingOccurrences(of:"; ", with:";");
            riferimentoDaConvertire = riferimentoDaConvertire.remove(0, 1);
            while (!riferimentoDaConvertire.isEmpty) {
                let posizioneSpazio = riferimentoDaConvertire.indexOf(" ");
                let posizionePuntoVirgola = riferimentoDaConvertire.indexOf(";");
                if (posizionePuntoVirgola == -1) {
                    riferimentoDaConvertire = "";
                }
                else {
                    if (posizioneSpazio >= 0) {
                        let nLibro:Int = Int(riferimentoDaConvertire[0..<posizioneSpazio]) ?? -1
                        if nLibro>=0 {
                            riferimentoConvertito += formato.libriNomi[nLibro] + riferimentoDaConvertire[posizioneSpazio..<posizionePuntoVirgola]+"; "
                        }
                    }
                    else // c'è solo il numero del libro
                    {
                        let nLibro:Int = Int(riferimentoDaConvertire[0..<posizionePuntoVirgola]) ?? -1
                        if nLibro>=0 {
                            riferimentoConvertito += formato.libriNomi[nLibro]+"; ";
                        }
                    }
                    riferimentoDaConvertire = riferimentoDaConvertire.remove(0, posizionePuntoVirgola + 1);
                }
            }
        }
        var riferimentoStringa = riferimentoConvertito.trimmingCharacters(in: .whitespacesAndNewlines)
        if (riferimentoStringa.hasSuffix(";")) {
            riferimentoStringa = riferimentoStringa.remove(riferimentoStringa.count - 1, 1);
        }
        return riferimentoStringa;
    }
    
    //#region NormalizzaRiferimento
    
    /// <summary>
    /// Converte un riferimento in formato testuale ad uno più bello. Usa le abbreviazioni dei libri.
    /// </summary>
    /// <param name="riferimento">Il riferimento da convertire.</param>
    /// <returns>Il riferimento convertito.</returns>
    public func normalizzaRiferimento(_ riferimento:String) -> String
    {
        return normalizzaRiferimento(riferimento, RiferimentoFormato.Abbreviazione);
    }
    
    /// <summary>
    /// Converte un riferimento in formato testuale ad uno più bello.
    /// </summary>
    /// <param name="riferimento">Il riferimento da convertire.</param>
    /// <param name="formatoDelRiferimento">Il formato del riferimento da visualizzare.</param>
    /// <returns>Il riferimento convertito.</returns>
    public func normalizzaRiferimento(_ riferimento:String, _ formatoDelRiferimento:RiferimentoFormato) -> String
    {
        // tested
        return normalizzaRiferimento(convertiRiferimento(riferimento), formatoDelRiferimento);
    }
    
    /// <summary>
    /// Converte un riferimento nel formato del programma ad uno più bello.
    /// </summary>
    /// <param name="riferimento">Il riferimento da convertire.</param>
    /// <returns>Il riferimento convertito.</returns>
    public func normalizzaRiferimento(_ riferimento:Riferimento) -> String
    {
        // tested
        return normalizzaRiferimento(riferimento, RiferimentoFormato.Abbreviazione);
    }
    
    /// <summary>
    /// Converte un riferimento nel formato del programma ad uno più bello.
    /// </summary>
    /// <param name="riferimento">Il riferimento da convertire.</param>
    /// <param name="formatoDelRiferimento">Il formato del riferimento da visualizzare.</param>
    /// <returns>Il riferimento convertito.</returns>
    public func normalizzaRiferimento(_ riferimento:Riferimento, _ formatoDelRiferimento:RiferimentoFormato) -> String
    {
        // tested
        var riferimentoNormalizzato = "";
        let separatori = separatoriNeiRiferimenti();
        
        if (formatoDelRiferimento != RiferimentoFormato.Nessuno && riferimento.versetti) { // se è un riferimento con note, restituisce niente
            var riferimentoTestuale = ""
            var sLibroVecchio:UInt8  = 0;
            var sCapitoloVecchio:UInt8  = 0;
            let nRiferimenti = riferimento.count();
            for i in stride(from:0, to:nRiferimenti, by:1) {
                riferimentoTestuale = convertiRiferimentoDa3ByteATesto(riferimento.brani[i], formatoDelRiferimento);
                if (riferimentoTestuale.hasSuffix(":")) { // se RifTipo==RIFTIPO_CITAZIONE
                    riferimentoTestuale = riferimentoTestuale[0..<riferimentoTestuale.count-1];
                }
                if (!riferimentoNormalizzato.isEmpty) {
                    if (riferimento.brani[i][0] == sLibroVecchio && riferimento.brani[i][1] == sCapitoloVecchio && riferimento.brani[i][0] == riferimento.brani[i][3] && riferimento.brani[i][1] == riferimento.brani[i][4])
                    {
                        riferimentoTestuale = riferimentoTestuale[(riferimentoTestuale.indexOf(" ") + 1)...];
                        riferimentoTestuale = riferimentoTestuale[(riferimentoTestuale.indexOf(separatori[1]) + 1)...];
                        riferimentoNormalizzato += separatori[2];
                    }
                    else {
                        riferimentoNormalizzato += "; ";
                        if (riferimento.brani[i][0] == sLibroVecchio && riferimento.brani[i][0] == riferimento.brani[i][3]) {
                            riferimentoTestuale = riferimentoTestuale[(riferimentoTestuale.indexOf(" ") + 1)...];
                        }
                    }
                }
                riferimentoNormalizzato += riferimentoTestuale;
                sLibroVecchio = 0;
                if (riferimento.brani[i][0] == riferimento.brani[i][3]) {
                    sLibroVecchio = riferimento.brani[i][3];
                }
                sCapitoloVecchio = 0;
                if (riferimento.brani[i][0] == riferimento.brani[i][3] && riferimento.brani[i][1] == riferimento.brani[i][4]) {
                    sCapitoloVecchio = riferimento.brani[i][4];
                }
            }
        }
        
        if (formato.riferimentoTipo == RiferimentoTipo.Citazione && !riferimentoNormalizzato.isEmpty) {
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
    public func normalizzaRiferimento(_ libroInizio:UInt8, _ capitoloInizio:UInt8, _ versettoInizio:UInt8, _ libroFine:UInt8, _ capitoloFine:UInt8, _ versettoFine:UInt8) -> String
    {
        return normalizzaRiferimento(Riferimento([ libroInizio, capitoloInizio, versettoInizio, libroFine, capitoloFine, versettoFine ]));
    }
    
    /// <summary>
    /// Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
    /// </summary>
    /// <param name="libro">Il numero del libro.</param>
    /// <param name="capitolo">Il capitolo.</param>
    /// <param name="versetto">Il versetto.</param>
    /// <returns>Il riferimento convertito.</returns>
    public func normalizzaRiferimento(_ libro:UInt8, _ capitolo:UInt8, _ versetto:UInt8) -> String
    {
        return normalizzaRiferimento(Riferimento(libro, capitolo, versetto));
    }
    
    /// <summary>
    /// Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
    /// </summary>
    /// <param name="libro">Il numero del libro.</param>
    /// <param name="capitolo">Il capitolo.</param>
    /// <param name="versetto">Il versetto.</param>
    /// <returns>Il riferimento convertito.</returns>
    public func normalizzaRiferimento(_ libro:Int, _ capitolo:Int, _ versetto:Int) -> String
    {
        return normalizzaRiferimento(Riferimento(libro, capitolo, versetto));
    }
    
    /// <summary>
    /// Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
    /// </summary>
    /// <param name="libro">Il numero del libro, come stringa.</param>
    /// <param name="capitolo">Il capitolo, come stringa.</param>
    /// <param name="versetto">Il versetto, come stringa.</param>
    /// <returns>Il riferimento convertito.</returns>
    public func normalizzaRiferimento(_ libro:String, _ capitolo:String, _ versetto:String) -> String
    {
        let nLibro:UInt8 = UInt8(libro) ?? 255
        let nCapitolo:UInt8 = UInt8(capitolo) ?? 255
        let nVersetto:UInt8 = UInt8(versetto) ?? 255
        if nLibro==255 || nCapitolo==255 || nVersetto==255 {
            return ""
        }
        else {
            return normalizzaRiferimento(nLibro, nCapitolo, nVersetto);
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
    public func normalizzaRiferimento(_ libroInizio:String, _ capitoloInizio:String, _ versettoInizio:String, _ libroFine:String, _ capitoloFine:String, _ versettoFine:String) -> String
    {
        let nLibroInizio:UInt8 = UInt8(libroInizio) ?? 255
        let nCapitoloInizio:UInt8 = UInt8(capitoloInizio) ?? 255
        let nVersettoInizio:UInt8 = UInt8(versettoInizio) ?? 255
        let nLibroFine:UInt8 = UInt8(libroFine) ?? 255
        let nCapitoloFine:UInt8 = UInt8(capitoloFine) ?? 255
        let nVersettoFine:UInt8 = UInt8(versettoFine) ?? 255
        if nLibroInizio==255 || nCapitoloInizio==255 || nVersettoInizio==255 || nLibroFine==255 || nCapitoloFine==255 || nVersettoFine==255 {
            return ""
        }
        else {
            return normalizzaRiferimento(nLibroInizio, nCapitoloInizio, nVersettoInizio, nLibroFine, nCapitoloFine, nVersettoFine);
        }
    }
    
    /*
     
     /// <summary>
     /// Converte un segnalibro ad un formato testuale più bello.
     /// </summary>
     /// <param name="segnalibro">Il riferimento del segnalibro.</param>
     /// <returns>Il riferimento convertito.</returns>
     public string NormalizzaRiferimentoSegnalibro(string segnalibro)
     {
     if (string.IsNullOrEmpty(segnalibro))
     return "";
     var riferimento = ""
     char[] spazio = new char[] { ' ' };
     string[] brani = SplitString(segnalibro, ';');
     foreach (string brano in brani)
     {
     string[] numeri = SplitString(brano, spazio);
     if (numeri.Length >= 6)
     riferimento.Append(NormalizzaRiferimento(numeri[0], numeri[1], numeri[2], numeri[3], numeri[4], numeri[5])).Append(";");
     else if (numeri.Length >= 3)
     riferimento.Append(NormalizzaRiferimento(numeri[0], numeri[1], numeri[2])).Append(";");
     }
     string riferimentoNormalizzato = riferimento.ToString();
     if (riferimentoNormalizzato.EndsWith(";"))
     riferimentoNormalizzato = riferimentoNormalizzato.Remove(riferimentoNormalizzato.Length - 1, 1);
     return riferimentoNormalizzato;
     }
     
     */
    /// <summary>
    /// Converti un riferimento testuale al formato usato dal programma.
    /// </summary>
    /// <param name="riferimento">Il riferimento da convertire.</param>
    /// <returns>Il riferimento nel formato usato dal programma.</returns>
    /// <seealso cref="Riferimento"/>
    func convertiRiferimento(_ riferimentoIn:String) -> Riferimento
    {
        var riferimento = riferimentoIn
        var nuovoRiferimento = Riferimento()
        if riferimento.isEmpty {
            return nuovoRiferimento
        }
        riferimento = riferimento.trimmingCharacters(in: .whitespacesAndNewlines).lowercased();
        if (riferimento.hasPrefix("\\") && riferimento.indexOf(" ") >= 0) {// a volte il link inizia con \f0 ...
            riferimento = riferimento.remove(0, riferimento.indexOf(" ") + 1);
        }
        if (riferimento.isEmpty) {
            return nuovoRiferimento;
        }
        // cancellare eventuali spazi dopo punteggiatura o un numero (per esempio 2 re)
        for i in (1..<riferimento.count).reversed() {
            if (riferimento[i] == " " && (riferimento[i - 1] == ":" || riferimento[i - 1] == "," || riferimento[i - 1] == "." || riferimento[i - 1] == ";" || riferimento[i - 1] == "-" || Character(riferimento[i - 1]).isWholeNumber)) {
                riferimento = riferimento.remove(i, 1);
            }
        }
        // cancellare eventuali punti o virgole dopo il nome di un libro (virgole succede con RIFTIPO_CITAZIONE)
        for i in (1..<riferimento.count).reversed()
        {
            if ((riferimento[i] == ".") && (Character(riferimento[i - 1]).isLetter)) {
                riferimento = riferimento.remove(i, 1);
            }
            else
            {
                if ((riferimento[i] == ",") && (Character(riferimento[i - 1]).isLetter))
                {
                    if (i == riferimento.count - 1 || (Character(riferimento[i + 1]).isWholeNumber) && (i == riferimento.count - 2 || !Character(riferimento[i + 2]).isLetter)) { // non nel caso di mr,gv o mr,3g ma sì nel caso di mr,3,4
                        riferimento = riferimento.remove(i, 1);
                    }
                }
            }
        }
        // cancellare eventuali due punti alla fine o prima di punteggiatura (possibile con RIFTIPO_CITAZIONE)
        for i in (1..<riferimento.count).reversed() {
            if (riferimento[i] == ":" && (i == riferimento.count - 1 || (riferimento[i + 1] == ";" || riferimento[i + 1] == "," || riferimento[i + 1] == "."))) {
                riferimento = riferimento.remove(i, 1);
            }
        }
        
        if ((formato.riferimentoTipo == RiferimentoTipo.Virgola || formato.riferimentoTipo == RiferimentoTipo.Citazione) && (riferimento.indexOf(":") < 0 || riferimento.indexOf(":") >= riferimento.count - 2)) {
            riferimento = riferimento.replacingOccurrences(of:",", with:":");
            riferimento = riferimento.replacingOccurrences(of:".", with:",");
            while (riferimento.indexOf(";") >= 0)
            {
                var dopoDivisore = riferimento.indexOf(";") + 1; // controlla situazioni come Is 7,1-10;12 che viene tradotto in modo diverso
                while (dopoDivisore <= riferimento.count - 1 && ((Character(riferimento[dopoDivisore]).isWholeNumber) || riferimento[dopoDivisore] == " ")) {
                    dopoDivisore += 1;
                }
                if (dopoDivisore > riferimento.count - 1 || (riferimento[dopoDivisore] != ":" && riferimento[dopoDivisore] != "." && (!Character(riferimento[dopoDivisore]).isLetter))) {
                    riferimento = riferimento[1..<(dopoDivisore)] + ":1-200" + riferimento[dopoDivisore...]
                }
                riferimento = riferimento.replacingOccurrences(of:";", with:",");
            }
        }
        
        var punteggiature = 0
        var capitolo = 0
        var trattinoVecchio = true
        var trattino = false
        var versettoMancante = false;
        var riferimentoDaAnalizzare = ""
        var libroNome = ""
        var riferimentoBrano:[UInt8] = [ 0, 0, 0, 0, 0, 0, 0, 0 ]
        var riferimentoBranoPrecedente:[UInt8] = [ 0, 0, 0, 0, 0, 0, 0, 0 ]
        var riferimentoBrano4Byte:[UInt8] = [ 0, 0, 0, 0, 0, 0, 0, 0 ]
        repeat {
            // troviamo il riferimento del primo brano, cioè fino alla prima punteggiatura
            punteggiature = riferimento.indexOf(",");
            if (punteggiature < 0 || (riferimento.indexOf(";") < punteggiature && riferimento.indexOf(";") >= 0)) {
                punteggiature = riferimento.indexOf(";");
            }
            if (punteggiature < 0 || (riferimento.indexOf("-") < punteggiature && riferimento.indexOf("-") >= 0)) {
                punteggiature = riferimento.indexOf("-");
                if (punteggiature >= 0) {
                    trattino = true;
                }
            }
            if (punteggiature >= 0) {
                riferimentoDaAnalizzare = riferimento[0..<punteggiature]; // il riferimento del primo brano
                riferimento = riferimento.remove(0, punteggiature + 1).trimmingCharacters(in: .whitespacesAndNewlines) // il resto del riferimento, che analizzeremo più tardi
            }
            else {
                riferimentoDaAnalizzare = riferimento;
                riferimento = "";
            }
            riferimentoBrano = convertiRiferimentoDaTestoA4Byte(riferimentoDaAnalizzare, trattinoVecchio); // il primo brano, in formatto a 4 byte
            if (riferimentoBrano[0] == 0 && !riferimentoDaAnalizzare.isEmpty && (!Character(riferimentoDaAnalizzare[0]).isLetter))
            {
                if (riferimentoDaAnalizzare.indexOf(":") == -1 && riferimentoDaAnalizzare.indexOf(".") == -1 && !versettoMancante) {
                    riferimentoDaAnalizzare = String(capitolo) + ":" + riferimentoDaAnalizzare;
                }
                riferimentoDaAnalizzare = libroNome + riferimentoDaAnalizzare;
                riferimentoBrano = convertiRiferimentoDaTestoA4Byte(riferimentoDaAnalizzare, trattinoVecchio);
            }
            versettoMancante = false;
            if (riferimentoBrano[0] > 0)
            {
                riferimentoBrano4Byte = riferimentoBrano;
                if (riferimentoDaAnalizzare.indexOf(":") == -1 && riferimentoDaAnalizzare.indexOf(".") == -1)
                {
                    versettoMancante = true;
                    if (trattino)
                    {
                        if (!riferimento.isEmpty) && (!Character(riferimento[0]).isLetter) && (riferimento.count == 1 || (!Character(riferimento[1]).isLetter)) {
                            riferimento = formato.libriAbbreviazioniUsate[Int(riferimentoBrano4Byte[0])] + riferimento;
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
                libroNome = formato.libriAbbreviazioniUsate[Int(riferimentoBrano4Byte[0])];
                capitolo = Int(riferimentoBrano4Byte[1])
            }
            if (!trattinoVecchio) {
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
            else {
                if (trattino) {
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
            if (riferimentoBrano[0] > 0 && riferimentoBrano[4] > 0) {
                nuovoRiferimento.aggiungiBrano8Byte(riferimentoBrano);
            }
        }
        while (!riferimento.isEmpty);
        return nuovoRiferimento;
    }
    
    /// <summary>
    /// Trova tutti i riferimenti in una stringa.
    /// </summary>
    /// <param name="stringaDaAnalizzare">La stringa in cui cercare i riferimenti.</param>
    /// <returns>I riferimenti trovati, nel formato usato dal programma.</returns>
    public func convertiRiferimenti(_ stringaDaAnalizzare:String) -> Riferimento
    {
        var riferimentoTrovato = "";
        if (!stringaDaAnalizzare.isEmpty)
        {
            let numeri = [ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" ];
            var indice = stringaDaAnalizzare.indexOfAny(numeri, 1);
            var primaLetteraDopo=0, primaLetteraPrima=0;
            while (indice > 0)
            {
                primaLetteraDopo = indice + 1;
                while (primaLetteraDopo < stringaDaAnalizzare.count && !Character(stringaDaAnalizzare[primaLetteraDopo]).isLetter) {
                    primaLetteraDopo += 1
                }
                primaLetteraPrima = indice - 1;
                if (Character(stringaDaAnalizzare[primaLetteraPrima]).isWhitespace)
                {
                    while (primaLetteraPrima > 0 && Character(stringaDaAnalizzare[primaLetteraPrima]).isWhitespace) {
                        primaLetteraPrima -= 1
                    }
                    // adesso andiamo all'inizio di questa parola
                    while (primaLetteraPrima > 0 && Character(stringaDaAnalizzare[primaLetteraPrima - 1]).isLetter) {
                        primaLetteraPrima -= 1
                    }
                    // aggiustiamo per 1Giovanni eccetera
                    if (primaLetteraPrima > 0 && (stringaDaAnalizzare[primaLetteraPrima - 1] >= "1" && stringaDaAnalizzare[primaLetteraPrima - 1] <= "3")) {
                        primaLetteraPrima -= 1
                    }
                    if (primaLetteraPrima > 1 && Character(stringaDaAnalizzare[primaLetteraPrima - 1]).isWhitespace && (stringaDaAnalizzare[primaLetteraPrima - 2] >= "1" && stringaDaAnalizzare[primaLetteraPrima - 2] <= "3")) {
                        primaLetteraPrima -= 2;
                    }
                    riferimentoTrovato += stringaDaAnalizzare[primaLetteraPrima..<primaLetteraDopo] + ";";
                }
                indice = (primaLetteraDopo == stringaDaAnalizzare.count ? -1 : stringaDaAnalizzare.indexOfAny(numeri, primaLetteraDopo));
            }
        }
        var riferimento = convertiRiferimento(riferimentoTrovato);
        for i in stride(from:riferimento.count()-1, through:0, by:-1) {
            if (riferimento.brani[i][4] == 255 && riferimento.brani[i][5] == 255) {
                riferimento.rimuovi(i);
            }
        }
        return riferimento;
    }
    
    func convertiTitoloNotaARiferimento(_ notaDaConvertire:String) -> Riferimento
    {
        let note = notaDaConvertire.split(separator:"#").map{String($0)}
        var brano:[UInt8] = [0,0,0,0,0,0]
        var rif = Riferimento()
        
        for nota in note {
            brano[0] = UInt8(nota[0..<2]) ?? 0
            brano[1] = UInt8(nota[2..<5]) ?? 0
            brano[2] = UInt8(nota[5..<8]) ?? 0
            brano[3] = UInt8(nota[13..<15]) ?? 0 // [8...13] is 0000hyphen
            brano[4] = UInt8(nota[15..<18]) ?? 0
            brano[5] = UInt8(nota[18..<21]) ?? 0
            rif.aggiungiBrano(brano)
        }
        
        return rif
    }
    
    /// <summary>
    /// Converte il titolo di una nota che inizia con # ad una stringa con il riferimento in formato leggibile.
    /// </summary>
    /// <param name="notaDaConvertire">Il titolo di una nota.</param>
    /// <returns>Un riferimento come una stringa.</returns>
    func convertiTitoloNotaARiferimentoLeggibile(_ notaDaConvertire:String) -> String
    {
        // vedi anche Riferimento.ComeNota per l'altra direzione
        if (notaDaConvertire.isEmpty) {
            return "";
        }
        
        let separatori = separatoriNeiRiferimenti();
        var riferimento = ""
        
        let note = notaDaConvertire.split(separator:"#").map{String($0)}
        for nota in note {
            if (!riferimento.isEmpty) {
                riferimento.append(";");
            }
            // nota non ha # all'inizio qui
            let libro1:UInt8 = UInt8(nota[0..<2]) ?? 0;
            riferimento.append(formato.libriAbbreviazioniUsate[Int(libro1)]);
            let capitolo1:Int32 = Int32(nota[2..<5]) ?? 0;
            let versetto1:Int32 = Int32(nota[5..<8]) ?? 0;
            let numeroParola1:Int32 = Int32(nota[8..<12]) ?? 0;
            let capitoliInLibro1 = capitoliInLibro(libro1, ultimaBibbiaCompleta);
            if (capitolo1 > 0)
            {
                riferimento.append(separatori[0]);
                if (capitoliInLibro1 != 1) {
                    riferimento.append(String(capitolo1));
                }
                if (versetto1 > 0)
                {
                    if (capitoliInLibro1 != 1) {
                        riferimento.append(separatori[1]);
                    }
                    riferimento.append(String(versetto1));
                    if (numeroParola1 > 0) {
                        riferimento.append("/"+String(numeroParola1));
                    }
                }
            }
            
            if (nota[0..<12] != nota[13..<25])
            {
                riferimento.append("-");
                let libro2:UInt8 = UInt8(nota[13..<15]) ?? 0;
                let capitolo2:Int32 = Int32(nota[15..<18]) ?? 0;
                let versetto2:Int32 = Int32(nota[18..<21]) ?? 0;
                let numeroParola2:Int32 = Int32(nota[21..<25]) ?? 0;
                let capitoliInLibro2 = capitoliInLibro(libro2, ultimaBibbiaCompleta);
                if (libro2 != libro1)
                {
                    riferimento.append(formato.libriAbbreviazioniUsate[Int(libro2)]);
                    if (capitolo2 > 0)
                    {
                        riferimento.append(separatori[0]);
                        if (capitoliInLibro2 != 1) {
                            riferimento.append(String(capitolo2));
                        }
                        if (versetto2 > 0)
                        {
                            if (capitoliInLibro1 != 1) {
                                riferimento.append(separatori[1]);
                            }
                            riferimento.append(String(versetto2));
                            if (numeroParola2 > 0) {
                                riferimento.append("/"+String(numeroParola2));
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
                            riferimento.append(String(capitolo2));
                            if (versetto2 > 0)
                            {
                                riferimento.append(separatori[1]+String(versetto2));
                                if (numeroParola2 > 0) {
                                    riferimento.append("/"+String(numeroParola2));
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
                                riferimento.append(String(versetto2));
                                if (numeroParola2 > 0) {
                                    riferimento.append("/"+String(numeroParola2));
                                }
                            }
                        }
                    }
                }
            }
            
        }
        return riferimento;
    }
    
    func convertiRiferimentoDaTestoA4Byte(_ riferimentoTestuale:String, _ primaDelTrattino:Bool) -> [UInt8]
    {
        // convertire a 4 interi un riferimento di un versetto+parola
        // se primaDelTrattino = false, il riferimento va dopo il trattino
        var riferimentoRestituito:[UInt8] = [ 0, 0, 0, 0, 0, 0, 0, 0 ]
        var primaNonLettera = -1;
        var riferimento = riferimentoTestuale.lowercased().trimmingCharacters(in: .whitespacesAndNewlines)
        if (riferimento.isEmpty) {
            return riferimentoRestituito;
        }
        
        var nomeLibro = "";
        if (riferimento[0] >= "1" && riferimento[0] <= "3")
        {
            nomeLibro = riferimento[0];
            riferimento = riferimento.remove(0, 1).trimmingCharacters(in: .whitespacesAndNewlines)
        }
        
        let riferimentocount = riferimento.count
        repeat {
            primaNonLettera += 1
        } while (primaNonLettera < riferimentocount - 1 && Character(riferimento[primaNonLettera]).isLetter);
        
        var riferimentoRimanente = "";
        var capitolo:UInt8 = 0; var versetto:UInt8 = 0; var parola:UInt8 = 0;
        if (primaNonLettera == riferimentocount - 1 && Character(riferimento[riferimentocount - 1]).isLetter) {
            nomeLibro += riferimento;
        }
        else {
            nomeLibro += riferimento[0..<primaNonLettera]
            riferimentoRimanente = riferimento[primaNonLettera...].trimmingCharacters(in: .whitespacesAndNewlines)
            var capitoloNumerico = ""
            for j in stride(from:0, to:riferimentoRimanente.count, by:1) {
                if (Character(riferimentoRimanente[j]).isWholeNumber) {
                    capitoloNumerico.append(riferimentoRimanente[j]);
                }
                else {
                    break
                }
            }
            
            capitolo = UInt8(capitoloNumerico) ?? 0
        }
        
        if (riferimentoRimanente != "") {
            var posDivisoreCapitoloVersetto = riferimentoRimanente.indexOf(":");
            if (posDivisoreCapitoloVersetto == -1 || (riferimentoRimanente.indexOf(".") < posDivisoreCapitoloVersetto && riferimentoRimanente.indexOf(".") >= 0)) {
                posDivisoreCapitoloVersetto = riferimentoRimanente.indexOf(".");
            }
            if ((formato.riferimentoTipo == RiferimentoTipo.Virgola || formato.riferimentoTipo == RiferimentoTipo.Citazione) && (posDivisoreCapitoloVersetto == -1 || (riferimentoRimanente.indexOf(",") < posDivisoreCapitoloVersetto && riferimentoRimanente.indexOf(",") >= 0))) {
                posDivisoreCapitoloVersetto = riferimentoRimanente.indexOf(",");
            }
            if (posDivisoreCapitoloVersetto >= 0) {
                riferimentoRimanente = riferimentoRimanente.remove(0, posDivisoreCapitoloVersetto + 1).trimmingCharacters(in: .whitespacesAndNewlines)
            }
            else {
                riferimentoRimanente = "";
            }
            var versettoNumerico = ""
            for j in stride(from:0, to:riferimentoRimanente.count, by:1) {
                if Character(riferimentoRimanente[j]).isWholeNumber {
                    versettoNumerico.append(riferimentoRimanente[j])
                }
                else {
                    break
                }
            }
            versetto = UInt8(versettoNumerico) ?? 0
        }
        
        // trovare eventuale parola dopo /
        if riferimentoRimanente != ""
        {
            let posDivisoreVersettoParola = riferimentoRimanente.indexOf("/");
            if (posDivisoreVersettoParola >= 0) {
                riferimentoRimanente = riferimentoRimanente.remove(0, posDivisoreVersettoParola + 1).trimmingCharacters(in: .whitespacesAndNewlines)
            }
            else {
                riferimentoRimanente = "";
            }
            var parolaNumerico = ""
            for j in stride(from:0, to:riferimentoRimanente.count, by:1) {
                if Character(riferimentoRimanente[j]).isWholeNumber {
                    parolaNumerico.append(riferimentoRimanente[j]);
                }
                else {
                    break
                }
            }
            parola = UInt8(parolaNumerico) ?? 0
        }
        
        let libro = getLibroNumeroDaAbbreviazione(nomeLibro);
        
        if (libro > 0) {
            riferimentoRestituito[0] = libro;
            if ((libro == 38 || libro == 64 || libro == 70 || libro == 71 || libro == 72) && versetto == 0) {
                versetto = capitolo;
                capitolo = 1;
            }
            if (capitolo == 0) {
                if (primaDelTrattino) {
                    riferimentoRestituito[1] = 1;
                    riferimentoRestituito[2] = 1;
                }
                else {
                    riferimentoRestituito[1] = 255;
                    riferimentoRestituito[2] = 255;
                }
            } // if (iCapitolo==0)
            else {
                riferimentoRestituito[1] = capitolo;
                if (versetto == 0) {
                    if (primaDelTrattino) {
                        versetto = 1;
                    }
                    else {
                        versetto = 255;
                    }
                }
                riferimentoRestituito[2] = versetto;
            }
        } // if (!string.IsNullOrEmpty(rifOut))
        
        riferimentoRestituito[3] = parola;
        
        return riferimentoRestituito;
    }
    
    func convertiRiferimentoDa3ByteATesto(_ rif:[UInt8], _ rf:RiferimentoFormato) -> String
    {
        if (rf == RiferimentoFormato.Nessuno) {
            return ""
        }
        
        var riferimentoTestuale = "";
        let libro1 = rif[0];
        let capitolo1 = rif[1];
        let versetto1 = rif[2];
        let libro2 = rif[3];
        let capitolo2 = rif[4];
        let versetto2 = rif[5];
        
        let dopoLibro = (formato.riferimentoTipo == RiferimentoTipo.Citazione ? "., " : " ");
        if (rf == RiferimentoFormato.Intero) {
            riferimentoTestuale = formato.libriNomi[Int(libro1)] + dopoLibro
        }
        else {
            if (rf == RiferimentoFormato.Abbreviazione) {
                riferimentoTestuale = formato.libriAbbreviazioniUsate[Int(libro1)] + dopoLibro
            }
            else {
                if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta) {
                    riferimentoTestuale = libriAbbreviazioniRiconosciute.abbreviazione(libro1);
                    if (riferimentoTestuale.indexOf(",") < 0) {
                        riferimentoTestuale += dopoLibro;
                    }
                    else {
                        riferimentoTestuale = riferimentoTestuale[0..<riferimentoTestuale.indexOf(",")] + dopoLibro
                    }
                }
            }
        }
        
        let separatori = separatoriNeiRiferimenti();
        var rifSB = riferimentoTestuale
        
        if (capitolo1 == 1 && capitolo2 == 255) {
            if (libro1 == libro2) { // Gv
                //rifSB += "";
            }
            else { // Gv-At
                rifSB.append("-");
                if (rf == RiferimentoFormato.Intero) {
                    rifSB.append(formato.libriNomi[Int(libro2)]);
                }
                else {
                    if (rf == RiferimentoFormato.Abbreviazione) {
                        rifSB.append(formato.libriAbbreviazioniUsate[Int(libro2)]);
                    }
                    else {
                        if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta) {
                            let s = libriAbbreviazioniRiconosciute.abbreviazione(libro2);
                            rifSB.append(s[0..<s.indexOf(",")]);
                        }
                    }
                }
            }
        }
        else {
            if ((versetto1 == 1 && versetto2 == 255) || (versetto1 == 0 && versetto2 == 0)) {
                if (libro1 == 38 || libro1 == 64 || libro1 == 70 || libro1 == 71 || libro1 == 72) {
                    //rifSB += "";
                }
                else {
                    rifSB.append(String(capitolo1));
                }
                
                if (libro1 == libro2) {
                    if (capitolo1 == capitolo2) { // Gv 4
                        //rifSB += "";
                    }
                    else {
                        rifSB.append("-")
                        rifSB.append(String(capitolo2));
                    }// Gv 4-5
                }
                else { // Gv 4-At 3
                    rifSB.append("-");
                    if (rf == RiferimentoFormato.Intero) {
                        rifSB.append(formato.libriNomi[Int(libro2)])
                        rifSB.append(dopoLibro);
                    }
                    else {
                        if (rf == RiferimentoFormato.Abbreviazione) {
                            rifSB.append(formato.libriAbbreviazioniUsate[Int(libro2)])
                            rifSB.append(dopoLibro);
                        }
                        else {
                            if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta) {
                                let s = libriAbbreviazioniRiconosciute.abbreviazione(libro2);
                                rifSB.append(s[0..<s.indexOf(",")])
                                rifSB.append(dopoLibro);
                            }
                        }
                    }
                    if (libro2 == 38 || libro2 == 64 || libro2 == 70 || libro2 == 71 || libro2 == 72) {
                        //rifSB += "";
                    }
                    else {
                        rifSB.append(String(capitolo2));
                    }
                }
            }
            else {
                if (libro1 == 38 || libro1 == 64 || libro1 == 70 || libro1 == 71 || libro1 == 72) {
                    rifSB.append(String(versetto1));
                }
                else {
                    rifSB.append(String(capitolo1))
                    rifSB.append(separatori[1])
                    rifSB.append(String(versetto1));
                }
                
                if (libro1 == libro2) {
                    if (capitolo1 == capitolo2) {
                        if (versetto1 != versetto2) {
                            rifSB.append("-")
                            rifSB.append(String(versetto2));
                        }
                    }
                    else {
                        rifSB.append("-")
                        rifSB.append(String(capitolo2))
                        rifSB.append(separatori[1])
                        rifSB.append(String(versetto2))
                    }
                }
                else {
                    rifSB.append("-");
                    if (rf == RiferimentoFormato.Intero) {
                        rifSB.append(formato.libriNomi[Int(libro2)])
                        rifSB.append(dopoLibro);
                    }
                    else {
                        if (rf == RiferimentoFormato.Abbreviazione) {
                            rifSB.append(formato.libriAbbreviazioniUsate[Int(libro2)])
                            rifSB.append(dopoLibro);
                        }
                        else {
                            if (rf == RiferimentoFormato.AbbreviazioneRiconosciuta) {
                                let s = libriAbbreviazioniRiconosciute.abbreviazione(libro2);
                                rifSB.append(s[0..<s.indexOf(",")])
                                rifSB.append(dopoLibro);
                            }
                        }
                    }
                    if (libro2 == 38 || libro2 == 64 || libro2 == 70 || libro2 == 71 || libro2 == 72) {
                        rifSB.append(String(versetto2))
                    }
                    else {
                        rifSB.append(String(capitolo2))
                        rifSB.append(separatori[1])
                        rifSB.append(String(versetto2));
                    }
                }
            }
        }
        
        if (formato.riferimentoTipo == RiferimentoTipo.Citazione) {
            rifSB.append(":");
        }
        
        return rifSB.trimmingCharacters(in: .whitespacesAndNewlines).replacingOccurrences(of:" -", with:"-");
    }
    
    /// <summary>
    /// I caratteri da mettere nei riferimenti, secondo le opzioni.
    /// </summary>
    /// <returns>Un array di tre stringhe: la prima è fra il libro e il capitolo, la seconda fra il capitolo e il versetto, la terza fra due versetti.</returns>
    func separatoriNeiRiferimenti() -> [String]
    {
        var separatori:[String] = ["","",""]
        switch (formato.riferimentoTipo)
        {
        case RiferimentoTipo.Virgola:
            separatori[0] = " ";
            separatori[1] = ",";
            separatori[2] = ".";
            break;
        case RiferimentoTipo.Citazione:
            separatori[0] = ((formato.riferimentoFormato == RiferimentoFormato.Abbreviazione) ? "., " : ", ");
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
    /*
     
     
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
     throw TextNotExistException
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
     throw TextNotExistException
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
     throw TextNotExistException
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
     return parola;
     else
     {
     try
     {
     return versioni[nomeVersione].RadiceDiParola(parola);
     }
     catch (KeyNotFoundException)
     {
     throw TextNotExistException
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
     throw TextNotExistException
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
     throw TextNotExistException
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
     throw TextNotExistException
     }
     }
     
     
     
     /*
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
      throw TextNotExistException
      }
      }
      */
     
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
     throw TextNotExistException
     }
     }
     
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
     throw TextNotExistException
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
     
     return versioni[nomeVersione].GetNotaTesto(titolo);
     
     }
     catch (KeyNotFoundException)
     {
     throw TextNotExistException
     }
     }
     
     
     /*
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
      RichTextBoxEx rtb = new RichTextBoxEx();
      string testo, versione;
      Collection<string> collezioniDaVisualizzare = NomiVersioni(TestoTipi.Dizionario);
      for (int i = 0; i < collezioniDaVisualizzare.Count; ++i)
      {
      versione = collezioniDaVisualizzare[i];
      testo = GetNotaTesto(titolo, versione);
      if (string.IsNullOrEmpty(testo) && !string.IsNullOrEmpty(radice))
      testo = GetNotaTesto(radice, versione);
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
      if (testo.EndsWith("\\par\r\n}\r\n"))
      testo = testo.Remove(testo.count - 9) + "\r\n}\r\n";
      rtb.Dispose();
      return testo;
      }
      */
     
     
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
     /// Restituisce un elenco di tutte le note.
     /// </summary>
     /// <param name="nomeVersione">Il nome della versione.</param>
     /// <returns>Una collezione con i nomi di tutte le note.</returns>
     public Collection<string> Note(string nomeVersione)
     {
     try
     {
     List<string> note = new List<string>(versioni[nomeVersione].NoteTitoli.Count);
     note.AddRange(versioni[nomeVersione].NoteTitoli);
     return new Collection<string>(note);
     }
     catch (KeyNotFoundException)
     {
     throw TextNotExistException
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
     Collection<string> note = new Collection<string>();
     int numeroNote = versioni[nomeVersione].NoteTitoli.Count;
     for (int i = 0; i < numeroNote; ++i)
     if (!versioni[nomeVersione].NoteTitoli[i].StartsWith("#"))
     note.Add(versioni[nomeVersione].NoteTitoli[i]);
     return note;
     }
     catch (KeyNotFoundException)
     {
     throw TextNotExistException
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
     Collection<string> titoli = new Collection<string>();
     if (!String.IsNullOrEmpty(nomeVersione))
     {
     Collection<string> noteInOrdine = GetNoteInOrdine(nomeVersione);
     List<string> note = new List<string>(Note(nomeVersione));
     ConfrontoCI confronto = new ConfrontoCI();
     note.Sort(confronto);
     
     // aggiungere prima le note in ordine, poi le altre note in ordine alfabetico
     int indiceNota;
     string notaSenzaTab;
     char[] trimTab = { '\t' };
     
     foreach (string nota in noteInOrdine)
     {
     if (!string.IsNullOrEmpty(nota))
     {
     notaSenzaTab = nota.TrimStart(trimTab); // possono essere note dalle note in ordine, ma senza l'indentazione (indicata da una tabulazione) rimossa
     if (!string.IsNullOrEmpty(notaSenzaTab) && (conNoteSuBrani || !notaSenzaTab.StartsWith("#")))
     {
     titoli.Add(notaSenzaTab);
     indiceNota = note.BinarySearch(notaSenzaTab, confronto);
     if (indiceNota > -1)
     note.RemoveAt(indiceNota);
     }
     }
     }
     
     foreach (string nota in note)
     {
     if (!string.IsNullOrEmpty(nota) && (conNoteSuBrani || !nota.StartsWith("#")))
     titoli.Add(nota);
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
     List<string> note = new List<string>();
     note.AddRange(versioni[nomeVersione].noteInOrdine);
     return new Collection<string>(note);
     }
     catch (KeyNotFoundException)
     {
     throw TextNotExistException
     }
     }
     
     /*
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
      throw TextNotExistException
      }
      }
      */
     
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
     ordine[i] = ordine[i].TrimStart();
     if (ordine.Count < 2)
     return new string[] { "", "" };
     ordine.RemoveAt(0);
     int indice = ordine.indexOf(titolo);
     
     int indicePrecedente = indice - 1;
     while (indicePrecedente >= 0)
     {
     if (!string.IsNullOrEmpty(GetNotaTesto(ordine[indicePrecedente], nomeVersione)))
     break;
     --indicePrecedente;
     }
     
     int indiceSuccessivo = (indice >= 0 ? indice + 1 : ordine.Count);
     while (indiceSuccessivo < ordine.Count)
     {
     if (!string.IsNullOrEmpty(GetNotaTesto(ordine[indiceSuccessivo], nomeVersione)))
     break;
     indiceSuccessivo += 1
     }
     return new string[] { ((indicePrecedente >= 0) ? ordine[indicePrecedente] : ""), ((indiceSuccessivo < ordine.Count) ? ordine[indiceSuccessivo] : "") };
     }
     
     /// <summary>
     /// Scrive una collezione di note ad un file del programma.
     /// </summary>
     /// <param name="bw">Un binary writer dove i dati saranno scritti.</param>
     /// <param name="posizioneInizioDati">La posizione nel file in cui i dati iniziano.</param>
     /// <param name="noteTitolo">I titoli delle note</param>
     /// <param name="noteTesto">I testi delle note (formato RTF o testo normale)</param>
     /// <returns>Due interi senza segno, con la posizione dell'inizio dei titoli e la posizione dell'inizio dell'indice delle note, sempre relativo a pInizioDati</returns>
     [CLSCompliant(false)]
     public static UInt32[] ScriviNote(BinaryWriter bw, UInt32 posizioneInizioDati, string[] noteTitolo, string[] noteTesto)
     {
     if (bw == null)
     throw new ArgumentNullException("bw");
     if (noteTitolo == null)
     throw new ArgumentNullException("noteTitolo");
     if (noteTesto == null)
     throw new ArgumentNullException("noteTesto");
     
     UInt32[] indici = new UInt32[2];
     
     int numeroNote = noteTitolo.Length;
     var titoliNote = ""
     UInt32[] posizioniNote = new UInt32[numeroNote];
     for (int i = 0; i < numeroNote; ++i)
     {
     if (!string.IsNullOrEmpty(noteTitolo[i]))
     {
     titoliNote.Append(noteTitolo[i]).Append("|");
     posizioniNote[i] = (UInt32)(bw.Seek(0, SeekOrigin.Current));
     bw.Write(noteTesto[i]);
     }
     }
     
     indici[0] = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - posizioneInizioDati; // diventa inizioTestoIndiceLC
     bw.Write(titoliNote.ToString());
     indici[1] = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - posizioneInizioDati; // diventa inizioTestoIndice
     for (int i = 0; i < numeroNote; ++i)
     if (!string.IsNullOrEmpty(noteTitolo[i]))
     bw.Write(posizioniNote[i] - posizioniNote[0]);
     
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
     List<Riferimento> riferimenti = new List<Riferimento>();
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
     
     /*
      /// <summary>
      /// Analizza un testo per aggiungere le sue parole ad una corcordanza.
      /// </summary>
      /// <param name="testo">Il testo da analizzare.</param>
      /// <param name="numeroVoce">Il numero del testo nella versione della Bibbia o nella collezione di note.</param>
      /// <param name="chiave">La chiave a cui aggiungere le parole.</param>
      /// <param name="lingua">Le lingue (separate da una riga verticale |) delle parole (necessaria per decidere la fine di una parola con apostrofe).</param>
      [CLSCompliant(false)]
      public static SortedDictionary<string, List<OccorrenzaParola>> TrovaParoleInVoce(string testo, UInt32 numeroVoce, SortedDictionary<string, List<OccorrenzaParola>> chiave, string lingua)
      {
      string[] lingue = lingua.ToLower(CultureInfo.InvariantCulture).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      
      while (testo.indexOf(InizioLink.ToString()) >= 0)
      testo = testo.Remove(testo.indexOf(InizioLink.ToString()), 1);
      int invisibileInizio = testo.indexOf(FineLink1.ToString());
      while (invisibileInizio >= 1)
      {
      int invisibileFine = testo.indexOf(FineLink2.ToString(), invisibileInizio);
      if (invisibileFine >= 0)
      {
      testo = testo.Remove(invisibileInizio, invisibileFine - invisibileInizio + 1);
      invisibileInizio = testo.indexOf(FineLink1.ToString());
      }
      else
      invisibileInizio = -1; // problema con i link; basta uscire e non analizzarli più
      }
      testo = testo.replacingOccurrences(of:"’", with:"'");
      testo = testo.replacingOccurrences(of:"\\rquote ", with:"'");
      
      OccorrenzaParola vp = new OccorrenzaParola();
      vp.Voce = numeroVoce;
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
      parola += c;
      else if (Char.IsPunctuation(c) || Char.IsWhiteSpace(c) || Char.IsSymbol(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.Format || c <= FineLinkFile || c == '' || c == '' || c == '' || c == '') // ASCII 144, 145 e 151, 154
      {
      analizzaParola = true;
      if (c == '\'' || c == '' || c == '') // ACII 145 e 146
      {
      // in un dizionario greco-altra lingua, dobbiamo scegliere la lingua giusta
      linguaDaUsare = linguaPrincipale;
      if (dizionarioGreco && i > 0 && !IsLetteraGreca(testo[i - 1]))
      linguaDaUsare = lingue[1];
      else if (dizionarioEbraico && i > 0 && !IsLetteraEbraica(testo[i - 1]))
      linguaDaUsare = lingue[1];
      if (linguaDaUsare.Length > 2)
      linguaDaUsare = linguaDaUsare.Substring(0, 2);
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
      && Character(testo[i + 1]).isLetter
      && (i == testo.Length - 2 || !IsLetteraONumero(testo[i + 2]))))
      {
      parola += c;
      analizzaParola = false;
      }
      else if (dizionarioEbraico && i < nCaratteri - 1 && (Character(testo[i - 1]).isLetter && testo[i + 1] == '-'))
      { // per il dizionario Strong's Hebrew, che ha pronunce come eh'-sheth
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
      && (testo.Substring(i + 1, 2) == "en" || testo.Substring(i + 1, 2) == "er" || testo.Substring(i + 1, 2) == "ll" || testo.Substring(i + 1, 2) == "lt" || testo.Substring(i + 1, 2) == "ry" || testo.Substring(i + 1, 2) == "st"))
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
      if ((IsLetteraONumero(testo[i - 1]) && (IsLetteraONumero(testo[i + 1]) || testo[i + 1] == '\'' || testo[i + 1] == '«' || testo[i + 1] == '“')) || (Array.BinarySearch(paroleItalianeConApostrofe, parola, confrontoParole) >= 0))
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
      parola += c;
      else if (i < nCaratteri - 1 && Character(testo[i - 1]).isLetter && Character(testo[i + 1]).isLetter)
      {
      parola += c;
      analizzaParola = false;
      }
      else if (Character(testo[i - 1]).isLetter && (i == nCaratteri - 1 || (i < nCaratteri - 1 && !Character(testo[i + 1]).isLetter)))
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
      if (IsLettera(testo[i - 1]) && IsLettera(testo[i + 1])  // per esempio Eben-Ezer
      || (dizionarioEbraico && testo[i - 1] == '\'' && Character(testo[i + 1]).isLetter)) // per esempio eh'-sheth in Strong's Hebrew
      {
      parola += '-'; // ASCII 45
      analizzaParola = false;
      }
      }
      if (!String.IsNullOrEmpty(parola) && analizzaParola)
      {
      ++nParola;
      vp.Parola = nParola;
      parola = parola.ToLower(CultureInfo.InvariantCulture);
      if (!chiave.ContainsKey(parola))
      chiave.Add(parola, new List<OccorrenzaParola>());
      chiave[parola].Add(vp);
      parola = "";
      }
      }
      else
      throw new CarattereSconosciutoException("Carattere sconosciuto in " + testo);
      }
      
      if (!String.IsNullOrEmpty(parola))
      {
      ++nParola;
      vp.Parola = nParola;
      parola = parola.ToLower(CultureInfo.InvariantCulture);
      if (!chiave.ContainsKey(parola))
      chiave.Add(parola, new List<OccorrenzaParola>());
      chiave[parola].Add(vp);
      parola = "";
      }
      
      // il processo può essere lungo (per tutte le parole) per una collezione grande
      System.Windows.Forms.Application.DoEvents();
      return chiave;
      }
      */
     
     /// <summary>
     /// Trova tutte le citazioni a riferimenti in una nota.
     /// </summary>
     /// <param name="testo">Il testo della nota da analizzare.</param>
     /// <returns>Una lista con tutti i brani.</returns>
     private List<Riferimento> TrovaRiferimentiInVoce(string testo)
     {
     List<Riferimento> riferimenti = new List<Riferimento>();
     Riferimento riferimentoLink = new Riferimento();
     
     int posizione = testo.indexOf(InizioLink.ToString());
     int posizioneLink;
     while (posizione >= 0)
     {
     try
     {
     posizioneLink = testo.indexOf(FineLink1.ToString(), posizione);
     if (testo[posizioneLink + 1] == FineLinkBrano)
     {
     riferimentoLink = ConvertiRiferimento(convertiTitoloNotaARiferimento(testo.Substring(posizioneLink + 2, testo.indexOf(FineLink2.ToString(), posizioneLink) - posizioneLink - 2)));
     for (int i = 0; i < riferimentoLink.Count; ++i)
     riferimenti.Add(new Riferimento(riferimentoLink.Brani[i]));
     }
     }
     catch
     {
     // errore nel formato del link; lo saltiamo
     }
     posizione = testo.indexOf(InizioLink.ToString(), posizione + 1);
     }
     
     // quando un file RTF con riferimento è salvato, i caratteri per indicare i riferimenti
     // vengono convertiti, quindi dobbiamo cercare anche loro
     string inizioLink = @"\'0" + ((int)InizioLink).ToString();
     string fineLink1 = @"\'0" + ((int)FineLink1).ToString();
     string fineLink2 = @"\'0" + ((int)FineLink2).ToString();
     string fineLinkBrano = @"\'0" + ((int)FineLinkBrano).ToString();
     posizione = testo.indexOf(inizioLink);
     while (posizione >= 0)
     {
     try
     {
     posizioneLink = testo.indexOf(fineLink1, posizione);
     if (testo.Substring(posizioneLink + 4, 4) == fineLinkBrano)
     {
     riferimentoLink = ConvertiRiferimento(convertiTitoloNotaARiferimento(testo.Substring(posizioneLink + 8, testo.indexOf(fineLink2, posizioneLink) - posizioneLink - 8)));
     for (int i = 0; i < riferimentoLink.Count; ++i)
     riferimenti.Add(new Riferimento(riferimentoLink.Brani[i]));
     }
     }
     catch
     {
     // errore nel formato del link; lo saltiamo
     }
     posizione = testo.indexOf(inizioLink, posizione + 1);
     }
     
     return riferimenti;
     }
     
     
     /*
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
      throw TextNotExistException
      }
      }
      */
     
     /*
      /// <summary>
      /// Indica se almeno una collezione di note è stata modificata.
      /// </summary>
      /// <returns>Se una collezione di note è stata modificata.</returns>
      public bool NoteModificate()
      {
      foreach (Versione versione in versioni.Values)
      {
      if (versione.NoteModificate)
      return true;
      }
      return false;
      }
      */
     
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
     throw TextNotExistException
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
     throw TextNotExistException
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
     throw TextNotExistException
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
     return new Collection<string>();
     }
     catch (ArgumentNullException)
     {
     return new Collection<string>();
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
     throw TextNotExistException
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
     throw TextNotExistException
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
     Collection<string> listaRiferimentiDiversi = new Collection<string>();
     foreach (Int16[] riferimentoDiverso in versioni[nomeVersione].riferimentiDiversi)
     listaRiferimentiDiversi.Add(new StringBuilder().Append(riferimentoDiverso[0]).Append("|").Append(riferimentoDiverso[1]).Append("|").Append(riferimentoDiverso[2]).Append("|").Append(riferimentoDiverso[3]).Append("|").Append(riferimentoDiverso[4]).Append("|").Append(riferimentoDiverso[5]).ToString());
     //            listaRiferimentiDiversi.Add(riferimentoDiverso[0] + "|" + riferimentoDiverso[1] + "|" + riferimentoDiverso[2] + "|" + riferimentoDiverso[3] + "|" + riferimentoDiverso[4] + "|" + riferimentoDiverso[5]);
     return listaRiferimentiDiversi;
     }
     catch (KeyNotFoundException)
     {
     throw TextNotExistException
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
     throw TextNotExistException
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
     Collection<string> listaParoleRadici = new Collection<string>();
     foreach (string parola in versioni[nomeVersione].Parole)
     listaParoleRadici.Add(parola + "=" + versioni[nomeVersione].RadiceDiParola(parola));
     return listaParoleRadici;
     }
     catch (KeyNotFoundException)
     {
     throw TextNotExistException
     }
     }
     
     #region Funzioni generali
     
     private static string[] SplitString(string stringa, char divisore)
     {
     return SplitString(stringa, new char[] { divisore });
     }
     
     private static string[] SplitString(string stringa, char[] divisore)
     {
     
     return stringa.Split(divisore, StringSplitOptions.RemoveEmptyEntries);
     
     }
     
     private static bool IsLettera(char c)
     { // anche in funzioni.cs
     return (Character(c).isLetter || Char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark || (c >= '\u02be' && c <= '\u02bf')); // gli ultimi caratteri sono usati nella traslitterazione dell'ebraico
     }
     
     private static bool IsLetteraONumero(char c)
     {
     return (Char.IsLetterOrDigit(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.OtherNumber || Char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark || (c >= '\u02be' && c <= '\u02bf'));
     }
     
     private static bool IsLetteraEbraica(char c)
     {
     return ((c >= '\u0591' && c <= '\u05f4') || (c >= '\ufb1e' && c <= '\ufb4f'));
     }
     
     private static bool IsLetteraGreca(char c)
     { // anche in funzioni.cs
     return ((c >= '\u0370' && c <= '\u03ff') || (c >= '\u1f00' && c <= '\u1fff'));
     }
     */
    /// <summary>
    /// Data una stringa con diverse lingue separate da una riga verticale |, restituisce la prima
    /// </summary>
    /// <param name="lingua">Un elenco di lingue separate da una riga verticale.</param>
    /// <returns>La lingua principale.</returns>
    func linguaPrincipale(_ lingua:String) -> String
    { // anche in funzioni.cs, Light
        if (!lingua.isEmpty) {
            return (lingua.split(separator:"|").map{String($0)})[0].lowercased()
        }
        else {
            return "";
        }
    }
    
    func rightToLeft(_ lingua:String) -> Bool
    { // anche in funzioni.cs
        let linguaPrincipale = linguaPrincipale(lingua);
        return (linguaPrincipale == "he" || linguaPrincipale == "ar");
    }
    
    /*
     /// <summary>
     /// Cancella il testo nascosto da una stringa in formato RTF.
     /// </summary>
     /// <param name="testoRtf">La stringa da cui cancellare il testo nascosto.</param>
     /// <returns>Una stringa senza il testo nascosto.</returns>
     public static string RimuoviTestoNascosto(string testoRtf)
     {
     // qualcosa di simile in rtbEx.cs::CopiaSenzaTestoNascosto
     while (testoRtf.indexOf(@"\v\'01") > 0)
     {
     testoRtf = testoRtf.Remove(testoRtf.indexOf(@"\v\'01"), 14); // InizioRiferimento
     }
     while (testoRtf.indexOf(@"\'01") > 0)
     {
     testoRtf = testoRtf.Remove(testoRtf.indexOf(@"\'01"), 12); // InizioRiferimento
     }
     while (testoRtf.indexOf(@"\v\'02\v0 ") > 0) // InizioLink
     {
     testoRtf = testoRtf.Remove(testoRtf.indexOf(@"\v\'02\v0 "), 10);
     }
     while (testoRtf.indexOf(@"\v\'03") > 0) // FineLink1
     {
     int p = testoRtf.indexOf(@"\'04", testoRtf.indexOf(@"\v\'03")); // FineLink2
     if (p > 0 && p + 6 < testoRtf.Length && testoRtf.Substring(p, 7) == @"\'04\v0")
     testoRtf = testoRtf.Remove(testoRtf.indexOf(@"\v\'03"), p - testoRtf.indexOf(@"\v\'03") + 7);
     else
     {
     p = testoRtf.indexOf(@"\'04", testoRtf.indexOf(@"\v\'03"));
     if (p > 0)
     testoRtf = testoRtf.Remove(testoRtf.indexOf(@"\v\'03"), p - testoRtf.indexOf(@"\v\'03") + 4);
     }
     }
     while (testoRtf.indexOf(@"\v\'0e") > 0) // testo ricercato
     {
     int p = testoRtf.indexOf(@"\v0", testoRtf.indexOf(@"\v\'0e"));
     if (p > 0)
     testoRtf = testoRtf.Remove(p, 3).Remove(testoRtf.indexOf(@"\v\'0e"), 6);
     }
     return testoRtf.replacingOccurrences(of:"\\v\\", with:"\\").replacingOccurrences(of:"\\'0e", with:"").replacingOccurrences("of:\\'02", with:"").replacingOccurrences(of:"\\v0", with:"");
     }
     
     */
    func convertiRTF(_ rtfIn:String, _ tipoVisualizzazione:Int = 0) -> String
    {
        var rtf = rtfIn
        var html = ""
        var n1:Int, n2:Int, n3:Int, fine:Int
        var tagChiuso:Bool;
        var inizioGrassetto = -1, inizioCorsivo = -1, inizioSotto = -1, inizioCaps = -1, inizioApice = -1;
        let charFineTag = " 1\\}<";
        
        if (rtf.hasPrefix("{\\rtf"))
        {
            rtf = rtf.replacingOccurrences(of:"\r", with:"");
            rtf = rtf.replacingOccurrences(of:"\n", with:"");
        }
        rtf = rtf.replacingOccurrences(of:"{\\rtf", with:"\\rtf");
        rtf = rtf.replacingOccurrences(of:"\\~", with:"&nbsp;");
        rtf = rtf.replacingOccurrences(of:"\\pard", with:"\\xpar");
        rtf = rtf.replacingOccurrences(of:"\\par ", with:"\n");
        rtf = rtf.replacingOccurrences(of:"\\par", with:"\n");
        rtf = rtf.replacingOccurrences(of:"\\xpar", with:"\\pard");
        
        n1 = rtf.indexOf("{\\fonttbl");
        while (n1 > 0)
        {
            fine = rtf.indexOf("}}", rtf.indexOf("{\\fonttbl"));
            if (fine > 0) {
                rtf = rtf.remove(rtf.indexOf("{\\fonttbl"), fine - rtf.indexOf("{\\fonttbl") + 2);
            }
            n1 = rtf.indexOf("{\\fonttbl");
        }
        n1 = rtf.indexOf("{\\colortbl");
        while (n1 > 0)
        {
            fine = rtf.indexOf("}", rtf.indexOf("{\\colortbl"));
            if (fine > 0) {
                rtf = rtf.remove(rtf.indexOf("{\\colortbl"), fine - rtf.indexOf("{\\colortbl") + 1);
            }
            n1 = rtf.indexOf("{\\colortbl");
        }
        n1 = rtf.indexOf("\\viewkind");
        while (n1 > 0)
        {
            fine = rtf.lastIndexOf("\\rtf", n1);
            rtf = rtf.remove(fine, n1 - fine + 10);
            n1 = rtf.indexOf("\\viewkind");
        }
        
        rtf = rtf.replacingOccurrences(of: "\\v\\f0\\fs24", with: "\\f0\\fs24\\v") // in Commentario abbreviato
        
        // TOD2 quando ci sono dizionari/libri: Minuscole in MNT non si vede, probabilmente perché troppo lungo; tabella in MNT -> HTML
        rtf = rtf.replacingOccurrences(of:"\\'02", with:InizioLink).replacingOccurrences(of:"\\'03", with:FineLink1).replacingOccurrences(of:"\\'04", with:FineLink2).replacingOccurrences(of:"\\'05", with:FineLinkBrano).replacingOccurrences(of:"\\'06", with:FineLinkNota);
        // per esempio in Vincent's WS c'è il codice \'02 invece del carattere
        rtf = rtf.replacingOccurrences(of:"\\v"+InizioLink, with:"\\v "+InizioLink).replacingOccurrences(of:"\\v"+FineLink1, with:"\\v "+FineLink1);
        //    ({\\i1 vedi} \\v\\v0 Isaia 7:14\\v#290070140000-290070140000\\v
        // -> ({\\i1 vedi} \\v \\v0 Isaia 7:14\\v#290070140000-290070140000\\v
        
        // \\v\\v0 A Student's Guide to New Testament Textual Variants\\v\ahttp://bible.ovc.edu/tc/lay01mat.htm#mt5_4\\v0
        n1 = rtf.indexOf("Guide to New Testament Textual Variants")
        while n1 > 0 {
            n2 = rtf.lastIndexOf("\\v ", n1)
            n3 = rtf.indexOf("\\v0", n1)
            if rtf[(n2-11)..<n2] == "{\\i1 Vedi }" {
                n2 -= 11
            }
            if rtf[n3+3] == " " {
                n3 += 1
            }
            rtf = rtf.remove(n2, n3-n2+3)
            n1 = rtf.indexOf("Guide to New Testament Textual Variants")
        }
        rtf = rtf.replacingOccurrences(of: "\n \n", with: "\n")
        
        n1 = rtf.indexOf("\\v "+InizioLink+"\\v0 ");
        var tipoLink = 0, step = 0, numeroPrimaDellInizio = 0;
        var link:String
        while (n1 > 0) {
            numeroPrimaDellInizio = (Character(rtf[n1 - 1]).isWholeNumber) ? 0 : -1;
            n2 = rtf.indexOf("\\v ", n1 + 1);
            n3 = rtf.indexOf("\\v0", n2);
            link = rtf[(n2 + 5)..<(n3-1)].replacingOccurrences(of:" ", with:"+"); // per esempio in "Nuova Riveduta"
            switch (rtf[(n2 + 3)..<(n2 + 5)])
            {
            case FineLink1+FineLinkNota:
                tipoLink = 1;
                break;
            case FineLink1+FineLinkBrano:
                tipoLink = 2;
                break;
            default:
                tipoLink = 0;
                break;
            }
            if (rtf[n1 - 1] == "{" && rtf[n3 + 3] == "}") {
                step = 1; // rimuovere anche { } all'inizio e alla fine
            }
            else {
                step = 0;
            }
            rtf.insert(n3 + 3 + ((rtf[n3 + 3] == "\\" || (rtf[n3 + 3] == "}" && step == 0)) ? 0 : 1), "</a>");
            rtf = rtf.remove(n2, n3 - n2 + 3 + step); rtf=rtf.remove(n1 - step, 8);
            switch (tipoLink)
            {
            case 1: // asc(2), asc(3), asc(6), asc(4)
                // \\v \\v0 \\f4 Papiri\\v \\f4 Papiri\\v0
                // \\v \\v0 Mt 1:1\\v Mt 1:1\\v0
                rtf.insert(n1 + numeroPrimaDellInizio, " <a href=\"lpnn://" + ((stripRTF(link)).addingPercentEncoding(withAllowedCharacters: .alphanumerics) ?? "") + "?ip=1\">");
                //rtf.insert(n1 + numeroPrimaDellInizio, " <a href=\"lpnn://" + HttpUtility.UrlEncode(StripRTF(link)) + "?ip=1\">");
                break;
            case 2: // asc(2), asc(3), asc(5), asc(4)
                // {\\v \\v0 Giob 38:7; Sal 19:1,2; 104:24,31; Lam 3:38; 1Ti 4:4\\v #220380070000-220380070000#230190010000-230190010000#230190020000-230190020000#231040240000-231040240000#231040310000-231040310000#310030380000-310030380000#610040040000-610040040000\\v0}
                // \\v \\v0 *\\v Nuova Riveduta\\\\#550040050000+550040070000\\v
                rtf.insert(n1 + numeroPrimaDellInizio, " <a href=\"lpnb://" + link + "?ip=1\">");
                break;
            default:
                break;
            }
            n1 = rtf.indexOf("\\v "+InizioLink+"\\v0 ");
        }
        
        // \\v\\v0 A Student's Guide to New Testament Textual Variants\\v\ahttp://bible.ovc.edu/tc/lay01mat.htm#mt5_4\\v0
        n1 = rtf.indexOf("\\v "+InizioLink+"\\v0 "); // questo non funziona, ma forse non ci sono link esterni nelle collezioni
        while (n1 > 0)
        {
            n2 = rtf.indexOf("\\v"+FineLink1, n1 + 1);
            n3 = rtf.indexOf(FineLink2+"\\v0", n2);
            link = rtf[(n2 + 4)..<(n3)]
            if (link.hasPrefix("http:"))
            { // perché il programma non può aprire siti non sicuri (http: invece di https:)
                if link.hasPrefix("http://bible.ovc.edu/tc") { // sito non esiste più
                    rtf = rtf.remove(n1, n3-n1+4)
                }
                else {
                    rtf.insert(n3 + 4, " (" + link + ")");
                    rtf = rtf.remove(n2, n3 - n2 + 4);
                    rtf = rtf.remove(n1, 7);
                }
            }
            else
            {
                rtf.insert(n3 + 4, "</a>");
                rtf = rtf.remove(n2, n3 - n2 + 4);
                rtf = rtf.remove(n1, 7);
                rtf.insert(n1, " <a href=\"" + link + "?ip=1\">");
            }
            n1 = rtf.indexOf("\\v"+InizioLink+"\\v0 ");
        }
        
        n1 = rtf.indexOf("\\v ");
        var rtftemp:String
        while (n1 > 0)
        { // toglie i segnalibri per i versetti, ma forse serviranno per altre cose
            rtftemp = rtf[n1...]
            n2 = rtftemp.indexOf("}");
            n3 = rtftemp.indexOf("\\v0");
            /*n2 = rtf.indexOf("}", n1);
             n3 = rtf.indexOf("\\v0", n1);*/
            if ((n2 < n3 || n3 < 0) && n2 > 0)
            {
                rtf = rtf.remove(n1 - 1, n2 + 2); // togliere anche { prima di 'v '
                //rtf = rtf.remove(n1 - 1, n2 - n1 + 2); // togliere anche { prima di 'v '
            }
            else if (n3 > 0) {
                rtf = rtf.remove(n1, n3 + 3);
                //rtf = rtf.remove(n1, n3 - n1 + 3);
            }
            n1 = rtf.indexOf("\\v ");
        }
        
        n1 = rtf.indexOf("\\'");
        while (n1 > 0)
        {
            rtf.insert(n1+4, String(UnicodeScalar(UInt8(rtf[n1+2..<n1+4], radix:16) ?? 32)))
            rtf = rtf.remove(n1, 4)
            n1 = rtf.indexOf("\\'");
        }
        
        var i = 0
        var c = "q"
        var cpiu1 = "q"
        var i1 = 1
        let slash2 = "\\"
        let rtfcount = rtf.count
        let rtfArray = Array(rtf)
        var i2b = true
        let spazio = " "
        let rbracket = "}"
        while (i < rtfcount)
        {
            c = String(rtfArray[i])
            tagChiuso = false;
            if (c == "{" && (i==0 || (i>0 && String(rtfArray[i-1]) != slash2)))
            {
                // { come codice RTF
            }
            else if (c == rbracket && String(rtfArray[i-1]) != slash2)
            {
                if (inizioGrassetto >= 0)
                {
                    inizioGrassetto = -1;
                    html.append("</b>");
                }
                if (inizioCorsivo >= 0)
                {
                    inizioCorsivo = -1;
                    html.append("</i>");
                }
                if (inizioSotto >= 0)
                {
                    inizioSotto = -1;
                    html.append("</u>");
                }
                if (inizioCaps >= 0)
                {
                    inizioCaps = -1;
                    html.append("</span>");
                }
                if (inizioApice >= 0)
                {
                    inizioApice = -1;
                    html.append("</sup>");
                }
            }
            else if (c == slash2)
            {
                i1 = i + 1
                if (i + 6 < rtfcount && rtf[(i1)..<(i+7)] == "lptit1")
                {
                    if (!html.hasSuffix("<p>")) {
                        html.append("</p><p>");
                    }
                }
                if (inizioGrassetto >= 0)
                {
                    if (String(rtfArray[i1]) + String(rtfArray[i1+1]) == "b0" || rtf[(i1)..<(i+5)] == "pard" || rtf[(i1)..<(i+6)] == "plain")
                    {
                        inizioGrassetto = -1;
                        html.append("</b>");
                    }
                }
                if (inizioCorsivo >= 0)
                {
                    if (String(rtfArray[i1]) + String(rtfArray[i1+1]) == "i0" || rtf[(i1)..<(i+5)] == "pard" || (rtfcount - i > 6 && rtf[(i1)..<(i+6)] == "plain"))
                    {
                        inizioCorsivo = -1;
                        html.append("</i>");
                    }
                }
                if (inizioSotto >= 0)
                {
                    if (rtf[(i1)..<(i+4)] == "ul0" || rtf[(i1)..<(i+5)] == "pard" || rtf[(i1)..<(i+6)] == "plain" || rtf[(i1)..<(i+7)] == "ulnone")
                    {
                        inizioSotto = -1;
                        html.append("</u>");
                    }
                }
                if (inizioCaps >= 0)
                {
                    if (rtf[(i1)..<(i+6)] == "caps0" || rtf[(i1)..<(i+5)] == "pard" || rtf[(i1)..<(i+6)] == "plain")
                    {
                        inizioCaps = -1;
                        html.append("</div>");
                    }
                }
                if (inizioApice >= 0)
                {
                    if (rtf[(i1)..<(i+11)] == "nosupersub" || rtf[(i1)..<(i+5)] == "pard" || rtf[(i1)..<(i+6)] == "plain")
                    {
                        inizioApice = -1;
                        html.append("</sup>");
                    }
                    
                }
                
                if (String(rtfArray[i1]) == "{")
                {
                    html.append("&lbrace;");
                    i = i1 // ie i+1
                    i1 += 1
                    tagChiuso = true;
                }
                if (String(rtfArray[i1]) == "u" && rtfArray[i + 2].isWholeNumber)
                { // questo "if" deve essere dopo il controllo (rtf[i + 1] == '{') e prima del controllo } (altrimenti errore quando \} all fine della stringa
                    fine = rtf.indexOf("?", i);
                    if (fine > 0 && fine <= i + 7)
                    {
                        //if (rtf.Substring(i + 2, fine - i - 2) == "962")
                        //    fine = fine + 1 - 1;
                        html.append("&#"+rtf[(i+2)..<(fine)]+";")
                        
                        i = fine;
                        tagChiuso = true;
                    }
                }
                if (String(rtfArray[i1]) == "}")
                {
                    html.append("&rbrace;");
                    i = i1 // ie i+1
                    i1 += 1
                    tagChiuso = true;
                }
                
                if (!tagChiuso)
                {
                    i1 = i + 1 // i poteva essere cambiato nelle righe precedenti
                    cpiu1 = String(rtfArray[i1])
                    //let i2b = (charFineTag.indexOf(rtf[i + 2])>=0)
                    i2b = (rtfcount-i>2 && charFineTag.indexOf(String(rtfArray[i + 2]))>=0)
                    if (cpiu1 == "b" && i2b)
                    {
                        if (inizioGrassetto < 0) {
                            html.append("<b>");
                        }
                        inizioGrassetto = 1;
                    }
                    else if (cpiu1 == "i" && i2b)
                    {
                        if (inizioCorsivo < 0) {
                            html.append("<i>");
                        }
                        inizioCorsivo = 1;
                    }
                    else if (rtf[(i1)..<(i+3)] == "ul" && charFineTag.indexOf(rtf[i + 3])>=0)
                    {
                        if (inizioSotto < 0) {
                            html.append("<u>")
                        }
                        inizioSotto = 1;
                    }
                    else if (rtfcount - i > 6 && rtf[(i1)..<(i+5)] == "caps" && charFineTag.indexOf(rtf[i + 5])>=0)
                    {
                        if (inizioCaps < 0) {
                            html.append("<span style=\"text-transform: uppercase;\">");
                        }
                        inizioCaps = 1;
                    }
                    else if (rtfcount - i > 7 && rtf[(i1)..<(i+6)] == "super" && charFineTag.indexOf(rtf[i + 6])>=0)
                    {
                        if (inizioApice < 0) {
                            html.append("<sup>");
                        }
                        inizioApice = 1;
                    }
                    
                    rtftemp = rtf[i1...]
                    //n1 = rtf.indexOf(spazio, i) - i1;
                    n1 = rtftemp.indexOf(spazio) + 1 // spazio va saltato, ma gli altri termini (\, }, \n, <) vanno inclusi
                    n2 = rtftemp.indexOf(slash2);
                    if (n1 < 1 || (n2 >= 0 && n2 < n1)) {n1 = n2}
                    n2 = rtftemp.indexOf(rbracket)
                    if (n1 < 0 || (n2 >= 0 && n2 < n1)) {n1 = n2}
                    n2 = rtftemp.indexOf("\n") ;
                    if (n1 < 0 || (n2 >= 0 && n2 < n1)) {n1 = n2}
                    n2 = rtftemp.indexOf("<");
                    if (n1 < 0 || (n2 >= 0 && n2 < n1)) {n1 = n2}
                    if (n1 < 0) { n1 = rtfcount-i1 }
                    i = n1+i;
                }
            }
            else
            {
                if (c == "\n") {
                    html.append("<br />");
                }
                else
                {
                    if (c <= "~") { // così includo anche charatteri 128-255
                        //if (c <= "ÿ") {
                        html.append(c);
                    }
                    else {
                        for scalar in c.unicodeScalars {
                            html.append("&#" + String(scalar.value) + ";")
                        }
                    }
                }
            }
            i += 1
        }
        
        if html == " " {
            html = String(localized: "Questo brano non esiste in questa versione.")
        }
        
        return mostraHtml(html, tipoVisualizzazione)
    }
    
    func mostraHtml(_ s:String, _ tipoVisualizzazione:Int = 0) -> String
    {
        switch tipoVisualizzazione {
        case 1: // informazioni sul testo in Biblioteca
            return "<html><head><meta content='width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=0' name='viewport' />" + "</head><body><p>" + s + "</p></body></html>";
        case 2: // convertire un pezzo di testo, senza crearne una pagina HTML
            return s
        default: // caso normale, cioè = 0
            if s.hasPrefix("<html><head><meta content") && s.hasSuffix("</p></div></body></html>") {
                return s
            }
            let dimFontStringa = String(formato.fontDimensione)
            // questo codice anche in Guida.swift
            let fontNomeDaUsare = (formato.fontNome=="SF Pro" ? "-apple-system" : formato.fontNome) // lo spazio nel nome è un problema
            let nomeFontStringa = (!formato.fontNome.isEmpty ? "font-family:" + fontNomeDaUsare + ";" : "");
            
            // dark mode da https://css-tricks.com/a-complete-guide-to-dark-mode-on-the-web/
            let stile = "<style>body {color: #222;background: #fff;}a {color: #0033cc;}@media(prefers-color-scheme: dark){body {color: #eee;background: #121212;background-color: #121212;}body a {color: #809fff;}}</style>";
            let script = """
    <script type='text/javascript'>
    function isElementInViewport(el) {
    const rect = el.getBoundingClientRect();
    return (rect.top >= 0 && rect.left >= 0 && rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && rect.right <= (window.innerWidth || document.documentElement.clientWidth) );
    }
    
    function findFirstVisibleTarget() {
    const targets = document.querySelectorAll('[id]');
    for (let target of targets) {
    if (isElementInViewport(target)) {
    return target.id;
    }
    }
    return "";
    }
    </script>
    """
            //return "<html><head><meta content='width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=0' name='viewport' />" + "</head><body oncontextmenu='contextMenu(event)'><script type='text/javascript'>function contextMenu(e) {e.preventDefault();}</script>" + stile + "<div style='" + nomeFontStringa + "font-size:" + dimFontStringa + "px;';><p>" + s + "</p></div></body></html>";
            return "<html><head><meta content='width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=0' name='viewport' /><title>Title</title><script type='text/javascript'>function testev() {return 3;}</script>" + script.replacingOccurrences(of: "\n", with: " ") + "</head><body>" + stile + "<div style='" + nomeFontStringa + "font-size:" + dimFontStringa + "px;';><p>" + s + "</p></div></body></html>";
        }
    }
    
    func stripRTF(_ rtfIn:String) -> String
    {
        var rtf = rtfIn.trim();
        var i = rtf.indexOf("\\");
        var n1:Int, n2:Int;
        while (i >= 0)
        {
            n1 = rtf.indexOf(" ", i);
            n2 = rtf.indexOf("+", i);
            if (n1 < 0 || (n2 > 0 && n2 < n1)) { n1 = n2 }
            n2 = rtf.indexOf("\\", i+1) - 1;
            if (n1 < 0 || (n2 > 0 && n2 < n1)) { n1 = n2 }
            n2 = rtf.indexOf("}", i) - 1;
            if (n1 < 0 || (n2 > 0 && n2 < n1)) { n1 = n2 }
            n2 = rtf.indexOf("\n", i) - 1;
            if (n1 < 0 || (n2 > 0 && n2 < n1)) { n1 = n2 }
            if (n1 < 0) { n1 = rtf.count-1 }
            rtf = rtf.remove(i, n1 - i + 1);
            i = rtf.indexOf("\\");
        }
        return rtf;
    }
    
    func stripHtml(_ s:String) -> String
    {
        var a = s
        a = a.replacingOccurrences(of:"&nbsp;", with:" ");
        a = a.replacingOccurrences(of:"</p>", with:"\n");
        a = a.replacingOccurrences(of:"<br />", with:"\n");
        var n = a.indexOf("<");
        var n1:Int;
        while (n >= 0)
        {
            n1 = a.indexOf(">", n);
            if (n1 < 0) {
                break
            }
            a = a.remove(n, n1 - n + 1);
            n = a.indexOf("<");
        }
        return a;
    }
}
