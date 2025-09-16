//
//  Versione.swift
//  LaParola
//
//  Created by admin on 15/02/24.
//

import Foundation

struct RadiceDiversa
{
    var  occorrenzaRadice:OccorrenzaParola = OccorrenzaParola()
    var  nuovaRadice:String = ""
    
    init(_ occorrenzaRadice: OccorrenzaParola = OccorrenzaParola(), _ nuovaRadice: String = "") {
        self.occorrenzaRadice = occorrenzaRadice
        self.nuovaRadice = nuovaRadice
    }
}

struct CitazioneRiferimento
{
    var Brano:[UInt8] = []
    var NumeroNota:UInt32 = 0
}

public class Versione
{
    //#region Proprietà
    
    private var genitore:Texts
    var info = VersioneInformazioni()
    let handle: FileHandle?
    let semaphore = DispatchSemaphore(value: 1)
    
    var pTesto: UInt32 = 0
    var pIndice: UInt32 = 0
    var pParole: UInt32 = 0
    var pRadici: UInt32 = 0
    var pParoleIndiceIndice: UInt32 = 0
    var pParoleIndice: UInt32 = 0
    var pCitazioniRiferimenti: UInt32 = 0
    var pInizioDati: UInt32 = 0
    var pRadiciDiParole:UInt64 = 0
    
    var capitoliInLibro:[UInt8] = []
    var versettiInCapitolo:[UInt8] = []
    var indiceLibro:[UInt16] = []
    var indiceCapitolo:[UInt16] = []
    
    var radiciDiverse:[RadiceDiversa] = []
    
    var riferimentiDiversi:[[Int16]] = []
    
    var _parole:[String] = []
    var _radici:[String] = []
    var _radiceDiParola:[UInt32] = []
    var _paroleDiRadice:[String] = []
    
    public func parole() -> [String]
    {
        if (_parole.count == 0)
        {
            do {
                semaphore.wait()
                defer { semaphore.signal()}
                
                try handle?.seek(toOffset:UInt64(pParole))
                _parole = try readString().split(separator:"|", omittingEmptySubsequences: true).map{String($0)}
            }
            catch {}
        }
        return _parole;
    }
    
    public func radici() -> [String]
    {
        if (_radici.count == 0)
        {
            do {
                if (pRadici > pInizioDati) { // quando ==, non ci sono radici in questa versione
                    semaphore.wait()
                    defer { semaphore.signal()}
                    
                    try handle?.seek(toOffset:UInt64(pRadici))
                    _radici = try readString().split(separator:"|", omittingEmptySubsequences: true).map{String($0)}
                    pRadiciDiParole = try handle?.offset() ?? 0
                }
                else {
                    //_radici = []; // non necessario, perché è già []
                }
            }
            catch {}
        }
        return _radici;
    }
    
    
    var citazioniRiferimenti:[CitazioneRiferimento] = []
    
    var noteInOrdine:[String] = []
    var noteTitoli:[String] = []
    var notePosizione:[Int] = []
    var noteNuoveTesto:[String] = []
    
    /// <summary>
    /// Costruttore della classe che descrive un testo nel programma.
    /// </summary>
    /// <param name="testi">La classe genitore che contiene tutti i testi.</param>
    /// <param name="nomeFile">Il nome del file (incluso il percorso) che contiene il testo.</param>
    init(_ testi:Texts, _ nomeFile:String) throws
    {
        genitore = testi;
        info.nomeDelFile = nomeFile;
        
        do {
            try handle = FileHandle(forReadingFrom: URL(filePath: nomeFile))
        }
        catch {
            handle = FileHandle()
            return
        }
        if (handle == nil) {
            return;
        }
        
        let tipo:UInt8
        let pRadiciDiverse:UInt32
        let pRiferimentiDiversi:UInt32
        let pNoteInOrdine:UInt32
        var data:Data?
        
        do {
            try data = handle?.read(upToCount:3)
            if data?[0] != Character("L").asciiValue || data?[1] != Character("P").asciiValue || data?[2] != Character("N").asciiValue {
                return
            }
            
            try data = handle?.read(upToCount:4)
            info.versione = String(data?[0] ?? 120) + "." + String(data?[1] ?? 120) + "." + String(data?[2] ?? 120)
            // la versione del programma deve essere dopo quella del testo
            let versioneApp = Bundle.main.infoDictionary?["CFBundleShortVersionString"] as? String
            if (confrontaVersioni(versioneApp ?? "0.0.0", info.versione) < 0) {
                return
            }
            
            let pInizioTesto = try readUInt32()
            try handle?.seek(toOffset:UInt64(pInizioTesto))
            
            pInizioDati = try readUInt32()
            try info.nome = readString();
            try info.abbreviazione = readString();
            try info.titolo = readString();
            
            // il campo dell'autore è stato introdotto nella versione 7.08 del programma
            let versioneArray = info.versione.split(separator:".").map{String($0)}
            if ((Int(versioneArray[0]) ?? 0) >= 8 || ((Int(versioneArray[0]) ?? 0) >= 7 && (Int(versioneArray[1]) ?? 0) >= 8)) {
                try info.autore = readString();
            }
            
            info.casaEditrice = try readString();
            info.data = try readString();
            info.copyright = try readString();
            info.isbn = try readString();
            info.descrizione = try readString();
            info.lingua = try readString();
            info.versioneDelleNote = try readString();
            
            // il campo sul bloccaggio è stato introdotto nella versione 7.08 del programma
            if ((Int(versioneArray[0]) ?? 0) >= 8 || ((Int(versioneArray[0]) ?? 0) >= 7 && (Int(versioneArray[1]) ?? 0) >= 8)) {
                info.bloccato = try BloccatoTipi(rawValue: readByte()) ?? BloccatoTipi.Sbloccato
            }
            
            tipo = try readByte()
            try handle?.seek(toOffset:UInt64(pInizioDati))
            try pTesto = readUInt32() + pInizioDati;
            var pIndiceLibriCapitoli:UInt32 = 0
            var pIndiceNote:UInt32 = 0
            switch tipo
            {
            case 0:
                pIndiceLibriCapitoli = try readUInt32() + pInizioDati;
            case 1:
                pIndiceNote = try readUInt32() + pInizioDati;
            default:
                break
            }
            
            pIndice = try readUInt32() + pInizioDati
            pParole = try readUInt32() + pInizioDati
            pParoleIndiceIndice = try readUInt32() + pInizioDati
            pParoleIndice = try readUInt32() + pInizioDati
            pRadici = try readUInt32() + pInizioDati
            pRadiciDiverse = try readUInt32() + pInizioDati
            pRiferimentiDiversi = try readUInt32() + pInizioDati
            pCitazioniRiferimenti = try readUInt32() + pInizioDati
            pNoteInOrdine = try readUInt32() + pInizioDati
            
            switch (tipo)
            {
            case 0:
                //#region Bibbia
                info.tipo = TestoTipi.Bibbia;
                try handle?.seek(toOffset:UInt64(pIndiceLibriCapitoli))
                var somma:UInt16 = 0;
                capitoliInLibro.append(0);
                indiceLibro.append(0);
                let capitoliArray = try readBytes(73);
                for i in 0...72
                {
                    capitoliInLibro.append(capitoliArray[i])
                    somma += UInt16(capitoliArray[i]);
                    indiceLibro.append(somma);
                }
                versettiInCapitolo.append(0);
                indiceCapitolo.append(0);
                let versettiArray = try readBytes(Int(somma));
                somma = 0;
                var numeroVersetto = 0;
                for i in 1...73
                {
                    for _ in stride(from:1, through:capitoliInLibro[i], by:1)
                    {
                        versettiInCapitolo.append(versettiArray[numeroVersetto])
                        somma += UInt16(versettiArray[numeroVersetto])
                        indiceCapitolo.append(somma);
                        numeroVersetto += 1;
                    }
                }
            case 1:
                //#region Note
                try handle?.seek(toOffset:UInt64(pIndiceNote))
                noteTitoli += try readString().split(separator:"|").map{String($0)}
                let numeroNote = noteTitoli.count;
                //notePosizione.capacity = numeroNote;
                var commentario = (numeroNote == 0); // collezione vuota automaticamente di tutto e due i tipi
                var dizionario = (numeroNote == 0);
                for i in stride(from:0, to:numeroNote, by:1) {
                    if (noteTitoli[i].hasPrefix("#")) {
                        commentario = true;
                    }
                    else {
                        dizionario = true;
                    }
                    notePosizione.append(i);
                }
                if (commentario) {
                    info.tipo = TestoTipi.Commentario
                }
                if (dizionario) {
                    info.tipo = TestoTipi(rawValue: info.tipo.rawValue | TestoTipi.Dizionario.rawValue) ?? TestoTipi.None
                }
            default:
                throw FileNonValidoException.fileNonValido;
            }
        }
        catch {
            info.nome = ""
            return
        }
        
        do {
            if (pRadiciDiverse > pInizioDati) {
                try handle?.seek(toOffset:UInt64(pRadiciDiverse))
                let nRadiciDiverse = try readUInt32();
                switch (tipo)
                {
                case 0:
                    var riferimento:[UInt8]
                    var riferimento6:[UInt8] = [0,0,0,0,0,0];
                    var versetto:[UInt16]
                    for _ in stride(from:0, to:nRadiciDiverse, by:1) {
                        riferimento = try readBytes(3);
                        for j in 0...2
                        {
                            riferimento6[j] = riferimento[j];
                            riferimento6[j + 3] = riferimento[j];
                        }
                        versetto = numeroVersettoDaRiferimento(riferimento6);
                        let op = OccorrenzaParola(UInt(versetto[0]), try readUInt16());
                        let rd = RadiceDiversa(op, try readString());
                        radiciDiverse.append(rd);
                    }
                    break;
                case 1:
                    for _ in stride(from:0, to:nRadiciDiverse, by:1)
                    {
                        let op = OccorrenzaParola(UInt(try readUInt32()), try readUInt16());
                        let rd = RadiceDiversa(op, try readString());
                        radiciDiverse.append(rd);
                    }
                    break;
                default:
                    break
                }
            }
            
            if (pRiferimentiDiversi > pInizioDati) { // quando ==, non ci sono riferimenti diversi in questa versione
                try handle?.seek(toOffset:UInt64(pRiferimentiDiversi))
                let nRiferimentiDiversi = try readUInt32();
                for _ in stride(from:0, to:nRiferimentiDiversi, by:1) {
                    riferimentiDiversi.append([ try readInt16(), try readInt16(), try readInt16(), try readInt16(), try readInt16(), try readInt16() ]);
                }
            }
            
            if (pNoteInOrdine > pInizioDati) // quando ==, non ci sono note in ordine
            {
                try handle?.seek(toOffset:UInt64(pNoteInOrdine))
                let nNoteInOrdine = try readUInt32();
                for _ in stride(from:0, to:nNoteInOrdine, by:1) {
                    noteInOrdine.append(try readString())
                }
                if (nNoteInOrdine > 0) {
                    info.tipo = TestoTipi(rawValue:(info.tipo.rawValue | TestoTipi.Libro.rawValue)) ?? TestoTipi.Libro
                }
            }
        }
        catch
        {
            throw FileNonValidoException.fileNonValido
        }
    }
    
    private func confrontaVersioni(_ v1:String, _ v2:String) -> Int
    {
        let v1a = v1.split(separator:".").map{String($0)}
        let v2a = v2.split(separator:".").map{String($0)}
        if (v1a.count < 3 || v2.count < 3) {
            return 0;
        }
        if ((Int(v1a[0]) ?? 0) < (Int(v2a[0]) ?? 0)) {
            return -1;
        }
        if ((Int(v1a[0]) ?? 0) > (Int(v2a[0]) ?? 0)) {
            return 1;
        }
        if ((Int(v1a[1]) ?? 0) < (Int(v2a[1]) ?? 0)) {
            return -1;
        }
        if ((Int(v1a[1]) ?? 0) > (Int(v2a[1]) ?? 0)) {
            return 1;
        }
        /*if (Int32.Parse(v1a[2]) < v2[2])
         return -1;
         if (Int32.Parse(v1a[2]) > v2[2])
         return 1;*/ // va bene se solo l'ultimo numero è diverso
        return 0;
    }
    
