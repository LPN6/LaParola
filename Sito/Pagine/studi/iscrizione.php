<?
$descriz = "Iscrizione";
$key = "iscrizione";
$sezione = "Studi";
$sezioneurl = "/studi/";
$titolo = "Iscrizione";
require("../capo.php");
?>

<h1>Iscrizione</h1>
<p><strong>Nota:</strong> Per leggere gli studi, non &egrave; necessario iscriversi. L'iscrizione &egrave; necessaria solo per inserire un proprio studio o per dare un voto ad uno studio. L'iscrizione &egrave; gratuita, e non riceverai nessuna posta come conseguenza.</p>
<p>Campi segnati con * sono obbligatori.</p>
<form name="IscrizioneForm" action="/studi/" method="post" onsubmit="return(validare())">
<table>
<tr><td>*Nome utente:</td><td><input class="text" type="text" name="nome" maxlength="255" size="30" /></td></tr>
<tr><td>*Password:</td><td><input class="text" type="password" name="password" maxlength="16" size="30" /></td></tr>
<tr><td>*Ripetere password:</td><td><input class="text" type="password" name="password2" maxlength="16" size="30" /></td></tr>
<tr><td>*E-mail:</td><td><input class="text" type="text" name="email" maxlength="255" size="30" /></td></tr>
<tr><td>*Pubblicare l'indirizzo e-mail su questo sito?</td><td><select name="emailpubblico" size="2" style="background:#68ffff;">
  <option selected value="S">S&igrave;
  <option value="N">No
  </select></td></tr>
<tr><td>Il tuo sito internet:</td><td><input class="text" type="text" name="sito" maxlength="255" size="30" /></td></tr>
<tr><td>Note personali:</td><td><textarea name="descrizione" rows="3" cols="30" /></textarea></td></tr>
<tr><td><input class="submit" type="submit" name="Iscrizione" value="Iscriversi" /></td>
<td><input class="reset" type="reset" name="Reset" value="Annulla" /></td></tr>
</table>
</form>

<p>Leggi la <a href="/privacy.php">privacy policy</a> del sito.</p>

<script language="JavaScript">
<!--
function validare() {
var errors = false;
var errString = "Errore, ci sono dati mancanti:";
if (document.forms.IscrizioneForm.nome.value == "") {
   document.forms.IscrizioneForm.nome.focus();
   errors = true;
   errString += "\n   Nome";
}
if (document.forms.IscrizioneForm.password.value == "") {
   document.forms.IscrizioneForm.password.focus();
   errors = true;
   errString += "\n   Password";
}
if (document.forms.IscrizioneForm.password2.value == "") {
   document.forms.IscrizioneForm.password2.focus();
   errors = true;
   errString += "\n   Password ripetuta";
}
if (document.forms.IscrizioneForm.email.value == "") {
   document.forms.IscrizioneForm.email.focus();
   errors = true;
   errString += "\n   La tua e-mail";
}

if (errors) alert(errString);

if (document.forms.IscrizioneForm.password2.value != document.forms.IscrizioneForm.password.value) {
  document.forms.IscrizioneForm.password.focus();
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