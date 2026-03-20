<?
$descriz = "Tutta la Bibbia per libro";
$key = "";
$titolo = "La Sacra Bibbia";
$sezione = "Bibbia per capitoli";
require("../capo.php");
?>
<h1>La Sacra Bibbia</h1>
<p>Qui ci sono dei link a tutti i libri della <strong>Sacra Bibbia</strong>, da cui ci sono dei link ad ogni capitolo del libro.</p><p>
<?
include("../conn.php");
global $conn;
$sql = "SELECT Nome,Numero FROM Libri";
if ($ris=mysqli_query ($conn, "$sql")) {
  while ($row=mysqli_fetch_array ($ris)) {
    $n = $row["Numero"];
    $libro = $row["Nome"];
    if ($n==72 || $n==71 || $n==70 || $n==64 || $n==38)
      echo "<a href=\"/testo.php?versioni[]=C.E.I.&riferimento=".$libro."\" title=\"".$libro."\">".$libro."</a><br />";
    else {
//      echo "<a href=\"libro.php?libro=".$libro."&n=".$n."\" title=\"".$libro."\">".$libro."</a><br />";
      echo "<a href=\"libro.php?n=".$n."\" title=\"".$libro."\">".$libro."</a><br />";
//      $libro2 = $libro;
//      if ($libro2=="Giosuè") $libro2="Giosue";
//      echo "<a href=\"".$libro2."-".$n.".htm\" title=\"".$libro."\">".$libro."</a><br />";
    }
  }
}
echo "</p>";
require("../piede.php");
?>