    internal func creaListaRadiceDiParole()
    {
        if (_radiceDiParola.count == 0) {
            let numeroParole = parole().count;
            let numeroRadici = radici().count; // serve solo per costringere la lettura delle radici, che imposta pRadiciDiParole correttamente
            _radiceDiParola = Array(repeating:UInt32(0), count:numeroParole)
            do {
                if (numeroRadici > 0 && pRadiciDiParole > 0) { // quando pRadiciDiParole==0 (valore predefinito), non ci sono radici in questa versione
                    // numeroRadici>0 quindi non è necessario, ma è incluso per fare sì che la riga che definisce numeroRadici è usata
                    semaphore.wait()
                    try handle?.seek(toOffset:pRadiciDiParole)
                    let radiciArray = try readBytes(numeroParole * 4);
                    semaphore.signal()
                    var i4 = 0
                    for i in stride(from:0, to:numeroParole, by:1) {
                        i4 = 4 * i;
                        _radiceDiParola[i] = (UInt32)(256 * (256 * (256 * UInt32(radiciArray[i4 + 3]) + UInt32(radiciArray[i4 + 2])) + UInt32(radiciArray[i4 + 1])) + UInt32(radiciArray[i4]));
                    }
                }
            }
            catch { semaphore.signal() }
        }
    }
    /*
     internal void CreaListaCitazioni()
     {
     lock.lock()
     defer { lock.unlock()}
     if (citazioniRiferimenti == null)
     {
     citazioniRiferimenti = new List<CitazioneRiferimento>();
     if (pCitazioniRiferimenti > pInizioDati) // quando ==, non ci sono collegamenti a riferimenti
     {
     fs.Seek(pCitazioniRiferimenti, SeekOrigin.Begin);
     UInt32 nCitazioniRiferimenti = br.ReadUInt32();
     //                        byte[] brano = new byte[6];
     //                        UInt32 numeroNota;
     CitazioneRiferimento citazione;
     int i10;
     byte[] citazioniArray = br.ReadBytes(10 * (int)nCitazioniRiferimenti);
     for (int i = 0; i < nCitazioniRiferimenti; ++i)
     {
     i10 = 10 * i;
     citazione.Brano = new byte[6] { citazioniArray[i10 + 0], citazioniArray[i10 + 1], citazioniArray[i10 + 2], citazioniArray[i10 + 3], citazioniArray[i10 + 4], citazioniArray[i10 + 5] };
     citazione.NumeroNota = (UInt32)(256 * (256 * (256 * citazioniArray[i10 + 9] + citazioniArray[i10 + 8]) + citazioniArray[i10 + 7]) + citazioniArray[i10 + 6]);
     //                            brano = br.ReadBytes(6);
     //                            numeroNota = br.ReadUInt32();
     //citazione.Brano = brano;
     //                            citazione.NumeroNota = numeroNota;
     citazioniRiferimenti.Add(citazione);
     }
     }
     }
     }
     */
    //#region Chiusura
    
    /*func Rimuovi() // usato Chiudi invece
     {
     // chiudi il file
     try
     {
     br.Close();
     }
     catch { }
     try
     {
     fs.Close();
     }
     catch { }
     }
     */
    func cancella() -> Bool
    {
        // chiude il testo e cancella il file che lo contiene
        var successo = true
        chiudi();
        do {
            try genitore.fileManager.removeItem(atPath: info.nomeDelFile)
        } catch {
            successo = false
        }
        return successo
    }
    
    public func chiudi()
    {
        //let nomeVersione = info.nome;
        //let nomeFile = nomeVersione;
        /*bool successoScrittura = true;
         *
         if (noteModificate)
         {
         FileStream fsNuovo = null;
         BinaryWriter bwNuovo = null;
         
         SortedDictionary<string, List<OccorrenzaParola>> chiave = new SortedDictionary<string, List<OccorrenzaParola>>(confrontoParole);
         
         try
         {
         int suffisso = 0;
         while (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeFile + ".laparola"))
         {
         suffisso += 1;
         nomeFile = nomeVersione + suffisso.ToString();
         }
         nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeFile + ".laparola";
         
         fsNuovo = new FileStream(nomeFile, FileMode.Create, FileAccess.Write);
         bwNuovo = new BinaryWriter(fsNuovo);
         bwNuovo.Write(new char[] { 'L', 'P', 'N', System.Convert.ToChar(Assembly.GetExecutingAssembly().GetName().Version.Major), System.Convert.ToChar(Assembly.GetExecutingAssembly().GetName().Version.Minor), System.Convert.ToChar(Assembly.GetExecutingAssembly().GetName().Version.Build), (char)1 });
         bwNuovo.Write((UInt32)11);
         bwNuovo.Write((UInt32)0);
         
         bwNuovo.Write(nomeVersione);
         bwNuovo.Write(info.Abbreviazione);
         bwNuovo.Write(info.Titolo);
         bwNuovo.Write(info.Autore);
         bwNuovo.Write(info.CasaEditrice);
         bwNuovo.Write(info.Data);
         bwNuovo.Write(info.Copyright);
         bwNuovo.Write(info.Isbn);
         bwNuovo.Write(info.Descrizione);
         bwNuovo.Write(info.Lingua);
         bwNuovo.Write(info.VersioneDelleNote);
         bwNuovo.Write((byte)(info.Bloccato));
         bwNuovo.Write(Convert.ToByte(1)); // sempre una collezione di note
         UInt32 pInizioDati = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current));
         bwNuovo.Seek(11, SeekOrigin.Begin);
         bwNuovo.Write(pInizioDati);
         bwNuovo.Seek(0, SeekOrigin.End);
         
         UInt32 inizioTestoIndiceLC = 0, inizioTestoIndice = 0;
         //                        UInt32 inizioTesto = pInizioDati + 44; // '44' va cambiato qui, nella riga successiva, e 2 volte in ImportaBibbia.cs
         bwNuovo.Write((UInt32)44); // inizio del testo
         bwNuovo.Write((UInt32)0); // inizio indici libri e capitoli/inizio titoli note
         bwNuovo.Write((UInt32)0); // inizio indice versetti/note
         bwNuovo.Write((UInt32)0); // inizio elenco parole
         bwNuovo.Write((UInt32)0); // inizio indice dell'indice delle parole
         bwNuovo.Write((UInt32)0); // inizio indice delle parole
         bwNuovo.Write((UInt32)0); // inizio elenco radici
         bwNuovo.Write((UInt32)0); // inizio elenco radici diverse
         bwNuovo.Write((UInt32)0); // inizio elenco differenze nei riferimenti
         bwNuovo.Write((UInt32)0); // inizio indice dei riferimenti citati
         bwNuovo.Write((UInt32)0); // inizio note in ordine
         
         int numeroNote = noteTitoli.Count;
         string[] nuoviTesti = new string[numeroNote];
         
         RichTextBoxEx rtb = new RichTextBoxEx();
         for (UInt32 i = 0; i < numeroNote; ++i)
         {
         nuoviTesti[i] = GetNotaTesto(noteTitoli[(int)i]);
         try
         {
         rtb.Rtf = nuoviTesti[i];
         chiave = TrovaParoleInVoce(rtb.Text, i, chiave, info.Lingua);
         }
         catch
         {
         chiave = TrovaParoleInVoce(nuoviTesti[i], i, chiave, info.Lingua);
         }
         }
         UInt32[] indici = new UInt32[2];
         indici = ScriviNote(bwNuovo, pInizioDati, noteTitoli.ToArray(), nuoviTesti);
         inizioTestoIndiceLC = indici[0];
         inizioTestoIndice = indici[1];
         
         UInt32 inizioParole = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         StringBuilder parole = new StringBuilder("");
         foreach (string s in chiave.Keys)
         parole.Append(s).Append("|");
         bwNuovo.Write(parole.ToString());
         
         UInt32 inizioParoleIndiceIndice = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         UInt32 numeroApparenze = 0;
         byte[] datiDaScrivere = new byte[4 * chiave.Count + 4];
         MemoryStream ms = new MemoryStream(datiDaScrivere, true);
         BinaryWriter bwMemoria = new BinaryWriter(ms);
         bwMemoria.Write((UInt32)0);
         foreach (List<OccorrenzaParola> lista in chiave.Values)
         {
         numeroApparenze += (UInt32)(lista.Count);
         bwMemoria.Write(6 * numeroApparenze);
         }
         bwMemoria.Seek(0, SeekOrigin.Begin);
         bwNuovo.Write(datiDaScrivere);
         
         UInt32 inizioParoleIndice = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         foreach (List<OccorrenzaParola> lista in chiave.Values)
         {
         byte[] datiDaScrivereParoleIndice = new byte[lista.Count * 6];
         MemoryStream msParoleIndice = new MemoryStream(datiDaScrivereParoleIndice, true);
         BinaryWriter bwMemoriaParoleIndice = new BinaryWriter(msParoleIndice);
         foreach (OccorrenzaParola op in lista)
         {
         bwMemoriaParoleIndice.Write(op.Voce);
         bwMemoriaParoleIndice.Write(op.Parola);
         }
         bwMemoriaParoleIndice.Seek(0, SeekOrigin.Begin);
         bwNuovo.Write(datiDaScrivereParoleIndice);
         }
         
         UInt32 inizioRadici = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         int numeroRadici = Radici.Length;
         if (numeroRadici > 0)
         {
         StringBuilder listaRadici = new StringBuilder("");
         for (int i = 0; i < numeroRadici; ++i)
         listaRadici.Append(Radici[i]).Append("|");
         bwNuovo.Write(listaRadici.ToString());
         foreach (string s in chiave.Keys)
         bwNuovo.Write(RadiceNumeroDiParola(s));
         }
         else
         inizioRadici = 0;
         
         UInt32 inizioRadiciDiverse = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         int numeroRadiciDiverse = radiciDiverse.Count;
         if (numeroRadiciDiverse > 0)
         {
         bwNuovo.Write((UInt32)numeroRadiciDiverse);
         for (int i = 0; i < numeroRadiciDiverse; ++i)
         {
         bwNuovo.Write(radiciDiverse[i].OccorrenzaRadice.Voce);
         bwNuovo.Write(radiciDiverse[i].OccorrenzaRadice.Parola);
         bwNuovo.Write(radiciDiverse[i].NuovaRadice);
         }
         
         }
         else
         inizioRadiciDiverse = 0;
         
         UInt32 inizioRiferimentiDiversi = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         int numeroRiferimentiDiversi = riferimentiDiversi.Count;
         if (numeroRiferimentiDiversi > 0)
         {
         bwNuovo.Write((UInt32)numeroRiferimentiDiversi);
         for (int i = 0; i < numeroRiferimentiDiversi; ++i)
         for (int j = 0; j < 6; ++j)
         bwNuovo.Write(riferimentiDiversi[i][j]);
         }
         else
         inizioRiferimentiDiversi = 0;
         
         UInt32 inizioRiferimentiCitati = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         if (!genitore.ScriviRiferimentiCitati(bwNuovo, nuoviTesti))
         inizioRiferimentiCitati = 0;
         
         UInt32 inizioNoteInOrdine = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
         int numeroNoteInOrdine = noteInOrdine.Count;
         if (numeroNoteInOrdine > 0)
         {
         bwNuovo.Write((UInt32)numeroNoteInOrdine);
         for (int i = 0; i < numeroNoteInOrdine; ++i)
         bwNuovo.Write(noteInOrdine[i]);
         }
         else
         inizioNoteInOrdine = 0;
         
         bwNuovo.Seek((int)pInizioDati + 4, SeekOrigin.Begin);
         bwNuovo.Write(inizioTestoIndiceLC);
         bwNuovo.Write(inizioTestoIndice);
         bwNuovo.Write(inizioParole);
         bwNuovo.Write(inizioParoleIndiceIndice);
         bwNuovo.Write(inizioParoleIndice);
         bwNuovo.Write(inizioRadici);
         bwNuovo.Write(inizioRadiciDiverse);
         bwNuovo.Write(inizioRiferimentiDiversi);
         bwNuovo.Write(inizioRiferimentiCitati);
         bwNuovo.Write(inizioNoteInOrdine);
         bwNuovo.Seek(0, SeekOrigin.End);
         }
         catch
         {
         successoScrittura = false;
         }
         finally
         {
         if (bwNuovo != null)
         bwNuovo.Close();
         if (fsNuovo != null)
         fsNuovo.Close();
         }
         
         if (successoScrittura)
         {
         try
         {
         br.Close();
         }
         catch { }
         try
         {
         fs.Close();
         }
         catch { }
         try
         {
         if (nomeFile != info.NomeDelFile)
         {
         File.Delete(info.NomeDelFile);
         File.Move(nomeFile, info.NomeDelFile);
         }
         }
         catch
         {
         throw new ImpossibileScrivereModificheException();
         }
         }
         else
         {
         throw new ImpossibileScrivereModificheException();
         }
         }*/
        do {
            if handle != nil
            {
                try handle?.close()
            }
        }
        catch {
        }
    }
    
    //#region Riferimento
    
    private func riferimentoDaNumeroVersetto(_ numeroVersetto:UInt) -> [UInt8]
    {
        var libro:Int = 0;
        var capitolo:Int = 0;
        repeat {
            capitolo += 1
        } while (indiceCapitolo[capitolo] < numeroVersetto);
        repeat {
            libro += 1
        } while (indiceLibro[libro] < capitolo);
        
        let b1 = UInt8(capitolo - Int(indiceLibro[libro - 1]));
        let b2 = UInt8(Int(numeroVersetto) - Int(indiceCapitolo[capitolo - 1]));
        let rif = [ UInt8(libro), b1, b2, UInt8(libro), b1, b2 ]
        return rif;
    }
    
    func numeroVersettoDaRiferimento(_ riferimento:[UInt8]) -> [UInt16]
    {
        var b1 = riferimento[1];
        if (b1 > capitoliInLibro[Int(riferimento[0])]) {
            b1 = capitoliInLibro[Int(riferimento[0])];
        }
        var b2 = riferimento[2];
        if (b2 > versettiInCapitolo[Int(indiceLibro[Int(riferimento[0] - 1)]) + Int(b1)]) {
            b2 = versettiInCapitolo[Int(indiceLibro[Int(riferimento[0] - 1)]) + Int(b1)];
        }
        var b4 = riferimento[4];
        if (b4 > capitoliInLibro[Int(riferimento[3])]) {
            b4 = capitoliInLibro[Int(riferimento[3])];
        }
        var b5 = riferimento[5];
        if (b5 > versettiInCapitolo[Int(indiceLibro[Int(riferimento[3] - 1)]) + Int(b4)]) {
            b5 = versettiInCapitolo[Int(indiceLibro[Int(riferimento[3] - 1)]) + Int(b4)];
        }
        let inizio = indiceCapitolo[Int(indiceLibro[Int(riferimento[0] - 1)]) + Int(b1) - 1] + UInt16(b2);
        let fine = indiceCapitolo[Int(indiceLibro[Int(riferimento[3] - 1)]) + Int(b4) - 1] + UInt16(b5);
        
        return [inizio, fine];
    }
    
