<?
$descriz = "";
$key = "";
$non_includere_quot = 1;
include("../conn.php");
include("../vistesto.php");
$sezione = "Studi";
$sezioneurl = "/studi/";
$titolo = "Inserimento di multipli studi";
require("../capo.php");
?>

<?
$cid=0;
if (isset($_REQUEST["cid"]))
  $cid=(int)$_REQUEST["cid"];
if ($cid==0) {	
  if (isset($_COOKIE["cid"]))
    $cid=(int)$_COOKIE["cid"];
}

$cnome="";
if (isset($_REQUEST["cnome"]))
  $cnome=$_REQUEST["cnome"];
  $cnome = str_replace("<", "", $cnome); // affinché tag HTML non possono essere inseriti nella pagina
  $cnome = str_replace(">", "", $cnome);
  $cnome = str_replace("\"", "", $cnome);
if ($cnome==0) {	
  if (isset($_COOKIE["cnome"]))
    $cnome=(int)$_COOKIE["cnome"];
}

if ($cid==0 || $cnome="") {
?>
<h1>Errore</h1>
<p>Non sei entrato come utente registrato.</p>
<p><a href="/studi/">Ritornare alla pagina principale degli studi</a></p>
<?
}
else {
  echo "<h1>Nuovi studi</h1>";
  $n = 10;
  echo "<p>In questa pagina &egrave; possibile inserire nuovi studi al database. Gli studi dovrebbero trattarsi di un brano particolare della Bibbia, e non per esempio di un tema biblico.</p>";
  echo "<p>Per aggiungere un singolo studo, &egrave; meglio usare <a href=\"regostud.php\">questa pagina</a>, perch&eacute; &egrave; anche possibile inserire direttamente il testo dello studio e non solo un collegamento.";

  echo "<p>&Egrave; necessario riempire tutte e tre le caselle per ogni studio.</p>";
  echo "<form name=\"InserzioneForm\" action=\"/studi/\" method=\"post\">";
  echo "<table>";
  echo "<tr><th></th><th>Titolo:</th><th>Brano:</th><th>Indirizzo dello studio:</th></tr>\n";
  for ($i=1; $i<=$n; $i++) {
    echo "<tr><td><strong>$i</strong></td>";
    echo "<td><input class=\"text\" type=\"text\" name=\"titolo$i\" maxlength=\"255\" size=\"30\" /></td>";
    echo "<td><input class=\"text\" type=\"text\" name=\"brano$i\" maxlength=\"255\" size=\"30\" /></td>";
    echo "<td><input class=\"text\" type=\"text\" name=\"indirizzo$i\" maxlength=\"255\" size=\"40\" /></td></tr>\n";
  }
  echo "</table>";
  echo "<p><center><input class=\"submit\" type=\"submit\" name=\"InsStudi\" value=\"Inserire studi\" />";
  echo "&nbsp;&nbsp;&nbsp;<input class=\"reset\" type=\"reset\" name=\"Reset\" value=\"Annulla\" /></center></p>";
  echo "</form>";
}
require("../piede.php");
?>
