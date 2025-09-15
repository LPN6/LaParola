using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

[assembly: CLSCompliant(true)]
namespace Update
{
    public partial class Principale : Form
    {
        private ResourceManager risorseUpdate = new ResourceManager("Update.updateRisorse", typeof(Principale).Assembly);
        private List<FileDaAggiornare> listaFileDaAggiornare = new List<FileDaAggiornare>();
        private bool aggiornareDopoLoad = false;
        private string versioneAttuale = "0.0.0";
        private string proxyHost = "";
        private int proxyPorta = 0;
        private string credentialUtente = "";
        private string credentialPassword = "";
        private string credentialDominio = "";
        internal static bool isRunningOnMono;

        public Principale()
        {
            InitializeComponent();

            isRunningOnMono = (Type.GetType("Mono.Runtime") != null);

            string[] argomenti = Environment.GetCommandLineArgs();
            string nomeFileTemp = "";
            if (argomenti.Length >= 2)
                nomeFileTemp = argomenti[1];

            if (!File.Exists(nomeFileTemp))
            {
                etiMessaggio.Text = risorseUpdate.GetString("FileNonTrovato");
                return;
            }

            if (!isRunningOnMono)
                HttpWebRequest.DefaultCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

            bool senzaErrore = true;
            try
            {
                XmlDocument xmlDocumento = new XmlDocument();
                xmlDocumento.Load(nomeFileTemp);

                XmlNode nodo = xmlDocumento.SelectSingleNode("versioni").SelectSingleNode("versioneAttuale");
                if (nodo != null)
                    versioneAttuale = nodo.InnerText;

                nodo = xmlDocumento.SelectSingleNode("versioni").SelectSingleNode("proxyHost");
                if (nodo != null)
                    proxyHost = nodo.InnerText;

                nodo = xmlDocumento.SelectSingleNode("versioni").SelectSingleNode("proxyPorta");
                if (nodo != null)
                    proxyPorta = Convert.ToInt32(nodo.InnerText, CultureInfo.CurrentCulture);

                nodo = xmlDocumento.SelectSingleNode("versioni").SelectSingleNode("credentialUtente");
                if (nodo != null)
                    credentialUtente = nodo.InnerText;

                nodo = xmlDocumento.SelectSingleNode("versioni").SelectSingleNode("credentialPassword");
                if (nodo != null)
                    credentialPassword = nodo.InnerText;

                nodo = xmlDocumento.SelectSingleNode("versioni").SelectSingleNode("credentialDominio");
                if (nodo != null)
                    credentialDominio = nodo.InnerText;

                Version versione = new Version(versioneAttuale);
                // in questo modo un nuovo componente è aggiunto ai file del programma (cioè non un componente facoltativo)
                // è anche necessario aggiungere il codice in LaParola/aggiorna.cs per aggiornare questo file nei futuri aggiornamenti
                if (versione.Major < 7 || (versione.Major == 7 && versione.Minor <= 7))
                {
                    FileDaAggiornare fileDaAggiornare = new FileDaAggiornare
                    {
                        nome = "",
                        nomeFile = Application.StartupPath + Path.DirectorySeparatorChar + "testi.tlb",
                        url = "http://www.ecmitalia.org/versione7/testi.tlb.gz"
                    };
                    listaFileDaAggiornare.Add(fileDaAggiornare);
                }

                if (versione.Major < 7 || (versione.Major == 7 && versione.Minor <= 9))
                {
                    FileDaAggiornare fileDaAggiornare = new FileDaAggiornare
                    {
                        nome = "",
                        nomeFile = Application.StartupPath + Path.DirectorySeparatorChar + "Light.exe",
                        url = "http://www.ecmitalia.org/versione7/Light.exe.gz"
                    };
                    listaFileDaAggiornare.Add(fileDaAggiornare);

                    FileDaAggiornare fileDaAggiornare2 = new FileDaAggiornare
                    {
                        nome = "",
                        nomeFile = Application.StartupPath + Path.DirectorySeparatorChar + "it" + Path.DirectorySeparatorChar + "Light.resources.dll",
                        url = "http://www.ecmitalia.org/versione7/Light.resources.dll.gz"
                    };
                    listaFileDaAggiornare.Add(fileDaAggiornare2);
                }

                if (versione.Major < 7 || (versione.Major == 7 && versione.Minor <= 16))
                {
                    FileDaAggiornare fileDaAggiornare = new FileDaAggiornare
                    {
                        nome = "",
                        nomeFile = Application.StartupPath + Path.DirectorySeparatorChar + "es" + Path.DirectorySeparatorChar + "LaParola.resources.dll",
                        url = "http://www.ecmitalia.org/versione7/LaParola.resources.es.dll.gz"
                    };
                    listaFileDaAggiornare.Add(fileDaAggiornare);

                    FileDaAggiornare fileDaAggiornare2 = new FileDaAggiornare
                    {
                        nome = "",
                        nomeFile = Application.StartupPath + Path.DirectorySeparatorChar + "es" + Path.DirectorySeparatorChar + "Light.resources.dll",
                        url = "http://www.ecmitalia.org/versione7/Light.resources.es.dll.gz"
                    };
                    listaFileDaAggiornare.Add(fileDaAggiornare2);
                    Directory.CreateDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "es" + Path.DirectorySeparatorChar);
                }

                XmlNodeList nodiDeiFile = xmlDocumento.SelectSingleNode("versioni").SelectNodes("file");
                foreach (XmlNode nodoDiFile in nodiDeiFile)
                {
                    FileDaAggiornare fileDaAggiornare = new FileDaAggiornare
                    {
                        nome = nodoDiFile.Attributes.GetNamedItem("nome").Value,
                        nomeFile = nodoDiFile.SelectSingleNode("nomeFile").InnerText,
                        url = nodoDiFile.SelectSingleNode("url").InnerText
                    };
                    listaFileDaAggiornare.Add(fileDaAggiornare);
                }
            }
            catch (Exception eccezione)
            {
                etiMessaggio.Text = string.Format(CultureInfo.CurrentCulture, risorseUpdate.GetString("ErroreNelFile"), eccezione.Message);
                senzaErrore = false;
                //File.Copy(nomeFileTemp, @"c:\laparola.txt");
            }
            finally
            {
                try
                {
                    File.Delete(nomeFileTemp);
                }
                catch
                {
                }
            }
            if (!senzaErrore)
                return;

