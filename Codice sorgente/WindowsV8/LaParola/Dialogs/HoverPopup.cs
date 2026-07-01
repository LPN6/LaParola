using LaParola.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola.Dialogs
{
    public static class HoverPopup
    {
        private static readonly Popup _hoverPopup;
        private static readonly RichTextBox _popupViewer;
        // usare RichTextBoxEx potrebbe permettere ipertesto da ipertesto, ma invece dà errore in _hoverPopup.IsOpen = true;
        private static readonly Border border;

        // Dedicated field to track the active hyperlink object safely
        private static Hyperlink? _activeLink;

        static HoverPopup()
        {
            _hoverPopup = new Popup
            {
                AllowsTransparency = true,
                StaysOpen = true,
                // Using MousePoint ensures it opens precisely under the cursor
                Placement = PlacementMode.MousePoint
            };

            border = new()
            {
                Background = Application.Current.Resources["AppBackgroundBrush"] as Brush ?? Brushes.White,
                BorderBrush = Application.Current.Resources["SubtleBorderBrush"] as Brush ?? Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6),
                MaxWidth = 450,
                MaxHeight = 250
            };

            _popupViewer = new RichTextBox
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                IsTabStop = true,
                Focusable = true,
                IsReadOnly = true,       // Prevent user from typing in the popup
                IsUndoEnabled = false
            };

            // Remove default padding/margins inside RichTextBox paragraphs
            Style style = new(typeof(Paragraph));
            style.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            _popupViewer.Resources.Add(typeof(Paragraph), style);

            border.Child = _popupViewer;
            _hoverPopup.Child = border;

            _hoverPopup.MouseLeave += (s, e) => EvaluateMouseLeave();
        }

        public static void CambiaTema()
        {
            border.Background = Application.Current.Resources["AppBackgroundBrush"] as Brush ?? Brushes.White;
            border.BorderBrush = Application.Current.Resources["SubtleBorderBrush"] as Brush ?? Brushes.Gray;
        }

        public static async void OnHyperlinkHover(object sender, MouseEventArgs e)
        {
            // TODO2 also note, file?
            if (sender is Hyperlink hyperlink)
            {
                // GUARD 1: If we are already displaying/loading this exact link, do nothing.
                // This prevents endless loops if MouseMove triggers multiple times on the same link.
                if (_activeLink == hyperlink && _hoverPopup.IsOpen)
                {
                    return;
                }

                _hoverPopup.IsOpen = false;

                // Assign the link to our tracker variable
                _activeLink = hyperlink;

                // Find the actual UIElement (e.g. RichTextBox) hosting this text hyperlink
                DependencyObject parent = hyperlink;
                while (parent != null && parent is not UIElement)
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }
                _hoverPopup.PlacementTarget = parent as UIElement;

                hyperlink.MouseLeave -= Hyperlink_MouseLeave;
                hyperlink.MouseLeave += Hyperlink_MouseLeave;

                _popupViewer.Document.Blocks.Clear();
                //_popupViewer.Document.Blocks.Add(new Paragraph(new Run("")));
                _hoverPopup.IsOpen = true;

                string uri = hyperlink.NavigateUri?.OriginalString ?? "";

                if (uri.StartsWith("bibbia:"))
                {
                    try
                    {
                        string code = uri.Replace("bibbia:", "");
                        string versioneDaUtilizzare;
                        Riferimento riferimento;
                        (versioneDaUtilizzare, riferimento) = MainWindow.VersionePerLinkBibbia(code);

                        string rtf = await MainWindow.Testi.TestoBranoAsync(riferimento, versioneDaUtilizzare);
                        rtf = Funzioni.ConvertiUnicodeInRtf(MainWindow.AncoraRegEx.Replace(rtf, ""));

                        ImpostaTestoHover(rtf, hyperlink);
                    }
                    catch
                    {
                        _hoverPopup.IsOpen = false;
                    }
                }
                else if (uri.StartsWith("nota:"))
                {
                    try
                    {
                        string noteName = uri.Replace("nota:", "");
                        string collezioneNuovaNota = "";
                        if (noteName.IndexOf('\\') > 0) // in questo modo, è possibile creare un link "Nuova Riveduta\#010010010000-01001002000"
                        { // in versione.cs, il nome del commentario è stato nel link affinché possa aprire nello stesso commentario
                            collezioneNuovaNota = noteName[..noteName.IndexOf('\\')];
                            noteName = noteName[(noteName.IndexOf('\\') + 1)..];

                            // se la collezione richiesta non esiste, non fare niente
                            if (!MainWindow.Testi.VersioneEsiste(collezioneNuovaNota))
                                return;
                        }
                        if (!string.IsNullOrEmpty(collezioneNuovaNota))
                        {
                            if (!noteName.StartsWith('#') && Char.IsDigit(noteName[^1]) && MainWindow.Testi.GetNumeroNotaTitolo(collezioneNuovaNota, noteName) < 0)
                            { // riferimenti ai brani nel con #, come Mt 1:21 -> Mt 1:1 in Note NR
                                Riferimento noteInBrano = MainWindow.Testi.ElencaNoteInBrano(MainWindow.Testi.ConvertiRiferimento(noteName), collezioneNuovaNota);
                                if (noteInBrano.Count > 0)
                                {
                                    noteName = string.Join("", noteInBrano.Note);
                                }
                            }
                            if (noteName.StartsWith('#'))
                            {
                                Riferimento riferimentoNota = MainWindow.Testi.ConvertiRiferimento(MainWindow.Testi.ConvertiTitoloNotaARiferimento(noteName));
                                string rtf = await MainWindow.Testi.TestoBranoAsync(riferimentoNota, collezioneNuovaNota);
                                rtf = Funzioni.ConvertiUnicodeInRtf(MainWindow.AncoraRegEx.Replace(rtf, ""));

                                ImpostaTestoHover(rtf, hyperlink);
                            }
                            // TODO2 else open nota su tema
                        }
                    }
                    catch
                    {
                        _hoverPopup.IsOpen = false;
                    }

                    // TODO2 file or non fare niente in questo caso
                    /*
                    // CASE 3: External Local Machine Files
                    else if (uri.StartsWith("filenome:"))
                    {
                        string fileName = uri.Replace("filename:", "");
                        hyperlink.ToolTip = $"Apri file esterno: {Path.GetFileName(fileName)}";
                    }
                    */
                }
            }
        }

        private static async void ImpostaTestoHover(string rtf, Hyperlink hyperlink)
        {
            // GUARD 2: If the user moved their mouse to a DIFFERENT link while the database 
            // query was executing, cancel this UI update so we don't display wrong text.
            if (_activeLink != hyperlink) return;

            await _popupViewer.Dispatcher.InvokeAsync(() =>
            {
                // GUARD 3: Re-verify on the UI thread to eliminate asynchronous race conditions
                if (_activeLink != hyperlink) return;

                // Clear the empty paragraph entirely before loading the RTF blocks
                _popupViewer.Document.Blocks.Clear();

                if (!string.IsNullOrEmpty(rtf))
                {
                    try
                    {
                        TextRange range = new(_popupViewer.Document.ContentStart, _popupViewer.Document.ContentEnd);
                        using MemoryStream ms = new(Encoding.UTF8.GetBytes(rtf));
                        range.Load(ms, DataFormats.Rtf);

                        if (Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode))
                        {
                            Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, RtfColorTransformer.GetDarkThemedBrush(fg, Brushes.White));
                        }
                    }
                    catch //(Exception rtfEx)
                    {
                        //Debug.WriteLine($"RTF Parsing failed: {rtfEx.Message}");

                        // Fallback: Display as plain text if it fails to parse
                        _popupViewer.Document.Blocks.Clear();
                        _popupViewer.Document.Blocks.Add(new Paragraph(new Run("")));
                    }
                }
                else
                {
                    _popupViewer.Document.Blocks.Add(new Paragraph(new Run("")));
                    _popupViewer.Focus();
                }
            });

        }

        private static void Hyperlink_MouseLeave(object sender, MouseEventArgs e)
        {
            EvaluateMouseLeave();
            if (sender is Hyperlink hyperlink)
            {
                hyperlink.MouseLeave -= Hyperlink_MouseLeave;
            }
        }

        private static void EvaluateMouseLeave()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                // FIX 5: Check against our strongly typed tracking variable instead of casting PlacementTarget
                if (!_hoverPopup.IsMouseOver && (_activeLink == null || !_activeLink.IsMouseOver))
                {
                    _hoverPopup.IsOpen = false;
                    _activeLink = null; // Clear out reference when closed
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
