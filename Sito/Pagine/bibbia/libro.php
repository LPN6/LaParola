<?
//$libro = (isset($_REQUEST["libro"])?$_REQUEST["libro"]:"Genesi");
//if ($libro=="Giosue") $libro="Giosuè";
$n = (int)(isset($_REQUEST["n"])?$_REQUEST["n"]:1);

include("../conn.php");
global $conn;
$sql = "SELECT Nome,Numero FROM Libri WHERE Numero=$n";
$libro = "";
if ($ris=mysqli_query ($conn, "$sql")) {
  while ($row=mysqli_fetch_array ($ris))
    $libro = $row["Nome"];
}
if ($n==6) $libro = "Giosuè";
$libroHTML = htmlentities($libro, 0, "ISO-8859-1"); // per Giosuè

$descriz = "I capitoli del libro di $libroHTML nella Bibbia, con il testo nelle versioni C.E.I. (1976) e Nuova Riveduta, e un elenco di altre risorse per studiarli";
$key = $libroHTML;
$titolo = $libroHTML;
$sezione = "Bibbia per capitoli";
$sezioneurl = "";
require("../capo.php");
?>
<style>
.tableCapitoli { border-collapse: collapse; width: 100%; }
.tableCapitoli, .tableCapitoli th, .tableCapitoli td { border: 1px solid black; padding: 8px; text-align: left; }
</style>
<?

echo "<h1>$libroHTML</h1>";
echo "<p>In questa pagina c'&egrave; un elenco dei capitoli di <strong>$libroHTML</strong>, con collegamenti al testo e ad un indice di risorse per studiare il capitolo.</p>";
if ($n==23)
    echo "<p>Nota che le versioni C.E.I. (1974), Ricciotti, Tinitori, e Martini usano l'enumerazione della traduzione greca dei Salmi, mentre le altre versioni e i commentari usano l'enumerazione del testo ebraico. Per questo motivo, il testo dal Salmo 10 al Salmo 147 in un'altra versione corrisponde al Salmo con il numero precedente in quelle 4 versioni.</p>";
$sql = "SELECT DISTINCT Capitolo FROM Versetti WHERE id_t=2 AND Libro=$n";
if ($ris=mysqli_query($conn, $sql)) {
echo "<table class=\"tableCapitoli\">";
  while ($row=mysqli_fetch_array($ris)) {
    $c = $row["Capitolo"];
    echo "<tr><td><a href=\"capitolo.php?libro=$n&capitolo=$c\" title=\"".$libroHTML." ".$c."\">".$libroHTML." ".$c.": Indice di risorse</a></td>";
    echo "<td><a href=\"/testo.php?riferimento=".urlencode($libro).$c."&versioni[]=C.E.I.\" title=\"".$libroHTML." ".$c."\">Testo di ".$libroHTML." ".$c." (C.E.I. 1974)</a></td>";
    if (($n==34 && $c>=13) || ($n==36 && $c>=4)) {
        echo "<td></td>"; // Dan 13-14, Gioele 4
    }
    else if ($n==17 || $n==18 || $n==20 || $n==21 || $n==27 || $n==28 || $n==32) {
        // apocrifa
    }
    else {
        echo "<td><a href=\"/testo.php?riferimento=".urlencode($libro).$c."&versioni[]=Nuova%20Riveduta\" title=\"".$libroHTML." ".$c."\">Testo di ".$libroHTML." ".$c." (Nuova Riveduta)</a></td>";
    }
    echo "</tr>\n";
  }
  if ($n==46) {
    echo "<tr><td><a href=\"capitolo.php?libro=$n&capitolo=4\" title=\"".$libroHTML." 4\">".$libroHTML." 4: Indice di risorse</a></td>";
    echo "<td><a href=\"/testo.php?riferimento=".urlencode($libro)."3,19-25&versioni[]=C.E.I.\" title=\"".$libroHTML." 4\">Testo di ".$libroHTML." 4 (C.E.I. 1974, dove &egrave; 3,19-25)</a></td>";
    echo "<td><a href=\"/testo.php?riferimento=".urlencode($libro)."4&versioni[]=Nuova%20Riveduta\" title=\"".$libroHTML." 4\">Testo di ".$libroHTML." 4 (Nuova Riveduta)</a></td>";
    echo "</tr>\n";
  }
echo "</table>";
}

echo "<p><a href=\"/bibbia/\" title=\"Ritornare all'elenco dei libri della Bibbia\">Tutti i libri della Bibbia</a></p>";

require("../piede.php");
?>
