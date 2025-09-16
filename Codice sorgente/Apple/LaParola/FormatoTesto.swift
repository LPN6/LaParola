//
//  FormatoTesto.swift
//  LaParola
//
//  Created by admin on 14/02/24.
//

import Foundation
import SwiftUI

//#region enum per il formato del testo

/// <summary>
/// Come visualizzare il testo proprio.
/// </summary>
public enum TestoVisualizzato : Encodable, Decodable
{
    /// <summary>
    /// Mostrare ogni versetto su una riga diversa.
    /// </summary>
    case Versetti
    /// <summary>
    /// Mostrare il testo come paragrafi.
    /// </summary>
    case Paragrafi
    /// <summary>
    /// Non mostrare il testo.
    /// </summary>
    case Nessuno
};

/// <summary>
/// Come visualizzare i riferimenti nel testo.
/// </summary>
public enum RiferimentoTipo : Encodable, Decodable
{
    /// <summary>
    /// Con due punti fra il capitolo e il versetto, per esempio 1P 5:2,6-7
    /// </summary>
    case DuePunti
    /// <summary>
    /// Con una virgola fra il capitolo e il versetto, per esempio 1P 5,2.6-7
    /// </summary>
    case Virgola
    /// <summary>
    /// Come una citazione, per esempio 1P., 5, 2.6-7:
    /// </summary>
    case Citazione
};

/// <summary>
/// Come visualizzare il libro nel riferimento.
/// </summary>
public enum RiferimentoFormato : Encodable, Decodable
{
    /// <summary>
    /// Il nome intero del libro.
    /// </summary>
    case Intero
    /// <summary>
    /// L'abbreviazione del libro.
    /// </summary>
    case Abbreviazione
    /// <summary>
    /// Non mostrare nessun riferimento.
    /// </summary>
    case Nessuno
    /// <summary>
    /// Non mostrare il nome del libro.
    /// </summary>
    case NessunoLibro
    /// <summary>
    /// Usare un'abbreviazione del libro che il programma riconosce.
    /// </summary>
    case AbbreviazioneRiconosciuta
};

/// <summary>
/// Dove visualizzare i riferimenti nel testo.
/// </summary>
public enum RiferimentoPosto : Encodable, Decodable
{
    /// <summary>
    /// Prima del testo del versetto, sulla stessa riga.
    /// </summary>
    case PrimaStessaRiga
    /// <summary>
    /// Prima del testo del versetto, sulla riga precedente.
    /// </summary>
    case PrimaRigaDiversa
    /// <summary>
    /// Dopo il testo del versetto.
    /// </summary>
    case Dopo
};

/// <remarks>
/// Descrive il formato usato per visualizzare il testo biblico.
/// </remarks>
@Observable public class FormatoTesto: Encodable, Decodable/*, Equatable */
{
var libriNomi:[String] = []
    var libriAbbreviazioniUsate:[String] = []
    var libriAbbreviazioniRiconosciute:[String] = []
    
#if os(macOS)
    var fontDimensione:Double=12;
    var fontGrecoDimensione:Double=12;
    var fontEbraicoDimensione:Double=12;
    var fontRiferimentoDimensione:Double=12;
    var fontRicercaDimensione:Double=12;
#endif
#if os(iOS)
    var fontDimensione:Double=17;
    var fontGrecoDimensione:Double=17;
    var fontEbraicoDimensione:Double=17;
    var fontRiferimentoDimensione:Double=17;
    var fontRicercaDimensione:Double=17;
#endif
    
#if os(macOS)
    var fontNome:String = "Helvetica"
    var fontGrecoNome:String = "Helvetica"
    var fontEbraicoNome:String = "Helvetica"
    var fontRiferimentoNome:String = "Helvetica"
    var fontRicercaNome:String = "Helvetica"
#endif
#if os(iOS)
    var fontNome:String = "SF Pro"
    var fontGrecoNome:String = "SF Pro"
    var fontEbraicoNome:String = "SF Pro"
    var fontRiferimentoNome:String = "SF Pro"
    var fontRicercaNome:String = "SF Pro"
#endif
    
    var fontGrassetto:Bool=false;
    var fontCorsivo:Bool=false;
    var fontSottolineato:Bool=false;
    //var fontColore:Color = Color.black
    
