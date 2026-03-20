<?
if (isset($_REQUEST["cid"]))
  $cid=(int)$_REQUEST["cid"];
else {
  $cid=0;
}
if (isset($_REQUEST["cnome"]))
  $cnome=$_REQUEST["cnome"];
else
  $cnome="";
  $cnome = str_replace("<", "", $cnome); // affinché tag HTML non possono essere inseriti nella pagina
  $cnome = str_replace(">", "", $cnome);

setcookie("cid", "");
setcookie("cnome", "");

$descriz = "";
$key = "";
include("../conn.php");
$titolo = "Iscrizione cancellata";
$sezione = "Studi";
$sezioneurl = "/studi/";
require("../capo.php");
if ($cid==0 || $cnome="") {
?>
<h1>Errore</h1>
<p>Non sei entrato come utente registrato.</p>
<p><a href="/studi/">Ritornare alla pagina principale degli studi</a></p>
<?
}
else {
$sql = "DELETE FROM Voti WHERE id_a=$cid";
mysqli_query($conn, "$sql");
$sql = "DELETE FROM Studi WHERE id_a=$cid";
mysqli_query($conn, "$sql");
$sql = "DELETE FROM Autori WHERE id_a=$cid";
mysqli_query($conn, "$sql");
?>
<h1>Iscrizione cancellata</h1>
<p>Non sei pi&ugrave; iscritto.</p>
<p><a href="/studi/">Ritornare alla pagina principale degli studi</a></p>
<?
}
require("../piede.php");
?>
