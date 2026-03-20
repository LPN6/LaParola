<!DOCTYPE html>
<html lang="it">
<head>
<meta charset="UTF-8">
<link rel="stylesheet" href="/stili/stilebase6book.css" type="text/css" />
</head>
<body>
<?
include("../conn.php");
?>

<!--
different versions of the book with different lines (no lemma, translit x 2, grammatical analysis)
-->

<style>
body {
    font-family: "Times New Roman", "Cardo", serif;
}
.title-page {
  text-align: center;
  page-break-after: always;
  margin-top: 90mm;
}

.title-page h1 {
  font-family: "Times New Roman", serif;
  font-size: 40pt;
  font-weight: bold;
  margin-bottom: 18pt;
  letter-spacing: 0.03em;
}

.title-page h2 {
  font-size: 20pt;
  font-weight: normal;
  margin-bottom: 120pt;
}

.title-page .author {
  font-size: 18pt;
  font-weight: normal;
    font-variant: small-caps;
  letter-spacing: 0.05em;
}

.copyright-page {
  page-break-after: always;
  font-family: "Times New Roman", serif;
  font-size: 9.5pt;
  line-height: 1.3;
/*  width: 70%;*/
  margin-top: 170mm; /* pushes content to lower half */
}

.copyright-page p {
  text-align: center;
  margin: 0 0 4pt 0;
}

.copyright-page .isbn {
  margin-top: 10pt;
  letter-spacing: 0.03em;
}

/* Title */
.toc-title {
  text-align: center;
  font-size: 18pt;
  font-weight: bold;
  letter-spacing: 1px;
  margin-bottom: 2em;
}

/* TOC list */
.toc {
  list-style: none;
  padding: 0;
  margin: 0;
}

/* Each entry */
.toc li {
  display: flex;
  align-items: baseline;
  margin-bottom: 0.4em;
}

/* Book title */
.toc .book {
  white-space: nowrap;
}

/* Dot leader */
.toc .dots {
  flex: 1;
  border-bottom: 1px dotted #000;
  margin: 0 8px;
  transform: translateY(-3px);
}

/* Page number */
.toc .page {
  white-space: nowrap;
}

.introduction-page {
    page-break-before: always;   /* start on a new page */
    page-break-after: always;    /* ensure the next section starts on a new page */
    display: block;
    text-align: justify;          /* or center if you prefer */
    font-family: "Times New Roman", serif;
    font-size: 12pt;
    line-height: 1.3;
}

.introduction-page h1 {
    text-align: center;
    font-size: 18pt;
    font-weight: bold;
    margin-bottom: 1em;
}
.introduction-page h2 {
    font-size: 14pt;                        /* slightly smaller than the main h1 title */
    font-weight: bold;                       /* stand out from paragraphs */
    margin-top: 1em;                         /* space above the subheading */
    margin-bottom: 0.5em;                    /* space below before the paragraph */
    text-align: left;                        /* align to the left for hierarchy clarity */
    page-break-after: avoid;                 /* do not break page after subheading */
}

.introduction-page p {
    margin-bottom: 1em;
}

.wider-first {
  width: 100%;
  table-layout: auto;   /* Important: allow natural column sizing */
}

.wider-first td.no-wrap {
  white-space: nowrap;   /* Prevent wrapping */
}

.wider-first td:last-child {
  width: 100%;           /* Make second column take available space */
}

.main-text {
    page: main;
}

sup {
    font-size: 0.7em;
    vertical-align: 0.3em;
}
.container {
    display: block;
      text-align: justify;
  word-spacing: -0.02em;
}
.text-block {
  display: inline-block;
  vertical-align: top;
  margin-right: 10px;
  margin-bottom: 2pt; /* was 4; 2 or 1? */
  break-inside: avoid;
  page-break-inside: avoid;
  widows: 1;
  orphans: 1;
}
.line {
  text-align: center;
  white-space: nowrap;
  line-height: 1.04;
}
  .versetto {
    font-size: 7.5pt; /* 7.5? */
    font-weight: 600;
  border: 1pt solid #555;
  border-radius: 2pt;
      padding: 0px 1px;
    margin-right: 0px;
    vertical-align: top;
    display: inline-block;
  }  

