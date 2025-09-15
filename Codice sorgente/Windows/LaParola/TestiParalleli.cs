using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TestiBiblici;

namespace LaParola
{
    public partial class TestiParalleli : Template
    {
        #region proprietà

        private Principale genitore;
        public List<string> testi = new List<string>();
        private string cartella = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
        public List<TestoTipi> tipiTestiSelezionati = new List<TestoTipi>();

        #endregion

        #region costruttori

        public TestiParalleli(Principale sender)
        {
            CostruttoreComune(sender, new List<string>());
        }

        public TestiParalleli(Principale sender, List<string> versioni)
        {
            CostruttoreComune(sender, versioni);
        }

        private void CostruttoreComune(Principale sender, List<string> versioni)
        {
            InitializeComponent();

            cartella += "TestiParalleli" + Path.DirectorySeparatorChar;

            lbBibbie.BeginUpdate();
            lbBibbie.Items.Clear();
            lbBibbie.Items.AddRange(new List<string>(Principale.testi.NomiVersioni(TestoTipi.Bibbia)).ToArray());
            lbBibbie.EndUpdate();
            lbCommentari.BeginUpdate();
            lbCommentari.Items.Clear();
            lbCommentari.Items.AddRange(new List<string>(Principale.testi.NomiVersioni(TestoTipi.Commentario)).ToArray());
            lbCommentari.EndUpdate();
            lbDizionari.BeginUpdate();
            lbDizionari.Items.Clear();
            lbDizionari.Items.AddRange(new List<string>(Principale.testi.NomiVersioni(TestoTipi.Dizionario)).ToArray());
            lbDizionari.EndUpdate();

            AggiungiTesti(versioni);

            ImpostaPulApriEnabled();

            genitore = sender;
            guidaFile.HelpNamespace = genitore.NomeFileGuida();
        }

        #endregion

        private void ImpostaPulApriEnabled()
        {
            string[] fileTrovati = Directory.GetFiles(cartella, "*.*");
            pulApri.Enabled = (fileTrovati.Length > 0);
        }

        #region eventi

        private void TestiParalleli_Resize(object sender, EventArgs e)
        {
            lbBibbie.Width = Width / 2 - 36 - lbBibbie.Left;
            lbBibbie.Height = (Height - lbBibbie.Top - 16 - 16 - 48) / 3;
            lbCommentari.Top = lbBibbie.Bottom + 16;
            lbCommentari.Width = lbBibbie.Width;
            lbCommentari.Height = lbBibbie.Height;
            lbDizionari.Top = lbCommentari.Bottom + 16;
            lbDizionari.Width = lbBibbie.Width;
            lbDizionari.Height = lbBibbie.Height;
            etiCommentari.Top = lbBibbie.Bottom;
            etiDizionari.Top = lbCommentari.Bottom;

            lbScelti.Left = Width / 2 + 18;
            lbScelti.Height = pulSalva.Top - 32;
            lbScelti.Width = lbBibbie.Width;
            etiScelti.Left = Width / 2 + 18 - 3;

            pulAggiungiBibbia.Left = lbBibbie.Right + 6;
            pulAggiungiCommentario.Left = pulAggiungiBibbia.Left;
            pulAggiungiDizionario.Left = pulAggiungiBibbia.Left;
            pulAggiungiBibbia.Top = lbBibbie.Top + (lbBibbie.Height - pulAggiungiBibbia.Height) / 2;
            pulAggiungiCommentario.Top = lbCommentari.Top + (lbCommentari.Height - pulAggiungiCommentario.Height) / 2;
            int pulADTop = lbDizionari.Top + (lbDizionari.Height - pulAggiungiDizionario.Height) / 2;
            if (pulADTop + pulAggiungiDizionario.Height > lbScelti.Bottom - 1)
                pulADTop = lbScelti.Bottom - 1 - pulAggiungiDizionario.Height;
            pulAggiungiDizionario.Top = pulADTop;

            pulRimuovi.Left = lbScelti.Left - 29;
            pulGiu.Left = pulRimuovi.Left;
            int pGT = lbScelti.Top + lbScelti.Height / 2 + 5;
            if (pGT + pulGiu.Height > pulAggiungiCommentario.Top - 5)
                pGT = pulAggiungiCommentario.Top - 5 - pulGiu.Height;
            pulGiu.Top = pGT;
            pulSu.Left = pulRimuovi.Left;
            pulSu.Top = pGT - 31;
        }

