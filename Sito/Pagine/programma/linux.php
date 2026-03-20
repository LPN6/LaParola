<?
$descriz = "Programma della Bibbia per Linux";
$key = "Linux";
$titolo = "Linux";
$sezione = "Programma";
require("../capo.php");
?>
<h1>Linux</h1>
<p class="primalettera">Ho sviluppato due programmi della Bibbia per Linux. Il primo porta la versione completa per Windows a Linux, quindi &egrave; pi&ugrave; completa ma il processo di portare il programma usando Mono non &egrave; del tutto funzionante. Il secondo &egrave; una versione in Java che &egrave; identica alla versione per Macintosh. Puoi provare tutti e due i programmi per capire quale versioni &egrave; meglio per te.</p>

<h2>Versione in Mono</h2>

<h3>Installazione</h3>

<p>&Egrave; possibile che Mono sia gi&agrave; installato nella tua copia di Linux; per&ograve; &egrave; anche possibile che non sia una versione pi&ugrave; abbastanza recente.
Il programma richiede almeno la versione 1.2.5, rilasciata in agosto 2007. Per&ograve; funziona molto meglio con la versione 2.0 di Mono.
Mono per diverse distribuzioni pu&ograve; essere scaricato dal <a href="http://www.go-mono.com/mono-downloads/download.html">sito di Mono</a>; 
se la tua distribuzione non &egrave; elencata clicca il link &quot;Other&quot;.</p>

<p>Quando Mono &egrave; installato, si pu&ograve; installare il programma in uno di tre modi.</p>
<ul>
<li>Se hai gi&agrave; installato la versione per Windows, puoi copiare il programma e i file dei dati a Linux.
Devi copiare tutto il contenuto della cartella in cui hai installato il programma per Windows (di solito c:\programmi\LaParola\) ad una cartella qualsiasi di Linux.
Poi devi copiare il contenuto della cartella dei dati (di solito c:\utenti\&lt;nome&nbsp;utente&gt;\AppData\Roaming\LaParola\ in Windows Vista; c:\Documents&nbsp;and&nbsp;Settings\&lt;nome&nbsp;utente&gt;\Dati&nbsp;Applicazioni\LaParola\ in Windows XP)
nella cartella /home/&lt;nome&nbsp;utente&gt;/.config/LaParola/ di Linux.</li>
<li>Se hai gi&agrave; installato la versione per Linux, non devi scaricare niente, basta usare il comando <i>Aggiorna</i> del programma.</li> 
<li>Altrimenti, puoi scaricare <a href="/file/LaParola.tar.gz">la versione per Linux</a>. I file sono esattamente uguali a quelli di Windows, ma sono in un archivio tar.gz invece di un programma di installazione.
Poi devi usare il comando <i>tar xzvf LaParola.tar.gz</i> oppure un programma grafico per scompattare il file in una cartella qualsiasi.</li>
</ul>
<p>In ogni caso, per avviare il programma bisogna usare il comando <i>mono percorso/del/programma/LaParola.exe</i></p>
<p>Nota per&ograve; che Mono non &egrave; un sostituto completo di .NET e ha ancora degli errori, per cui il programma in Mono non &egrave; ancora perfezionato.</p>

<p>Ci sono tre errori che possono apparire, a causa di problemi nell'installazione di Mono in alcune distribuzioni di Linux:</p>
<ol>
<li>Non tutti i componenti necessari per eseguire il programma sono installati. Per correggere questo problema, installa il programma <em>MonoDevelop</em>, usando il metodo normale per aggiungere dei programmi a Linux.</li>
<li>Un messaggio di errore che riguarda la codepage 1252. Per correggere questo problema, installa il file libmono-i18n2.0-cil utilizzando il metodo normale per scaricare componenti addizionali nella tua versione di Linux.</li>
<li>Un messaggio di errore <code>no implementation for interface method Atk.TableImplementor</code> quando si esegue il programma. Per correggere questo problema, esegui il comando <code>sudo apt-get remove .uia.</code> dalla riga di comando.</li>
</ol>

<h3>Funzionamento del programma</h3>

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

<!--
<h2>Versione in Java</h2>
<p>Questa &egrave; una versione beta (prova) del programma della Bibbia. &Egrave; pubblicata qui affinch&eacute; tutti possano cercare problemi nel programma, che potr&ograve; correggere per la prossima versione. Se trovi qualcosa che il programma va male, ti chiedo di scrivermi. Non serve scrivere delle cose che il programma non fa; so gi&agrave; quello che manca, e secondo il tempo disponibile aggiunger&ograve; nuovi funzioni al programma nel futuro. Attualmente &egrave; disponibile solo la versione Nuova Riveduta della Bibbia; altre versioni saranno aggiunte nella versione del programma.</p>
<p>La versione attualmente disponibile &egrave; la beta 2. Il cambiamento principale in questa versione &egrave; la possibilit&agrave; di ricercare parole e frasi nella Bibbia. La ricerca pi&ugrave; semplice &egrave; di una parola. Per&ograve; &egrave; possibile fare ricerche molto pi&ugrave; complicate -  vedi <a href="/java_ricerca.php">questa pagina</a> per una descrizione.</p>

<h3>Installazione</h3>
<p>Prima di tutto, &egrave; necessario che Java sia installato sul computer. Tutti i computer che ho provato hanno gi&agrave; Java installato, per cui non dovrebbe essere un problema. Quindi installa il programma seguendo le istruzioni qui sotto. Ma se c'&egrave; un messaggio di errore che dice che Java manca, lo puoi scaricare dal sito di <a href="http://www.java.com/it/download/manual.jsp">Java</a> e poi scrivimi affinch&eacute; io possa migliorare queste istruzioni.</p>

<p>Scarica <a href="/file/laparolajava.zip">questo file</a>, e scompatta il contenuto in una cartella qualsiasi. Nella sottocartella <i>bin</i>, cambia i permessi del file <i>laparola</i> (non laparola.exe) per renderlo eseguibile. Di solito basta cliccare con il tasto destro e scegli la scheda <i>Permessi</i> delle propriet&agrave;, ma dipende dalla versione di Linux usata. Poi fai un doppio clic sullo stesso file <i>laparola</i> per eseguirlo. Alternativamente, dalla riga di comando, cambia directory alla cartella <i>bin</i> ed esegui il comando <i>./laparola</i>.</p>

<h3>Codice sorgente</h3>
<p>Se vuoi vedere come &egrave; stato scritto il programma, e forse contribuire al suo sviluppo, il codice sorgente &egrave; <a href="/file/LaParolaNetBeans.zip">qui</a>. Richiede <a href="http://netbeans.org/downloads/start.html?platform=windows&lang=it&option=javase">NetBeans</a>.</p>
-->
<?
require("../piede.php");
?>