@media print {
  body {
    font-family: "Times New Roman", "Cardo", serif;
    font-size: 10.5pt;
    line-height: 1.04;
    color: #000;
  }
  h1 {
    margin-top: 0pt;
    text-align: center;
    break-before: page;
    page-break-before: always;
    string-set: book-title content(text);
  }
  h2 {
    string-set: chapter content(text);
    break-after: avoid;
    page-break-after: avoid;
  }

  .line {
    display: block;
    text-align: center;
    line-height: 1.04;
    white-space: nowrap;
    margin: 0;
    padding: 0;
  }

  .line:first-child {
    font-size: 10.5pt; /* was 11 */
    font-weight: 500;
  letter-spacing: -0.015em;    
  }

  .line:not(:first-child) {
    font-size: 9pt; /* 8.5? */
    font-weight: 400;
    color: #111;
    letter-spacing: -0.01em;    
  }
}

 /*   @page {
    margin: 14mm 10mm 16mm 10mm;
  }*/
@page {
    margin-top: 16mm;
    margin-bottom: 10mm;
    @top-left {
        content: none;  /* no running header */
    }
    @top-right {
        content: none;  /* no running header */
    }
}

@page :right {
    margin-left: 15mm;
    margin-right: 10mm;
}

@page :left {
    margin-left: 10mm;
    margin-right: 15mm;
}

@page main :right {
    @top-left {
        content: counter(page);
        font-size: 9pt;
        padding-left: 4mm; 
    }
    @top-right {
        content: string(chapter);
        font-size: 9pt;
        font-weight: bold;
        padding-right: 2mm;
    }
}

@page main :left {
  @top-right {
    content: counter(page);
    font-size: 9pt;
    padding-right: 4mm;
  }
  @top-left {
    content:string(chapter);
    font-size: 9pt;
    font-weight: bold;
    padding-left: 2mm;
  }
}

</style>

<div class="title-page">

  <h1>
    Traduzione interlineare<br />
    del Nuovo Testamento
  </h1>

  <h2>
  </h2>

  <div class="author">
    Richard Wilson<br />
    Daniele Wilson
  </div>

</div>

<div class="copyright-page">

  <p>Quest'opera &egrave; stata rilasciata con licenza</p>
  <p>Creative Commons Attribuzione 4.0 Internazionale</p>
  <p>https://creativecommons.org/licenses/by/4.0/deed.it</p>
  <p>tranne il testo della Riveduta 2020, che &egrave;</p>
  <p>Copyright &copy; 2020, ADI-Media</p>

  <p class="isbn">
    ISBN-13: 9798248577026
  </p>

</div>

<div class="toc-title">SOMMARIO</div>
<ul class="toc">
  <li><span class="book">Matteo</span><span class="dots"></span><span class="page">5</span></li>
  <li><span class="book">Marco</span><span class="dots"></span><span class="page">64</span></li>
  <li><span class="book">Luca</span><span class="dots"></span><span class="page">101</span></li>
  <li><span class="book">Giovanni</span><span class="dots"></span><span class="page">164</span></li>
  <li><span class="book">Atti</span><span class="dots"></span><span class="page">212</span></li>
  <li><span class="book">Romani</span><span class="dots"></span><span class="page">276</span></li>
  <li><span class="book">1Corinzi</span><span class="dots"></span><span class="page">302</span></li>
  <li><span class="book">2Corinzi</span><span class="dots"></span><span class="page">326</span></li>
  <li><span class="book">Galati</span><span class="dots"></span><span class="page">343</span></li>
  <li><span class="book">Efesini</span><span class="dots"></span><span class="page">352</span></li>
  <li><span class="book">Filippesi</span><span class="dots"></span><span class="page">361</span></li>
  <li><span class="book">Colossesi</span><span class="dots"></span><span class="page">367</span></li>
  <li><span class="book">1Tessalonicesi</span><span class="dots"></span><span class="page">373</span></li>
  <li><span class="book">2Tessalonicesi</span><span class="dots"></span><span class="page">379</span></li>
  <li><span class="book">1Timoteo</span><span class="dots"></span><span class="page">383</span></li>
  <li><span class="book">2Timoteo</span><span class="dots"></span><span class="page">390</span></li>
  <li><span class="book">Tito</span><span class="dots"></span><span class="page">395</span></li>
  <li><span class="book">Filemone</span><span class="dots"></span><span class="page">398</span></li>
  <li><span class="book">Ebrei</span><span class="dots"></span><span class="page">400</span></li>
  <li><span class="book">Giacomo</span><span class="dots"></span><span class="page">419</span></li>
  <li><span class="book">1Pietro</span><span class="dots"></span><span class="page">426</span></li>
  <li><span class="book">2Pietro</span><span class="dots"></span><span class="page">433</span></li>
  <li><span class="book">1Giovanni</span><span class="dots"></span><span class="page">438</span></li>
  <li><span class="book">2Giovanni</span><span class="dots"></span><span class="page">445</span></li>
  <li><span class="book">3Giovanni</span><span class="dots"></span><span class="page">446</span></li>
  <li><span class="book">Giuda</span><span class="dots"></span><span class="page">447</span></li>
  <li><span class="book">Apocalisse</span><span class="dots"></span><span class="page">449</span></li>