            Process[] processiLaParola = Process.GetProcessesByName("laparola");
            bool programmaChiuso = true;

            foreach (Process processoLaParola in processiLaParola)
            {
                // non funziona su Windows 98/ME, perché UseShellExecute è vero (vedi Help su CloseMainWindow)
                if (Environment.OSVersion.Version.Major >= 5)
                    processoLaParola.CloseMainWindow();

                Thread.Sleep(2000);
                if (!processoLaParola.HasExited)
                    programmaChiuso = false;
            }

            if (!programmaChiuso)
            {
                etiMessaggio.Text = risorseUpdate.GetString("ProgrammaNonChiuso");
                pulOK.Visible = true;
                pulChiudi.Visible = false;
            }
            else
                aggiornareDopoLoad = true;
        }

        private void Principale_Shown(object sender, EventArgs e)
        {
            if (aggiornareDopoLoad)
                AggiornaFile();
        }

        private void pulChiudi_Click(object sender, EventArgs e)
        {
            ChiudiProgramma();
        }

        private void ChiudiProgramma()
        {
            string percorso = Application.StartupPath + Path.DirectorySeparatorChar + "LaParola.exe";
            try
            {
                if (isRunningOnMono)
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("mono", percorso)
                    {
                        UseShellExecute = false
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                else
                    System.Diagnostics.Process.Start(percorso);
            }
            catch (Win32Exception)
            {
                // file non esiste, basta chiudere questo programma senza aprire il programma principale
            }

            Close();
        }

        private void pulOK_Click(object sender, EventArgs e)
        {
            pulOK.Visible = false;
            pulChiudi.Visible = true;

            Process[] processiLaParola = Process.GetProcessesByName("laparola");
            bool programmaChiuso = true;
            foreach (Process processoLaParola in processiLaParola)
            {
                processoLaParola.CloseMainWindow();
                Thread.Sleep(1000);
                if (!processoLaParola.HasExited)
                    programmaChiuso = false;
            }

            if (!programmaChiuso)
            {
                etiMessaggio.Text = risorseUpdate.GetString("ProgrammaNonChiuso2");
            }
            else
                AggiornaFile();
        }

        private void AggiornaFile()
        {
            string testoChiudi = pulChiudi.Text;
            pulChiudi.Text = risorseUpdate.GetString("Annulla");
            Application.DoEvents();

            try
            {
                WebClient cliente = new WebClient();
                if (!string.IsNullOrEmpty(proxyHost))
                {
                    if (proxyPorta == 0)
                        cliente.Proxy = new WebProxy(proxyHost);
                    else
                        cliente.Proxy = new WebProxy(proxyHost, proxyPorta);
                    if (!string.IsNullOrEmpty(credentialUtente))
                    {
                        if (string.IsNullOrEmpty(credentialDominio))
                            cliente.Proxy.Credentials = new NetworkCredential(credentialUtente, credentialPassword);
                        else
                            cliente.Proxy.Credentials = new NetworkCredential(credentialUtente, credentialPassword, credentialDominio);
                    }
                }

                int numeroFileDaAggiornare = listaFileDaAggiornare.Count;
                int numeroComponentiDaAggiornare = numeroFileDaAggiornare;
                for (int i = 0; i < numeroFileDaAggiornare; ++i)
                    if (string.IsNullOrEmpty(listaFileDaAggiornare[i].nome))
                        --numeroComponentiDaAggiornare;
                int componentiAggiornati = 0, numeroCaratteri;
                string nomeFile;

                for (int i = 0; i < numeroFileDaAggiornare; ++i)
                {
                    if (!string.IsNullOrEmpty(listaFileDaAggiornare[i].nome))
                    {
                        etiMessaggio.Text = string.Format(CultureInfo.CurrentCulture, risorseUpdate.GetString("FileScaricando"), componentiAggiornati.ToString(CultureInfo.InvariantCulture), numeroComponentiDaAggiornare.ToString(CultureInfo.InvariantCulture), listaFileDaAggiornare[i].nome);
                        ++componentiAggiornati;
                        Application.DoEvents();
                    }

                    nomeFile = listaFileDaAggiornare[i].nomeFile;
                    if (!nomeFile.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) && File.Exists(nomeFile))
                        File.Delete(nomeFile);
                    Application.DoEvents();

                    numeroCaratteri = listaFileDaAggiornare[i].url.Length;
                    StringBuilder fileDaScaricareEncoded = new StringBuilder("", 2 * numeroCaratteri);
                    for (int j = 0; j < numeroCaratteri; ++j)
                    {
                        if (listaFileDaAggiornare[i].url[j] >= 256)
                        {
                            // un carattere unicode che non viene tradotto (e così il file non è scaricato)
                        }
                        else if (listaFileDaAggiornare[i].url[j] >= 128)
                            fileDaScaricareEncoded.Append("%" + Uri.HexEscape(listaFileDaAggiornare[i].url[j]).Substring(1));
                        else
                            fileDaScaricareEncoded.Append(listaFileDaAggiornare[i].url[j]);
                    }
                    cliente.DownloadFile(new Uri(fileDaScaricareEncoded.ToString()), nomeFile);
                    Application.DoEvents();

                    if (listaFileDaAggiornare[i].url.EndsWith(".lptar", StringComparison.OrdinalIgnoreCase))
                    {
                        FileStream inFile = new FileStream(nomeFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        BinaryReader br = new BinaryReader(inFile);
                        int numeroFile = br.ReadInt32(), numeroByte;
                        string nomeFileTarato;
                        string nomeDirectory = Path.GetDirectoryName(nomeFile);
                        if (!nomeDirectory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                            nomeDirectory += Path.DirectorySeparatorChar;
                        nomeDirectory += Path.GetFileNameWithoutExtension(nomeFile) + Path.DirectorySeparatorChar;
                        Directory.CreateDirectory(nomeDirectory);
                        for (int j = 0; j < numeroFile; ++j)
                        {
                            nomeFileTarato = br.ReadString();
                            numeroByte = br.ReadInt32();
                            byte[] byteFile = new byte[numeroByte];
                            byteFile = br.ReadBytes(numeroByte);
                            File.WriteAllBytes(nomeDirectory + nomeFileTarato, byteFile);
                            Application.DoEvents();
                        }
                        br.Close();
                        inFile.Close();
                        File.Delete(nomeFile);
                    }
                    else if (listaFileDaAggiornare[i].url.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                    {
                        // Open the file as a FileStream object.
                        FileStream inFile = new FileStream(nomeFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Application.DoEvents();
                        byte[] bufferIn = new byte[inFile.Length];
                        inFile.Read(bufferIn, 0, bufferIn.Length);
                        inFile.Close();
                        Application.DoEvents();
                        // a volte lo scaricamento scompatta anche automaticamente il file
                        // in questi casi (quando i primi due caratteri non sono il "numero magico") non dobbiamo fare niente
                        if (bufferIn[0] == 31 && bufferIn[1] == 139)
                        {
                            MemoryStream streamCompresso = new MemoryStream();
                            streamCompresso.Write(bufferIn, 0, bufferIn.Length);
                            // Use the newly created memory stream for the compressed data.
                            streamCompresso.Position = 0;
                            GZipStream streamNonCompresso = new GZipStream(streamCompresso, CompressionMode.Decompress);
                            Application.DoEvents();
                            FileStream outFile = new FileStream(nomeFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            Application.DoEvents();

                            byte[] bufferOut = new byte[bufferIn.Length];
                            int bytesLetti;
                            while (true)
                            {
                                bytesLetti = streamNonCompresso.Read(bufferOut, 0, bufferOut.Length);
                                if (bytesLetti == 0)
                                    break;
                                outFile.Write(bufferOut, 0, bytesLetti);
                            }
                            Application.DoEvents();
                            streamNonCompresso.Close();
                            outFile.Close();
                            streamCompresso.Close();
                        }
                    }
                }
            }
            catch (Exception eccezione)
            {
                etiMessaggio.Text = string.Format(CultureInfo.CurrentCulture, risorseUpdate.GetString("ErroreNelloScaricamento"), eccezione.Message);
                pulChiudi.Text = testoChiudi;
                return;
            }

            ChiudiProgramma();
        }

    }

    struct FileDaAggiornare
    {
        public string nome;
        public string url;
        public string nomeFile;
    }

}