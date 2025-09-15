using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;

namespace LaParola
{
    public partial class Quiz : Template
    {
        private Principale genitore;
        //string[] categorie;
        string[] domande;
        Random randObj;
        private int risposta;
        private string spiegazione, domanda, rispostaGiusta;
        int giustoOggi = 0, sbagliatoOggi = 0, giustoSempre, sbagliatoSempre;

        public Quiz(Principale formGenitore)
        {
            InitializeComponent();
            genitore = formGenitore;
        }

        private void Quiz_Load(object sender, EventArgs e)
        {
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            tbRisposta.Rtf = "{\\rtf1\\fs20 " + Principale.LocRM.GetString("QuizInstructions") + "}";
            //categorie = LaParola.Properties.Resources.quiztemi.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            domande = LaParola.Properties.Resources.quiz.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            randObj = new Random();
            giustoSempre = Settings.Default.QuizGiusto;
            sbagliatoSempre = Settings.Default.QuizSbagliato;
            NuovaDomanda();
        }

        private void NuovaDomanda()
        {
            int n = randObj.Next(domande.Length);
            string s = domande[n];
            int categoria = GetNumero(ref s);
            domanda = GetString(ref s);
            string risposta1 = GetString(ref s);
            string risposta2 = GetString(ref s);
            string risposta3 = GetString(ref s);
            string risposta4 = GetString(ref s);
            risposta = GetNumero(ref s);
            spiegazione = GetString(ref s);
            // "Nel quinto giorno";"Nel quarto giorno";"Nel settimo giorno";"Nel sesto giorno";4;"Infatti è Scritto: Poi Dio disse: ""Facciamo l'uomo a nostra immagine, conforme alla nostra somiglianza, e abbia dominio sui pesci del mare, sugli uccelli del cielo, sul bestiame, su tutta la terra e su tutti i rettili che strisciano sulla terra"". Dio creò l'uomo a sua immagine; lo creò a immagine di Dio; li creò maschio e femmina. Genesi 1:26-27,31";

            tbDomanda.Text = domanda;
            rbRis1.Text = "&1. " + risposta1;
            rbRis2.Text = "&2. " + risposta2;
            rbRis3.Text = "&3. " + risposta3;
            rbRis4.Text = "&4. " + risposta4;

            switch (risposta)
            {
                case 1:
                    rispostaGiusta = risposta1;
                    break;
                case 2:
                    rispostaGiusta = risposta2;
                    break;
                case 3:
                    rispostaGiusta = risposta3;
                    break;
                case 4:
                    rispostaGiusta = risposta4;
                    break;
            }
        }

        private int GetNumero(ref string s)
        {
            int p = s.IndexOf(";");
            int numero = int.Parse(s.Substring(0, p));
            s = s.Substring(p + 1);
            return numero;
        }

        private string GetString(ref string s)
        {
            int p = s.StartsWith("\"\"\"") ? s.IndexOf("\"\"\"", 2) : s.IndexOf("\";", 2);
            string testo = TogliVirgolette(s.Substring(0, p));
            s = s.Substring(s.IndexOf(";", p) + 1);
            return testo;
        }

        private string TogliVirgolette(string p)
        {
            while (p.StartsWith("\""))
                p = p.Remove(0, 1);
            while (p.EndsWith("\""))
                p = p.Remove(p.Length - 1);
            return p.Replace("\"\"", "\"");
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbRis_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                int utente = Int16.Parse(((Control)sender).Tag.ToString());
                etiGiustoSbagliato.Text = (utente == risposta ? Principale.LocRM.GetString("QuizRightAnswer") : Principale.LocRM.GetString("QuizWrongAnswer"));
                if (utente == risposta)
                {
                    giustoOggi += 1;
                    giustoSempre += 1;
                    Settings.Default.QuizGiusto = giustoSempre;
                }
                else
                {
                    sbagliatoOggi += 1;
                    sbagliatoSempre += 1;
                    Settings.Default.QuizSbagliato = sbagliatoSempre;
                }
                string messaggio = "{\\rtf {\\b " + Principale.LocRM.GetString("QuizQuestion") + ":} " + domanda + "\\par {\\b " + Principale.LocRM.GetString("QuizAnswer") + ":} " + rispostaGiusta + "\\par\\par " + spiegazione;
                messaggio += "\\par\\par {\\b " + Principale.LocRM.GetString("QuizScore") + ":} " + Principale.LocRM.GetString("QuizRight") + ": " + giustoOggi + " " + Principale.LocRM.GetString("QuizWrong") + ": " + sbagliatoOggi + " (" + perc(giustoOggi, sbagliatoOggi) + "%)";
                messaggio += "\\par {\\b " + Principale.LocRM.GetString("QuizAlways") + ":} " + Principale.LocRM.GetString("QuizRight") + ": " + giustoSempre + " " + Principale.LocRM.GetString("QuizWrong") + ": " + sbagliatoSempre + " (" + perc(giustoSempre, sbagliatoSempre) + "%)";
                tbRisposta.Clear();
                tbRisposta.Rtf = messaggio;
                NuovaDomanda();
                rbRis1.Checked = false;
                rbRis2.Checked = false;
                rbRis3.Checked = false;
                rbRis4.Checked = false;
            }
        }

        private string perc(int g, int s)
        {
            return g + s == 0 ? "100" : Math.Round(100.0 * g / (g + s), 0, MidpointRounding.AwayFromZero).ToString();
        }
    }
}