    //#region Ricerca
    
    public func ricercaRadiceInBrano(_ radice:String, _ branoDaRicercare:Riferimento) -> Riferimento
    {
        // se branoDaRicerca non contiene brani, tutta la Bibbia (o collezione di note) è ricercata
        if (branoDaRicercare.brani.count == 0) {
            return ricercaRadiceInBrano(radice);
        }
        else {
            return restringiRiferimentoABrano(occorrenzeRadice(radice), branoDaRicercare);
        }
    }
    
    public func ricercaRadiceInBrano(_ radice:String) -> Riferimento
    {
        return convertiOccorrenzeARiferimento(occorrenzeRadice(radice));
    }
    
    private func occorrenzeRadice(_ radice:String) -> [OccorrenzaParola]
    {
        var occorrenze:[OccorrenzaParola] = []
        let paroleDaRicercare = paroleNumeriDiRadice(radice).split(separator:"|", omittingEmptySubsequences: true).map{String($0)}
        for parolaDaRicercare in paroleDaRicercare {
            occorrenze.append(contentsOf:occorrenzeParola(Int(parolaDaRicercare) ?? 0, true));
        }
        occorrenze.append(contentsOf:occorrenzeRadiceDiversa(radice));
        occorrenze.sort();
        return occorrenze;
    }
    
    /// <summary>
    /// Trova tutti i versetti in un brano che contengono una parola.
    /// Se la parola non esiste nella versione, un riferimento vuoto è restituito.
    /// </summary>
    /// <param name="parola">La parola da ricercare.</param>
    /// <param name="branoDaRicercare">Il brano in cui cercare la parola.</param>
    /// <returns>Il riferimento di tutti i versetti.</returns>
    public func ricercaParolaInBrano(_ parola:String, _ branoDaRicercare:Riferimento) -> Riferimento
    {
        // se branoDaRicerca non contiene brani, tutta la Bibbia (o collezione di note) è ricercata
        if (branoDaRicercare.brani.count == 0) {
            return ricercaParolaInBrano(parola);
        }
        //return Riferimento()
        return restringiRiferimentoABrano(ricercaParola(parola), branoDaRicercare);
    }
    
    /// <summary>
    /// Trova tutti i versetti nella Bibbia che contengono una parola.
    /// Se la parola non esiste nella versione, un riferimento vuoto è restituito.
    /// </summary>
    /// <param name="parola">La parola da ricercare.</param>
    /// <returns>Il riferimento di tutti i versetti.</returns>
    public func ricercaParolaInBrano(_ parola:String) -> Riferimento
    {
        //return Riferimento()
        return convertiOccorrenzeARiferimento(ricercaParola(parola));
    }
    
    private func restringiRiferimentoABrano(_ occorrenze:[OccorrenzaParola], _ branoDaRicercare:Riferimento) -> Riferimento
    {
        var occorrenzeInBrano:Riferimento = Riferimento((info.tipo.rawValue & TestoTipi.Bibbia.rawValue) == TestoTipi.Bibbia.rawValue);
        let numeroBrani = branoDaRicercare.brani.count;
        for op in occorrenze {
            if (occorrenzeInBrano.versetti) {
                var inizioBrani:[UInt16] = []
                var fineBrani:[UInt16] = []
                var numeroVersetto:[UInt16];
                for b in branoDaRicercare.brani {
                    numeroVersetto = numeroVersettoDaRiferimento(b);
                    inizioBrani.append(numeroVersetto[0]);
                    fineBrani.append(numeroVersetto[1]);
                }
                for i in stride(from:0, to:numeroBrani, by:1) {
                    if (inizioBrani[i] <= op.voce && fineBrani[i] >= op.voce)
                    {
                        occorrenzeInBrano.brani.append(riferimentoDaNumeroVersetto(op.voce));
                        occorrenzeInBrano.numeroParola.append([op.parola]);
                        break;
                    }
                }
            }
            else
            {
                var nomeNota = ""
                var libro:UInt8, capitolo:UInt8, versetto:UInt8
                for i in stride(from:0, to:numeroBrani, by:1) {
                    nomeNota = noteTitoli[Int(op.voce)];
                    if (nomeNota.hasPrefix("#")) // altrimenti fa parte di un dizionario
                    {
                        libro = UInt8(nomeNota[1..<3]) ?? 0;
                        capitolo = UInt8(nomeNota[3..<6]) ?? 0;
                        versetto = UInt8(nomeNota[6..<9]) ?? 0;
                        if ((branoDaRicercare.brani[i][0] < libro
                             || (branoDaRicercare.brani[i][0] == libro && branoDaRicercare.brani[i][1] < capitolo)
                             || (branoDaRicercare.brani[i][0] == libro && branoDaRicercare.brani[i][1] == capitolo && branoDaRicercare.brani[i][2] <= versetto))
                            &&
                            (branoDaRicercare.brani[i][3] > libro
                             || (branoDaRicercare.brani[i][3] == libro && branoDaRicercare.brani[i][4] > capitolo)
                             || (branoDaRicercare.brani[i][3] == libro && branoDaRicercare.brani[i][4] == capitolo && branoDaRicercare.brani[i][5] >= versetto))) {
                            if op.voce < noteTitoli.count { // il contrario è possibile se una nota è stata cancellata
                                occorrenzeInBrano.note.append(noteTitoli[Int(op.voce)]);
                                occorrenzeInBrano.numeroParola.append([op.parola]);
                            }
                        }
                    }
                }
            }
        }
        return occorrenzeInBrano;
    }
    
    private func convertiOccorrenzeARiferimento(_ occorrenze:[OccorrenzaParola]) -> Riferimento
    {
        var occorrenzeInBibbia:Riferimento = Riferimento((info.tipo.rawValue & TestoTipi.Bibbia.rawValue) == TestoTipi.Bibbia.rawValue);
        for op in occorrenze {
            if (occorrenzeInBibbia.versetti) {
                occorrenzeInBibbia.brani.append(riferimentoDaNumeroVersetto(op.voce));
                occorrenzeInBibbia.numeroParola.append([op.parola]);
            }
            else
            {
                if op.voce < noteTitoli.count { // il contrario è possibile se una nota è stata cancellata
                    occorrenzeInBibbia.note.append(noteTitoli[Int(op.voce)]);
                    occorrenzeInBibbia.numeroParola.append([op.parola]);
                }
            }
        }
        return occorrenzeInBibbia;
    }
    
    private func ricercaParola(_ parolaRicercata:String) -> [OccorrenzaParola]
    {
        var parola = parolaRicercata
        creaListaRadiceDiParole();
        
        var occorrenze: [OccorrenzaParola] = []
        var cercaRadice = false
        var cercaRadiceDiParola = false;
        
        if (parola.hasPrefix("\\")) // tutte le parole con la stessa radice della parola
        {
            cercaRadiceDiParola = true;
            cercaRadice = true; // perché la ricerca sarà convertita in /(radice della parola)
            parola = parola[1...];
        }
        if (parola.hasPrefix("/")) // tutte le parole della radice
        {
            cercaRadice = true;
            parola = parola[1...];
        }
        if (parola.indexOf("*") > -1 || parola.indexOf("?") > -1)
        {
            do {
                let regExpParola = try Regex("^" + parola.replacingOccurrences(of:"?", with:".").replacingOccurrences(of:"*", with:".*") + "$");
                let numeroDiParole = parole().count;
                for i in stride(from:0, to:numeroDiParole, by:1) {
                    if (parole()[i].contains(regExpParola)) {
                        var radiceDaRicercare = parole()[i];
                        if (cercaRadiceDiParola) {
                            radiceDaRicercare = radici()[Int(_radiceDiParola[i])];
                        }
                        if (cercaRadice) {
                            let paroleDaRicercare = paroleNumeriDiRadice(radiceDaRicercare).split(separator:"|", omittingEmptySubsequences: true).map{String($0)}
                            for parolaDaRicercare in paroleDaRicercare {
                                occorrenze.append(contentsOf:occorrenzeParola(Int(parolaDaRicercare) ?? -1, true));
                            }
                            occorrenze.append(contentsOf:occorrenzeRadiceDiversa(radiceDaRicercare));
                        }
                        else {
                            occorrenze.append(contentsOf:occorrenzeParola(i));
                        }
                    }
                }
            }
            catch {}
        }
        else if (!parola.isEmpty)
        {
            if (cercaRadiceDiParola)
            {
                if (radici().count > 0)
                {
                    let numeroParola = numeroDiParola(parola);
                    if (numeroParola >= 0) {
                        parola = radici()[Int(_radiceDiParola[numeroParola])];
                    }
                    else {
                        parola = ""; // parola non esiste in questo testo
                    }
                }
                else {
                    // cerchiamo "parola" anche quando la ricerca è per \parola
                    cercaRadice = false;
                }
            }
            if (cercaRadice)
            {
                let paroleDaRicercare = paroleNumeriDiRadice(parola).split(separator:"|", omittingEmptySubsequences: true).map{String($0)}
                for parolaDaRicercare in paroleDaRicercare {
                    occorrenze.append(contentsOf:occorrenzeParola(Int(parolaDaRicercare) ?? -1, true));
                }
                occorrenze.append(contentsOf:occorrenzeRadiceDiversa(parola));
            }
            else {
                occorrenze.append(contentsOf:occorrenzeParola(numeroDiParola(parola))); // anche se negativo, funziona perché OccorrenzeParola resitutisce niente
            }
        }
        
        occorrenze.sort();
        return occorrenze;
    }
    
    private func occorrenzeRadiceDiversa(_ radice:String) -> [OccorrenzaParola]
    {
        // restituisce una lista con tutte le occorrenze di una radice quando non è la radice normale della parola
        var occorrenze:[OccorrenzaParola] = []
        for i in stride(from:0, to:radiciDiverse.count, by:1) {
            if (radiciDiverse[i].nuovaRadice.lowercased() == radice) {
                occorrenze.append(radiciDiverse[i].occorrenzaRadice);
            }
        }
        return occorrenze;
    }
    
    private func occorrenzeParola(_ nParola:Int, _ solaRadiceNormale:Bool) -> [OccorrenzaParola]
    {
        // restituisce una lista con tutte le occorrenze di una parola; con la radice normale oppure solo quando non c'è una radice diversa
        
        creaListaRadiceDiParole();
        
        var occorrenze:[OccorrenzaParola] = []
        if (nParola >= 0) {
            var nByte = 0
            var occArray:[UInt8] = []
            semaphore.wait()
            
            do {
                try handle?.seek(toOffset:UInt64(Int(pParoleIndiceIndice) + 4 * nParola))
                let inizioVersetti = try readUInt32()
                let fineVersetti = try readUInt32()
                try handle?.seek(toOffset:UInt64(pParoleIndice + inizioVersetti))
                nByte = Int(fineVersetti - inizioVersetti)
                occArray = try readBytes(nByte)
            }
            catch {}
            
            semaphore.signal()
            
            let nOccorrenze = nByte / 6; // 6 perché ogni occorrenza prende 6 byte (UInt32 + UInt16)
            var radice = "";
            if (solaRadiceNormale) {
                radice = radiceDiParola(parole()[nParola]);
            }
            for i in stride(from:0, to:nOccorrenze, by:1) {
                var op = OccorrenzaParola();
                op.voce = UInt(16777216 * UInt(occArray[6 * i + 3]) + 65536 * UInt(occArray[6 * i + 2]) + 256 * UInt(occArray[6 * i + 1]) + UInt(occArray[6 * i]));
                op.parola = UInt16(256 * UInt16(occArray[6 * i + 5]) + UInt16(occArray[6 * i + 4]));
                if (!solaRadiceNormale) {
                    occorrenze.append(op);
                }
                else {
                    var radiceEDiversa = false;
                    for j in stride(from:0, to:radiciDiverse.count, by:1) {
                        if (radiciDiverse[j].occorrenzaRadice.compareTo(op) == 0)
                        {
                            radiceEDiversa = (radiciDiverse[j].nuovaRadice != radice);
                            if (radiceEDiversa) {
                                break;
                            }
                        }
                    }
                    if (!radiceEDiversa) {
                        occorrenze.append(op);
                    }
                }
            }
        }
        return occorrenze;
    }
    
    private func occorrenzeParola(_ nParola:Int) -> [OccorrenzaParola]
    {
        // restituisce una lista con tutte le occorrenze di una parola
        return occorrenzeParola(nParola, false);
    }
    
    //#region Parole e Radici
    
    public func esistonoRadici() -> Bool
    {
        return (radici().count > 0);
    }
    
    private func numeroDiParola(_ parola:String) -> Int
    {
        if (parola.isEmpty) {
            return -1;
        }
        else { // BinarySearch non funziona sempre con parole greche, neanche con confrontoParole
            //var n = parole().bisectToFirstIndex{$0.localizedStandardCompare(parola) != .orderedAscending} ?? -1
            var n = parole().binarySearch(for: parola) ?? -1
            if n < 0 { // se il binary search non lo trova, in Windows succedeva con parole greche
                n = parole().firstIndex(of: parola) ?? -1
            }
            return n
        }
    }
    
