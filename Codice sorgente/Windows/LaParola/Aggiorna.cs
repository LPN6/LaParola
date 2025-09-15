using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;
using LaParola.Properties;
using System.Text;

namespace LaParola
{
    public partial class Aggiorna : Template
    {
        private Collection<FileDaAggiornare> fileDaAggiornare = null;
        private string aggiornaProgrammaAggiornamentoCartella;
        // se cambi la seguente riga, bisogna anche cambiare UpdateComponentTypes nelle risorse
        private string[] componenteTipiXML = { "programma", "aggiornamento", "testo", "Bibbia", "commentario", "dizionario", "note", "libro", "parallelo", "collegamento", "segnalibro", "lettura", "video", "testiparalleli" };
        private string[] componentoTipi;

        public Aggiorna(Principale formGenitore, Collection<FileDaAggiornare> listaFileDaAggiornare, string aggiornaUpdateInternetCartella)
        {
            InitializeComponent();
            guidaFile.HelpNamespace = formGenitore.NomeFileGuida();

            aggiornaProgrammaAggiornamentoCartella = aggiornaUpdateInternetCartella;

            colAzione.Items.Add(Principale.LocRM.GetString("UpdateNoUpdate"));
            colAzione.Items.Add(Principale.LocRM.GetString("UpdateUpdateNow"));
            colAzione.Items.Add(Principale.LocRM.GetString("UpdateHide"));

            fileDaAggiornare = new Collection<FileDaAggiornare>(listaFileDaAggiornare);

            string[] testiNascosti = Settings.Default.AggiornamentoTestiNascosti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            componentoTipi = Principale.LocRM.GetString("UpdateComponentTypes").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            int numeroFileDaAggiornare = listaFileDaAggiornare.Count;
            string nuovo = Principale.LocRM.GetString("UpdateNew");
            for (int i = 0; i < numeroFileDaAggiornare; ++i)
            {
                gridFile.Rows.Add(new string[] {listaFileDaAggiornare[i].nome,
                    TraduciTipo(listaFileDaAggiornare[i].tipo),
                    (listaFileDaAggiornare[i].versioneAttuale == "0.0.0") ? nuovo : Funzioni.VersioneMinore2Cifre(listaFileDaAggiornare[i].versioneAttuale),
                    listaFileDaAggiornare[i].versioneNuova,
                    listaFileDaAggiornare[i].dimensione, 
                    colAzione.Items[0].ToString() });
                if (Array.IndexOf(testiNascosti, listaFileDaAggiornare[i].componente) >= 0)
                    gridFile.Rows[i].Visible = false;
                gridFile.Rows[i].Cells[0].Tag = i;
            }
        }

        private string TraduciTipo(string s)
        {
            if (Array.IndexOf(componenteTipiXML, s) >= 0)
                return componentoTipi[Array.IndexOf(componenteTipiXML, s)];
            else
                return "";
        }

