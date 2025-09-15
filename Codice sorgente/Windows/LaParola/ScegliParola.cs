using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace LaParola
{
    public partial class ScegliParola : Template
    {
        #region proprietà

        private Principale genitore;
        private int daSpostare = 0;

        private Font fontEbraico = null;
        private Font fontGreco = null;

        private string versione;
        public string Versione
        {
            get { return versione; }
        }

        #endregion

        public ScegliParola(Principale formGenitore, String versioneDaUsare)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");

            InitializeComponent();
            versione = versioneDaUsare;
            genitore = formGenitore;

            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            bool aggiustaLB = false;

            switch (Funzioni.LinguaPrincipale(Principale.testi.Info(versione).Lingua))
            {
                case "he":
                case "he-t":
                    try
                    {
                        fontEbraico = new Font(Principale.testi.Formato.FontEbraicoNome, Principale.testi.Formato.FontEbraicoDimensione * tbParole.Font.Size / Principale.testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                        //
                    }
                    lbParole.Font = fontEbraico;
                    tbParole.Font = fontEbraico;
                    lbRadici.Font = fontEbraico;
                    tbRadici.Font = fontEbraico;
                    lbParoleDiRadice.Font = fontEbraico;
                    tbParoleDiRadice.Font = fontEbraico;
                    aggiustaLB = true;
                    break;
                case "el":
                    try
                    {
                        fontGreco = new Font(Principale.testi.Formato.FontGrecoNome, Principale.testi.Formato.FontGrecoDimensione * tbParole.Font.Size / Principale.testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                        //
                    }
                    lbParole.Font = fontGreco;
                    tbParole.Font = fontGreco;
                    lbRadici.Font = fontGreco;
                    tbRadici.Font = fontGreco;
                    lbParoleDiRadice.Font = fontGreco;
                    tbParoleDiRadice.Font = fontGreco;
                    aggiustaLB = true;
                    break;
            }
            if (aggiustaLB)
            {
                daSpostare = tbParole.Top + tbParole.Height + 3 - lbParole.Top;
                int nuovoTop = lbParole.Top + daSpostare;
                int nuovoHeight = lbParole.Height - daSpostare;
                lbParole.Top = nuovoTop;
                lbRadici.Top = nuovoTop;
                lbParoleDiRadice.Top = nuovoTop;
                lbParole.Height = nuovoHeight;
                lbRadici.Height = nuovoHeight;
                lbParoleDiRadice.Height = nuovoHeight;
            }

            btnCanc.Text = Principale.LocRM.GetString("MiscClose");
            // per creare un elenco di tutte le parole usate nella versione
            //              System.IO.File.WriteAllLines(@"c:\Documents and Settings\Wilson\Desktop\latinwords.txt", Principale.testi.Parole(versione));
            // per creare un elenco di tutte le parole della radice
            //System.IO.File.WriteAllLines(@"d:\Documenti\Visual Studio 2008\Projects\LaParola\paroleradici\parole.txt", new List<string>(Principale.testi.ParoleDiRadice("*", versione)).ToArray());
            lbParole.Items.AddRange(Principale.testi.Parole(versione));
            lbRadici.Items.AddRange(Principale.testi.Radici(versione));
            if (lbRadici.Items.Count == 0)
            {
                labRadici.Visible = false;
                labRadiceDiParola.Visible = false;
                tbRadici.Visible = false;
                tbParoleDiRadice.Visible = false;
                lbRadici.Visible = false;
                lbParoleDiRadice.Visible = false;
                labNumeroVolteRadice.Visible = false;
                labParoleDiRadice.Visible = false;
                labNumeroParoleDiRadice.Visible = false;
                labNumeroVolteParolaDiRadice.Visible = false;
                btnRadice.Visible = false;
                btnParolaDiRadice.Visible = false;
            }
        }

        private void ScegliParola_Resize(object sender, EventArgs e)
        {
            int larghezzaColonna = (Width - 50) / 3;
            int posizioneColonna2 = larghezzaColonna + 14;
            int posizioneColonna3 = 2 * larghezzaColonna + 25;

            labRadici.Left = posizioneColonna2;
            labParoleDiRadice.Left = posizioneColonna3;

            tbRadici.Left = posizioneColonna2;
            tbParoleDiRadice.Left = posizioneColonna3;
            tbParole.Width = larghezzaColonna;
            tbRadici.Width = larghezzaColonna;
            tbParoleDiRadice.Width = larghezzaColonna;

            lbRadici.Left = posizioneColonna2;
            lbParoleDiRadice.Left = posizioneColonna3;
            Size grandezzaLB = new Size(larghezzaColonna, Height - 185 - daSpostare);
            lbParole.Size = grandezzaLB;
            lbRadici.Size = grandezzaLB;
            lbParoleDiRadice.Size = grandezzaLB;

            labNumeroVolteRadice.Left = posizioneColonna2;
            labNumeroParoleDiRadice.Left = posizioneColonna2;
            labNumeroVolteParolaDiRadice.Left = posizioneColonna3;

            btnRadice.Left = posizioneColonna2;
            btnParolaDiRadice.Left = posizioneColonna3;
            btnParola.Width = larghezzaColonna;
            btnRadice.Width = larghezzaColonna;
            btnParolaDiRadice.Width = larghezzaColonna;
        }

        private void ScegliParola_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fontEbraico != null)
            {
                fontEbraico.Dispose();
                fontEbraico = null;
            }
            if (fontGreco != null)
            {
                fontGreco.Dispose();
                fontGreco = null;
            }
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region elenchi

        private void txtParole_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbParole.Text))
                lbParole.SelectedIndex = -1;
            else
                lbParole.SelectedIndex = lbParole.FindString(tbParole.Text);
        }

        private void txtRadici_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbRadici.Text))
                lbRadici.SelectedIndex = -1;
            else
                lbRadici.SelectedIndex = lbRadici.FindString(tbRadici.Text);
        }

        private void txtParoleDiRadice_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbParoleDiRadice.Text))
                lbParoleDiRadice.SelectedIndex = -1;
            else
                lbParoleDiRadice.SelectedIndex = lbParoleDiRadice.FindString(tbParoleDiRadice.Text);
        }

        private void lbParole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbParole.SelectedIndex >= 0)
            {
                labNumeroVolteParola.Text = labNumeroVolteParola.Text.Substring(0, labNumeroVolteParola.Text.IndexOf(": ", StringComparison.Ordinal) + 2) + Principale.testi.NumeroVolteParola(lbParole.Items[lbParole.SelectedIndex].ToString(), versione).ToString(CultureInfo.InvariantCulture);
                labRadiceDiParola.Text = labRadiceDiParola.Text.Substring(0, labRadiceDiParola.Text.IndexOf(": ", StringComparison.Ordinal) + 2) + Principale.testi.RadiceDiParola(lbParole.Items[lbParole.SelectedIndex].ToString(), versione);
                btnParola.Enabled = true;
            }
            else
            {
                labNumeroVolteParola.Text = labNumeroVolteParola.Text.Substring(0, labNumeroVolteParola.Text.IndexOf(": ", StringComparison.Ordinal) + 2);
                labRadiceDiParola.Text = labRadiceDiParola.Text.Substring(0, labRadiceDiParola.Text.IndexOf(": ", StringComparison.Ordinal) + 2);
                btnParola.Enabled = false;
            }
        }

        private void lbRadici_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbParoleDiRadice.SelectedIndex = -1;
            lbParoleDiRadice.Items.Clear();
            if (lbRadici.SelectedIndex >= 0)
            {
                Cursor cursoreAttuale = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    lbParoleDiRadice.Items.AddRange(new List<string>(Principale.testi.ParoleDiRadice(lbRadici.Items[lbRadici.SelectedIndex].ToString(), versione)).ToArray());
                    if (lbParoleDiRadice.Items.Count > 0)
                        lbParoleDiRadice.SelectedIndex = 0;
                    labNumeroVolteRadice.Text = labNumeroVolteRadice.Text.Substring(0, labNumeroVolteRadice.Text.IndexOf(": ", StringComparison.Ordinal) + 2) + Principale.testi.NumeroVolteRadice(lbRadici.Items[lbRadici.SelectedIndex].ToString(), versione).ToString(CultureInfo.InvariantCulture);
                    labNumeroParoleDiRadice.Text = labNumeroParoleDiRadice.Text.Substring(0, labNumeroParoleDiRadice.Text.IndexOf(": ", StringComparison.Ordinal) + 2) + lbParoleDiRadice.Items.Count.ToString(CultureInfo.InvariantCulture);
                    btnRadice.Enabled = true;
                }
                finally
                {
                    Cursor.Current = cursoreAttuale;
                    if (cursoreAttuale != null)
                        cursoreAttuale.Dispose();
                }
            }
            else
            {
                labNumeroVolteRadice.Text = labNumeroVolteRadice.Text.Substring(0, labNumeroVolteRadice.Text.IndexOf(": ", StringComparison.Ordinal) + 2);
                labNumeroParoleDiRadice.Text = labNumeroParoleDiRadice.Text.Substring(0, labNumeroParoleDiRadice.Text.IndexOf(": ", StringComparison.Ordinal) + 2);
                btnRadice.Enabled = false;
            }
        }

        private void lbParoleRadice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbParoleDiRadice.SelectedIndex >= 0)
            {
                labNumeroVolteParolaDiRadice.Text = labNumeroVolteParolaDiRadice.Text.Substring(0, labNumeroVolteParolaDiRadice.Text.IndexOf(": ", StringComparison.Ordinal) + 2) + Principale.testi.NumeroVolteParola(lbParoleDiRadice.Items[lbParoleDiRadice.SelectedIndex].ToString(), versione).ToString(CultureInfo.InvariantCulture);
                btnParolaDiRadice.Enabled = true;
            }
            else
            {
                labNumeroVolteParolaDiRadice.Text = labNumeroVolteParolaDiRadice.Text.Substring(0, labNumeroVolteParolaDiRadice.Text.IndexOf(": ", StringComparison.Ordinal) + 2);
                btnParolaDiRadice.Enabled = false;
            }
        }

        #endregion

        #region pulsanti

        private void btnParola_Click(object sender, EventArgs e)
        {
            if (lbParole.SelectedIndex >= 0)
                AggiungiParola(lbParole.Items[lbParole.SelectedIndex].ToString());
        }

        private void btnRadice_Click(object sender, EventArgs e)
        {
            if (lbRadici.SelectedIndex >= 0)
                AggiungiParola(lbRadici.Items[lbRadici.SelectedIndex].ToString());
        }

        private void btnParolaDiRadice_Click(object sender, EventArgs e)
        {
            if (lbParoleDiRadice.SelectedIndex >= 0)
                AggiungiParola(lbParoleDiRadice.Items[lbParoleDiRadice.SelectedIndex].ToString());
        }

        #endregion

        private void AggiungiParola(string parola)
        {
            if (String.IsNullOrEmpty(parola) || parola[0] == '*')
                MessageBox.Show(Principale.LocRM.GetString("ChooseWordErrorWordNotChosen"), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            else
            {
                if (parola.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) >= 0 && parola.IndexOfAny(new char[] { '<', '>' }) == 0)
                    parola = "<" + parola + ">";
                bool trovato = false;
                foreach (Form f in MdiParent.MdiChildren)
                {
                    if (f.Tag != null && f.Tag.ToString() == "Ricerca")
                    {
                        Ricerca formRicerca = (Ricerca)f;
                        trovato = true;

                        String fraseDaRicercare = formRicerca.EspressioneDaRicercare;
                        if (String.IsNullOrEmpty(fraseDaRicercare))
                            fraseDaRicercare = parola;
                        else
                        {
                            char s = fraseDaRicercare[fraseDaRicercare.Length - 1];
                            if (s != ' ' && s != '(' && s != '[' && s != '/' && s != '\\' && s != '~' && s != '^')
                                fraseDaRicercare += " ";
                            fraseDaRicercare += parola;
                        }
                        formRicerca.EspressioneDaRicercare = fraseDaRicercare;
                        formRicerca.SetEspressioneInizioSelezione(fraseDaRicercare.Length);
                        break;
                    }
                } // foreach (Form formFiglio in Principale.MdiChildren)
                if (!trovato)
                {
                    Ricerca formRicerca = new Ricerca(genitore)
                    {
                        MdiParent = this.MdiParent,
                        EspressioneDaRicercare = parola
                    };
                    formRicerca.Show();
                }
            }
        }

    }
}