    public func numeroVolteParola(_ parola:String) -> Int
    {
        let numeroVolte:Int;
        let numeroParola = numeroDiParola(parola);
        if (numeroParola >= 0) {
            semaphore.wait()
            defer { semaphore.signal()}
            do {
                try handle?.seek(toOffset:UInt64(Int(pParoleIndiceIndice) + 4 * numeroParola))
                //fs.Seek(pParoleIndiceIndice + 4 * numeroParola, SeekOrigin.Begin);
                let inizioVersetti = try readUInt32();
                numeroVolte = Int((try readUInt32() - inizioVersetti) / 6)
            }
            catch {
                numeroVolte = 0
            }
        }
        else {
            numeroVolte = 0;
        }
        return numeroVolte;
    }
    
    internal func getApparenzeParole() -> [UInt8]
    {
        do {
            try handle?.seek(toOffset:UInt64(pParoleIndice - 4))
            let count = try readUInt32();
            return try readBytes(Int(count));
        }
        catch {
            return []
        }
    }
    
    /// <summary>
    /// Il numero di occorrenze delle parole che hanno questa radice, cioè non considera quando una di queste
    /// parole ha una radice diversa, oppure parole con altre radici con questa come radice diversa.
    /// </summary>
    /// <param name="radice">La radice di cui si vuole il numero di occorrenze.</param>
    /// <returns>Il numero di occorrenze.</returns>
    public func numeroVolteRadice(_ radice:String) -> Int
    {
        let paroleNumeri = paroleNumeriDiRadice(radice).split(separator:"|", omittingEmptySubsequences: true).map{String($0)}
        var numeroVolte:UInt32 = 0;
        var inizioVersetti:UInt32
        do {
            semaphore.wait()
            for parolaNumero in paroleNumeri
            {
                try handle?.seek(toOffset:UInt64(pParoleIndiceIndice + 4 * (UInt32(parolaNumero) ?? 0)))
                inizioVersetti = try readUInt32();
                numeroVolte += (try readUInt32() - inizioVersetti) / 6;
            }
            semaphore.signal()
            for radiceDiversa in radiciDiverse
            {
                if (radiceDiversa.nuovaRadice == radice) {
                    numeroVolte += 1;
                }
            }
            return Int(numeroVolte)
        }
        catch {
            semaphore.signal()
            return 0
        }
    }
    
    public func radiceDiParola(_ parola:String) -> String
    {
        // la radice normale, non un'eventuale radice diversa
        
        if (radici().count == 0) {
            return "";
        }
        
        creaListaRadiceDiParole();
        
        let numeroParola = numeroDiParola(parola);
        return ((numeroParola >= 0) ? radici()[Int(_radiceDiParola[numeroParola])] : "");
    }
    
    public func radiceNumeroDiParola(_ parola:String) -> UInt32
    {
        // la radice normale, non un'eventuale radice diversa
        if (radici().count == 0) {
            return UInt32(radici().firstIndex(of: "*") ?? 0)
            //return (UInt32)(Array.BinarySearch(radici(), "*", confrontoParole));
        }
        
        creaListaRadiceDiParole();
        
        let numeroParola = numeroDiParola(parola);
        return ((numeroParola >= 0) ? (_radiceDiParola[numeroParola]) : UInt32(radici().firstIndex(of: "*") ?? 0));
    }
    
    public func paroleDiRadice(_ radice:String) -> [String]
    {
        // le parole che solitamente hanno questa radice, non altre parole che la hanno a volte come radice diversa
        let paroleNumeri = paroleNumeriDiRadice(radice).split(separator:"|", omittingEmptySubsequences: true).map{String($0)}
        var paroleDiRadice:[String] = []
        for rn in paroleNumeri {
            paroleDiRadice.append(parole()[Int(rn) ?? 0]);
        }
        return paroleDiRadice;
    }
    
    private func paroleNumeriDiRadice(_ radice:String) -> String
    {
        // restituisce tutte le parole di una certa radice - restituisce una stringa con i numeri delle parole separati da |
        //let numeroRadice = radici().bisectToFirstIndex{$0.localizedStandardCompare(radice) != .orderedAscending} ?? -1
        let numeroRadice = radici().binarySearch(for: radice) ?? -1
        if (numeroRadice >= 0) { // se non ci sono radici nella versione, il resto non viene eseguito
            if (_paroleDiRadice.count == 0) {
                // siccome la creazione di paroleDiRadice richiede un po' di tempo, lo facciamo solo la prima volta che è necessario
                creaListaRadiceDiParole();
                
                let numeroRadici = radici().count
                for _ in stride(from:0, to:numeroRadici, by:1) {
                    _paroleDiRadice.append("")
                }
                
                let numeroParole = parole().count;
                for i in stride(from:0, to:numeroParole, by:1) {
                    _paroleDiRadice[Int(_radiceDiParola[i])] += String(i)+"|";
                }
            }
            return _paroleDiRadice[numeroRadice];
        }
        else {
            return "";
        }
    }
    
    /*
     public void AggiungiRadiciAllaVersione(string[] elencoRadici, string[] radiceStringaDiParole)
     {
     CreaListaRadiceDiParole();
     
     if (radici == null)
     radici = new string[elencoRadici.Length];
     else
     Array.Resize(ref radici, elencoRadici.Length);
     elencoRadici.CopyTo(radici, 0);
     int numeroParole = parole.Length;
     for (int i = 0; i < numeroParole; ++i)
     _radiceDiParola[i] = (UInt32)(Array.BinarySearch(radici, radiceStringaDiParole[i], confrontoParole));
     noteModificate = true;
     _paroleDiRadice = null;
     }
     */
    
    //#region Note
    
    /// <summary>
    /// Trova una nota con un certo titolo.
    /// </summary>
    /// <param name="titolo">Il titolo da cercare.</param>
    /// <returns>Il numero della nota se esiste una nota con quel titolo, altrimenti un numero negativo.</returns>
    func getNumeroNotaTitolo(_ titolo:String) -> Int
    {
        if (titolo.isEmpty) {
            return -1;
        }
        //var numeroNota = noteTitoli.bisectToFirstIndex{$0 >= titolo} ?? -1//.firstIndex(of:titolo)//.BinarySearch(titolo, new ConfrontoCS());
        /*if (numeroNota < 0) {
         numeroNota = noteTitoli.BinarySearch(titolo, confrontoParole);
         }*/
        //return noteTitoli.bisectToFirstIndex{$0.localizedStandardCompare(titolo) != .orderedAscending} ?? -1;
        return noteTitoli.binarySearch(for: titolo) ?? -1
    }
    
    func getNotaTesto(_ titolo:String) -> String
    {
        if (titolo.isEmpty) {
            return "";
        }
        
        // prima cerchiamo la nota con esattamente lo stesso titolo, poi con lettere minuscole
        var numeroNota = getNumeroNotaTitolo(titolo);
        if (numeroNota < 0 && !titolo.hasPrefix("#") && Character(titolo[titolo.count - 1]).isWholeNumber)
        // possibilmente una nota ad un versetto, ma nel formato Mt 2:1
        {
            let noteInBrano = elencaNoteInBrano(genitore.convertiRiferimento(titolo));
            if (noteInBrano.count() > 1) { // diverse note nel brano, restituiamo il testo di tutte insieme
                return testoBrano(noteInBrano, [], []);
            }
            if (noteInBrano.count() > 0) {
                //numeroNota = noteTitoli.BinarySearch(noteInBrano.note[0], confrontoParole);
                numeroNota = noteTitoli.binarySearch(for: noteInBrano.note[0]) ?? -1
            }
        }
        
        if (numeroNota < 0)
        {
            return "";
            /* alternativa
             // se c'è una nota sullo stesso versetto, la restituiamo
             numeroNota = ~numeroNota; // la prima nota dopo quella ricercata
             if (numeroNota == NoteTitoli.Count)
             return "";
             if (!titolo.StartsWith("#")|| !NoteTitoli[numeroNota].StartsWith("#") || titolo[0..<9] != NoteTitoli[numeroNota][0..<9])
             return "";
             */
        }
        else { // numeroNota>=0
            if (notePosizione[numeroNota] >= 0)
            {
                var testo = "";
                semaphore.wait()
                defer { semaphore.signal()}
                do {
                    try handle?.seek(toOffset:UInt64(Int(pIndice) + 4 * notePosizione[numeroNota]))
                    try handle?.seek(toOffset:UInt64(pTesto + (try readUInt32())))
                    testo = try readString();
                }
                catch {} // return testo="" if error
                return testo;
            }
            else
            {
                return noteNuoveTesto[-notePosizione[numeroNota] - 1];
            }
        }
    }
    
    /*
     public void SetNotaTesto(string testo, string titolo)
     {
     noteModificate = true;
     int numeroNota = noteTitoli.BinarySearch(titolo, confrontoParole);
     if (numeroNota >= 0)
     {
     for (int i = radiciDiverse.Count - 1; i >= 0; --i)
     {
     // se nota modificata, il numero della parola nella nota non è più necessariamente giusta, e dobbiamo cancellare la voce della radice diversa
     if (radiciDiverse[i].OccorrenzaRadice.Voce == numeroNota)
     radiciDiverse.RemoveAt(i);
     }
     }
     if (String.IsNullOrEmpty(testo))
     {
     // cancella la nota
     if (numeroNota >= 0) // se <0, nota non esiste e non serve cancellarla
     {
     noteTitoli.RemoveAt(numeroNota);
     notePosizione.RemoveAt(numeroNota);
     }
     }
     else
     {
     if (numeroNota < 0)
     {
     // nuova nota
     noteNuoveTesto.Add(testo);
     noteTitoli.Insert(~numeroNota, titolo);
     notePosizione.Insert(~numeroNota, -noteNuoveTesto.Count);
     for (int i = radiciDiverse.Count - 1; i >= 0; --i)
     {
     if (radiciDiverse[i].OccorrenzaRadice.Voce >= ~numeroNota)
     {
     RadiceDiversa radiceDiversa = new RadiceDiversa();
     radiceDiversa.OccorrenzaRadice.Parola = radiciDiverse[i].OccorrenzaRadice.Parola;
     radiceDiversa.OccorrenzaRadice.Voce = radiciDiverse[i].OccorrenzaRadice.Voce + 1;
     radiceDiversa.NuovaRadice = radiciDiverse[i].NuovaRadice;
     radiciDiverse[i] = radiceDiversa;
     }
     }
     }
     else
     {
     if (notePosizione[numeroNota] >= 0)
     {
     // nota esistente, non ancora modificata
     noteNuoveTesto.Add(testo);
     notePosizione[numeroNota] = -noteNuoveTesto.Count;
     }
     else
     {
     // nota esistente e già modificata
     noteNuoveTesto[-notePosizione[numeroNota] - 1] = testo;
     }
     }
     }
     }
     
     public void SetNoteInOrdine(Collection<string> ordine)
     {
     noteInOrdine.Clear();
     noteInOrdine.append(contentsOf:ordine);
     noteModificate = true;
     }
     */
    
    func elencaNoteInBrano(_ riferimento:Riferimento) -> Riferimento
    {
        var noteInBrano = Riferimento(false)
        var libroInizio:UInt8, capitoloInizio:UInt8, versettoInizio:UInt8, libroFine:UInt8, capitoloFine:UInt8, versettoFine:UInt8;
        for titolo in noteTitoli {
            if (titolo.hasPrefix("#"))
            {
                let titoliNote = titolo.split(separator:"#").map{String($0)}
                for titoloNota in titoliNote {
                    
                    libroInizio = UInt8(titoloNota[0..<2]) ?? UInt8.max;
                    capitoloInizio = UInt8(titoloNota[2..<5]) ?? UInt8.max;
                    versettoInizio = UInt8(titoloNota[5..<8]) ?? UInt8.max;
                    if (titoloNota.count < 21) {
                        libroFine = libroInizio;
                        capitoloFine = capitoloInizio;
                        versettoFine = versettoInizio;
                    }
                    else {
                        libroFine = UInt8(titoloNota[13..<15]) ?? 0;
                        capitoloFine = UInt8(titoloNota[15..<18]) ?? 0;
                        if (capitoloFine == 0) { // tutto il libro, quindi dobbiamo garantire che il capitolo cercato sia sempre trovato
                            capitoloFine = UInt8.max;
                        }
                        versettoFine = UInt8(titoloNota[18..<21]) ?? 0;
                        if (versettoFine == 0) { // tutto il capitolo, quindi dobbiamo garantire che il capitolo cercato sia sempre trovato
                            versettoFine = UInt8.max;
                        }
                    }
                    for brano in riferimento.brani {
                        if ((brano[0] < libroFine
                             || (brano[0] == libroFine && brano[1] < capitoloFine)
                             || (brano[0] == libroFine && brano[1] == capitoloFine && brano[2] <= versettoFine))
                            &&
                            (brano[3] > libroInizio
                             || (brano[3] == libroInizio && brano[4] > capitoloInizio)
                             || (brano[3] == libroInizio && brano[4] == capitoloInizio && brano[5] >= versettoInizio)))
                        {
                            noteInBrano.note.append(titolo);
                            noteInBrano.numeroParola.append([UInt16]());
                            break;
                        }
                    }
                    
                }
            }
        }
        return noteInBrano;
    }
    
