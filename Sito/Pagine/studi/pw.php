<?
$descriz = "";
$key = "";
$sezione = "Studi";
$sezioneurl = "/studi/";
$titolo = "Password dimenticata";
require("../capo.php");
?>
<h1>Richiesta password</h1>
<p>Se hai dimenticato la tua password, digita qui il nome che hai usato per iscriverti e fa' clic sul pulsante. Un messaggio sar&agrave; spedito all'indirizzo e-mail con cui ti sei iscritto.</p>
<form action="pwspedita.php" method="post">
<table>
<tr><td>Nome:</td><td><input class="text" type="text" name="nome" maxlength="255" size="30" /></td></tr>
<tr><td><input class="submit" type="submit" name="Login" value="Mandami la password" /></td>
<td><input class="reset" type="reset" name="Reset" value="Annulla" /></td></tr>
</table>
</form>
<?
require("../piede.php");
?>