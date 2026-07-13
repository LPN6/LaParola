using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using static LaParola.Utilities.Funzioni;

namespace LaParola
{
    partial class Versione : IDisposable
    {
        #region Proprietà

        private readonly VersioneInformazioni info = new();
        public VersioneInformazioni Info
        {
            get { return info; }
        }

        private readonly FileStream fs;
        private readonly BinaryReader br;
        private readonly UInt32 pTesto;
        private readonly UInt32 pIndice;
        private readonly UInt32 pParole;
        private readonly UInt32 pRadici;
        private readonly UInt32 pParoleIndiceIndice;
        private readonly UInt32 pParoleIndice;
        private readonly Texts genitore;

        public List<byte> capitoliInLibro = [];
        public List<byte> versettiInCapitolo = [];
        public List<UInt16> indiceLibro = [];
        public List<UInt16> indiceCapitolo = [];

        private string[]? parole = null;
        private string[]? radici = null;
        private UInt32[]? radiceDiParola = null;
        private StringBuilder[]? paroleDiRadice = null;

        private static readonly ConfrontoCI confrontoParole = new();

        public string[] Parole
        {
            get
            {
                if (parole == null)
                {
                    lock (fileLock)
                    {
                        fs.Seek(pParole, SeekOrigin.Begin);
                        parole = SplitString(br.ReadString(), divisore);
                    }
                }
                return parole;
            }
        }
        public string[] Radici
        {
            get
            {
                if (radici == null)
                {
                    lock (fileLock)
                    {
                        if (pRadici > pInizioDati) // quando ==, non ci sono radici in questa versione
                        {
                            fs.Seek(pRadici, SeekOrigin.Begin);
                            radici = SplitString(br.ReadString(), divisore);
                            pRadiciDiParole = fs.Position;
                        }
                        else
                        {
                            radici = [];
                        }
                    }
                }
                return radici;
            }
        }

        private struct RadiceDiversa
        {
            public OccorrenzaParola OccorrenzaRadice;
            public string NuovaRadice;
        }
        private readonly List<RadiceDiversa> radiciDiverse = [];

        internal List<Int16[]> riferimentiDiversi = [];

        private struct CitazioneRiferimento
        {
            public byte[] Brano;
            public UInt32 NumeroNota;
        }
        private List<CitazioneRiferimento>? citazioniRiferimenti = null;

        public List<string> noteInOrdine = [];

        private readonly List<string> noteTitoli = [];
        public List<string> NoteTitoli
        {
            get { return noteTitoli; }
        }
        private readonly List<int> notePosizione = [];
        private readonly List<string> noteNuoveTesto = [];

        private bool noteModificate;
        public bool NoteModificate
        {
            get { return noteModificate; }
        }

        readonly UInt32 pCitazioniRiferimenti, pInizioDati;
        long pRadiciDiParole = 0;

        private readonly bool isRunningOnMono = false;

        private readonly Object fileLock = new();
        private static readonly char[] divisore = ['|'];

        #endregion