    /*
     public Boolean EsistonoCitazioni()
     {
     CreaListaCitazioni();
     return (citazioniRiferimenti.Count > 0);
     }
     
     public Collection<string> GetRiferimentiCitati()
     {
     CreaListaCitazioni();
     Collection<string> riferimentiCitati = new Collection<string>();
     int numeroCitazioniInCollezione = citazioniRiferimenti.Count;
     for (int i = 0; i < numeroCitazioniInCollezione; ++i)
     riferimentiCitati.Add(new StringBuilder().Append(citazioniRiferimenti[i].Brano[0]).Append("|").Append(citazioniRiferimenti[i].Brano[1]).Append("|").Append(citazioniRiferimenti[i].Brano[2]).Append("|").Append(citazioniRiferimenti[i].Brano[3]).Append("|").Append(citazioniRiferimenti[i].Brano[4]).Append("|").Append(citazioniRiferimenti[i].Brano[5]).Append("|").Append(citazioniRiferimenti[i].NumeroNota).Append("|").ToString());
     return riferimentiCitati;
     }
     
     public Riferimento Citazioni(Riferimento riferimento)
     {
     List<int> note = new List<int>();
     int numeroBrani = riferimento.Count;
     CreaListaCitazioni();
     int numeroCitazioniInCollezione = citazioniRiferimenti.Count;
     int posizione;
     for (int i = 0; i < numeroBrani; ++i)
     {
     for (int j = 0; j < numeroCitazioniInCollezione; ++j)
     {
     if (ConfrontaBrani(riferimento.Brani[i], citazioniRiferimenti[j].Brano) == 0)
     {
     posizione = note.BinarySearch((int)(citazioniRiferimenti[j].NumeroNota));
     if (posizione < 0) // non esiste già
     note.Insert(~posizione, (int)(citazioniRiferimenti[j].NumeroNota));
     }
     }
     }
     Riferimento citazioni = new Riferimento(false);
     foreach (int numeroNota in note)
     {
     citazioni.Note.Add(noteTitoli[numeroNota]);
     citazioni.numeroParola.Add(new List<UInt16>());
     }
     citazioni.OrdinaNote();
     return citazioni;
     }
     */
    // -1 se tutto brano1 è prima di brano2
    // 0 se si sovrappongono
    // 1 se tutto brano1 è dopo brano2
    // brano1/2 sono di 6 byte
    private static func confrontaBrani(_ brano1:[UInt8], _ brano2:[UInt8]) -> Int
    {
        if (confrontaVersetti(brano1[3], brano1[4], brano1[5], brano2[0], brano2[1], brano2[2]) < 0) {
            return -1;
        }
        if (confrontaVersetti(brano1[0], brano1[1], brano1[2], brano2[3], brano2[4], brano2[5]) > 0) {
            return 1;
        }
        return 0;
    }
    
    // -1 se tutto brano1 è prima di brano2
    // 0 se si sovrappongono
    // 1 se tutto brano1 è dopo brano2
    private static func confrontaVersetti(_ libro1:UInt8, _ capitolo1:UInt8, _ versetto1:UInt8, _ libro2:UInt8, _ capitolo2:UInt8, _ versetto2:UInt8) -> Int
    {
        var confronto = 0;
        if (libro1 < libro2) {
            confronto = -1;
        }
        if (libro1 > libro2) {
            confronto = 1;
        }
        if (confronto == 0) {
            if (capitolo1 < capitolo2) {
                confronto = -1;
            }
            if (capitolo1 > capitolo2) {
                confronto = 1;
            }
        }
        if (confronto == 0) {
            if (versetto1 < versetto2) {
                confronto = -1;
            }
            if (versetto1 > versetto2) {
                confronto = 1;
            }
        }
        return confronto;
    }
    
    //#region TestoBrano
    
