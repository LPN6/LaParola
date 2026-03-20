<?
//$libro = (isset($_REQUEST["libro"])?$_REQUEST["libro"]:"Matteo");
$n = (int)(isset($_REQUEST["n"])?$_REQUEST["n"]:47);
include("../conn.php");
global $conn;
$sql = "SELECT Nome,Numero FROM Libri WHERE Numero=$n";
$libro = "";
if ($ris=mysqli_query ($conn, "$sql")) {
  while ($row=mysqli_fetch_array ($ris))
    $libro = $row["Nome"];
}
$libroHTML = htmlentities($libro, 0, "ISO-8859-1"); // per Giosuè

$descriz = "Commentario sul ".$libroHTML;
$key = $libroHTML.",commentario";
$titolo = "Commentario su $libroHTML";
$sezione = "Commentari sulla Bibbia per capitoli";
$sezioneurl = "";
require("../capo.php");
echo "<h1>$libroHTML</h1>";
?>
<p>Qui ci sono dei link al commentario di ogni capitolo del libro di <strong><?echo $libroHTML;?></strong>.</p><p>
<?
$sql = "SELECT DISTINCT Capitolo FROM Versetti WHERE Libro=$n";
if ($ris=mysqli_query($conn, $sql)) {
    $v = "versioni[]=Commentario";
    if ($n >= 47)
        $v = $v."&versioni[]=CommentarioNT";
  while ($row=mysqli_fetch_array($ris)) {
    $c = $row["Capitolo"];
    echo "<a href=\"/testo.php?".$v."&riferimento=".urlencode($libro).$c."\" title=\"".$libroHTML." ".$c."\">".$libroHTML." ".$c."</a>";
    echo "<br />\n";
  }
}
echo "</p>";
require("../piede.php");
?>