        /// <summary>
        /// Costruttore della classe che descrive un testo nel programma.
        /// </summary>
        /// <param name="testi">La classe genitore che contiene tutti i testi.</param>
        /// <param name="nomeFile">Il nome del file (incluso il percorso) che contiene il testo.</param>
        /// <param name="testoInFile">Il numero (partendo da 0) del testo nel file.</param>
        /// <exception cref="FileNonValidoException">Se c'è un errore nel file.</exception>
        public Versione(Texts testi, string nomeFile, byte testoInFile)
        {
            //                int tick0 = Environment.TickCount;
            //                Trace.WriteLine("  inizio nomeVersione " +nomeFile+" "+ (Environment.TickCount - tick0).ToString());

            genitore = testi;
            info.NomeDelFile = nomeFile;
            isRunningOnMono = (Type.GetType("Mono.Runtime") != null);
            try
            {
                fs = new FileStream(nomeFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                br = new BinaryReader(fs);
                char[] c = br.ReadChars(3);
                if (!(c[0].Equals('L') && c[1].Equals('P') && c[2].Equals('N')))
                {
                    throw new FileNonValidoException();
                }

                byte[] versioneByte = br.ReadBytes(4);
                // la versione del programma deve essere dopo quella del testo
                Version? versione = Assembly.GetExecutingAssembly().GetName().Version;
                if (versione == null || versione.Major < versioneByte[0] || (versione.Major == versioneByte[0] && versione.Minor < versioneByte[1]))
                {
                    throw new FileNonValidoException();
                }

                info.Versione = versioneByte[0].ToString(CultureInfo.InvariantCulture) + "." + versioneByte[1].ToString(CultureInfo.InvariantCulture) + "." + versioneByte[2].ToString(CultureInfo.InvariantCulture);

                UInt32 pInizioTesto = 0;
                for (byte i = 0; i <= testoInFile; ++i)
                {
                    pInizioTesto = br.ReadUInt32();
                }

                fs.Seek(pInizioTesto, SeekOrigin.Begin);
                pInizioDati = br.ReadUInt32();
                info.Nome = br.ReadString();
                info.Abbreviazione = br.ReadString();
                info.Titolo = br.ReadString();
                // il campo dell'autore è stato introdotto nella versione 7.08 del programma
                if (versioneByte[0] >= (byte)8 || (versioneByte[0] >= (byte)7 && versioneByte[1] >= (byte)8))
                {
                    info.Autore = br.ReadString();
                }

                info.CasaEditrice = br.ReadString();
                info.Data = br.ReadString();
                info.Copyright = br.ReadString();
                info.Isbn = br.ReadString();
                info.Descrizione = br.ReadString();
                info.Lingua = br.ReadString();
                info.VersioneDelleNote = br.ReadString();
                // il campo sul bloccaggio è stato introdotto nella versione 7.08 del programma
                if (versioneByte[0] >= (byte)8 || (versioneByte[0] >= (byte)7 && versioneByte[1] >= (byte)8))
                {
                    info.Bloccato = (BloccatoTipi)br.ReadByte();
                }

                byte tipo = br.ReadByte();

                fs.Seek(pInizioDati, SeekOrigin.Begin);
                pTesto = br.ReadUInt32() + pInizioDati;
                UInt32 pIndiceLibriCapitoli = 0, pIndiceNote = 0;
                switch (tipo)
                {
                    case 0:
                        pIndiceLibriCapitoli = br.ReadUInt32() + pInizioDati;
                        break;
                    case 1:
                        pIndiceNote = br.ReadUInt32() + pInizioDati;
                        break;
                }
                pIndice = br.ReadUInt32() + pInizioDati;
                pParole = br.ReadUInt32() + pInizioDati;
                pParoleIndiceIndice = br.ReadUInt32() + pInizioDati;
                pParoleIndice = br.ReadUInt32() + pInizioDati;
                pRadici = br.ReadUInt32() + pInizioDati;
                UInt32 pRadiciDiverse = br.ReadUInt32() + pInizioDati;
                UInt32 pRiferimentiDiversi = br.ReadUInt32() + pInizioDati;
                pCitazioniRiferimenti = br.ReadUInt32() + pInizioDati;
                UInt32 pNoteInOrdine = br.ReadUInt32() + pInizioDati;

                //                    Trace.WriteLine("  indici " + (Environment.TickCount - tick0).ToString());
                switch (tipo)
                {
                    case 0:
                        #region Bibbia
                        info.Tipo = TestoTipi.Bibbia;

                        fs.Seek(pIndiceLibriCapitoli, SeekOrigin.Begin);
                        UInt16 somma = 0;
                        capitoliInLibro.Add(0);
                        indiceLibro.Add(0);
                        byte[] capitoliArray = br.ReadBytes(73);
                        for (int i = 0; i < 73; ++i)
                        {
                            capitoliInLibro.Add(capitoliArray[i]);
                            somma += capitoliArray[i];
                            indiceLibro.Add(somma);
                        }
                        versettiInCapitolo.Add(0);
                        indiceCapitolo.Add(0);
                        byte[] versettiArray = br.ReadBytes(somma);
                        somma = 0;
                        int numeroVersetto = 0;
                        for (int i = 1; i <= 73; ++i)
                        {
                            for (int j = 1; j <= capitoliInLibro[i]; ++j)
                            {
                                versettiInCapitolo.Add(versettiArray[numeroVersetto]);
                                somma += versettiArray[numeroVersetto];
                                indiceCapitolo.Add(somma);
                                ++numeroVersetto;
                            }
                        }

                        break;
                        #endregion
                    case 1:
                        #region Note

                        fs.Seek(pIndiceNote, SeekOrigin.Begin);
                        noteTitoli.AddRange(SplitString(br.ReadString(), '|'));
                        int numeroNote = noteTitoli.Count;
                        notePosizione.Capacity = numeroNote;
                        bool commentario = (numeroNote == 0); // collezione vuota automaticamente di tutto e due i tipi
                        bool dizionario = (numeroNote == 0);
                        for (int i = 0; i < numeroNote; ++i)
                        {
                            if (noteTitoli[i].StartsWith('#'))
                            {
                                commentario = true;
                            }
                            else
                            {
                                dizionario = true;
                            }

                            notePosizione.Add(i);
                        }
                        if (commentario)
                        {
                            info.Tipo = TestoTipi.Commentario;
                        }

                        if (dizionario)
                        {
                            info.Tipo |= TestoTipi.Dizionario;
                        }

                        break;
                        #endregion
                    default:
                        throw new FileNonValidoException();
                }

                if (pRadiciDiverse > pInizioDati)
                {
                    fs.Seek(pRadiciDiverse, SeekOrigin.Begin);
                    UInt32 nRadiciDiverse = br.ReadUInt32();
                    switch (tipo)
                    {
                        case 0:
                            byte[] riferimento = new byte[3];
                            byte[] riferimento6 = new byte[6];
                            UInt16[] versetto = new UInt16[2];
                            for (UInt32 i = 0; i < nRadiciDiverse; ++i)
                            {
                                riferimento = br.ReadBytes(3);
                                for (int j = 0; j <= 2; ++j)
                                {
                                    riferimento6[j] = riferimento[j];
                                    riferimento6[j + 3] = riferimento[j];
                                }
                                versetto = NumeroVersettoDaRiferimento(riferimento6);
                                OccorrenzaParola op = new()
                                {
                                    Voce = versetto[0],
                                    Parola = br.ReadUInt16()
                                };
                                RadiceDiversa rd = new()
                                {
                                    OccorrenzaRadice = op,
                                    NuovaRadice = br.ReadString()
                                };
                                radiciDiverse.Add(rd);
                            }
                            break;
                        case 1:
                            for (UInt32 i = 0; i < nRadiciDiverse; ++i)
                            {
                                OccorrenzaParola op = new()
                                {
                                    Voce = br.ReadUInt32(),
                                    Parola = br.ReadUInt16()
                                };
                                RadiceDiversa rd = new()
                                {
                                    OccorrenzaRadice = op,
                                    NuovaRadice = br.ReadString()
                                };
                                radiciDiverse.Add(rd);
                            }
                            break;
                    }
                }

                //                    Trace.WriteLine("  rad diverse " + (Environment.TickCount - tick0).ToString());

                if (pRiferimentiDiversi > pInizioDati) // quando ==, non ci sono riferimenti diversi in questa versione
                {
                    fs.Seek(pRiferimentiDiversi, SeekOrigin.Begin);
                    UInt32 nRiferimentiDiversi = br.ReadUInt32();
                    //                        byte[] riferimentiDiversiArray = br.ReadBytes(12 * (int)nRiferimentiDiversi);
                    //                        int i12;
                    for (int i = 0; i < nRiferimentiDiversi; ++i)
                    {
                        //                            i12 = i * 12;
                        //                            riferimentiDiversi.Add(new Int16[] { (Int16)(256 * riferimentiDiversiArray[i12 + 1] + riferimentiDiversiArray[i12]),
                        //                            (Int16)(256 * riferimentiDiversiArray[i12 + 3] + riferimentiDiversiArray[i12+2]),
                        //                            (Int16)(256 * riferimentiDiversiArray[i12 + 5] + riferimentiDiversiArray[i12+4]),
                        //                            (Int16)(256 * riferimentiDiversiArray[i12 + 7] + riferimentiDiversiArray[i12+6]),
                        //                            (Int16)(256 * riferimentiDiversiArray[i12 + 9] + riferimentiDiversiArray[i12+8]),
                        //                            (Int16)(256 * riferimentiDiversiArray[i12 + 11] + riferimentiDiversiArray[i12+10])});
                        riferimentiDiversi.Add([br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16()]);
                    }
                }


                if (pNoteInOrdine > pInizioDati) // quando ==, non ci sono note in ordine
                {
                    fs.Seek(pNoteInOrdine, SeekOrigin.Begin);
                    UInt32 nNoteInOrdine = br.ReadUInt32();
                    for (int i = 0; i < nNoteInOrdine; ++i)
                    {
                        noteInOrdine.Add(br.ReadString());
                    }

                    if (nNoteInOrdine > 0)
                    {
                        info.Tipo |= TestoTipi.Libro;
                    }
                }
            }
            catch
            {
                throw new FileNonValidoException();
            }
        }

        internal void CreaListaRadiceDiParole()
        {
            lock (fileLock)
            {
                if (radiceDiParola == null)
                {
                    int numeroParole = Parole.Length;
                    int numeroRadici = Radici.Length; // serve solo per costringere la lettura delle radici, che imposta pRadiciDiParole correttamente
                    radiceDiParola = new UInt32[numeroParole];
                    if (numeroRadici > 0 && pRadiciDiParole > 0)
                    { // quando pRadiciDiParole==0 (valore predefinito), non ci sono radici in questa versione
                      // numeroRadici>0 quindi non è necessario, ma è incluso per fare sì che la riga che definisce numeroRadici è usata
                        fs.Seek(pRadiciDiParole, SeekOrigin.Begin);
                        byte[] radiciArray = br.ReadBytes(numeroParole * 4);
                        int i4;
                        for (int i = 0; i < numeroParole; ++i)
                        {
                            i4 = 4 * i;
                            radiceDiParola[i] = (UInt32)(256 * (256 * (256 * radiciArray[i4 + 3] + radiciArray[i4 + 2]) + radiciArray[i4 + 1]) + radiciArray[i4]);
                        }
                    }
                }
            }
        }

        internal void CreaListaCitazioni()
        {
            lock (fileLock)
            {
                if (citazioniRiferimenti == null)
                {
                    citazioniRiferimenti = [];
                    if (pCitazioniRiferimenti > pInizioDati) // quando ==, non ci sono collegamenti a riferimenti
                    {
                        fs.Seek(pCitazioniRiferimenti, SeekOrigin.Begin);
                        UInt32 nCitazioniRiferimenti = br.ReadUInt32();
                        CitazioneRiferimento citazione;
                        int i10;
                        byte[] citazioniArray = br.ReadBytes(10 * (int)nCitazioniRiferimenti);
                        for (int i = 0; i < nCitazioniRiferimenti; ++i)
                        {
                            i10 = 10 * i;
                            citazione.Brano = [citazioniArray[i10 + 0], citazioniArray[i10 + 1], citazioniArray[i10 + 2], citazioniArray[i10 + 3], citazioniArray[i10 + 4], citazioniArray[i10 + 5]];
                            citazione.NumeroNota = (UInt32)(256 * (256 * (256 * citazioniArray[i10 + 9] + citazioniArray[i10 + 8]) + citazioniArray[i10 + 7]) + citazioniArray[i10 + 6]);
                            citazioniRiferimenti.Add(citazione);
                        }
                    }
                }
            }
        }

        #region Chiusura

        internal void Rimuovi()
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

        internal void Cancella()
        {
            // chiude il testo e cancella il file che lo contiene
            Rimuovi();
            File.Delete(info.NomeDelFile);
        }
        public void Chiudi()
        {
            string nomeVersione = info.Nome;
            string nomeFile = nomeVersione;

            if (noteModificate)
            {
                SortedDictionary<string, List<OccorrenzaParola>> chiave = new(confrontoParole);

                int suffisso = 0;
                while (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeFile + ".laparola"))
                {
                    suffisso += 1;
                    nomeFile = nomeVersione + suffisso.ToString(CultureInfo.InvariantCulture);
                }
                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeFile + ".laparola";

                try
                {
                    using var fsNuovo = new FileStream(nomeFile, FileMode.Create, FileAccess.Write);
                    using var bwNuovo = new BinaryWriter(fsNuovo);

                    Version versioneApp = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(8, 0, 0);
                    bwNuovo.Write(['L', 'P', 'N', System.Convert.ToChar(versioneApp.Major), System.Convert.ToChar(versioneApp.Minor), System.Convert.ToChar(versioneApp.Build), (char)1]);
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

                    RichTextBoxEx rtb = new(); // TODO2
                    for (UInt32 i = 0; i < numeroNote; ++i)
                    {
                        nuoviTesti[i] = GetNotaTestoTitolo(noteTitoli[(int)i]);
                        try
                        {
                            rtb.Rtf = nuoviTesti[i];
                            chiave = Texts.TrovaParoleInVoce(rtb.Text, i, chiave, info.Lingua);
                        }
                        catch
                        {
                            chiave = Texts.TrovaParoleInVoce(nuoviTesti[i], i, chiave, info.Lingua);
                        }
                    }
                    UInt32[] indici = new UInt32[2];
                    indici = Texts.ScriviNote(bwNuovo, pInizioDati, [.. noteTitoli], nuoviTesti);
                    inizioTestoIndiceLC = indici[0];
                    inizioTestoIndice = indici[1];

                    UInt32 inizioParole = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
                    StringBuilder parole = new("");
                    foreach (string s in chiave.Keys)
                    {
                        parole.Append(s).Append('|');
                    }

                    bwNuovo.Write(parole.ToString());

                    UInt32 inizioParoleIndiceIndice = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
                    UInt32 numeroApparenze = 0;
                    byte[] datiDaScrivere = new byte[4 * chiave.Count + 4];
                    MemoryStream ms = new(datiDaScrivere, true);
                    BinaryWriter bwMemoria = new(ms);
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
                        MemoryStream msParoleIndice = new(datiDaScrivereParoleIndice, true);
                        BinaryWriter bwMemoriaParoleIndice = new(msParoleIndice);
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
                        StringBuilder listaRadici = new("");
                        for (int i = 0; i < numeroRadici; ++i)
                        {
                            listaRadici.Append(Radici[i]).Append('|');
                        }

                        bwNuovo.Write(listaRadici.ToString());
                        foreach (string s in chiave.Keys)
                        {
                            bwNuovo.Write(RadiceNumeroDiParola(s));
                        }
                    }
                    else
                    {
                        inizioRadici = 0;
                    }

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
                    {
                        inizioRadiciDiverse = 0;
                    }

                    UInt32 inizioRiferimentiDiversi = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
                    int numeroRiferimentiDiversi = riferimentiDiversi.Count;
                    if (numeroRiferimentiDiversi > 0)
                    {
                        bwNuovo.Write((UInt32)numeroRiferimentiDiversi);
                        for (int i = 0; i < numeroRiferimentiDiversi; ++i)
                        {
                            for (int j = 0; j < 6; ++j)
                            {
                                bwNuovo.Write(riferimentiDiversi[i][j]);
                            }
                        }
                    }
                    else
                    {
                        inizioRiferimentiDiversi = 0;
                    }

                    UInt32 inizioRiferimentiCitati = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
                    if (!genitore.ScriviRiferimentiCitati(bwNuovo, nuoviTesti))
                    {
                        inizioRiferimentiCitati = 0;
                    }

                    UInt32 inizioNoteInOrdine = (UInt32)(bwNuovo.Seek(0, SeekOrigin.Current)) - pInizioDati;
                    int numeroNoteInOrdine = noteInOrdine.Count;
                    if (numeroNoteInOrdine > 0)
                    {
                        bwNuovo.Write((UInt32)numeroNoteInOrdine);
                        for (int i = 0; i < numeroNoteInOrdine; ++i)
                        {
                            bwNuovo.Write(noteInOrdine[i]);
                        }
                    }
                    else
                    {
                        inizioNoteInOrdine = 0;
                    }

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

                    try { br?.Dispose(); } catch { }
                    try { fs?.Dispose(); } catch { }

                    if (nomeFile != info.NomeDelFile)
                    {
                        File.Move(nomeFile, info.NomeDelFile, overwrite: true);
                    }
                }
                catch (Exception)
                {
                    // Catches any write failures or file swapping access exceptions
                    throw new ImpossibileScrivereModificheException();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
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
        }

        ~Versione()
        {
            Dispose(false);
        }

        #endregion

        #region Riferimento

        private byte[] RiferimentoDaNumeroVersetto(UInt32 numeroVersetto)
        {
            byte libro = 0;
            UInt16 capitolo = 0;
            do
            {
                ++capitolo;
            }
            while (indiceCapitolo[capitolo] < numeroVersetto);
            do
            {
                ++libro;
            }
            while (indiceLibro[libro] < capitolo);
            byte b1 = (byte)(capitolo - indiceLibro[libro - 1]);
            byte b2 = (byte)(numeroVersetto - indiceCapitolo[capitolo - 1]);
            byte[] rif = [libro, b1, b2, libro, b1, b2];
            return rif;
        }

        private UInt16[] NumeroVersettoDaRiferimento(byte[] riferimento)
        {
            UInt16 inizio, fine;
            byte b1 = riferimento[1];
            if (b1 > capitoliInLibro[riferimento[0]])
            {
                b1 = capitoliInLibro[riferimento[0]];
            }

            byte b2 = riferimento[2];
            if (b2 > versettiInCapitolo[indiceLibro[riferimento[0] - 1] + b1])
            {
                b2 = versettiInCapitolo[indiceLibro[riferimento[0] - 1] + b1];
            }

            byte b4 = riferimento[4];
            if (b4 > capitoliInLibro[riferimento[3]])
            {
                b4 = capitoliInLibro[riferimento[3]];
            }

            byte b5 = riferimento[5];
            if (b5 > versettiInCapitolo[indiceLibro[riferimento[3] - 1] + b4])
            {
                b5 = versettiInCapitolo[indiceLibro[riferimento[3] - 1] + b4];
            }

            inizio = (UInt16)(indiceCapitolo[indiceLibro[riferimento[0] - 1] + b1 - 1] + b2);
            fine = (UInt16)(indiceCapitolo[indiceLibro[riferimento[3] - 1] + b4 - 1] + b5);
            UInt16[] numeroVersetto = [inizio, fine];
            return numeroVersetto;
        }

        #endregion riferimento

        #region Ricerca

        public Riferimento RicercaRadiceInBrano(string radice, Riferimento branoDaRicercare)
        {
            // se branoDaRicerca non contiene brani, tutta la Bibbia (o collezione di note) è ricercata
            if (branoDaRicercare.Brani.Count == 0)
            {
                return RicercaRadiceInBrano(radice);
            }
            else
            {
                return RestringiRiferimentoABrano(OccorrenzeRadice(radice), branoDaRicercare);
            }
        }

        public Riferimento RicercaRadiceInBrano(string radice)
        {
            return ConvertiOccorrenzeARiferimento(OccorrenzeRadice(radice));
        }

        private List<OccorrenzaParola> OccorrenzeRadice(string radice)
        {
            List<OccorrenzaParola> occorrenze = [];
            string[] paroleDaRicercare = SplitString(ParoleNumeriDiRadice(radice), '|');
            foreach (string parolaDaRicercare in paroleDaRicercare)
            {
                occorrenze.AddRange(OccorrenzeParola(Convert.ToInt32(parolaDaRicercare, CultureInfo.InvariantCulture), true));
            }

            occorrenze.AddRange(OccorrenzeRadiceDiversa(radice));
            occorrenze.Sort();
            return occorrenze;
        }

        /// <summary>
        /// Trova tutti i versetti in un brano che contengono una parola.
        /// Se la parola non esiste nella versione, un riferimento vuoto è restituito.
        /// </summary>
        /// <param name="parola">La parola da ricercare.</param>
        /// <param name="branoDaRicercare">Il brano in cui cercare la parola.</param>
        /// <returns>Il riferimento di tutti i versetti.</returns>
        public Riferimento RicercaParolaInBrano(string parola, Riferimento branoDaRicercare)
        {
            // se branoDaRicerca non contiene brani, tutta la Bibbia (o collezione di note) è ricercata
            if (branoDaRicercare.Brani.Count == 0)
            {
                return RicercaParolaInBrano(parola);
            }

            return RestringiRiferimentoABrano(RicercaParola(parola), branoDaRicercare);
        }

        /// <summary>
        /// Trova tutti i versetti nella Bibbia che contengono una parola.
        /// Se la parola non esiste nella versione, un riferimento vuoto è restituito.
        /// </summary>
        /// <param name="parola">La parola da ricercare.</param>
        /// <returns>Il riferimento di tutti i versetti.</returns>
        public Riferimento RicercaParolaInBrano(string parola)
        {
            return ConvertiOccorrenzeARiferimento(RicercaParola(parola));
        }

        private Riferimento RestringiRiferimentoABrano(List<OccorrenzaParola> occorrenze, Riferimento branoDaRicercare)
        {
            Riferimento occorrenzeInBrano = new((info.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia);
            int numeroBrani = branoDaRicercare.Brani.Count;
            foreach (OccorrenzaParola op in occorrenze)
            {
                if (occorrenzeInBrano.Versetti)
                {
                    List<int> inizioBrani = [];
                    List<int> fineBrani = [];
                    UInt16[] numeroVersetto;
                    foreach (byte[] b in branoDaRicercare.Brani)
                    {
                        numeroVersetto = NumeroVersettoDaRiferimento(b);
                        inizioBrani.Add(numeroVersetto[0]);
                        fineBrani.Add(numeroVersetto[1]);
                    }
                    for (int i = 0; i < numeroBrani; ++i)
                    {
                        if (inizioBrani[i] <= op.Voce && fineBrani[i] >= op.Voce)
                        {
                            occorrenzeInBrano.Brani.Add(RiferimentoDaNumeroVersetto(op.Voce));
                            List<UInt16> lista =
                            [
                                op.Parola
                            ];
                            occorrenzeInBrano.numeroParola.Add(lista);
                            break;
                        }
                    }
                }
                else
                {
                    string nomeNota;
                    byte libro, capitolo, versetto;
                    for (int i = 0; i < numeroBrani; ++i)
                    {
                        nomeNota = noteTitoli[(int)(op.Voce)];
                        if (nomeNota.StartsWith('#')) // altrimenti fa parte di un dizionario
                        {
                            libro = Convert.ToByte(nomeNota.Substring(1, 2), CultureInfo.InvariantCulture);
                            capitolo = Convert.ToByte(nomeNota.Substring(3, 3), CultureInfo.InvariantCulture);
                            versetto = Convert.ToByte(nomeNota.Substring(6, 3), CultureInfo.InvariantCulture);
                            if ((branoDaRicercare.Brani[i][0] < libro
                                || (branoDaRicercare.Brani[i][0] == libro && branoDaRicercare.Brani[i][1] < capitolo)
                                || (branoDaRicercare.Brani[i][0] == libro && branoDaRicercare.Brani[i][1] == capitolo && branoDaRicercare.Brani[i][2] <= versetto))
                                &&
                                (branoDaRicercare.Brani[i][3] > libro
                                || (branoDaRicercare.Brani[i][3] == libro && branoDaRicercare.Brani[i][4] > capitolo)
                                || (branoDaRicercare.Brani[i][3] == libro && branoDaRicercare.Brani[i][4] == capitolo && branoDaRicercare.Brani[i][5] >= versetto)))
                            {
                                try
                                {
                                    occorrenzeInBrano.Note.Add(noteTitoli[(int)(op.Voce)]);
                                    List<UInt16> lista =
                                    [
                                        op.Parola
                                    ];
                                    occorrenzeInBrano.numeroParola.Add(lista);
                                }
                                catch
                                {
                                    // la prima riga sopra può dare un errore se una nota è stata cancellata, e quindi op.Voce>noteTitoli.Count
                                }
                            }
                        }
                    }
                }
            }
            return occorrenzeInBrano;
        }

        private Riferimento ConvertiOccorrenzeARiferimento(List<OccorrenzaParola> occorrenze)
        {
            Riferimento occorrenzeInBibbia = new((info.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia);
            foreach (OccorrenzaParola op in occorrenze)
            {
                if (occorrenzeInBibbia.Versetti)
                {
                    occorrenzeInBibbia.Brani.Add(RiferimentoDaNumeroVersetto(op.Voce));
                    List<UInt16> lista =
                    [
                        op.Parola
                    ];
                    occorrenzeInBibbia.numeroParola.Add(lista);
                }
                else
                {
                    try
                    {
                        occorrenzeInBibbia.Note.Add(noteTitoli[(int)(op.Voce)]);
                        List<UInt16> lista =
                        [
                            op.Parola
                        ];
                        occorrenzeInBibbia.numeroParola.Add(lista);
                    }
                    catch
                    {
                        // la prima riga sopra può dare un errore se una nota è stata cancellata, e quindi op.Voce>noteTitoli.Count
                    }
                }
            }
            return occorrenzeInBibbia;
        }

        private List<OccorrenzaParola> RicercaParola(string parola)
        {
            CreaListaRadiceDiParole();

            List<OccorrenzaParola> occorrenze = [];
            bool cercaRadice = false, cercaRadiceDiParola = false;

            if (parola.StartsWith('\\')) // tutte le parole con la stessa radice della parola
            {
                cercaRadiceDiParola = true;
                cercaRadice = true; // perché la ricerca sarà convertita in /(radice della parola)
                parola = parola[1..];
            }
            if (parola.StartsWith('/')) // tutte le parole della radice
            {
                cercaRadice = true;
                parola = parola[1..];
            }
            if (parola.IndexOf('*') > -1 || parola.IndexOf('?') > -1)
            {
                Regex regExpParola = new("^" + parola.Replace("?", ".").Replace("*", @".*") + "$");
                int numeroDiParole = Parole.Length;
                for (int i = 0; i < numeroDiParole; ++i)
                {
                    if (regExpParola.IsMatch(Parole[i]))
                    {
                        String radiceDaRicercare = Parole[i];
                        if (cercaRadiceDiParola)
                        {
                            radiceDaRicercare = Radici[(int)(radiceDiParola[i])];
                        }

                        if (cercaRadice)
                        {
                            string[] paroleDaRicercare = SplitString(ParoleNumeriDiRadice(radiceDaRicercare), '|');
                            foreach (string parolaDaRicercare in paroleDaRicercare)
                            {
                                occorrenze.AddRange(OccorrenzeParola(Convert.ToInt32(parolaDaRicercare, CultureInfo.InvariantCulture), true));
                            }

                            occorrenze.AddRange(OccorrenzeRadiceDiversa(radiceDaRicercare));
                        }
                        else
                        {
                            occorrenze.AddRange(OccorrenzeParola(i));
                        }
                    }
                }
            }
            else if (!String.IsNullOrEmpty(parola))
            {
                if (cercaRadiceDiParola)
                {
                    if (Radici.Length > 0)
                    {
                        int numeroParola = NumeroDiParola(parola);
                        if (numeroParola >= 0)
                        {
                            parola = Radici[(int)(radiceDiParola[numeroParola])];
                        }
                        else
                        {
                            parola = ""; // parola non esiste in questo testo
                        }
                    }
                    else
                    {
                        // cerchiamo "parola" anche quando la ricerca è per \parola
                        cercaRadice = false;
                    }
                }
                if (cercaRadice)
                {
                    string[] paroleDaRicercare = SplitString(ParoleNumeriDiRadice(parola), '|');
                    foreach (string parolaDaRicercare in paroleDaRicercare)
                    {
                        occorrenze.AddRange(OccorrenzeParola(Convert.ToInt32(parolaDaRicercare, CultureInfo.InvariantCulture), true));
                    }

                    occorrenze.AddRange(OccorrenzeRadiceDiversa(parola));
                }
                else
                {
                    occorrenze.AddRange(OccorrenzeParola(NumeroDiParola(parola))); // anche se negativo, funziona perché OccorrenzeParola resitutisce niente
                }
            }

            occorrenze.Sort();
            return occorrenze;
        }

        private List<OccorrenzaParola> OccorrenzeRadiceDiversa(string radice)
        {
            // restituisce una lista con tutte le occorrenze di una radice quando non è la radice normale della parola
            List<OccorrenzaParola> occorrenze = [];
            for (int i = 0; i < radiciDiverse.Count; ++i)
            {
                if (radiciDiverse[i].NuovaRadice.Equals(radice, StringComparison.CurrentCultureIgnoreCase))
                {
                    occorrenze.Add(radiciDiverse[i].OccorrenzaRadice);
                }
            }
            return occorrenze;
        }

        private List<OccorrenzaParola> OccorrenzeParola(int nParola, bool solaRadiceNormale)
        {
            // restituisce una lista con tutte le occorrenze di una parola; con la radice normale oppure solo quando non c'è una radice diversa

            CreaListaRadiceDiParole();

            List<OccorrenzaParola> occorrenze = [];
            if (nParola >= 0)
            {
                int nByte;
                byte[] occArray;
                lock (fileLock)
                {
                    fs.Seek(pParoleIndiceIndice + 4 * nParola, SeekOrigin.Begin);
                    UInt32 inizioVersetti = br.ReadUInt32();
                    UInt32 fineVersetti = br.ReadUInt32();
                    fs.Seek(pParoleIndice + inizioVersetti, SeekOrigin.Begin);
                    nByte = (int)(fineVersetti - inizioVersetti);
                    occArray = new byte[nByte];
                    br.Read(occArray, 0, nByte);
                }
                int nOccorrenze = nByte / 6; // 6 perché ogni occorrenza prende 6 byte (UInt32 + UInt16)
                string radice = "";
                if (solaRadiceNormale)
                {
                    radice = RadiceDiParola(Parole[nParola]);
                }

                for (int i = 0; i < nOccorrenze; ++i)
                {
                    OccorrenzaParola op = new()
                    {
                        Voce = (UInt32)(16777216 * occArray[6 * i + 3] + 65536 * occArray[6 * i + 2] + 256 * occArray[6 * i + 1] + occArray[6 * i]),
                        Parola = (UInt16)(256 * occArray[6 * i + 5] + occArray[6 * i + 4])
                    };
                    if (!solaRadiceNormale)
                    {
                        occorrenze.Add(op);
                    }
                    else
                    {
                        bool radiceEDiversa = false;
                        for (int j = 0; j < radiciDiverse.Count; ++j)
                        {
                            if (radiciDiverse[j].OccorrenzaRadice.CompareTo(op) == 0)
                            {
                                radiceEDiversa = (radiciDiverse[j].NuovaRadice != radice);
                                if (radiceEDiversa)
                                {
                                    break;
                                }
                            }
                        }
                        if (!radiceEDiversa)
                        {
                            occorrenze.Add(op);
                        }
                    }
                }
            }
            return occorrenze;
        }

        private List<OccorrenzaParola> OccorrenzeParola(int nParola)
        {
            // restituisce una lista con tutte le occorrenze di una parola
            return OccorrenzeParola(nParola, false);
        }

        #endregion

        #region Parole e Radici

        public bool EsistonoRadici()
        {
            return (Radici.Length > 0);
        }

        private int NumeroDiParola(string parola)
        {
            if (string.IsNullOrEmpty(parola))
            {
                return -1;
            }
            else
            { // BinarySearch non funziona sempre con parole greche, neanche con confrontoParole
                if (IsLetteraGreca(parola[0]))
                {
                    return Array.IndexOf(Parole, parola.ToLower(CultureInfo.InvariantCulture));
                }
                else
                {
                    return Array.BinarySearch(Parole, parola, confrontoParole);
                }
            }
        }

        public int NumeroVolteParola(string parola)
        {
            int numeroVolte;
            int numeroParola = NumeroDiParola(parola);
            if (numeroParola >= 0)
            {
                lock (fileLock)
                {
                    fs.Seek(pParoleIndiceIndice + 4 * numeroParola, SeekOrigin.Begin);
                    int inizioVersetti = (int)br.ReadUInt32();
                    numeroVolte = ((int)br.ReadUInt32() - inizioVersetti) / 6;
                }
            }
            else
            {
                numeroVolte = 0;
            }

            return numeroVolte;
        }

        internal byte[] GetApparenzeParole()
        {
            fs.Seek(pParoleIndice - 4, SeekOrigin.Begin);
            int count = (int)br.ReadUInt32();
            return br.ReadBytes(count);
        }

        /// <summary>
        /// Il numero di occorrenze delle parole che hanno questa radice, cioè non considera quando una di queste
        /// parole ha una radice diversa, oppure parole con altre radici con questa come radice diversa.
        /// </summary>
        /// <param name="radice">La radice di cui si vuole il numero di occorrenze.</param>
        /// <returns>Il numero di occorrenze.</returns>
        public int NumeroVolteRadice(string radice)
        {
            string[] paroleNumeri = SplitString(ParoleNumeriDiRadice(radice), '|');
            int numeroVolte = 0;
            lock (fileLock)
            {
                foreach (string parolaNumero in paroleNumeri)
                {
                    fs.Seek(pParoleIndiceIndice + 4 * Convert.ToInt32(parolaNumero, CultureInfo.InvariantCulture), SeekOrigin.Begin);
                    int inizioVersetti = (int)br.ReadUInt32();
                    numeroVolte += ((int)br.ReadUInt32() - (int)inizioVersetti) / 6;
                }
            }
            foreach (RadiceDiversa radiceDiversa in radiciDiverse)
            {
                if (radiceDiversa.NuovaRadice == radice)
                {
                    ++numeroVolte;
                }
            }
            return numeroVolte;
        }

        public string RadiceDiParola(string parola)
        {
            // la radice normale, non un'eventuale radice diversa

            if (Radici.Length == 0)
            {
                return "";
            }

            CreaListaRadiceDiParole();

            int numeroParola = NumeroDiParola(parola);
            return ((numeroParola >= 0) ? Radici[(int)(radiceDiParola[numeroParola])] : "");
        }

        public UInt32 RadiceNumeroDiParola(string parola)
        {
            // la radice normale, non un'eventuale radice diversa
            if (Radici.Length == 0)
            {
                return (UInt32)(Array.BinarySearch(Radici, "*", confrontoParole));
            }

            CreaListaRadiceDiParole();

            int numeroParola = NumeroDiParola(parola);
            return ((numeroParola >= 0) ? (radiceDiParola[numeroParola]) : (UInt32)(Array.BinarySearch(Radici, "*", confrontoParole)));
        }

        public Collection<string> ParoleDiRadice(string radice)
        {
            // le parole che solitamente hanno questa radice, non altre parole che la hanno a volte come radice diversa
            string[] paroleNumeri = SplitString(ParoleNumeriDiRadice(radice), '|');
            Collection<string> paroleDiRadice = [];
            foreach (string rn in paroleNumeri)
            {
                paroleDiRadice.Add(Parole[Convert.ToInt32(rn, CultureInfo.InvariantCulture)]);
            }

            return paroleDiRadice;
        }

        private string ParoleNumeriDiRadice(string radice)
        {
            // restituisce tutte le parole di una certa radice - restituisce una stringa con i numeri delle parole separati da |
            int numeroRadice = Array.BinarySearch(Radici, radice, confrontoParole);
            if (numeroRadice >= 0)
            {
                if (paroleDiRadice == null)
                // siccome la creazione di paroleDiRadice richiede un po' di tempo, lo facciamo solo la prima volta che è necessario
                {
                    CreaListaRadiceDiParole();

                    int numeroRadici = Radici.Length;
                    paroleDiRadice = new StringBuilder[numeroRadici];
                    for (int i = 0; i < numeroRadici; ++i)
                    {
                        paroleDiRadice[i] = new StringBuilder();
                    }

                    int numeroParole = Parole.Length;
                    for (UInt32 i = 0; i < numeroParole; ++i)
                    {
                        paroleDiRadice[radiceDiParola[i]].Append(i.ToString(CultureInfo.InvariantCulture)).Append('|');
                    }
                }
                return paroleDiRadice[numeroRadice].ToString();
            }
            else
            {
                return "";
            }
        }

        public void AggiungiRadiciAllaVersione(string[] elencoRadici, string[] radiceStringaDiParole)
        {
            CreaListaRadiceDiParole();

            if (radici == null)
            {
                radici = new string[elencoRadici.Length];
            }
            else
            {
                Array.Resize(ref radici, elencoRadici.Length);
            }

            elencoRadici.CopyTo(radici, 0);
            int numeroParole = parole.Length;
            for (int i = 0; i < numeroParole; ++i)
            {
                radiceDiParola[i] = (UInt32)(Array.BinarySearch(radici, radiceStringaDiParole[i], confrontoParole));
            }

            noteModificate = true;
            paroleDiRadice = null;
        }

        #endregion

        #region Note

        /// <summary>
        /// Trova una nota con un certo titolo.
        /// </summary>
        /// <param name="titolo">Il titolo da cercare.</param>
        /// <returns>Il numero della nota se esiste una nota con quel titolo, altrimenti un numero negativo.</returns>
        public int GetNumeroNotaTitolo(string titolo)
        {
            if (string.IsNullOrEmpty(titolo))
            {
                return -1;
            }

            int numeroNota = noteTitoli.BinarySearch(titolo, new ConfrontoCS());
            if (numeroNota < 0)
            {
                numeroNota = noteTitoli.BinarySearch(titolo, confrontoParole);
            }

            return numeroNota;
        }

        public string GetNotaTestoTitolo(string titolo)
        {
            if (string.IsNullOrEmpty(titolo))
            {
                return "";
            }

            // prima cerchiamo la nota con esattamente lo stesso titolo, poi con lettere minuscole
            int numeroNota = GetNumeroNotaTitolo(titolo);
            if (numeroNota < 0)
            {
                return "";
            }
            else // numeroNota>=0
            {
                if (notePosizione[numeroNota] >= 0)
                {
                    string testo;
                    lock (fileLock)
                    {
                        fs.Seek(pIndice + 4 * notePosizione[numeroNota], SeekOrigin.Begin);
                        fs.Seek(pTesto + br.ReadUInt32(), SeekOrigin.Begin);
                        testo = br.ReadString();
                    }
                    return testo;
                }
                else
                {
                    return noteNuoveTesto[-notePosizione[numeroNota] - 1];
                }
            }
        }

        public async Task<string> GetNotaTestoAsync(string titolo)
        {
            if (string.IsNullOrEmpty(titolo))
            {
                return "";
            }

            // prima cerchiamo la nota con esattamente lo stesso titolo, poi con lettere minuscole
            int numeroNota = GetNumeroNotaTitolo(titolo);
            if (numeroNota < 0 && !titolo.StartsWith('#') && Char.IsDigit(titolo[^1]))
            // possibilmente una nota ad un versetto, ma nel formato Mt 2:1
            {
                Riferimento noteInBrano = ElencaNoteInBrano(genitore.ConvertiRiferimento(titolo));
                if (noteInBrano.Count > 1) // diverse note nel brano, restituiamo il testo di tutte insieme
                {
                    return await TestoBranoAsync(noteInBrano, [], []);
                }

                if (noteInBrano.Count > 0)
                {
                    numeroNota = NoteTitoli.BinarySearch(noteInBrano.Note[0], confrontoParole);
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
                if (!titolo.StartsWith("#",StringComparison.Ordinal)|| !NoteTitoli[numeroNota].StartsWith("#",StringComparison.Ordinal) || titolo.Substring(0, 9) != NoteTitoli[numeroNota].Substring(0, 9))
                    return "";
                 */
            }
            else // numeroNota>=0
            {
                if (notePosizione[numeroNota] >= 0)
                {
                    string testo;
                    lock (fileLock)
                    {
                        fs.Seek(pIndice + 4 * notePosizione[numeroNota], SeekOrigin.Begin);
                        fs.Seek(pTesto + br.ReadUInt32(), SeekOrigin.Begin);
                        testo = br.ReadString();
                    }
                    return testo;
                }
                else
                {
                    return noteNuoveTesto[-notePosizione[numeroNota] - 1];
                }
            }
        }

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
                    {
                        radiciDiverse.RemoveAt(i);
                    }
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
                            RadiceDiversa radiceDiversa = new();
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
            noteInOrdine.AddRange(ordine);
            noteModificate = true;
        }

        public Riferimento ElencaNoteInBrano(Riferimento riferimento)
        {
            if (riferimento.Note.Count > 0)
                return riferimento; // già contiene l'elenco di note da restituire

            Riferimento noteInBrano = new(false);
            byte libroInizio, capitoloInizio, versettoInizio, libroFine, capitoloFine, versettoFine;
            char[] divisore = ['#'];
            foreach (string titolo in noteTitoli)
            {
                if (titolo.StartsWith('#'))
                {
                    string[] titoliNote = SplitString(titolo, divisore);
                    //string[] titoliNote = titolo.Split(divisore, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string titoloNota in titoliNote)
                    {
                        try
                        {
                            libroInizio = Convert.ToByte(titoloNota[..2], CultureInfo.InvariantCulture);
                            capitoloInizio = Convert.ToByte(titoloNota.Substring(2, 3), CultureInfo.InvariantCulture);
                            versettoInizio = Convert.ToByte(titoloNota.Substring(5, 3), CultureInfo.InvariantCulture);
                            libroFine = Convert.ToByte(titoloNota.Substring(13, 2), CultureInfo.InvariantCulture);
                            capitoloFine = Convert.ToByte(titoloNota.Substring(15, 3), CultureInfo.InvariantCulture);
                            if (capitoloFine == 0) // tutto il libro, quindi dobbiamo garantire che il capitolo cercato sia sempre trovato
                            {
                                capitoloFine = byte.MaxValue;
                            }

                            versettoFine = Convert.ToByte(titoloNota.Substring(18, 3), CultureInfo.InvariantCulture);
                            if (versettoFine == 0) // tutto il capitolo, quindi dobbiamo garantire che il capitolo cercato sia sempre trovato
                            {
                                versettoFine = byte.MaxValue;
                            }

                            foreach (byte[] brano in riferimento.Brani)
                            {
                                if ((brano[0] < libroFine
                                || (brano[0] == libroFine && brano[1] < capitoloFine)
                                || (brano[0] == libroFine && brano[1] == capitoloFine && brano[2] <= versettoFine))
                                &&
                                (brano[3] > libroInizio
                                || (brano[3] == libroInizio && brano[4] > capitoloInizio)
                                || (brano[3] == libroInizio && brano[4] == capitoloInizio && brano[5] >= versettoInizio)))
                                {
                                    noteInBrano.Note.Add(titolo);
                                    noteInBrano.numeroParola.Add([]);
                                    break;
                                }
                            }
                        }
                        catch { } // se titolo non è nel formato giusto, titolo.Substring può dare errore
                    }
                }
            }
            return noteInBrano;
        }

        public Boolean EsistonoCitazioni()
        {
            CreaListaCitazioni();
            return (citazioniRiferimenti.Count > 0);
        }

        public Collection<string> GetRiferimentiCitati()
        {
            CreaListaCitazioni();
            Collection<string> riferimentiCitati = [];
            int numeroCitazioniInCollezione = citazioniRiferimenti.Count;
            for (int i = 0; i < numeroCitazioniInCollezione; ++i)
            {
                riferimentiCitati.Add(new StringBuilder().Append(citazioniRiferimenti[i].Brano[0]).Append('|').Append(citazioniRiferimenti[i].Brano[1]).Append('|').Append(citazioniRiferimenti[i].Brano[2]).Append('|').Append(citazioniRiferimenti[i].Brano[3]).Append('|').Append(citazioniRiferimenti[i].Brano[4]).Append('|').Append(citazioniRiferimenti[i].Brano[5]).Append('|').Append(citazioniRiferimenti[i].NumeroNota).Append('|').ToString());
            }

            return riferimentiCitati;
        }

        public Riferimento Citazioni(Riferimento riferimento)
        {
            List<int> note = [];
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
                        {
                            note.Insert(~posizione, (int)(citazioniRiferimenti[j].NumeroNota));
                        }
                    }
                }
            }
            Riferimento citazioni = new(false);
            foreach (int numeroNota in note)
            {
                citazioni.Note.Add(noteTitoli[numeroNota]);
                citazioni.numeroParola.Add([]);
            }
            citazioni.OrdinaNote();
            return citazioni;
        }

