<?
$non_includere_quot = 1;
include("../conn.php");
include("../vistesto.php");
$sezione = "Brani";
$sezioneurl = "/brani/";

function mostraNuovaRicerca()
{
	echo "<p>Scegli un brano dall'<a href=\"/brani/\">elenco di brani discussi</a>, oppure fai un'altra ricerca.</p>";
	echo "<form action=\"brani.php\" method=\"post\">";
	echo "<p><label for=\"brano\">Brano:</label>&nbsp;<input class=\"text\" type=\"text\" name=\"r\" id=\"brano\" maxlength=\"255\" size=\"30\" /></p>";
	echo "<p><input class=\"submit\" type=\"submit\" name=\"Submit\" value=\"Ricercare\" />\n";
	echo "<input class=\"reset\" type=\"reset\" name=\"Reset\" value=\"Annulla\" />";
	echo "</p>";
	echo "</form>";
}

$b = 0;
if (isset($_REQUEST["b"]))
  $b = (int)$_REQUEST["b"];
$r = "";
if (isset($_REQUEST["r"])) {
  $r = $_REQUEST["r"];
  $r = str_replace("<", "", $r); // affinché tag HTML non possono essere inseriti nella pagina
  $r = str_replace(">", "", $r);
  $r = str_replace("\"", "", $r);
}
 
if ($b!=0)
	$sql="SELECT * FROM Brani WHERE id_b=\"$b\"";
else {
  $rif = converti_rif($r);
  if ($rif!="") {
    $l1=ord($rif[0]);
    $c1=ord($rif[1]);
    $v1=ord($rif[2]);
    $l2=ord($rif[3]);
    $c2=ord($rif[4]);
    $v2=ord($rif[5]);
	// vediamo se c'è un brano con esattamente questo riferimento (utile quando c'è un link al brano da un altro brano),
	// altrimenti scegliamo tutti i brani che contengono il riferimento
	$sql="SELECT * FROM Brani WHERE (Libro1=$l1 AND Capitolo1=$c1 AND Versetto1=$v1 AND Libro2=$l2 AND Capitolo2=$c2 AND Versetto2=$v2)";
	if ($ris2 = mysqli_query($conn, "$sql")) {
  		if (mysqli_num_rows($ris2)<>1) {
    		$sql="SELECT * FROM Brani WHERE (Libro1<$l2 OR (Libro1=$l2 AND (Capitolo1<$c2 OR (Capitolo1=$c2 AND Versetto1<=$v2)))) AND (Libro2>$l1 OR (Libro2=$l1 AND (Capitolo2>$c1 OR (Capitolo2=$c1 AND Versetto2>=$v1))))";
		}
	}
	else {
  		$sql="SELECT * FROM Brani WHERE (Libro1<$l2 OR (Libro1=$l2 AND (Capitolo1<$c2 OR (Capitolo1=$c2 AND Versetto1<=$v2)))) AND (Libro2>$l1 OR (Libro2=$l1 AND (Capitolo2>$c1 OR (Capitolo2=$c1 AND Versetto2>=$v1))))";
	}
  }
  else
  	$sql="SELECT * FROM Brani";
}
//echo $sql;
if ($ris = mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris)==0) {
    $titolo = "Nessuno trovato";
    $descriz = "Brano non trovato";
    $key = "difficolt&agrave;, brani, brani difficili, spiegazione, interpretazione, commentario";
    require("../capo.php");
    echo "<h1>Brano non trovato</h1>";
    echo "<p>Purtroppo, il brano richiesto non &egrave; stato trovato.</p>";
	mostraNuovaRicerca();
  }
  if (mysqli_num_rows($ris)==1) {
    $row=mysqli_fetch_array($ris);
    $titolo = $row["Domanda"];
    $descriz = "Una risposta alla domanda: ".$titolo;
    $key = "difficolt&agrave;, brani, brani difficili, spiegazione, interpretazione, commentario,".$titolo;
    require("../capo.php");
    echo "<h1>".$titolo."</h1>";
	if ($row["Libro1"]>0)
      echo "<h2>".converti_rif3($row["Libro1"],$row["Capitolo1"],$row["Versetto1"],$row["Libro2"],$row["Capitolo2"],$row["Versetto2"])."</h2>";
    $risposta = $row["Risposta"];
    $risposta = str_replace("<table", "<table style=\"table-layout:fixed;width:100%;\"", $risposta);
    $risposta = str_replace("<td>", "<td><div style=\"overflow-wrap:break-word\">", $risposta);
    $risposta = str_replace("</td>", "</div></td>", $risposta); 
    echo $risposta;
//      echo "<p>".str_replace("&amp;", "&", nl2br($row["testo"]))."</p>";
  }
  if (mysqli_num_rows($ris)>1) {
    $titolo = "Brani trovati";
    $descriz = "Brani difficili";
    $key = "difficolt&agrave;, brani, brani difficili, spiegazione, interpretazione, commentario,";
    require("../capo.php");
    echo "<h1>".$titolo."</h1>";
	if ($rif=="") {
		echo "<p>Il riferimento $r non &egrave; stato riconosciuto.</p>";
		mostraNuovaRicerca();		
	}
	else {
    	echo "<p>I seguenti brani sono stati trovati. Scegli quello desiderato.</p>";
		echo "<table>";
    	while ($row=mysqli_fetch_array ($ris)) {
      		echo "<tr><td>".converti_rif3($row["Libro1"],$row["Capitolo1"],$row["Versetto1"],$row["Libro2"],$row["Capitolo2"],$row["Versetto2"])."</td>";
			echo "<td><a href=\"brani.php?b=".$row["id_b"]."\">".$row["Domanda"]."</a></td></tr>\n";
    	}
    	echo "</table>";
  	}
  }
}
else {
  $titolo = "Errore";
  require("../capo.php");
    errore2("interrogazione database per brani");
}

require("../piede.php");
?>
