<?
$descriz = "Tutta la Bibbia per libro";
$key = "";
$titolo = "La Sacra Bibbia";
$sezione = "Bibbia per capitoli";
require("../capo.php");
?>
<h1>La Sacra Bibbia</h1>
<p>In questa pagina ci sono collegamenti a tutti i libri della <strong>Sacra Bibbia</strong>.
La pagina per ogni libro contiene collegamenti ad ogni capitolo del libro. La pagina per ogni capitolo contiene un indice di risorse per leggere e studiare quel capitolo.</p><p>
<?
include("../conn.php");
global $conn;
$sql = "SELECT Nome,Numero FROM Libri";
if ($ris=mysqli_query ($conn, "$sql")) {
  while ($row=mysqli_fetch_array ($ris)) {
    $n = $row["Numero"];
    $libro = $row["Nome"];
    if ($n==6) $libro = "Giosuè";
    $libroHTML = htmlentities($libro, 0, "ISO-8859-1");
    if ($n==72 || $n==71 || $n==70 || $n==64 || $n==38)
      echo "<a href=\"/testo.php?versioni[]=C.E.I.&riferimento=".$libro."\" title=\"".$libro."\">".$libro."</a><br />";
    else {
      echo "<a href=\"libro.php?n=".$n."\" title=\"".$libroHTML."\">".$libroHTML."</a><br />";
    }
  }
}
echo "</p>";
require("../piede.php");
?>
