<?
$descriz = "Programmi della Bibbia per Windows";
$key = "Windows";
$titolo = "Windows";
$sezione = "Programma";
require("../capo.php");
?>
<h1>Windows</h1>
<h2>La versione 7.20.7</h2>

<h2>Scaricamento e installazione</h2>

<p>Bisogna <a href="/file/laparola-it.exe">scaricare il programma
della Bibbia</a>. &Egrave; un programma di installazione; basta avviare il file scaricato e rispondere alle domande.</p>

<!--<p>Questo programma richiede la versione 2.0 o dopo del .NET framework.<br />
Windows Vista e dopo lo contiene gi&agrave;.<br />
Per le versioni precedenti di Windows, il modo pi&ugrave; facile per ottenere il .NET framework &egrave; con <i>Microsoft Update</i> in Windows; altrimenti, pu&ograve; essere scaricato dal sito della
<a href="http://www.microsoft.com/downloads/details.aspx?familyid=0A391ABD-25C1-4FC0-919F-B21F31AB88B7&displaylang=it">Microsoft</a>.
Per&ograve; c'&egrave; un'incompatibilit&agrave; fra la versione 2.0 di .NET e le versioni pi&ugrave; recenti, e il programma potrebbe dare un errore quando avviato. In quel caso, scarica <a href="/file/NETprob.zip">questo file</a>, scompattalo, e metti i tre file che contiene nella stessa cartella del programma installato.
</p>-->

<!--<p>Quando il .NET framework &egrave; installato, si pu&ograve; <a href="/file/laparola-it.exe">scaricare il programma
della Bibbia</a>. &Egrave; un programma di installazione; basta avviare il file scaricato e rispondere alle domande.</p>
-->

<p>Il programma viene poi avviato scegliendolo dal menu <i>Start</i> di Windows<!--  in fondo a sinistra dello schermo, o in Windows 8 digitando <i>LaParola</i>-->.</p>

<p><b>Nota:</b> A volte quando si cerca di avviare il programma di installazione, c'&egrave; un messaggio "Microsoft Defendere SmartScreen ha impedito l'avvio di un'app non riconosciuta".
Questo non &egrave; perch&eacute; un virus o altro malware sono stati rilevati, ma perch&eacute; (secondo Microsoft) il programma non &egrave; stato scaricato abbastanza spesso
per dimostrare che &egrave; sicuro. Il programma pu&ograve; essere installato senza problemi, cliccando "Ulteriori informazioni" e poi il pulsante "Esegui comunque".</p>

