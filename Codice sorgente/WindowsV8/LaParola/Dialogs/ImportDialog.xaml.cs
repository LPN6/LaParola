using LaParola.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LaParola.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window
    {
        public ImportDialog(TipoImportazione tipo)
        {
            InitializeComponent();
            if (tipo == TipoImportazione.Crea) // non mostrare la prima riga
            {
                LabelFile.Visibility = Visibility.Collapsed;
                TxtFile.Visibility = Visibility.Collapsed;
                LabelCome.Visibility = Visibility.Collapsed;
                StackCome.Visibility = Visibility.Collapsed;
            }
            else if (tipo == TipoImportazione.ImportaRtf) // mostrare Cartella:
            {
                LabelFile.Content = (string)(Application.Current.TryFindResource("ImportaDialogoCartella") ?? "Directory:");
                TooltipFile.Text = (string)(Application.Current.TryFindResource("ImportaDialogoCartellaAiuto") ?? "The directory that contains the files with the text to be imported. To change the directory, close this dialog and start again.");
                LabelCome.Visibility = Visibility.Collapsed;
                StackCome.Visibility = Visibility.Collapsed;
            }
            // else (ImportaPDF) mostrare File:
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            ThemeManager.SetDarkTitleBar(this, ThemeManager.IsDark(MainWindow.settings.ThemeMode));
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MostraGuida((string)(Application.Current.TryFindResource("ImportaDialogoAiutoPagine") ?? "Bibliographical Information"));
        }

        public string File
        {
            get => TxtFile.Text;
            set => TxtFile.Text = value;
        }

        public string Abbreviazione
        {
            get => TxtAbbreviation.Text;
            set => TxtAbbreviation.Text = value;
        }

        public string Isbn
        {
            get => TxtIsbn.Text;
            set => TxtIsbn.Text = value;
        }

        public string Titolo
        {
            get => TxtTitle.Text;
            set => TxtTitle.Text = value;
        }

        public string Autore
        {
            get => TxtAuthor.Text;
            set => TxtAuthor.Text = value;
        }

        public string CasaEditrice
        {
            get => TxtPublisher.Text;
            set => TxtPublisher.Text = value;
        }

        public string Data
        {
            get => TxtDate.Text;
            set => TxtDate.Text = value;
        }

        public string Copyright
        {
            get => TxtCopyright.Text;
            set => TxtCopyright.Text = value;
        }

        public string Lingua
        {
            get => TxtLanguage.Text;
            set => TxtLanguage.Text = value;
        }

        public string VersioneDiNote
        {
            get => TxtBibleOfNotes.Text;
            set => TxtBibleOfNotes.Text = value;
        }

        public string Descrizione
        {
            get => TxtDescription.Text;
            set => TxtDescription.Text = value;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // Put any validation rules here (e.g., checking if Abbreviation is empty)
            if (string.IsNullOrEmpty(Titolo))
            {
                MessageBoxLPN.Show(Application.Current.MainWindow, (string)(Application.Current.TryFindResource("ImportaDialogoNoTitolo") ?? "You must type a title for the text."), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                return;
            }
            if (string.IsNullOrEmpty(Abbreviazione))
            {
                Abbreviazione = ImportaService.CreaAbbreviazione(Titolo);
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
