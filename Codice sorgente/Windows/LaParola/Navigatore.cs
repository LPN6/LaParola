using System;
using System.Drawing;
using System.Windows.Forms;
using TestiBiblici;
using System.Globalization;

namespace LaParola
{
    public partial class Navigatore : Form
    {
        #region proprietà

        private byte libroAttuale = 0, capitoloAttuale = 0, versettoAttuale = 0;
        private Color foreColor, backColor;
        private Principale genitore;
        public int Splitter1
        {
            get { return splitLibri.SplitterDistance; }
            set { splitLibri.SplitterDistance = value; }
        }
        public int Splitter2
        {
            get { return splitCapitoliVersetti.SplitterDistance; }
            set { splitCapitoliVersetti.SplitterDistance = value; }
        }

        #endregion

        public Navigatore(Principale formGenitore)
        {
            InitializeComponent();
            foreach (Control c in formGenitore.Controls)
            {
                if (c.GetType().Name == "MdiClient")
                {
                    Height = c.Height - 4;
                    Left = c.Width - Width - 4;
                    break;
                }
            }

            string ultimaBibbiaCompleta = Principale.testi.UltimaBibbiaCompleta;
            Control[] labLibri = new Control[73];
            int top = 0;
            for (byte i = 1; i <= 73; ++i)
            {
                Label labLibro = new Label
                {
                    Location = new Point(3, top),
                    Text = Principale.testi.GetLibroNome(i)
                };
                labLibro.Height = labLibro.PreferredHeight;
                labLibro.Width = labLibro.PreferredWidth;
                labLibro.Tag = i;
                if (Principale.testi.CapitoliInLibro(i, ultimaBibbiaCompleta) == 0)
                    labLibro.Visible = false;
                else
                {
                    labLibro.Click += new EventHandler(labLibro_Click);
                    labLibro.DoubleClick += new EventHandler(labLibro_DoubleClick);
                    top += labLibro.Height;
                }
                labLibri[i - 1] = labLibro;
            }
            splitLibri.Panel1.Controls.AddRange(labLibri);

            foreColor = labLibri[0].ForeColor;
            backColor = labLibri[0].BackColor;

            genitore = formGenitore;
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            labLibro_Click(splitLibri.Panel1.Controls[0], null);
        }

        void labLibro_Click(object sender, EventArgs e)
        {
            Control etichetta = (Control)sender;

            if (libroAttuale > 0)
            { // altrimenti è l'apertura della finestra
                splitLibri.Panel1.Controls[libroAttuale - 1].BackColor = backColor;
                splitLibri.Panel1.Controls[libroAttuale - 1].ForeColor = foreColor;
            }
            etichetta.BackColor = foreColor;
            etichetta.ForeColor = backColor;

            splitCapitoliVersetti.Panel1.Controls.Clear();
            for (int i = splitCapitoliVersetti.Panel1.Controls.Count - 1; i >= 0; --i)
                splitCapitoliVersetti.Panel1.Controls[i].Dispose();

            libroAttuale = Convert.ToByte(etichetta.Tag.ToString(), CultureInfo.InvariantCulture);
            int nCapitoli = Principale.testi.CapitoliInLibro(libroAttuale, Principale.testi.UltimaBibbia);
            if (nCapitoli==0)
                nCapitoli = Principale.testi.CapitoliInLibro(libroAttuale, Principale.testi.UltimaBibbiaCompleta);
            if (nCapitoli == 0)
                return;
            Control[] labCapitoli = new Control[nCapitoli];
            for (int i = 1; i <= nCapitoli; ++i)
            {
                Label labCapitolo = new Label();
                labCapitolo.Location = new Point(3, (i - 1) * labCapitolo.PreferredHeight);
                labCapitolo.Text = i.ToString();
                labCapitolo.Height = labCapitolo.PreferredHeight;
                labCapitolo.Width = labCapitolo.PreferredWidth;
                labCapitolo.Tag = i;
                labCapitolo.Click += new EventHandler(labCapitolo_Click);
                labCapitolo.DoubleClick += new EventHandler(labCapitolo_DoubleClick);
                labCapitoli[i - 1] = labCapitolo;
            }
            splitCapitoliVersetti.Panel1.Controls.AddRange(labCapitoli);

            capitoloAttuale = 0;
            labCapitolo_Click(splitCapitoliVersetti.Panel1.Controls[0], null);
        }