</ul>

<div class="introduction-page">
    <h1>Introduzione</h1>

<p>Questa traduzione interlineare del Nuovo Testamento contiene il testo greco e, sotto ogni parola, il lemma della parola, per aiutare a trovarla in un dizionario,
e come viene tradotta in italiano nella versione <i>Riveduta 2020</i>.
&Egrave; la versione cartacea della traduzione interlineare del sito <i>LaParola.net</i>,
e la versione elettronica a https://www.laparola.net/interlineare/ contiene pi&ugrave; informazioni di quanto era possibile includere in questo libro.</p>
<h2>Testo greco</h2>
<p>Il testo greco &egrave; quello che &egrave; stato effettivamente tradotto per creare la <i>Riveduta 2020</i>.
La base del testo visualizzato &egrave; l'edizione <i>SBL Greek New Testament</i> (https://www.sblgnt.com/) che &egrave; distribuita con licenza Creative Commons Attribuzione 4.0 Internazionale.
Le parentesi quadrate [...] nel testo indicano che le parole incluse sono dubbie.</p>
<p>Quando la versione <i>Riveduta 2020</i> traduce un testo greco diverso, un asterisco (*) &egrave; messo nel testo.
&Egrave; possibile controllare le diverse letture di alcune edizioni greche e di molti manoscritti cercando il versetto a https://www.laparola.net/greco/.</p>
<p>I lemmi delle parole greche sono presi da <i>MorphGNT SBLGNT</i> (https://github.com/morphgnt/sblgnt)
che &egrave; distribuito con la licenza Creative Commons Attribuzione - Condividi allo stesso modo 3.0 Unported.</p>
<h2>Testo italiano</h2>
<p>Sotto le parole greche &egrave; il testo della <i>Riveduta 2020</i>, edizione del 2025. Questo testo &egrave;<br />
Copyright &copy; 2020, ADI-Media<br />
Utilizzato con il permesso dell'editore. Tutti i diritti riservati in tutto il mondo.</p>
<p>Ringraziamo la casa editrice per l'autorizzazione di usare il testo, e la disponibilit&agrave; e la collaborazione per la creazione della versione interlineare.</p>
<p></p>
<p>Ci sono alcuni simboli usati nel testo italiano:</p>
<table class="wider-first">
<tr><td>&ndash;</td><td>&nbsp;</td><td>la parola greca non &egrave; tradotta in italiano</td></tr>
<tr><td class="no-wrap">-, + dopo una parola</td><td>&nbsp;</td><td>la parola italiana appare nel versetto precedente o successivo nel testo italiano relativo al testo greco</td></tr>
<tr><td>&gt;</td><td>&nbsp;</td><td>la parola greca &egrave; tradotta con la parola successiva per formare il testo italiano</td></tr>
<tr><td>&lt;</td><td>&nbsp;</td><td>la parola greca &egrave; tradotta con la parola precedente per formare il testo italiano</td></tr>
<tr><td>&gt;&gt;, &lt;&lt;</td><td>&nbsp;</td><td>la parola greca &egrave; tradotta con la parola dopo la successiva o prima della precedente; similmente per pi&ugrave; di due segni</td></tr>
<tr><td>{...}</td><td>&nbsp;</td><td>la parola italiana &egrave; stata aggiunta, non c'&egrave; una parola greca corrispondente</td></tr>
<tr><td>[...]</td><td>&nbsp;</td><td>simboli riportati dal testo italiano cartaceo, che racchiudono parole assenti in alcuni manoscritti</td></tr> 
<tr><td>numero in apice</td><td>&nbsp;</td><td>il numero della parola nell'ordine nel testo italiano</td></tr>
</table>
</div>

<div class="main-text">
<?
$capitolo = 2;
$rad = 1; $r2 = 1;
$books = [
    "Matteo", "Marco", "Luca", "Giovanni", "Atti", "Romani", "1Corinzi", "2Corinzi",
    "Galati", "Efesini", "Filippesi", "Colossesi", "1Tessalonicesi", "2Tessalonicesi",
    "1Timoteo", "2Timoteo", "Tito", "Filemone", "Ebrei", "Giacomo", "1Pietro", "2Pietro",
    "1Giovanni", "2Giovanni", "3Giovanni", "Giuda", "Apocalisse"
];
$chapters = [
    28, 16, 24, 21, 28, 16, 16, 13, 6, 6,
    4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22
];

$conn->set_charset("utf8");
for ($libro=1; $libro<=27; ++$libro) {
//for ($libro=1; $libro<=1; ++$libro) {
  //for ($icapitolo=8; $icapitolo<=8; ++$icapitolo) {
  for ($icapitolo=1; $icapitolo<=$chapters[$libro-1]; ++$icapitolo) {
$sql = "SELECT * FROM Interlineare WHERE (Libro=$libro) AND (Capitolo=$icapitolo)";
$inizioVersetto = false;
$versettoAttuale = 0;
$nextStar = 0;

if ($ris = mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris) > 0) {

    // Load all rows so we can look ahead
    $rows = [];
    while ($r = mysqli_fetch_assoc($ris)) {
        $rows[] = $r;
    }

    if ($icapitolo == 1)
        echo "<h1 id=\"".$books[$libro-1]."\" class=\"book-title\">".$books[$libro-1]."</h1>";

    echo "<h2>".$books[$libro-1]." ".$icapitolo."</h2>";
    echo "<div class=\"container\">";

    for ($i = 0; $i < count($rows); $i++) {

        $row  = $rows[$i];
        $next = $rows[$i+1] ?? null; // safe look-ahead

        // --- START: punctuation removal rule ---
        if ($next !== null) {
            $currentR2   = $row["R2"];
            $nextR2      = $next["R2"];
            $nextParola  = $next["Parola"];
            $currGreco    = $row["Greco"];
            $nextGreco    = $next["Greco"];
            // Both Versetto strings must end with punctuation
            if (preg_match('/([.,;:!?\x{00B7}])$/u', $currGreco, $m1) &&
                preg_match('/([.,;:!?\x{00B7}])$/u', $nextGreco, $m2))
            {
                $p1 = $m1[1];
                $p2 = $m2[1];

                if (
                    $currentR2 != "*" &&
                    $nextR2    != "*" &&
                    ($nextParola % 100 !== 0) &&
                    $p1 === $p2
                ) {
                    // Remove punctuation from the current Versetto
                    $row["Greco"] = preg_replace('/[.,;:!?\x{00B7}]$/u', '', $row["Greco"]);
                }
            }
        }
        // --- END: punctuation removal rule ---

        if ($row["Versetto"] != $versettoAttuale) {
            $inizioVersetto = true;
            $versettoAttuale = $row["Versetto"];
        } else {
            $inizioVersetto = false;
        }

        $parolaR2 = $row["R2"];
        if ($parolaR2 == "-") $parolaR2 = "&ndash;";

        if ($parolaR2 != "*") {

            echo "<div class=\"text-block\">";
            $p = $row["Greco"];

            if ($row["Parola"] % 100 !== 0) {
                $p = $p . "*";
            }
            else if ($nextStar == 1) {
                $p = "* " . $p;
            }

            echo "<div class=\"line\">"
                 . ($inizioVersetto ? "<div class=\"versetto\">".$row["Versetto"]."</div> " : "")
                 . $p
                 . "</div>";

            if ($rad) echo "<div class=\"line\">" . $row["Radice"] . "</div>";
            if ($r2)  echo "<div class=\"line\">" . $parolaR2 . "</div>";

            $nextStar = 0;
            echo "</div>";

        } else {
            if ($row["Parola"] % 100 == 0)
                $nextStar = 1;
        }
    }

    echo "</div>";
  }
  else {
      echo "<p>Errore nei risultati dal database.</p>";
  }
}
else {
  echo "<p>Errore nel collegamento al database.</p>";
  echo $sql;
}  }
}

?>
</div>
</body>
</html>