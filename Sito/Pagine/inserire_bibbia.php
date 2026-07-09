<?
$descriz = "Come inserire codice sul proprio sito per visualizzare il testo della Bibbia";
$key = "inserire codice";
$titolo = "Come inserire codice";
$sezione = "Informazioni sul sito";
require("capo.php");
?>
<h1>Come inserire codice per visualizzare e ricercare la Bibbia sul proprio sito</h1>
<p class="primalettera">Ci sono diversi tipi di collegamenti che si possono creare:</p>
<ol>
<li><a href="#popup">ad una piccola finestra</a> (pop-up) che mostra solo il brano desiderato,</li>
<li><a href="#pagina">ad una pagina di questo sito</a>,</li>
<li><a href="#letture">per inserire il testo biblico nel proprio sito con il formato del sito</a> (cio&egrave; non un link al mio sito)</li>
<!--<li><a href="#phpnuke">in un modulo di PHP-Nuke</a>.</li>-->
</ol>
<p>Se usi il mio sito per visualizzare la Bibbia sul tuo sito, chiedo che metta un link al mio sito dalla pagina dove metti questo codice, per esempio:</p>
<p>Il testo della Bibbia &egrave; preso dal sito &lt;a href=&quot;https://www.laparola.net/&quot; title=&quot;La Bibbia&quot;&gt;della Bibbia, LaParola.net&lt;/a&gt;.</p>
<a name="popup"></a>
<h2>Ad una finestra pop-up</h2>
<p>Questo modo pu&ograve; essere utile se vuoi mettere un riferimento ad una brano sul proprio sito, senza digitare tutto il brano. Con un link di questo tipo, l'utente del sito pu&ograve; aprire e leggere il testo se lo desidera. Ci sono diversi esempi di questo sistema su questo sito, per esempio sulla pagina del <a href="/intro/racconto%20bibbia.php">racconto della Bibbia</a>.
<h3>Berea</h3>
<p>Con l'aggiunta di solo una riga ad una pagina Internet, &egrave; possibile creare automaticamente un link per tutti i riferimenti biblici nella pagina. Vedi <a href="berea.php">questa pagina su Berea</a> per ulteriori informazioni.</p>
<h3>Vecchio modo</h3>
<p>C'&egrave; un altro modo, che apre una nuova finestra quando un link viene cliccato. &Egrave; pi&ugrave; complicato usare questo modo sul tuo sito, ma ho lasciato questa descrizione per chi vuole utilizzare questo metodo.</p>
<p>Il codice minimo necessario &egrave; il seguente:<br />
All'inizio della pagina</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>&lt;script language=&quot;JavaScript&quot; src=&quot;https://www.laparola.net/popup.js&quot;&gt;&lt;/script&gt;</code></p></div>
<p>e poi per ogni riferimento</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>&lt;a href=&quot;JavaScript:popup('gv 3:16');&quot;&gt;Giovanni 3,16&lt;/a&gt;</code></p></div>
<p>dove chiaramente bisogna sostituire il riferimento desiderato.</p>
<p>Personalmente, per dare un'indicazione sulla barra di stato in fondo alla finestra del browser, io aggiungo nella tag &lt;a...&gt</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>onmouseover=&quot;window.status='Visualizza Giov 3:16'; return true&quot; onmouseout=&quot;window.status=''; return true"</code></p></div>
ma non &egrave; necessario.</p>
<p>Il codice qui sopra usa sempre la versione Nuova Ricevuta della Bibbia. Per usare un'altra versione, o per mostrare pi&ugrave; di una versiona, bisogna aggiungere il nome o i nomi delle versioni come argomenti. Per esempio</p>
<p><code>&lt;a href=&quot;JavaScript:popup('gv3:16', 'C.E.I.');&quot;&gt;Giovanni 3,16&lt;/a&gt;</code></p>
<p>visualizza il versetto nella C.E.I., mentre</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>&lt;a href=&quot;JavaScript:popup('gv3:16','Nuova Riveduta','Commentario');&quot;&gt;Giovanni 3,16&lt;/a&gt;</code></p></div>
<p>lo visualizza nella Nuova Riveduta, con un commentario. I nomi che possono essere usati per le versioni sono: 'Nuova Riveduta', 'C.E.I.', 'Nuova Diodati', 'Riveduta 2020', 'Nuova Riveduta (1994)', Riveduta', 'Ricciotti', 'Tintori', 'Martini', 'Diodati', 'Volgare', 'CommentarioHenry', 'CommentarioNT', 'CommentarioCalvino', Commentario', 'CommentarioBarnes', 'CommentarioGinevra', CommentarioGill', CommentarioPulpito', 'CommentarioIllustratore', 'CommentarioMeyer', 'CommentarioTesoro', o 'Riferimenti+incrociati'.</p>
<p>Similmente, per mostrare i risultati di una ricerca in una finestra pop-up, bisogna mettere lo stesso codice all'inizio della pagina, e poi per ogni ricerca</p>
<p><code>&lt;a href=&quot;JavaScript:popupr('abba', 'C.E.I.', 'mt-gv');&quot;&gt;abba&lt;/a&gt;</code></p>
<p>dove la versione e il brano in cui ricercare sono facoltativi.</p>

