<?
$descriz = "";
$key = "";
include("../conn.php");
$titolo = "Dati personali";
$sezione = "Studi";
$sezioneurl = "/studi/";
require("../capo.php");

$cid=0;
if (isset($_COOKIE["cid"]))
  $cid=(int)$_COOKIE["cid"];
$cnome="";
if (isset($_REQUEST["cnome"]))
  $cnome=$_REQUEST["cnome"];
  $cnome = str_replace("<", "", $cnome); // affinché tag HTML non possono essere inseriti nella pagina
  $cnome = str_replace(">", "", $cnome);
  $cnome = str_replace("\"", "", $cnome);

if ($cid==0 || $cnome="") {
?>
<h1>Errore</h1>
<p>Non sei entrato come utente registrato.</p>
<p><a href="/studi/">Ritornare alla pagina principale degli studi</a></p>
<?
}
else {
$sql = "SELECT * FROM Autori WHERE id_a=$cid";
if ($ris = mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris)>=1) {
    $row=mysqli_fetch_array($ris);
?>
<h1>Dati personali</h1>
<p>Campi segnati con * sono obbligatori.</p>
<form name="ModRegoForm" action="/studi/" method="post" onsubmit="return(validare())">
<table>
<tr><td>*Nome:</td><td><input class="text" type="text" name="nome" maxlength="255" size="30" value="<?echo $row["nome"]?>" /></td></tr>
<tr><td>*Password:</td><td><input class="text" type="password" name="password" maxlength="16" size="30" value="<?echo $row["password"]?>" /></td></tr>
<tr><td>*Ripetere password:</td><td><input class="text" type="password" name="password2" maxlength="16" size="30" value="<?echo $row["password"]?>" /></td></tr>
<tr><td>*E-mail:</td><td><input class="text" type="text" name="email" maxlength="255" size="30" value="<?echo $row["email"]?>" /></td></tr>
<tr><td>*Pubblicare l'indirizzo e-mail su questo sito?</td><td><select name="emailpubblico" size="2" style="background:#68ffff">
<?
if ($row["emailpubblico"]=="S") {
  echo "<option selected value=\"S\">S&igrave;";
  echo "<option value=\"N\">No";
}
else {
  echo "<option value=\"S\">S&igrave;";
  echo "<option selected value=\"N\">No";
}
?>
  </select></td></tr>
<tr><td>Sito internet:</td><td><input class="text" type="text" name="sito" maxlength="255" size="30" value="<?echo $row["sito"]?>" /></td></tr>
<tr><td>Note personali:</td><td><textarea name="descrizione" rows="3" cols="30"><?echo $row["descrizione"]?></textarea></td></tr>
<tr><td><input class="submit" type="submit" name="ModRego" value="Modificare dati" /></td>
<td><input class="reset" type="reset" name="Reset" value="Annulla" /></td></tr>
</table>
</form>
<?
  }
  else
    errore2("Non ho trovato i tuoi dati.");
}
else
  errore2("interrogazione database per gli autori");
}
?>

<script language="JavaScript">
<!--
//function setFormPointer() {
  form1 = document.forms.ModRegoForm;
//}
function validare() {
var errors = false;
var errString = "Errore, ci sono dati mancanti:";
if (form1.nome.value == "") {
   form1.nome.focus();
   errors = true;
   errString += "\n   Nome";
}
if (form1.password.value == "") {
   form1.password.focus();
   errors = true;
   errString += "\n   Password";
}
if (form1.password2.value == "") {
   form1.password2.focus();
   errors = true;
   errString += "\n   Password ripetuta";
}
if (form1.email.value == "") {
   form1.email.focus();
   errors = true;
   errString += "\n   La tua e-mail";
}

if (errors) alert(errString);

if (form1.password2.value != form1.password.value) {
  form1.password.focus();
  errors = true;
  alert ("Le due password digitate non sono identiche.");
}

return !errors;
}
-->
</script>

<?
require("../piede.php");
?>