    func testoBrano(_ riferimento:Riferimento, _ collezioniDaVisualizzare:[String], _ noteDaVisualizzare:[Riferimento], _ conNomiDelleNote:Bool = true, _ paroleRicercate:Riferimento = Riferimento()) -> String
    {
        var testoComeStringa = ""
        let numeroCommentari = collezioniDaVisualizzare.count;
        var convertitodaRTF = false
        
        var formatoGreco = "\\f3\\fs" + String(lround(genitore.formato.fontGrecoDimensione * 2)) + "\\cf3";
        if (genitore.formato.fontGrecoGrassetto) {
            formatoGreco += "\\b";
        }
        if (genitore.formato.fontGrecoCorsivo) {
            formatoGreco += "\\i";
        }
        if (genitore.formato.fontGrecoSottolineato) {
            formatoGreco += "\\ul";
        }
        formatoGreco += " ";
        
        var formatoEbraico = "\\f4\\fs" + String(lround(genitore.formato.fontEbraicoDimensione * 2)) + "\\cf4";
        if (genitore.formato.fontEbraicoGrassetto) {
            formatoEbraico += "\\b";
        }
        if (genitore.formato.fontEbraicoCorsivo) {
            formatoEbraico += "\\i";
        }
        if (genitore.formato.fontEbraicoSottolineato) {
            formatoEbraico += "\\ul";
        }
        formatoEbraico += " ";
        
        var formatoRiferimento = "\\f1\\fs" + String(lround(genitore.formato.fontRiferimentoDimensione * 2)) + "\\cf1";
        var fineRiferimentoBase = "}"
        if (genitore.formato.fontRiferimentoGrassetto) {
            formatoRiferimento += "<b>" //\\b";
            fineRiferimentoBase = "</b>" + fineRiferimentoBase
        }
        if (genitore.formato.fontRiferimentoCorsivo) {
            formatoRiferimento += "<i>" //\\i";
            fineRiferimentoBase = "</i>" + fineRiferimentoBase;
        }
        if (genitore.formato.fontRiferimentoSottolineato) {
            formatoRiferimento += "<u>" // \\ul";
            fineRiferimentoBase = "</u>" + fineRiferimentoBase;
        }
        
        // per le note, quando il riferimento è il titolo, non è mai messo in apice
        
        // FontRicerca (\f2) e FontRicercaDimensione non è usato, per non disturbare troppo il font del testo
        //                string formatoRicerca = "\\f2\\fs" + Convert.ToString(Convert.ToInt32(genitore.Formato.FontRicercaDimensione * 2)) + "\\cf2";
        var formatoRicercaNote =  "\\v " + genitore.ParolaRicercata + "\\v0"
        var formatoRicerca = formatoRicercaNote + "\\cf2";
        // comunque, modificare il font e il colore non funzionano, perché \f? e \cf? non necessariamente corrispondono al font e al colore giusti
        if (genitore.formato.fontRicercaGrassetto)
        {
            formatoRicerca += "\\b";
            formatoRicercaNote += "\\b";
        }
        if (genitore.formato.fontRicercaCorsivo)
        {
            formatoRicerca += "\\i";
            formatoRicercaNote += "\\i";
        }
        if (genitore.formato.fontRicercaSottolineato)
        {
            formatoRicerca += "\\ul";
            formatoRicercaNote += "\\ul";
        }
        
        var ultimaParolaRicercata = -1;
        
        let numeroParoleRicercate = paroleRicercate.count();
        if (riferimento.versetti) {
            //#region brano biblico
            if (info.tipo == TestoTipi.Bibbia) {
                if (genitore.formato.riferimentoApice) {
                    // in apice solo quando riferimento, non quando titolo di una nota
                    formatoRiferimento += "<sup>" // \\super";
                    fineRiferimentoBase.insert(fineRiferimentoBase.count-1, "</sup>")
                }
                
                var formatoRiferimentoContestoInizio = "", formatoRiferimentoContestoFine = "";
                if (genitore.formato.riferimentoContestoRicerche) {
                    //formatoRiferimentoContestoInizio = "\\v" + genitore.InizioLink + "\\v0 *\\v " + genitore.FineLink1 + genitore.FineLinkBrano + info.nome + "##";
                    formatoRiferimentoContestoInizio = "<a href=\"lpnb://" + info.nome + "##"
                    //formatoRiferimentoContestoFine = "0000</a>"
                    formatoRiferimentoContestoFine = "0000?ip=1\">*</a>";
                }
                
                let ebraico = (genitore.linguaPrincipale(info.lingua).hasPrefix("he"));
                let greco = (genitore.linguaPrincipale(info.lingua) == "el");
                let rtl = genitore.rightToLeft(info.lingua);
                
                var riferimentoPosto = genitore.formato.riferimentoPosto;
                var testoVisualizzato = genitore.formato.testoVisualizzato;
                if (rtl && genitore.formato.riferimentoFormato != RiferimentoFormato.Nessuno) {
                    riferimentoPosto = RiferimentoPosto.PrimaRigaDiversa;
                    testoVisualizzato = TestoVisualizzato.Versetti;
                }
                
                var testoDaVisualizzare = ""
                var testoDaVisualizzareComeStringa = ""
                var cap0:UInt8, cap1:UInt8, vers0:UInt8, vers1:UInt8;
                var riferimentoVersetto = ""
                var libroPunt = "", capitoloPunt = "", libroCapitoloPunt = "";
                var riferimentoLibro = "";
                let punteggiaturaFraLibroECapitolo = genitore.separatoriNeiRiferimenti()[0];
                let punteggiaturaFraCapitoloEVersetto = genitore.separatoriNeiRiferimenti()[1];
                var libroStringa = "", capitoloStringa = "", versettoStringa = "", versettoStringaInTestoNascosto = "";
                var versettoStringa1 = "", testoDaAppendere = ""
                var p:Int, p1:Int;
                
                var riferimentoDaMostrare:[UInt8] = [0,0,0,0,0,0]
                let riferimentocount = riferimento.count()
                for i in stride(from:0, to:riferimentocount, by:1) {
                    if (i > 0) {
                        // riga vuota fra i brani
                        if (testoDaVisualizzare.hasSuffix("\\par ")) {
                            testoDaVisualizzare.append("\\par ");
                        }
                        else {
                            if (!testoDaVisualizzare.isEmpty) {
                                testoDaVisualizzare.append("\\par\\par ");
                            }
                        }
                    }
                    
                    riferimentoDaMostrare = riferimento.brani[i];
                    if  (riferimentoDaMostrare[0] == riferimentoDaMostrare[3] && riferimentoDaMostrare[1] == riferimentoDaMostrare[4] && riferimentoDaMostrare[2] == 0 && riferimentoDaMostrare[5]==0) {
                        riferimentoDaMostrare[2] = 1; // tutto il capitolo
                        riferimentoDaMostrare[5] = 200;
                    }

                    var seek:UInt64 = UInt64(indiceLibro[Int(riferimentoDaMostrare[0]) - 1] + UInt16(riferimentoDaMostrare[1]) - 1)
                    seek = UInt64(indiceCapitolo[Int(seek)]) + UInt64(riferimentoDaMostrare[2]) - 1
                    seek = UInt64(pIndice) + 4 * seek
                        self.semaphore.wait()
                        do {
                            try self.handle?.seek(toOffset:seek)
                            try self.handle?.seek(toOffset:UInt64(self.pTesto + UInt32(try self.readInt32())))
                        }
                        catch {
                            self.semaphore.signal()
                            return ""
                        }
                        
                        //fs.Seek(pTesto + ReadInt32(), SeekOrigin.Begin);
                        var fineRiferimento = "", formatoRifPerVersetto = "", testoVersetto = "", testoVersettoTitolo = "", testoVersettoTestoBiblico = "";
                  
                        let soloUnVersetto = (riferimentoDaMostrare[0] == riferimentoDaMostrare[3] && riferimentoDaMostrare[1] == riferimentoDaMostrare[4] && riferimentoDaMostrare[2] == riferimentoDaMostrare[5]);
                        
                        for lib in stride(from:riferimentoDaMostrare[0], through:riferimentoDaMostrare[3], by:1) {
                            if (lib == riferimentoDaMostrare[0]) {
                                cap0 = riferimentoDaMostrare[1]
                            }
                            else  {
                                cap0 = 1;
                            }
                            if (lib == riferimentoDaMostrare[3]) {
                                cap1 = riferimentoDaMostrare[4];
                            }
                            else {
                                cap1 = self.capitoliInLibro[Int(lib)];
                            }
                            if (cap1 > self.capitoliInLibro[Int(lib)]) {
                                cap1 = self.capitoliInLibro[Int(lib)];
                            }
                            switch (self.genitore.formato.riferimentoFormato)
                            {
                            case RiferimentoFormato.Intero:
                                riferimentoLibro = self.genitore.formato.libriNomi[Int(lib)];
                                break;
                            case RiferimentoFormato.Abbreviazione:
                                riferimentoLibro = self.genitore.formato.libriAbbreviazioniUsate[Int(lib)];
                                break;
                            case RiferimentoFormato.Nessuno:
                                break;
                            case RiferimentoFormato.NessunoLibro:
                                break;
                            case RiferimentoFormato.AbbreviazioneRiconosciuta:
                                riferimentoLibro = self.genitore.libriAbbreviazioniRiconosciute.abbreviazione(lib);
                                break;
                            }
                            
                            libroStringa = (lib <= 9 ? "0" + String(lib) : String(lib));
                            libroPunt = riferimentoLibro + punteggiaturaFraLibroECapitolo;
                            
                            for cap in stride(from:cap0, through:cap1, by:1) {
                                if (lib > riferimentoDaMostrare[0] && cap == cap0) {
                                    // messo qui invece di prima del loop per evitare righe addizionali quando ci sono libri mancanti per es. l'Apocrifa
                                    
                                    if (testoVersetto.hasSuffix("\\par ")) {
                                        testoDaVisualizzare.append("\\par ");
                                    }
                                    else {
                                        testoDaVisualizzare.append("\\par\\par "); // riga vuota fra i libri
                                    }
                                    
                                }
                                if (lib == riferimentoDaMostrare[0] && cap == riferimentoDaMostrare[1]) {
                                    vers0 = riferimentoDaMostrare[2];
                                }
                                else {
                                    vers0 = 1;
                                }
                                if (lib == riferimentoDaMostrare[3] && cap == riferimentoDaMostrare[4]) {
                                    vers1 = riferimentoDaMostrare[5];
                                }
                                else {
                                    vers1 = self.versettiInCapitolo[Int(self.indiceLibro[Int(lib) - 1]) + Int(cap)];
                                }
                                if (vers1 > self.versettiInCapitolo[Int(self.indiceLibro[Int(lib) - 1]) + Int(cap)]) {
                                    vers1 = self.versettiInCapitolo[Int(self.indiceLibro[Int(lib) - 1]) + Int(cap)];
                                }
                                capitoloStringa = "00" + String(cap);
                                capitoloStringa = libroStringa + capitoloStringa[(capitoloStringa.count - 3)...];
                                if (cap > cap0)
                                {
                                    
                                    if (testoVersetto.hasSuffix("\\par ")) {
                                        testoDaVisualizzare.append("\\par ");
                                    }
                                    else {
                                        testoDaVisualizzare.append("\\par\\par "); // riga vuota fra capitoli
                                    }
                                    
                                }
                                
                                capitoloPunt = String(cap) + punteggiaturaFraCapitoloEVersetto;
                                libroCapitoloPunt = libroPunt;
                                if (self.capitoliInLibro[Int(lib)] > 1) {
                                    libroCapitoloPunt += capitoloPunt;
                                }
                                for vers in stride(from:vers0, through:vers1, by:1) {
                                    riferimentoVersetto = ""
                                    switch (self.genitore.formato.riferimentoFormato)
                                    {
                                    case RiferimentoFormato.Intero:
                                        riferimentoVersetto.append(libroCapitoloPunt+String(vers));
                                        break;
                                    case RiferimentoFormato.Abbreviazione:
                                        if (vers == vers0) {
                                            //  if (cap == cap0)
                                            riferimentoVersetto.append(libroCapitoloPunt+String(vers));
                                            //  else
                                            //    riferimento = cap.ToString() + punt2 + vers.ToString();
                                            // prima della versione 7, il riferimento aveva il libro solo all'inizio e con un nuovo libro
                                            // qui c'è il libro all'inizio di ogni capitolo, altrimenti sposta il testo in Sfoglia non funziona,
                                            // perché quando cerca il testo Gen 47:1 trova 47:1 per esempio
                                        }
                                        else {
                                            riferimentoVersetto.append(String(vers));
                                        }
                                        break;
                                    case RiferimentoFormato.Nessuno:
                                        break;
                                    case RiferimentoFormato.NessunoLibro:
                                        if (self.capitoliInLibro[Int(lib)] > 1) {
                                            riferimentoVersetto.append(capitoloPunt);
                                        }
                                        riferimentoVersetto.append(String(vers));
                                        break;
                                    case RiferimentoFormato.AbbreviazioneRiconosciuta:
                                        riferimentoVersetto.append(libroCapitoloPunt+String(vers));
                                        break;
                                    }
                                    if (self.genitore.formato.riferimentoTipo == RiferimentoTipo.Citazione) {
                                        riferimentoVersetto.append(":");
                                    }
                                    
                                    fineRiferimento = fineRiferimentoBase
                                    
                                    formatoRifPerVersetto = formatoRiferimento;
                                    if (!riferimentoVersetto.isEmpty) {
                                        if !formatoRifPerVersetto.hasSuffix(">") {
                                            formatoRifPerVersetto += " ";
                                        }
                                        fineRiferimento += "\\~"; // ma il non breaking space non funziona con il controllo RichEdit usato in .NET
                                    }
                                    
                                    versettoStringa = "00" + String(vers);
                                    versettoStringa = capitoloStringa + versettoStringa[(versettoStringa.count - 3)...];
                                    
                                    versettoStringaInTestoNascosto = "" // {\\v " + genitore.InizioRiferimento + versettoStringa + "}";
                                    // reference in hidden text gets take out later anyway in Testi.convertiRTF, togliendo qui rende più veloce
                                    // forse sarà necessario inserire dopo, ma sarà meglio mettere direttamente il codice HTML
                                    riferimentoVersetto.insert(0, "{"+formatoRifPerVersetto)
                                    if (soloUnVersetto && self.genitore.formato.riferimentoContestoRicerche && self.genitore.formato.riferimentoFormato != RiferimentoFormato.Nessuno)
                                    {
                                        versettoStringa1 = "00" + String(vers > 3 ? vers - 3 : 1);
                                        riferimentoVersetto.append(formatoRiferimentoContestoInizio+capitoloStringa+versettoStringa1[(versettoStringa1.count - 3)...]+"0000-");
                                        // + in un riferimento invece di - indica che il riferimento è sempre visualizzato nella finestra Visualizza (in Principale::LinkCliccato)
                                        versettoStringa1 = "00" + String(vers + 3);
                                        riferimentoVersetto.append(capitoloStringa+versettoStringa1[(versettoStringa1.count-3)...]+formatoRiferimentoContestoFine);
                                    }
                                    riferimentoVersetto.append(fineRiferimento);
                                    
                                    testoDaVisualizzareComeStringa = testoDaVisualizzare.trimmingCharacters(in: .whitespacesAndNewlines)
                                    
                                    if (!testoDaVisualizzare.isEmpty && !testoDaVisualizzareComeStringa.hasSuffix("\\par") && !testoDaVisualizzareComeStringa.hasSuffix("\\par}") && !testoDaVisualizzareComeStringa.hasSuffix("\\par }") && !testoDaVisualizzare.hasSuffix(" ")) {
                                        testoDaVisualizzare.append(" ");
                                    }
                                    testoDaVisualizzare.append(versettoStringaInTestoNascosto);
                                    
                                    do {
                                        switch (testoVisualizzato)
                                        {
                                        case TestoVisualizzato.Versetti:
                                            testoVersetto = try self.readString();
                                            
                                            if (!testoVersetto.trimSuffix().hasSuffix("\\par")) {
                                                testoVersetto += "\\par ";
                                            }
                                            
                                            break;
                                        case TestoVisualizzato.Paragrafi:
                                            testoVersetto = try self.readString();
                                            break;
                                        case TestoVisualizzato.Nessuno:
                                            testoVersetto = "";
                                            break;
                                        }
                                    }
                                    catch {
                                        self.semaphore.signal()
                                        return ""
                                    }
                                    
                                    if (lib == riferimentoDaMostrare[0] && cap == cap0 && vers == vers0) {
                                        testoVersetto = self.modificaFormatoParole(testoVersetto, riferimento.numeroParola[i], "{" + formatoRicerca + " ", "}", self.info.lingua);
                                    }
                                    for numeroParolaRicercata in stride(from:ultimaParolaRicercata + 1, to:numeroParoleRicercate, by:1) {
                                        if (lib > paroleRicercate.brani[numeroParolaRicercata][0]) {
                                            ultimaParolaRicercata = numeroParolaRicercata;
                                        }
                                        else {
                                            if (lib < paroleRicercate.brani[numeroParolaRicercata][0]) {
                                                break;
                                            }
                                            else {
                                                if (cap == paroleRicercate.brani[numeroParolaRicercata][1] && vers == paroleRicercate.brani[numeroParolaRicercata][2]) {
                                                    testoVersetto = self.modificaFormatoParole(testoVersetto, paroleRicercate.numeroParola[numeroParolaRicercata], "{" + formatoRicerca + " ", "}", self.info.lingua);
                                                }
                                            }
                                        }
                                    }
                                    
                                    testoVersettoTitolo = "";
                                    testoVersettoTestoBiblico = testoVersetto;
                                    
                                    if (testoVersetto.indexOf("\\lptit1 ") == 0) {
                                        p = testoVersetto.indexOf("\\lptit0 ");
                                        if (p > -1) {
                                            testoVersettoTitolo = self.genitore.formato.titoliVisualizzati ? testoVersetto[0..<(p + 8)] : "";
                                            testoVersettoTestoBiblico = testoVersetto[(p + 8)...];
                                        }
                                    }
                                    
                                    if (!self.genitore.formato.titoliVisualizzati) {
                                        while (testoVersettoTestoBiblico.indexOf("\\lptit1 ") >= 0)
                                        { // quando ci sono due titoli in un versetto, come Sal 24 nella CEI
                                            p1 = testoVersettoTestoBiblico.indexOf("\\lptit1 ");
                                            p = testoVersettoTestoBiblico.indexOf("\\lptit0 ");
                                            if (p > -1) {
                                                testoVersettoTestoBiblico = testoVersettoTestoBiblico[0..<p1] + testoVersettoTestoBiblico[(p + 8)...];
                                            }
                                            else {
                                                testoVersettoTestoBiblico = testoVersettoTestoBiblico[0..<p1] + testoVersettoTestoBiblico[(p1 + 8)...]; // in questo caso, c'è un errore nel testo
                                            }
                                        }
                                    }
                                    
                                    // inserire le note nel posto giusto nel testo
                                    var notaStringa = ""
                                    for iCommentario in stride(from:0, to:numeroCommentari, by:1) {
                                        let numeroNote = noteDaVisualizzare[iCommentario].count();
                                        for iNota in stride(from: 0, to:numeroNote, by:1) {
                                            notaStringa = noteDaVisualizzare[iCommentario].note[iNota];
                                            if (notaStringa[1..<9] == versettoStringa
                                                || (notaStringa[6..<9] == "000" && notaStringa[1..<6] + "001" == versettoStringa) // nota per tutto il capitolo mostrato all'inizio del primo versetto
                                                || (notaStringa[3..<9] == "000000" && notaStringa[1..<3] + "001001" == versettoStringa)) // nota per tutto il libro mostrato all'inizio del primo versetto
                                            {
                                                let numeroDellaParola = UInt16(noteDaVisualizzare[iCommentario].note[iNota][9..<13]) ?? 0;
                                                testoVersettoTestoBiblico = self.modificaFormatoParole(testoVersettoTestoBiblico, numeroDellaParola, "", "{\\v " + self.genitore.InizioLink + "}*{\\v " + self.genitore.FineLink1 + self.genitore.FineLinkNota + collezioniDaVisualizzare[iCommentario] + "\\\\" + noteDaVisualizzare[iCommentario].note[iNota] + self.genitore.FineLink2 + "}" + (iCommentario == 0 ? "" : " "), self.info.lingua);
                                            }
                                        }
                                    }
                                    
                                    if (greco)
                                    {
                                        testoVersettoTestoBiblico = "{" + formatoGreco + testoVersettoTestoBiblico + "}";
                                        if (testoVersettoTestoBiblico.hasSuffix("\\par }")) {
                                            testoVersettoTestoBiblico = testoVersettoTestoBiblico[0..<(testoVersettoTestoBiblico.count - 6)] + "}\\par ";
                                        }
                                    }
                                    if (ebraico)
                                    {
                                        testoVersettoTestoBiblico = "{" + formatoEbraico + testoVersettoTestoBiblico + "}";
                                        if (testoVersettoTestoBiblico.hasSuffix("\\par }")) {
                                            testoVersettoTestoBiblico = testoVersettoTestoBiblico[0..<(testoVersettoTestoBiblico.count - 6)] + "}\\par ";
                                        }
                                    }
                                    
                                    testoVersettoTestoBiblico = self.genitore.convertiRTF(testoVersettoTestoBiblico, 2)
                                    riferimentoVersetto = self.genitore.convertiRTF(riferimentoVersetto, 2)
                                    testoVersettoTitolo = self.genitore.convertiRTF(testoVersettoTitolo, 2)
                                    
                                    switch (riferimentoPosto)
                                    {
                                    case RiferimentoPosto.PrimaStessaRiga:
                                        testoDaAppendere = testoVersettoTitolo+riferimentoVersetto+testoVersettoTestoBiblico
                                        break;
                                    case RiferimentoPosto.PrimaRigaDiversa:
                                        testoDaAppendere = testoVersettoTitolo+riferimentoVersetto+"\\par "+testoVersettoTestoBiblico
                                        break;
                                    case RiferimentoPosto.Dopo:
                                        if (testoVersettoTestoBiblico.hasSuffix("\\par")) {
                                            testoVersettoTestoBiblico = testoVersettoTestoBiblico.remove(testoVersettoTestoBiblico.count - 4, 4);
                                            riferimentoVersetto.append("\\par");
                                        }
                                        if (testoVersettoTestoBiblico.hasSuffix("\\par ")) {
                                            testoVersettoTestoBiblico = testoVersettoTestoBiblico.remove(testoVersettoTestoBiblico.count - 5, 5);
                                            riferimentoVersetto.append("\\par ");
                                            if (testoVersettoTestoBiblico.hasSuffix("\\par ")) { // nuovo paragrafo, ma il testo è visualizzato a versetti
                                                testoVersettoTestoBiblico = testoVersettoTestoBiblico.remove(testoVersettoTestoBiblico.count - 5, 5);
                                                riferimentoVersetto.append("\\par ");
                                            }
                                        }
                                        testoDaAppendere = testoVersettoTitolo+testoVersettoTestoBiblico + " - " + riferimentoVersetto
                                        break;
                                    }
                                    if testoDaAppendere.hasPrefix("</p>") {
                                        testoDaAppendere.insert(4, "<a id=\""+versettoStringa+"\" href=\"#"+versettoStringa+"\"></a>")
                                    }
                                    else {
                                        testoDaAppendere = "<a id=\""+versettoStringa+"\" href=\"#"+versettoStringa+"\"></a>" + testoDaAppendere
                                    }
                                    testoDaVisualizzare.append(testoDaAppendere);
                                }
                            }
                        }
                        self.semaphore.signal()
                }
                
                testoComeStringa = testoDaVisualizzare
                
                if (rtl) {
                    //testoComeStringa = "{\\qr " + testoComeStringa + "}";
                    testoComeStringa = "<div style=\"text-align:right\">" + testoComeStringa + "</div>";
                }
                testoComeStringa = genitore.RtfIntestazione() + testoComeStringa //+ "}"; tolto } perché RtfIntestazione ora è vuoto
                
            } // if (testoFileArray.Tipo==TestoTipo.Bibbia)
            else { // tutte le note in un certo brano
                testoComeStringa = testoBrano(elencaNoteInBrano(riferimento), collezioniDaVisualizzare, noteDaVisualizzare, conNomiDelleNote, paroleRicercate);
                convertitodaRTF = true
            }
        }
        else
        {
            //#region nota
            // collezioniDaVisualizzare e noteDaVisualizzare non sono usati in questo caso
            var titoloNota = "", titoloNotaDaLeggere = "", testoDelBrano = ""
            
            var notaSuBrano:Bool
            let numeroNote = riferimento.note.count;

            for i in stride(from:0, to:numeroNote, by:1) {
                if (i > 0) { // riga vuota fra i brani
                    if (testoDelBrano.hasSuffix("</p><p>")) {
                        testoDelBrano.append("</p>");
                    }
                    else {
                        testoDelBrano.append("</p><p></p>");
                    }
                }
                titoloNota = riferimento.note[i];
                notaSuBrano = titoloNota.hasPrefix("#");
                titoloNotaDaLeggere = (notaSuBrano ? genitore.normalizzaRiferimento(genitore.convertiTitoloNotaARiferimento(titoloNota), RiferimentoFormato.Abbreviazione) : titoloNota);
                if notaSuBrano {
                    testoDelBrano.append("<a id=\""+titoloNota[1..<9]+"\" href=\"#"+titoloNota[1..<9]+"\"></a>")
                }
                if (conNomiDelleNote) {
                    testoDelBrano.append("<h3>" + titoloNotaDaLeggere + "</h3>")
                }
                testoDelBrano.append("<p>");
                var testoModificato = genitore.convertiRTF(getNotaTesto(titoloNota),2);
                if testoModificato.hasSuffix("<br />") {
                    testoModificato.removeLast(6)
                }
                testoDelBrano.append(testoModificato);
            }
            
            testoComeStringa = testoDelBrano
            
            if (testoComeStringa.hasSuffix("\r\n")) {
                testoComeStringa = testoComeStringa.remove(testoComeStringa.count - 2, 2);
            }
            if (testoComeStringa.hasSuffix("\r\n}")) {
                testoComeStringa = testoComeStringa.remove(testoComeStringa.count - 3, 2);
            }
            if (testoComeStringa.hasSuffix("\\par}")) {
                testoComeStringa = testoComeStringa.remove(testoComeStringa.count - 5, 4);
            }
            if (testoComeStringa.hasSuffix("\\f0}")) {
                testoComeStringa = testoComeStringa.remove(testoComeStringa.count - 4, 3);
            }
            if (testoComeStringa.hasSuffix("</p><p>")) {
                testoComeStringa = testoComeStringa.remove(testoComeStringa.count - 7, 7);
            }
        }
        
        if !convertitodaRTF {
            testoComeStringa = genitore.convertiRTF(testoComeStringa)
        }
        return testoComeStringa
    }
    /*
     public string TestoVersettoRaw(byte libro, byte capitolo, byte versetto)
     {
     string testoVersetto = "";
     if (info.Tipo == TestoTipi.Bibbia) // altrimenti solo una stringa vuota è restituita
     {
     lock.lock()
     defer { lock.unlock()}
     fs.Seek(pIndice + 4 * (indiceCapitolo[indiceLibro[libro - 1] + capitolo - 1] + versetto - 1), SeekOrigin.Begin);
     fs.Seek(pTesto + br.ReadInt32(), SeekOrigin.Begin);
     testoVersetto = br.ReadString();
     }
     return testoVersetto;
     }
     
     private static string ConvertiCaratteriInRtf(string stringa)
     {
     int lunghezza = stringa.Length;
     string nuovaStringa = stringa;
     for (int i = lunghezza - 1; i >= 0; --i)
     {
     if (stringa[i] > 256)
     nuovaStringa = nuovaStringa.replacingOccurrences(of:String(stringa[i]), with:"\\u" + Convert.ToUInt32(stringa[i]).ToString() + "?");
     else if (stringa[i] > 127)
     nuovaStringa = nuovaStringa.replacingOccurrences(of:String(stringa[i]), with:"\\'" + Uri.HexEscape(stringa[i]).Remove(0, 1));
     }
     return nuovaStringa.replacingOccurrences(of:"\r\n", with:"\\par ");
     }
     
     */
    
