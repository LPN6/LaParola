using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TestiBiblici;

namespace LaParola
{
    public partial class GestisciTesti : Template
    {
        private Principale genitore;

        private enum TipoAzione
        {
            Nuova,
            Copia,
            Rinomina,
            Unisci,
            SolaLettura,
            Esporta,
            FileUnico,
            Cancella,
            AggiungiRadici,
            GeneraLista
        }

        public GestisciTesti(Principale formGenitore)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");

            InitializeComponent();
            genitore = formGenitore;
            cbAzioni.SelectedIndex = 0;
            guidaFile.HelpNamespace = genitore.NomeFileGuida();
        }

        private void cbAzioni_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbCollezioni.Items.Clear();
            cbCollezioni2.Items.Clear();
            //            string[] versioni = new List<string>(Principale.testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario | TestoTipi.Libro)).ToArray();
            TipoAzione indice = (TipoAzione)(cbAzioni.SelectedIndex);
            switch (indice)
            {
                case TipoAzione.Nuova:
                case TipoAzione.GeneraLista:
                    break;
                case TipoAzione.Esporta:
                case TipoAzione.FileUnico:
                case TipoAzione.Unisci:
                case TipoAzione.AggiungiRadici:
                    cbCollezioni.Items.AddRange(new List<string>(Principale.testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario | TestoTipi.Libro)).ToArray());
                    if (indice == TipoAzione.Unisci)
                        cbCollezioni2.Items.AddRange(new List<string>(Principale.testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario | TestoTipi.Libro)).ToArray());
                    break;
                case TipoAzione.SolaLettura:
                    foreach (string collezione in Principale.testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario | TestoTipi.Libro))
                    {
                        if (Principale.testi.Info(collezione).Bloccato != BloccatoTipi.BloccatoSempre)
                            cbCollezioni.Items.Add(collezione);
                    }
                    break;
                case TipoAzione.Copia:
                case TipoAzione.Rinomina:
                case TipoAzione.Cancella:
                    cbCollezioni.Items.AddRange(new List<string>(Principale.testi.NomiVersioni()).ToArray());
                    break;
            }
            if (cbCollezioni.Items.Count > 0)
                cbCollezioni.SelectedIndex = 0;
            if (cbCollezioni2.Items.Count > 1)
                cbCollezioni2.SelectedIndex = 1;
            else if (cbCollezioni2.Items.Count == 1)
                cbCollezioni2.SelectedIndex = 0;
            bool esistonoCollezioni = (cbCollezioni.Items.Count > 0);
            etiCancellaCollezioneNessuna.Visible = (indice != TipoAzione.Nuova && indice != TipoAzione.GeneraLista && !esistonoCollezioni);
            cbCollezioni.Visible = (indice != TipoAzione.Nuova && indice != TipoAzione.GeneraLista && esistonoCollezioni);
            etiCancellaCollezione.Visible = cbCollezioni.Visible;
            cbCollezioni2.Visible = (indice == TipoAzione.Unisci && esistonoCollezioni);
            etiCollezione2.Visible = cbCollezioni2.Visible;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string nomeVersione = "";
            if (cbCollezioni.SelectedIndex >= 0)
                nomeVersione = cbCollezioni.Items[cbCollezioni.SelectedIndex].ToString();
            TipoAzione indice = (TipoAzione)(cbAzioni.SelectedIndex);
            switch (indice)
            {
                case TipoAzione.Nuova: // nuova collezione
                    using (ImportaBibbia ib = new ImportaBibbia(genitore, TipoImportazione.NuovaNote))
                    {
                        ib.ShowDialog();
                    }
                    break;
                case TipoAzione.Copia: // copia collezione
                    if (!String.IsNullOrEmpty(nomeVersione))
                    {
                        try
                        {
                            string vecchioNome = Principale.testi.Info(nomeVersione).NomeDelFile;
                            int count = 1;
                            string nuovoNomeTesto = nomeVersione + count.ToString(CultureInfo.InvariantCulture);
                            while (Principale.testi.VersioneEsiste(nuovoNomeTesto))
                            {
                                ++count;
                                nuovoNomeTesto = nomeVersione + count.ToString(CultureInfo.InvariantCulture);
                            }
                            string nuovoNome = Principale.testi.CopiaTesto(nomeVersione, nuovoNomeTesto, Path.GetDirectoryName(vecchioNome) + Path.DirectorySeparatorChar + String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesCopyOf"), Path.GetFileName(vecchioNome)));
                            genitore.SetBarraDiStatoTesto(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesCopied"), nomeVersione, nuovoNome));
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesErrorNotCopied"), exc.Message, nomeVersione), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                        }
                    }
                    break;
                case TipoAzione.Rinomina: // rinomina un testo
                    if (!String.IsNullOrEmpty(nomeVersione))
                    {
                        string nuovoNome = "";
                        using (InputBox ibForm = new InputBox(Principale.LocRM.GetString("ManageNotesRenameCaption"), Principale.LocRM.GetString("ManageNotesRenameQuestion"), ""))
                        {
                            ibForm.ShowDialog();
                            nuovoNome = ibForm.Risposta;
                        }
                        if (!String.IsNullOrEmpty(nuovoNome))
                        {
                            try
                            {
                                Principale.testi.RinominaTesto(nomeVersione, nuovoNome);
                                genitore.SetBarraDiStatoTesto(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesRenamed"), nomeVersione, nuovoNome));
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesErrorNotRenamed"), exc.Message, nomeVersione), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                            }
                        }
                    }
                    break;
                case TipoAzione.Unisci: // unire due collezioni
                    string nomeVersioneUnita = "";
                    using (ImportaBibbia ib = new ImportaBibbia(genitore, TipoImportazione.NuovaNote, false))
                    {
                        if (ib.ShowDialog() == DialogResult.OK)
                        {
                            nomeVersioneUnita = ib.NomeVersione;
                        }
                    }

                    // è necessario chiudere la finestra per permettere la creazione della nuova collezione
                    // in un thread, che non viene eseguito mentre la finestra di dialogo è aperto
                    if (!string.IsNullOrEmpty(nomeVersioneUnita))
                    {
                        Cursor cursoreAttuale = Cursor.Current;
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            Collection<string> note1 = new Collection<string>(Principale.testi.Note(nomeVersione));
                            string nomeVersione2 = "";
                            if (cbCollezioni2.SelectedIndex >= 0)
                                nomeVersione2 = cbCollezioni2.Items[cbCollezioni2.SelectedIndex].ToString();
                            Collection<string> note2 = Principale.testi.Note(nomeVersione2);
                            List<string> noteOrdinate1 = new List<string>(Principale.testi.GetNoteInOrdine(nomeVersione));
                            Collection<string> noteOrdinate2 = Principale.testi.GetNoteInOrdine(nomeVersione2);
                            RichTextBoxEx rtb = new RichTextBoxEx();

                            foreach (string nota in note1)
                            {
                                if (note2.IndexOf(nota) < 0)
                                    Principale.testi.SetNotaTesto(Principale.testi.GetNotaTesto(nota, nomeVersione), nota, nomeVersioneUnita);
                                else
                                {
                                    if (nota.StartsWith("#", StringComparison.Ordinal))
                                    { // unire due note su un versetto in un'unica nota
                                        rtb.Rtf = Principale.testi.GetNotaTesto(nota, nomeVersione);
                                        rtb.SelectionStart = rtb.Text.Length;
                                        if (rtb.Text.EndsWith("\n", StringComparison.Ordinal))
                                            rtb.SelectedText = "\n";
                                        else
                                            rtb.SelectedText = "\n\n";
                                        rtb.AggiungiRtf(Principale.testi.GetNotaTesto(nota, nomeVersione2));
                                        Principale.testi.SetNotaTesto(rtb.Rtf, nota, nomeVersioneUnita);
                                        note2.Remove(nota);
                                    }
                                    else
                                    { // cambiare il nome di una nota su un tema se è uguale nella seconda collezione
                                        string nuovoNomeNota = nota;
                                        int numeroDaAggiungere = 0;
                                        do
                                        {
                                            ++numeroDaAggiungere;
                                            nuovoNomeNota = nota + " (" + numeroDaAggiungere.ToString(CultureInfo.InvariantCulture) + ")";
                                        } while (note1.IndexOf(nuovoNomeNota) >= 0);
                                        Principale.testi.SetNotaTesto(Principale.testi.GetNotaTesto(nota, nomeVersione), nota, nomeVersioneUnita);
                                        Principale.testi.SetNotaTesto(Principale.testi.GetNotaTesto(nota, nomeVersione2), nuovoNomeNota, nomeVersioneUnita);
                                        note2.Remove(nota);
                                        if (noteOrdinate2.IndexOf(nota) >= 0)
                                            noteOrdinate2[noteOrdinate2.IndexOf(nota)] = nuovoNomeNota;
                                    }
                                }
                            }
                            foreach (string nota in note2)
                            {
                                Principale.testi.SetNotaTesto(Principale.testi.GetNotaTesto(nota, nomeVersione2), nota, nomeVersioneUnita);
                            }

                            if (noteOrdinate1.Count > 0 && string.IsNullOrEmpty(noteOrdinate1[0]) && noteOrdinate2.Count > 0 && !string.IsNullOrEmpty(noteOrdinate2[0]))
                                noteOrdinate1[0] = noteOrdinate2[0];
                            if (noteOrdinate1.Count == 0 && noteOrdinate2.Count > 0)
                                noteOrdinate1.Add(noteOrdinate2[0]);
                            if (noteOrdinate2.Count > 0)
                                noteOrdinate2.RemoveAt(0);
                            noteOrdinate1.AddRange(noteOrdinate2);
                            if (noteOrdinate1.Count > 0)
                                Principale.testi.SetNoteInOrdine(new Collection<string>(noteOrdinate1), nomeVersioneUnita);
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesErrorNotJoined"), exc.Message, nomeVersioneUnita), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                        }
                        finally
                        {
                            Cursor.Current = cursoreAttuale;
                            if (cursoreAttuale != null)
                                cursoreAttuale.Dispose();
                        }
                    }
                    break;
                case TipoAzione.SolaLettura: // modificare lo stato di sola lettura
                    Principale.testi.CambiaSolaLettura(nomeVersione);
                    break;
                case TipoAzione.Esporta: // esporta una collezione
                    Funzioni.EsportaTesto(genitore, EsportoTestoTipo.CollezioneFile, nomeVersione);
                    break;
                case TipoAzione.FileUnico: // crea un unico file da una collezione
                    genitore.MostraBranoInEditor(Principale.testi.NotePrimaOrdinate(nomeVersione, true), nomeVersione);
                    break;
                case TipoAzione.Cancella: // cancella collezione
                    if (!String.IsNullOrEmpty(nomeVersione))
                    {
                        try
                        {
                            Principale.testi.CancellaTesto(nomeVersione);
                            genitore.SetBarraDiStatoTesto(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesDeleted"), nomeVersione));
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesErrorNotDeleted"), exc.Message, nomeVersione), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                        }
                    }
                    break;
                case TipoAzione.AggiungiRadici: // aggiungere le radici alle parole della collezione
                    if (!String.IsNullOrEmpty(nomeVersione))
                    {
                        Cursor cursoreAttuale = Cursor.Current;
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            List<string> listaRadici = new List<string>(8192);
                            string[] radiceDiParola = Funzioni.AggiungiRadiciDaFile(Application.StartupPath, Principale.testi.Info(nomeVersione).Lingua, Principale.testi.Parole(nomeVersione), listaRadici);
                            Principale.testi.AggiungiRadiciAllaVersione(listaRadici.ToArray(), radiceDiParola, nomeVersione);
                        }
                        finally
                        {
                            Cursor.Current = cursoreAttuale;
                            if (cursoreAttuale != null)
                                cursoreAttuale.Dispose();
                        }
                    }
                    break;
                case TipoAzione.GeneraLista:
                    using (ListaTesti fListaTesti = new ListaTesti(genitore))
                    {
                        fListaTesti.ShowDialog();
                    }
                    break;
            }

            if (!cbLasciaAperta.Checked)
                Close();
            else
            {
                TipoAzione azione = (TipoAzione)(cbAzioni.SelectedIndex);
                if (azione == TipoAzione.Rinomina || azione == TipoAzione.Cancella)
                    cbAzioni_SelectedIndexChanged(sender, e);
            }
        }
    }
}