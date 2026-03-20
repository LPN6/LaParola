<?
$descriz = "";
$key = "";
$non_includere_quot = 1;
include("../conn.php");
include("../vistesto.php");
include("funzionis.php");
$sezione = "Studi";
$sezioneurl = "/studi/";
$titolo = "Modifica studi";
require("../capo.php");

if (isset($_COOKIE["cid"]))
  $cid=(int)$_COOKIE["cid"];
else
  $cid=0;
if (isset($_REQUEST["cnome"]))
  $cnome=$_REQUEST["cnome"];
else
  $cnome="";
  $cnome = str_replace("<", "", $cnome); // affinché tag HTML non possono essere inseriti nella pagina
  $cnome = str_replace(">", "", $cnome);

if ($cid==0 || $cnome="") {
?>
<h1>Errore</h1>
<p>Non sei entrato come utente registrato.</p>
<?
}
else {
?>
<h1>Modifica studi</h1>
<?
$sql = "SELECT * FROM Studi WHERE Studi.id_a=$cid";
if ($ris = mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris)>0) {
    echo "<table><tr><th>Titolo</th><th>Brano</th><th>Data</th><th>Voto</th><th></th><th></th></tr>";
    while ($row=mysqli_fetch_array ($ris)) {
      echo "<tr><td>".$row["titolo"]."</td>";
      echo "<td>".converti_rif3($row["libro1"],$row["capitolo1"],$row["versetto1"],$row["libro2"],$row["capitolo2"],$row["versetto2"])."</td>";
      echo "<td>".formatta_data($row["data"])."</td>";
      $v = voti($row["id_s"], $conn);
      $pos = strpos($v,"|");
      $nvoti = substr($v,0,$pos);
      if ($nvoti>0)
        $v = substr($v,$pos+1);
      else
        $v = "s.v.";
      echo "<td>".$v."</td>";
      echo "<td><a href=\"regostud.php?s=".$row["id_s"]."\">Modifica</a></td>";
      echo "<td><a href=\"regostud.php?s=".-$row["id_s"]."\">Cancella</a></td></tr>";
    }
    echo "</table>";
  }
  else
    echo "Nessun tuo studio &egrave; stato trovato.";
}
else
  errore2("interrogazione database per studi");
}
echo "<p><a href=\"/studi/\">Ritornare alla pagina principale degli studi</a></p>";
require("../piede.php");
?>
