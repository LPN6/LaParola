<?
$libro = (int)(isset($_REQUEST["libro"])?$_REQUEST["libro"]:1);
$capitolo = (int)(isset($_REQUEST["capitolo"])?$_REQUEST["capitolo"]:1);

require("../vistesto.php");
include("../conn.php");
global $conn;
$sql = "SELECT Nome,Numero FROM Libri WHERE Numero=$libro";
$libronome = "";
if ($ris=mysqli_query ($conn, "$sql")) {
  while ($row=mysqli_fetch_array ($ris))
    $libronome = $row["Nome"];
}
if ($libro==6) $libronome = "Giosuè";
$libroHTML = htmlentities($libronome, 0, "ISO-8859-1"); // per Giosuè

$descriz = "Informazioni su $libroHTML $capitolo nella Bibbia";
$key = $libroHTML;
$titolo = $libroHTML;
$sezione = "Bibbia per capitoli";
$sezioneurl = "";
require("../capo.php");
$apoc = ($libro==17 || $libro==18 || $libro==20 || $libro==21 || $libro==27 || $libro==28 || $libro==32) || ($libro==34 && $capitolo>=13);
$ceiextra = ($libro==36 && $capitolo==4); // Gioele 4
echo "<h1>$libroHTML $capitolo</h1>";
echo "<p>Un indice di pagine su <strong>$libroHTML $capitolo</strong>, con il testo biblico, commentari, e altre risorse.</p>";
if ($libro==23 && ($capitolo>=9 && $capitolo<=147))
    echo "<p>Nota che le versioni C.E.I. (1974), Ricciotti, Tinitori, e Martini usano l'enumerazione della traduzione greca dei Salmi, mentre le altre versioni e i commentari usano l'enumerazione del testo ebraico. Per questo motivo, il testo dal Salmo 10 al Salmo 147 in un'altra versione corrisponde al Salmo con il numero precedente in quelle 4 versioni.</p>";
if ($ceiextra)
    echo "<p>Il testo di Gioele 4 nella versione C.E.I. (1974) &egrave; nel capitolo 3 di tutte le altre versioni, per cui ci sono poche informazioni qui e pi&ugrave; informazioni sul testo del capitolo nell'<a href=\"capitolo.php?libro=36&capitolo=3\" title=\"Gioele 3\">indice per Gioele 3</a>.";
echo "<h2>Versioni italiane della Bibbia</h2>";
if (!$apoc && !$ceiextra)
    echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Nuova%20Riveduta\" title=\"Il capitolo nella versione Nuova Riveduta\">Nuova Riveduta</a></p>";