    func modificaFormatoParole(_ testoDaModificare:String, _ numeroParolaDaModificare:UInt16, _ formatoPrimaDellaParola:String, _ formatoDopoLaParola:String, _ lingua:String) -> String
    {
        let listaParole:[UInt16] = [numeroParolaDaModificare];
        return modificaFormatoParole(testoDaModificare, listaParole, formatoPrimaDellaParola, formatoDopoLaParola, lingua);
    }
    
    func modificaFormatoParole(_ testo:String, _ paroleDaModificare:[UInt16], _ formatoPrima:String, _ formatoDopo:String, _ lingua:String) -> String
    {
        var testoDaModificare = testo
        var numeriParoleDaModificare = paroleDaModificare
        var formatoPrimaDellaParola = formatoPrima
        var formatoDopoLaParola = formatoDopo
        if ((formatoPrimaDellaParola == "{" && formatoDopoLaParola == "}") || (formatoPrimaDellaParola.isEmpty && formatoDopoLaParola.isEmpty) || (numeriParoleDaModificare.count == 0)) {
            return testoDaModificare; // non ci sono modifiche da fare, quindi rimane uguale
        }
        
        let lingue = lingua.lowercased().split(separator:"|").map{String($0)}
        var linguaDaUsare:String
        let linguaPrincipale = (lingue.count >= 1 ? lingue[0] : "");
        let dizionarioGreco = (linguaPrincipale == "el" && lingue.count >= 2);
        let dizionarioEbraico = (linguaPrincipale.hasPrefix("he") && lingue.count >= 2);
        
        if (genitore.rightToLeft(linguaPrincipale))
        { // in lingue RTL (come ebraico), l'inserimento di un carattere non RTL, anche nascosto, rovina la visualizzazione
            // per questo motivo un testo ricercato in ebraico non salterà alla prima apparizione di una parola ricercata, ma non è troppo grave
            while (formatoPrimaDellaParola.indexOf("\\v ") >= 0) {
                formatoPrimaDellaParola = formatoPrimaDellaParola[0..<formatoPrimaDellaParola.indexOf("\\v ")] + formatoPrimaDellaParola[(formatoPrimaDellaParola.indexOf("\\v0") + 3)...];
            }
            while (formatoDopoLaParola.indexOf("\\v ") >= 0) {
                formatoDopoLaParola = formatoDopoLaParola[0..<formatoDopoLaParola.indexOf("\\v ")] + formatoDopoLaParola[(formatoDopoLaParola.indexOf("\\v0") + 3)...];
            }
        }
        
        var nParoleDaCambiare = numeriParoleDaModificare.count;
        // a volte si chiede che la stessa parola sia modificata 2 volte; non è possibile quindi togliamo i doppioni
        for i in stride(from:nParoleDaCambiare-1, through:1, by:-1) {
            if (numeriParoleDaModificare[i] == numeriParoleDaModificare[i - 1]) {
                numeriParoleDaModificare.remove(at:i);
                nParoleDaCambiare -= 1;
            }
        }
        var iParolaDaCambiare = 0;
        var nProssimaParolaDaCambiare = numeriParoleDaModificare[0];
        var paroleTrovate = 0;
        var parola = ""
        var statoCambiamento = 0; // 0=niente da cambiare, 1=cambiare la prossima, 2=chiudere il cambiamento alla fine di questa parola
        if (nProssimaParolaDaCambiare == 1) {
            statoCambiamento = 1;
            iParolaDaCambiare += 1
            if (iParolaDaCambiare < nParoleDaCambiare) {
                nProssimaParolaDaCambiare = numeriParoleDaModificare[iParolaDaCambiare];
            }
        }
        var c = ""
        var analizzaParola:Bool
        var carattereIniziale = 0, iCarattere1 = 0, iCarattere2 = 0, carattereDaInserire = 0;
        if (testoDaModificare.indexOf("\\viewkind") > 0) { // saltare un'eventuale intestazione RTF
            carattereIniziale = testoDaModificare.indexOf("\\viewkind") + 10; // +10 perché c'è un numero dopo viewkind
        }
        if (testoDaModificare.indexOf("\\deflang", carattereIniziale) > 0) { // saltare un'eventuale intestazione RTF
            carattereIniziale = testoDaModificare.indexOf("\\deflang", carattereIniziale) + 12; // +12 perché ci sono quattro cifre dopo deflang
        }
        if (testoDaModificare.hasPrefix("{\\rtf") && carattereIniziale == 0 && testoDaModificare.indexOf("\\pard", carattereIniziale) > 0) {
            carattereIniziale = testoDaModificare.indexOf("\\pard", carattereIniziale) + 6;
        }
        if (nProssimaParolaDaCambiare == 0) {
            testoDaModificare.insert(carattereDaInserire, formatoPrimaDellaParola + formatoDopoLaParola);
            carattereIniziale += (formatoPrimaDellaParola + formatoDopoLaParola).count;
            iParolaDaCambiare += 1
            if (iParolaDaCambiare < nParoleDaCambiare) {
                nProssimaParolaDaCambiare = numeriParoleDaModificare[iParolaDaCambiare];
            }
        }
        
        var i = carattereIniziale
        var spostamento = 0
        var testoModificato = testoDaModificare
        let testoDaModificareCount = testoDaModificare.count
        while i < testoDaModificareCount {
            //for i in stride(from:carattereIniziale, to:testoDaModificare.count, by:1) {
            c = testoDaModificare[i];
            if (c.isLetterOrNumber() || (c == "\\" && i < testoDaModificareCount - 3 && (testoDaModificare[i + 1] == "'" || (testoDaModificare[i + 1] == "u" && Character(testoDaModificare[i + 2]).isWholeNumber)))) {
                if (i <= testoDaModificareCount - 1 && c == genitore.InizioLink) {
                    i += 0;
                }
                else {
                    if (c.isLetterOrNumber()) {
                        parola.append(c);
                    }
                    else { if (testoDaModificare[i + 1] == "'") {
                        parola.append(testoDaModificare[i..<(i+4)]);
                    }
                        else {
                            if (testoDaModificare[i + 1] == "u" && Character(testoDaModificare[i + 2]).isWholeNumber) {
                                parola.append(testoDaModificare[i..<(testoDaModificare.indexOf("?", i) + 1)])
                            }
                        }
                    }
                    if (statoCambiamento == 1) {
                        testoModificato.insert(i + spostamento, formatoPrimaDellaParola);
                        spostamento += formatoPrimaDellaParola.count;
                        statoCambiamento = 2;
                    }
                    if (!c.isLetterOrNumber()) {
                        if (testoDaModificare[i + 1] == "'") {
                            i += 3;
                        }
                        else { // unicode \u1234? oppure \u123?
                            i = testoDaModificare.indexOf("?", i);
                        }
                    }
                }
            }
            else if (Character(c).isPunctuation || Character(c).isWhitespace || Character(c).isSymbol)
            //else if (Char.IsPunctuation(c) || Char.IsWhiteSpace(c) || Char.IsSymbol(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.Format)
            {
                analizzaParola = true;
                carattereDaInserire = i;
                if (c == "'")
                {
                    // in un dizionario greco-altra lingua, dobbiamo scegliere la lingua giusta
                    linguaDaUsare = linguaPrincipale;
                    if (dizionarioGreco && i > 0 && !testoDaModificare[i - 1].isLetterGreek()) {
                        linguaDaUsare = lingue[1];
                    }
                    else {
                        if (dizionarioEbraico && i > 0 && !testoDaModificare[i - 1].isLetterHebrew()) {
                            linguaDaUsare = lingue[1];
                        }
                    }
                    if (linguaDaUsare.count > 2) {
                        linguaDaUsare = linguaDaUsare[0..<2];
                    }
                    switch (linguaDaUsare)
                    {
                    case "en":
                        if ((i == 1 || !testoDaModificare[i - 1].isLetterOrNumber())
                            && ((i < testoDaModificareCount - 1 && (testoDaModificare[i + 1] == "t" || testoDaModificare[i + 1] == "T") && (i == testoDaModificareCount - 2 || !testoDaModificare[i + 2].isLetterOrNumber()))
                                || (i < testoDaModificareCount - 3 && testoDaModificare[(i+1)..<(i+3)].lowercased() == "tis" && (i == testoDaModificareCount - 4 || !testoDaModificare[i + 4].isLetterOrNumber()))
                                || (i < testoDaModificareCount - 4 && testoDaModificare[(i+1)..<(i+4)].lowercased() == "twas" && (i == testoDaModificareCount - 5 || !testoDaModificare[i + 5].isLetterOrNumber()))))
                        {
                            parola.append(c);
                            analizzaParola = false;
                        }
                        else if (i >= 2)
                        {
                            if (i < testoDaModificareCount - 1 &&
                                (testoDaModificare[i - 1].isLetterOrNumber()
                                 && Character(testoDaModificare[i + 1]).isLetter
                                 && (i == testoDaModificareCount - 2 || !testoDaModificare[i + 2].isLetterOrNumber())))
                            {
                                parola.append(c);
                                analizzaParola = false;
                            }
                            else if (dizionarioEbraico && i < testoDaModificareCount - 1 && (Character(testoDaModificare[i - 1]).isLetter && testoDaModificare[i + 1] == "-"))
                            { // per il dizionario Strong's Hebrew, che ha pronunce come eh'-sheth
                                parola.append(c);
                                analizzaParola = false;
                            }
                            else if ((testoDaModificare[i - 1] == "s" || testoDaModificare[i - 1] == "S")
                                     && (i == testoDaModificareCount - 1 || !Character(testoDaModificare[i + 1]).isPunctuation)
                                     && genitore.paroleInglesiSenzaApostrofe.binarySearch(for: parola) ?? -1 < 0)
                            {
                                parola.append(c);
                                analizzaParola = false;
                            }
                            else if (i < testoDaModificareCount - 2
                                     && testoDaModificare[i - 1].isLetterOrNumber() && (i == testoDaModificareCount - 3 || !testoDaModificare[i + 3].isLetterOrNumber())
                                     && (testoDaModificare[(i+1)..<(i+3)] == "en" || testoDaModificare[(i+1)..<(i+3)] == "er" || testoDaModificare[(i+1)..<(i+3)] == "ll" || testoDaModificare[(i+1)..<(i+3)] == "lt" || testoDaModificare[(i+1)..<(i+3)] == "ry" || testoDaModificare[(i+1)..<(i+3)] == "st"))
                            {
                                parola.append(c);
                                analizzaParola = false;
                            }
                            else if (i < testoDaModificareCount - 4
                                     && testoDaModificare[i - 1].isLetterOrNumber() && (i == testoDaModificareCount - 3 || !testoDaModificare[i + 5].isLetterOrNumber())
                                     && (testoDaModificare[(i+1)..<(i+5)] == "ring"))
                            {
                                parola.append(c);
                                analizzaParola = false;
                            }
                        }
                        break;
                    case "it":
                        if (i > 0 && i < testoDaModificareCount - 1)
                        {
                            if ((testoDaModificare[i - 1].isLetterOrNumber() && (testoDaModificare[i + 1].isLetterOrNumber() || testoDaModificare[i + 1] == "'" || testoDaModificare[i + 1] == "«")) || (genitore.paroleItalianeConApostrofe.binarySearch(for: parola) ?? -1 >= 0))
                            {
                                // per esempio l'uomo
                                parola.append(c);
                            }
                        }
                        break;
                    case "el":
                        if (i > 0)
                        {
                            if (testoDaModificare[i - 1].isLetterGreek()) {
                                parola.append(c);
                            }
                            else if (i < testoDaModificareCount - 1 && Character(testoDaModificare[i - 1]).isLetter && Character(testoDaModificare[i + 1]).isLetter)
                            {
                                parola.append(c);
                                analizzaParola = false;
                            }
                        }
                        break;
                    case "": // interlineare
                        parola.append(c);
                        break;
                    default:
                        parola.append(c);
                        break;
                    }
                }
                else if (c == "[" || c == "]")
                {
                    //                            if (linguaLC == "el")
                    //                            {
                    if (i > 0 && i < testoDaModificareCount - 1)
                    {
                        if (Character(testoDaModificare[i - 1]).isLetter && Character(testoDaModificare[i + 1]).isLetter)
                        {
                            // parentesi quadrate in mezzo ad una parola
                            analizzaParola = false;
                        }
                    }
                    //                            }
                }
                else if (c == "-") {
                    if (i > 0 && i < testoDaModificareCount - 1) {
                        if (((Character(testoDaModificare[i - 1]).isLetter || (testoDaModificare[i - 1] == "?" && i > 1 && Character(testoDaModificare[i - 2]).isWholeNumber)) &&
                             (Character(testoDaModificare[i + 1]).isLetter || (i < testoDaModificareCount - 2 && testoDaModificare[(i+1)..<(i+3)] == "\\u"))) // per esempio Eben-Ezer e \u963?-\u960? ma non 1-2
                            || (dizionarioEbraico && testoDaModificare[i - 1] == "'" && Character(testoDaModificare[i + 1]).isLetter)) // per esempio eh'-sheth in Strong's Hebrew
                        {
                            parola.append(c);
                            analizzaParola = false;
                        }
                    }
                }
                else if (c == "}")
                {
                    if (i > 0 && i < testoDaModificareCount - 1)
                    {
                        if (Character(testoDaModificare[i - 1]).isLetter && Character(testoDaModificare[i + 1]).isLetter)
                        {
                            // per esempio una parola parzialmente in italico come {\\i1 del}la
                            analizzaParola = false;
                        }
                    }
                }
                else if (c == "\\" || c == "{") { // saltare codice RTF
                    if (i < testoDaModificareCount - 6 && testoDaModificare[(i)..<(i+7)] == "\\lptit1") {
                        i = testoDaModificare.indexOf("\\lptit0 ", i) + 7; // saltare un titolo nel testo
                        if (i == 6) {
                            i = testoDaModificareCount - 1;
                        }
                    }
                    else {
                        if (i > 0 && c == "{" && Character(testoDaModificare[i - 1]).isLetter) {
                            // per esempio una parola parzialmente in italico come tuffata{\\i1 la}
                            analizzaParola = false;
                        }
                        // trova la fine del codice RTF cioè prossimo \ o spazio
                        iCarattere1 = testoDaModificare.indexOf("\\", i + 1) - 1;
                        if (iCarattere1 == i) {
                            iCarattere1 = -1;
                        }
                        iCarattere2 = testoDaModificare.indexOf(" ", i);
                        if (iCarattere1 >= 0 && iCarattere1 < iCarattere2) { // \ prima di spazio
                            if (i > 0 && c == "\\" && Character(testoDaModificare[i - 1]).isLetter && iCarattere1 < testoDaModificareCount - 2 && testoDaModificare[iCarattere1 + 2] == "'") {
                                // per esempio una parola come necessit\\f2\\'e0
                                analizzaParola = false;
                            }
                            iCarattere2 = iCarattere1;
                        }
                        else {
                            if (i > 0 && c == "\\" && Character(testoDaModificare[i - 1]).isLetter && iCarattere2 >= 0 && iCarattere2 < testoDaModificareCount - 1 && testoDaModificare[i..<iCarattere2] != "\\par" && (Character(testoDaModificare[iCarattere2 + 1]).isLetter)) {
                                // per esempio una parola come ess\f1 ere
                                analizzaParola = false;
                            }
                        }
                        if (iCarattere2 == -1) {
                            iCarattere2 = testoDaModificareCount - 1;
                        }
                        i = iCarattere2;
                    }
                }
                if (!parola.isEmpty && analizzaParola) {
                    if (statoCambiamento == 2) {
                        testoModificato.insert(carattereDaInserire + spostamento, formatoDopoLaParola);
                        spostamento += formatoDopoLaParola.count;
                        statoCambiamento = 0;
                    }
                    paroleTrovate += 1
                    if (paroleTrovate == nProssimaParolaDaCambiare - 1) {
                        statoCambiamento = 1;
                        iParolaDaCambiare += 1
                        if (iParolaDaCambiare < nParoleDaCambiare) {
                            nProssimaParolaDaCambiare = numeriParoleDaModificare[iParolaDaCambiare];
                        }
                    }
                    parola = "" // parola.remove(0, parola.count);
                }
            }
            i += 1
        } // while i < testoDaModificarecount
        //} // for (int iCarattere = 0; iCarattere < testoVersetto.count; ++iCarattere)
        if (statoCambiamento == 2) {
            testoModificato += "}";
        }
        return testoModificato;
    }
    