<p><b>Nota:</b> Un modo alternativo di ottenere il programma e tutti i testi disponibili (invece di usare l'aggiornamento automatico descritto qui sotto)
&egrave; di scaricare il <a href="/file/laparola.exe">programma di installazione che contiene tutti i testi</a>, con cui puoi scegliere quali componenti installare. <b>Attenzione:</b> il file &egrave; di pi&ugrave; di 850Mb!.</p>

<p><b>Nota:</b> A volte durante l'installazione c'&egrave; un messaggio di errore che il framework .NET non &egrave; installato. Il messaggio pu&ograve; essere ignorato; il programma funzioner&agrave; comunque.</p>

<p><b>Nota:</b> Quando il programma viene avviato, il <i>Controllo dell'account utente</i> di Windows chieder&agrave; se vuoi consentire all'app di apportare modifiche al dispositivo.
Questo &egrave; necessario per permettere al programma di aggiornarsi e installare altri testi.
Se vuoi togliere il messaggio per <i>LaParola</i>, puoi creare un file batch (cio&egrave;, un file di testo ma con estensione .bat) con il seguente testo:<br />
<code>
Set ApplicationPath="C:\Programmi\LaParola\LaParola.exe"<br />
cmd /min /C "set __COMPAT_LAYER=RUNASINVOKER && start "" %ApplicationPath%"
</code><br />
dove metti il percorso dell'app nella prima riga. Poi se esegui quel file batch (con un doppio clic), l'app viene eseguito senza il messaggio. Puoi anche creare un collegamento al file per metterlo in un posto pi&ugrave; comodo sul computer, per esempio il desktop.</p>   

<!--
<h3>Nota per Windows Vista</h3>

<p>Questa versione di Windows ha introdotto un sistema chiamato <i>Controllo dell'account utente</i>
(in inglese, User Account Control o UAC), che significa che certe attivit&agrave; richiedono
un'autorizzazione esplicita anche quando eseguite da un amministratore del computer.
Ci&ograve; vuol dire che questo programma richiede l'autorizzazione di eseguire un aggiornamento,
perch&eacute; sostituisce e esegue altri file. Per questo motivo il programma &egrave; stato creato
in modo di essere eseguito con i diritti dell'amministratore, e quindi quando &egrave; avviato
quando l'UAC &egrave; attivo (che lo &egrave; come impostazione predefinita in Windows Vista, bench&eacute;
l'UAC possa essere disattivato nella sezione <i>Utenti</i> del Panello di controllo)
una finestra si aprir&agrave; chiedendo se tu voglia eseguire il programma.
Quando consenti, il programma non avr&agrave; ulteriori problemi.</p>
-->
<h2>Funzionamento del programma</h2>

<p>Quando &egrave; avviato, il programma controlla la lingua del sistema operativo (Windows o Linux). Se &egrave; italiano, il programma sar&agrave;
in italiano. Se non &egrave; italiano, il programma sar&agrave; in inglese. In ogni caso, &egrave; possibile cambiare
la lingua dell'interfaccia nella scheda <i>Interfaccia</i> delle opzioni del programma.
</p>

<p>Il programma al primo avvio controlla anche la disponibilit&agrave; di aggiornamenti e di altri componenti.
Infatti il file scaricato include solo una versione della Bibbia, e puoi scaricare altre versioni e altri file per utilizzare tutte le funzioni del programma.
Devi essere collegato ad Internet affinch&eacute; il programma possa fare questo controllo.
Puoi anche cercare altri componenti in un secondo momento usando il comando <i>Aggiorna</i> del menu <i>Strumenti</i>.
Se usi un proxy per collegarti ad Internet, dovrai prima immettere i dati del proxy nelle <i>Opzioni</i>.
Il programma controlla in modo regolare se ci sono aggiornamenti al programma o ai suoi componenti, per cui non lo dovrai mai pi&ugrave; scaricare. La frequenza dei controlli pu&ograve; essere impostata nelle <i>Opzioni</i> del programma.
</p>
<p>Alternativamente, puoi scaricare tutti i file addizionali desiderati dalla pagina con l'<a href="/program/addins.php">elenco di tutti i componenti addizionali disponibili</a>. Ogni file scaricato va scompattato, e il contenuto messo nella cartella in cui il programma &egrave; stato installato.</p>
<p>Si suggerisce anche di leggere la sezione del file della Guida <i>Come posso...</i> per capire i compiti principali che puoi eseguire con il programma.
</p>
<!--<p>Un altro modo per imparare ad usare il programma &egrave; di guardare i <a href="video.php">video delle lezioni</a>. Gli stessi filmati possono essere guardati nel programma (se sono stati scaricati con il comando <i>Aggiorna</i>) dal menu <i>?</i>.</p>-->

<p>Clicca l'immagine per ingrandire questo esempio dell'uso del programma.<br />
<a href="/immagini/esempio7.jpg"><img src="/immagini/esempio7a.jpg" width="431" height="389" alt="Un esempio dell'uso del programma della Bibbia" title="Un esempio dell'uso del programma della Bibbia"></a>
</p>

<!--<a name="screensaver"></a>
<h2>Salvaschermo</h2>
<p>C'&egrave; un salvaschermo (screensaver) che usa i file del programma per visualizzare il testo della Bibbia quando il computer non &egrave; utilizzato per un certo periodo di tempo.
Prima di usare il salvaschermo, bisogna avviare almeno una volta il programma principale.
Poi scarica <a href="/v7/LaParola Screensaver.scr">il salvaschermo</a>, e copia il file alla cartella di sistema di Windows (di solito c:\windows\system32\, in versioni a 64 bit c:\windows\SysWOW64\). Oppure, clicca con il tasto destro del mouse sul file scaricato, e scegli la voce <i>Installa</i>.
Poi nel <i>Panello di controllo</i> di Windows apri la finestra con le impostazioni dei salvaschermi, e scegli <i>LaParola Screensaver</i> dall'elenco.
Puoi anche cambiare le impostazioni del salvaschermo per visualizzare il testo in diversi modi.</p>

<a name="macro"></a>
<h2>Macro per Word</h2>

<p>&Egrave; possibile inserire un comando nel programma <i>Word</i> per automaticamente inserire dei versetti biblici, senza dover aprire il programma della Bibbia.
Per aggiungere questa possibilit&agrave; a Word, scarica e scompatta <a href="/file/LaParolaMacro.zip">questo file</a> e segui le istruzioni nel file <i>LeggiMi.txt</i>.
-->

<a name="source"></a>
<h2>Codice sorgente</h2>

<p>Per i programmatori, non solo &egrave; il codice sorgente disponibile <a href="/file/laparolacode.zip">qui</a>, ma
anche l'ambiente di sviluppo, cio&egrave;
<a href="https://visualstudio.microsoft.com/vs/">Visual C# Express</a>. Se viene chiesta una password per aprire il file, &egrave; <em>bibbia</em>.</p>

<h2>Versioni future</h2>

<p>Per essere informato di aggiornamenti al programma, puoi:</p>
<ul>
<li>iscriverti alla <a href="/mailing_list.php">mailing list</a> del programma (messaggi non frequenti, solo per cambiamenti grandi); oppure</li>
<li>controllare periodicamente la <a href="/novita.php">pagina delle novit&agrave;</a> oppure controllare il <a href="/novita.php">feed XML</a> del sito; oppure</li>
<!--<li>leggere il <a href="/blog/">blog</a> sulla Bibbia.</li>-->
<li>seguire la <a href="http://www.facebook.com/pages/LaParola/114249198597738">pagina del programma su Facebook</a>.</li> 
</ul>

<!--<h2>Versioni precedenti</h2>
<p>Chi ha un computer pi&ugrave; vecchio che ha difficolt&agrave; con questa versione del programma potrebbe provare la versione 6.5 del programma. Funziona ancora, anche se non &egrave; pi&ugrave; sviluppato da diversi anni. Puoi scaricare questa versione del programma dalla <a href="/programma/scaricw.php">pagina per la versione 6.5</a>.</p>
-->
<?
require("../piede.php");
?>
