<?
$descriz = "Programmi della Bibbia per Windows";
$key = "Windows";
$titolo = "Windows";
$sezione = "Programma";
require("../capo.php");
?>
<h2>App per Windows - versione 8 (beta)</h2>

<p>Sto riscrivendo l'app per Window. L'app attuale ha quasi 20 anni, e si vede.
La nuova versione ha un'interfaccia moderna e anche usa tecnologie nuove.
Metter&ograve; diversi mesi per completare l'app, e ogni qualche settimana pubblicher&ograve;
qui una nuova versione da provare. Se trovate dei problemi, fatemi sapere affinch&eacute;
li possa correggere nella prossima versione beta.<p>

<h2>Scaricamento e installazione</h2>

<p>Bisogna scaricare l'app beta in una qualsiasi cartella.
Non serve l'installazione; basta avviare il file scaricato.<br />
Ci sono due versioni dell'app da cui scegliere:<br />
1. Versione autonoma da <a href="LaParola0.exe">scaricare qui</a>; circa 150Mb. Non ha bisogno di certi componenti facoltativi di Windows<br />
2. Versione dipendente da <a href="LaParola.exe">scaricare qui</a>; circa 5Mb. Possibilmente (dipende dalle app che hai gi&agrave; installato) la prima volta che avvi l'app ti dir&agrave; che devi prima installare
.NET Desktop Runtime versione 8; in quel caso segui le istruzioni per scaricare e avviare il programma per installare il componente.</p>

<p>L'app beta trova tutte le versioni installate dalla versione 7 dell'app nella cartella
c:\users\&lt;nome utente&gt;\AppData\Roaming\LaParola .
Se non hai la versione 7 installata o le versioni della Bibbia non sono installate in quella cartella,
bisogna scaricare e scompattare i file desiderati da <a href="/program/addins.php">https://www.laparola.net/program/addins.php</a>
e mettere i file con estensione .laparola nella stessa cartella dell'app beta.</p>

<p> L'installazione sar&agrave; migliorata in una versione futura della versione beta.</p>

<h2>Caratteristiche</h2>

<h3>Versione 8.0.0.0</h3>
<ul>
<li><i>Mostra brano:</i> digita un riferimento e scegli le versioni da visualizzare
(solo la Bibbia, non ancora i commentari).</li>
<li><i>Convertitore misure:</i> converte varie unit&agrave; di misura nella Bibbia.</li>
<li><i>Opzioni:</i> lingua dell'interfaccia, modalit&agrave; notte/giorno.</li>
<li>Modalit&agrave; notte (non su Windows 10).</li>
<li>Testo ebraico visualizzato bene. Una conseguenza dell'utilizzo delle nuove tecnologie &egrave; che &egrave; possibile visualizzare il testo ebraico un versetto dopo l'altro e che altro testo in caratteri latini pu&ograve; essere inserito.
Per esempio, il nuovo testo <a href="Westminster Leningrad Codex morphological.laparola">Westminster Leningrad Codex morphological</a> mostra un codice per la morfologia dopo ogni parola, e quindi non pu&ograve; essere usato nella versione 7 dell'app.
Per provarlo, puoi scaricare quel file e copiarlo alla stessa cartella del programma (versione 8), poi usare il comando "Mostra brano" per visualizzarne il testo.
I codici sono spiegati al sito di <a href="https://hb.openscriptures.org/parsing/HebrewMorphologyCodes.html">Open Scriptures</a>.</li>
<li>App portabile: pu&ograve; essere avviata da una chiavetta
(con i file dei testi sulla stessa chiavetta). Bisognerebbe usare la versione autonoma in questo caso, se non puoi installare il componente facoltativo di Windows sul computer.</li>
<li>La maggior parte delle finestre hanno un punto interrogativo in alto a destra,
che quando cliccato d&agrave; indicazioni sulla finestra.
Lasciare il mouse sopra un componente di una finestra spesso d&agrave; altre informazioni.
Ci sar&agrave; un migliore sistema di aiuto in una versione futura.</li> 
</ul>

<a name="source"></a>
<h2>Codice sorgente</h2>

<p>Per i programmatori, il codice sorgente &egrave; disponibile su <a href="https://github.com/LPN6/LaParola/tree/main/Codice%20sorgente">Github</a>.
Per leggere il codice, puoi usare l'ambiente di sviluppo gratuito
<a href="https://visualstudio.microsoft.com/vs/">Visual C# Express</a>.</p>

<h2>Versioni future</h2>

<p>Per essere informato di aggiornamenti al programma e le nuove versioni beta, puoi:</p>
<ul>
<li>iscriverti alla <a href="/mailing_list.php">mailing list</a> del programma (messaggi non frequenti, solo per cambiamenti grandi); oppure</li>
<li>controllare periodicamente la <a href="/novita.php">pagina delle novit&agrave;</a> oppure controllare il <a href="/novita.php">feed XML</a> del sito; oppure</li>
<li>seguire la <a href="http://www.facebook.com/pages/LaParola/114249198597738">pagina del programma su Facebook</a>.</li> 
</ul>

<?
require("../piede.php");
?>
