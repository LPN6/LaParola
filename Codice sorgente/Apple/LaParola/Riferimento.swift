//
//  Riferimento.swift
//  LaParola
//
//  Created by admin on 14/02/24.
//

import Foundation

/// <remarks>
/// Una classe che contiene informazioni sul riferimento di un brano, o una lista di note.
/// </remarks>
public struct Riferimento
{
    var versetti: Bool = true
    var brani:[[UInt8]] = []
    var daTradurre: Bool = false
    var note:[String] = []
    var numeroParola:[[UInt16]] = []
    
    init ()
    {
        versetti = true
    }

    /// <summary>
    /// Il costruttore della classe, che crea un riferimento a versetti con un versetto.
    /// </summary>
    /// <param name="libro">Il libro del versetto.</param>
    /// <param name="capitolo">Il capitolo del versetto.</param>
    /// <param name="versetto">Il versetto del versetto.</param>
    init (_ libro:UInt8, _ capitolo:UInt8, _ versetto:UInt8)
    {
        brani.append([libro, capitolo, versetto, libro, capitolo, versetto]);
        numeroParola.append([]);
    }
    
    init (_ libro:Int, _ capitolo:Int, _ versetto:Int)
    {
        brani.append([UInt8(libro), UInt8(capitolo), UInt8(versetto), UInt8(libro), UInt8(capitolo), UInt8(versetto)]);
        numeroParola.append([]);
    }
    
    // tutto il capitolo
    init (_ libro:Int, _ capitolo:Int)
    {
        brani.append([UInt8(libro), UInt8(capitolo), UInt8(1), UInt8(libro), UInt8(capitolo), UInt8(255)]);
        numeroParola.append([]);
    }

    /// <summary>
    /// Il costruttore della classe, che crea un riferimento a versetti con un brano.
    /// </summary>
    /// <param name="brano">Il brano del riferimento.</param>
    init (_ brano:[UInt8])
    {
        brani.append(brano)
        numeroParola.append([])
    }

    /// <summary>
    /// Il costruttore della classe, che crea un riferimento a versetti o a note.
    /// </summary>
    /// <param name="brano">Se vero, crea un riferimento ad un brano (l'alternativa è un elenco di note).</param>
    init (_ versetti:Bool)
    {
        self.versetti = versetti;
    }

    /// <summary>
    /// Il costruttore della classe, che crea un riferimento a versetti con un brano.
    /// </summary>
    /// <param name="riferimento">Un riferimento, di cui questo nuovo riferimento è una copia.</param>
    init (_ riferimento:Riferimento)
    {
        versetti = riferimento.versetti;
        daTradurre = riferimento.daTradurre;

        if (versetti) {
            for i in stride(from:0, to:riferimento.brani.count, by:1) {
                brani.append([ riferimento.brani[i][0], riferimento.brani[i][1], riferimento.brani[i][2], riferimento.brani[i][3], riferimento.brani[i][4], riferimento.brani[i][5] ]);
                numeroParola.append([]);
                for j in stride(from:0, to:riferimento.numeroParola[i].count, by:1) {
                            numeroParola[i].append(riferimento.numeroParola[i][j]);
                }
            }
        }
        else {
            for nota in riferimento.note {
                note.append(nota);
            }
        }
    }

    /// <summary>
    /// Aggiunge un brano ad un riferimento, in cui possibilmente ci sono anche la parola iniziale e finale
    /// </summary>
    /// <param name="brano">Il brano da aggiungere, come libro, capitolo, versetto, parola iniziale e libro, capitolo, versetto, parola finale.</param>
    mutating func aggiungiBrano8Byte(_ brano:[UInt8])
    {
        brani.append([ brano[0], brano[1], brano[2], brano[4], brano[5], brano[6] ]);
        var listaParole:[UInt16] = []
        if (brano[3] != 0 || brano[7] != 0)
        {
            listaParole.append(UInt16(brano[3]));
            listaParole.append(UInt16(brano[7]));
        }
        numeroParola.append(listaParole);
    }
    
