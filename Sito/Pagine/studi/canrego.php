<?
$descriz = "";
$key = "";
$sezione = "Studi";
$sezioneurl = "/studi/";
$titolo = "Cancellazione iscrizione";
require("../capo.php");

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

if ($cid==0 || $cnome="") {
?>
<h1>Errore</h1>
<p>Non sei entrato come utente registrato.</p>
<p><a href="/studi/">Ritornare alla pagina principale degli studi</a></p>
<?
}
else {
?>
<h1>Cancellazione iscrizione</h1>
<p>Sei sicuro di voler cancellare la tua iscrizione? Cos&igrave; facendo, anche tutti i tuoi studi e voti saranno rimossi dal sito.</p>
<p><a href="nonrego.php">S&igrave;, voglio cancellare la mia iscrizione</a></p>
<p><a href="/studi/">No, non voglio cancellare la mia iscrizione</a></p>
<?
}
require("../piede.php");
?>
