<?
$descriz = "";
$key = "";
include("../conn.php");
$titolo = "Password spedita";
$sezione = "Studi";
$sezioneurl = "/studi/";
require("../capo.php");
$nome = "";
if (isset($_REQUEST["nome"]))
  $nome = $_REQUEST["nome"];
$nome = str_replace("<", "", $nome); // affinché tag HTML non possono essere inseriti nella pagina
$nome = str_replace(">", "", $nome);

$sql = "SELECT email, password FROM Autori WHERE nome=\"$nome\"";
$ris = mysqli_query($conn, "$sql");
if (mysqli_num_rows($ris)==1) {
  $row = mysqli_fetch_array($ris);
  $testo = "La password di $nome a http://www.laparola.net/studi/ è ".$row["password"].".";
  mail($row["email"], "Richiesta password", $testo, "From: ", "-finfo@laparola.net");
?>
<h1>Password spedita</h1>
<p>La tua password &egrave; stata spedita al tuo indirizzo e-mail. Quando l'avrai ricevuta, potrai usarla per entrare di nuovo come utente registrato.</p>
<p>Se non ricevi un messaggio entro un giorno, probabilmente l'indirizzo e-mail che hai usato per registrarti non &egrave; valido. In quel caso, scrivi a info@laparola.net per ricevere la password.</p>
<p><a href="/studi/">Ritornare alla pagina degli studi</a></p>
<?
}
else {
  echo "<h1>Password non spedita</h1>";
  echo "<p>Il nome $nome non &egrave; di un utente iscritto. <a href=\"pw.php\">Prova di nuovo</a>, o <a href=\"/studi/\">ritorna alla pagina degli studi</a>.</p>";
}
require("../piede.php");
?>