<a name="pagina"></a>
<h2>Ad una pagina di questo sito</h2>
<h3>Al testo di un brano della Bibbia</h3>
<p>Per inserire un link ad una pagina di questo sito con il testo di qualsiasi brano della Bibbia, bisogna inserire codice HTML simile a:</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>&lt;a href=&quot;https://www.laparola.net/testo.php?riferimento=gv3:16&versioni[]=Nuova+Riveduta&versioni[]=C.E.I.&quot;&gt;Giovanni 3:16&lt;/a&gt; &egrave; il versetto pi&ugrave; conosciuto nella Bibbia.</code></p></div>
<p>Questo codice sarebbe visualizzato cos&igrave; in un browser:<p>
<p><a href="/testo.php?riferimento=gv3:16&versioni[]=Nuova+Riveduta&versioni[]=C.E.I.">Giovanni 3:16</a> &egrave; il versetto pi&ugrave; conosciuto nella Bibbia.</p>
<p>Tutti i caratteri nella stringa che segue 'href' devono essere in una sola riga, non divisi fra due o pi&ugrave; righe come qui sopra.</p>
<p>Per il riferimento, &egrave; possibile usare quasi qualsiasi formato, come spiegato nell'<a href="aiutovis.php" title="Come digitare un riferimento e mostrare un brano">aiuto per visualizzare la Bibbia</a>. Nota per&ograve; che l'indirizzo di una pagina non pu&ograve; contenere uno spazio, quindi bisogna togliere tutti gli spazi dal riferimento, oppure sostituire ogni spazio con un segno di pi&ugrave; (+).</p>
<p>Mettere la versione o le versioni desiderate &egrave; facoltativo; se non c'&egrave; nessuna versiona, la Nuova Riveduta &egrave; usata. Per usare una versione diversa o multiple versioni, bisogna aggiungerle all'indirizzo come nell'esempio qui sopra. Nota le parentesi quadrate dopo 'versioni'. I nomi che si possono usare per le versioni sono: 'Nuova+Riveduta', 'C.E.I.', 'Nuova+Diodati', 'Riveduta+2020', 'Nuova+Riveduta+(1994)', 'Riveduta', 'Ricciotti', 'Tintori', 'Martini', 'Diodati', 'Volgare', 'CommentarioHenry', 'CommentarioNT', 'CommentarioCalvino', Commentario', 'CommentarioBarnes', 'CommentarioGill', 'CommentarioGinevra', CommentarioPulpito', 'CommentarioIllustratore', 'CommentarioMeyer', 'CommentarioTesoro', e 'Riferimenti+incrociati'. (Come sempre, il segno di pi&ugrave; sostituisce uno spazio nel nome.)</p>
<h3>Ad una ricerca di un'espressione</h3>
<p>Il codice pi&ugrave; semplice &egrave;
<div style="word-wrap: break-word;word-break: break-all;"><p><code>La parola '&lt;a href=&quot;https://www.laparola.net/ricerca.php?frase=abb&agrave;&quot;&gt;abb&agrave;&lt;/a&gt;' appare tre volte nella Bibbia.</code></p></div>
<p>Un indirizzo di questo tipo cerca nella versione Nuova Riveduta, in tutta la Bibbia. Per ricercare un'altra versione o solo in una parte della Bibbia, bisogna usare codice simile a</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>La parola '&lt;a href=&quot;https://www.laparola.net/ricerca.php?frase=abb&agrave;&versione=C.E.I.&brano=mt-gv&quot;&gt;abb&agrave;&lt;/a&gt;' appare una volta nei Vangeli.</code></p></div>
<p>Per il parametro <code>frase</code>, invece di solo una parola, si pu&ograve; usare un'espressione pi&ugrave; complicata, come spiegato nell'<a href="aiutoric.php" title="I codici da usare per ricerche complicate">aiuto per ricercare la Bibbia</a>.
<h3>Alla definizione di un nome</h3>
<p>Per usare il dizionario dei nomi, bisogna aggiungere un collegamento di questo tipo:</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>&lt;a href=&quot;https://www.laparola.net/nomi/nomi.php?nome=davide&quot;&gt;Davide&lt;/a&gt; &egrave; il nome pi&ugrave; citato nella Bibbia.</p></code></div>
<p>&Egrave; anche possibile usare solo le prime lettere del nome.</p>
<h3>Per mettere un riquadro con tutte le versioni</h3>
<p>Per mettere una tabella come quella sulla <a href="/">prima pagina di questo sito</a>, con la possibilit&agrave; di mostrare un brano o fare una ricerca di qualsiasi versione, bisogna usare codice simile al seguente. &Egrave; possibile visualizzare il codice HTML della prima pagina e copiarlo al proprio sito.</p>
<div style="word-wrap: break-word;word-break: break-all;">
<p><code>
&lt;center&gt;&lt;table border=&quot;1&quot; cellpadding=&quot;9&quot;&gt;<br />
&lt;tr&gt;&lt;td width=&quot;50%&quot; valign=&quot;top&quot;&gt;<br />
&lt;p&gt;&lt;b&gt;Visualizzare un brano&lt;/b&gt;&lt;/p&gt;<br />
&lt;form action=&quot;https://www.laparola.net/testo.php&quot; method=&quot;post&quot; onsubmit=&quot;if (riferimento.value.length==0) {alert('Digitare il riferimento di un brano')}; return riferimento.value.length!=0;&quot;&gt;<br />
&lt;p&gt;Brano da visualizzare:&lt;br&gt;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;input type=&quot;text&quot; name=&quot;riferimento&quot;&gt;&lt;/p&gt;<br />
&lt;p&gt;Testo/i da visualizzare:&lt;br&gt;<br />
&lt;select multiple name=&quot;versioni[]&quot; size=&quot;23&quot;&gt;<br />
&lt;option selected value=&quot;Nuova Riveduta&quot;&gt;Nuova Riveduta&lt;/option&gt;<br />
&lt;option value=&quot;C.E.I.&quot;&gt;C.E.I. (1974)&lt;/option&gt;<br />
&lt;option value=&quot;Nuova Diodati&quot;&gt;Nuova Diodati&lt;/option&gt;<br />
&lt;option value=&quot;Riveduta 2020&quot;&gt;Riveduta 2020&lt;/option&gt;<br />
&lt;option value=&quot;Nuova Riveduta (1994)&quot;&gt;Nuova Riveduta (1994)&lt;/option&gt;<br />
&lt;option value=&quot;Riveduta&quot;&gt;Luzzi/Riveduta&lt;/option&gt;<br />
&lt;option value=&quot;Ricciotti&quot;&gt;Ricciotti&lt;/option&gt;<br />
&lt;option value=&quot;Tintori&quot;&gt;Tintori&lt;/option&gt;<br />
&lt;option value=&quot;Martini&quot;&gt;Martini&lt;/option&gt;<br />
&lt;option value=&quot;Diodati&quot;&gt;Diodati&lt;/option&gt;<br />
&lt;option value=&quot;Volgare&quot;&gt;Bibbia in Volgare&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioHenry&quot;&gt;Commentario completo di Matthew Henry&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioNT&quot;&gt;Commentario Nuovo Testamento&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioCalvino&quot;&gt;Commentario di Giovanni Calvino&lt;/option&gt;<br />
&lt;option value=&quot;Commentario&quot;&gt;Commentario abbreviato&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioBarnes&quot;&gt;Note di Albert Barnes&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioGinevra&quot;&gt;Note della Bibbia di Ginevra&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioGill&quot;&gt;Esposizione della Bibbia di Gill&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioPulpito&quot;&gt;Commentario del Pulpito&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioIllustratore&quot;&gt;Illustratore biblico&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioMeyer&quot;&gt;Commento di Frederick Brotherton Meyer&lt;/option&gt;<br />
&lt;option value=&quot;CommentarioTesoro&quot;&gt;Tesoro di Davide&lt;/option&gt;<br />
&lt;option value=&quot;Riferimenti incrociati&quot;&gt;Riferimenti incrociati&lt;/option&gt;<br />
&lt;/select&gt;&lt;/p&gt;<br />
&lt;input type=&quot;submit&quot; name=&quot;Submit&quot; value=&quot;Visualizza testo&quot;&gt;<br />
&lt;p&gt;&lt;a href=&quot;https://www.laparola.net/aiutovis.php&quot;&gt;Aiuto per visualizzare un brano della Bibbia&lt;/a&gt;&lt;/p&gt;<br />
&lt;/form&gt;<br />
&lt;/td&gt;&lt;td valign=&quot;top&quot;&gt;<br />
&lt;form action=&quot;https://www.laparola.net/ricerca.php&quot; method=&quot;post&quot; onsubmit=&quot;if (frase.value.length==0) {alert('Digitare una parola o espressione da ricercare')}; return frase.value.length!=0;&quot;&gt;<br />
&lt;p&gt;&lt;b&gt;Ricercare un'espressione&lt;/b&gt;&lt;/p&gt;<br />
&lt;p&gt;Parola o frase da ricercare:&lt;br&gt;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;input type=&quot;text&quot; name=&quot;frase&quot;&gt;&lt;/p&gt;<br />
&lt;p&gt;Versione da ricercare:&lt;br&gt;<br />
&lt;select name=&quot;versione&quot; size=&quot;11&quot;&gt;<br />
&lt;option selected value=&quot;Nuova Riveduta&quot;&gt;Nuova Riveduta&lt;/option&gt;<br />
&lt;option value=&quot;C.E.I.&quot;&gt;C.E.I. (1974)&lt;/option&gt;<br />
&lt;option value=&quot;Nuova Diodati&quot;&gt;Nuova Diodati&lt;/option&gt;<br />
&lt;option value=&quot;Riveduta 2020&quot;&gt;Riveduta 2020&lt;/option&gt;<br />
&lt;option value=&quot;Nuova Riveduta 1994&quot;&gt;Nuova Riveduta (1994)&lt;/option&gt;<br />
&lt;option value=&quot;Riveduta&quot;&gt;Luzzi/Riveduta&lt;/option&gt;<br />
&lt;option value=&quot;Ricciotti&quot;&gt;Ricciotti&lt;/option&gt;<br />
&lt;option value=&quot;Tintori&quot;&gt;Tintori&lt;/option&gt;<br />
&lt;option value=&quot;Martini&quot;&gt;Martini&lt;/option&gt;<br />
&lt;option value=&quot;Diodati&quot;&gt;Diodati&lt;/option&gt;<br />
&lt;option value=&quot;Volgare&quot;&gt;Bibbia in Volgare&lt;/option&gt;<br />
&lt;/select&gt;&lt;/p&gt;<br />
&lt;p&gt;Brano in cui ricercare:&lt;br&gt;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;input type=&quot;text&quot; name=&quot;brano&quot;&gt;&lt;/p&gt;<br />
&lt;input type=&quot;submit&quot; name=&quot;Submit&quot; value=&quot;Ricerca&quot;&gt;<br />
&lt;p&gt;&lt;a href=&quot;https://www.laparola.net/aiutoric.php&quot;&gt;Aiuto per ricercare un'espressione della Bibbia&lt;/a&gt;&lt;/p&gt;<br />
&lt;/form&gt;<br />
&lt;/td&gt;&lt;/tr&gt;&lt;/table&gt;<br />
&lt;p&gt;Un servizio di &lt;a href=&quot;https://www.laparola.net/&quot; title=&quot;La Bibbia&quot;&gt;La Bibbia&lt;/a&gt;.&lt;/p&gt;<br />
&lt;/center&gt;
</code></p>
</div>