        void labLibro_DoubleClick(object sender, EventArgs e)
        {
            capitoloAttuale = 1;
            labCapitolo_Click(splitCapitoliVersetti.Panel1.Controls[0], null);
            labCapitolo_DoubleClick(null, null);
        }

        void labCapitolo_Click(object sender, EventArgs e)
        {
            Control etichetta = ((Control)sender);
            
            if (capitoloAttuale > 0)
            { // altrimenti è l'apertura della finestra
                splitCapitoliVersetti.Panel1.Controls[capitoloAttuale - 1].BackColor = backColor;
                splitCapitoliVersetti.Panel1.Controls[capitoloAttuale - 1].ForeColor = foreColor;
            }
            etichetta.BackColor = foreColor;
            etichetta.ForeColor = backColor;

            splitCapitoliVersetti.Panel2.Controls.Clear();
            for (int i = splitCapitoliVersetti.Panel2.Controls.Count - 1; i >= 0; --i)
                splitCapitoliVersetti.Panel2.Controls[i].Dispose();

            capitoloAttuale = Convert.ToByte(etichetta.Text, CultureInfo.InvariantCulture);
            int nVersetti = Principale.testi.VersettiInCapitolo(libroAttuale, Convert.ToByte(etichetta.Tag.ToString(), CultureInfo.InvariantCulture), Principale.testi.UltimaBibbia);
            if (nVersetti==0)
                nVersetti = Principale.testi.VersettiInCapitolo(libroAttuale, Convert.ToByte(etichetta.Tag.ToString(), CultureInfo.InvariantCulture), Principale.testi.UltimaBibbiaCompleta);
            if (nVersetti == 0)
                return;

            Control[] labVersetti = new Control[nVersetti];
            for (int i = 1; i <= nVersetti; ++i)
            {
                Label labVersetto = new Label();
                labVersetto.Location = new Point(3, (i - 1) * labVersetto.PreferredHeight);
                labVersetto.Text = i.ToString();
                labVersetto.Height = labVersetto.PreferredHeight;
                labVersetto.Width = labVersetto.PreferredWidth;
                labVersetto.Tag = i;
                labVersetto.Click += new EventHandler(labVersetto_Click);
                labVersetti[i - 1] = labVersetto;
            }
            splitCapitoliVersetti.Panel2.Controls.AddRange(labVersetti);

            versettoAttuale = 1;
            splitCapitoliVersetti.Panel2.Controls[0].BackColor = foreColor;
            splitCapitoliVersetti.Panel2.Controls[0].ForeColor = backColor;
        }

        void labCapitolo_DoubleClick(object sender, EventArgs e)
        {
            versettoAttuale = 1;
            labVersetto_Click(splitCapitoliVersetti.Panel2.Controls[0], null);
        }

        void labVersetto_Click(object sender, EventArgs e)
        {
            Control etichetta = ((Control)sender);
            
            if (versettoAttuale > 0)
            { // altrimenti è l'apertura della finestra
                splitCapitoliVersetti.Panel2.Controls[versettoAttuale - 1].BackColor = backColor;
                splitCapitoliVersetti.Panel2.Controls[versettoAttuale - 1].ForeColor = foreColor;
            }
            etichetta.BackColor = foreColor;
            etichetta.ForeColor = backColor;

            versettoAttuale = Convert.ToByte(etichetta.Text, CultureInfo.InvariantCulture);

            Visualizza formVisualizza = null;
            if (genitore.ultimaVisualizza != null && genitore.ultimaVisualizza.panes.Count > 0)
                // secondo caso è quando l'ultima finestra visualizzata è poi stata chiusa
                formVisualizza = genitore.ultimaVisualizza;
            else
                formVisualizza = genitore.VisualizzaTesto(Principale.testi.UltimaBibbia, TestoTipi.Bibbia);
            formVisualizza.SpostaTesto(Principale.testi.ConvertiDaStandard(new Riferimento(libroAttuale, capitoloAttuale, versettoAttuale), formVisualizza.paneAttivo.Versione), true);
        }
    }
}
