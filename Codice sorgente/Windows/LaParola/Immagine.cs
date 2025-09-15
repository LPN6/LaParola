using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace LaParola
{
    public partial class Immagine : Template
    {
        private int altezza;
        private int larghezza;
        private string collezione;
        private List<string> linkNome = new List<string>();
        private List<Rectangle> linkZone = new List<Rectangle>();

        private string nomeFile;
        public string NomeFile
        {
            get { return nomeFile; }
        }

        public Immagine(string file)
        {
            InitializeComponent();
            nomeFile = file;
        }

        private void Immagine_Load(object sender, EventArgs e)
        {
            Bitmap immagine = new Bitmap(nomeFile);
            altezza = immagine.Height;
            larghezza = immagine.Width;
            ClientSize = new Size(larghezza, altezza);

            pbImmagine.LoadAsync(@"file:///" + nomeFile);

            Text = Path.GetFileNameWithoutExtension(nomeFile);

            pbImmagine.SizeMode = PictureBoxSizeMode.Zoom;

            XmlNode nodePrincipale, subNode;
            int x1, x2, y1, y2;
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(Path.GetDirectoryName(nomeFile) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(nomeFile) + ".image_link");
                nodePrincipale = xd.SelectSingleNode("image");
                subNode = nodePrincipale.SelectSingleNode("collection");
                collezione = (subNode == null ? "" : subNode.InnerText);
                subNode = nodePrincipale.SelectSingleNode("links");
                if (subNode != null && !string.IsNullOrEmpty(collezione))
                {
                    XmlNodeList nodeLink = subNode.SelectNodes("name");
                    foreach (XmlNode nodaLink in nodeLink)
                    {
                        x1 = Convert.ToInt32(nodaLink.Attributes.GetNamedItem("x1").Value, CultureInfo.InvariantCulture);
                        x2 = Convert.ToInt32(nodaLink.Attributes.GetNamedItem("x2").Value, CultureInfo.InvariantCulture);
                        y1 = Convert.ToInt32(nodaLink.Attributes.GetNamedItem("y1").Value, CultureInfo.InvariantCulture);
                        y2 = Convert.ToInt32(nodaLink.Attributes.GetNamedItem("y2").Value, CultureInfo.InvariantCulture);
                        linkNome.Add(nodaLink.InnerText);
                        linkZone.Add(new Rectangle(x1, y1, x2 - x1, y2 - y1));
                    }
                }
            }
            catch
            {
                // errore nell'XML, saltiamo il file
            }
        }

        private void pbImmagine_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float zoom = Math.Max((float)larghezza / pbImmagine.Width, (float)altezza / pbImmagine.Height);
                int x = Convert.ToInt32((e.X - (pbImmagine.Width - (float)larghezza / zoom) / 2) * zoom);
                int y = Convert.ToInt32((e.Y - (pbImmagine.Height - (float)altezza / zoom) / 2) * zoom);
                for (int i = 0; i < linkNome.Count; ++i)
                {
                    if (linkZone[i].Contains(x, y))
                    {
                        if (Principale.testi.VersioneEsiste(collezione))
                            ((Principale)MdiParent).ApriNotaInEditor(linkNome[i], collezione);
                        else
                            MessageBox.Show(string.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImmaginiCollezioneNonTrovata"), collezione), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                    }
                }
            }
        }

        private void pbImmagine_MouseMove(object sender, MouseEventArgs e)
        {
            float zoom = Math.Max((float)larghezza / pbImmagine.Width, (float)altezza / pbImmagine.Height);
            int x = Convert.ToInt32((e.X - (pbImmagine.Width - (float)larghezza / zoom) / 2) * zoom);
            int y = Convert.ToInt32((e.Y - (pbImmagine.Height - (float)altezza / zoom) / 2) * zoom);
            bool inLink = false;
            for (int i = 0; i < linkNome.Count; ++i)
            {
                if (linkZone[i].Contains(x, y))
                {
                    inLink = true;
                    ((Principale)MdiParent).SetBarraDiStatoTesto(linkNome[i]);
                    break;
                }
            }
            pbImmagine.Cursor = (inLink ? Cursors.Hand : Cursors.Default);
        }

        private void pmZoom_Opening(object sender, CancelEventArgs e)
        {
            int zoom = Convert.ToInt32(Math.Min(pbImmagine.Width * 100.0 / larghezza, pbImmagine.Height * 100.0 / altezza));
            foreach (ToolStripItem voceMenu in pmZoom.Items)
                ((ToolStripMenuItem)voceMenu).Checked = (ComeNumero(voceMenu.Text) == zoom);
        }

        private void pmZoomVoce_Click(object sender, EventArgs e)
        {
            int zoom = ComeNumero(((ToolStripMenuItem)sender).Text);
            ClientSize = new Size(larghezza * zoom / 100, altezza * zoom / 100);
        }

        private static int ComeNumero(string s)
        {
            for (int i = s.Length - 1; i >= 0; --i)
                if (s[i] == '&' || s[i] == '%')
                    s = s.Remove(i, 1);
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }

    }
}