        private void lbBibbie_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            pulAggiungiBibbia.Enabled = (lbBibbie.SelectedIndex >= 0);
        }

        private void lbCommentari_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            pulAggiungiCommentario.Enabled = (lbCommentari.SelectedIndex >= 0);
        }

        private void lbDizionari_SelectedIndexChanged(object sender, EventArgs e)
        {
            pulAggiungiDizionario.Enabled = (lbDizionari.SelectedIndex >= 0);
        }

        private void lbScelti_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            pulRimuovi.Enabled = (lbScelti.SelectedIndex >= 0);
            pulSu.Enabled = (lbScelti.SelectedIndex >= 1);
            pulGiu.Enabled = (lbScelti.SelectedIndex >= 0 && lbScelti.SelectedIndex < lbScelti.Items.Count - 1);
            btnOK.Enabled = (lbScelti.Items.Count > 0);
            // pulSalva è sempre enabled, per poter salvare una lista vuota con un certo nome e così cancellare un file
        }

        private void pulAggiungiBibbia_Click(object sender, System.EventArgs e)
        {
            lbScelti.Items.Add(lbBibbie.SelectedItem.ToString());
            tipiTestiSelezionati.Add(TestoTipi.Bibbia);
            lbScelti_SelectedIndexChanged(sender, null);
        }

        private void pulAggiungiCommentario_Click(object sender, System.EventArgs e)
        {
            lbScelti.Items.Add(lbCommentari.SelectedItem.ToString());
            tipiTestiSelezionati.Add(TestoTipi.Commentario);
            lbScelti_SelectedIndexChanged(sender, null);
        }

        private void pulAggiungiDizionario_Click(object sender, EventArgs e)
        {
            lbScelti.Items.Add(lbDizionari.SelectedItem.ToString());
            tipiTestiSelezionati.Add(TestoTipi.Dizionario);
            lbScelti_SelectedIndexChanged(sender, null);
        }

        private void pulRimuovi_Click(object sender, System.EventArgs e)
        {
            lbScelti.BeginUpdate();
            int si = lbScelti.SelectedIndex;
            lbScelti.Items.RemoveAt(si);
            lbScelti.SelectedIndex = (si < lbScelti.Items.Count ? si : si - 1);
            lbScelti.EndUpdate();
            tipiTestiSelezionati.RemoveAt(si);
        }

        private void pulSu_Click(object sender, System.EventArgs e)
        {
            int si = lbScelti.SelectedIndex;
            if (si > 0)
            {
                lbScelti.BeginUpdate();
                lbScelti.Items.Insert(si - 1, lbScelti.SelectedItem);
                lbScelti.Items.RemoveAt(si + 1);
                lbScelti.SelectedIndex = si - 1;
                lbScelti.EndUpdate();
                tipiTestiSelezionati.Reverse(si - 1, 2);
            }
        }

        private void pulGiu_Click(object sender, System.EventArgs e)
        {
            int si = lbScelti.SelectedIndex;
            if (si < lbScelti.Items.Count - 1)
            {
                lbScelti.BeginUpdate();
                lbScelti.Items.Insert(si + 2, lbScelti.SelectedItem);
                lbScelti.Items.RemoveAt(si);
                lbScelti.SelectedIndex = si + 1;
                lbScelti.EndUpdate();
                tipiTestiSelezionati.Reverse(si, 2);
            }
        }

        private void pulSalva_Click(object sender, System.EventArgs e)
        {
            string nomeFile = "";
            if (String.IsNullOrEmpty(cartella))
                Directory.CreateDirectory(cartella);

            using (InputBox ibForm = new InputBox(Principale.LocRM.GetString("ParallelTextsSaveCaption"), Principale.LocRM.GetString("ParallelTextsSaveQuestion"), ""))
            {
                ibForm.ShowDialog();
                nomeFile = ibForm.Risposta;
            }

            if (!string.IsNullOrEmpty(nomeFile))
            {
                int numeroTesti = lbScelti.Items.Count;
                if (numeroTesti > 0)
                {
                    string[] testi = new string[numeroTesti + 1];
                    string numeroVersione = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    testi[0] = "#" + Funzioni.VersioneMinore2Cifre(numeroVersione.Remove(numeroVersione.LastIndexOf('.')));
                    for (int i = 0; i < numeroTesti; ++i)
                    {
                        testi[i + 1] = lbScelti.Items[i].ToString() + "#";
                        switch (tipiTestiSelezionati[i])
                        {
                            case TestoTipi.Bibbia:
                                testi[i + 1] += "b";
                                break;
                            case TestoTipi.Commentario:
                                testi[i + 1] += "c";
                                break;
                            case TestoTipi.Dizionario:
                                testi[i + 1] += "d";
                                break;
                        }
                    }
                    File.WriteAllLines(cartella + nomeFile, testi, System.Text.Encoding.UTF8);
                    pulApri.Enabled = true;
                    genitore.AggiungiTestiParalleliAlMenu(cartella + nomeFile);
                }
                else // cancellare il file
                {
                    try
                    {
                        File.Delete(cartella + nomeFile);
                        genitore.RimuoviTestiParalleliDalMenu(nomeFile);
                    }
                    catch { } // per esempio nomeFile ha caratteri non permessi per i nomi dei file
                    ImpostaPulApriEnabled();
                }
            }
        }

        private void pulApri_Click(object sender, System.EventArgs e)
        {
            pmApri.Show(pulApri, 0, pulApri.Height - 1);
        }

        private void pmApri_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pmApri.Items.Clear();
            string[] fileTrovati = Directory.GetFiles(cartella, "*.*");
            Array.Sort(fileTrovati);
            foreach (string fileTrovato in fileTrovati)
                pmApri.Items.Add(new ToolStripMenuItem(Path.GetFileNameWithoutExtension(fileTrovato), null, ApriFile));
            e.Cancel = (pmApri.Items.Count == 0);
        }

        private void ApriFile(object sender, EventArgs e)
        {
            AggiungiTesti(new List<string>(File.ReadAllLines(cartella + ((ToolStripMenuItem)sender).Text)));
        }

        private void AggiungiTesti(List<string> testi)
        {
            tipiTestiSelezionati.Clear();
            lbScelti.BeginUpdate();
            lbScelti.Items.Clear();
            string testoSenzaTipo;
            TestoTipi tipo;

            foreach (string testo in testi)
            { // se il testo non esiste, il tipo è Nessuno e non è aggiunto
                if (testo.IndexOf('#') > 0)
                {
                    testoSenzaTipo = testo.Substring(0, testo.IndexOf('#'));
                    switch (testo.Substring(testo.IndexOf('#') + 1))
                    {
                        case "b":
                            tipo = TestoTipi.Bibbia;
                            break;
                        case "c":
                            tipo = TestoTipi.Commentario;
                            break;
                        case "d":
                            tipo = TestoTipi.Dizionario;
                            break;
                        default:
                            tipo = TestoTipi.None;
                            break;
                    }
                }
                else
                {
                    testoSenzaTipo = testo;
                    tipo = TestoTipi.None;
                }

                TestoTipi tipoDelTesto = Principale.testi.TipoPrincipaleDiTesto(testoSenzaTipo);
                if (tipoDelTesto != TestoTipi.None)
                {
                    lbScelti.Items.Add(testoSenzaTipo);
                    tipiTestiSelezionati.Add(tipo == TestoTipi.None ? tipoDelTesto : tipo);
                }
            }

            lbScelti.EndUpdate();
            lbScelti_SelectedIndexChanged(null, null);
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            int numeroTesti = lbScelti.Items.Count;
            for (int i = 0; i < numeroTesti; ++i)
                testi.Add(lbScelti.Items[i].ToString());

            Close();
        }

        #endregion

    }
}