    var fontGrecoGrassetto:Bool=false;
    var fontGrecoCorsivo:Bool=false;
    var fontGrecoSottolineato:Bool=false;
    //var fontGrecoColore:Color = Color.black
    
    
    var fontEbraicoGrassetto:Bool=false;
    var fontEbraicoCorsivo:Bool=false;
    var fontEbraicoSottolineato:Bool=false;
    //var fontEbraicoColore:Color = Color.black
    
    
    var fontRiferimentoGrassetto:Bool=true;
    var fontRiferimentoCorsivo:Bool=false;
    var fontRiferimentoSottolineato:Bool=false;
    //var fontRiferimentoColore:Color = Color.black
    
    var riferimentoApice:Bool = false;
    var riferimentoContestoRicerche:Bool = false
    
    var fontRicercaGrassetto:Bool=false;
    var fontRicercaCorsivo:Bool=false;
    var fontRicercaSottolineato:Bool=true;
    //var fontRicercaColore:Color = Color.black
    
    var titoliVisualizzati:Bool=true;
    var riferimentoTipo:RiferimentoTipo = RiferimentoTipo.DuePunti;
    var riferimentoFormato:RiferimentoFormato = RiferimentoFormato.Abbreviazione;
    var riferimentoPosto:RiferimentoPosto = RiferimentoPosto.PrimaStessaRiga;
    var testoVisualizzato:TestoVisualizzato = TestoVisualizzato.Paragrafi;
    
    /// <summary>
    /// Copia tutte le caratteristiche di un formato ad un altro.
    /// </summary>
    /// <param name="formato">Il formato a cui copiare le caratteristiche.</param>
    func copiaA(_ formato: inout
                FormatoTesto)
    {
        formato.libriNomi = libriNomi
        formato.libriAbbreviazioniUsate = libriAbbreviazioniUsate
        formato.libriAbbreviazioniRiconosciute = libriAbbreviazioniRiconosciute
        
        formato.fontNome = fontNome;
        formato.fontDimensione = fontDimensione;
        formato.fontGrassetto = fontGrassetto;
        formato.fontCorsivo = fontCorsivo;
        formato.fontSottolineato = fontSottolineato;
        //formato.fontColore = fontColore;
        
        formato.fontGrecoNome = fontGrecoNome;
        formato.fontGrecoDimensione = fontGrecoDimensione;
        formato.fontGrecoGrassetto = fontGrecoGrassetto;
        formato.fontGrecoCorsivo = fontGrecoCorsivo;
        formato.fontGrecoSottolineato = fontGrecoSottolineato;
        //formato.fontGrecoColore = fontGrecoColore;
        
        formato.fontEbraicoNome = fontEbraicoNome;
        formato.fontEbraicoDimensione = fontEbraicoDimensione;
        formato.fontEbraicoGrassetto = fontEbraicoGrassetto;
        formato.fontEbraicoCorsivo = fontEbraicoCorsivo;
        formato.fontEbraicoSottolineato = fontEbraicoSottolineato;
        //formato.fontEbraicoColore = fontEbraicoColore;
        
        formato.fontRiferimentoNome = fontRiferimentoNome;
        formato.fontRiferimentoDimensione = fontRiferimentoDimensione;
        formato.fontRiferimentoGrassetto = fontRiferimentoGrassetto;
        formato.fontRiferimentoCorsivo = fontRiferimentoCorsivo;
        formato.fontRiferimentoSottolineato = fontRiferimentoSottolineato;
        //formato.fontRiferimentoColore = fontRiferimentoColore;
        formato.riferimentoApice = riferimentoApice;
        formato.riferimentoContestoRicerche = riferimentoContestoRicerche;
        
        formato.fontRicercaNome = fontRicercaNome;
        formato.fontRicercaDimensione = fontRicercaDimensione;
        formato.fontRicercaGrassetto = fontRicercaGrassetto;
        formato.fontRicercaCorsivo = fontRicercaCorsivo;
        formato.fontRicercaSottolineato = fontRicercaSottolineato;
        //formato.fontRicercaColore = fontRicercaColore;
        
        formato.titoliVisualizzati = titoliVisualizzati;
        formato.riferimentoTipo = riferimentoTipo;
        formato.riferimentoFormato = riferimentoFormato;
        formato.riferimentoPosto = riferimentoPosto;
        formato.testoVisualizzato = testoVisualizzato;
    }
}
