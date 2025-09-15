using System;
using System.IO;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Esporta : Template
    {
        private int tipoBibbiaPreferitoTesto = 0;
        private int tipoCollPreferitoTesto = 0;
        private readonly Principale genitore;

        public Esporta(Principale formGenitore)
        {
            InitializeComponent();

            genitore = formGenitore ?? throw new ArgumentNullException("formGenitore");
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            btnCanc.Text = Principale.LocRM.GetString("MiscClose");
        }

        private void Esporta_Load(object sender, System.EventArgs e)
        {
            string versionePrecedente = Settings.Default.EsportaVersione;

            switch (Settings.Default.EsportaTipoBibbia)
            {
                case 1:
                    rbBibbiaFileOsis.Checked = true;
                    break;
                case 2:
                    rbBibbiaFileZefania.Checked = true;
                    break;
                case 3:
                    rbBibbiaFileTesto.Checked = true;
                    break;
                default:
                    rbBibbiaFileJava.Checked = true;
                    break;
            }
            switch (Settings.Default.EsportaTipoCollezione)
            {
                case 1:
                    rbCollMultiFile.Checked = true;
                    break;
                case 2:
                    rbCollUnicoFile.Checked = true;
                    break;
                default:
                    rbCollJavaFile.Checked = true;
                    break;
            }

            cbVersione.BeginUpdate();
            foreach (string s in Principale.testi.NomiVersioni())
            {
                cbVersione.Items.Add(s);
                if (s == versionePrecedente)
                    cbVersione.SelectedIndex = cbVersione.Items.Count - 1;
            }

            if (cbVersione.Items.Count > 0)
            {
                if (cbVersione.SelectedIndex < 0)
                    cbVersione.SelectedIndex = 0;
            }
            else
            {
                btnOK.Visible = false;
            }
            cbVersione.EndUpdate();

            tbDirectory.Text = Settings.Default.EsportaDirectory;
            if (string.IsNullOrEmpty(tbDirectory.Text))
                tbDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
        }

        private void Esporta_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.WindowsShutDown) && (e.CloseReason != CloseReason.TaskManagerClosing))
            {
                Settings.Default.EsportaVersione = cbVersione.SelectedItem.ToString();
                Settings.Default.EsportaTipoBibbia = tipoBibbiaPreferitoTesto;
                Settings.Default.EsportaTipoCollezione = tipoCollPreferitoTesto;
                Settings.Default.EsportaDirectory = tbDirectory.Text;
            }
        }

        private void cbVersione_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string nomeVersione = cbVersione.SelectedItem.ToString();

            TestoTipi tipo = Principale.testi.Info(nomeVersione).Tipo;
            bool tipoBibbia = ((tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia);
            rbBibbiaFileTesto.Visible = tipoBibbia;
            rbBibbiaFileOsis.Visible = tipoBibbia;
            rbBibbiaFileZefania.Visible = tipoBibbia;
            rbBibbiaFileJava.Visible = tipoBibbia;
            rbCollMultiFile.Visible = !tipoBibbia;
            rbCollUnicoFile.Visible = !tipoBibbia;
            rbCollJavaFile.Visible = !tipoBibbia;

            if (!rbBibbiaFileTesto.Visible && (rbBibbiaFileTesto.Checked || rbBibbiaFileOsis.Checked || rbBibbiaFileZefania.Checked || rbBibbiaFileJava.Checked))
            {
                switch (tipoCollPreferitoTesto)
                {
                    case 1:
                        rbCollMultiFile.Checked = true;
                        break;
                    case 2:
                        rbCollUnicoFile.Checked = true;
                        break;
                    default:
                        rbCollJavaFile.Checked = true;
                        break;
                }
            }
            if (!rbCollMultiFile.Visible && (rbCollMultiFile.Checked || rbCollUnicoFile.Checked || rbCollJavaFile.Checked))
            {
                switch (tipoBibbiaPreferitoTesto)
                {
                    case 1:
                        rbBibbiaFileOsis.Checked = true;
                        break;
                    case 2:
                        rbBibbiaFileZefania.Checked = true;
                        break;
                    case 3:
                        rbBibbiaFileTesto.Checked = true;
                        break;
                    default:
                        rbBibbiaFileJava.Checked = true;
                        break;
                }
            }
        }

        private void rb_CheckedChanged(object sender, System.EventArgs e)
        {
            if (sender == rbBibbiaFileTesto)
                tipoBibbiaPreferitoTesto = 0;
            else if (sender == rbBibbiaFileOsis)
                tipoBibbiaPreferitoTesto = 1;
            else if (sender == rbBibbiaFileZefania)
                tipoBibbiaPreferitoTesto = 2;
            else if (sender == rbBibbiaFileJava)
                tipoBibbiaPreferitoTesto = 3;
            else if (sender == rbCollMultiFile)
                tipoCollPreferitoTesto = 0;
            else if (sender == rbCollUnicoFile)
                tipoCollPreferitoTesto = 1;
            else if (sender == rbCollJavaFile)
                tipoCollPreferitoTesto = 2;

            etiDirectory.Visible = (sender != rbCollUnicoFile);
            tbDirectory.Visible = etiDirectory.Visible;
            pulSfoglia.Visible = etiDirectory.Visible;
        }

        private void pulSfoglia_Click(object sender, EventArgs e)
        {
            string directory = tbDirectory.Text;
            if (!Directory.Exists(directory))
            {
                directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(directory);
            }

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = directory;
                folderBrowserDialog.ShowNewFolderButton = false;
                folderBrowserDialog.Description = Principale.LocRM.GetString("ImportDirectoryDescription");
                if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                    tbDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            string nomeVersione = cbVersione.SelectedItem.ToString();

            string directoryBase = tbDirectory.Text;
            if (!string.IsNullOrEmpty(directoryBase))
            {
                if (directoryBase[directoryBase.Length - 1] != Path.DirectorySeparatorChar)
                    directoryBase += Path.DirectorySeparatorChar;
            }
            else
                directoryBase = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
            try
            {
                Directory.CreateDirectory(directoryBase);
            }
            catch
            {
                directoryBase = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(directoryBase);
            }

            EsportoTestoTipo tipo;
            if (rbBibbiaFileTesto.Checked)
                tipo = EsportoTestoTipo.BibbiaFile;
            else if (rbBibbiaFileOsis.Checked)
                tipo = EsportoTestoTipo.BibbiaOsis;
            else if (rbBibbiaFileZefania.Checked)
                tipo = EsportoTestoTipo.BibbiaZefania;
            else if (rbBibbiaFileJava.Checked)
                tipo = EsportoTestoTipo.BibbiaJava;
            else if (rbCollMultiFile.Checked)
                tipo = EsportoTestoTipo.CollezioneFile;
            else if (rbCollUnicoFile.Checked)
                tipo = EsportoTestoTipo.CollezioneUnico;
            else
                tipo = EsportoTestoTipo.CollezioneJava;

            Funzioni.EsportaTesto(genitore, tipo, nomeVersione, directoryBase);
        }

        private void btnCanc_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

    }
}
