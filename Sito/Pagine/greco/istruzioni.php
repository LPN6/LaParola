<?
header("Content-type: text/html; charset=utf-8");
$fontuni = (isset($_REQUEST["greco_fontuni"])?$_REQUEST["greco_fontuni"]:"");
  $fontuni = str_replace("<", "", $fontuni); // affinché tag HTML non possono essere inseriti nella pagina
  $fontuni = str_replace(">", "", $fontuni);
$lin = (isset($_REQUEST["greco_lingua"])?$_REQUEST["greco_lingua"]:"");
  $lin = str_replace("<", "", $lin); // affinché tag HTML non possono essere inseriti nella pagina
  $lin = str_replace(">", "", $lin);
if ($lin=="")
   if (isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) $lin=substr($_SERVER['HTTP_ACCEPT_LANGUAGE'],0,2);
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<?if ($lin=="it") echo "it"; else echo "en";?>">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title><?if ($lin=="it") echo "La Sacra Bibbia - Manoscritti del Nuovo Testamento - istruzioni e spiegazione"; else echo "New Testament Manuscripts - Instructions and Explanations";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Istruzioni e spiegazione per la visualizzazione delle letture varianti dei manoscritti del Nuovo Testamento, per la critica testuale"; else echo "Instructions and explanations for viewing the variant readings of the manuscripts of the New Testamento, for textual criticism";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "Nuovo Testamento,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante"; else echo "New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
font-size: small;
}
</style>
</head>
<body>
<?if ($lin=="it") {?>
<h1>Manoscritti del Nuovo Testamento - Istruzioni</h1>
<p>Questo sito d&agrave; le varianti principali del Nuovo Testamento greco - siccome ci sono migliaia di manoscritti del NT, &egrave; naturale che
ci siano delle differenze fra essi, e non &egrave; sembra facile determinare quale era la lettura originale, anche se per il 99% dei versetti
il testo originale &egrave; chiaro.</p>

<p>Ci sono anche delle informazioni sulle parole greche nel testo. Sposta il cursore sopra una parola (senza cliccare) per vedere la sua forma grammaticale e sotto i versetti la definizione (in inglese), e clicca sulla parola per una definizione ed altre informazioni.</p>

<p>Per visualizzare il testo greco e varianti, scegliere un libro e digitare il riferimento di un brano (per esempio 3:2-4,6, 4:7) e fare clic sul pulsante <i>Visualizza testo</i>.</p>

<p>Le varianti di un versetto, se esistono, sono elencate separate da una riga vuota. Il testo che il sito utilizza (UBS/NA26) &egrave; sempre
dato prima e le letture alternative seguono sulle righe successive. Ogni lettura &egrave; seguita dall'evidenza, cio&egrave; i manoscritti
(o alcuni dei manoscritti) che la contengono. Se il mouse &egrave; lasciato momentaneamente sul nome di un manoscritto, si visualizza la data e
il tipo di testo del manoscritto (che pu&ograve; essere utile quando un manoscritto ha diversi tipi di testo, e si vuole controllare il tipo dei
manoscritti nel terzo ordine nelle opzioni).</p>

<p>L'ordine predefinito per i manoscritti &egrave;: papiri, onciali (manoscritti maiuscoli), famiglie, minisculi, lezionari, versioni antiche, padri, edizioni e versioni italiane.</p>
<ul>
<li><b>Papiri:</b> I manoscritti scritti su papiro invece di su pergamena. Sono indicati con la lettera p e un numero in apice.</li>
<li><b>Onciali:</b> I manoscritti scritti con caratteri maiuscoli. Sono elencati con la lettera ebraica alef (<span class="uni">&#8237;&#1488;</span>), una lettera maiuscola (latina o greca) o un numero che inizia con zero.</li>
<li><b>Famiglie:</b> Due famiglie di minisculi sono citate: f1 (=1, 118, 131, 209, 1582) e f13 (=13, 69, 124, 174, 230, 346, 543, 788, 826, 828, 983, 1689, 1709).</li>
<li><b>Minuscoli:</b> I manoscritti scritti con caratteri minuscoli. Sono elencati con un numero. Biz indica la maggior parte dei manoscritti bizantini; Biz<sup>2005</sup> indica la edizione di Robinson del 2005 quando diversa dalle edizioni precedenti.</li>
<li><b>Lezionari:</b> Lez indica la maggiore parte dei lezionari. Lezionari individuali sono indicati con una l e un numero in apice. Dopo il numero, 'pt' (=in parte) indica che il versetto appare almeno due volte, con letture diverse; 'm' indica un lezionario nel suo Menologion (le letture per i giorni speciali); 's' indica un lezionario nel suo Sinaxarion (le letture per l'anno liturgico).</li>
<li><b>Versioni antiche:</b> Le traduzioni del Nuovo Testamento fatte nei primi secoli dopo Cristo.</li>
<li><b>Padri:</b> Le citazioni degli autori cristiani dei primi secoli dopo Cristo.</li>
<li><b>Edizioni:</b> Tre altre edizioni del testo greco, quando differiscono dal testo UBS/NA26, sono indicate: il Textus Receptus (<span class="uni">&#962;</span>), la edizione di Westcott e Hort (WH), e la Editio Critica Maior (ECM). A volte le diverse edizioni del Textus Receptus hanno letture diverse; in tali casi sono citate le edizioni di Stephanus (1550) e di Scrivener (1894). 11 volte il testo di Nestle-Aland &egrave; diverso da quello dell'UBS, una volta le edizioni dell'UBS e una volta le edizioni del NA sono diverse; questi casi sono indicati con NA e UBS.</li>
<li><b>Versioni italiani:</b> Otto versioni sono elencate: la Nuova Riveduta (NR), la C.E.I. o Gerusalemme (CEI), la Nuova Diodati (ND), la Riveduta/Luzzi (Riv), la Diodati (Dio), la Traduzione interconfessionale in lingua corrente (TILC), la Nuovissima (Nv) e la Nuovo Mondo (NM). Quando la NR o la Riveduta mettono una lettura alternativa in una nota a pi&egrave;  di pagina, &egrave; indicato con una m. Lo scopo qui non &egrave; di elencare tutte le traduzioni diverse di queste versioni, ma indicare quali manoscritti hanno seguito.</li>
</ul>

<p>La pagina dei <a href="manoscritti.php">manoscritti</a> ha ulteriori informazioni sui codici usati per i manoscritti, e inoltre la data,
il contenuto e il tipo del testo.</p>

<h4>Abbreviazioni</h4>
<p>Nel testo delle varianti le seguenti abbreviazioni latine possono essere usate:<br />
<i>omit</i> (= omittit, omittunt) omette il seguente testo<br />
<i>add</i> (= addit, addunt) aggiunge il seguente testo</p>

<p>Un manoscritto pu&ograve;  essere accompagnato dai seguenti segni:<br />
(...) ha qualche differenza minore<br />
[...] il testo in quel manoscritto &egrave; inserito fra parentesi quadrate<br />
* la lettura originale<br />
<sup>a,b,c</sup> tre manoscritti degli scritti di Teofilatto<br />
<sup>c</sup> una correzione; quando diverse persone hanno corretto un manoscritto sono indicate da 1, 2, 3 eccetera<br />
<sup>testo</sup> il testo di un manoscritto o di un Padre se &egrave; diverso dalla lettura nel commentario che accompagna il testo<br />
<sup>lem</sup> una citazione di una lemma di un Padre, cio&egrave; il testo che precede il commentario<br />
<sup>comm</sup> una citazione dal testo di un commentario di un Padre, dove diverso dal testo biblico citato<br />
<sup>mg</sup> una lettura nel margine<br />
<sup>dub</sup> una citazione da un Padre, ma c'&egrave; dubbio che sia opera di quel Padre<br />
<sup>gr</sup> il testo greco di un Padre<br />
<sup>lat</sup> il testo latino di un Padre<br />
<sup>arm</sup> il testo armeno di un Padre<br />
<sup>sir</sup> il testo siriaco di un Padre<br />
<sup>slav</sup> il testo slavonico di un Padre<br />
<sup>arab</sup> il testo arabo di un Padre<br />
<sup><i>vid</i></sup> (=videtur) la lettura non &egrave; certa, per esempio se il documento &egrave; danneggiato<br />
<sup>l.v.</sup> lettura variante specificamente indicata in un manoscritto come alternativa<br />
<sup>2/3</sup> ecc la seconda cifra sta per quante volte il brano &egrave; citato da un Padre, la prima cifra sta per quante volte &egrave; citato con la forma di quella variante</p>

<p>Alla fine di un gruppo di manoscritti o di tutta la evidenza, possono essere aggiunti:<br />
<i>pc</i> (=pauci) anche pochi altri manoscritti non importanti (meno del 5% di tutti i manoscritti)<br />
<i>al</i> (=alii) molti manoscritti (dal 5 al 25%)<br />
<i>pm</i> (=permulti) la maggior parte dei manoscritti (dal 25 al 70%)<br />
<i>pl</i> (=plerique) quasi tutti i manoscritti<br />
pt una parte del gruppo citato<br />
ms uno manoscritto di quella famiglia<br />
mss alcuni manoscritti di quella famiglia</p>

<p>Altre abbreviazioni:<br />
<i>(ex lat?)</i> la variante potrebbe essere influenzata da una parte o da tutta la tradizione latina<br />
p) dopo una variante nei Vangeli indica che &egrave; forse stata influenzata da un brano parallelo<br />
s,ss (= uerses sequens, sequentes) e il seguente/i seguenti<br />
<i>sic</i> un errore riprodotto esattamente</p>

<h2>Confronta manoscritti</h2>
<p>Questa ricerca elenca tutte le varianti nel database in cui due manoscritti hanno letture uguali o diversi.
Per&ograve; siccome il database non contiene tutte le letture di ogni manoscritti, gli elenchi non sono completi.
&Egrave; possibile restringere le varianti elencate ad una parte della Bibbia - il sito pu&ograve; capire quasi qualsiasi riferimento che si potrebbe digitare, oppure si pu&ograve; lasciare il campo vuoto per fare la ricerca in tutto il NT.
Per i nomi che si devono usare per i manoscritti, vedi le <a href="sigle.php">sigle dei manoscritti permesse</a>.</p>

<a name="TrovaVersetti"></a>
<h2>Trova versetti</h2>
<p>La ricerca pi&ugrave; semplice &egrave; di cercare tutti i versetti che contengono una parola. Bisogna digitare la parola con lettere normali, senza accenti e un'eventuale h iniziale e sempre minuscole, secondo il seguente schema:</p>
<table>
<tr><td><span class="uni">&#945;</span></td><td>a</td></tr>
<tr><td><span class="uni">&#946;</span></td><td>b</td></tr>
<tr><td><span class="uni">&#947;</span></td><td>g</td></tr>
<tr><td><span class="uni">&#948;</span></td><td>d</td></tr>
<tr><td><span class="uni">&#949;</span></td><td>e</td></tr>
<tr><td><span class="uni">&#950;</span></td><td>z</td></tr>
<tr><td><span class="uni">&#951;</span></td><td>&#234;</td></tr>
<tr><td><span class="uni">&#952;</span></td><td>th</td></tr>
<tr><td><span class="uni">&#953;</span></td><td>i</td></tr>
<tr><td><span class="uni">&#954;</span></td><td>k</td></tr>
<tr><td><span class="uni">&#955;</span></td><td>l</td></tr>
<tr><td><span class="uni">&#956;</span></td><td>m</td></tr>
<tr><td><span class="uni">&#957;</span></td><td>n</td></tr>
<tr><td><span class="uni">&#958;</span></td><td>x</td></tr>
<tr><td><span class="uni">&#959;</span></td><td>o</td></tr>
<tr><td><span class="uni">&#960;</span></td><td>p</td></tr>
<tr><td><span class="uni">&#961;</span></td><td>r</td></tr>
<tr><td><span class="uni">&#963;</span></td><td>s</td></tr>
<tr><td><span class="uni">&#964;</span></td><td>t</td></tr>
<tr><td><span class="uni">&#965;</span></td><td>u <i>oppure</i> y</td></tr>
<tr><td><span class="uni">&#966;</span></td><td>f <i>oppure</i> ph</td></tr>
<tr><td><span class="uni">&#967;</span></td><td>ch</td></tr>
<tr><td><span class="uni">&#968;</span></td><td>ps</td></tr>
<tr><td><span class="uni">&#969;</span></td><td>&#244;</td></tr>
</table>
<p>Per digitare &#234; e &#244;, in Windows tieni premuto il tasto ALT mentre digiti 0234 o 0244 con il tastierino numerico. &Egrave; anche possibile usare ? invece di una lettera e * per qualsiasi numero di lettere.</p>
<p><b>Esempi:</b><br />
abussou trova <span class="uni">&#7936;&#946;&#973;&#963;&#963;&#959;&#965;</span><br />
abusso? trova <span class="uni">&#7936;&#946;&#973;&#963;&#963;&#959;&#965;</span> e <span class="uni">&#7940;&#946;&#965;&#963;&#963;&#959;&#957;</span><br />
a*el trova <span class="uni">&#7949;&#946;&#949;&#955;</span></p>
<p>Per cercare una radice invece di una parola, bisogna mettere / davanti alla radice scritta nello stesso modo. Si pu&ograve; attaccare la radice alla parola per specificare quale ricercare.</p>
<p><b>Esempi:</b><br />
/abussos trova <span class="uni">&#7936;&#946;&#973;&#963;&#963;&#959;&#965;</span> e <span class="uni">&#7940;&#946;&#965;&#963;&#963;&#959;&#957;</span><br />
autou/autos trova <span class="uni">&#945;&#8016;&#964;&#959;&#8166;</span> come pronome o aggettivo<br />
autou/autou trova <span class="uni">&#945;&#8016;&#964;&#959;&#8166;</span> come avverbio</p>
<p>Si pu&ograve; ricercare una parola anche per la forma grammaticale, iniziando con il simbolo #. Dopo il simbolo vanno messe una o pi&ugrave; parole per indicare la forma desiderata. Se c'&egrave; pi&ugrave; di una parola, bisogna separarle con un trattino.
Il seguente elenco indica le parole che si possono usare, non pi&ugrave; di una da ogni riga. Alcune scelte escludono altre: per esempio una preposizione non ha altre caratteristiche, e quindi cercare una preposizione e un'altra cosa restituir&agrave; sempre 0 versetti.
Si pu&ograve; anche usare delle abbreviazioni, a patto che contengano almeno le lettere in grassetto.</p>
<p><b>ve</b>rbo, <b>so</b>stantivo, <b>av</b>verbio, <b>ag</b>gettivo, <b>ar</b>ticolo, pronome dimostrativo (abbrev. <b>pd</b>), pronome interrogativo/indefinito (abbrev. <b>pin</b>), pronome personale/possessivo (abbrev. <b>pp</b>), pronome relativo (abbrev. <b>prel</b>), <b>prep</b>osizione, <b>congiunz</b>ione, <b>int</b>eriezione, <b>partic</b>ella<br />
<b>pri</b>ma persona, <b>se</b>conda persona, <b>t</b>erza persona<br />
<b>pres</b>ente, <b>imperf</b>etto, <b>fu</b>turo, <b>ao</b>risto, <b>pe</b>rfetto, <b>piu</b>ccheperfetto<br />
<b>at</b>tivo, <b>me</b>dio, <b>pas</b>sivo<br />
<b>ind</b>icativo, <b>impera</b>tivo, <b>congiunt</b>ivo, <b>o</b>ttativo, <b>inf</b>inito, <b>partici</b>pio<br />
<b>nom</b>inativo, <b>vo</b>cativo, <b>acc</b>usativo, <b>g</b>enitivo, <b>da</b>tivo<br />
<b>si</b>ngolare, <b>plur</b>ale<br />
<b>ma</b>schile, <b>fe</b>mminile, <b>ne</b>utro<br />
<b>com</b>parativo, <b>sup</b>erlativo</p>
<p>&Egrave; possibile attaccare la forma grammaticale alla parola e/o alla radice.</p>
<p><b>Esempi:</b><br />
#o trova tutti i verbi ottativi<br />
autou#pp-ne trova <span class="uni">&#945;&#8016;&#964;&#959;&#8166;</span> come pronome personale neutro (cio&egrave; non maschile e non avverbio)<br />
t????/o*#pd-nom trova ogni parola di cinque lettere con una radice che inizia con o e che &egrave; un pronome dimostrativo nominativo
</p>
<p>Invece delle lettere traslitterate, &egrave; possibile inserire le parole usando Unicode (UTF-8). Per&ograve; &egrave; difficile digitare le lettere in questo formato! Si pu&ograve; comunque copiare e incollare una parola di una pagina di questo sito, o da un altro programma, per fare una ricerca.</p>
<p>L'espressione da ricercare pu&ograve; contenere diverse parole, con simboli fra le parole per indicare il tipo di ricerca. Se c'&egrave; uno spazio, i versetti che contengono tutte e due le parole saranno trovati. Se c'&egrave; il carattere | oppure il carattere !, i versetti che contengono almeno una delle parole saranno trovati.
I caratteri ~ e ^ indicano NON la parola seguente. Infine, un numero da 1 a 9 indica la prima parola e, entro quel numero di versetti, anche la seconda parola.
Le parentesi possono essere usate per creare ricerche pi&ugrave; complicate.</p>
<p><b>Esempi:</b><br />
abba /kraz&#244; trova tutti i versetti che contengono <span class="uni">&#945;&#946;&#946;&#945;</span> e una parola con la radice <span class="uni">&#954;&#961;&#8049;&#950;&#969;</span><br />
i&#234;sous kurios~christos trova tutti i versetti che contengono le parole <span class="uni">&#7992;&#951;&#963;&#959;&#8166;&#962;</span> e <span class="uni">&#954;&#8059;&#961;&#953;&#959;&#962;</span> ma non <span class="uni">&#935;&#961;&#953;&#963;&#964;&#8057;&#962;</span><br />
(/silas|/silouanos)5/timotheos^/timotheos trova tutti i versetti che contengono una delle forme del nome Sila ed entro 5 versetti anche Timoteo, ma non Timoteo nel versetto stesso
</p>
<p>Per cercare le parole in ordine in una frase, e non solo in qualsiasi posto in un versetto, mettile dentro le parentesi quadrate. Le parentesi e il simbolo per OPPURE possono essere usati dentro le parentesi quadrate.
I numeri hanno un significato diverso: la seconda parola deve apparire entro quel numero di parole della prima. Se ci sono pi&ugrave; di due parole, il numero &egrave; sempre relativo alla prima parola, come nel secondo esempio.</p>
<p><b>Esempi:</b><br />
[ampliaton 9 ourbanon] trova le persone Ampliato e Urbano vicine - nota che la seconda parola non deve essere nello stesso versetto della prima<br />
[/metanoia eis2/afesis] trova i versetti che parlano del ravvedimento per perdono, con le tre parole in quell'ordine<br />
[(/air&#244;|/lamban&#244;) 2 stauron] trova tutti i versetti che contengono uno dei due verbi e non pi&ugrave; di due parole dopo l'accusativo della parola per croce<br />
a*i?? [#ind-sing 5/pisteu&#244;]^(d*|tounantion)3theos trova uno versetto
</p>
<p>Dopo aver digitato l'espressione da ricercare, puoi mettere il riferimento del brano in cui vuoi ricercare l'espressione. Il sito &egrave; in grado di capire quasi qualsiasi forma di riferimento che si potrebbe usare. Lasciando il campo vuoto, la ricerca sar&agrave; eseguita in tutto il Nuovo Testamento.
</p>
<p>Infine, bisogna scegliere in quale testo greco eseguire la ricerca.</p>

<h2>Trova parole</h2>
<p>In questa ricerca si possono trovare tutte le parole con certe caratteristiche.
Se si seleziona una forma particolare della parola (verbo, sostantivo, ecc.), ulteriori opzioni vengono attivate.
A volte non tutte queste opzioni addizionali hanno senso; per esempio, se verbo &egrave; selezionato, tutti i participi nella prima persona possono essere ricercati, anche se non esistono tali participi.
Se almeno una di queste opzioni addizionali &egrave; selezionata, il sito cerca le parole. Se non, il sito cerca le radici con queste caratteristiche.
Digitando i riferimenti, si possono confrontare le parole in due brani del NT (per cercare in tutto il NT, lascia il campo vuoto). Per esempio, tutte le parole nel NT che appaiono almeno due volte in un certo brano, o tutte le parole in un brano che appaiono non pi&ugrave; di 10 volte in un altro brano.
</p>

<h2>Opzioni</h2>
<p>1. Puoi scegliere se visualizzare le letture varianti, oppure solo il testo greco.</p>
<p>2. Puoi scegliere l'ordine in cui i manoscritti saranno elencati:</p>
<ul>
<li>per il tipo di manoscritto, cio&egrave; l'ordine descritto qui sotto;</li>
<li>per la data del manoscritto, prima i pi&ugrave; antichi;</li>
<li>per il tipo di testo dei manoscritti, prima quelli con il testo alessandrino, poi cesareano, poi occidentale e infine bizantino.
<b>Nota:</b> nel database ogni manoscritto appartiene ad un solo tipo di testo, quindi per esempio il manoscitto A &egrave; sempre messo fra i manoscritti
con testo alessandrino anche se &egrave; bizantino nei Vangeli.</li>
</ul>
<p>3. Puoi scegliere di anche visualizzare il testo secondo Westcott e Hort, e il testo secondo Tischendorf (ottava edizione). Per Tischendorf c'&egrave; stata qualche modifica ai numeri dei versetti (per esempio alla fine di Giovanni 1) per usare gli stessi numeri del testo di NA/UBS.</p>
<p>4. Puoi scegliere di visualizzare i riferimenti ai versetti scelti nei Padri prima del concilio di Nicea. Questi riferimenti sono stati presi dal sito <a href="http://www.earlychristianwritings.com/e-catena/">Early Christian Writings</a>, che ha creato un indice di questi padri nel sito del <a href="http://ccel.org/fathers2/">Christian Classics Ethereal Library</a>. Ci sono per&ograve; degli errori nel sito di <i>Early Christian Writings</i>, di cui ne ho corretti alcuni.
Per ogni versetto, ogni riferimenti nei Padri &egrave; su una riga diversa, con il titolo del libro del Padre, un collegamento al testo completo, e una piccola frase (in inglese) che contiene l'allusione.</p>
<p>5. Puoi scegliere, quando ci sono diversi testi (i testi greci, le varianti e le allusioni), di visualizzarli uno dopo l'altro verticalmente oppure orizzontalmente in colonne parallele.</p>
<p>6. Puoi scegliere la lingua da usare per queste pagine (italiano o inglese).</p>
<p>7. Puoi scegliere il font Unicode da utilizzare per il testo greco - vedi la pagina dei <a href="font.php">font</a> per una spiegazione.</p>

<h2>Link</h2>
<p>Per approfondire lo studio della critica testuale in italiano,
vedi <a href="http://www.christianismus.it/sezscritti/cop/index.html">Christianismus</a> e
la <a href="http://www.chiesariformatasalerno.net/documents/30.html">Chiesa Riformata Evangelica di Salerno</a>. In inglese, c'&egrave; un'introduzione pi&ugrave;
dettagliata nell'<a href="http://www.skypoint.com/~waltzmn/">Encyclopedia of New Testament Textual Criticism</a>; vedi anche i siti elencati
a <a href="http://www.ntgateway.com/resource/textcrit.htm">New Testament Gateway</a>.</p>

<p>Indietro al <a href="index.php">Nuovo Testamento greco</a>.</p>
<?}else{?>
<h1>New Testamento manuscripts - Instructions</h1>
<p>This site gives the main variants of the Greek New Testament - since there are thousands of NT manuscripts, it is natural that there are
differences between them, and it is not always easy to determine what the original reading was, even through for 99% of the verses the
original text is clear.</p>

<p>There is also some information on the Greek words in the text. Move the cursor above a word (without clicking) to see its grammatical form and definition, and click on a parola for a definition and other information.</p>

<p>To show the Greek text and variants, choose a book and type the reference of a passage (for example 3:2-4,6, 4:7) and click the button <i>View Text</i>.</p>

<p>The variants of a verse, if they exist, are listed separated by a blank line. The text that the site uses (UBS/NA26) is always given
first and the alternative readings follow on the next lines. Every reading is followed by the evidence, that is the manuscripts
(or some of the manuscripts) that contain it. If the mouse is left briefly on the name of a manuscripts, you will be able to see the date and
text type of the manuscript (which can be useful when a manuscript has several text types, and you want to check the text type of the
manuscript in the third ordering in the options).</p>

<p>The default ordering of the manuscripts is: papyri, uncials, families, minuscules, lectionaries, ancient versions, Fathers, editions and Italian translations.</p>
<ul>
<li><b>Papyri:</b> The manuscripts written on papyrus. They are indicated by the letter p and a superscript number.</li>
<li><b>Uncials:</b> The manuscripts written with capital letters. They are listed with the Hebrew letter aleph (<span class="uni">&#8237;&#1488;</span>), a capital letter (Latin or Greek) or a number that begins with zero.</li>
<li><b>Families:</b> Two families of minuscules are used: f1 (=1, 118, 131, 209, 1582) and f13 (=13, 69, 124, 174, 230, 346, 543, 788, 826, 828, 983, 1689, 1709).</li>
<li><b>Minuscles:</b> The manuscripts written with small letters. They are listed by a number. Byz indicates the majority of the Byzantine manuscripts; Byz<sup>2005</sup> indicates Robinson's 2005 edition when different from previous editions.</li>
<li><b>Lectionaries:</b> Lect indicates the majority of the lectionaries. Individual lectionaries are indicated by the letter l and a superscript number. After the number, 'pt' (=partly) indicates that the verse appears at least two times, with different readings; 'm' indicates the Menologion reading (those for special days); 's' indicates the Sinaxarion reading (those for the liturgical year).</li>
<li><b>Ancient versions:</b> The translations of the New Testament made in the first centuries after Christ.</li>
<li><b>Fathers:</b> The quotations by Christian authors in the first centuries after Christ.</li>
<li><b>Editions:</b> Two other editions of the Greek text, when different from the UBS/NA26 text, are indicated:
the Textus Receptus (<span class="uni">&#962;</span>), the edition of Westcott and Hort (WH), and the Editio Critica Maior (ECM). Some times the different editions of the Textus Receptus
have different readings; in such cases the editions of Stephanus (1550) and of Scrivener (1894) are cited.
11 times the Nestle-Aland text is different from that of the UBS, once the editions of UBS and once the editions of NA are different; these cases are indicated by NA and UBS.</li>
<li><b>Italian translations:</b> Eight versions are listed: the Nuova Riveduta (NR), the C.E.I. or Gerusalemme (CEI), the Nuova Diodati (ND), the Riveduta/Luzzi (Riv),
the Diodati (Dio), the Traduzione interconfessionale in lingua corrente (TILC), the Nuovissima (Nv) and the Nuovo Mondo (NM).
When the NR or the Riveduta put an alternative reading in a footnote, it is indicated by an m.
The aim here is not to list all the different translations of these versions, but to indicate which manuscripts they followed.</li>
</ul>

<p>The <a href="manoscritti.php">manuscripts</a> page has more information on the codes used for the manuscripts, and also their date, contents and text type.</p>

<h4>Abbreviations</h4>
<p>In the text of the variants the following Latin abbreviations are used:<br />
<i>omit</i> (= omittit, omittunt) omit the following text<br />
<i>add</i> (= addit, addunt) add the following text</p>

<p>A manuscripts can be accompagnied by the following signs:<br />
(...) has some minor difference<br />
[...] the text in the manuscript is between square brackets<br />
* the original reading<br />
<sup>a,b,c</sup> three manoscripts of the writings of Theophylact<br />
<sup>c</sup> a correction; when different people have corrected a manuscript they are indicated by 1, 2, 3 etcetera<br />
<sup>text</sup> the text of a manuscript or Father when different from the text in the commentary that accompagnies the text<br />
<sup>lem</sup> a quotation in a lemma of a Father, that is the text the precedes a commentary<br />
<sup>comm</sup> a quotation in the text of a commentary of a Father, when different from the Biblical text quoted<br />
<sup>mg</sup> a reading in the margin<br />
<sup>dub</sup> a quotation from a Father, where there is doubt that it is the work of that Father<br />
<sup>gr</sup> the Greek text of a Father<br />
<sup>lat</sup> the Latin text of a Father<br />
<sup>arm</sup> the Armenian text of a Father<br />
<sup>syr</sup> the Syriac text of a Father<br />
<sup>slav</sup> the Slavonic text of a Father<br />
<sup>arab</sup> the Arab text of a Father<br />
<sup><i>vid</i></sup> (=videtur) the reading is not certain, for example if the document is damaged<br />
<sup>v.r.</sup> variant reading specifically indicated in the manuscript as an alternative<br />
<sup>2/3</sup> etc the second number gives how many times the passage is quoted by a Father, the first number how many times it is quoted in the form of that variant</p>

<p>At the end of a group of manuscripts or of all the evidence, the following can be added:<br />
<i>pc</i> (=pauci) also a few other manuscripts (less than 5% of all the manuscripts)<br />
<i>al</i> (=alii) many manuscripts (from 5 to 25%)<br />
<i>pm</i> (=permulti) most of the manuscripts (from 25 to 70%)<br />
<i>pl</i> (=plerique) almost all the other manuscripts<br />
pt part of the group<br />
ms one manuscript of that group<br />
mss some manuscripts of that group</p>

<p>Other abbreviations:<br />
<i>(ex lat?)</i> the variant might have been influenced by a part of all of the Latin translation<br />
p) after a variant in the Gospels indicates that it was possibly influenced by a parallel passage<br />
f,ff (=uerses sequens, sequentes) and the following<br />
<i>sic</i> an error reproduced exactly</p>

<!--<h2>Compare manuscripts</h2>
<p>This search lists all the variants in the database in which two manuscripts have the same or different readings.
However, since the database does not contain all the reading of every manuscript, the lists are not complete.
You can restrict the listed variants to one part or the Bible - the site can understand almost any reference that you might type, or you can leave the field blank to search in all the NT.
For the names that must be used for the the manuscripts, see the <a href="sigle.php">allowed codes for the manuscripts</a>.</p>
-->
<a name="TrovaVersetti"></a>
<h2>Find verses</h2>

<p>The simplest search is to look for all the verses that contain a word. You need to type the word with normal letters, without accents or an initial h, and in lower case letters, using the following scheme:</p>
<table>
<tr><td><span class="uni">&#945;</span></td><td>a</td></tr>
<tr><td><span class="uni">&#946;</span></td><td>b</td></tr>
<tr><td><span class="uni">&#947;</span></td><td>g</td></tr>
<tr><td><span class="uni">&#948;</span></td><td>d</td></tr>
<tr><td><span class="uni">&#949;</span></td><td>e</td></tr>
<tr><td><span class="uni">&#950;</span></td><td>z</td></tr>
<tr><td><span class="uni">&#951;</span></td><td>&#234;</td></tr>
<tr><td><span class="uni">&#952;</span></td><td>th</td></tr>
<tr><td><span class="uni">&#953;</span></td><td>i</td></tr>
<tr><td><span class="uni">&#954;</span></td><td>k</td></tr>
<tr><td><span class="uni">&#955;</span></td><td>l</td></tr>
<tr><td><span class="uni">&#956;</span></td><td>m</td></tr>
<tr><td><span class="uni">&#957;</span></td><td>n</td></tr>
<tr><td><span class="uni">&#958;</span></td><td>x</td></tr>
<tr><td><span class="uni">&#959;</span></td><td>o</td></tr>
<tr><td><span class="uni">&#960;</span></td><td>p</td></tr>
<tr><td><span class="uni">&#961;</span></td><td>r</td></tr>
<tr><td><span class="uni">&#963;</span></td><td>s</td></tr>
<tr><td><span class="uni">&#964;</span></td><td>t</td></tr>
<tr><td><span class="uni">&#965;</span></td><td>u <i>or</i> y</td></tr>
<tr><td><span class="uni">&#966;</span></td><td>f <i>or</i> ph</td></tr>
<tr><td><span class="uni">&#967;</span></td><td>ch</td></tr>
<tr><td><span class="uni">&#968;</span></td><td>ps</td></tr>
<tr><td><span class="uni">&#969;</span></td><td>&#244;</td></tr>
</table>
<p>To type &#234; and &#244;, in Windows hold the key ALT down whilst you type 0234 or 0244 with the numeric keyboard. It is also possible to use ? instead of a letter and * for any number of letters.</p>
<p><b>Examples:</b><br />
abussou finds <span class="uni">&#7936;&#946;&#973;&#963;&#963;&#959;&#965;</span><br />
abusso? finds <span class="uni">&#7936;&#946;&#973;&#963;&#963;&#959;&#965;</span> and <span class="uni">&#7940;&#946;&#965;&#963;&#963;&#959;&#957;</span><br />
a*el finds <span class="uni">&#7949;&#946;&#949;&#955;</span></p>
<p>To search for a root instead of a word, you need to put / in front of the root written in the same way. You can attach the root to the word to specify which one to search for.</p>
<p><b>Examples:</b><br />
/abussos finds <span class="uni">&#7936;&#946;&#973;&#963;&#963;&#959;&#965;</span> and <span class="uni">&#7940;&#946;&#965;&#963;&#963;&#959;&#957;</span><br />
autou/autos finds <span class="uni">&#945;&#8016;&#964;&#959;&#8166;</span> as a pronoun or adjective<br />
autou/autou finds <span class="uni">&#945;&#8016;&#964;&#959;&#8166;</span> as an adverb</p>
<p>You can also search for a word according to its grammatical form, starting with the symbol #. After the symbol one or more words are put to indicate the form to search for. If there is more than one word, you need to separate them with hyphens.
The following list indicates the words that you can use, but no more than one from each line. Some choices exclude others: for example a preposition does not have other characteristics, and so search for a preposition and something else will always return 0 verses.
You can also use abbreviations, as long as they contain at least the letters in bold.</p>
<p><b>ve</b>rb, <b>nou</b>n, <b>adv</b>erb, <b>adj</b>ective, <b>ar</b>ticole, demonstrative pronoun (abbrev. <b>dp</b>), interrogative/indefinite pronoun (abbrev. <b>ip</b>), personal/possessive pronoun (abbrev. <b>pp</b>), relative pronoun (abbrev. <b>rp</b>), <b>prep</b>osition, <b>conj</b>unction, <b>int</b>erjection, <b>particl</b>e<br />
<b>fi</b>rst person, <b>se</b>cond persone, <b>t</b>hird person<br />
<b>pres</b>ent, <b>imperf</b>ect, <b>fu</b>ture, <b>ao</b>rist, <b>pe</b>rfect, <b>plup</b>erfect<br />
<b>act</b>ive, <b>mi</b>ddle, <b>pas</b>sive<br />
<b>ind</b>icative, <b>impera</b>tive, <b>sub</b>junctive, <b>o</b>ptative, <b>inf</b>initive, <b>partici</b>ple<br />
<b>nom</b>inative, <b>vo</b>cative, <b>acc</b>usative, <b>g</b>enitive, <b>da</b>tive<br />
<b>si</b>ngular, <b>plur</b>al<br />
<b>ma</b>sculine, <b>fe</b>minine, <b>ne</b>uter<br />
<b>com</b>parative, <b>sup</b>erlative</p>
<p>It is also possible to attach the grammatical form to the word and/or to the root.</p>
<p><b>Examples:</b><br />
#o finds all the optative verbs<br />
autou#pp-ne finds <span class="uni">&#945;&#8016;&#964;&#959;&#8166;</span> as the neuter personal pronoun (that is not masculine and not an adverb)<br />
t????/o*#pd-nom finds every word of five letters with a root that begins with o and that is a nominative dimostrative pronoun
</p>
<p>Instead of the translitterated letters, you can insert the words using Unicode (UTF-8). However, it is difficult to type the letters in this format. You can however copy and paste a word of a page of this sito, or from another progam, to do a search.</p>
<p>The search expression can contain several words, with symbols between the words to indicate the type of search. If there is a space, the verses that contain both the words will be found. If there is the character | or the character !, the verses that contain at least one of the words will be found.
The characters ~ and ^ mean NOT the following word. Finally, a digit from 1 to 9 means the first word and, within that number of verses, also the second.
Parenthesis can be used to create more complicated researches.</p>
<p><b>Examples:</b><br />
abba /kraz&#244; finds all the verses than contain <span class="uni">&#945;&#946;&#946;&#945;</span> and a word with the root <span class="uni">&#954;&#961;&#8049;&#950;&#969;</span><br />
i&#234;sous kurios~christos finds all the verses than contain the words <span class="uni">&#7992;&#951;&#963;&#959;&#8166;&#962;</span> and <span class="uni">&#954;&#8059;&#961;&#953;&#959;&#962;</span> but not <span class="uni">&#935;&#961;&#953;&#963;&#964;&#8057;&#962;</span><br />
(/silas|/silouanos)5/timotheos^/timotheos finds all the versetti than contain one of the forms of the name Silas and within five verses also Timothy, but not with Timothy in the same verse
</p>
<p>To search the words in order in a phrase, and not only in any place in the verse, put them inside square brackets. The parenthesis and the symbol for OR can be used inside the square brackets as well.
The numbers have a different meaning: the second word must appear within that number of words from the first. If there are more than two words, the number is always relative to the first, as in the second example.</p>
<p><b>Examples:</b><br />
[ampliaton 9 ourbanon] finds the names Ampliatus and Urbanus close together - note that the second word does not have to appear in the same verse as the first<br />
[/metanoia eis2/afesis] finds the verses that speak of repentence for forgiveness, with the three words together in that order<br />
[(/air&#244;|/lamban&#244;) 2 stauron] finds the verses that contain one of the two verbs and no more than two words after the accusative of the word for cross<br />
a*i?? [#ind-sing 5/pisteu&#244;]^(d*|tounantion)3theos finds one verse
</p>
<p>After having typed the search expression, you can put the reference of the passage in which you want to search the expression. The site is able to understand almost any form of reference that you might use. Leaving the field empty, the search will be done in all the New Testament.
</p>
<p>Finally, you need to choose which Greek text you want to search.</p>

<h2>Find words</h2>
<p>In this search you can find all the words with certain characteristics.
If you choose a certain form of the word (verb, noun, etc.), further options are activate.
Sometimes not all these extra options are meaningful; for example, if verb is chosed, and the first person participles can be search for, even if such participles don't exist.
If at least one of these extra options is chosen, the site searches for the words. Otherwise, the site searches for the roots with these characteristics.
Typing the references, you can compare the words in two passages of the NT (to search in all of the NT, leave the field blank). For example, all the words in the NT that appear at least twice in a certain passage, or all the words of a passage that appear less than 10 times in another passage.
</p>

<h2>Options</h2>

<p>1. You can choose whether to view the variant readings, or only the Greek text.</p>
<p>2. You can choose the order in which the manuscripts will be listed:</p>
<ul>
<li>by the type of manuscript, that is the order described below;</li>
<li>by the date of the manuscript, oldest first;<li>
<li>by the text type of the manuscript, first those with an Alexandrian text, then C&aelig;sarean, then Western and finely Byzantine.
<b>Note:</b> in the database every manuscript belongs to only one type, so for example the manuscript A is always put amongst the Alexandrian
manuscripts even though it is Byzantine in the Gospels.</li>
</ul>
<p>3. You can choose to also view the text of Westcott and Hort, and the text of Tischendorf (eighth edition). For Tischendorf there were some changes made to the verse numbers (for example at the end of John 1) to use the same numbers as the NA/UBS text.</p>
<p>4. You can choose to view the references to the chosen verses in the Ante-Nicene Fathers. There references were taken from the site <a href="http://www.earlychristianwritings.com/e-catena/">Early Christian Writings</a>, that had created an index of these Fathers on the site of the <a href="http://ccel.org/fathers2/">Christian Classics Ethereal Library</a>. There are however some errors on the site of <i>Early Christian Writings</i>, of which I have corrected some.
For every verse, every reference in the Fathers is on a different line, with the title of the book of the Fathers, a link to the complete text, and a small phrase that contains the allusion.</p>
<p>5. You can choose, when there are multiple texts (the Greek texts, the variants and the allusions), to view them one after another vertically or horizonally in parallel columns.</p>
<p>6. You can also choose the language to use for these pages (English or Italian).</p>
<p>7. You can choose the Unicode font to use per the Greek text - see the <a href="font.php">font</a> page for an explanation.</p>

<h2>Links</h2>
<p>For more information on Text Criticism, see the <a href="http://www.skypoint.com/~waltzmn/">Encyclopedia of New Testament Textual Criticism</a>
and the sites listed at the <a href="http://www.ntgateway.com/resource/textcrit.htm">New Testament Gateway</a>.</p>

<p>Back to the <a href="index.php">Greek New Testament</a>.</p>
<?}?>
</body>
</html>

