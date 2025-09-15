using TestiBiblici;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System;

namespace TestProject
{

    /// <summary>
    ///This is a test class for TextsTest and is intended
    ///to contain all TextsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TextsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for EsistonoRadici
        ///</summary>
        [TestMethod()]
        public void EsistonoRadiciTest()
        {
            Texts target = new Texts();
            string nomeVersione = "Nuova Riveduta";
            bool expected = true;
            bool actual;
            actual = target.EsistonoRadici(nomeVersione);
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(false, target.EsistonoRadici("Albanian Translation"));
        }

        /// <summary>
        ///A test for RadiceDiParola
        ///</summary>
        [TestMethod()]
        public void RadiceDiParolaTest()
        {
            Texts target = new Texts();
            string parola = "va";
            string nomeVersione = "Nuova Riveduta";
            string expected = "andare";
            string actual;
            actual = target.RadiceDiParola(parola, nomeVersione);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RadiceDiParola
        ///</summary>
        [TestMethod()]
        public void RadiceDiParolaTest2()
        {
            Texts target = new Texts();
            string parola = "ἦ";
            string nomeVersione = "Septuagint";
            string expected = "εἰ";
            string actual;
            actual = target.RadiceDiParola(parola, nomeVersione);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ParoleDiRadice
        ///</summary>
        [TestMethod()]
        public void ParoleDiRadiceTest()
        {
            Texts target = new Texts();
            string radice = "abbaiare";
            string nomeVersione = "Nuova Riveduta";
            Collection<string> expected = new Collection<string>();
            expected.Add("abbaiare");
            expected.Add("abbaierà");
            Collection<string> actual;
            actual = target.ParoleDiRadice(radice, nomeVersione);
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);

            Assert.AreEqual(target.ParoleDiRadice("fare", "Nuova Riveduta").Count, 94);
        }

        /// <summary>
        ///A test for VersioneEsiste
        ///</summary>
        [TestMethod()]
        public void VersioneEsisteTest()
        {
            Texts target = new Texts();
            string nomeVersione = "Nuova Riveduta";
            bool expected = true;
            bool actual;
            actual = target.VersioneEsiste(nomeVersione);
            Assert.AreEqual(expected, actual);

            nomeVersione = "sdf";
            expected = false;
            actual = target.VersioneEsiste(nomeVersione);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Info
        ///</summary>
        [TestMethod()]
        public void InfoTest()
        {
            Texts target = new Texts();
            string nomeVersione = "Nuova Riveduta";
            TestoTipi expected = TestoTipi.Bibbia;
            VersioneInformazioni actual;
            actual = target.Info(nomeVersione);
            Assert.AreEqual(expected, actual.Tipo);

            nomeVersione = "Note della Nuova Riveduta";
            expected = TestoTipi.Commentario | TestoTipi.Dizionario;
            actual = target.Info(nomeVersione);
            Assert.AreEqual(expected, actual.Tipo);
        }

        /// <summary>
        ///A test for Ricerca
        ///</summary>
        [TestMethod()]
        public void RicercaTest()
        {
            Texts target = new Texts();
            string[] libriAbbreviazioniUsate = Texts.LibriAbbreviazioniUsateItaliano.Split('|');
            for (byte i = 1; i <= 73; ++i)
            {
                target.SetLibroAbbreviazioneUsata(i, libriAbbreviazioniUsate[i]);
            }

            string espressione = "abbà";
            string nomeVersione = "Nuova Riveduta";
            Riferimento expected = new Riferimento();
            Collection<ushort> lista = new Collection<UInt16>();
            lista.Add(2);
            expected.AggiungiBranoEParole(new byte[] { 48, 14, 36, 48, 14, 36 }, lista);
            lista.Clear();
            lista.Add(25);
            expected.AggiungiBranoEParole(new byte[] { 52, 8, 15, 52, 8, 15 }, lista);
            lista.Clear();
            lista.Add(18);
            expected.AggiungiBranoEParole(new byte[] { 55, 4, 6, 55, 4, 6 }, lista);

            Riferimento actual;
            actual = target.Ricerca(espressione, nomeVersione);
            Assert.AreEqual(actual.Uguale(expected), true);
            /*Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; ++i)
            {
                for (int j = 0; j < 6; ++j)
                    Assert.AreEqual(expected.Brani[i][j], actual.Brani[i][j]);
                Assert.AreEqual(expected.Uguale
            }*/

            // nota: il seguente test funziona solo se la lingua dei libri è italiano
            string actual2 = target.TestoBrano(actual, nomeVersione);
            string expected2 = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040{\\fonttbl{\\f0\\fnil\\fcharset0 Palatino Linotype;}}\r\n{\\colortbl ;\\red0\\green0\\blue0;}\r\n\\viewkind4\\uc1\\pard\\cf1\\v\\f0\\fs24\\'0148014036\\b\\v0 Mc 14:36\\b0  Diceva: \\'ab\\v\\'0e\\ul\\v0 Abb\\'e0\\ulnone , Padre! Ogni cosa ti \\'e8 possibile; allontana da me questo calice! Per\\'f2, non quello che io voglio, ma quello che tu vuoi\\'bb.\\par\r\n\\par\r\n\\v\\'0152008015\\b\\v0 Rm 8:15\\b0  E voi non avete ricevuto uno spirito di servit\\'f9 per ricadere nella paura, ma avete ricevuto lo Spirito di adozione, mediante il quale gridiamo: \\'ab\\v\\'0e\\ul\\v0 Abb\\'e0\\ulnone ! Padre!\\'bb\\par\r\n\\par\r\n\\v\\'0155004006\\b\\v0 Gal 4:6\\b0  E, perch\\'e9 siete figli, Dio ha mandato lo Spirito del Figlio suo nei nostri cuori, che grida: \\'ab\\i\\v\\'0e\\ul\\v0 Abb\\'e0\\ulnone\\i0 , Padre\\'bb.\\par\r\n}\r\n";
            Assert.AreEqual(expected2, actual2);
        }

        /// <summary>
        ///A test for NumeroVolteRadice
        ///</summary>
        [TestMethod()]
        public void NumeroVolteRadiceTest()
        {
            Texts target = new Texts();
            string radice = "fare";
            string nomeVersione = "Nuova Riveduta";
            int expected = 6360;
            int actual;
            actual = target.NumeroVolteRadice(radice, nomeVersione);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CapitoliInLibro
        ///</summary>
        [TestMethod()]
        public void CapitoliInLibroTest()
        {
            Texts target = new Texts();
            byte libro = 1;
            string nomeVersione = "Nuova Riveduta";
            byte expected = 50;
            byte actual;
            actual = target.CapitoliInLibro(libro, nomeVersione);
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(0, target.CapitoliInLibro(libro, "Tischendorf"));
        }

        /// <summary>
        ///A test for VersettiInCapitolo
        ///</summary>
        [TestMethod()]
        public void VersettiInCapitoloTest()
        {
            Texts target = new Texts();
            byte libro = 1;
            byte capitolo = 1;
            string nomeVersione = "Nuova Riveduta";
            byte expected = 31;
            byte actual;
            actual = target.VersettiInCapitolo(libro, capitolo, nomeVersione);
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(0, target.VersettiInCapitolo(libro, capitolo, "Tischendorf"));
        }

        /// <summary>
        ///A test for CapitoliFinoALibro
        ///</summary>
        [TestMethod()]
        public void CapitoliFinoALibroTest()
        {
            Texts target = new Texts();
            byte libro = 73;
            string nomeVersione = "C.E.I.";
            ushort expected = 1328;
            ushort actual;
            actual = target.CapitoliFinoALibro(libro, nomeVersione);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for LibroDiCapitolo
        ///</summary>
        [TestMethod()]
        public void LibroDiCapitoloTest()
        {
            Texts target = new Texts();
            int capitolo = 52;
            string nomeVersione = "Nuova Riveduta";
            byte expected = 2;
            byte actual;
            actual = target.LibroDiCapitolo(capitolo, nomeVersione);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for NormalizzaRiferimento
        ///</summary>
        [TestMethod()]
        public void NormalizzaRiferimentoTest1()
        {
            Texts target = new Texts();
            int libro = 1;
            int capitolo = 2;
            int versetto = 3;
            string expected = "Gen 2:3";
            string actual;
            actual = target.NormalizzaRiferimento(libro, capitolo, versetto);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for NormalizzaRiferimento
        ///</summary>
        [TestMethod()]
        public void NormalizzaRiferimentoTest()
        {
            Texts target = new Texts();
            string[] libriAbbreviazioniUsate = Texts.LibriAbbreviazioniUsateItaliano.Split('|');
            for (byte i = 1; i <= 73; ++i)
            {
                target.SetLibroAbbreviazioneUsata(i, libriAbbreviazioniUsate[i]);
            }

            string riferimento = "gn 2:3,ro4:1-2";
            string expected = "Gen 2:3; Rm 4:1-2";
            string actual;
            actual = target.NormalizzaRiferimento(riferimento);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertiTitoloNotaARiferimento
        ///</summary>
        [TestMethod()]
        public void ConvertiTitoloNotaARiferimentoTest()
        {
            Texts target = new Texts();
            string notaDaConvertire = "#010020030004-010050060007";
            string expected = "Gen 2:3/4-5:6/7";
            string actual;
            actual = target.ConvertiTitoloNotaARiferimento(notaDaConvertire);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertiRiferimentoDa3Numeri
        ///</summary>
        [TestMethod()]
        public void ConvertiRiferimentoDa3NumeriTest()
        {
            Texts target = new Texts();
            string[] libriNomi = Texts.LibriNomiItaliano.Split('|');
            for (byte i = 1; i <= 73; ++i)
            {
                target.SetLibroNome(i, libriNomi[i]);
            }

            string riferimentoDaConvertire = "1 28:14; 4 24:17";
            string expected = "Genesi 28:14; Numeri 24:17";
            string actual;
            actual = target.ConvertiRiferimentoDa3Numeri(riferimentoDaConvertire);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Radici
        ///</summary>
        [TestMethod()]
        public void RadiciTest()
        {
            Texts target = new Texts();
            string nomeVersione = "Nuova Riveduta";
            //            string[] expected = null;
            string[] actual;
            actual = target.Radici(nomeVersione);
            Assert.AreEqual("*", actual[0]);
            Assert.AreEqual("a", actual[1]);
            Assert.AreEqual("Aara", actual[2]);
        }

        /// <summary>
        ///A test for Parole
        ///</summary>
        [TestMethod()]
        public void ParoleTest()
        {
            Texts target = new Texts();
            string nomeVersione = "Nuova Riveduta";
            //string[] expected = null;
            string[] actual;
            actual = target.Parole(nomeVersione);
            Assert.AreEqual("a", actual[0]);
            Assert.AreEqual("aara", actual[1]);
            Assert.AreEqual("aarel", actual[2]);
            Assert.AreEqual(27385, actual.Length);
        }

        /// <summary>
        ///A test for GetParoleRadici
        ///</summary>
        [TestMethod()]
        public void GetParoleRadiciTest()
        {
            Texts target = new Texts();
            string nomeVersione = "Nuova Riveduta";
            //            Collection<string> expected = null; // TODO: Initialize to an appropriate value
            Collection<string> actual;
            actual = target.GetParoleRadici(nomeVersione);
            Assert.AreEqual("a=a", actual[0]);
            Assert.AreEqual("aara=Aara", actual[1]);
            Assert.AreEqual("abbà=abbà", actual[15]);
            Assert.AreEqual(27385, actual.Count);
        }

        /// <summary>
        ///A test for ConvertiDaStandard
        ///</summary>
        [TestMethod()]
        public void ConvertiDaStandardTest()
        {
            Texts target = new Texts();
            Riferimento riferimento = new Riferimento(23, 3, 3);
            string nomeVersione = "Nuova Riveduta";
            Riferimento expected = new Riferimento(23, 3, 2);
            Riferimento actual;
            actual = target.ConvertiDaStandard(riferimento, nomeVersione);
            for (int i = 0; i < 6; ++i)
                Assert.AreEqual(expected.Brani[0][i], actual.Brani[0][i]);
        }

        /// <summary>
        ///A test for ConvertiAStandard
        ///</summary>
        [TestMethod()]
        public void ConvertiAStandardTest()
        {
            Texts target = new Texts();
            Riferimento riferimento = new Riferimento(46, 3, 24);
            string nomeVersione = "C.E.I.";
            Riferimento expected = new Riferimento(46, 4, 6);
            Riferimento actual;
            actual = target.ConvertiAStandard(riferimento, nomeVersione);
            for (int i = 0; i < 6; ++i)
                Assert.AreEqual(expected.Brani[0][i], actual.Brani[0][i]);
        }

        /// <summary>
        ///A test for EsistonoCitazioni
        ///</summary>
        [TestMethod()]
        public void EsistonoCitazioniTest()
        {
            Texts target = new Texts();
            string nomeVersione = "ASV Footnotes";
            bool expected = true;
            bool actual;
            actual = target.EsistonoCitazioni(nomeVersione);
            Assert.AreEqual(expected, actual);
            nomeVersione = "King James Version Footnotes";
            expected = false;
            actual = target.EsistonoCitazioni(nomeVersione);
            Assert.AreEqual(expected, actual);
        }
    }
}
