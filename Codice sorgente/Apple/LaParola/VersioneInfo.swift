//
//  VersioneInfo.swift
//  LaParola
//
//  Created by admin on 13/02/24.
//

import Foundation

/// <summary>
/// Il modo in cui una collezione è bloccata.
/// </summary>
public enum BloccatoTipi : UInt8
{
    /// <summary>
    /// La collezione non è bloccata.
    /// </summary>
    case Sbloccato = 0
    /// <summary>
    /// La collezione è bloccata, ma può essere sbloccata.
    /// </summary>
    case Bloccato = 1
    /// <summary>
    /// La collezione è bloccata, ma non può essere sbloccata dall'utente.
    /// </summary>
    case BloccatoSempre = 2
}

/// <remarks>
/// Il tipo di un certo testo.
/// </remarks>
public enum TestoTipi: UInt8
{
    /// <summary>
    /// Il tipo non è stato impostato.
    /// </summary>
    case None=0
    /// <summary>
    /// Una versione della Bibbia (o una parte).
    /// </summary>
    case Bibbia=1
    /// <summary>
    /// Un commentario, cioè delle note collegate a versetti o brani.
    /// </summary>
    case Commentario=2
    /// <summary>
    /// Un dizionario, cioè delle note collegate a temi.
    /// </summary>
    case Dizionario = 4
    /// <summary>
    /// Un libro, cioè note che hanno un ordine.
    /// </summary>
    case Libro = 8
};

/// <remarks>
/// Informazione su un testo che è in un file dei dati, che può essere una versione della Bibbia
/// oppure un commentario e/o un un dizionario e/o un libro.
/// </remarks>
public struct VersioneInformazioni
{
    var versione: String = "0.0.0"
    var nomeDelFile: String = "";
    var nome: String  = "";
    var abbreviazione: String  = "";
    var titolo: String  = "";
    var autore: String  = "";
    var casaEditrice: String  = "";
    var data: String  = "";
    var copyright: String  = "";
    var isbn: String  = "";
    var descrizione: String  = "";
    var lingua: String  = "";
    var versioneDelleNote: String  = "";
    var tipo = TestoTipi.None;
    var bloccato = BloccatoTipi.Sbloccato;
    /*
    private string versione;
    /// <summary>
    /// Il numero della versione del file (nel formato 7.13.11)
    /// </summary>
    public string Versione
    {
        get { return versione; }
        set { versione = value; }
    }

    private string nomeDelFile;
    /// <summary>
    /// Il nome e percorso del file che contiene il testo.
    /// </summary>
    public string NomeDelFile
    {
        get { return nomeDelFile; }
        set { nomeDelFile = value; }
    }

    private string nome;
    /// <summary>
    /// Il nome del testo.
    /// </summary>
    public string Nome
    {
        get { return nome; }
        set { nome = value; }
    }

    private string abbreviazione;
    /// <summary>
    /// L'abbreviazione del testo.
    /// </summary>
    public string Abbreviazione
    {
        get { return abbreviazione; }
        set { abbreviazione = value; }
    }

    private string titolo;
    /// <summary>
    /// Il titolo del testo, di solito più lungo del nome, e visualizzato solo nella finestra Informazioni su.
    /// </summary>
    public string Titolo
    {
        get { return titolo; }
        set { titolo = value; }
    }

    private string autore;
    /// <summary>
    /// L'autore del testo (per una Bibbia, di solito è vuota)
    /// </summary>
    public string Autore
    {
        get { return autore; }
        set { autore = value; }
    }

    private string casaEditrice;
    /// <summary>
    /// La casa editrice del testo.
    /// </summary>
    public string CasaEditrice
    {
        get { return casaEditrice; }
        set { casaEditrice = value; }
    }

    private string data;
    /// <summary>
    /// La data di pubblicazione del testo.
    /// </summary>
    public string Data
    {
        get { return data; }
        set { data = value; }
    }

    private string copyright;
    /// <summary>
    /// Una stringa che descrive il copyright del testo.
    /// </summary>
    public string Copyright
    {
        get { return copyright; }
        set { copyright = value; }
    }

    private string isbn;
    /// <summary>
    /// Il numero ISBN del testo.
    /// </summary>
    public string Isbn
    {
        get { return isbn; }
        set { isbn = value; }
    }

    private string descrizione;
    /// <summary>
    /// Una descrizione del testo. Può essere in formato RTF.
    /// </summary>
    public string Descrizione
    {
        get { return descrizione; }
        set { descrizione = value; }
    }

    private string lingua;
    /// <summary>
    /// La lingua principale del testo. Deve essere un codice ISO 639-1 (2 lettere) oppure ISO 639-2 (3 lettere).
    /// Può anche essere diverse lingue separate da una riga verticale |, principale (che è la lingua quando il testo è considerato come dizionario) e secondarie,
    /// per esempio un dizionario greco-italiano avrebbe lingua el|it.
    /// </summary>
    public string Lingua
    {
        get { return lingua; }
        set { lingua = value; }
    }

    private string versioneDelleNote;
    /// <summary>
    /// La versione della Bibbia a cui le note fanno riferimento. È vuoto per una Bibbia.
    /// </summary>
    public string VersioneDelleNote
    {
        get { return versioneDelleNote; }
        set { versioneDelleNote = value; }
    }

    private TestoTipi tipo;
    /// <summary>
    /// Il tipo del testo.
    /// </summary>
    public TestoTipi Tipo
    {
        get { return tipo; }
        set { tipo = value; }
    }

    private BloccatoTipi bloccato;
    /// <summary>
    /// Il tipo del bloccaggio di una collezione di note.
    /// </summary>
    public BloccatoTipi Bloccato
    {
        get { return bloccato; }
        set { bloccato = value; }
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
*/
}