    /// <summary>
    /// Aggiunge un brano al riferimento.
    /// </summary>
    /// <param name="brano">Il brano da aggiungere.</param>
    mutating func aggiungiBrano(_ brano:[UInt8])
    {
        brani.append(brano);
        let listaParole:[UInt16] = []
        numeroParola.append(listaParole);
    }

    /// <summary>
    /// Cancella un brano o una nota dal riferimento. Se il paramento è meno di 0 o più del numero dei brani/note, non succede niente.
    /// </summary>
    /// <param name="numero">Il numero del brano o della nota da rimuovere.</param>
    mutating func rimuovi(_ numero:Int)
    {
        if (numero >= 0 && numero < count())
        {
            if (versetti) {
                brani.remove(at: numero)
            }
            else {
                note.remove(at: numero)
            }
        }
    }
    /*
    /// <summary>
    /// Rimuove un versetto da un riferimento che contiene dai versetti.
    /// Se il versetto non è nel riferimento, o se il riferimento contiene delle note, non succede niente.
    /// </summary>
    /// <param name="brano">Il versetto di rimuovere.</param>
    public void RimuoviVersetto(byte[] brano)
    {
        if (versetti)
        {
            for (int i = brani.Count - 1; i >= 0; --i)
            {
                if (brani[i][0] == brano[0] && brani[i][1] == brano[1] && brani[i][2] == brano[2])
                    Rimuovi(i);
            }
        }
    }

    /// <summary>
    /// Rimuove tutti i versetti in un riferimento dal riferimento.
    /// </summary>
    /// <param name="riferimentoDaRimuovere">Il riferimento che contiene i versetti da rimuovere.</param>
    public void RimuoviVersetti(Riferimento riferimentoDaRimuovere)
    {
        foreach (byte[] brano in riferimentoDaRimuovere.Brani)
            RimuoviVersetto(brano);
    }
*/
    /// <summary>
    /// Cancella tutti i dati del riferimento.
    /// </summary>
    mutating func clear()
    {
        brani.removeAll();
        note.removeAll();
        numeroParola.removeAll();
        daTradurre = false;
        versetti = true
    }

    /// <summary>
    /// Indica se il primo versetto di due brani sono uguali.
    /// </summary>
    /// <param name="primoIndice">L'indice del primo brano.</param>
    /// <param name="secondoIndice">L'indice del secondo brano.</param>
    /// <returns>Un boolean che dà se i primi versetti sono uguali.</returns>
    public func primoVersettoUguale(_ primoIndice:Int, _ secondoIndice:Int) -> Bool
    {
        return (brani[primoIndice][0] == brani[secondoIndice][0] && brani[primoIndice][1] == brani[secondoIndice][1] && brani[primoIndice][2] == brani[secondoIndice][2]);
    }
    /*
    /// <summary>
    /// Indica se un brano è composto da uno solo versetto.
    /// </summary>
    /// <param name="indice">L'indice del brano.</param>
    /// <returns>Un boolean che dà se il brano è composto da uno solo versetto.</returns>
    public bool SoloUnoVersetto(int indice)
    {
        return (brani[indice][0] == brani[indice][3] && brani[indice][1] == brani[indice][4] && brani[indice][2] == brani[indice][5]);
    }

    internal void OrdinaNote()
    {
        note.Sort();
    }

 */
    /// <summary>
    /// Il numero di brani o di note nel riferimento.
    /// </summary>
    func count() -> Int
    {
         return versetti ? brani.count : note.count;
    }