        // -1 se tutto brano1 è prima di brano2
        // 0 se si sovrappongono
        // 1 se tutto brano1 è dopo brano2
        // brano1/2 sono di 6 byte
        private static int ConfrontaBrani(byte[] brano1, byte[] brano2)
        {
            if (ConfrontaVersetti(brano1[3], brano1[4], brano1[5], brano2[0], brano2[1], brano2[2]) < 0)
            {
                return -1;
            }

            if (ConfrontaVersetti(brano1[0], brano1[1], brano1[2], brano2[3], brano2[4], brano2[5]) > 0)
            {
                return 1;
            }

            return 0;
        }

        // -1 se tutto brano1 è prima di brano2
        // 0 se si sovrappongono
        // 1 se tutto brano1 è dopo brano2
        private static int ConfrontaVersetti(byte libro1, byte capitolo1, byte versetto1, byte libro2, byte capitolo2, byte versetto2)
        {
            int confronto = 0;
            if (libro1 < libro2)
            {
                confronto = -1;
            }

            if (libro1 > libro2)
            {
                confronto = 1;
            }

            if (confronto == 0)
            {
                if (capitolo1 < capitolo2)
                {
                    confronto = -1;
                }

                if (capitolo1 > capitolo2)
                {
                    confronto = 1;
                }
            }
            if (confronto == 0)
            {
                if (versetto1 < versetto2)
                {
                    confronto = -1;
                }

                if (versetto1 > versetto2)
                {
                    confronto = 1;
                }
            }
            return confronto;
        }

