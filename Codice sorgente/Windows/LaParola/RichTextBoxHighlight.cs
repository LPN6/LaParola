using TestiBiblici;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace LaParola
{
    #region classi

    internal enum TipoHighlight
    {
        Nessuno,
        Evidenziatore,
        Colore,
        Sottolineatura
    }

    public class Highlight
    {
        internal int libro;
        internal int capitolo;
        internal int versetto;
        internal string voce;
        /// <summary>
        /// Se è una versione della Bibbia, l'inizio della selezione evidenziata in confronto con l'inizio del capitolo.
        /// Se è una nota, l'inizio della selezione evidenziata in confronto con l'inizio della nota
        /// </summary>
        internal int inizio;
        internal int lunghezza;
        //        internal int fine;
        internal TipoHighlight tipo;
        /// <summary>
        /// Usato solo se è tipo 1 o 2 (evidenziatore o colore).
        /// </summary>
        internal Color colore;
        /// <summary>
        /// Usato solo se è tipo 3 (sottolineatura).
        /// </summary>
        internal byte tipoSottolineatura;
    }

    #endregion

    public class RichTextBoxHighlight : RichTextBoxEx
    {
        public List<Highlight> highlightAttuale = new List<Highlight>();
        TestoTipi tipoTesto;

        private byte libro;
        public byte Libro
        {
            get { return libro; }
            set { libro = value; }
        }

        private byte capitolo;
        public byte Capitolo
        {
            get { return capitolo; }
            set { capitolo = value; }
        }

        private byte versetto;
        public byte Versetto
        {
            get { return versetto; }
            set { versetto = value; }
        }

        private string voce;
        public string Voce
        {
            get { return voce; }
            set { voce = value; }
        }

        public void AggiungiHighlightDaFile()
        {
            tipoTesto = Principale.testi.Info(Versione).Tipo;

            highlightAttuale.Clear();
            string nomeFileHighlight = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Principale.testi.Info(Versione).NomeDelFile) + ".highlight.xml";
            if (File.Exists(nomeFileHighlight))
            {
                try
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(nomeFileHighlight);
                    XmlNode nodoPrincipale = xd.SelectSingleNode("highlights");
                    XmlNodeList nodi = nodoPrincipale.SelectNodes("highlight");
                    foreach (XmlNode sottoNodo in nodi)
                    {
                        Highlight highlight = new Highlight();
                        try
                        {
                            if (tipoTesto == TestoTipi.Dizionario)
                            {
                                highlight.voce = sottoNodo.SelectSingleNode("item").InnerText;
                            }
                            else
                            {
                                highlight.libro = Convert.ToInt32(sottoNodo.SelectSingleNode("book").InnerText, CultureInfo.InvariantCulture);
                                highlight.capitolo = Convert.ToInt32(sottoNodo.SelectSingleNode("chapter").InnerText, CultureInfo.InvariantCulture);
                                highlight.versetto = Convert.ToInt32(sottoNodo.SelectSingleNode("verse").InnerText, CultureInfo.InvariantCulture);
                                //                                    highlight.fine = Convert.ToInt32(sottoNodo.SelectSingleNode("end").InnerText, CultureInfo.InvariantCulture);
                            }
                            highlight.inizio = Convert.ToInt32(sottoNodo.SelectSingleNode("start").InnerText, CultureInfo.InvariantCulture);
                            highlight.lunghezza = Convert.ToInt32(sottoNodo.SelectSingleNode("length").InnerText, CultureInfo.InvariantCulture);
                            highlight.tipo = (TipoHighlight)(Convert.ToInt32(sottoNodo.SelectSingleNode("type").InnerText, CultureInfo.InvariantCulture));
                            if (highlight.tipo == TipoHighlight.Sottolineatura)
                                highlight.tipoSottolineatura = Convert.ToByte(sottoNodo.SelectSingleNode("underline").InnerText, CultureInfo.InvariantCulture);
                            else
                                highlight.colore = Color.FromName(sottoNodo.SelectSingleNode("colour").InnerText);
                            highlightAttuale.Add(highlight);
                        }
                        catch
                        {
                            // errore in una delle voci; salta
                        }
                    }
                }
                catch
                {
                    // altro errore nel file; salta
                }
            }
        }

        internal void MettiHighlight(Highlight highlight, int offset)
        {
            SelectionStart = highlight.inizio + offset;
            SelectionLength = highlight.lunghezza;
            switch (highlight.tipo)
            {
                case TipoHighlight.Evidenziatore:
                    if (!Principale.isRunningOnMono)
                        ImpostaSfondoNotMono(highlight.colore);
                    break;
                case TipoHighlight.Colore:
                    SelectionColor = highlight.colore;
                    break;
                case TipoHighlight.Sottolineatura:
                    if (!Principale.isRunningOnMono)
                        SetSelectionUnderlineTypeNotMono(highlight.tipoSottolineatura);
                    break;
            }
        }

        private void ImpostaSfondoNotMono(Color colore)
        {
            SelectionBackColor = colore;
        }

        #region HighlightPulsanti

        internal void HighlighterClick(Color colore, TipoHighlight tipo)
        {
            bool modificato = Modified;
            switch (tipo)
            {
                case TipoHighlight.Nessuno:
                    break;
                case TipoHighlight.Evidenziatore:
                    SelectionBackColor = colore;
                    ImpostaHighlight(TipoHighlight.Evidenziatore, colore);
                    break;
                case TipoHighlight.Colore:
                    SelectionColor = colore;
                    ImpostaHighlight(TipoHighlight.Colore, colore);
                    break;
            }
            Modified = modificato;
        }

        internal void HighlighterClick(byte tipoSottolineatura)
        {
            bool modificato = Modified;
            SetSelectionUnderlineTypeNotMono(tipoSottolineatura);
            ImpostaHighlightSottolineatura(tipoSottolineatura);
            Modified = modificato;
        }

        internal void HighlighterNoneClick()
        {
            bool modificato = Modified;
            SelectionColor = ForeColor;
            if (!Principale.isRunningOnMono)
                SetSelectionUnderlineTypeNotMono(0);
            ImpostaHighlightNone();
            Modified = modificato;
        }

        internal void HighlighterNoneNotMonoClick()
        {
            bool modificato = Modified;
            SelectionBackColor = BackColor;
            Modified = modificato;
        }

        private void ImpostaHighlight(TipoHighlight tipoHighlight, Color colore)
        {
            Highlight nuovoHighlight = new Highlight
            {
                tipo = tipoHighlight,
                colore = colore
            };
            ImpostaHighlightComune(nuovoHighlight);
        }

        private void ImpostaHighlightSottolineatura(byte tipoSottolineatura)
        {
            Highlight nuovoHighlight = new Highlight
            {
                tipo = TipoHighlight.Sottolineatura,
                tipoSottolineatura = tipoSottolineatura
            };
            ImpostaHighlightComune(nuovoHighlight);
        }

        private void ImpostaHighlightNone()
        {
            if (tipoTesto == TestoTipi.Dizionario)
            {
                for (int i = 0; i < highlightAttuale.Count; ++i)
                {
                    if (highlightAttuale[i].voce == voce)
                        i = CancellaHighlight(i, 0);
                }
            }
            else
            {
                string versettoAttuale = VersettoAttuale(SelectionStart);
                int libroNumero = Convert.ToInt32(versettoAttuale.Substring(0, 2), CultureInfo.InvariantCulture);
                int capitoloNumero = Convert.ToInt32(versettoAttuale.Substring(2, 3), CultureInfo.InvariantCulture);
                int versettoNumero = Convert.ToInt32(versettoAttuale.Substring(5, 3), CultureInfo.InvariantCulture);
                string tagInizioCapitolo = RichTextBoxEx.InizioRiferimento + versettoAttuale.Substring(0, 5) + "001";

                for (int i = 0; i < highlightAttuale.Count; ++i)
                {
                    if (highlightAttuale[i].libro == libroNumero && highlightAttuale[i].capitolo == capitoloNumero)
                    {
                        if (tipoTesto == TestoTipi.Bibbia)
                        {
                            int posizioneInizioCapitolo = Text.IndexOf(tagInizioCapitolo, StringComparison.Ordinal);
                            if (posizioneInizioCapitolo >= 0)
                                i = CancellaHighlight(i, posizioneInizioCapitolo);
                        }
                        else
                        {
                            if (highlightAttuale[i].versetto == versettoNumero)
                                i = CancellaHighlight(i, 0);
                        }
                    }
                }
            }
            SalvaHighlight();
            OnHighlightChangedEvent(new HighlightChangedEventArgs(Versione));
        }

        private int CancellaHighlight(int numeroHighlight, int posizioneInizio)
        {
            if (SelectionStart - posizioneInizio <= highlightAttuale[numeroHighlight].inizio && SelectionStart - posizioneInizio + SelectionLength >= highlightAttuale[numeroHighlight].inizio)
            {
                int nuovoInizio = SelectionStart - posizioneInizio + SelectionLength;
                int nuovaLunghezza = highlightAttuale[numeroHighlight].inizio + highlightAttuale[numeroHighlight].lunghezza - nuovoInizio;
                if (nuovaLunghezza <= 0)
                {
                    highlightAttuale.RemoveAt(numeroHighlight);
                    --numeroHighlight;
                }
                else
                {
                    highlightAttuale[numeroHighlight].inizio = nuovoInizio;
                    highlightAttuale[numeroHighlight].lunghezza = nuovaLunghezza;
                }
            }
            else if (SelectionStart - posizioneInizio > highlightAttuale[numeroHighlight].inizio && SelectionStart - posizioneInizio < highlightAttuale[numeroHighlight].inizio + highlightAttuale[numeroHighlight].lunghezza)
            {
                int nessunoInizio = SelectionStart - posizioneInizio;
                if (nessunoInizio + SelectionLength >= highlightAttuale[numeroHighlight].inizio + highlightAttuale[numeroHighlight].lunghezza)
                {
                    highlightAttuale[numeroHighlight].lunghezza = nessunoInizio - highlightAttuale[numeroHighlight].inizio;
                }
                else
                { // il "nessuno" è in mezzo e divide l'highlight in due
                    // creare un nuovo highlight per la seconda parte
                    Highlight nuovoHighlight = new Highlight();
                    if (tipoTesto != TestoTipi.Dizionario)
                    {
                        nuovoHighlight.libro = highlightAttuale[numeroHighlight].libro;
                        nuovoHighlight.capitolo = highlightAttuale[numeroHighlight].capitolo;
                        nuovoHighlight.versetto = highlightAttuale[numeroHighlight].versetto;
                    }
                    else
                    {
                        nuovoHighlight.voce = highlightAttuale[numeroHighlight].voce;
                    }
                    nuovoHighlight.tipo = highlightAttuale[numeroHighlight].tipo;
                    nuovoHighlight.colore = highlightAttuale[numeroHighlight].colore;
                    nuovoHighlight.tipoSottolineatura = highlightAttuale[numeroHighlight].tipoSottolineatura;
                    nuovoHighlight.inizio = nessunoInizio + SelectionLength;
                    nuovoHighlight.lunghezza = highlightAttuale[numeroHighlight].inizio + highlightAttuale[numeroHighlight].lunghezza - nuovoHighlight.inizio;
                    highlightAttuale.Insert(numeroHighlight, nuovoHighlight);
                    // accorciare l'highlight esistente per diventare la prima parte
                    ++numeroHighlight;
                    highlightAttuale[numeroHighlight].lunghezza = nessunoInizio - highlightAttuale[numeroHighlight].inizio;
                }
            }
            return numeroHighlight;
        }

        private void ImpostaHighlightComune(Highlight nuovoHighlight)
        {
            //nuovoHighlight.inizio = -1;
            switch (tipoTesto)
            {
                case TestoTipi.Bibbia:
                    string riferimentoAttuale = VersettoAttuale(SelectionStart);
                    nuovoHighlight.libro = Convert.ToInt32(riferimentoAttuale.Substring(0, 2), CultureInfo.InvariantCulture);
                    nuovoHighlight.capitolo = Convert.ToInt32(riferimentoAttuale.Substring(2, 3), CultureInfo.InvariantCulture);
                    nuovoHighlight.versetto = 0; // Convert.ToInt32(riferimentoAttuale.Substring(5, 3), CultureInfo.InvariantCulture);
                    int posizioneCapitolo = Text.IndexOf(RichTextBoxEx.InizioRiferimento + riferimentoAttuale.Substring(0, 5) + "001", StringComparison.Ordinal);
                    if (posizioneCapitolo >= 0)
                    {
                        nuovoHighlight.inizio = SelectionStart - posizioneCapitolo; //  Math.Max(0, SelectionStart - funzioni.InizioTestoDaInizioRiferimento(Text, posizioneVersetto)); // se lo highlight include una parte del riferimento, lo mettiamo solo sul testo (perché il formato del riferimento potrebbe essere diverso altre volte)
                        // se l'inizio del capitolo non è stato trovato, l'evidenziatore non è salvato
                        /*string riferimentoFinale = VersettoAttuale(SelectionStart + SelectionLength);
                        posizioneVersetto = Text.IndexOf(RichTextBoxEx.InizioRiferimento + riferimentoFinale.Substring(0, 8), StringComparison.Ordinal);
                        if (posizioneVersetto >= 0)
                        {
                            nuovoHighlight.libro2 = Convert.ToInt32(riferimentoFinale.Substring(0, 2), CultureInfo.InvariantCulture);
                            nuovoHighlight.capitolo2 = Convert.ToInt32(riferimentoFinale.Substring(2, 3), CultureInfo.InvariantCulture);
                            nuovoHighlight.versetto2 = Convert.ToInt32(riferimentoFinale.Substring(5, 3), CultureInfo.InvariantCulture);
                            nuovoHighlight.fine = Math.Max(0, SelectionStart + SelectionLength - funzioni.InizioTestoDaInizioRiferimento(Text, posizioneVersetto));
                        }*/
                    }
                    break;
                case TestoTipi.Commentario:
                    nuovoHighlight.libro = libro;
                    nuovoHighlight.capitolo = capitolo;
                    nuovoHighlight.versetto = versetto;
                    nuovoHighlight.inizio = SelectionStart;
                    break;
                case TestoTipi.Dizionario:
                    nuovoHighlight.voce = voce;
                    nuovoHighlight.inizio = SelectionStart;
                    break;
            }

            //if (nuovoHighlight.inizio >= 0)
            //{
            nuovoHighlight.lunghezza = SelectionLength;
            highlightAttuale.Add(nuovoHighlight);
            SalvaHighlight();
            //    OnHighlightChangedEvent(new HighlightChangedEventArgs(Versione));
            //}
        }

        private void SalvaHighlight()
        {
            string nomeFileHighlight = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Principale.testi.Info(Versione).NomeDelFile) + ".highlight.xml";
            if (highlightAttuale.Count > 0)
            {
                string testoFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
                testoFile += Environment.NewLine + "<highlights>";
                foreach (Highlight highlight in highlightAttuale)
                {
                    testoFile += Environment.NewLine + "<highlight>";
                    if (tipoTesto == TestoTipi.Dizionario)
                    {
                        testoFile += Environment.NewLine + "<item>" + highlight.voce + "</item>";
                    }
                    else
                    {
                        testoFile += Environment.NewLine + "<book>" + highlight.libro.ToString(CultureInfo.InvariantCulture) + "</book>";
                        testoFile += Environment.NewLine + "<chapter>" + highlight.capitolo.ToString(CultureInfo.InvariantCulture) + "</chapter>";
                        testoFile += Environment.NewLine + "<verse>" + highlight.versetto.ToString(CultureInfo.InvariantCulture) + "</verse>";
                        //testoFile += Environment.NewLine + "<book2>" + highlight.libro2.ToString(CultureInfo.InvariantCulture) + "</book2>";
                        //testoFile += Environment.NewLine + "<chapter2>" + highlight.capitolo2.ToString(CultureInfo.InvariantCulture) + "</chapter2>";
                        //testoFile += Environment.NewLine + "<verse2>" + highlight.versetto2.ToString(CultureInfo.InvariantCulture) + "</verse2>";
                        //testoFile += Environment.NewLine + "<end>" + highlight.fine.ToString(CultureInfo.InvariantCulture) + "</end>";
                    }
                    testoFile += Environment.NewLine + "<start>" + highlight.inizio.ToString(CultureInfo.InvariantCulture) + "</start>";
                    testoFile += Environment.NewLine + "<length>" + highlight.lunghezza.ToString(CultureInfo.InvariantCulture) + "</length>";
                    testoFile += Environment.NewLine + "<type>" + ((int)(highlight.tipo)).ToString(CultureInfo.InvariantCulture) + "</type>";
                    if (highlight.tipo == TipoHighlight.Sottolineatura)
                        testoFile += Environment.NewLine + "<underline>" + highlight.tipoSottolineatura.ToString(CultureInfo.InvariantCulture) + "</underline>";
                    else
                        testoFile += Environment.NewLine + "<colour>" + highlight.colore.Name + "</colour>";
                    testoFile += Environment.NewLine + "</highlight>";
                }
                testoFile += Environment.NewLine + "</highlights>";
                File.WriteAllText(nomeFileHighlight, testoFile);
            }
            else // non c'è evidenziatore, quindi cancelliamo il file
            {
                File.Delete(nomeFileHighlight);
            }
        }

        #endregion

        #region Highlight changed event
        /// <summary>
        /// Gli argomenti dell'evento quando lo highlight è cambiato.
        /// </summary>
        [ComVisible(false)]
        public class HighlightChangedEventArgs : EventArgs
        {
            private string versione;
            /// <summary>
            /// La versione della Bibbia dello highlight.
            /// </summary>
            public string Versione
            {
                get { return versione; }
            }

            /// <summary>
            /// Il costruttore della classe.
            /// </summary>
            /// <param name="versioneDelloHighlight">La versione della Bibbia a cui lo highlight è stato messo.</param>
            public HighlightChangedEventArgs(string versioneDelloHighlight)
            {
                versione = versioneDelloHighlight;
            }
        }

        /// <summary>
        /// L'evento quando lo highlight è cambiato.
        /// </summary>
        public event EventHandler<HighlightChangedEventArgs> HighlightChangedEvent;

        /// <summary>
        /// L'evento quando lo highlight è cambiato.
        /// </summary>
        /// <param name="e">Gli argomenti dell'evento.</param>
        protected virtual void OnHighlightChangedEvent(HighlightChangedEventArgs e)
        {
            HighlightChangedEvent(this, e);
        }

        #endregion
    }
}
