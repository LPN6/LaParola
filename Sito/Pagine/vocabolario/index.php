<?php
$descriz = "Un vocabolario greco-italiano del Nuovo Testamento";
$key = "greco,vocabolario,dizionario,greco antico,antico,greco-italiano,greco italiano,significato,Nuovo Testamento,NT,dizionario greco,vocabolario greco,nuovo testamento greco";
$titolo = "Vocabolario greco";
$sezione = "Strumenti";
require("../capo.php");
include("../conn.php");
?>
<h1>Vocabolario greco-italiano del Nuovo Testamento</h1>
<p class="primalettera">Questo vocabolario d&agrave; il significato in italiano di ogni parola usata nel testo greco del Nuovo Testamento (greco antico).
Per ogni parola, c'&egrave; anche un elenco di tutte le forme grammaticali che appaiono nel NT e come viene tradotta in due versioni italiane.</p>
<p>
<h2>Istruzioni</h2>
<p>Nella casella qui sotto, puoi cercare una parola in diversi modi:<br />
1. scrivendo con le lettere greche; se digiti anche gli accenti trovare la parola esatta,
ma puoi digitare le prime lettere senza gli accenti e il sito elencher&agrave; tutte le parole che iniziano con quelle lettere.<br />
2. scrivendo con lettere latine; se corrispondono alla traslitterazione di esattamente una parola greca sar&agrave; visualizzata,
altrimenti elencher&agrave; tutte le parole di cui la traslitterazione inizia con quelle lettere.
Per &#951; (eta) puoi usare una di: &egrave;, &eacute;, &emacr;; per &#969; (omicron) puoi usare una di: &ograve;, &oacute;, &omacr;.
Per &#950; (theta) 'th', &#966; (phi o fi) 'f', &#967; (chi) 'ch', &#968; (psi) 'ps'.
&Egrave; necessario inserire una 'h' all'inizio di una parola che comincia con uno spirito aspro, tranne nel caso di rho con uno spirito (&#x1FE5;) che &egrave; traslitterato 'rh'. Tutti gli altri accenti non devono essere inclusi. 
<br />
3. cliccare / toccare una delle lettere greche per elencare tutte le parole che iniziano con quella lettera.</p>
<?
require("indvocab.php");
?>
<h2>Altri formati</h2>
<p>Il vocabolario &egrave; anche disponibile:<br />
1. come componente facoltativo del <a href="/programma/windowsbeta.php">programma gratuito per Windows</a> (versione 8 o superiore);<br />
2. come <a href="https://www.amazon.it/dp/B0HQFL8ND2">libro cartaceo</a>, da Amazon;<br />
3. come <a href="Vocabolario del Nuovo Testamento.pdf">file PDF</a>; il formato &egrave; esattamente uguale al libro cartaceo.</p>
</p>
<h2>Altre risorse</h2>
<p>Per chi &egrave; interessato nel Nuovo Testamento in greco, su questo sito ci sono anche:<br />
- l'<a href="/interlineare/">interlineare greco-italiano</a> del Nuovo Testamento e<br />
- il <a href="/greco/">testo del Nuovo Testamento greco</a> con le letture varianti dei manoscritti, quattro testi greci, e altro ancora.</p>

<?
require("../piede.php");
?>
