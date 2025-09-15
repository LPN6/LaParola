using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using LaParola_Screensaver.Properties;
using TestiBiblici;
using System.Threading;

namespace LaParola_Screensaver
{
    public partial class Opzioni : Form
    {
        Font font;
        Color fontColore;

        public Opzioni()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Settings.Default.InterfacciaLingua))
            {
                try
                {
                    // Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
                }
                // se Settings vuoto o non riconosciuto, non facciamo niente e il valore predefinito è usato
                catch (ArgumentNullException) { }
                catch (ArgumentException) { }
            }
            Settings.Default.InterfacciaLingua = Thread.CurrentThread.CurrentUICulture.Name;
            if (Settings.Default.InterfacciaLingua.Length >= 2 && Settings.Default.InterfacciaLingua.ToUpperInvariant().Substring(0, 2) == "IT")
                cbLingua.SelectedIndex = 1;
            else
                cbLingua.SelectedIndex = 0;

            pulSfondoColore.BackColor = Settings.Default.SfondoColore;
            ImpostaPulsanteSfondoColore();

            FontStyle fs = FontStyle.Regular;
            /*            if (Settings.Default.FontGrassetto)
                            fs |= FontStyle.Bold;
                        if (Settings.Default.FontCorsivo)
                            fs |= FontStyle.Italic;
                        if (Settings.Default.FontSottolineato)
                            fs |= FontStyle.Underline;
             */
            try
            {
                font = new Font(Settings.Default.FontNome, Settings.Default.FontDimensione, fs);
            }
            catch (ArgumentException)
            { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                font = new Font(Settings.Default.FontNome, Settings.Default.FontDimensione);
            }
            fontColore = Settings.Default.FontColore;

            if (font != null)
                SetEtichettaFont(etiFontEsempio, font, fontColore);
            else
                etiFontEsempio.Text = Settings.Default.FontNome + " " + Settings.Default.FontDimensione.ToString(CultureInfo.InvariantCulture);

            cbDirezione.SelectedIndex = (Settings.Default.DirezioneVerticale ? 1 : 0);
            cbPosizione.SelectedIndex = (Settings.Default.PosizioneCasuale ? 0 : 1);

            // settings misura schermi / secondo, e di solito va da 0 (fisso) a 0,4 (2,5 secondi per schermo)
            // trackbar va da 0 a 100
            int velocita = Convert.ToInt32(Settings.Default.Velocita * 250);
            if (velocita < tbVelocita.Minimum)
                velocita = tbVelocita.Minimum;
            if (velocita > tbVelocita.Maximum)
                velocita = tbVelocita.Maximum;
            tbVelocita.Value = velocita;

            tbBrano.Text = Settings.Default.Brano;
            cbRaggruppa.SelectedIndex = (Settings.Default.PerCapitoli ? 1 : 0);
            cbOrdine.SelectedIndex = (Settings.Default.OrdineBiblico ? 0 : 1);

            string cartellaDati = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
            Texts testi = new Texts(cartellaDati);
            string versioneDaCercare = Settings.Default.Versione;
            if (string.IsNullOrEmpty(versioneDaCercare))
                versioneDaCercare = testi.UltimaBibbia;
            cbVersione.BeginUpdate();
            foreach (string bibbia in testi.NomiVersioni(TestoTipi.Bibbia))
            {
                cbVersione.Items.Add(bibbia);
                if (bibbia == versioneDaCercare)
                    cbVersione.SelectedIndex = cbVersione.Items.Count - 1;
            }
            if (cbVersione.SelectedIndex < 0 && cbVersione.Items.Count > 0)
                cbVersione.SelectedIndex = 0;
            cbVersione.EndUpdate();
        }

        private void Opzioni_Load(object sender, EventArgs e)
        {
            cbDirezione_SelectedIndexChanged(null, null);
        }

        private void Opzioni_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (font != null)
                font.Dispose();
        }

        private void ImpostaPulsanteSfondoColore()
        {
            pulSfondoColore.ForeColor = pulSfondoColore.BackColor.GetBrightness() > 0.6 ? Color.Black : Color.White;
        }

        private static void SetEtichettaFont(Label etichetta, Font font, Color fontColore)
        {
            etichetta.Text = font.Name + " " + font.SizeInPoints.ToString(CultureInfo.InvariantCulture);
            etichetta.Font = font;
            etichetta.ForeColor = fontColore;
            if (etichetta.ForeColor == etichetta.BackColor)
                etichetta.BackColor = Color.White;
            if (etichetta.ForeColor == etichetta.BackColor)
                etichetta.BackColor = Color.Black;
        }

        private void pulSfondoColore_Click(object sender, EventArgs e)
        {
            colorDialog.Color = pulSfondoColore.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pulSfondoColore.BackColor = colorDialog.Color;
                ImpostaPulsanteSfondoColore();
            }
        }

        private void pulFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog
            {
                AllowScriptChange = false,
                AllowVerticalFonts = false,
                //fd.ShowEffects = false;
                ShowColor = true
            };
            if (font != null)
                fd.Font = font;
            fd.Color = fontColore;
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                font = fd.Font;
                fontColore = fd.Color;
                SetEtichettaFont(etiFontEsempio, font, fontColore);
            }
        }

        private void cbDirezione_SelectedIndexChanged(object sender, EventArgs e)
        {
            etiPosizione.Visible = (cbDirezione.SelectedIndex == 0);
            cbPosizione.Visible = etiPosizione.Visible;
        }

        private void cbLingua_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lingua = cbLingua.SelectedItem.ToString();
            if (lingua.Length >= 2 && lingua.ToUpperInvariant().Substring(0, 2) == "IT")
            {
                etiSfondoColore.Text = "Colore dello &sfondo:";
                etiFont.Text = "&Font:";
                pulFont.Text = "&Cambia";
                etiBrano.Text = "Sezione della &Bibbia: (lascia vuoto per tutta la Bibbia)";
                etiRaggruppa.Text = "&Raggruppa per:";
                cbRaggruppa.Items[0] = "Versetti";
                cbRaggruppa.Items[1] = "Capitoli";
                etiOrdine.Text = "&Ordine:";
                cbOrdine.Items[0] = "Ordine biblico";
                cbOrdine.Items[1] = "Casuale";
                etiDirezione.Text = "&Direzione:";
                cbDirezione.Items[0] = "Orizzontale";
                cbDirezione.Items[1] = "Verticale";
                etiPosizione.Text = "&Posizione:";
                cbPosizione.Items[0] = "Casuale";
                cbPosizione.Items[1] = "Centro";
                etiVelocita.Text = "&Velocità:";
                etiVelocita1.Text = "  Fermo Lento";
                etiVelocita2.Text = "Veloce";
                etiVersione.Text = "V&ersione:";
                etiLingua.Text = "&Lingua:";
                pulAnnulla.Text = "Annulla";
            }
            else
            {
                etiSfondoColore.Text = "Bac&kground colour:";
                etiFont.Text = "&Font:";
                pulFont.Text = "&Change";
                etiBrano.Text = "Part of the &Bible: (leave empty for all of the Bible)";
                etiRaggruppa.Text = "&Group by:";
                cbRaggruppa.Items[0] = "Verses";
                cbRaggruppa.Items[1] = "Chapters";
                etiOrdine.Text = "&Order:";
                cbOrdine.Items[0] = "Biblical order";
                cbOrdine.Items[1] = "Random";
                etiDirezione.Text = "&Direction:";
                cbDirezione.Items[0] = "Horizontal";
                cbDirezione.Items[1] = "Vertical";
                etiPosizione.Text = "&Position:";
                cbPosizione.Items[0] = "Random";
                cbPosizione.Items[1] = "Center";
                etiVelocita.Text = "&Speed:";
                etiVelocita1.Text = "Stopped Slow";
                etiVelocita2.Text = "Fast";
                etiVersione.Text = "&Translation:";
                etiLingua.Text = "&Language:";
                pulAnnulla.Text = "Cancel";
            }
        }

        private void pulOK_Click(object sender, EventArgs e)
        {
            Settings.Default.SfondoColore = pulSfondoColore.BackColor;

            Settings.Default.FontNome = font.Name;
            Settings.Default.FontDimensione = font.Size;
            Settings.Default.FontGrassetto = font.Bold;
            Settings.Default.FontCorsivo = font.Italic;
            Settings.Default.FontSottolineato = font.Underline;
            Settings.Default.FontColore = fontColore;

            Settings.Default.DirezioneVerticale = (cbDirezione.SelectedIndex == 1);
            Settings.Default.PosizioneCasuale = (cbPosizione.SelectedIndex == 0);

            Settings.Default.Velocita = tbVelocita.Value / 250.0;

            Settings.Default.Brano = tbBrano.Text;
            Settings.Default.PerCapitoli = (cbRaggruppa.SelectedIndex == 1);
            Settings.Default.OrdineBiblico = (cbOrdine.SelectedIndex == 0);

            Settings.Default.Versione = cbVersione.SelectedItem.ToString();

            Settings.Default.InterfacciaLingua = (cbLingua.SelectedIndex == 1 ? "it" : "en");

            Close();
        }

        private void pulAnnulla_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
