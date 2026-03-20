<?
$descriz = "Come inserire link a tutti i riferimenti biblici nel tuo sito";
$key = "berea,link,collegamento";
$titolo = "Berea: Come inserire un link per tutti i riferimenti biblici in un sito";
$sezione = "Informazioni sul sito";
require("capo.php");
?>
<h1>Berea: Come inserire un link per tutti i riferimenti biblici in un sito</h1>
<p class="primalettera">Con lo script (programma) <em>Berea</em> &egrave; possibile convertire automaticamente tutti i riferimenti biblici nel tuo sito ad un link che mostra il testo del riferimento quando il mouse passa sopra il riferimento (oppure in un'altra finestra quando si clicca sul link). Per esempio, passa il mouse sopra il riferimento Gv 3:16 senza cliccare.</p>
<h2>Installazione</h2>
<p>Per inserire i link, devi semplicemente inserire la riga<br />
<code>&lt;script type="text/javascript" charset="utf-8" src="https://www.laparola.net/berea.js"&gt;&lt;/script&gt;</code><br />
immediatamente prima del tag &lt;/body&gt; sulla pagina.</p>
<p>Se il tuo sito &egrave; composto da diversi file HTML, dovrai aggiungere manualmente la riga a tutte le pagine (o a tutte le pagine che contengono un riferimento). Per&ograve;, alcuni programmi (come Dreamweaver, FrontPage, qualche editor di testo) possono fare una ricerca e sostituzione veloce in tutti i file per sostituire &lt;/body&gt; con la riga dello script pi&ugrave; &lt;/body&gt;.</p>
<p>Se il tuo sito &egrave; stato costruito con dei file template (o modelli), dovr&agrave; essere sufficiente aggiungere la riga al file che contiene il tag &lt;/body&gt; per aggiungere la riga a tutte le pagine del sito. In un Content Management System, programma di Blog o Forum come WordPress, Blogger, phpBB eccetera, &egrave; necessario solo aggiungere la riga al modello del programma.</p>
<h2>Impostazioni avanzate</h2>
<p>Puoi modificare l'aspetto della finestra che si apre, per esempio per renderlo pi&ugrave; simile al tuo sito. Lo fai aggiungendo delle righe fra la riga dello script e il tag &lt;/body&gt;. Le possibilit&agrave; sono:<br />
<code>&lt;script type="text/javascript"&gt;LPNaltezza = 300;&lt;/script&gt;</code>
per impostare l'altezza della finestra. 300 pixel &egrave; il valore predefinito, ma puoi mettere un altro valore.<br />
<code>&lt;script type="text/javascript"&gt;LPNlarghezza = 400;&lt;/script&gt;</code>
per impostare la larghezza della finestra. 400 pixel &egrave; il valore predefinito, ma puoi mettere un altro valore.<br />
<code>&lt;script type="text/javascript"&gt;LPNcolore = "yellow";&lt;/script&gt;</code>
per impostare il colore di sfondo della finestra. Giallo &egrave; il colore predefinito, ma puoi mettere un altro valore, con il nome del colore in inglese oppure il codice di sei cifre usato in HTML per il colore.<br />
<code>&lt;script type="text/javascript"&gt;LPNcolore2 = "#ffcc33";&lt;/script&gt;</code>
per impostare il colore di sfondo della parte inferiore della finestra. #ffcc33 (simile ad aranchione) &egrave; il colore predefinito, ma puoi mettere un altro valore, con il nome del colore in inglese oppure il codice di sei cifre usato in HTML per il colore.<br />
<code>&lt;script type="text/javascript"&gt;LPNversione = "Nuova Riveduta";&lt;/script&gt;</code>
per impostare la versione da visualizzare. La Nuova Riveduta &egrave; la versione predefinita, ma puoi mettere "C.E.I.", "Nuova Diodati", "Riveduta 2020", "Nuova Riveduta 1994", "Riveduta", "Ricciotti", "Tintori", "Martini", "Diodati", "Commentario", "CommentarioNT", o "Riferimenti incrociati".<br />
<code>&lt;script type="text/javascript"&gt;LPNritardo = 0;&lt;/script&gt;</code>
per impostare il ritardo (in millisecondi) prima di aprire la finestra con il testo. 0 &egrave; il valore predefinito (nessun ritardo), ma puoi mettere un altro valore, per esempio 2000 per aspettare due secondi.<br />
</p>
<p>C'&egrave; un'altra impostazione facoltativa che va messa <em>prima</em> della riga dello script. Potrebbe succedere che vuoi usare lo script in tutte le pagine del sito tranne alcune. In quel caso, puoi mettere la riga dello script in tutte le pagine, e nelle pagine in cui non lo vuoi usare inserire la riga<br />
<code>&lt;script type="text/javascript"&gt;LPNnoscript = 1;&lt;/script&gt;</code>
</p>
<h2>Note</h2>
<p>Molti tipi di riferimenti e abbreviazioni per i libri sono riconosciuti. Se per&ograve; trovi nel tuo sito un riferimento non riconosciuto o qualcosa che &egrave; riconosciuto come riferimento biblico che non lo &egrave;, scrivimi e aggiuster&ograve; il codice.</p>
<p>Il codice non aggiunge un link ai riferimenti che sono contenuti nei tag &lt;a&gt;, &lt;input&gt;, &lt;h1&gt;, &lt;h2&gt;, &lt;h3&gt;, &lt;code&gt;.</p>
<p>Ringrazio Filippo Barb&egrave; per il suo prezioso aiuto per sviluppare <em>Berea</em>.<br />
Questo programma si chiama <em>Berea</em>, perch&eacute; vuole essere un servizio per quanti desiderano imitare i credenti di quella citt&agrave; anche nell'era informatica. La preghiera &egrave; che possa essere utile a qualcuno per conoscere la giustificazione per fede in Ges&ugrave; Cristo.<br />
<cite>Essi erano di sentimenti pi&ugrave; nobili perch&eacute; ricevettero la Parola con ogni premura, esaminando ogni giorno le Scritture per vedere se le cose stavano cos&igrave;. (Atti 17:10-11)</cite>
</p>
<?
require("piede.php");
?>
