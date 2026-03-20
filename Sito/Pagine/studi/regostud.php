<?
$descriz = "";
$key = "";
$non_includere_quot = 1;
include("../conn.php");
include("../vistesto.php");
$titolo = "Studio";
$sezione = "Studi";
$sezioneurl = "/studi/";
require("../capo.php");
?>

<?
$s=0;
if (isset($_REQUEST["s"]))
  $s=(int)$_REQUEST["s"];

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
if ($s!=0) {
  $sql = "SELECT * FROM Studi WHERE id_s=".abs($s);
  if ($ris = mysqli_query($conn, "$sql")) {
    if (mysqli_num_rows($ris)>0) {
      $row=mysqli_fetch_array($ris);
      if ($row["id_a"]!=$cid) {
        echo "<h1>Errore</h1>";
        echo "<p>Non sei l'autore di questo studio.</p>";
        echo "<p><a href=\"/studi/\">Ritorna alla pagina principale degli studi</a> o crea un nuovo studio.</p>";
        $s = 0;
      }
    }
    else {
      echo "<h1>Errore</h1>";
      echo "<p>Lo studio richiesto non &egrave; stato trovato.</p>";
      echo "<p><a href=\"/studi/\">Ritorna alla pagina principale degli studi</a> o crea un nuovo studio.</p>";
      $s = 0;
    }
  }
  else {
    errore2("collegamento al database degli studi");
    $s = 0;
  }
}
if ($s<0) {
  echo "<h1>Cancellazione studio</h1>";
  echo "<p>Sei sicuro di voler cancellare questo studio?</p>";
  echo "<p><a href=\"/studi/?s=$s&InsStudio=1\">S&igrave;, voglio cancellare lo studio</a></p>";
  echo "<p><a href=\"modstud.php\">No, non voglio cancellare lo studio</a></p>";
}
else {
  if ($s>0) {
    echo "<h1>Modifica studio</h1>";
    $tit = $row["titolo"];
    $br=converti_rif3($row["libro1"],$row["capitolo1"],$row["versetto1"],$row["libro2"],$row["capitolo2"],$row["versetto2"]);
    $t = $row["testo"];
    $ind = $row["indirizzo"];
    $nrighe = floor(strlen($row["testo"])/50)+5;
  }
  else {
    echo "<h1>Nuovo studio</h1>";
    $tit = "";
    $br = "";
    $t = "";
    $ind = "";
    $nrighe = 40;
    echo "<p>In questa pagina &egrave; possibile inserire un nuovo studio al database. Lo studio dovrebbe trattarsi di un brano particolare della Bibbia, e non per esempio di un tema biblico.</p>";
    echo "<p>Per aggiungere multipli studi, &egrave; pi&ugrave; veloce usare <a href=\"regonst.php\">questa pagina</a>.";
  }
  echo "<p>&Egrave; necessario riempire le prime due caselle, o una delle altre due.</p>";
  echo "<form name=\"InserzioneForm\" action=\"/studi/\" method=\"post\" onsubmit=\"return(validare())\">";
  echo "<input type=\"hidden\" name=\"s\" value=".$s." />";
  echo "<table>";
  echo "<tr><td>Titolo:</td><td><input class=\"text\" type=\"text\" name=\"titolo\" maxlength=\"255\" size=\"30\" value=\"".$tit."\" /></td></tr>";
  echo "<tr><td>Brano:</td><td><input class=\"text\" type=\"text\" name=\"brano\" maxlength=\"255\" size=\"30\" value=\"".$br."\" /></td></tr>";
  if ($s>0)
    echo "<tr><td>Data:</td><td>".$row["data"]."</td></tr>";
  echo "</table>";
  echo "<p>Se lo studio esiste gi&agrave; su Internet e vuoi inserire un link ad esso, riempi la prima casella.<br />";
  echo "Se vuoi inserire il testo di uno studio su questo sito, riempi la seconda casella.</p>";
  echo "<p>Indirizzo dello studio: <input class=\"text\" type=\"text\" name=\"indirizzo\" maxlength=\"255\" size=\"60\" value=\"".$ind."\" /></p>";
  echo "<p>Testo dello studio:</p><p><textarea name=\"testo\" rows=\"$nrighe\" cols=\"60\">".$t."</textarea></p>";
  echo "<p><input class=\"submit\" type=\"submit\" name=\"InsStudio\" value=\"Inserire studio\" />";
  echo "&nbsp;&nbsp;&nbsp;<input class=\"reset\" type=\"reset\" name=\"Reset\" value=\"Annulla\" /></p>";
  echo "</form>";
}
}
?>
<script language="JavaScript">
<!--
//function setFormPointer() {
//         form1 = document.forms.InserzioneForm;
//}
function validare() {
var errors = false;
var errString = "Errore, ci sono dati mancanti:";
if (document.getElementById("titolo").value == "") {
   document.getElementById("testo").focus();
   errors = true;
   errString += "\n   Titolo";
}
if (document.getElementById("brano").value == "") {
   document.getElementById("testo").focus();
   errors = true;
   errString += "\n   Brano";
}
if (document.getElementById("testo").value == "" && document.getElementById("indirizzo").value == "") {
   document.getElementById("testo").focus();
   errors = true;
   errString += "\n   Testo o indirizzo";
}

if (errors) alert(errString);

return !errors;
}
-->
</script>

<?
require("../piede.php");
?>