        #endregion

        #region TestoBrano

        internal Task<string> TestoBranoAsync(
            Riferimento riferimento,
            Collection<string> collezioniDaVisualizzare,
            List<Riferimento> noteDaVisualizzare,
            Riferimento? paroleRicercate = null) // Parametro opzionale
            => TestoBranoAsync(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, conNomiDelleNote: true, paroleRicercate ?? new Riferimento(), null, null);

        internal Task<string> TestoBranoAsync(
            Riferimento riferimento,
            Collection<string> collezioniDaVisualizzare,
            List<Riferimento> noteDaVisualizzare,
            bool conNomiDelleNote,
            BackgroundWorker? worker,
            DoWorkEventArgs? e)
            => TestoBranoAsync(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, conNomiDelleNote, new Riferimento(), worker, e);

        internal async Task<string> TestoBranoAsync(
            Riferimento riferimento,
            Collection<string> collezioniDaVisualizzare,
            List<Riferimento> noteDaVisualizzare,
            bool conNomiDelleNote,
            Riferimento paroleRicercate,
            BackgroundWorker? worker,
            DoWorkEventArgs? e)
        {
            string testoComeStringa;
            int numeroCommentari = collezioniDaVisualizzare.Count;

            string formatoGreco = @"\f3\fs" + Convert.ToString(Convert.ToInt32(genitore.Formato.FontGrecoDimensione * 2), CultureInfo.InvariantCulture) + @"\cf3";
            if (genitore.Formato.FontGrecoGrassetto)
            {
                formatoGreco += @"\b";
            }

            if (genitore.Formato.FontGrecoCorsivo)
            {
                formatoGreco += @"\i";
            }

            if (genitore.Formato.FontGrecoSottolineato)
            {
                formatoGreco += @"\ul";
            }

            formatoGreco += " ";

            string formatoEbraico = @"\f4\fs" + Convert.ToString(Convert.ToInt32(genitore.Formato.FontEbraicoDimensione * 2), CultureInfo.InvariantCulture) + @"\cf4";
            if (genitore.Formato.FontEbraicoGrassetto)
            {
                formatoEbraico += @"\b";
            }

            if (genitore.Formato.FontEbraicoCorsivo)
            {
                formatoEbraico += @"\i";
            }

            if (genitore.Formato.FontEbraicoSottolineato)
            {
                formatoEbraico += @"\ul";
            }

            formatoEbraico += " ";

            string formatoRiferimento = GetFormatoRiferimento();
            string formatoRicerca, formatoRicercaNote;
            (formatoRicerca, formatoRicercaNote) = GetFormatoRicerca();

            int ultimaParolaRicercata = -1;

            int numeroParoleRicercate = paroleRicercate.Count;
            if (riferimento.Versetti)
            {
                #region brano biblico
                if (info.Tipo == TestoTipi.Bibbia)
                {
                    if (genitore.Formato.RiferimentoApice) // in apice solo quando riferimento, non quando titolo di una nota
                    {
                        formatoRiferimento += @"\super";
                    }

                    string formatoRiferimentoContestoInizio = "", formatoRiferimentoContestoFine = "";
                    if (genitore.Formato.RiferimentoContestoRicerche)
                    {
                        formatoRiferimentoContestoInizio = @"\v" + RichTextBoxEx.InizioLink + @"\v0 *\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkBrano + info.Nome + @"\\#";
                        formatoRiferimentoContestoFine = "0000" + RichTextBoxEx.FineLink2 + @"\v0";
                    }

                    bool ebraico = (LinguaPrincipale(info.Lingua).StartsWith("he"));
                    bool greco = (LinguaPrincipale(info.Lingua) == "el");
                    bool rtl = RightToLeft(info.Lingua);

                    RiferimentoPosto riferimentoPosto = genitore.Formato.RiferimentoPosto;
                    TestoVisualizzato testoVisualizzato = genitore.Formato.TestoVisualizzato;
                    /*
                    if (rtl && genitore.Formato.RiferimentoFormato != RiferimentoFormato.Nessuno)
                    {
                        riferimentoPosto = RiferimentoPosto.PrimaRigaDiversa;
                        testoVisualizzato = TestoVisualizzato.Versetti;
                    }
                    */

                    StringBuilder testoDaVisualizzare = new(1024);
                    UInt16 cap0, cap1, vers0, vers1;
                    StringBuilder riferimentoVersetto = new(128);
                    string libroPunt, capitoloPunt, libroCapitoloPunt;
                    string riferimentoLibro = "";
                    string punteggiaturaFraLibroECapitolo = genitore.SeparatoriNeiRiferimenti()[0];
                    string punteggiaturaFraCapitoloEVersetto = genitore.SeparatoriNeiRiferimenti()[1];
                    string libroStringa, capitoloStringa, versettoStringa;
                    //string versettoStringaInTestoNascosto;
                    string versettoStringa1;
                    int p, p1;

                    byte[] riferimentoDaMostrare;
                    int nRiferimenti = riferimento.Count;
                    //                        Trace.WriteLine(DateTime.Now);
                    for (int i = 0; i < nRiferimenti; ++i)
                    {
                        if (i > 0)
                        { // riga vuota fra i brani
                            if (testoDaVisualizzare.ToString().EndsWith(@"\par ", StringComparison.Ordinal))
                            {
                                testoDaVisualizzare.Append(@"\par ");
                            }
                            else
                            {
                                if (testoDaVisualizzare.Length > 0)
                                {
                                    testoDaVisualizzare.Append(@"\par\par ");
                                }
                            }
                        }
                        riferimentoDaMostrare = riferimento.Brani[i];
                        lock (fileLock)
                        {
                            fs.Seek(pIndice + 4 * (indiceCapitolo[indiceLibro[riferimentoDaMostrare[0] - 1] + riferimentoDaMostrare[1] - 1] + riferimentoDaMostrare[2] - 1), SeekOrigin.Begin);
                            fs.Seek(pTesto + br.ReadInt32(), SeekOrigin.Begin);
                            string fineRiferimento, formatoRifPerVersetto = "", testoVersetto = "", testoVersettoTitolo, testoVersettoTestoBiblico;
                            bool soloUnVersetto = (riferimentoDaMostrare[0] == riferimentoDaMostrare[3] && riferimentoDaMostrare[1] == riferimentoDaMostrare[4] && riferimentoDaMostrare[2] == riferimentoDaMostrare[5]);

                            for (byte lib = riferimentoDaMostrare[0]; lib <= riferimentoDaMostrare[3]; ++lib)
                            {
                                if (lib == riferimentoDaMostrare[0])
                                {
                                    cap0 = riferimentoDaMostrare[1];
                                }
                                else
                                {
                                    cap0 = 1;
                                }

                                if (lib == riferimentoDaMostrare[3])
                                {
                                    cap1 = riferimentoDaMostrare[4];
                                }
                                else
                                {
                                    cap1 = capitoliInLibro[lib];
                                }

                                if (cap1 > capitoliInLibro[lib])
                                {
                                    cap1 = capitoliInLibro[lib];
                                }

                                switch (genitore.Formato.RiferimentoFormato)
                                {
                                    case RiferimentoFormato.Intero:
                                        riferimentoLibro = genitore.libriNomi[lib];
                                        break;
                                    case RiferimentoFormato.Abbreviazione:
                                        riferimentoLibro = genitore.libriAbbreviazioniUsate[lib];
                                        break;
                                    case RiferimentoFormato.Nessuno:
                                        break;
                                    case RiferimentoFormato.NessunoLibro:
                                        break;
                                    case RiferimentoFormato.AbbreviazioneRiconosciuta:
                                        riferimentoLibro = genitore.LibriAbbreviazioniRiconosciute.Abbreviazione(lib);
                                        break;
                                }

                                libroStringa = (lib <= 9 ? "0" + lib.ToString(CultureInfo.InvariantCulture) : lib.ToString(CultureInfo.InvariantCulture));
                                libroPunt = riferimentoLibro + punteggiaturaFraLibroECapitolo;

                                for (UInt16 cap = cap0; cap <= cap1; ++cap)
                                {
                                    if (lib > riferimentoDaMostrare[0] && cap == cap0)
                                    { // messo qui invece di prima del loop per evitare righe addizionali quando ci sono libri mancanti per es. l'Apocrifa
                                        if (testoVersetto.EndsWith(@"\par ", StringComparison.Ordinal))
                                        {
                                            testoDaVisualizzare.Append(@"\par ");
                                        }
                                        else
                                        {
                                            testoDaVisualizzare.Append(@"\par\par "); // riga vuota fra i libri
                                        }
                                    }
                                    if (lib == riferimentoDaMostrare[0] && cap == riferimentoDaMostrare[1])
                                    {
                                        vers0 = riferimentoDaMostrare[2];
                                    }
                                    else
                                    {
                                        vers0 = 1;
                                    }

                                    if (lib == riferimentoDaMostrare[3] && cap == riferimentoDaMostrare[4])
                                    {
                                        vers1 = riferimentoDaMostrare[5];
                                    }
                                    else
                                    {
                                        vers1 = versettiInCapitolo[indiceLibro[lib - 1] + cap];
                                    }

                                    if (vers1 > versettiInCapitolo[indiceLibro[lib - 1] + cap])
                                    {
                                        vers1 = versettiInCapitolo[indiceLibro[lib - 1] + cap];
                                    }

                                    capitoloStringa = "00" + cap.ToString(CultureInfo.InvariantCulture);
                                    capitoloStringa = libroStringa + capitoloStringa[^3..];
                                    if (cap > cap0)
                                    {
                                        if (testoVersetto.EndsWith(@"\par ", StringComparison.Ordinal))
                                        {
                                            testoDaVisualizzare.Append(@"\par ");
                                        }
                                        else
                                        {
                                            testoDaVisualizzare.Append(@"\par\par "); // riga vuota fra capitoli
                                        }
                                    }

                                    capitoloPunt = cap.ToString(CultureInfo.CurrentCulture) + punteggiaturaFraCapitoloEVersetto;
                                    libroCapitoloPunt = libroPunt;
                                    if (capitoliInLibro[lib] > 1)
                                    {
                                        libroCapitoloPunt += capitoloPunt;
                                    }

                                    for (UInt16 vers = vers0; vers <= vers1; ++vers)
                                    {
                                        riferimentoVersetto.Length = 0;
                                        switch (genitore.Formato.RiferimentoFormato)
                                        {
                                            case RiferimentoFormato.Intero:
                                                riferimentoVersetto.Append(libroCapitoloPunt).Append(vers);
                                                break;
                                            case RiferimentoFormato.Abbreviazione:
                                                if (vers == vers0)
                                                {
                                                    //  if (cap == cap0)
                                                    riferimentoVersetto.Append(libroCapitoloPunt).Append(vers);
                                                    //  else
                                                    //    riferimento = cap.ToString(CultureInfo.CurrentCulture) + punt2 + vers.ToString(CultureInfo.CurrentCulture);
                                                    // prima della versione 7, il riferimento aveva il libro solo all'inizio e con un nuovo libro
                                                    // qui c'è il libro all'inizio di ogni capitolo, altrimenti sposta il testo in Sfoglia non funziona,
                                                    // perché quando cerca il testo Gen 47:1 trova 47:1 per esempio
                                                }
                                                else
                                                {
                                                    riferimentoVersetto.Append(vers);
                                                }
                                                break;
                                            case RiferimentoFormato.Nessuno:
                                                break;
                                            case RiferimentoFormato.NessunoLibro:
                                                if (capitoliInLibro[lib] > 1)
                                                {
                                                    riferimentoVersetto.Append(capitoloPunt);
                                                }
                                                riferimentoVersetto.Append(vers);
                                                break;
                                            case RiferimentoFormato.AbbreviazioneRiconosciuta:
                                                riferimentoVersetto.Append(libroCapitoloPunt).Append(vers);
                                                break;
                                        }
                                        if (genitore.Formato.RiferimentoTipo == RiferimentoTipo.Citazione)
                                        {
                                            riferimentoVersetto.Append(':');
                                        }

                                        fineRiferimento = "}";

                                        formatoRifPerVersetto = formatoRiferimento;
                                        if (riferimentoVersetto.Length > 0)
                                        {
                                            formatoRifPerVersetto += " ";
                                            fineRiferimento += "\\~";
                                        }

                                        versettoStringa = $"{capitoloStringa}{vers:000}";
                                        string versettoStringaInTestoNascosto = MainWindow.LPN_ANCORA + versettoStringa;
                                        riferimentoVersetto.Insert(0, formatoRifPerVersetto).Insert(0, @"{");
                                        if (soloUnVersetto && genitore.Formato.RiferimentoContestoRicerche && genitore.Formato.RiferimentoFormato != RiferimentoFormato.Nessuno)
                                        {
                                            versettoStringa1 = "00" + (vers > 1 ? vers - 1 : vers).ToString(CultureInfo.InvariantCulture);
                                            riferimentoVersetto.Append(formatoRiferimentoContestoInizio).Append(capitoloStringa).Append(versettoStringa1[^3..]).Append("0000+");
                                            // + in un riferimento invece di - indica che il riferimento è sempre visualizzato nella finestra Visualizza (in Principale::LinkCliccato)
                                            versettoStringa1 = "00" + (vers + 1).ToString(CultureInfo.InvariantCulture);
                                            riferimentoVersetto.Append(capitoloStringa).Append(versettoStringa1[^3..]).Append(formatoRiferimentoContestoFine);
                                        }
                                        riferimentoVersetto.Append(fineRiferimento);

                                        if (testoDaVisualizzare.Length > 0 &&
                                            !EndsWith(testoDaVisualizzare, @"\par") &&
                                            !EndsWith(testoDaVisualizzare, @"\par}") &&
                                            !EndsWith(testoDaVisualizzare, @"\par }") &&
                                            testoDaVisualizzare[^1] != ' ')
                                        {
                                            testoDaVisualizzare.Append(' ');
                                        }

                                        testoDaVisualizzare.Append(versettoStringaInTestoNascosto);

                                        switch (testoVisualizzato)
                                        {
                                            case TestoVisualizzato.Versetti:
                                                testoVersetto = br.ReadString();
                                                if (!testoVersetto.TrimEnd().EndsWith(@"\par", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    testoVersetto += @"\par ";
                                                }

                                                break;
                                            case TestoVisualizzato.Paragrafi:
                                                testoVersetto = br.ReadString();
                                                break;
                                            case TestoVisualizzato.Nessuno:
                                                testoVersetto = "";
                                                break;
                                        }
                                        if (lib == riferimentoDaMostrare[0] && cap == cap0 && vers == vers0)
                                        {
                                            testoVersetto = ModificaFormatoParole(testoVersetto, riferimento.numeroParola[i], "{" + formatoRicerca + " ", "}", info.Lingua);
                                        }

                                        for (int numeroParolaRicercata = ultimaParolaRicercata + 1; numeroParolaRicercata < numeroParoleRicercate; ++numeroParolaRicercata)
                                        {
                                            if (lib > paroleRicercate.Brani[numeroParolaRicercata][0])
                                            {
                                                ultimaParolaRicercata = numeroParolaRicercata;
                                            }
                                            else if (lib < paroleRicercate.Brani[numeroParolaRicercata][0])
                                            {
                                                break;
                                            }
                                            else if (cap == paroleRicercate.Brani[numeroParolaRicercata][1] && vers == paroleRicercate.Brani[numeroParolaRicercata][2])
                                            {
                                                testoVersetto = ModificaFormatoParole(testoVersetto, paroleRicercate.numeroParola[numeroParolaRicercata], "{" + formatoRicerca + " ", "}", info.Lingua);
                                            }
                                        }

                                        testoVersettoTitolo = "";
                                        testoVersettoTestoBiblico = testoVersetto;

                                        if (testoVersetto.StartsWith(@"\lptit1 ", StringComparison.Ordinal))
                                        {
                                            p = testoVersetto.IndexOf(@"\lptit0 ", StringComparison.Ordinal);
                                            if (p > -1)
                                            {
                                                testoVersettoTitolo = genitore.Formato.TitoliVisualizzati ? testoVersetto[..(p + 8)] : "";
                                                testoVersettoTestoBiblico = testoVersetto[(p + 8)..];
                                            }
                                        }

                                        if (!genitore.Formato.TitoliVisualizzati)
                                        {
                                            while ((p1 = testoVersettoTestoBiblico.IndexOf(@"\lptit1 ", StringComparison.Ordinal)) >= 0)
                                            { // quando ci sono due titoli in un versetto, come Sal 24 nella CEI
                                                p = testoVersettoTestoBiblico.IndexOf(@"\lptit0 ", StringComparison.Ordinal);
                                                if (p > -1)
                                                {
                                                    testoVersettoTestoBiblico = testoVersettoTestoBiblico[..p1] + testoVersettoTestoBiblico[(p + 8)..];
                                                }
                                                else
                                                {
                                                    testoVersettoTestoBiblico = testoVersettoTestoBiblico[..p1] + testoVersettoTestoBiblico[(p1 + 8)..]; // in questo caso, c'è un errore nel testo
                                                }
                                            }
                                        }

                                        // inserire le note nel posto giusto nel testo
                                        string notaStringa;
                                        for (int iCommentario = 0; iCommentario < numeroCommentari; ++iCommentario)
                                        {
                                            int numeroNote = noteDaVisualizzare[iCommentario].Count;
                                            for (int iNota = numeroNote - 1; iNota >= 0; --iNota)
                                            { // al contrario, per quando 2 note in un versetto meglio disturbare prima il testo posteriore
                                                notaStringa = noteDaVisualizzare[iCommentario].Note[iNota];
                                                if (notaStringa.AsSpan(1, 8).SequenceEqual(versettoStringa)
                                                    || (notaStringa.AsSpan(6, 3).SequenceEqual("000") && string.Concat(notaStringa.AsSpan(1, 5), "001") == versettoStringa) // nota per tutto il capitolo mostrato all'inizio del primo versetto
                                                    || (notaStringa.AsSpan(3, 6).SequenceEqual("000000") && string.Concat(notaStringa.AsSpan(1, 2), "001001") == versettoStringa)) // nota per tutto il libro mostrato all'inizio del primo versetto
                                                {
                                                    UInt16 numeroDellaParola = 0;
                                                    try
                                                    {
                                                        numeroDellaParola = Convert.ToUInt16(noteDaVisualizzare[iCommentario].Note[iNota].Substring(9, 4), CultureInfo.InvariantCulture);
                                                    }
                                                    catch (FormatException) { } // rimane 0 cioè all'inizio del versetto
                                                    catch (OverflowException) { }
                                                    testoVersettoTestoBiblico = ModificaFormatoParole(testoVersettoTestoBiblico, numeroDellaParola, "", @"{\v " + RichTextBoxEx.InizioLink + @"}*{\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkNota + collezioniDaVisualizzare[iCommentario] + @"\\" + noteDaVisualizzare[iCommentario].Note[iNota] + RichTextBoxEx.FineLink2 + "}" + (iCommentario == 0 ? "" : " "), info.Lingua);
                                                }
                                            }
                                        }

                                        if (greco)
                                        {
                                            testoVersettoTestoBiblico = @"{" + formatoGreco + testoVersettoTestoBiblico + "}";
                                            if (testoVersettoTestoBiblico.EndsWith(@"\par }", StringComparison.OrdinalIgnoreCase))
                                            {
                                                testoVersettoTestoBiblico = testoVersettoTestoBiblico[..^6] + @"}\par ";
                                            }
                                        }
                                        if (ebraico)
                                        {
                                            testoVersettoTestoBiblico = @"{" + formatoEbraico + testoVersettoTestoBiblico + "}";
                                            if (testoVersettoTestoBiblico.EndsWith(@"\par }", StringComparison.OrdinalIgnoreCase))
                                            {
                                                testoVersettoTestoBiblico = testoVersettoTestoBiblico[..^6] + @"}\par ";
                                            }
                                        }

                                        switch (riferimentoPosto)
                                        {
                                            case RiferimentoPosto.PrimaStessaRiga:
                                                testoDaVisualizzare.Append(testoVersettoTitolo).Append(riferimentoVersetto).Append(testoVersettoTestoBiblico);
                                                break;
                                            case RiferimentoPosto.PrimaRigaDiversa:
                                                testoDaVisualizzare.Append(testoVersettoTitolo).Append(riferimentoVersetto).Append(@"\par ").Append(testoVersettoTestoBiblico);
                                                break;
                                            case RiferimentoPosto.Dopo:
                                                if (testoVersettoTestoBiblico.EndsWith(@"\par", StringComparison.Ordinal))
                                                {
                                                    testoVersettoTestoBiblico = testoVersettoTestoBiblico[..^4];
                                                    riferimentoVersetto.Append(@"\par");
                                                }
                                                if (testoVersettoTestoBiblico.EndsWith(@"\par ", StringComparison.Ordinal))
                                                {
                                                    testoVersettoTestoBiblico = testoVersettoTestoBiblico[..^5];
                                                    riferimentoVersetto.Append(@"\par ");
                                                    if (testoVersettoTestoBiblico.EndsWith(@"\par ", StringComparison.Ordinal)) // nuovo paragrafo, ma il testo è visualizzato a versetti
                                                    {
                                                        testoVersettoTestoBiblico = testoVersettoTestoBiblico[..^5];
                                                        riferimentoVersetto.Append(@"\par ");
                                                    }
                                                }
                                                testoDaVisualizzare.Append(testoVersettoTitolo).Append(testoVersettoTestoBiblico).Append(" - ").Append(riferimentoVersetto);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        worker?.ReportProgress(-1, e);
                    }
                    testoComeStringa = testoDaVisualizzare.ToString();
                    if (rtl)
                    {
                        testoComeStringa = @"\qr " + testoComeStringa;
                    }

                    testoComeStringa = genitore.RtfIntestazione() + testoComeStringa + "}";
                } // if (testoFileArray.Tipo==TestoTipo.Bibbia)
                else // tutte le note in un certo brano
                {
                    testoComeStringa = await TestoBranoAsync(ElencaNoteInBrano(riferimento), collezioniDaVisualizzare, noteDaVisualizzare, conNomiDelleNote, paroleRicercate, null, null);
                }
                testoComeStringa = ConvertiLink(testoComeStringa);
                #endregion
            }
            else
            {
                #region nota
                List<string> stringheRtf = StringheBranoCommentario(riferimento, conNomiDelleNote, paroleRicercate, formatoRicercaNote, formatoRiferimento, formatoRicerca);
                // collezioniDaVisualizzare e noteDaVisualizzare non sono usati in questo caso
                testoComeStringa = await genitore.MergeManyRtfAsStringAsync(ConvertiLink(stringheRtf));
                if (string.IsNullOrEmpty(testoComeStringa))
                {
                    testoComeStringa = "";
                }

                if (testoComeStringa.EndsWith("\r\n", StringComparison.Ordinal))
                {
                    testoComeStringa = testoComeStringa[..^2];
                }

                if (testoComeStringa.EndsWith("\r\n}", StringComparison.Ordinal))
                {
                    testoComeStringa = testoComeStringa.Remove(testoComeStringa.Length - 3, 2);
                }

                if (testoComeStringa.EndsWith(@"\par}", StringComparison.Ordinal))
                {
                    testoComeStringa = testoComeStringa.Remove(testoComeStringa.Length - 5, 4);
                }

                if (testoComeStringa.EndsWith(@"\f0}", StringComparison.Ordinal))
                {
                    testoComeStringa = testoComeStringa.Remove(testoComeStringa.Length - 4, 3);
                }
                #endregion
            }
            return testoComeStringa;
        }

        private string GetFormatoRiferimento()
        {
            string formatoRiferimento = @"\f1\fs" + Convert.ToString(Convert.ToInt32(genitore.Formato.FontRiferimentoDimensione * 2), CultureInfo.InvariantCulture) + @"\cf1";
            if (genitore.Formato.FontRiferimentoGrassetto)
            {
                formatoRiferimento += @"\b";
            }

            if (genitore.Formato.FontRiferimentoCorsivo)
            {
                formatoRiferimento += @"\i";
            }

            if (genitore.Formato.FontRiferimentoSottolineato)
            {
                formatoRiferimento += @"\ul";
            }
            // per le note, quando il riferimento è il titolo, non è mai messo in apice

            return formatoRiferimento;
        }

        private (string, string) GetFormatoRicerca()
        {
            // FontRicerca (\f2) e FontRicercaDimensione non è usato, per non disturbare troppo il font del testo
            //                string formatoRicerca = @"\f2\fs" + Convert.ToString(Convert.ToInt32(genitore.Formato.FontRicercaDimensione * 2), CultureInfo.InvariantCulture) + @"\cf2";
            // TODO2 da cancellare?
            //string formatoRicercaNote = (isRunningOnMono ? "" : @"\v " + RichTextBoxEx.ParolaRicercata + @"\v0"); // per non disturbare il formato delle note, cambiare solo lo stile delle parole ricercate, non il colore (il font e la dimensione non sono cambiati comunque, neanche per Bibbie)
            string formatoRicercaNote = "";
            string formatoRicerca = formatoRicercaNote + @"\cf2";
            // comunque, modificare il font e il colore non funzionano, perché \f? e \cf? non necessariamente corrispondono al font e al colore giusti
            if (genitore.Formato.FontRicercaGrassetto)
            {
                formatoRicerca += @"\b";
                formatoRicercaNote += @"\b";
            }
            if (genitore.Formato.FontRicercaCorsivo)
            {
                formatoRicerca += @"\i";
                formatoRicercaNote += @"\i";
            }
            if (genitore.Formato.FontRicercaSottolineato)
            {
                formatoRicerca += @"\ul";
                formatoRicercaNote += @"\ul";
            }
            return (formatoRicerca, formatoRicercaNote);
        }

        public async Task<FlowDocument> FlowDocumentBranoCommentarioAsync(Riferimento riferimento, Riferimento paroleRicercate)
        {
            string formatoRiferimento = GetFormatoRiferimento();
            string formatoRicerca, formatoRicercaNote;
            (formatoRicerca, formatoRicercaNote) = GetFormatoRicerca();

            List<string> stringheRtf = StringheBranoCommentario(ElencaNoteInBrano(riferimento), true, paroleRicercate, formatoRicercaNote, formatoRiferimento, formatoRicerca);

            // Direct compilation without string conversion overhead
            return await Texts.MergeManyRtfAsDocumentAsync(ConvertiLink(stringheRtf));
        }

        private List<string> StringheBranoCommentario(Riferimento riferimento, bool conNomiDelleNote, Riferimento paroleRicercate, string formatoRicercaNote, string formatoRiferimento, string formatoRicerca)
        {
            int numeroParoleRicercate = paroleRicercate.Count;
            int ultimaParolaRicercata = -1;
            string titoloNota, titoloNotaDaLeggere;
            List<string> stringheRtf = [];

            string inizioFormatoRicercaNote = '{' + formatoRicercaNote + " ";
            string inizioFormatoRiferimento = '{' + formatoRiferimento + " ";
            string inizioInizioRiferimento = MainWindow.LPN_ANCORA;
            bool notaSuBrano;
            int numeroNote = riferimento.Note.Count;
            // TODO2 int quantoSpessoAggiornaBarra = numeroNote / 100 + 1;
            // TODO2 int quantoSpessoAggiornaBarraMenoUno = quantoSpessoAggiornaBarra - 1;
            for (int i = 0; i < numeroNote; ++i)
            {
                if (i > 0)
                { // riga vuota fra i brani
                    stringheRtf.Add(genitore.RtfIntestazione() + @"\par}");
                }
                titoloNota = riferimento.Note[i];
                notaSuBrano = titoloNota.StartsWith('#');
                titoloNotaDaLeggere = (notaSuBrano ? genitore.ConvertiTitoloNotaARiferimento(titoloNota) : titoloNota);
                if (conNomiDelleNote)
                {
                    stringheRtf.Add(new StringBuilder(genitore.RtfIntestazione()).Append(notaSuBrano ? string.Concat(inizioInizioRiferimento, titoloNota.AsSpan(1, 8)) : "").Append(inizioFormatoRiferimento).Append(ConvertiUnicodeInRtf(titoloNotaDaLeggere)).Append(@"}\par}").ToString());
                    //stringheRtf.Add(new StringBuilder(genitore.RtfIntestazione()).Append(inizioFormatoRiferimento).Append(ConvertiUnicodeInRtf(titoloNotaDaLeggere)).Append(@"}\par}").ToString());
                }

                string testoModificato = ModificaFormatoParole(GetNotaTestoTitolo(titoloNota), riferimento.numeroParola[i], inizioFormatoRicercaNote, "}", info.Lingua);
                for (int numeroParolaRicercata = ultimaParolaRicercata + 1; numeroParolaRicercata < numeroParoleRicercate; ++numeroParolaRicercata)
                {
                    switch (string.CompareOrdinal(riferimento.Note[i], paroleRicercate.Note[numeroParolaRicercata]))
                    {
                        case 1:
                            ultimaParolaRicercata = numeroParolaRicercata;
                            break;
                        case -1:
                            numeroParolaRicercata = numeroParoleRicercate; // finire il loop, non ci sono più note uguali
                            break;
                        case 0:
                            testoModificato = ModificaFormatoParole(testoModificato, paroleRicercate.numeroParola[numeroParolaRicercata], "{" + formatoRicerca + " ", "}", info.Lingua);
                            break;
                    }
                }
                if (!testoModificato.StartsWith(@"{\rtf", StringComparison.Ordinal) && !testoModificato.EndsWith('}'))
                {
                    testoModificato = genitore.RtfIntestazione() + testoModificato.Replace("\r\n", @"\par ") + "}";
                }
                stringheRtf.Add(ConvertiUnicodeInRtf(testoModificato));
                // TODO2 progress bar
                //if (worker != null && (i % quantoSpessoAggiornaBarra == quantoSpessoAggiornaBarraMenoUno))
                //{
                //    worker.ReportProgress(-quantoSpessoAggiornaBarra, e);
                //}

            }
            return stringheRtf;
        }

        private string ConvertiLink(string rtfString)
        {
            if (string.IsNullOrEmpty(rtfString)) return "";

            bool aggiungicf0 = rtfString.Contains("colortbl ;");

            // Pattern matches: \v \'02\v0 [Anchor] \v \'03 [\'05|\'06|\'07] [Data] \'04\v0 [optional trailing delimiter space]
            // Note: In verbatim strings (@""), the .NET Regex engine natively interprets \uXXXX escape codes.
            //string linkPattern = @"\\v\s*(?:\u0002|\\'02)\\v0\s*(?<anchor>.*?)\\v\s*(?:\u0003|\\'03)(?<type>[\u0005\u0006\u0007]|\\'0[567])(?<data>.*?)(?:\u0004|\\'04)\\v0\s?";
            // string linkpattern = @"\\v\s*(?:\\f\d+\s*)*(?:\u0002|\\'02)\\v0\s*(?<anchor>.*?)\\v\s*(?:\u0003|\\'03)(?<type>[\u0005\u0006\u0007]|\\'0[567])(?<data>.*?)(?:\u0004|\\'04)(?:\\cf\d+\s*)*\\v0\s?";
            Regex linkRegex = RegExConvertiIperlink();

            // Translate the old custom markers into standard RTF fields on the fly
            string processedRtf = linkRegex.Replace(rtfString, m =>
            {
                string anchor = m.Groups["anchor"].Value;

                // Normalize RTF hex escapes (e.g., "\'05") back to standard string representation if needed
                string type = m.Groups["type"].Value;
                if (type == "\\'05") type = "\u0005";
                else if (type == "\\'06") type = "\u0006";
                else if (type == "\\'07") type = "\u0007";

                string data = m.Groups["data"].Value;

                // Convertiamo le doppie barre dell'RTF in una singola barra pulita C#
                data = data.Replace(@"\\", @"\");
                // Convertiamo gli escape esadecimali RTF (es. \'f9 -> ù) nei rispettivi caratteri.
                data = RegexRtf().Replace(data, match =>
                {
                    byte b = Convert.ToByte(match.Groups[1].Value, 16);
                    // Encoding.Latin1 gestisce perfettamente lettere accentate occidentali (à, è, é, ì, ò, ù)
                    return Encoding.Latin1.GetString([b]);
                });
                // Convert literal RTF '\u1234?' sequences into actual C# Unicode characters
                data = RegexConvertiUnicodeCaratteri().Replace(data, match =>
                {
                    int code = int.Parse(match.Groups[1].Value);
                    return ((char)code).ToString();
                });

                // Map the internal type byte to the URI schemes
                string scheme = type switch
                {
                    "\u0005" => "bibbia:",
                    "\u0006" => "nota:",
                    "\u0007" => "filenome:",
                    _ => ""
                };

                if (!data.Contains('\\'))
                {
                    if (scheme == "bibbia:" && Info.VersioneDelleNote.Length > 0)
                    {
                        data = Info.VersioneDelleNote + @"\" + data;
                    }
                    else if (scheme == "nota:" || scheme == "filenome:")
                    {
                        data = Info.Nome + @"\" + data;
                    }
                }

                // URL-encode the data payload to make it 100% safe for the WPF RTF Parser
                data = Uri.EscapeDataString(data);

                // Construct standard RTF hyperlink field code
                // aggiungere cf0 affinché il colore del link in RTF non cancelli il colore del setter in RichTextBoxEx
                if (aggiungicf0)
                    return $"{{\\field{{\\*\\fldinst HYPERLINK \"{scheme}{data}\"}}{{\\fldrslt {{\\cf0 {anchor}}}}}}}";
                else
                    return $"{{\\field{{\\*\\fldinst HYPERLINK \"{scheme}{data}\"}}{{\\fldrslt {anchor}}}}}";
            });

            return processedRtf;
        }

        private List<string> ConvertiLink(List<string> rtfStrings)
        {
            List<string> outStringhe = [];

            foreach (string rtfString in rtfStrings)
            {
                outStringhe.Add(ConvertiLink(rtfString));
            }
            return outStringhe;
        }

        public string TestoVersettoRaw(byte libro, byte capitolo, byte versetto)
        {
            string testoVersetto = "";
            if (info.Tipo == TestoTipi.Bibbia) // altrimenti solo una stringa vuota è restituita
            {
                lock (fileLock)
                {
                    fs.Seek(pIndice + 4 * (indiceCapitolo[indiceLibro[libro - 1] + capitolo - 1] + versetto - 1), SeekOrigin.Begin);
                    fs.Seek(pTesto + br.ReadInt32(), SeekOrigin.Begin);
                    testoVersetto = br.ReadString();
                }
            }
            return testoVersetto;
        }

        internal static string ModificaFormatoParole(string testoDaModificare, UInt16 numeroParolaDaModificare, string formatoPrimaDellaParola, string formatoDopoLaParola, string lingua)
        {
            List<UInt16> listaParole =
            [
                numeroParolaDaModificare
            ];
            return ModificaFormatoParole(testoDaModificare, listaParole, formatoPrimaDellaParola, formatoDopoLaParola, lingua);
        }

        private static string ModificaFormatoParole(string testoDaModificare, List<UInt16> numeriParoleDaModificare, string formatoPrimaDellaParola, string formatoDopoLaParola, string lingua)
        {
            if ((formatoPrimaDellaParola == "{" && formatoDopoLaParola == "}") || (string.IsNullOrEmpty(formatoPrimaDellaParola) && string.IsNullOrEmpty(formatoDopoLaParola)) || (numeriParoleDaModificare.Count == 0))
            {
                return testoDaModificare; // non ci sono modifiche da fare, quindi rimane uguale
            }

            string[] lingue = SplitString(lingua.ToLower(CultureInfo.InvariantCulture), '|');
            //string[] lingue = lingua.ToLower(CultureInfo.InvariantCulture).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string linguaDaUsare, linguaPrincipale = (lingue.Length >= 1 ? lingue[0] : "");
            bool dizionarioGreco = (linguaPrincipale == "el" && lingue.Length >= 2);
            bool dizionarioEbraico = (linguaPrincipale.StartsWith("he") && lingue.Length >= 2);

            if (RightToLeft(linguaPrincipale))
            { // in lingue RTL (come ebraico), l'inserimento di un carattere non RTL, anche nascosto, rovina la visualizzazione
              // per questo motivo un testo ricercato in ebraico non salterà alla prima apparizione di una parola ricercata, ma non è troppo grave
                while (formatoPrimaDellaParola.Contains(@"\v "))
                {
                    formatoPrimaDellaParola = formatoPrimaDellaParola[..formatoPrimaDellaParola.IndexOf(@"\v ", StringComparison.Ordinal)] + formatoPrimaDellaParola[(formatoPrimaDellaParola.IndexOf(@"\v0", StringComparison.Ordinal) + 3)..];
                }

                while (formatoDopoLaParola.Contains(@"{\v "))
                {
                    formatoDopoLaParola = formatoDopoLaParola[..formatoDopoLaParola.IndexOf(@"{\v ", StringComparison.Ordinal)] + formatoDopoLaParola[(formatoDopoLaParola.IndexOf('}', StringComparison.Ordinal) + 1)..];
                }

                while (formatoDopoLaParola.Contains(@"\v "))
                {
                    formatoDopoLaParola = formatoDopoLaParola[..formatoDopoLaParola.IndexOf(@"\v ", StringComparison.Ordinal)] + formatoDopoLaParola[(formatoDopoLaParola.IndexOf(@"\v0", StringComparison.Ordinal) + 3)..];
                }
            }

            int nParoleDaCambiare = numeriParoleDaModificare.Count;
            // a volte si chiede che la stessa parola sia modificata 2 volte; non è possibile quindi togliamo i doppioni
            for (int i = nParoleDaCambiare - 1; i >= 1; --i)
            {
                if (numeriParoleDaModificare[i] == numeriParoleDaModificare[i - 1])
                {
                    numeriParoleDaModificare.RemoveAt(i);
                    --nParoleDaCambiare;
                }
            }
            int iParolaDaCambiare = 0;
            int nProssimaParolaDaCambiare = numeriParoleDaModificare[0];
            int paroleTrovate = 0;
            StringBuilder parola = new("");
            int statoCambiamento = 0; // 0=niente da cambiare, 1=cambiare la prossima, 2=chiudere il cambiamento alla fine di questa parola
            if (nProssimaParolaDaCambiare == 1)
            {
                statoCambiamento = 1;
                ++iParolaDaCambiare;
                if (iParolaDaCambiare < nParoleDaCambiare)
                {
                    nProssimaParolaDaCambiare = numeriParoleDaModificare[iParolaDaCambiare];
                }
            }
            char c;
            bool analizzaParola;
            int carattereIniziale = 0, iCarattere1, iCarattere2, carattereDaInserire = 0;
            if (testoDaModificare.IndexOf(@"\viewkind", StringComparison.Ordinal) > 0) // saltare un'eventuale intestazione RTF
            {
                carattereIniziale = testoDaModificare.IndexOf(@"\viewkind", StringComparison.Ordinal) + 10; // +10 perché c'è un numero dopo viewkind
            }

            if (testoDaModificare.IndexOf(@"\deflang", carattereIniziale, StringComparison.Ordinal) > 0) // saltare un'eventuale intestazione RTF
            {
                carattereIniziale = testoDaModificare.IndexOf(@"\deflang", carattereIniziale, StringComparison.Ordinal) + 12; // +12 perché ci sono quattro cifre dopo deflang
            }

            if (testoDaModificare.StartsWith(@"{\rtf", StringComparison.Ordinal) && carattereIniziale == 0 && testoDaModificare.IndexOf(@"\pard", carattereIniziale, StringComparison.Ordinal) > 0)
            {
                carattereIniziale = testoDaModificare.IndexOf(@"\pard", carattereIniziale, StringComparison.Ordinal) + 6;
            }

            if (nProssimaParolaDaCambiare == 0)
            {
                testoDaModificare = testoDaModificare.Insert(carattereDaInserire, formatoPrimaDellaParola + formatoDopoLaParola);
                carattereIniziale += (formatoPrimaDellaParola + formatoDopoLaParola).Length;
                ++iParolaDaCambiare;
                if (iParolaDaCambiare < nParoleDaCambiare)
                {
                    nProssimaParolaDaCambiare = numeriParoleDaModificare[iParolaDaCambiare];
                }
            }

            for (int i = carattereIniziale; i < testoDaModificare.Length; ++i)
            {
                c = testoDaModificare[i];
                if (IsLetteraONumero(c) || (c == '\\' && i < testoDaModificare.Length - 3 && (testoDaModificare[i + 1] == '\'' || (testoDaModificare[i + 1] == 'u' && Char.IsDigit(testoDaModificare[i + 2])))))
                {
                    if (i <= testoDaModificare.Length - 1 && testoDaModificare[i] == RichTextBoxEx.InizioLink)
                    {
                        i += 0;
                    }
                    else
                    {
                        if (IsLetteraONumero(c))
                        {
                            parola.Append(c);
                        }
                        else if (testoDaModificare[i + 1] == '\'')
                        {
                            parola.Append(testoDaModificare.AsSpan(i, 4));
                        }
                        else if (testoDaModificare[i + 1] == 'u' && Char.IsDigit(testoDaModificare[i + 2]))
                        {
                            parola.Append(testoDaModificare.AsSpan(i, testoDaModificare.IndexOf('?', i) - i + 1));
                        }

                        if (statoCambiamento == 1)
                        {
                            testoDaModificare = testoDaModificare.Insert(i, formatoPrimaDellaParola);
                            i += formatoPrimaDellaParola.Length;
                            statoCambiamento = 2;
                        }
                        if (!IsLetteraONumero(c))
                        {
                            if (testoDaModificare[i + 1] == '\'')
                            {
                                i += 3;
                            }
                            else
                            {
                                if (testoDaModificare[i + 2] == '0' && testoDaModificare[i + 3] == '0')
                                    i += 5; // \u0005
                                else // unicode \u1234? oppure \u123?
                                    i = testoDaModificare.IndexOf('?', i);
                            }
                        }
                    }
                }
                else if (Char.IsPunctuation(c) || Char.IsWhiteSpace(c) || Char.IsSymbol(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.Format)
                {
                    analizzaParola = true;
                    carattereDaInserire = i;
                    if (c == '\'')
                    {
                        // in un dizionario greco-altra lingua, dobbiamo scegliere la lingua giusta
                        linguaDaUsare = linguaPrincipale;
                        if (dizionarioGreco && i > 0 && !IsLetteraGreca(testoDaModificare[i - 1]))
                        {
                            linguaDaUsare = lingue[1];
                        }
                        else if (dizionarioEbraico && i > 0 && !IsLetteraEbraica(testoDaModificare[i - 1]))
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
                                if ((i == 1 || !IsLetteraONumero(testoDaModificare[i - 1]))
                                    && ((i < testoDaModificare.Length - 1 && (testoDaModificare[i + 1] == 't' || testoDaModificare[i + 1] == 'T') && (i == testoDaModificare.Length - 2 || !IsLetteraONumero(testoDaModificare[i + 2])))
                                      || (i < testoDaModificare.Length - 3 && testoDaModificare.Substring(i + 1, 3).Equals("tis", StringComparison.CurrentCultureIgnoreCase) && (i == testoDaModificare.Length - 4 || !IsLetteraONumero(testoDaModificare[i + 4])))
                                      || (i < testoDaModificare.Length - 4 && testoDaModificare.Substring(i + 1, 4).Equals("twas", StringComparison.CurrentCultureIgnoreCase) && (i == testoDaModificare.Length - 5 || !IsLetteraONumero(testoDaModificare[i + 5])))))
                                {
                                    parola.Append(c);
                                    analizzaParola = false;
                                }
                                else if (i >= 2)
                                {
                                    if (i < testoDaModificare.Length - 1 &&
                                        (IsLetteraONumero(testoDaModificare[i - 1])
                                            && char.IsLetter(testoDaModificare[i + 1])
                                            && (i == testoDaModificare.Length - 2 || !IsLetteraONumero(testoDaModificare[i + 2]))))
                                    {
                                        parola.Append(c);
                                        analizzaParola = false;
                                    }
                                    else if (dizionarioEbraico && i < testoDaModificare.Length - 1 && (char.IsLetter(testoDaModificare[i - 1]) && testoDaModificare[i + 1] == '-'))
                                    { // per il dizionario Strong's Hebrew, che ha pronunce come eh'-sheth
                                        parola.Append(c);
                                        analizzaParola = false;
                                    }
                                    else if ((testoDaModificare[i - 1] == 's' || testoDaModificare[i - 1] == 'S')
                                        && (i == testoDaModificare.Length - 1 || !char.IsPunctuation(testoDaModificare[i + 1]))
                                        && Array.BinarySearch(Texts.paroleInglesiSenzaApostrofe, parola.ToString(), confrontoParole) < 0)
                                    {
                                        parola.Append(c);
                                        analizzaParola = false;
                                    }
                                    else if (i < testoDaModificare.Length - 2
                                         && IsLetteraONumero(testoDaModificare[i - 1]) && (i == testoDaModificare.Length - 3 || !IsLetteraONumero(testoDaModificare[i + 3]))
                                    && (testoDaModificare.Substring(i + 1, 2) == "en" || testoDaModificare.Substring(i + 1, 2) == "er" || testoDaModificare.Substring(i + 1, 2) == "ll" || testoDaModificare.Substring(i + 1, 2) == "lt" || testoDaModificare.Substring(i + 1, 2) == "ry" || testoDaModificare.Substring(i + 1, 2) == "st" || testoDaModificare.Substring(i + 1, 2) == "ve"))
                                    {
                                        parola.Append(c);
                                        analizzaParola = false;
                                    }
                                    else if (i < testoDaModificare.Length - 4
                                    && IsLetteraONumero(testoDaModificare[i - 1]) && (i == testoDaModificare.Length - 3 || !IsLetteraONumero(testoDaModificare[i + 5]))
                                    && (testoDaModificare.Substring(i + 1, 4) == "ring"))
                                    {
                                        parola.Append(c);
                                        analizzaParola = false;
                                    }
                                }
                                break;
                            case "it":
                                if (i > 0 && i < testoDaModificare.Length - 1)
                                {
                                    if ((IsLetteraONumero(testoDaModificare[i - 1]) && (IsLetteraONumero(testoDaModificare[i + 1]) || testoDaModificare[i + 1] == '\'' || testoDaModificare[i + 1] == '«' || testoDaModificare[i + 1] == ']' || (testoDaModificare[i + 1] == ')') && testoDaModificare.IndexOf("('") < i)) || (Array.BinarySearch(Texts.paroleItalianeConApostrofe, parola.ToString()) >= 0))
                                    {
                                        // per esempio l'uomo 
                                        parola.Append(c);
                                    }
                                }
                                break;
                            case "el":
                                if (i > 0)
                                {
                                    if (IsLetteraGreca(testoDaModificare[i - 1]))
                                    {
                                        parola.Append(c);
                                    }
                                    else if (i < testoDaModificare.Length - 1 && char.IsLetter(testoDaModificare[i - 1]) && char.IsLetter(testoDaModificare[i + 1]))
                                    {
                                        parola.Append(c);
                                        analizzaParola = false;
                                    }
                                }
                                break;
                            case "": // interlineare
                                parola.Append(c);
                                break;
                        }
                    }
                    else if (c == '[' || c == ']')
                    {
                        //                            if (linguaLC == "el")
                        //                            {
                        if (i > 0 && i < testoDaModificare.Length - 1)
                        {
                            if (IsLettera(testoDaModificare[i - 1]) && IsLettera(testoDaModificare[i + 1]))
                            {
                                // parentesi quadrate in mezzo ad una parola
                                analizzaParola = false;
                            }
                        }
                        //                            }
                    }
                    else if (c == '-')
                    {
                        if (i > 0 && i < testoDaModificare.Length - 1)
                        {
                            if (((IsLettera(testoDaModificare[i - 1]) || (testoDaModificare[i - 1] == '?' && i > 1 && char.IsDigit(testoDaModificare[i - 2]))) &&
                                (IsLettera(testoDaModificare[i + 1]) || (i < testoDaModificare.Length - 2 && testoDaModificare.Substring(i + 1, 2) == @"\u"))) // per esempio Eben-Ezer e \u963?-\u960? ma non 1-2
                                || (dizionarioEbraico && testoDaModificare[i - 1] == '\'' && char.IsLetter(testoDaModificare[i + 1]))) // per esempio eh'-sheth in Strong's Hebrew
                            {
                                parola.Append(c);
                                analizzaParola = false;
                            }
                        }
                    }
                    else if (c == '}')
                    {
                        if (i > 0 && i < testoDaModificare.Length - 1)
                        {
                            if (IsLettera(testoDaModificare[i - 1]) && IsLettera(testoDaModificare[i + 1]))
                            {
                                // per esempio una parola parzialmente in italico come {\\i1 del}la
                                analizzaParola = false;
                            }
                        }
                    }
                    else if (c == '\\' || c == '{') // saltare codice RTF
                    {
                        if (i < testoDaModificare.Length - 6 && testoDaModificare.Substring(i, 7) == @"\lptit1")
                        {
                            i = testoDaModificare.IndexOf(@"\lptit0 ", i, StringComparison.Ordinal) + 7; // saltare un titolo nel testo
                            if (i == 6)
                            {
                                i = testoDaModificare.Length - 1;
                            }
                        }
                        // Controllo per inizio testo nascosto (\v o \v1, escludendo \v0)
                        else if (c == '\\' && i + 1 < testoDaModificare.Length && testoDaModificare[i + 1] == 'v' &&
                                 !(i + 2 < testoDaModificare.Length && testoDaModificare[i + 2] == '0'))
                        {
                            // Cerca la fine del blocco di testo nascosto (\v0)
                            i = testoDaModificare.IndexOf(@"\v0", i, StringComparison.Ordinal) + 2;

                            // Se IndexOf restituisce -1 (non trovato), -1 + 2 fa 1.
                            if (i == 1)
                            {
                                i = testoDaModificare.Length - 1; // Salta fino alla fine del testo
                            }
                        }
                        else
                        {
                            if (i > 0 && c == '{' && IsLettera(testoDaModificare[i - 1]))
                            {
                                // per esempio una parola parzialmente in italico come tuffata{\\i1 la}
                                analizzaParola = false;
                            }
                            // trova la fine del codice RTF cioè prossimo \ o spazio
                            iCarattere1 = testoDaModificare.IndexOf('\\', i + 1) - 1;
                            if (iCarattere1 == i)
                            {
                                iCarattere1 = -1;
                            }

                            iCarattere2 = testoDaModificare.IndexOf(' ', i);
                            if (iCarattere1 >= 0 && iCarattere1 < iCarattere2)
                            { // \ prima di spazio
                                if (i > 0 && c == '\\' && IsLettera(testoDaModificare[i - 1]) && iCarattere1 < testoDaModificare.Length - 2 && testoDaModificare[iCarattere1 + 2] == '\'')
                                {
                                    // per esempio una parola come necessit\\f2\\'e0
                                    analizzaParola = false;
                                }
                                iCarattere2 = iCarattere1;
                            }
                            else
                            {
                                if (i > 0 && c == '\\' && IsLettera(testoDaModificare[i - 1]) && iCarattere2 >= 0 && iCarattere2 < testoDaModificare.Length - 1 && testoDaModificare[i..iCarattere2] != @"\par" && (IsLettera(testoDaModificare[iCarattere2 + 1])))
                                {
                                    // per esempio una parola come ess\f1 ere
                                    analizzaParola = false;
                                }
                            }
                            if (iCarattere2 == -1)
                            {
                                iCarattere2 = testoDaModificare.Length - 1;
                            }

                            i = iCarattere2;
                        }
                    }
                    if (parola.Length > 0 && analizzaParola)
                    {
                        if (statoCambiamento == 2)
                        {
                            testoDaModificare = testoDaModificare.Insert(carattereDaInserire, formatoDopoLaParola);
                            i += formatoDopoLaParola.Length;
                            statoCambiamento = 0;
                        }
                        ++paroleTrovate;
                        if (paroleTrovate == nProssimaParolaDaCambiare - 1)
                        {
                            statoCambiamento = 1;
                            ++iParolaDaCambiare;
                            if (iParolaDaCambiare < nParoleDaCambiare)
                            {
                                nProssimaParolaDaCambiare = numeriParoleDaModificare[iParolaDaCambiare];
                            }
                        }
                        parola.Remove(0, parola.Length);
                    }
                }
            } // for (int iCarattere = 0; iCarattere < testoVersetto.Length; ++iCarattere)
            if (statoCambiamento == 2)
            {
                testoDaModificare += "}";
            }

            return testoDaModificare;
        }

        internal bool EsisteBrano(Riferimento riferimento)
        {
            bool branoEsiste = false;
            byte[] branoDaControllare = [0, 0, 0, 0, 0, 0];

            if (riferimento.Versetti)
            {
                if (info.Tipo == TestoTipi.Bibbia)
                {
                    foreach (byte[] brano in riferimento.Brani)
                    {
                        for (int i = 0; i < 6; ++i)
                        {
                            branoDaControllare[i] = brano[i];
                        }
                        // altrimenti quando brano[] è cambiato, il valore originale nell'argomento viene modificato anche
                        if (indiceLibro[branoDaControllare[0] - 1] != indiceLibro[branoDaControllare[3]])
                        {
                            if (branoDaControllare[1] == 255)
                            {
                                branoDaControllare[1] = 1;
                            }

                            if (branoDaControllare[4] == 255)
                            {
                                branoDaControllare[4] = 1;
                            }

                            if (capitoliInLibro[branoDaControllare[0]] >= branoDaControllare[1] || capitoliInLibro[branoDaControllare[3]] >= branoDaControllare[4])
                            {
                                // c'è testo nella parte richiesta del primo o dell'ultimo libro
                                branoEsiste = true;
                                break;
                            }
                            if (branoDaControllare[3] > branoDaControllare[0] + 1 && indiceLibro[branoDaControllare[0]] != indiceLibro[branoDaControllare[3] - 1])
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
                    if (ElencaNoteInBrano(riferimento).Count > 0)
                    {
                        branoEsiste = true;
                    }
                }
            }
            else // if (riferimento.Versetti)
            {
                foreach (string nota in riferimento.Note)
                {
                    if (!string.IsNullOrEmpty(nota) && GetNumeroNotaTitolo(nota) >= 0)
                    {
                        branoEsiste = true;
                        break;
                    }
                }
            }
            return branoEsiste;
        }

        internal Collection<string> GetRadiciDiverse()
        {
            Collection<string> listaRadiciDiverse = [];
            if (info.Tipo == TestoTipi.Bibbia)
            {
                foreach (RadiceDiversa radice in radiciDiverse)
                {
                    byte[] rif = RiferimentoDaNumeroVersetto(radice.OccorrenzaRadice.Voce);
                    listaRadiciDiverse.Add(new StringBuilder().Append(rif[0]).Append('|').Append(rif[1]).Append('|').Append(rif[2]).Append('|').Append(radice.OccorrenzaRadice.Parola).Append('|').Append(radice.NuovaRadice).ToString());
                }
            }
            else
            {
                foreach (RadiceDiversa radice in radiciDiverse)
                {
                    listaRadiciDiverse.Add(new StringBuilder().Append(radice.OccorrenzaRadice.Voce).Append('|').Append(radice.OccorrenzaRadice.Parola).Append('|').Append(radice.NuovaRadice).ToString());
                }
            }
            return listaRadiciDiverse;
        }

        internal void CambiaSolaLettura()
        {
            switch (info.Bloccato)
            {
                case BloccatoTipi.Sbloccato:
                    info.Bloccato = BloccatoTipi.Bloccato;
                    noteModificate = true;
                    break;
                case BloccatoTipi.Bloccato:
                    info.Bloccato = BloccatoTipi.Sbloccato;
                    noteModificate = true;
                    break;
                case BloccatoTipi.BloccatoSempre: // non fare niente
                    break;
            }
        }
        #endregion

        static bool EndsWith(StringBuilder sb, string value)
        {
            if (sb.Length < value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (sb[sb.Length - value.Length + i] != value[i])
                    return false;
            }

            return true;
        }

        [GeneratedRegex(@"\\u(-?\d+)\??")]
        private static partial Regex RegexConvertiUnicodeCaratteri();
        [GeneratedRegex(@"\\'([0-9a-fA-F]{2})")]
        private static partial Regex RegexRtf();
        [GeneratedRegex(@"\\v\s*(?:\\f\d+\s*)*(?:\u0002|\\'02)\\v0\s*(?<anchor>.*?)\\v\s*(?:\u0003|\\'03)(?<type>[\u0005\u0006\u0007]|\\'0[567])(?<data>.*?)(?:\u0004|\\'04)(?:\\cf\d+\s*)*\\v0\s?", RegexOptions.Compiled)]
        private static partial Regex RegExConvertiIperlink();
    }
}