    internal func esisteBrano(_ riferimento:Riferimento) -> Bool
    {
        var branoEsiste = false;
        var branoDaControllare:[UInt8] = [UInt8](repeating:0, count:6)
        
        if (riferimento.versetti) {
            if (info.tipo == TestoTipi.Bibbia) {
                for brano in riferimento.brani {
                    for i in stride(from:0, to:6, by:1) {
                        branoDaControllare[i] = brano[i];
                    }
                    // altrimenti quando brano[] è cambiato, il valore originale nell'argomento viene modificato anche
                    if (indiceLibro[Int(branoDaControllare[0]) - 1] != indiceLibro[Int(branoDaControllare[3])])
                    {
                        if (branoDaControllare[1] == 255) {
                            branoDaControllare[1] = 1;
                        }
                        if (branoDaControllare[4] == 255) {
                            branoDaControllare[4] = 1;
                        }
                        if (capitoliInLibro[Int(branoDaControllare[0])] >= branoDaControllare[1] || capitoliInLibro[Int(branoDaControllare[3])] >= branoDaControllare[4])
                        {
                            // c'è testo nella parte richiesta del primo o dell'ultimo libro
                            branoEsiste = true;
                            break;
                        }
                        if (branoDaControllare[3] > branoDaControllare[0] + 1 && indiceLibro[Int(branoDaControllare[0])] != indiceLibro[Int(branoDaControllare[3]) - 1])
                        {
                            // c'è testo nei libri fra il primo e l'ultimo
                            branoEsiste = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (elencaNoteInBrano(riferimento).count() > 0) {
                    branoEsiste = true;
                }
            }
        }
        else // if (riferimento.Versetti)
        {
            for nota in riferimento.note {
                if (!getNotaTesto(nota).isEmpty)
                {
                    branoEsiste = true;
                    break;
                }
            }
        }
        return branoEsiste;
    }
    
    internal func getRadiciDiverse() -> [String]
    {
        var listaRadiciDiverse:[String] = []
        if (info.tipo == TestoTipi.Bibbia)
        {
            var rif:[UInt8]
            for radice in radiciDiverse {
                rif = riferimentoDaNumeroVersetto(radice.occorrenzaRadice.voce);
                listaRadiciDiverse.append(String(rif[0])+"|"+String(rif[1])+"|"+String(rif[2])+"|"+String(radice.occorrenzaRadice.parola)+"|"+radice.nuovaRadice);
            }
        }
        else {
            for radice in radiciDiverse {
                listaRadiciDiverse.append(String(radice.occorrenzaRadice.voce)+"|"+String(radice.occorrenzaRadice.parola)+"|"+radice.nuovaRadice);
            }
        }
        return listaRadiciDiverse;
    }
    
    /*
    func cambiaSolaLettura() {
        switch (info.bloccato)
        {
        case BloccatoTipi.Sbloccato:
            info.bloccato = BloccatoTipi.Bloccato;
            noteModificate = true;
            break;
        case BloccatoTipi.Bloccato:
            info.bloccato = BloccatoTipi.Sbloccato;
            noteModificate = true;
            break;
        case BloccatoTipi.BloccatoSempre: // non fare niente
            break;
        }
}
     */
    
    func readByte() throws -> UInt8 {
        do {
            let data = try handle?.read(upToCount:1)
            return UInt8(data?[0] ?? 0)
        }
        catch {
            throw FileNonValidoException.fileNonValido
        }
    }
    
    func readUInt16() throws -> UInt16 {
        do {
            let data = try handle?.read(upToCount:2)
            return (UInt16(data?[1] ?? 0)*256)+UInt16(data?[0] ?? 0)
        }
        catch {
            throw FileNonValidoException.fileNonValido
        }
    }
    
    func readUInt32() throws -> UInt32 {
        do {
            let data = try handle?.read(upToCount:4)
            return ((((UInt32(data?[3] ?? 0)*256+UInt32(data?[2] ?? 0))*256)+UInt32(data?[1] ?? 0))*256)+UInt32(data?[0] ?? 0)
        }
        catch {
            throw FileNonValidoException.fileNonValido
         }
    }
    
    func readInt16() throws -> Int16 {
        do {
            let data = try handle?.read(upToCount:2)
            return Int16(littleEndian: data?.withUnsafeBytes { $0.load(as: Int16.self) } ?? 0)
        }
        catch {
            throw FileNonValidoException.fileNonValido
        }
    }
    
    func readInt32() throws -> Int32 {
        do {
            let data = try handle?.read(upToCount:4)
            return Int32(littleEndian: data?.withUnsafeBytes { $0.load(as: Int32.self) } ?? 0)
        }
        catch {
            throw FileNonValidoException.fileNonValido
        }
    }
    
    func readBytes(_ n:Int) throws -> [UInt8] {
        do {
            let data = try handle?.read(upToCount:n)
            var o = [UInt8](repeating:0, count:n)
            for i in 0...n-1 {
                o[i] = UInt8(data?[i] ?? 0)
            }
            return o
        }
        catch {
            throw FileNonValidoException.fileNonValido
        }
    }
    
    func readString() throws -> String {
        var length = 0
        var size = 0
        do {
            var byte = try handle?.read(upToCount:1)?[0] ?? 0
            
            while byte >= 128 {
                length |= Int(byte & 0x7F) << size
                size += 7
                byte = try handle?.read(upToCount:1)?[0] ?? 0
            }
            
            length |= Int(byte) << size
            
            let stringData = handle?.readData(ofLength: length) ?? Data()
            return String(data:stringData, encoding: .utf8) ?? ""
        }
        catch {
            throw FileNonValidoException.fileNonValido
        }
    }
}
