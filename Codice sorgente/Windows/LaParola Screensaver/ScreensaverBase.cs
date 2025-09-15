using System;
using System.Drawing;
using System.Windows.Forms;
using LaParola_Screensaver.Properties;

namespace LaParola_Screensaver
{
    public partial class ScreensaverBase : Form
    {
        // per la prima impostazione quando si avvia lo screensaver
        protected bool finestraAttivata = false;
        // per verificare lo spostamento del mouse
        protected Point posizioneMouse;
        protected int posizioneVecchia = int.MaxValue;
        protected Random numeroCasuale = new Random();
        private Principale principale;

        public ScreensaverBase()
        {
            InitializeComponent();

            Cursor.Hide();

            rtTesto.BackColor = Settings.Default.SfondoColore;
            this.BackColor = rtTesto.BackColor;
            rtDummy.BackColor = rtTesto.BackColor;
        }

        internal void CaricaForm(Principale p)
        {
            // chiamato solo quando è a tutto schermo, non anteprima
            //this.TopMost = true;
            //this.WindowState = FormWindowState.Maximized;

            if (Settings.Default.DirezioneVerticale)
            {
                rtTesto.Width = 3 * Width / 5;
            }
            else
            {
                rtTesto.Multiline = false;
            }

            principale = p;
        }

        internal void CaricaForm(Principale p, Rectangle area)
        {
            this.WindowState = FormWindowState.Normal;
            this.Location = new Point(area.Left, area.Top);
            this.Size = new Size(area.Width, area.Height);
            CaricaForm(p);
        }

        internal void SpostaTesto(long numeroTick)
        {
            Application.DoEvents();
            if (Settings.Default.DirezioneVerticale)
            {
                int posizione = Height - 50 - Convert.ToInt32(Settings.Default.Velocita * numeroTick * Height / 1000);
                if (posizione != posizioneVecchia)
                {
                    rtTesto.Top = posizione;
                    posizioneVecchia = posizione;
                }
            }
            else // orizzontale
            {
                int posizione = Width - 100 - Convert.ToInt32(Settings.Default.Velocita * numeroTick * Width / 1000);
                if (posizione != posizioneVecchia)
                {
                    rtTesto.Left = posizione;
                    posizioneVecchia = posizione;
                }
            }

            this.Refresh();
            Application.DoEvents();
        }

        private void rtTesto_MouseMove(object sender, MouseEventArgs e)
        {
            Point nuovoPunto = ((Control)sender).PointToScreen(new Point(e.X, e.Y));
            if (!finestraAttivata)
            {
                // siccome può essere chiamato da rtTesto, rtDummy o dalla form, bisogna convertire alle coordinate dello schermo per controllare se il mouse è stato spostato
                posizioneMouse = nuovoPunto;
                finestraAttivata = true;
            }
            else
            {
                // quando andava in verticale, il mouse si muoveva giù quando il testo arriva in cima alla finestra
                // però non succede più, quando ho cambiato a "PointToScreen"
                //if (Math.Abs(nuovoPunto.X - posizioneMouse.X) > 5 || (Math.Abs(nuovoPunto.Y - posizioneMouse.Y) > 5 && !Settings.Default.DirezioneVerticale))
                if (Math.Abs(nuovoPunto.X - posizioneMouse.X) > 5 || Math.Abs(nuovoPunto.Y - posizioneMouse.Y) > 5)
                {
                    ChiudiProgramma();
                }
            }
        }

        private void rtTesto_MouseDown(object sender, MouseEventArgs e)
        {
            ChiudiProgramma();
        }

        private void rtTesto_KeyDown(object sender, KeyEventArgs e)
        {
            ChiudiProgramma();
        }

        protected void ChiudiProgramma()
        {
            if (principale != null) // in Preview, quando si chiude la finestra il form è già chiuso prima di arrivare qui
                principale.timer.Enabled = false;
            Application.Exit();
        }

        internal void SetTesto(string testoDaMostrare)
        {
            try
            {
                rtTesto.Rtf = testoDaMostrare;
            }
            catch
            {
                rtTesto.Text = testoDaMostrare;
            }

            if (Settings.Default.DirezioneVerticale)
            {
                rtTesto.Height = rtTesto.GetPositionFromCharIndex(rtTesto.TextLength).Y;
                int altezza = Settings.Default.Velocita == 0 ? (Height - rtTesto.Height) / 2 : Height - 50;
                if (altezza < 0)
                    altezza = 0;
                rtTesto.Location = new Point(Width / 5, altezza);
            }
            else // orizzontale
            {
                rtTesto.Width = Convert.ToInt32(Graphics.FromHwnd(Handle).MeasureString(rtTesto.Text.Trim(), new Font(Settings.Default.FontNome, Settings.Default.FontDimensione)).Width + 0.5);
                int differenzaAltezza = Height - rtTesto.Height;
                if (differenzaAltezza < 0)
                    differenzaAltezza = 0;
                int sinistra = (Settings.Default.Velocita == 0 ? (Width - rtTesto.Width) / 2 : Width - 100);
                if (sinistra < 0)
                    sinistra = 0;
                rtTesto.Location = new Point(sinistra, Settings.Default.PosizioneCasuale ? numeroCasuale.Next(0, differenzaAltezza) : differenzaAltezza / 2);
            }
        }
    }
}