        private void btnVisualizza_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridFile.Rows.Count; ++i)
                gridFile.Rows[i].Visible = true;
        }

        internal void EseguiAggiornamento(int tipoAggiornamento)
        {
            // tipoAggionamento: 0=manuale, 1=automatico di file esistenti, 2=automatico di tutti
            string testiNascosti = "";
            List<string> listaFileDaAggiornare = new List<string>();
            string aggiornaAdesso = colAzione.Items[1].ToString();
            string nuovo = Principale.LocRM.GetString("UpdateNew");
            List<string> versioniVideo = new List<string>(Settings.Default.VideoInstallati.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            bool videoTrovato;

            for (int i = 0; i < gridFile.Rows.Count; ++i)
            {
                // numeroFileDaAggiornare contiene il numero originale del componente nella lista, e il numero in fileDaAggiornare
                // può essere diverso dal numero attuale perché l'utente può cambiare il numero reordinando le righe in base ad un'altra colonna
                int numeroFileDaAggiornare = Convert.ToInt32(gridFile.Rows[i].Cells[0].Tag, CultureInfo.InvariantCulture);
                if (!gridFile.Rows[i].Visible || colAzione.Items.IndexOf(gridFile.Rows[i].Cells[5].Value) == 2)
                {
                    if (!string.IsNullOrEmpty(testiNascosti))
                        testiNascosti += "|";
                    testiNascosti += fileDaAggiornare[numeroFileDaAggiornare].componente;
                }

                if (gridFile.Rows[i].Cells[5].Value.ToString() == aggiornaAdesso || (tipoAggiornamento == 1 && gridFile.Rows[i].Cells[2].Value.ToString() != nuovo) || tipoAggiornamento == 2)
                {
                    string cartellaComputer = fileDaAggiornare[numeroFileDaAggiornare].nomeFile.Remove(fileDaAggiornare[numeroFileDaAggiornare].nomeFile.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    cartellaComputer = Funzioni.RimuoviCaratteriNonValidiInXml(cartellaComputer);
                    for (int j = 0; j < fileDaAggiornare[numeroFileDaAggiornare].url.Count; ++j)
                    {
                        if (j == 0)
                            listaFileDaAggiornare.Add("<file nome=\"" + fileDaAggiornare[numeroFileDaAggiornare].nome + "\">");
                        else
                            listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + fileDaAggiornare[numeroFileDaAggiornare].url[j] + "</url>");
                        string nomeFile = fileDaAggiornare[numeroFileDaAggiornare].url[j].Remove(0, fileDaAggiornare[numeroFileDaAggiornare].url[j].LastIndexOf('/') + 1).Replace('|', '/');
                        if (nomeFile.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                            nomeFile = nomeFile.Remove(nomeFile.Length - 3);
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + nomeFile + "</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                    }
                    if (fileDaAggiornare[numeroFileDaAggiornare].tipo == "programma")
                    {
                        string cartellaUrl = fileDaAggiornare[numeroFileDaAggiornare].url[0].Remove(fileDaAggiornare[numeroFileDaAggiornare].url[0].LastIndexOf('/') + 1);
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "testi.dll.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "testi.dll</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "testi.tlb.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "testi.tlb</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "LaParola.resources.dll.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "it" + Path.DirectorySeparatorChar + "LaParola.resources.dll</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "LaParola.resources.es.dll.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "es" + Path.DirectorySeparatorChar + "LaParola.resources.dll</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "Light.exe.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "Light.exe</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "Light.resources.dll.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "it" + Path.DirectorySeparatorChar + "Light.resources.dll</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "Light.resources.es.dll.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "es" + Path.DirectorySeparatorChar + "Light.resources.dll</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "laparola.chm.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "laparola.chm</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "laparola.it.chm.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "laparola.it.chm</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                        listaFileDaAggiornare.Add("<file nome=\"\">");
                        listaFileDaAggiornare.Add("<url>" + cartellaUrl + "laparola.es.chm.gz</url>");
                        listaFileDaAggiornare.Add("<nomeFile>" + cartellaComputer + "laparola.es.chm</nomeFile>");
                        listaFileDaAggiornare.Add("</file>");
                    }

                    if (fileDaAggiornare[numeroFileDaAggiornare].tipo == "video")
                    { // aggiornare l'elenco delle versioni dei video nei settings
                        videoTrovato = false;
                        for (int k = 0; k < versioniVideo.Count / 2; ++k)
                        {
                            if (versioniVideo[k * 2] == fileDaAggiornare[numeroFileDaAggiornare].nomeFile)
                            {
                                versioniVideo[k * 2 + 1] = fileDaAggiornare[numeroFileDaAggiornare].versioneNuova;
                                break;
                            }
                        }
                        if (!videoTrovato)
                        {
                            versioniVideo.Add(fileDaAggiornare[numeroFileDaAggiornare].nomeFile);
                            versioniVideo.Add(fileDaAggiornare[numeroFileDaAggiornare].versioneNuova);
                        }
                    }
                }
            }

            StringBuilder versioniVideoStringa = new StringBuilder(versioniVideo.Count * 15);
            for (int i = 0; i < versioniVideo.Count; ++i)
                versioniVideoStringa.Append(versioniVideo[i]).Append("|");
            Settings.Default.VideoInstallati = versioniVideoStringa.ToString();
            Settings.Default.AggiornamentoTestiNascosti = testiNascosti;

            if (listaFileDaAggiornare.Count > 0)
            {
                if (!string.IsNullOrEmpty(Settings.Default.AggiornamentoProxyHost))
                    listaFileDaAggiornare.Insert(0, "<proxyHost>" + Settings.Default.AggiornamentoProxyHost + "</proxyHost>");
                if (Settings.Default.AggiornamentoProxyPorta != 0)
                    listaFileDaAggiornare.Insert(0, "<proxyPorta>" + Settings.Default.AggiornamentoProxyPorta + "</proxyPorta>");
                if (!string.IsNullOrEmpty(Settings.Default.AggiornamentoProxyNomeUtente))
                    listaFileDaAggiornare.Insert(0, "<credentialUtente>" + Settings.Default.AggiornamentoProxyNomeUtente + "</credentialUtente>");
                if (!string.IsNullOrEmpty(Settings.Default.AggiornamentoProxyPassword))
                    listaFileDaAggiornare.Insert(0, "<credentialPassword>" + Settings.Default.AggiornamentoProxyPassword + "</credentialPassword>");
                if (!string.IsNullOrEmpty(Settings.Default.AggiornamentoProxyDominio))
                    listaFileDaAggiornare.Insert(0, "<credentialDominio>" + Settings.Default.AggiornamentoProxyDominio + "</credentialDominio>");

                string versioneAttuale = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                versioneAttuale = versioneAttuale.Remove(versioneAttuale.LastIndexOf('.'));
                listaFileDaAggiornare.Insert(0, "<versioneAttuale>" + versioneAttuale + "</versioneAttuale>");
                listaFileDaAggiornare.Insert(0, "<versioni>");
                listaFileDaAggiornare.Insert(0, "<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                listaFileDaAggiornare.Add("</versioni>");

                string nomeFileTemp = Path.GetTempFileName();
                File.WriteAllLines(nomeFileTemp, listaFileDaAggiornare.ToArray());

                try
                {
                    if (!string.IsNullOrEmpty(aggiornaProgrammaAggiornamentoCartella))
                    {
                        WebClient cliente = new WebClient();
                        if (!string.IsNullOrEmpty(Settings.Default.AggiornamentoProxyHost))
                        {
                            if (Settings.Default.AggiornamentoProxyPorta == 0)
                                cliente.Proxy = new WebProxy(Settings.Default.AggiornamentoProxyHost);
                            else
                                cliente.Proxy = new WebProxy(Settings.Default.AggiornamentoProxyHost, Settings.Default.AggiornamentoProxyPorta);
                            if (!string.IsNullOrEmpty(Settings.Default.AggiornamentoProxyNomeUtente))
                            {
                                if (string.IsNullOrEmpty(Settings.Default.AggiornamentoProxyDominio))
                                    cliente.Proxy.Credentials = new NetworkCredential(Settings.Default.AggiornamentoProxyNomeUtente, Settings.Default.AggiornamentoProxyPassword);
                                else
                                    cliente.Proxy.Credentials = new NetworkCredential(Settings.Default.AggiornamentoProxyNomeUtente, Settings.Default.AggiornamentoProxyPassword, Settings.Default.AggiornamentoProxyDominio);
                            }
                        }

                        string nomeFileUpdate = Application.StartupPath + Path.DirectorySeparatorChar + "Update.exe";
                        if (File.Exists(nomeFileUpdate))
                            File.Delete(nomeFileUpdate);
                        cliente.DownloadFile(new Uri(aggiornaProgrammaAggiornamentoCartella + "Update.exe"), nomeFileUpdate);

                        string nomeFileUpdateIt = Application.StartupPath + Path.DirectorySeparatorChar + "it" + Path.DirectorySeparatorChar + "Update.resources.dll";
                        if (File.Exists(nomeFileUpdateIt))
                            File.Delete(nomeFileUpdateIt);
                        cliente.DownloadFile(aggiornaProgrammaAggiornamentoCartella + "Update.resources.dll", nomeFileUpdateIt);

                        string nomeFileUpdateEs = Application.StartupPath + Path.DirectorySeparatorChar + "es" + Path.DirectorySeparatorChar + "Update.resources.dll";
                        if (File.Exists(nomeFileUpdateEs))
                            File.Delete(nomeFileUpdateEs);
                        cliente.DownloadFile(aggiornaProgrammaAggiornamentoCartella + "Update.resources.es.dll", nomeFileUpdateEs);
                    }

                    string percorsoUpdate = Application.StartupPath + Path.DirectorySeparatorChar + "Update.exe";
                    if (Principale.isRunningOnMono)
                    {
                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("mono", percorsoUpdate + " " + nomeFileTemp)
                        {
                            UseShellExecute = false
                        };
                        System.Diagnostics.Process.Start(psi);
                    }
                    else
                        System.Diagnostics.Process.Start(percorsoUpdate, "\"" + nomeFileTemp + "\"");
                    Application.Exit();
                }
                catch (Exception eccezione)
                {
                    MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Principale.LocRM.GetString("UpdateProgramNotRun"), eccezione.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                }
            }
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            EseguiAggiornamento(0);
            Close();
        }

        private void btnAggiornaTutti_Click(object sender, EventArgs e)
        {
            object aggiorna = colAzione.Items[1];
            for (int i = 0; i < gridFile.Rows.Count; ++i)
            {
                DataGridViewRow riga = gridFile.Rows[i];
                if (riga.Visible)
                    riga.Cells[5].Value = aggiorna;
            }
        }
    }
}