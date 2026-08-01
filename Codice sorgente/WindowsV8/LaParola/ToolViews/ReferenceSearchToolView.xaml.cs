using LaParola.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LaParola.ToolViews
{
    // TODO2 rimettere menu visibile in Menu visibile (anche separatore), sistema questa finestra e aggiungi raccogli info su tema
    public partial class ReferenceSearchToolView : UserControl
    {
        private static readonly Regex RegexTitoloNota = RegexNotaTitolo();

        private readonly string[] _nomiLibri = new string[74];
        //private readonly bool _caricato;
        private RisultatoRicerca[]? _tuttiRisultati;

        public class RisultatoRicerca
        {
            public string Nome { get; set; } = "";
            public string Autore { get; set; } = "";
            public TestoTipi Tipo { get; set; }
            public string Data { get; set; } = "";
            public string CasaEditrice { get; set; } = "";
            public string Copyright { get; set; } = "";
            public string Lingua { get; set; } = "";
            public string Abbreviazione { get; set; } = "";
            internal string Versione { get; set; } = "";
            internal string TitoloNota { get; set; } = "";
        }

        public bool IsTocVisible
        {
            get => TocBorder.Visibility == Visibility.Visible;
            set
            {
                TocBorder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                MainWindow.settings.ReferenceSearchTocVisible = value;
            }
        }

        public ReferenceSearchToolView()
        {
            try { InitializeComponent(); } catch { }
            //_caricato = true;
            try { CaricaAlberoLibri(); } catch { }
            try { TocBorder.Visibility = MainWindow.settings.ReferenceSearchTocVisible ? Visibility.Visible : Visibility.Collapsed; } catch { }
        }

        private void BtnToggleToc_Click(object sender, RoutedEventArgs e) => IsTocVisible = !IsTocVisible;

        private void CaricaAlberoLibri()
        {
            TocTreeView.Items.Clear();
            if (MainWindow.Testi == null) return;
            string? versioneBibbia = MainWindow.Testi.NomiVersioni(TestoTipi.Bibbia).FirstOrDefault();
            if (versioneBibbia == null) return;

            for (byte libro = 1; libro <= 73; libro++)
            {
                int totCap = MainWindow.Testi.CapitoliInLibro(libro, versioneBibbia);
                if (totCap == 0) continue;

                string nomeLibro = MainWindow.Testi.libriNomi[libro];
                if (string.IsNullOrEmpty(nomeLibro)) continue;

                var libroNode = new TreeViewItem { Header = nomeLibro, Tag = libro };

                for (byte cap = 1; cap <= totCap; cap++)
                {
                    int totVers = MainWindow.Testi.VersettiInCapitolo(libro, cap, versioneBibbia);
                    var capNode = new TreeViewItem { Header = cap.ToString(), Tag = new Tuple<byte, byte>(libro, cap) };

                    for (byte v = 1; v <= totVers; v++)
                        capNode.Items.Add(new TreeViewItem { Header = v.ToString(), Tag = new Tuple<byte, byte, byte>(libro, cap, v) });

                    libroNode.Items.Add(capNode);
                }
                TocTreeView.Items.Add(libroNode);
            }
        }

        private void TocTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (e.NewValue is TreeViewItem item)
                {
                    if (item.Tag is Tuple<byte, byte> t)
                        _ = CercaRiferimento(t.Item1, t.Item2, 1);
                    else if (item.Tag is Tuple<byte, byte, byte> t3)
                        _ = CercaRiferimento(t3.Item1, t3.Item2, t3.Item3);
                }
            }
            catch { }
        }

        private void TxtRiferimento_TextChanged(object sender, TextChangedEventArgs e) { }

        private void TxtRiferimento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled = true;
                string input = TxtRiferimento.Text.Trim();
                var rif = ParsaRiferimento(input);
                if (!rif.Valido)
                {
                    string corretto = Regex.Replace(input, @"^([1-3]?\s*[A-Za-z]+)\s*(\d+)", "$1 $2");
                    if (corretto != input)
                    {
                        rif = ParsaRiferimento(corretto);
                        if (rif.Valido) { TxtRiferimento.Text = corretto; TxtRiferimento.CaretIndex = corretto.Length; }
                    }
                }
                if (rif.Valido)
                {
                    ResultsDataGrid.ItemsSource = null;
                    _ = CercaRiferimento(rif.Libro, rif.Capitolo, rif.Versetto);
                }
                else
                    MessageBox.Show("Riferimento non valido. Usa: Libro capitolo:versetto (es. Gv 3:16)", "Cerca riferimenti", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PopolaFiltroLingua(RisultatoRicerca[] risultati)
        {
            LinguaFilterComboBox.Items.Clear();
            LinguaFilterComboBox.Items.Add(new ComboBoxItem { Content = (string)(Application.Current.TryFindResource("BibliotecaFiltroLinguaQualsiasi") ?? "All"), Tag = "All" });
            foreach (string? lang in risultati
                .Select(r => r.Lingua?.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(l => l))
                if (lang != null)
                {
                    LinguaFilterComboBox.Items.Add(new ComboBoxItem
                    {
                        Content = lang.ToUpperInvariant(),
                        Tag = lang
                    });
                }
            LinguaFilterComboBox.SelectedIndex = 0;
        }

        private void TipoFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) { try { ApplicaFiltri(); } catch { } }

        private void LinguaFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) { try { ApplicaFiltri(); } catch { } }

        private void DataConditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bool showYear = DataConditionComboBox.SelectedItem is ComboBoxItem item && item.Tag?.ToString() != "Any";
                if (FilterYearTextBox != null)
                    FilterYearTextBox.Visibility = showYear ? Visibility.Visible : Visibility.Collapsed;
                ApplicaFiltri();
            }
            catch { }
        }

        private void FilterYearTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { ApplicaFiltri(); } catch { }
        }

        private void ApplicaFiltri()
        {
            if (_tuttiRisultati == null) return;
            var filtrati = _tuttiRisultati.AsEnumerable();
            string? tipo = (TipoFilterComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(tipo) && tipo != "All")
                filtrati = filtrati.Where(r => r.Tipo.ToString() == tipo);
            string? lingua = (LinguaFilterComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(lingua) && lingua != "All")
                filtrati = filtrati.Where(r => r.Lingua?.IndexOf(lingua, StringComparison.OrdinalIgnoreCase) >= 0);
            string? dataCond = (DataConditionComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            string? anno = FilterYearTextBox.Text?.Trim();
            if (dataCond != "Any" && !string.IsNullOrEmpty(anno) && int.TryParse(anno, out int a))
            {
                filtrati = dataCond switch
                {
                    "Before" => filtrati.Where(r => { _ = int.TryParse(r.Data?.Split('-', '/', ' ')[0], out int y); return y > 0 && y < a; }),
                    "After" => filtrati.Where(r => { _ = int.TryParse(r.Data?.Split('-', '/', ' ')[0], out int y); return y > 0 && y >= a; }),
                    _ => filtrati
                };
            }
            ResultsDataGrid.ItemsSource = filtrati.ToArray();
        }

        private async Task CercaRiferimento(byte libro, byte capitolo, byte versetto)
        {
            try
            {
                var rif = new RiferimentoParsato { Libro = libro, Capitolo = capitolo, Versetto = versetto, Valido = true };
                _tuttiRisultati = await RicercaRiferimento(rif);
                PopolaFiltroLingua(_tuttiRisultati);
                ApplicaFiltri();
            }
            catch { }
        }

        private struct RiferimentoParsato
        {
            internal byte Libro; internal byte Capitolo; internal byte Versetto; internal bool Valido;
        }

        private static RiferimentoParsato ParsaRiferimento(string input)
        {
            var r = new RiferimentoParsato { Valido = false };
            var m = Regex.Match(input.Trim(), @"^([1-3]?\s?[A-Za-zàèéìòùÀÈÉÌÒÙ]+)\.?\s+(\d{1,3})(?::(\d{1,3}))?$");
            if (!m.Success) return r;
            string token = m.Groups[1].Value.Trim().ToLowerInvariant().Replace(" ", "").Replace(".", "");
            try
            {
                if (MainWindow.Testi == null) return r;
                r.Libro = MainWindow.Testi.GetLibroNumeroDaAbbreviazione(token);
                if (r.Libro < 1 || r.Libro > 73) return r;
                r.Capitolo = byte.Parse(m.Groups[2].Value);
                r.Versetto = m.Groups[3].Success ? byte.Parse(m.Groups[3].Value) : (byte)1;
                r.Valido = true;
            }
            catch { }
            return r;
        }

        private static RisultatoRicerca? CreaRisultato(string versione, TestoTipi tipo, string titoloNota)
        {
            try
            {
                if (MainWindow.Testi == null) return null;
                var info = MainWindow.Testi.Info(versione);
                return new RisultatoRicerca
                {
                    Nome = versione,
                    Autore = info.Autore ?? "",
                    Tipo = tipo,
                    Data = info.Data ?? "",
                    CasaEditrice = info.CasaEditrice ?? "",
                    Copyright = info.Copyright ?? "",
                    Lingua = info.Lingua ?? "",
                    Abbreviazione = info.Abbreviazione ?? "",
                    Versione = versione,
                    TitoloNota = titoloNota,
                };
            }
            catch { return null; }
        }

        private static async Task<RisultatoRicerca[]> RicercaRiferimento(RiferimentoParsato rif)
        {
            var risultati = new List<RisultatoRicerca>();
            var visti = new HashSet<string>();
            await Task.Yield();
            if (MainWindow.Testi == null) return [];

            foreach (string versione in MainWindow.Testi.NomiVersioni())
            {
                try
                {
                    var info = MainWindow.Testi.Info(versione);
                    if ((info.Tipo & (TestoTipi.Commentario | TestoTipi.Dizionario | TestoTipi.Libro)) == 0) continue;

                    foreach (string titolo in MainWindow.Testi.NoteConTitolo(versione))
                    {
                        if (string.IsNullOrEmpty(titolo)) continue;
                        try
                        {
                            if ((info.Tipo & TestoTipi.Commentario) == TestoTipi.Commentario && titolo.StartsWith('#'))
                            {
                                var mt = RegexTitoloNota.Match(titolo);
                                if (!mt.Success) continue;
                                if (byte.Parse(mt.Groups[1].Value) != rif.Libro || byte.Parse(mt.Groups[2].Value) != rif.Capitolo) continue;
                                if (byte.Parse(mt.Groups[4].Value) != rif.Libro || byte.Parse(mt.Groups[5].Value) != rif.Capitolo) continue;
                                byte vIni = byte.Parse(mt.Groups[3].Value);
                                byte vFine = byte.Parse(mt.Groups[6].Value);
                                if (rif.Versetto < vIni || rif.Versetto > vFine) continue;
                            }
                            else
                            {
                                string contenuto = MainWindow.Testi.GetNotaTesto(titolo, versione);
                                if (string.IsNullOrEmpty(contenuto)) continue;
                                string nomeLibro = MainWindow.Testi.GetLibroNome(rif.Libro);
                                string abbrLibro = MainWindow.Testi.LibriAbbreviazioniRiconosciute.Abbreviazione(rif.Libro);
                                string p1 = $"{nomeLibro} {rif.Capitolo}:{rif.Versetto}";
                                string p2 = $"{abbrLibro} {rif.Capitolo}:{rif.Versetto}";
                                if (contenuto.IndexOf(p1, StringComparison.OrdinalIgnoreCase) < 0 &&
                                    contenuto.IndexOf(p2, StringComparison.OrdinalIgnoreCase) < 0) continue;
                            }
                            string chiave = versione + "|" + titolo;
                            if (!visti.Add(chiave)) continue;
                            var r = CreaRisultato(versione, info.Tipo, titolo);
                            if (r != null) risultati.Add(r);
                        }
                        catch { }
                    }
                }
                catch { }
            }
            return [.. risultati];
        }

        private void ResultsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem is RisultatoRicerca r && !string.IsNullOrEmpty(r.Versione) && !string.IsNullOrEmpty(r.TitoloNota))
                App.DockingHost.OpenViewerDocument(r.Versione, r.TitoloNota);
        }

        [GeneratedRegex(@"^#(\d{2})(\d{3})(\d{3})(?:\d{4})-(\d{2})(\d{3})(\d{3})(?:\d{4})$", RegexOptions.Compiled)]
        private static partial Regex RegexNotaTitolo();
    }
}