<a name="letture"></a>
<h2>Per inserire il testo biblico nel proprio sito</h2>
<h3>Un brano o il risultato di una ricerca</h3>
<p>Per inserire il testo della Bibbia nel tuo sito nel formato del tuo sito, devi avere la possibilit&agrave; di usare PHP sul sito. Poi devi solo inserire una riga di codice PHP come</p>
<div style="word-wrap: break-word;word-break: break-all;"><p><code>
include(&quot;https://www.laparola.net/inserire_testo.php?riferimento=gv3:16&amp;versioni[]=Nuova+Riveduta&amp;versioni[]=C.E.I.&quot;);
</code></p>
</div>
<p>Gli argomenti di questo codice sono stati spiegati qui sopra.</p>
<p>Per inserire una ricerca invece, bisogna usare una riga come</p>
<div style="word-wrap: break-word;word-break: break-all;">
<p><code>
include(&quot;https://www.laparola.net/inserire_ricerca.php?frase=signore&amp;versione=C.E.I.&amp;brano=Gen-Mal&amp;nBraniInizio=10&amp;nBraniFine=30&quot;);
</code></p>
</div>
<p>che mostra dal decimo al trentesimo versetto nell'Antico Testamento che contiene la parola 'signore' nella versione C.E.I. Il sito mostrer&agrave; sempre un massimo di 50 versetti.<!-- <a href="/file/NelProprioSito.zip">Questo file</a> ne &egrave; un esempio che puoi adattare al proprio sito.--></p>
<p>In tutti e due i casi &egrave; possibile  creare un form HTML in cui l'utente pu&ograve; digitare il riferimento del brano o la frase da ricercare, e nella pagina con i risultati del form inserire una di queste due righe di codice.</p>
<p><b>Nota:</b> Questo codice non funziona su un server con PHP 5.2 o dopo con le impostazioni predefinite. In quel caso, bisogna cambiare nel file php.ini del tuo server la riga</p>
<code>
allow_url_include = Off
</code>
<p>in</p>
<code>
allow_url_include = On
</code>
<h3>Un brano</h3>
<p>Un modo alternativo per inserire un brano, utilizzando JavaScript e JSON, &egrave; di inserire il seguente codice (cambiando il riferimento e la versione), che mostrer&agrave; il testo con lo stile del tuo sito:<br />
<code>
&lt;div id=&quot;testo&quot;&gt;&lt;/div&gt;<br />
&lt;script language=&quot;JavaScript&quot; type=&quot;text/javascript&quot;&gt;<br />
obj = { &quot;riferimento&quot;:&quot;gv3:15-16&quot; , &quot;versione&quot;:&quot;Nuova Riveduta&quot;};<br />
param = JSON.stringify(obj);<br />
xmlhttp = new XMLHttpRequest();<br />
xmlhttp.onreadystatechange = function() {<br />
&nbsp;&nbsp;if (this.readyState == 4 &amp;&amp; this.status == 200) {<br />
&nbsp;&nbsp;&nbsp;&nbsp;res = this.responseText.replace(&quot;\\n&quot;,&quot;&quot;);<br />
&nbsp;&nbsp;&nbsp;&nbsp;res = res.replace(/\\/g, &quot;&quot;);<br />
&nbsp;&nbsp;&nbsp;&nbsp;res = res.substring(1, res.length-1);<br />
&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(&quot;testo&quot;).innerHTML = res;<br />
&nbsp;&nbsp;}<br />
};<br />
xmlhttp.open(&quot;GET&quot;, &quot;https://www.laparola.net/js.php?q=&quot; + param, true);<br />
xmlhttp.send();<br />
&lt;/script&gt;
</code></p>
<h3>La lettura del giorno</h3>
<p>Questo codice utilizza JavaScript (che di solito &egrave; abilitato nel browser dell'utente) per visualizzare la <a href="letoggi.php">lettura del giorno</a> di questo sito nel tuo sito, usando il formato del testo del tuo sito invece di quello di questo sito. &Egrave; molto semplice: dove vuoi inserire la lettura del giorno in una pagina devi solo mettere il codice HTML</p>
<div style="word-wrap: break-word;word-break: break-all;">
<p><code>
&lt;script language=&quot;JavaScript&quot; src=&quot;https://www.laparola.net/letoggijs.php&quot;&gt;&lt;/script&gt;<br />
&lt;noscript&gt;&lt;a href=&quot;https://www.laparola.net/letoggi.php&quot;&gt;La lettura del giorno&lt;/a&gt;&lt;/noscript&gt;
</code></p>
</div>

<!--
<a name="phpnuke"></a>
<h2>In un modulo di PHP-Nuke</h2>
<p><a href="https://www.phpnuke.org/">PHP-Nuke</a> &egrave; un sistema per la creazione di siti Internet, con la possibilit&agrave; di aggiungere dei moduli per personalizzare il sito.
Luciano Leoni ha creato ha un <a href="/file/phpnuke.zip">modulo per usare i servizi di questo sito in PHP-Nuke</a> - le istruzioni sono nel file leggimi.txt nel file scaricato.</p>
-->
<?
require("piede.php");
?>
