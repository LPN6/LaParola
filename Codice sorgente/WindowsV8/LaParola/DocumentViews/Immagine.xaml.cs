using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace LaParola.DocumentViews
{
    /// <summary>
    /// Interaction logic for Immagine.xaml
    /// </summary>
    /// 

    public partial class Immagine : UserControl
    {
        private int altezza;
        private int larghezza;
        private string collezione = "";
        private readonly List<string> linkNome = [];
        private readonly List<Rect> linkZone = []; // Using WPF's native Rect

        private readonly string _percorso;
        public string NomeFile => _percorso;

        public event Action<string, string>? LinkClicked; // Passa: NomeLink, Collezione

        public Immagine(string percorso)
        {
            InitializeComponent();
            _percorso = percorso;

            // Trigger load once initialized
            Loaded += Immagine_Loaded;
        }

        private void Immagine_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_percorso) || !File.Exists(_percorso)) return;

            try
            {
                // Load Image via WPF BitmapImage
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_percorso, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                altezza = bitmap.PixelHeight;
                larghezza = bitmap.PixelWidth;

                // Bind to image element
                pbImmagine.Source = bitmap;

                // Force layout container to mirror the exact native pixel bounds of the image
                DisplayGrid.Width = larghezza;
                DisplayGrid.Height = altezza;
                LinkCanvas.Width = larghezza;
                LinkCanvas.Height = altezza;

                LoadXmlLinks();
            }
            catch (Exception)
            {
                // Handle image corruptions safely
            }
        }

        private void LoadXmlLinks()
        {
            string? directory = Path.GetDirectoryName(_percorso);
            if (directory == null || directory.Length == 0)
                return;
            string xmlPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(_percorso) + ".image_link");
            if (!File.Exists(xmlPath)) return;

            try
            {
                XmlDocument xd = new();
                xd.Load(xmlPath);
                XmlNode? nodePrincipale = xd.SelectSingleNode("image");
                XmlNode? subNode = nodePrincipale?.SelectSingleNode("collection");
                collezione = subNode == null ? "" : subNode.InnerText;

                subNode = nodePrincipale?.SelectSingleNode("links");
                if (subNode != null && !string.IsNullOrEmpty(collezione))
                {
                    XmlNodeList? nodeLink = subNode.SelectNodes("name");
                    if (nodeLink != null)
                    {
                        foreach (XmlNode nodaLink in nodeLink)
                        {
                            XmlAttributeCollection? attributes = nodaLink.Attributes;
                            int x1 = Convert.ToInt32(attributes?.GetNamedItem("x1")?.Value, CultureInfo.InvariantCulture);
                            int x2 = Convert.ToInt32(attributes?.GetNamedItem("x2")?.Value, CultureInfo.InvariantCulture);
                            int y1 = Convert.ToInt32(attributes?.GetNamedItem("y1")?.Value, CultureInfo.InvariantCulture);
                            int y2 = Convert.ToInt32(attributes?.GetNamedItem("y2")?.Value, CultureInfo.InvariantCulture);

                            linkNome.Add(nodaLink.InnerText);
                            linkZone.Add(new Rect(x1, y1, x2 - x1, y2 - y1));
                        }
                    }
                }
            }
            catch
            {
                // XML format error, skip link assignment
            }
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MostraGuida((string)(Application.Current.TryFindResource("ImmagineTitolo") ?? "Image"));
        }

        private void PbImmagine_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // WPF automatically adjusts coordinates mapping directly to original dimensions
                Point p = e.GetPosition(LinkCanvas);

                for (int i = 0; i < linkNome.Count; ++i)
                {
                    if (linkZone[i].Contains(p))
                    {
                        // Solleva l'evento e lascia che sia chi ospita il controllo a gestire la logica
                        LinkClicked?.Invoke(linkNome[i], collezione);
                        break;
                    }
                }
            }
        }

        private void PbImmagine_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(LinkCanvas);
            bool inLink = false;

            for (int i = 0; i < linkNome.Count; ++i)
            {
                if (linkZone[i].Contains(p))
                {
                    inLink = true;
                    txtStatus.Text = linkNome[i] + " (" + collezione + ")";
                    break;
                }
            }

            if (!inLink)
            {
                txtStatus.Text = string.Empty;
            }

            LinkCanvas.Cursor = inLink ? Cursors.Hand : Cursors.Arrow;
        }

        private void LinkCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            LinkCanvas.Cursor = Cursors.Arrow;
            txtStatus.Text = string.Empty;
        }

        private void PmZoom_Opening(object sender, RoutedEventArgs e)
        {
            int currentZoom = Convert.ToInt32(ImageScale.ScaleX * 100);
            foreach (MenuItem item in pmZoom.Items)
            {
                if (int.TryParse(item.Tag?.ToString(), out int itemZoom))
                {
                    item.IsChecked = (itemZoom == currentZoom);
                }
            }
        }

        private void PmZoomVoce_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item && int.TryParse(item.Tag?.ToString(), out int zoom))
            {
                double scale = zoom / 100.0;
                ImageScale.ScaleX = scale;
                ImageScale.ScaleY = scale;
            }
        }
    }
}
