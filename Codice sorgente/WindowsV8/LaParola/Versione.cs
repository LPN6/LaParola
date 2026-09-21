using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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

        private string[]? radici = null;
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

        private UInt32[]? listaRadiceDiParola = null;
        internal UInt32[] ListaRadiceDiParola
        {
            get
            {
                if (listaRadiceDiParola == null)
                {
                    if (listaRadiceDiParola == null)
                    {
                        lock (fileLock)
                        {
                            int numeroParole = Parole.Length;
                            int numeroRadici = Radici.Length; // serve solo per costringere la lettura delle radici, che imposta pRadiciDiParole correttamente
                            listaRadiceDiParola = new UInt32[numeroParole];
                            if (numeroRadici > 0 && pRadiciDiParole > 0)
                            { // quando pRadiciDiParole==0 (valore predefinito), non ci sono radici in questa versione
                              // numeroRadici>0 quindi non è necessario, ma è incluso per fare sì che la riga che definisce numeroRadici è usata
                                fs.Seek(pRadiciDiParole, SeekOrigin.Begin);
                                byte[] radiciArray = br.ReadBytes(numeroParole * 4);
                                Buffer.BlockCopy(radiciArray, 0, listaRadiceDiParola, 0, radiciArray.Length);
                            }
                        }
                    }
                }
                return listaRadiceDiParola;
            }
        }

        private StringBuilder[]? paroleDiRadice = null;

        private static readonly ConfrontoCI confrontoParole = new();

        private struct RadiceDiversa
        {
            public OccorrenzaParola OccorrenzaRadice;
            public string NuovaRadice;
        }
        private readonly List<RadiceDiversa> radiciDiverse = [];

        internal List<Int16[]> riferimentiDiversi = [];

        public struct CitazioneRiferimento
        {
            public byte[] Brano;
            public UInt32 NumeroNota;
        }
        private List<CitazioneRiferimento>? citazioniRiferimenti = null;
        public List<CitazioneRiferimento> CitazioniRiferimenti
        {
            get
            {
                if (citazioniRiferimenti == null)
                {
                    citazioniRiferimenti = [];
                    if (pCitazioniRiferimenti > pInizioDati) // quando ==, non ci sono collegamenti a riferimenti
                    {
                        lock (fileLock)
                        {
                            fs.Seek(pCitazioniRiferimenti, SeekOrigin.Begin);
                            UInt32 nCitazioniRiferimenti = br.ReadUInt32();

                            if (nCitazioniRiferimenti > 0)
                            {
                                // 1. Pre-allocate list capacity to eliminate repeated array resizing during .Add()
                                citazioniRiferimenti.Capacity = citazioniRiferimenti.Count + (int)nCitazioniRiferimenti;

                                // 2. Read bytes into buffer and wrap in a Span for zero-cost slicing
                                byte[] citazioniArray = br.ReadBytes(10 * (int)nCitazioniRiferimenti);
                                ReadOnlySpan<byte> span = citazioniArray;
                                int offset;
                                for (int i = 0; i < nCitazioniRiferimenti; ++i)
                                {
                                    offset = 10 * i;

                                    citazioniRiferimenti.Add(new CitazioneRiferimento
                                    {
                                        Brano = span.Slice(offset, 6).ToArray(),
                                        NumeroNota = BinaryPrimitives.ReadUInt32LittleEndian(span[(offset + 6)..])
                                    });
                                }
                            }
                        }
                    }
                }
                return citazioniRiferimenti;
            }
        }

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

        //private readonly bool isRunningOnMono = false;

        private readonly Lock fileLock = new();
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
            //isRunningOnMono = (Type.GetType("Mono.Runtime") != null);
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
                            if (nRadiciDiverse > 0)
                            {
                                radiciDiverse.Capacity = radiciDiverse.Count + (int)nRadiciDiverse;
                                Span<byte> ref6 = stackalloc byte[6];
                                for (UInt32 i = 0; i < nRadiciDiverse; ++i)
                                {
                                    br.Read(ref6[..3]);
                                    ref6[..3].CopyTo(ref6[3..]);
                                    versetto = NumeroVersettoDaRiferimento(ref6.ToArray());

                                    radiciDiverse.Add(new RadiceDiversa
                                    {
                                        OccorrenzaRadice = new OccorrenzaParola
                                        {
                                            Voce = versetto[0],
                                            Parola = br.ReadUInt16()
                                        },
                                        NuovaRadice = br.ReadString()
                                    });
                                }
                            }
                            break;
                        case 1:
                            if (nRadiciDiverse > 0)
                            {
                                radiciDiverse.Capacity = radiciDiverse.Count + (int)nRadiciDiverse;
                                for (uint i = 0; i < nRadiciDiverse; ++i)
                                {
                                    radiciDiverse.Add(new RadiceDiversa
                                    {
                                        OccorrenzaRadice = new OccorrenzaParola
                                        {
                                            Voce = br.ReadUInt32(),
                                            Parola = br.ReadUInt16()
                                        },
                                        NuovaRadice = br.ReadString()
                                    });
                                }
                            }
                            break;
                    }
                }

                if (pRiferimentiDiversi > pInizioDati) // quando ==, non ci sono riferimenti diversi in questa versione
                {
                    fs.Seek(pRiferimentiDiversi, SeekOrigin.Begin);
                    UInt32 nRiferimentiDiversi = br.ReadUInt32();
                    if (nRiferimentiDiversi > 0)
                    {
                        // 1. Pre-allocate list capacity to eliminate internal array resizes
                        riferimentiDiversi.Capacity = riferimentiDiversi.Count + (int)nRiferimentiDiversi;

                        // 2. Read all 12-byte blocks (6 shorts * 2 bytes = 12 bytes per item) at once
                        byte[] byteBuffer = br.ReadBytes(12 * (int)nRiferimentiDiversi);

                        // 3. Reinterpret the byte array as a span of shorts (0-cost memory cast)
                        ReadOnlySpan<short> shortSpan = MemoryMarshal.Cast<byte, short>(byteBuffer);

                        for (int i = 0; i < nRiferimentiDiversi; ++i)
                        {
                            // Slices 6 shorts and allocates the 6-element array in a single vectorized memory copy
                            riferimentiDiversi.Add(shortSpan.Slice(i * 6, 6).ToArray());
                        }
                    }
                }

                if (pNoteInOrdine > pInizioDati) // quando ==, non ci sono note in ordine
                {
                    fs.Seek(pNoteInOrdine, SeekOrigin.Begin);
                    UInt32 nNoteInOrdine = br.ReadUInt32();
                    if (nNoteInOrdine > 0)
                    {
                        int count = (int)nNoteInOrdine;

                        // 1. Pre-allocate list capacity to avoid dynamic array resizes during .Add()
                        noteInOrdine.Capacity = noteInOrdine.Count + count;

                        // 2. Read strings cleanly with typed loop bound
                        for (int i = 0; i < count; ++i)
                        {
                            noteInOrdine.Add(br.ReadString());
                        }
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
                    using FileStream fsNuovo = new(nomeFile, FileMode.Create, FileAccess.Write);
                    using BinaryWriter bwNuovo = new(fsNuovo);

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
                            radiceDaRicercare = Radici[(int)(ListaRadiceDiParola[i])];
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
                            parola = Radici[(int)(ListaRadiceDiParola[numeroParola])];
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
            int numeroVolte = 0;
            ReadOnlySpan<char> span = ParoleNumeriDiRadice(radice).AsSpan();

            lock (fileLock)
            {
                int start = 0;
                while (start < span.Length)
                {
                    // 1. Slice by '|' without creating any string objects in memory
                    int pipeIndex = span[start..].IndexOf('|');
                    ReadOnlySpan<char> token;

                    if (pipeIndex < 0)
                    {
                        token = span[start..];
                        start = span.Length;
                    }
                    else
                    {
                        token = span.Slice(start, pipeIndex);
                        start += pipeIndex + 1;
                    }

                    if (token.IsEmpty) continue;

                    // 2. Fast 0-allocation int parsing directly from char span
                    int parolaNumero = int.Parse(token, NumberStyles.Integer, CultureInfo.InvariantCulture);

                    // 3. 64-bit long math for stream seeking
                    fs.Seek(pParoleIndiceIndice + (4L * parolaNumero), SeekOrigin.Begin);

                    uint inizioVersetti = br.ReadUInt32();
                    uint fineVersetti = br.ReadUInt32();

                    numeroVolte += (int)(fineVersetti - inizioVersetti) / 6;
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

            int numeroParola = NumeroDiParola(parola);
            return ((numeroParola >= 0) ? Radici[(int)(ListaRadiceDiParola[numeroParola])] : "");
        }

        public UInt32 RadiceNumeroDiParola(string parola)
        {
            // la radice normale, non un'eventuale radice diversa
            if (Radici.Length == 0)
            {
                return (UInt32)(Array.BinarySearch(Radici, "*", confrontoParole));
            }

            int numeroParola = NumeroDiParola(parola);
            return ((numeroParola >= 0) ? (ListaRadiceDiParola[numeroParola]) : (UInt32)(Array.BinarySearch(Radici, "*", confrontoParole)));
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
                    int numeroRadici = Radici.Length;
                    paroleDiRadice = new StringBuilder[numeroRadici];
                    for (int i = 0; i < numeroRadici; ++i)
                    {
                        paroleDiRadice[i] = new StringBuilder();
                    }

                    int numeroParole = Parole.Length;
                    for (UInt32 i = 0; i < numeroParole; ++i)
                    {
                        paroleDiRadice[ListaRadiceDiParola[i]].Append(i.ToString(CultureInfo.InvariantCulture)).Append('|');
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
            // Force lazy initialization of ListaRadiceDiParola FIRST.
            // If radici was null on disk, this reads pRadiciDiParole from file before we replace it.
            UInt32[] listaTemp = ListaRadiceDiParola;

            lock (fileLock)
            {
                radici = (string[])elencoRadici.Clone();
            }

            int numeroParole = Parole.Length;
            int limit = Math.Min(numeroParole, radiceStringaDiParole.Length);
            int index;
            for (int i = 0; i < limit; ++i)
            {
                index = Array.BinarySearch(radici, radiceStringaDiParole[i], confrontoParole);
                listaTemp[i] = index >= 0 ? (UInt32)index : 0;
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
            return (CitazioniRiferimenti.Count > 0);
        }

        public Collection<string> GetRiferimentiCitati()
        {
            Collection<string> riferimentiCitati = [];
            int numeroCitazioniInCollezione = CitazioniRiferimenti.Count;
            for (int i = 0; i < numeroCitazioniInCollezione; ++i)
            {
                riferimentiCitati.Add(new StringBuilder().Append(CitazioniRiferimenti[i].Brano[0]).Append('|').Append(CitazioniRiferimenti[i].Brano[1]).Append('|').Append(CitazioniRiferimenti[i].Brano[2]).Append('|').Append(CitazioniRiferimenti[i].Brano[3]).Append('|').Append(CitazioniRiferimenti[i].Brano[4]).Append('|').Append(CitazioniRiferimenti[i].Brano[5]).Append('|').Append(CitazioniRiferimenti[i].NumeroNota).Append('|').ToString());
            }

            return riferimentiCitati;
        }

        public Riferimento Citazioni(Riferimento riferimento)
        {
            List<int> note = [];
            int numeroBrani = riferimento.Count;
            int numeroCitazioniInCollezione = CitazioniRiferimenti.Count;
            int posizione;
            for (int i = 0; i < numeroBrani; ++i)
            {
                for (int j = 0; j < numeroCitazioniInCollezione; ++j)
                {
                    if (ConfrontaBrani(riferimento.Brani[i], CitazioniRiferimenti[j].Brano) == 0)
                    {
                        posizione = note.BinarySearch((int)(CitazioniRiferimenti[j].NumeroNota));
                        if (posizione < 0) // non esiste già
                        {
                            note.Insert(~posizione, (int)(CitazioniRiferimenti[j].NumeroNota));
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
            => TestoBranoAsync(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, conNomiDelleNote: true, paroleRicercate ?? new Riferimento());

        internal Task<string> TestoBranoAsync(
            Riferimento riferimento,
            Collection<string> collezioniDaVisualizzare,
            List<Riferimento> noteDaVisualizzare,
            bool conNomiDelleNote)
            => TestoBranoAsync(riferimento, collezioniDaVisualizzare, noteDaVisualizzare, conNomiDelleNote, new Riferimento());

        internal async Task<string> TestoBranoAsync(
            Riferimento riferimento,
            Collection<string> collezioniDaVisualizzare,
            List<Riferimento> noteDaVisualizzare,
            bool conNomiDelleNote,
            Riferimento paroleRicercate)
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
                            string testoVersetto = "", testoVersettoTitolo, testoVersettoTestoBiblico;
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
                                        // 1. Format verse string efficiently ("001", "042", etc.)
                                        versettoStringa = $"{capitoloStringa}{vers:000}";
                                        string versettoStringaInTestoNascosto = MainWindow.LPN_ANCORA + versettoStringa;

                                        // 2. Build the reference body first to check if it's empty
                                        riferimentoVersetto.Length = 0;
                                        switch (genitore.Formato.RiferimentoFormato)
                                        {
                                            case RiferimentoFormato.Intero:
                                                riferimentoVersetto.Append(libroCapitoloPunt).Append(vers);
                                                break;
                                            case RiferimentoFormato.Abbreviazione:
                                                if (vers == vers0)
                                                {
                                                    riferimentoVersetto.Append(libroCapitoloPunt).Append(vers);
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

                                        // 3. Prepend formatting WITHOUT StringBuilder.Insert(0, ...)
                                        bool haRiferimento = riferimentoVersetto.Length > 0;
                                        string bodyRef = riferimentoVersetto.ToString();
                                        riferimentoVersetto.Length = 0;

                                        riferimentoVersetto.Append('{').Append(formatoRiferimento);
                                        if (haRiferimento)
                                        {
                                            riferimentoVersetto.Append(' ').Append(bodyRef);
                                        }

                                        // 4. Build context search references efficiently using "000" formatting
                                        if (soloUnVersetto && genitore.Formato.RiferimentoContestoRicerche && genitore.Formato.RiferimentoFormato != RiferimentoFormato.Nessuno)
                                        {
                                            int prevVers = vers > 1 ? vers - 1 : vers;
                                            int nextVers = vers + 1;

                                            riferimentoVersetto.Append(formatoRiferimentoContestoInizio)
                                                               .Append(capitoloStringa)
                                                               .Append(prevVers.ToString("000", CultureInfo.InvariantCulture))
                                                               .Append("0000+")
                                                               .Append(capitoloStringa)
                                                               .Append(nextVers.ToString("000", CultureInfo.InvariantCulture))
                                                               .Append(formatoRiferimentoContestoFine);
                                        }

                                        riferimentoVersetto.Append(haRiferimento ? "}\\~" : "}");

                                        // 5. Spacing check for display text
                                        if (testoDaVisualizzare.Length > 0 &&
                                            !EndsWith(testoDaVisualizzare, @"\par") &&
                                            !EndsWith(testoDaVisualizzare, @"\par}") &&
                                            !EndsWith(testoDaVisualizzare, @"\par }") &&
                                            testoDaVisualizzare[^1] != ' ')
                                        {
                                            testoDaVisualizzare.Append(' ');
                                        }

                                        testoDaVisualizzare.Append(versettoStringaInTestoNascosto);

                                        // 6. Read verse text
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

                                        // Search highlights lookup
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
                                            {
                                                p = testoVersettoTestoBiblico.IndexOf(@"\lptit0 ", StringComparison.Ordinal);
                                                if (p > -1)
                                                {
                                                    testoVersettoTestoBiblico = testoVersettoTestoBiblico[..p1] + testoVersettoTestoBiblico[(p + 8)..];
                                                }
                                                else
                                                {
                                                    testoVersettoTestoBiblico = testoVersettoTestoBiblico[..p1] + testoVersettoTestoBiblico[(p1 + 8)..];
                                                }
                                            }
                                        }

                                        // 7. HIGH-PERFORMANCE NOTE MATCHING (Zero Allocations, No Exceptions)
                                        ReadOnlySpan<char> versetSpan = versettoStringa.AsSpan();

                                        for (int iCommentario = 0; iCommentario < numeroCommentari; ++iCommentario)
                                        {
                                            int numeroNote = noteDaVisualizzare[iCommentario].Count;
                                            for (int iNota = numeroNote - 1; iNota >= 0; --iNota)
                                            {
                                                string notaRaw = noteDaVisualizzare[iCommentario].Note[iNota];
                                                ReadOnlySpan<char> notaSpan = notaRaw.AsSpan();

                                                if (notaSpan.Length < 9) continue;

                                                // Zero-allocation Span checks instead of string.Concat & Substring
                                                bool isMatch = notaSpan.Slice(1, 8).SequenceEqual(versetSpan)
                                                    || (notaSpan.Slice(6, 3).SequenceEqual("000") && notaSpan.Slice(1, 5).SequenceEqual(versetSpan[..5]) && versetSpan.Slice(5, 3).SequenceEqual("001"))
                                                    || (notaSpan.Slice(3, 6).SequenceEqual("000000") && notaSpan.Slice(1, 2).SequenceEqual(versetSpan[..2]) && versetSpan.Slice(2, 6).SequenceEqual("001001"));

                                                if (isMatch)
                                                {
                                                    ushort numeroDellaParola = 0;
                                                    if (notaSpan.Length >= 13)
                                                    {
                                                        // Fast integer parsing with no exceptions thrown on failure
                                                        ushort.TryParse(notaSpan.Slice(9, 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out numeroDellaParola);
                                                    }

                                                    testoVersettoTestoBiblico = ModificaFormatoParole(
                                                        testoVersettoTestoBiblico,
                                                        numeroDellaParola,
                                                        "",
                                                        @"{\v " + RichTextBoxEx.InizioLink + @"}*{\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkNota + collezioniDaVisualizzare[iCommentario] + @"\\" + notaRaw + RichTextBoxEx.FineLink2 + "}" + (iCommentario == 0 ? "" : " "),
                                                        info.Lingua);
                                                }
                                            }
                                        }

                                        // 8. Optimized Greek / Hebrew RTF wrapping
                                        if (greco)
                                        {
                                            if (testoVersettoTestoBiblico.EndsWith(@"\par }", StringComparison.OrdinalIgnoreCase))
                                                testoVersettoTestoBiblico = @"{" + formatoGreco + testoVersettoTestoBiblico[..^6] + @"}\par ";
                                            else
                                                testoVersettoTestoBiblico = @"{" + formatoGreco + testoVersettoTestoBiblico + "}";
                                        }
                                        if (ebraico)
                                        {
                                            if (testoVersettoTestoBiblico.EndsWith(@"\par }", StringComparison.OrdinalIgnoreCase))
                                                testoVersettoTestoBiblico = @"{" + formatoEbraico + testoVersettoTestoBiblico[..^6] + @"}\par ";
                                            else
                                                testoVersettoTestoBiblico = @"{" + formatoEbraico + testoVersettoTestoBiblico + "}";
                                        }

                                        // 9. Output assembly
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
                                                    if (testoVersettoTestoBiblico.EndsWith(@"\par ", StringComparison.Ordinal))
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
                    testoComeStringa = await TestoBranoAsync(ElencaNoteInBrano(riferimento), collezioniDaVisualizzare, noteDaVisualizzare, conNomiDelleNote, paroleRicercate);
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
            int numeroNote = riferimento.Note.Count;
            if (numeroNote == 0) return [];

            // 1. Pre-allocate List capacity (each note adds 1 to 2 strings, plus optional separator lines)
            int maxCapacity = numeroNote * (conNomiDelleNote ? 3 : 2);
            List<string> stringheRtf = new(maxCapacity);

            // 2. Cache header & format strings outside the loop to avoid repeated concatenations
            string rtfHeader = genitore.RtfIntestazione();
            string rtfSeparator = rtfHeader + @"\par}";
            string inizioFormatoRicercaNote = "{" + formatoRicercaNote + " ";
            string inizioFormatoRiferimento = "{" + formatoRiferimento + " ";
            string inizioFormatoRicerca = "{" + formatoRicerca + " ";
            string inizioInizioRiferimento = MainWindow.LPN_ANCORA;

            int numeroParoleRicercate = paroleRicercate.Count;
            int ultimaParolaRicercata = -1;

            // 3. Reuse a single StringBuilder for all title operations
            StringBuilder sb = new(256);

            for (int i = 0; i < numeroNote; ++i)
            {
                if (i > 0)
                {
                    // Empty line between passages
                    stringheRtf.Add(rtfSeparator);
                }

                string titoloNota = riferimento.Note[i];
                bool notaSuBrano = titoloNota.StartsWith('#');
                string titoloNotaDaLeggere = notaSuBrano ? genitore.ConvertiTitoloNotaARiferimento(titoloNota) : titoloNota;

                if (conNomiDelleNote)
                {
                    sb.Clear();
                    sb.Append(rtfHeader);

                    if (notaSuBrano && titoloNota.Length >= 9)
                    {
                        // Append Span directly to StringBuilder (0 heap allocations)
                        sb.Append(inizioInizioRiferimento).Append(titoloNota.AsSpan(1, 8));
                    }

                    sb.Append(inizioFormatoRiferimento)
                      .Append(ConvertiUnicodeInRtf(titoloNotaDaLeggere))
                      .Append(@"}\par}");

                    stringheRtf.Add(sb.ToString());
                }

                string testoModificato = GetNotaTestoTitolo(titoloNota);
                //string testoModificato = ModificaFormatoParole(GetNotaTestoTitolo(titoloNota), riferimento.numeroParola[i], inizioFormatoRicercaNote, "}", info.Lingua);

                // 4. search for matching searched words with clean break
                for (int numeroParolaRicercata = ultimaParolaRicercata + 1; numeroParolaRicercata < numeroParoleRicercate; ++numeroParolaRicercata)
                {
                    int cmp = string.CompareOrdinal(titoloNota, paroleRicercate.Note[numeroParolaRicercata]);
                    if (cmp > 0)
                    {
                        ultimaParolaRicercata = numeroParolaRicercata;
                    }
                    else if (cmp < 0)
                    {
                        // Clean break out of loop when notes no longer match
                        break;
                    }
                    else
                    {
                        testoModificato = ModificaFormatoParole(testoModificato, paroleRicercate.numeroParola[numeroParolaRicercata], inizioFormatoRicerca, "}", info.Lingua);
                    }
                }

                // 5. Wrap non-RTF content with RTF headers
                if (!testoModificato.StartsWith(@"{\rtf", StringComparison.Ordinal) && !testoModificato.EndsWith('}'))
                {
                    testoModificato = rtfHeader + testoModificato.Replace("\r\n", @"\par ") + "}";
                }

                stringheRtf.Add(ConvertiUnicodeInRtf(testoModificato));
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

        private static string ModificaFormatoParole(
            string testoDaModificare,
            List<ushort> numeriParoleDaModificare,
            string formatoPrimaDellaParola,
            string formatoDopoLaParola,
            string lingua)
        {
            // 1. Guard check - Early exit
            if ((formatoPrimaDellaParola == "{" && formatoDopoLaParola == "}") ||
                (string.IsNullOrEmpty(formatoPrimaDellaParola) && string.IsNullOrEmpty(formatoDopoLaParola)) ||
                numeriParoleDaModificare == null ||
                numeriParoleDaModificare.Count == 0)
            {
                return testoDaModificare;
            }

            string[] lingue = SplitString(lingua.ToLowerInvariant(), '|');
            string linguaPrincipale = lingue.Length >= 1 ? lingue[0] : "";
            bool dizionarioGreco = linguaPrincipale == "el" && lingue.Length >= 2;
            bool dizionarioEbraico = linguaPrincipale.StartsWith("he", StringComparison.Ordinal) && lingue.Length >= 2;

            // In lingue RTL, rimuovere eventuali codici \v da formatoPrima / formatoDopo
            if (RightToLeft(linguaPrincipale))
            {
                formatoPrimaDellaParola = RimuoviTestoNascosto(formatoPrimaDellaParola, @"\v ", @"\v0", 3);
                formatoDopoLaParola = RimuoviTestoNascosto(formatoDopoLaParola, @"{\v ", "}", 1);
                formatoDopoLaParola = RimuoviTestoNascosto(formatoDopoLaParola, @"\v ", @"\v0", 3);
            }

            // Deduplicazione senza modificare la lista originale del chiamante
            List<ushort> meNumeri = new(numeriParoleDaModificare.Count);
            for (int i = 0; i < numeriParoleDaModificare.Count; i++)
            {
                if (i == 0 || numeriParoleDaModificare[i] != numeriParoleDaModificare[i - 1])
                {
                    meNumeri.Add(numeriParoleDaModificare[i]);
                }
            }

            int nParoleDaCambiare = meNumeri.Count;
            int iParolaDaCambiare = 0;
            int nProssimaParolaDaCambiare = meNumeri[0];
            int paroleTrovate = 0;
            StringBuilder parola = new();

            // Output Builder per evitare string.Insert in loop
            StringBuilder sb = new(testoDaModificare.Length + 64);
            int lastAppendedIndex = 0;

            int statoCambiamento = 0; // 0=niente, 1=cambiare la prossima, 2=in corso di cambiamento
            if (nProssimaParolaDaCambiare == 1)
            {
                statoCambiamento = 1;
                ++iParolaDaCambiare;
                if (iParolaDaCambiare < nParoleDaCambiare)
                {
                    nProssimaParolaDaCambiare = meNumeri[iParolaDaCambiare];
                }
            }

            // Determinazione del carattere iniziale (salta intestazione RTF)
            int carattereIniziale = 0;
            int idx = testoDaModificare.IndexOf(@"\viewkind", StringComparison.Ordinal);
            if (idx > 0)
            {
                carattereIniziale = idx + 10;
            }

            idx = testoDaModificare.IndexOf(@"\deflang", carattereIniziale, StringComparison.Ordinal);
            if (idx > 0)
            {
                carattereIniziale = idx + 12;
            }

            if (testoDaModificare.StartsWith(@"{\rtf", StringComparison.Ordinal) && carattereIniziale == 0)
            {
                idx = testoDaModificare.IndexOf(@"\pard", carattereIniziale, StringComparison.Ordinal);
                if (idx > 0)
                {
                    carattereIniziale = idx + 6;
                }
            }

            if (nProssimaParolaDaCambiare == 0)
            {
                sb.Append(testoDaModificare, 0, carattereIniziale);
                sb.Append(formatoPrimaDellaParola).Append(formatoDopoLaParola);
                lastAppendedIndex = carattereIniziale;

                ++iParolaDaCambiare;
                if (iParolaDaCambiare < nParoleDaCambiare)
                {
                    nProssimaParolaDaCambiare = meNumeri[iParolaDaCambiare];
                }
            }

            char c;
            bool analizzaParola;
            string linguaDaUsare;
            int iCarattere1, iCarattere2, iCarattere3;

            for (int i = carattereIniziale; i < testoDaModificare.Length; ++i)
            {
                c = testoDaModificare[i];

                // FIX BUG HYPERLINK: Salta l'intero blocco di istruzioni del campo RTF (\fldinst)
                if (c == '\\' && i <= testoDaModificare.Length - 8 && testoDaModificare.AsSpan(i, 8).Equals(@"\fldinst", StringComparison.Ordinal))
                {
                    if (parola.Length > 0)
                    {
                        if (statoCambiamento == 2)
                        {
                            sb.Append(testoDaModificare, lastAppendedIndex, i - lastAppendedIndex);
                            sb.Append(formatoDopoLaParola);
                            lastAppendedIndex = i;
                            statoCambiamento = 0;
                        }
                        ++paroleTrovate;
                        parola.Clear();
                    }

                    int depth = 1;
                    int j = i + 8;
                    while (j < testoDaModificare.Length && depth > 0)
                    {
                        if (testoDaModificare[j] == '\\')
                        {
                            if (j <= testoDaModificare.Length - 7 && testoDaModificare.AsSpan(j, 7).Equals(@"\fldrslt", StringComparison.Ordinal))
                            {
                                break;
                            }
                            if (j + 1 < testoDaModificare.Length)
                            {
                                j++;
                            }
                        }
                        else if (testoDaModificare[j] == '{')
                        {
                            depth++;
                        }
                        else if (testoDaModificare[j] == '}')
                        {
                            depth--;
                        }
                        j++;
                    }
                    i = j - 1;
                    continue;
                }

                if (IsLetteraONumero(c) || (c == '\\' && i < testoDaModificare.Length - 3 && (testoDaModificare[i + 1] == '\'' || (testoDaModificare[i + 1] == 'u' && char.IsDigit(testoDaModificare[i + 2])))))
                {
                    if (i <= testoDaModificare.Length - 1 && testoDaModificare[i] != RichTextBoxEx.InizioLink)
                    {
                        if (IsLetteraONumero(c))
                        {
                            parola.Append(c);
                        }
                        else if (testoDaModificare[i + 1] == '\'')
                        {
                            parola.Append(testoDaModificare.AsSpan(i, 4));
                        }
                        else if (testoDaModificare[i + 1] == 'u' && char.IsDigit(testoDaModificare[i + 2]))
                        {
                            int qIdx = testoDaModificare.IndexOf('?', i);
                            int len = (qIdx > i) ? (qIdx - i + 1) : 1;
                            parola.Append(testoDaModificare.AsSpan(i, len));
                        }

                        if (statoCambiamento == 1)
                        {
                            sb.Append(testoDaModificare, lastAppendedIndex, i - lastAppendedIndex);
                            sb.Append(formatoPrimaDellaParola);
                            lastAppendedIndex = i;
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
                                if (i + 3 < testoDaModificare.Length && testoDaModificare[i + 2] == '0' && testoDaModificare[i + 3] == '0')
                                {
                                    i += 5;
                                }
                                else
                                {
                                    int qIdx = testoDaModificare.IndexOf('?', i);
                                    if (qIdx > i)
                                    {
                                        i = qIdx;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (char.IsPunctuation(c) || char.IsWhiteSpace(c) || char.IsSymbol(c) || char.GetUnicodeCategory(c) == UnicodeCategory.Format)
                {
                    analizzaParola = true;
                    int carattereDaInserire = i;

                    if (c == '\'')
                    {
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
                                      || (i < testoDaModificare.Length - 3 && testoDaModificare.AsSpan(i + 1, 3).Equals("tis", StringComparison.OrdinalIgnoreCase) && (i == testoDaModificare.Length - 4 || !IsLetteraONumero(testoDaModificare[i + 4])))
                                      || (i < testoDaModificare.Length - 4 && testoDaModificare.AsSpan(i + 1, 4).Equals("twas", StringComparison.OrdinalIgnoreCase) && (i == testoDaModificare.Length - 5 || !IsLetteraONumero(testoDaModificare[i + 5])))))
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
                                    {
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
                                         && IsLetteraONumero(testoDaModificare[i - 1]) && (i == testoDaModificare.Length - 3 || !IsLetteraONumero(testoDaModificare[i + 3])))
                                    {
                                        ReadOnlySpan<char> sub = testoDaModificare.AsSpan(i + 1, 2);
                                        if (sub.Equals("en", StringComparison.Ordinal) || sub.Equals("er", StringComparison.Ordinal) ||
                                            sub.Equals("ll", StringComparison.Ordinal) || sub.Equals("lt", StringComparison.Ordinal) ||
                                            sub.Equals("ry", StringComparison.Ordinal) || sub.Equals("st", StringComparison.Ordinal) ||
                                            sub.Equals("ve", StringComparison.Ordinal))
                                        {
                                            parola.Append(c);
                                            analizzaParola = false;
                                        }
                                    }
                                    else if (i < testoDaModificare.Length - 4
                                        && IsLetteraONumero(testoDaModificare[i - 1]) && (i == testoDaModificare.Length - 3 || !IsLetteraONumero(testoDaModificare[i + 5]))
                                        && testoDaModificare.AsSpan(i + 1, 4).Equals("ring", StringComparison.Ordinal))
                                    {
                                        parola.Append(c);
                                        analizzaParola = false;
                                    }
                                }
                                break;

                            case "it":
                                if (i > 0 && i < testoDaModificare.Length - 1)
                                {
                                    if ((IsLetteraONumero(testoDaModificare[i - 1]) && (IsLetteraONumero(testoDaModificare[i + 1]) || testoDaModificare[i + 1] == '\'' || testoDaModificare[i + 1] == '«' || testoDaModificare[i + 1] == ']' || (testoDaModificare[i + 1] == ')' && testoDaModificare.IndexOf("('", StringComparison.Ordinal) < i))) || (Array.BinarySearch(Texts.paroleItalianeConApostrofe, parola.ToString()) >= 0))
                                    {
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

                            case "":
                                parola.Append(c);
                                break;
                        }
                    }
                    else if (c == '[' || c == ']')
                    {
                        if (i > 0 && i < testoDaModificare.Length - 1 && IsLettera(testoDaModificare[i - 1]) && IsLettera(testoDaModificare[i + 1]))
                        {
                            analizzaParola = false;
                        }
                    }
                    else if (c == '-')
                    {
                        if (i > 0 && i < testoDaModificare.Length - 1)
                        {
                            if (((IsLettera(testoDaModificare[i - 1]) || (testoDaModificare[i - 1] == '?' && i > 1 && char.IsDigit(testoDaModificare[i - 2]))) &&
                                (IsLettera(testoDaModificare[i + 1]) || (i < testoDaModificare.Length - 2 && testoDaModificare.AsSpan(i + 1, 2).Equals(@"\u", StringComparison.Ordinal))))
                                || (dizionarioEbraico && testoDaModificare[i - 1] == '\'' && char.IsLetter(testoDaModificare[i + 1])))
                            {
                                parola.Append(c);
                                analizzaParola = false;
                            }
                        }
                    }
                    else if (c == '}')
                    {
                        if (i > 0 && i < testoDaModificare.Length - 1 && IsLettera(testoDaModificare[i - 1]) && IsLettera(testoDaModificare[i + 1]))
                        {
                            analizzaParola = false;
                        }
                    }
                    else if (c == '{')
                    {
                        if (i > 0 && IsLettera(testoDaModificare[i - 1]))
                        {
                            analizzaParola = false;
                        }
                    }
                    else if (c == '\\')
                    {
                        if (i < testoDaModificare.Length - 6 && testoDaModificare.AsSpan(i, 7).Equals(@"\lptit1", StringComparison.Ordinal))
                        {
                            idx = testoDaModificare.IndexOf(@"\lptit0 ", i, StringComparison.Ordinal);
                            i = (idx >= 0) ? (idx + 7) : (testoDaModificare.Length - 1);
                        }
                        else if (i + 1 < testoDaModificare.Length && testoDaModificare[i + 1] == 'v' &&
                                 !(i + 2 < testoDaModificare.Length && testoDaModificare[i + 2] == '0'))
                        {
                            idx = testoDaModificare.IndexOf(@"\v0", i, StringComparison.Ordinal);
                            i = (idx >= 0) ? (idx + 2) : (testoDaModificare.Length - 1);
                        }
                        else
                        {
                            iCarattere1 = testoDaModificare.IndexOf('\\', i + 1) - 1;
                            if (iCarattere1 == i)
                            {
                                iCarattere1 = -1;
                            }

                            iCarattere2 = testoDaModificare.IndexOf(' ', i);
                            iCarattere3 = testoDaModificare.IndexOf('\n', i);
                            if (iCarattere3 > 0 && (iCarattere3 < iCarattere2 || iCarattere2 < 0))
                                iCarattere2 = iCarattere3; // trova primo whitespace, poi butta e riutilizza iCarattere3
                            iCarattere3 = testoDaModificare.IndexOf('{', i);
                            if (iCarattere1 >= 0 && (iCarattere1 < iCarattere2 || iCarattere2<0) &&( iCarattere1<iCarattere3||iCarattere3<0))
                            {
                                if (i > 0 && IsLettera(testoDaModificare[i - 1]) && iCarattere1 < testoDaModificare.Length - 2 && testoDaModificare[iCarattere1 + 2] == '\'')
                                {
                                    analizzaParola = false;
                                }
                                iCarattere3 = iCarattere1;
                            }
                            else if (iCarattere2>= 0 && (iCarattere2 < iCarattere3||iCarattere3<0))
                            {
                                if (i > 0 && IsLettera(testoDaModificare[i - 1]) && iCarattere2 >= 0 && iCarattere2 < testoDaModificare.Length - 1 && !testoDaModificare.AsSpan(i, iCarattere2 - i).Equals(@"\par", StringComparison.Ordinal) && !testoDaModificare.AsSpan(i, iCarattere2 - i).Equals("\\par\r", StringComparison.Ordinal) && IsLettera(testoDaModificare[iCarattere2 + 1]))
                                {
                                    analizzaParola = false;
                                }
                                iCarattere3 = iCarattere2;
                            }
                            else if (iCarattere3 >= 0)
                            {
                                if (i > 0 && IsLettera(testoDaModificare[i - 1]) && iCarattere3 < testoDaModificare.Length - 1 && IsLettera(testoDaModificare[iCarattere3 + 1]))
                                {
                                    analizzaParola = false;
                                }
                            }
                            else
                            {
                                iCarattere3 = testoDaModificare.Length - 1;
                            }

                            i = iCarattere3;
                        }
                    }

                    if (parola.Length > 0 && analizzaParola)
                    {
                        if (statoCambiamento == 2)
                        {
                            sb.Append(testoDaModificare, lastAppendedIndex, carattereDaInserire - lastAppendedIndex);
                            sb.Append(formatoDopoLaParola);
                            lastAppendedIndex = carattereDaInserire;
                            statoCambiamento = 0;
                        }

                        ++paroleTrovate;
                        if (paroleTrovate == nProssimaParolaDaCambiare - 1)
                        {
                            statoCambiamento = 1;
                            ++iParolaDaCambiare;
                            if (iParolaDaCambiare < nParoleDaCambiare)
                            {
                                nProssimaParolaDaCambiare = meNumeri[iParolaDaCambiare];
                            }
                        }
                        parola.Clear();
                    }
                }
            }

            // Append standard di chiusura
            if (statoCambiamento == 2)
            {
                sb.Append(testoDaModificare, lastAppendedIndex, testoDaModificare.Length - lastAppendedIndex);
                sb.Append(formatoDopoLaParola);
            }
            else
            {
                sb.Append(testoDaModificare, lastAppendedIndex, testoDaModificare.Length - lastAppendedIndex);
            }

            // 2. Main return guaranteed on all paths
            return sb.ToString();
        }
        private static string RimuoviTestoNascosto(string input, string tagInizio, string tagFine, int offsetFine)
        {
            int idx;
            while ((idx = input.IndexOf(tagInizio, StringComparison.Ordinal)) >= 0)
            {
                int idxFine = input.IndexOf(tagFine, idx + tagInizio.Length, StringComparison.Ordinal);
                if (idxFine < 0) break;
                input = string.Concat(input.AsSpan(0, idx), input.AsSpan(idxFine + offsetFine));
            }
            return input;
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
                case BloccatoTipi.Permanente:
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
