using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class ListaTesti : Template
    {
        private Principale genitore;

        public ListaTesti(Principale formGenitore)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");
            genitore = formGenitore;

            InitializeComponent();
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            cbFormato.SelectedIndex = Settings.Default.ListaTestiFormato;
            cbOrdine.SelectedIndex = Settings.Default.ListaTestiOrdine;
            string[] colonneSelezionate = Settings.Default.ListaTestiColonneSelezionate.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string colonna in colonneSelezionate)
            {
                int colonnaIntero = Convert.ToInt32(colonna, CultureInfo.InvariantCulture);
                if (colonnaIntero >= 0 && colonnaIntero < clbColonne.Items.Count)
                    clbColonne.SetItemChecked(colonnaIntero, true);
            }
        }

        private void btnTutti_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbColonne.Items.Count; ++i)
                clbColonne.SetItemChecked(i, true);
        }

        private void btnNessuno_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbColonne.Items.Count; ++i)
                clbColonne.SetItemChecked(i, false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string ultimaCartella = Settings.Default.UltimaDirectory;
                if (String.IsNullOrEmpty(ultimaCartella))
                    ultimaCartella = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.InitialDirectory = ultimaCartella;
                switch (cbFormato.SelectedIndex)
                {
                    case 1:
                        saveFileDialog.Filter = Principale.LocRM.GetString("ListTextsFilterRtf");
                        break;
                    case 2:
                        saveFileDialog.Filter = Principale.LocRM.GetString("ListTextsFilterCsv");
                        break;
                    case 3:
                        saveFileDialog.Filter = Principale.LocRM.GetString("ListTextsFilterHtml");
                        break;
                    case 4:
                        saveFileDialog.Filter = Principale.LocRM.GetString("ListTextsFilterXml");
                        break;
                    default: // include 0
                        saveFileDialog.Filter = Principale.LocRM.GetString("ListTextsFilterText");
                        break;
                }

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    Cursor cursoreAttuale = Cursor.Current;
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        Settings.Default.UltimaDirectory = Path.GetDirectoryName(saveFileDialog.FileName);

                        string inizioRiga, fineRiga, inizioColonna, fineColonna, info;
                        string[] componenti = Principale.LocRM.GetString("UpdateComponentTypes").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        string bibbia = componenti[3];
                        string commentario = componenti[4];
                        string dizionario = componenti[5];
                        string libro = componenti[7];
                        StringBuilder testoFile = new StringBuilder();

                        switch (cbFormato.SelectedIndex)
                        {
                            case 1:
                                inizioRiga = "";
                                fineRiga = "\\par\r\n";
                                inizioColonna = "{\\b $$$:} ";
                                fineColonna = "\\par\r\n";
                                testoFile.Append(Principale.testi.RtfIntestazione());
                                break;
                            case 2:
                                inizioRiga = "";
                                fineRiga = "\r\n";
                                inizioColonna = "";
                                fineColonna = ";";
                                break;
                            case 3:
                                inizioRiga = "<tr>\r\n";
                                fineRiga = "</tr>\r\n";
                                inizioColonna = "  <td>";
                                fineColonna = "</td>\r\n";
                                testoFile.Append("<html>\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n</head>\r\n<body>\r\n<table>\r\n");
                                testoFile.Append("<tr>\r\n  <th>").Append(Principale.LocRM.GetString("ListTextsText")).Append("</th>\r\n");
                                foreach (int i in clbColonne.CheckedIndices)
                                    testoFile.Append("  <th>").Append(clbColonne.Items[i]).Append("</th>\r\n");
                                testoFile.Append("</tr>\r\n");
                                break;
                            case 4:
                                inizioRiga = "<text>\r\n";
                                fineRiga = "</text>\r\n";
                                inizioColonna = "  <$$$>";
                                fineColonna = "</$$$>\r\n";
                                testoFile.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<texts>\r\n");
                                break;
                            default: // include 0
                                inizioRiga = "";
                                fineRiga = "----------\r\n";
                                inizioColonna = "$$$: ";
                                fineColonna = "\r\n";
                                break;
                        }

                        Collection<string> tuttiTesti = Principale.testi.NomiVersioni();
                        SortedList<string, string> dizionarioVersioniInOrdine = new SortedList<string, string>(tuttiTesti.Count);
                        foreach (string versione in tuttiTesti)
                        {
                            string chiave = "";
                            switch (cbOrdine.SelectedIndex)
                            {
                                // case 0: non serve fare niente
                                case 1: // tipo
                                    chiave = TipoPerOrdine(versione);
                                    break;
                                case 2: // lingua
                                    chiave = LinguaPerOrdine(versione);
                                    break;
                                case 3: // tipo, lingua
                                    chiave = TipoPerOrdine(versione) + LinguaPerOrdine(versione);
                                    break;
                                case 4: // lingua, tipo
                                    chiave = LinguaPerOrdine(versione) + TipoPerOrdine(versione);
                                    break;
                            }
                            dizionarioVersioniInOrdine.Add(chiave + versione, versione);
                        }

                        string stringaTemp;
                        foreach (KeyValuePair<string, string> kvp in dizionarioVersioniInOrdine)
                        {
                            string versione = kvp.Value;
                            testoFile.Append(inizioRiga);
                            if (cbFormato.SelectedIndex == 1)
                                testoFile.Append(@"{\qc");
                            stringaTemp = Principale.LocRM.GetString("ListTextsText");
                            if (cbFormato.SelectedIndex == 4) // XML
                                stringaTemp = stringaTemp.Replace(" ", "");
                            testoFile.Append(inizioColonna.Replace("$$$", stringaTemp)).Append(versione).Append(fineColonna.Replace("$$$", stringaTemp));
                            if (cbFormato.SelectedIndex == 1) // RTF
                                testoFile.Append(@"}");

                            foreach (int i in clbColonne.CheckedIndices)
                            {
                                switch (i)
                                {
                                    case 0:
                                        info = Principale.testi.Info(versione).Titolo;
                                        break;
                                    case 1:
                                        info = Principale.testi.Info(versione).Abbreviazione;
                                        break;
                                    case 2:
                                        info = "";
                                        TestoTipi tipi = Principale.testi.Info(versione).Tipo;
                                        if ((tipi & TestoTipi.Bibbia) == TestoTipi.Bibbia)
                                            info += bibbia + ", ";
                                        if ((tipi & TestoTipi.Commentario) == TestoTipi.Commentario)
                                            info += commentario + ", ";
                                        if ((tipi & TestoTipi.Dizionario) == TestoTipi.Dizionario)
                                            info += dizionario + ", ";
                                        if ((tipi & TestoTipi.Libro) == TestoTipi.Libro)
                                            info += libro + ", ";
                                        if (info.EndsWith(", ", StringComparison.Ordinal))
                                            info = info.Remove(info.Length - 2);
                                        break;
                                    case 3:
                                        info = Principale.testi.Info(versione).Autore;
                                        break;
                                    case 4:
                                        info = Principale.testi.Info(versione).Data;
                                        break;
                                    case 5:
                                        info = Principale.testi.Info(versione).Lingua;
                                        break;
                                    case 6:
                                        info = Principale.testi.Info(versione).CasaEditrice;
                                        break;
                                    case 7:
                                        info = Principale.testi.Info(versione).Isbn;
                                        break;
                                    case 8:
                                        info = Principale.testi.Info(versione).Copyright;
                                        break;
                                    case 9:
                                        info = Principale.testi.Info(versione).Descrizione;
                                        if (cbFormato.SelectedIndex == 1)
                                        {
                                            info = info.Replace(@"\", @"\\");
                                            info = info.Replace(@"{", @"\{");
                                            info = info.Replace(@"}", @"\}");
                                            info = info.Replace("\r\n", @"\par ");
                                        }
                                        break;
                                    case 10:
                                        info = Principale.testi.Info(versione).NomeDelFile;
                                        if (cbFormato.SelectedIndex == 1)
                                            info = info.Replace(@"\", @"\\");
                                        break;
                                    case 11:
                                        info = Funzioni.VersioneMinore2Cifre(Principale.testi.Info(versione).Versione);
                                        break;
                                    default: // non dovrebbe succedere
                                        info = "";
                                        break;
                                }
                                if (cbFormato.SelectedIndex == 2) // CSV
                                {
                                    info = info.Replace("\"", "\"\"");
                                    info = info.Replace("\r\n", " ");
                                    info = info.Replace("\n", " ");
                                    if (info.Contains(";") || info.Contains("\"\""))
                                        info = "\"" + info + "\"";
                                }
                                else if (cbFormato.SelectedIndex == 3 || cbFormato.SelectedIndex == 4)
                                    info = info.Replace("&", "&amp;");
                                stringaTemp = clbColonne.Items[i].ToString();
                                if (cbFormato.SelectedIndex == 4) // XML
                                    stringaTemp = stringaTemp.Replace(" ", "");
                                testoFile.Append(inizioColonna.Replace("$$$", stringaTemp)).Append(info).Append(fineColonna.Replace("$$$", stringaTemp));
                            }
                            testoFile.Append(fineRiga);
                        }

                        switch (cbFormato.SelectedIndex)
                        {
                            case 1:
                                testoFile.Append("}");
                                break;
                            case 2:
                                break;
                            case 3:
                                testoFile.Append("</table>\r\n</body>\r\n</html>");
                                break;
                            case 4:
                                testoFile.Append("</texts>");
                                break;
                            default: // include 0
                                break;
                        }
                        string testoStringa = testoFile.ToString();
                        if (cbFormato.SelectedIndex == 1)
                        {
                            for (int i = testoStringa.Length - 1; i >= 0; --i)
                            {
                                if (testoStringa[i] > 256)
                                    testoStringa = testoStringa.Substring(0, i) + @"\u" + Convert.ToUInt32(testoStringa[i]).ToString(CultureInfo.InvariantCulture) + "?" + testoStringa.Substring(i + 1);
                                else if (testoStringa[i] > 127)
                                    testoStringa = testoStringa.Substring(0, i) + @"\'" + Uri.HexEscape(testoStringa[i]).Remove(0, 1) + testoStringa.Substring(i + 1);
                            }
                        }

                        File.WriteAllText(saveFileDialog.FileName, testoStringa, (cbFormato.SelectedIndex == 1) ? Encoding.ASCII : Encoding.UTF8); // ==1 è RTF

                        switch (cbFormato.SelectedIndex)
                        {
                            case 2:
                            case 3:
                            case 4:
                                try
                                {
                                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                                }
                                catch (Win32Exception)
                                {
                                    // Firefox (forse con qualche add-in) dà questo errore se non è aperto quando il comando di aprire il file è eseguito
                                }
                                break;
                            case 1:
                            default: // include 0
                                genitore.ApriFile(saveFileDialog.FileName);
                                break;
                        }

                        Settings.Default.ListaTestiFormato = cbFormato.SelectedIndex;
                        Settings.Default.ListaTestiOrdine = cbOrdine.SelectedIndex;
                        StringBuilder colonneSelezionate = new StringBuilder();
                        foreach (int i in clbColonne.CheckedIndices)
                            colonneSelezionate.Append(i.ToString(CultureInfo.InvariantCulture)).Append("|");
                        Settings.Default.ListaTestiColonneSelezionate = colonneSelezionate.ToString();
                    }
                    finally
                    {
                        Cursor.Current = cursoreAttuale;
                        if (cursoreAttuale != null)
                            cursoreAttuale.Dispose();
                        Close();
                    }
                }
            }
        }

        static private string TipoPerOrdine(string versione)
        {
            int tipo = Convert.ToInt32(Principale.testi.Info(versione).Tipo, CultureInfo.InvariantCulture);
            if (tipo == 4)
                tipo = 11; // questo mette i tipi di testo nell'ordine che per me è naturale
            if (tipo == 14)
                tipo = 10;
            return Funzioni.AggiungiZero(tipo, 2);
        }

        static private string LinguaPerOrdine(string versione)
        {
            string lingua = Principale.testi.Info(versione).Lingua;
            if (lingua.Length <= 8)
                lingua += new String('!', 8 - lingua.Length);
            return lingua;
        }
    }
}