    /*
    /// <summary>
    /// Aggiunge tutti i brani di un riferimento al riferimento.
    /// </summary>
    /// <param name="riferimento">Il riferimento che contiene i brani da aggiungere.</param>
    public void AggiungiBraniDaRiferimento(Riferimento riferimento)
    {
        for (int i = 0; i < riferimento.Brani.Count; ++i)
            AggiungiBranoEParole(riferimento.Brani[i], new Collection<UInt16>(riferimento.numeroParola[i]));
    }

    /// <summary>
    /// Aggiunge un brano e una collezione di parole selezionate nel primo versetto del brano al riferimento.
    /// </summary>
    /// <param name="brano">Il brano da aggiungere.</param>
    /// <param name="parole">I numeri delle parole nel versetto.</param>
    public void AggiungiBranoEParole(byte[] brano, Collection<UInt16> parole)
    {
        brani.Add(brano);
        numeroParola.Add(new List<UInt16>(parole));
    }

    /// <summary>
    /// Aggiunge una nota e una collezione di parole selezionate al riferimento.
    /// </summary>
    /// <param name="nota">Il titolo della nota da aggiungere.</param>
    /// <param name="parole">I numeri delle parole nella nota.</param>
    public void AggiungiNotaEParole(string nota, Collection<UInt16> parole)
    {
        note.Add(nota);
        numeroParola.Add(new List<UInt16>(parole));
    }

    /// <summary>
    /// <summary>
    /// Restituisce una string che rappresenta come tutto il riferimento è mostrato quando è il titolo di una nota.
    /// </summary>
    /// <returns>Il riferimento come il titolo di una nota.</returns>
    /// </summary>
    /// <returns>I titoli delle note nel riferimento.</returns>
    public string ComeNotaTuttoRiferimento()
    {
        // vedi ConvertiTitoloNotaARiferimento per l'altra direzione
        if (brani.Count > 0)
        {
            StringBuilder comeNota = new StringBuilder(26 * Brani.Count);
            for (int i = 0; i < Brani.Count; ++i)
                comeNota.Append(ComeNotaUnBrano(i));
            return comeNota.ToString();
        }
        else
            return "";
    }

    /// <summary>
    /// Restituisce una string che rappresenta come il primo brano nel riferimento è mostrato quando è il titolo di una nota.
    /// </summary>
    /// <returns>Il riferimento come il titolo di una nota.</returns>
    public string ComeNotaPrimoRiferimento()
    {
        // vedi ConvertiTitoloNotaARiferimento per l'altra direzione
        if (brani.Count > 0)
            return ComeNotaUnBrano(0);
        else
            return "";
    }

    private string ComeNotaUnBrano(int numeroBrano)
    {
        StringBuilder comeNota = new StringBuilder("#", 32);
        string temp;
        temp = "0" + brani[numeroBrano][0].ToString(CultureInfo.InvariantCulture);
        temp = temp.Remove(0, temp.Length - 2);
        comeNota.Append(temp);
        temp = "00" + brani[numeroBrano][1].ToString(CultureInfo.InvariantCulture);
        temp = temp.Remove(0, temp.Length - 3);
        comeNota.Append(temp);
        temp = "00" + brani[numeroBrano][2].ToString(CultureInfo.InvariantCulture);
        temp = temp.Remove(0, temp.Length - 3);
        comeNota.Append(temp);
        if (numeroParola[numeroBrano].Count < 2)
            comeNota.Append("0000-");
        else
        {
            temp = "0000" + numeroParola[numeroBrano][0].ToString(CultureInfo.InvariantCulture);
            temp = temp.Remove(0, temp.Length - 4);
            comeNota.Append(temp).Append("-");
        }

        temp = "0" + brani[numeroBrano][3].ToString(CultureInfo.InvariantCulture);
        temp = temp.Remove(0, temp.Length - 2);
        comeNota.Append(temp);
        temp = "00" + brani[numeroBrano][4].ToString(CultureInfo.InvariantCulture);
        temp = temp.Remove(0, temp.Length - 3);
        comeNota.Append(temp);
        temp = "00" + brani[numeroBrano][5].ToString(CultureInfo.InvariantCulture);
        temp = temp.Remove(0, temp.Length - 3);
        comeNota.Append(temp);
        if (numeroParola[numeroBrano].Count < 2)
            comeNota.Append("0000");
        else
        {
            temp = "0000" + numeroParola[numeroBrano][1].ToString(CultureInfo.InvariantCulture);
            temp = temp.Remove(0, temp.Length - 4);
            comeNota.Append(temp);
        }

        string notaStringa = comeNota.ToString();
        if (notaStringa.EndsWith("2552550000", StringComparison.Ordinal)) // un riferimento per tutto il libro
            notaStringa = notaStringa.Substring(0, 3) + "000000" + notaStringa.Substring(9, 7) + "0000000000";
        if (notaStringa.EndsWith("2550000", StringComparison.Ordinal)) // un riferimento per tutto il capitolo
            notaStringa = notaStringa.Substring(0, 6) + "000" + notaStringa.Substring(9, 10) + "0000000";
        return notaStringa;
    }

    /// <summary>
    /// Valuta se due riferimenti sono uguali.
    /// </summary>
    /// <param name="riferimentoDaConfrontare">Il riferimento con cui confrontare quello attuale.</param>
    /// <returns>Vero se i due riferimenti sono identici.</returns>
    public bool Uguale(Riferimento riferimentoDaConfrontare)
    {
        if (brani.Count != riferimentoDaConfrontare.brani.Count || note.Count != riferimentoDaConfrontare.note.Count || daTradurre != riferimentoDaConfrontare.DaTradurre || numeroParola.Count != riferimentoDaConfrontare.numeroParola.Count || versetti != riferimentoDaConfrontare.Versetti)
            return false;
        bool uguale = true;
        for (int i = 0; i < brani.Count; ++i)
        {
            if (brani[i][0] != riferimentoDaConfrontare.brani[i][0]
                || brani[i][1] != riferimentoDaConfrontare.brani[i][1]
                || brani[i][2] != riferimentoDaConfrontare.brani[i][2]
                || brani[i][3] != riferimentoDaConfrontare.brani[i][3]
                || brani[i][4] != riferimentoDaConfrontare.brani[i][4]
                || brani[i][5] != riferimentoDaConfrontare.brani[i][5]
                )
                uguale = false;
        }
        for (int i = 0; i < note.Count; ++i)
        {
            if (note[i] != riferimentoDaConfrontare.note[i])
                uguale = false;
        }
        for (int i = 0; i < numeroParola.Count; ++i)
        {
            if (numeroParola[i].Count != riferimentoDaConfrontare.numeroParola[i].Count)
                uguale = false;
            else
            {
                for (int j = 0; j < numeroParola[i].Count; ++j)
                    if (numeroParola[i][j] != riferimentoDaConfrontare.numeroParola[i][j])
                        uguale = false;
            }
        }
        return uguale;
    }

    /// <summary>
    /// Decide se il riferimento contiene un certo versetto.
    /// </summary>
    /// <param name="versettoDaRicercare">Un riferimento che contiene il versetto da controllare (solo l'inizio del primo brano è controllato).</param>
    /// <returns>Vero se il riferimento contiene il versetto. Falso se il riferimento da controllare era vuoto.</returns>
    public bool ContieneVersetto(Riferimento versettoDaRicercare)
    {
        if (versettoDaRicercare.brani.Count == 0)
            return false;
        byte[] versettoDaRicercaComeByte = versettoDaRicercare.brani[0];
        foreach (byte[] brano in brani)
        {
            if ((brano[0] < versettoDaRicercaComeByte[0]
                || (brano[0] == versettoDaRicercaComeByte[0] && brano[1] < versettoDaRicercaComeByte[1])
                || (brano[0] == versettoDaRicercaComeByte[0] && brano[1] == versettoDaRicercaComeByte[1] && brano[2] <= versettoDaRicercaComeByte[2]))
                && (brano[3] > versettoDaRicercaComeByte[0]
                || (brano[3] == versettoDaRicercaComeByte[0] && brano[4] > versettoDaRicercaComeByte[1])
                || (brano[3] == versettoDaRicercaComeByte[0] && brano[4] == versettoDaRicercaComeByte[1] && brano[5] >= versettoDaRicercaComeByte[2])))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Decide se il riferimento contiene almeno una parte di un certo brano.
    /// </summary>
    /// <param name="branoDaRicercare">Un riferimento che contiene il brano da controllare.</param>
    /// <returns>Vero se il riferimento contiene una parte del brano. Falso se il brano da controllare era vuoto.</returns>
    public bool ContieneBrano(Riferimento branoDaRicercare)
    {
        foreach (byte[] parteBranoDaRicercare in branoDaRicercare.brani)
        {
            foreach (byte[] parteRiferimento in brani)
            {
                if (Sovrapposizione(parteRiferimento, parteBranoDaRicercare))
                    return true;
            }
        }
        return false;
    }

    private static bool Sovrapposizione(byte[] b1, byte[] b2)
    {
        return ((b1[0] < b2[3]
            || (b1[0] == b2[3] && b1[1] < b2[4])
            || (b1[0] == b2[3] && b1[1] == b2[4] && b1[2] <= b2[5]))
            && (b1[3] > b2[0]
            || (b1[3] == b2[0] && b1[4] > b2[1])
            || (b1[3] == b2[0] && b1[4] == b2[1] && b1[5] >= b2[2])));
    }

    #region IComparer Members

    /// <summary>
    /// Confonta due riferimenti per determinare quale è primo.
    /// </summary>
    /// <param name="x">Il primo riferimento.</param>
    /// <param name="y">Il secondo riferimento.</param>
    /// <returns>-1, 0 o 1 se il primo riferimento è prima, uguale a o dopo il secondo.</returns>
    public int Compare(object x, object y)
    {
        try
        {
            Riferimento riferimento1 = (Riferimento)x;
            Riferimento riferimento2 = (Riferimento)y;
            if (riferimento1.Count == 0)
                if (riferimento2.Count == 0)
                    return 0;
                else
                    return -1;
            if (riferimento2.Count == 0)
                return 1;
            if (riferimento1.Versetti)
            {
                if (riferimento2.Versetti)
                {
                    byte[] brano1 = riferimento1.brani[0];
                    byte[] brano2 = riferimento2.brani[0];
                    if (brano1[0] < brano2[0])
                        return -1;
                    if (brano1[0] > brano2[0])
                        return 1;
                    // libro uguale
                    if (brano1[1] < brano2[1])
                        return -1;
                    if (brano1[1] > brano2[1])
                        return 1;
                    // capitolo uguale
                    if (brano1[2] < brano2[2])
                        return -1;
                    if (brano1[2] > brano2[2])
                        return 1;
                    // versetto uguale
                    List<UInt16> parole1 = riferimento1.numeroParola[0];
                    List<UInt16> parole2 = riferimento2.numeroParola[0];
                    if (parole1.Count == 0)
                        if (parole2.Count == 0)
                            return 0;
                        else
                            return -1;
                    if (parole2.Count == 0)
                        return 1;
                    if (parole1[0] < parole2[0])
                        return -1;
                    if (parole1[0] > parole2[0])
                        return 1;
                    return 0;
                }
                else
                    return -1; // versetti prima di note
            }
            else
            {
                if (riferimento2.Versetti)
                {
                    return 1; // versetti dopo note
                }
                else
                    if (riferimento1.note[0] == riferimento2.note[0])
                { // nota uguale
                    List<UInt16> parole1 = riferimento1.numeroParola[0];
                    List<UInt16> parole2 = riferimento2.numeroParola[0];
                    if (parole1.Count == 0)
                        if (parole2.Count == 0)
                            return 0;
                        else
                            return -1;
                    if (parole2.Count == 0)
                        return 1;
                    if (parole1[0] < parole2[0])
                        return -1;
                    if (parole1[0] > parole2[0])
                        return 1;
                    return 0;
                }
                else
                    return String.Compare(riferimento1.note[0], riferimento2.note[0], StringComparison.Ordinal);
            }
        }
        catch
        {
            return 0;
        }
    }

*/
}