if ($libro != 46 || $capitolo != 4) {
    echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=C.E.I.\" title=\"Il capitolo nella versione C.E.I., edizione 1974\">C.E.I. (1974)</a></p>";
}
if (!$apoc && !$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Nuova%20Diodati\" title=\"Il capitolo nella versione Nuova Diodati\">Nuova Diodati</a></p>";
if (!$apoc && !$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Riveduta%202020\" title=\"Il capitolo nella versione Riveduta 2020\">Riveduta 2020</a></p>";
if (!$apoc && !$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Nuova%20Riveduta%201994\" title=\"Il capitolo nella versione Nuova Riveduta (1994)\">Nuova Riveduta (1994)</a></p>";
if ($libro >= 47) {
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Bibbia%20della%20Gioia\" title=\"Il capitolo nella versione La Parola &egrave; Vita\">La Parola &egrave; Vita</a></p>";
}
if (!$apoc && !$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Riveduta\" title=\"Il capitolo nella versione Riveduta, chiamata anche Luzzi\">Luzzi/Riveduta</a></p>";
if (!$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Ricciotti\" title=\"Il capitolo nella versione Ricciotti\">Ricciotti</a></p>";
if (!$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Tintori\" title=\"Il capitolo nella versione Tintori\">Tintori</a></p>";
if (!$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Martini\" title=\"Il capitolo nella versione Martini\">Martini</a></p>";
if (!$apoc && !$ceiextra)
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Diodati\" title=\"Il capitolo nella versione Diodati\">Diodati</a></p>";
if (!$apoc && !$ceiextra) {
echo "<h2>Commentari italiani</h2>";
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioHenry\" title=\"Un commentario del capitolo\">Commentario completo</a> di Matthew Henry</p>";
if ($libro >= 47) {
    echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioNT\" title=\"Un commentario del capitolo\">Commentario del Nuovo Testamento</a> di Enrico Bosio ed altri</p>";
}
if ($libro == 1 || ($libro >= 47 && $libro <= 69)) {
    echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioCalvino\" title=\"Un commentario del capitolo\">Commentario di Giovanni Calvino</a></p>";
}
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Commentario\" title=\"Un commentario del capitolo\">Commentario abbreviato</a> di Matthew Henry</p>";
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioBarnes\" title=\"Un commentario del capitolo\">Note di Albert Barnes</a></p>";
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioGinevra\" title=\"Un commentario del capitolo\">Note della Bibbia di Ginevra</a></p>";
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioGill\" title=\"Un commentario del capitolo\">Esposizione della Bibbia di Gill</a> di John Gill</p>";
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioPulpito\" title=\"Un commentario del capitolo\">Commentario del pulpito</a> di H. D. M. Spence</p>";
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioIllustratore\" title=\"Un commentario del capitolo\">Illustratore biblico</a> di Joseph S. Exell</p>";
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioMeyer\" title=\"Un commentario del capitolo\">Commentario di Frederick Brotherton Meyer</a></p>";
if ($libro == 23) {
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=CommentarioTesoro\" title=\"Un commentario del capitolo\">Tesoro di Davide</a> di Charles Spurgeon</p>";
}
echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Riferimenti%20incrociati\" title=\"Riferimenti incrociati del capitolo\">Riferimenti incrociati</a></p>";
}
echo "<h2>Diverse versioni</h2>";
if (!$ceiextra) {
if (!$apoc) {
    $bdg = $libro >= 47 ? "&versioni[]=Bibbia+della+Gioia" : "";
    $cnt = $libro >= 47 ? "&versioni[]=CommentarioNT" : "";
    $ctd = $libro == 23 ? "&versioni[]=CommentarioTesoro" : "";
    $cei = ($libro != 46 || $capitolo != 4) ? "&versioni[]=C.E.I." : "";
    echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=Nuova+Riveduta".$cei."&versioni[]=Nuova+Diodati&versioni[]=Riveduta+2020".$bdg."&versioni[]=Riveduta&versioni[]=Ricciotti&versioni[]=Tintori&versioni[]=Martini&versioni[]=Diodati&versioni[]=CommentarioHenry".$cnt."&versioni[]=Commentario&versioni[]=CommentarioBarnes&versioni[]=CommentarioGill&versioni[]=CommentarioPulpito&versioni[]=CommentarioIllustratore&versioni[]=CommentariMeyer".$ctd."&versioni[]=Riferimenti+incrociati\" title=\"Il capitolo in tutte le versioni e tutti i commentari disponibili\">Tutte le versione e tutti i commentari italiani</a></p>";
}
else {
    echo "<p><a href=\"/testo.php?riferimento=".urlencode($libronome).$capitolo."&versioni[]=C.E.I.&versioni[]=Ricciotti&versioni[]=Tintori&versioni[]=Martini\" title=\"Il capitolo in tutte le versioni disponibili\">Tutte le versione italiane</a></p>";
}
}
if (!$ceiextra) {
  if (!$apoc) {
      $commentario = "CommentarioHenry";
      //$commentario = $libro >= 47 ? "CommentarioNT" : "Commentario";
      $cei = ($libro != 46 || $capitolo != 4) ? "C.E.I." : "Ricciotti";
      $cei2 = ($libro != 46 || $capitolo != 4) ? "C.E.I. (1974)" : "Ricciotti";
      $urlTC = converti_linkTestoContinuto($libronome.$capitolo, ["Nuova Riveduta", $cei, "Nuova Diodati", $commentario]);
      echo "<p><a href=\"$urlTC\" title=\"La Bibbia e i commentari in un testo continuo\">Testo continuo</a> (Nuova Riveduta, $cei2, Nuova Diodati, e Commentario di Matthew Henry)</p>";
  }
  else {
      $urlTC = converti_linkTestoContinuto($libronome.$capitolo, ["C.E.I.", "Ricciotti", "Tintori", "Martini"]);
      echo "<p><a href=\"$urlTC\" title=\"La Bibbia in un testo continuo\">Testo continuo</a> (C.E.I. (1974), Ricciotti, Tintori, e Martini)</p>";
  }
}
else {
    $urlTC = converti_linkTestoContinuto($libronome.$capitolo, ["C.E.I."]);
    echo "<p><a href=\"$urlTC\" title=\"La Bibbia in un testo continuo\">Testo continuo</a> (C.E.I. (1974))</p>";
}
if ($libro >= 47) {
    echo "<p><a href=\"/greco/index.php?rif1=$libro&rif2=$capitolo\" title=\"Il testo del Nuovo Testamento greco, con le letture varianti dei manoscritti\">Testo greco</a> con diverse edizioni, interlineare, varianti, definizioni</p>";
    echo "<p><a href=\"/interlineare/index.php?libro=".($libro-46)."&capitolo=$capitolo\" title=\"Interlineare greco-italiano Nuovo Testamento, con tre versioni italiane\">Interlineare</a> dal greco a tre versioni italiane</p>";
}
echo "<h2>Altri strumenti</h2>";
if ($libro >= 47 && $libro <= 50) {
    echo "<p><a href=\"/sinossi.php?riferimento=$libronome$capitolo\" title=\"Un confronto dei quattro Vangeli\">Sinossi dei Vangeli</a></p>";
}

$rif = converti_rif($libronome.$capitolo);
if ($rif!="") {
  $l1=ord($rif[0]);
  $c1=ord($rif[1]);
  $v1=ord($rif[2]);
  $l2=ord($rif[3]);
  $c2=ord($rif[4]);
  $v2=ord($rif[5]);
  $cond = "WHERE (Libro1<$l2 OR (Libro1=$l2 AND (Capitolo1<$c2 OR (Capitolo1=$c2 AND Versetto1<=$v2)))) AND (Libro2>$l1 OR (Libro2=$l1 AND (Capitolo2>$c1 OR (Capitolo2=$c1 AND Versetto2>=$v1))))";
  $sql = "SELECT * FROM Brani $cond";
  //echo "<p>$sql</p>";
  if ($ris = mysqli_query($conn, "$sql")) {
    if (mysqli_num_rows($ris)>0) {
      echo "<p><a href=\"/brani/brani.php?r=".urlencode($libronome)."$capitolo\" title=\"Risposte a domande comuni sui brani della Bibbia che sono difficili da capire\">Brani difficili</a></p>";
    }
  }

  $sql = "SELECT * FROM Studi $cond";
  if ($ris = mysqli_query($conn, "$sql")) {
    if (mysqli_num_rows($ris)>0) {
      echo "<p><a href=\"/studi/studi.php?brano=".urlencode($libronome)."$capitolo\" title=\"Un indice di studi biblici su Internet\">Studi biblici</a> da altri siti Internet</p>";
    }
  }
}

$capitoloRiassunto = ($ceiextra?3:$capitolo);
echo "<p><a href=\"/riassunti/riassunto.php?libro=$libro&capitolo=$capitoloRiassunto\" title=\"Riassunti di tutti i capitoli della Bibbia, fatti da IA\">Riassunto</a> (generato da un'intelligenza artificiale)</p>";
echo "<p><a href=\"/bibbia/libro.php?n=$libro\" title=\"Ritornare ai capitoli del libro di $libroHTML\">Capitoli di $libroHTML</a></p>";
require("../piede.php");
?>
