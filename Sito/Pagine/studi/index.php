<?
$non_includere_quot = 1;
include("../conn.php");
include("../vistesto.php");

$cid=0;
if (isset($_REQUEST["cid"]))
  $cid = (int)$_REQUEST["cid"];
if ($cid==0) {	
  if (isset($_COOKIE["cid"]))
    $cid=(int)$_COOKIE["cid"];
}

$StudiNome="";
if (isset($_COOKIE["StudiNome"]))
  $StudiNome = $_COOKIE["StudiNome"];

$nomegiausato = 0;
$Login="";

$nome = "";
if (isset($_REQUEST["nome"])) {
  $nome = $_REQUEST["nome"];
  $nome = str_replace("<", "", $nome); // affinché tag HTML non possono essere inseriti nella pagina
  $nome = str_replace(">", "", $nome);
  $nome = str_replace("\"", "", $nome);
}
$nome = mysqli_real_escape_string($conn, $nome);
$password = "";
if (isset($_REQUEST["password"])) {
  $password = $_REQUEST["password"];
  $password = str_replace("<", "", $password); // affinché tag HTML non possono essere inseriti nella pagina
  $password = str_replace(">", "", $password);
  $password = str_replace("\"", "", $password);
}
$email = "";
if (isset($_REQUEST["email"]))
  $email = $_REQUEST["email"];
$email = str_replace("<", "", $email);
$email = str_replace(">", "", $email);
$email = str_replace("\"", "", $email);
$emailpubblico = "";
if (isset($_REQUEST["emailpubblico"]))
  $emailpubblico = $_REQUEST["emailpubblico"];
$emailpubblico = str_replace("<", "", $emailpubblico);
$emailpubblico = str_replace(">", "", $emailpubblico);
$emailpubblico = str_replace("\"", "", $emailpubblico);
$sito = "";
if (isset($_REQUEST["sito"]))
  $sito = $_REQUEST["sito"];
$sito = str_replace("<", "", $sito);
$sito = str_replace(">", "", $sito);
$sito = str_replace("\"", "", $sito);
$descrizione = "";
if (isset($_REQUEST["descrizione"]))
  $descrizione = $_REQUEST["descrizione"];
$descrizione = str_replace("<", "", $descrizione);
$descrizione = str_replace(">", "", $descrizione);
$descrizione = str_replace("\"", "", $descrizione);

$iscrizionefatta = 0;
$Iscrizione="";
if (isset($_REQUEST["Iscrizione"]))
  $Iscrizione = $_REQUEST["Iscrizione"];
$Iscrizione = str_replace("<", "", $Iscrizione);
$Iscrizione = str_replace(">", "", $Iscrizione);
$Iscrizione = str_replace("\"", "", $Iscrizione);
if ($Iscrizione!="" && !empty($_POST)) {
  $iscrizionefatta = -1;
  $sql = "SELECT id_a FROM Autori WHERE nome=\"$nome\"";
  if ($ris = mysqli_query($conn, "$sql")) {
    if (mysqli_num_rows($ris)>0)
      $nomegiausato = 1;
    else {
      $sql = "INSERT INTO Autori(nome,password,email,emailpubblico,sito,descrizione) VALUES (\"$nome\",\"$password\",\"$email\",\"$emailpubblico\",\"$sito\",\"".htmlentities($descrizione, ENT_QUOTES)."\")";
      if (mysqli_query($conn, "$sql")) {
        $Login=1;
        $iscrizionefatta = 1;
      }
    }
  }
}

$modregofatto = 0;
$ModRego="";
if (isset($_REQUEST["ModRego"]))
  $ModRego = $_REQUEST["ModRego"];
$ModRego = str_replace("<", "", $ModRego);
$ModRego = str_replace(">", "", $ModRego);
$ModRego = str_replace("\"", "", $ModRego);
if ($ModRego!="" && !empty($_POST)) {
  $modregofatto = -1;
  $sql = "SELECT id_a FROM Autori WHERE nome=\"$nome\"";
  if ($ris = mysqli_query($conn, "$sql")) {
    if (mysqli_num_rows($ris)!=1) {
      $modregofatto = -1;
    }
    else {
      $sql = "UPDATE Autori SET password=\"$password\",email=\"$email\",emailpubblico=\"$emailpubblico\",sito=\"$sito\",descrizione=\"".htmlentities($descrizione, ENT_QUOTES)."\" WHERE nome=\"$nome\"";
      if (mysqli_query($conn, "$sql")) {
        $Login = 1;
        $modregofatto = 1;
      }
    }
  }
}

$loginfatto = 0;
if (isset($_REQUEST["Login"]))
  $Login = $_REQUEST["Login"];
$Login = str_replace("<", "", $Login);
$Login = str_replace(">", "", $Login);
$Login = str_replace("\"", "", $Login);
if ($Login!="" && !empty($_POST)) {
  $loginfatto = -1;
  $sql = "SELECT id_a,nome FROM Autori WHERE nome=\"$nome\" AND password=\"$password\"";
  $cid = -1;
  if ($ris = mysqli_query($conn, "$sql")) {
    $loginfatto = 1;
    if (mysqli_num_rows($ris)==1) {
      $row = mysqli_fetch_array($ris);
      $cid = $row["id_a"];
      SetCookie("cid", $cid);
      SetCookie("cnome", $row["nome"]);
      SetCookie("StudiNome", $row["nome"], time()+3600000);
    }
  }
}
?>

<?
$descriz = "Studi biblici in italiano su Internet - un indice";
$key = "studi biblici,studi,biblici,studio biblico,studio,biblico";
$titolo = "Studi";
$sezione = "Strumenti";
require("../capo.php");

if ($cid==-1) {
  $cid = 0;
  echo "<h1>Errore</h1>";
  echo "<p>Il nome e/o la password digitati sono sbagliati. Riprova, o fa' clic sul link per ricevere di nuovo la password.</p>";
}
if ($nomegiausato==1) {
  echo "<h1>Errore</h1>";
  echo "<p>Purtroppo, il nome <strong>$nome</strong> &egrave; gi&agrave; usato da un altro utente. Scegli un altro nome per iscriverti.</p>";
}
if ($iscrizionefatta==-1) {
  echo "<h1>Errore</h1>";
  echo "<p>C'&egrave; stato un errore nel collegamento al database: non &egrave; stato possibile completare l'iscrizione. Riprova pi&ugrave; tardi.</p>";
}
if ($loginfatto==-1) {
  echo "<h1>Errore</h1>";
  echo "<p>C'&egrave; stato un errore nel collegamento al database: non &egrave; stato possibile controllare la password. Riprova pi&ugrave; tardi.</p>";
}
if ($modregofatto==-1) {
  echo "<h1>Errore</h1>";
  echo "<p>C'&egrave; stato un errore nel collegamento al database: i tuoi dati personali <strong>non</strong> sono stati modificati. Riprova pi&ugrave; tardi.</p>";
}
?>
<h1>Studi biblici</h1>
<p class="primalettera">In questa sezione del sito &egrave; possibile leggere degli studi, prediche e reflessioni sulla Bibbia, scritti da diversi autori. &Egrave; possibile cercare degli studi in base al brano che trattano, l'autore e/o la data. Per gli studi ospitati su questo sito, si pu&ograve; anche cercare uno studio biblico in base a qualche parola chiave nel testo dello studio, facendo una ricerca per la parola in tutto il sito.</p>
<p>Tutti possono leggere tutti gli studi del sito. Per&ograve;, per inserire un nuovo studio o dare un voto agli studi esistenti, &egrave; necessario prima iscriversi (gratuitamente), e poi quando ritorni a questo sito entrare come utente registrato.</p>

<h2>Trovare uno studio biblico</h2>
<form action="studi.php" method="post">
<table>
<tr><td><label for="brano">Brano:</label></td><td><input class="text" type="text" name="brano" id="brano" maxlength="255" size="30" /></td></tr>
<tr><td><label for="autore">Autore:</label></td><td><input class="text" type="text" name="autore" id="autore" maxlength="255" size="30" /></td></tr>
<tr><td><label for="data">Scritto negli ultimi</label></td><td><input class="text" type="text" name="data" id="data" maxlength="4" size="4" /> giorni</td></tr>
<tr><td><input class="submit" type="submit" name="Submit" value="Ricercare" /></td>
<td><!--<input class="reset" type="reset" name="Reset" value="Annulla" />--></td></tr>
</table>
</form>
<?
$sql = "SELECT COUNT(*) FROM Studi";
$nstudi = -1;
if ($ris = mysqli_query($conn, "$sql"))
  if ($row = mysqli_fetch_array($ris))
    $nstudi = $row[0];
echo "<p><a href=\"studi.php\" title=\"Un elenco di tutti gli studi sul sito - &egrave; lungo!\">Tutti gli studi</a>";
if ($nstudi>=0)
  echo " ($nstudi) ";
echo "</p>";

$sql = "SELECT DISTINCT Autori.id_a FROM Autori, Studi WHERE Autori.id_a=Studi.id_a";
if ($ris = mysqli_query($conn, "$sql"))
  $nautori = mysqli_num_rows($ris);
else
  $nautori = -1;
echo "<p><a href=\"autori.php\" title=\"Un elenco di tutte le persone che hanno contributo uno studio biblico\">Tutti gli autori</a>";
if ($nautori>=0)
  echo " ($nautori)";
echo "</p>";

if ($cid==0) {
?>
<h2>Registrazione</h2>
<p>Per entrare come utente registrato:</p>
<form action="/studi/" method="post">
<table>
<tr><td><label for="nome">Nome:</label></td><td><input class="text" type="text" name="nome" id="nome" maxlength="255" size="30"
<?
  echo " value='".$StudiNome."'"
?>
 /></td></tr>
<tr><td><label for="password">Password:</label></td><td><input class="text" type="password" name="password" id="password" maxlength="16" size="30" /></td></tr>
<tr><td><input class="submit" type="submit" name="Login" value="Entrare" /></td>
<td><!--<input class="reset" type="reset" name="Reset" value="Annulla" />--></td></tr>
</table>
</form>
<p><a href="pw.php" title="Spedisce la tua password al tuo indirizzo e-mail">Sono gi&agrave; iscritto ma ho dimenticato la mia password</a></p>
<p><a href="iscrizione.php" title="Apre la scheda di registrazione">Voglio iscrivermi</a></p>
<!--
<p><strong>Nota:</strong> &Egrave; pi&ugrave; veloce cercare uno studio su un brano senza visitare questo sito installando la <a href="/toolbar.php">toolbar</a> di LaParola.net.<br />
<a href="/toolbar.php"><img src="/immagini/toolbar.jpg" width="850" height="112" alt="La toolbar del sito" style="border:0px" /></a></p>
-->

<?
}
else {
?>
<h2>Per gli iscritti</h2>
<?
$InsStudi = "";
if (isset($_REQUEST["InsStudi"]))
  $InsStudi = $_REQUEST["InsStudi"];
$InsStudi = str_replace("<", "", $InsStudi);
$InsStudi = str_replace(">", "", $InsStudi);
$InsStudi = str_replace("\"", "", $InsStudi);
if ($InsStudi!="") {
  $valori = "";
  $titoli = "";
  for ($i=1; $i<=10; $i++) {
    $var = "titolo".$i;
    $ti = isset($_REQUEST[$var])?$_REQUEST[$var]:"";
    $ti = str_replace("<", "", $ti); // affinché tag HTML non possono essere inseriti nella pagina
    $ti = str_replace(">", "", $ti);
    $ti = str_replace("\"", "", $ti);
    $var = "brano".$i;
    $bi = isset($_REQUEST[$var])?$_REQUEST[$var]:"";
    $bi = str_replace("<", "", $bi); // affinché tag HTML non possono essere inseriti nella pagina
    $bi = str_replace(">", "", $bi);
    $bi = str_replace("\"", "", $bi);
    $var = "indirizzo".$i;
    $ii = isset($_REQUEST[$var])?$_REQUEST[$var]:"";
    $ii = str_replace("<", "", $ii); // affinché tag HTML non possono essere inseriti nella pagina
    $ii = str_replace(">", "", $ii);
    $ii = str_replace("\"", "", $ii);
    if ($ti!="" && $bi!="" && $ii!="") {
      $rif = converti_rif($bi);
      if ($rif=="")
        echo "Lo studio <strong>$ti</strong> non &egrave; stato aggiunto, perch&egrave; non &egrave; stato possibile capire il riferimento $bi.<br />";
      else {
        if ($titoli!="")
          $titoli .= ", ";
        $titoli .= $ti;
        if ($valori!="")
          $valori .= ",";
        $valori .= "($cid,\"".htmlentities($ti, ENT_QUOTES)."\",". ord($rif[0]).",".ord($rif[1]).",".ord($rif[2]).",".ord($rif[3]).",".ord($rif[4]).",".ord($rif[5]).",\"\",CURDATE(),\"$ii\")";
      }
    }
  }
  if ($valori!="") {
    $sql = "INSERT INTO Studi(id_a,titolo,libro1,capitolo1,versetto1,libro2,capitolo2,versetto2,testo,data,indirizzo) VALUES ".$valori;
    if (mysqli_query($conn, "$sql"))
      echo "Gli studi <strong>".$titoli."</strong> sono stati aggiunti.";
    else
      errore2("aggiornamento database con lo studio");
  }
}

$InsStudio = "";
if (isset($_REQUEST["InsStudio"]))
  $InsStudio = $_REQUEST["InsStudio"];
  $InsStudio = str_replace("<", "", $InsStudio); // affinché tag HTML non possono essere inseriti nella pagina
  $InsStudio = str_replace(">", "", $InsStudio);
  $InsStudio = str_replace("\"", "", $InsStudio);
if ($InsStudio!="") {
  $s = isset($_REQUEST["s"])?(int)$_REQUEST["s"]:0;
  $titolo = isset($_REQUEST["titolo"])?$_REQUEST["titolo"]:"";
  $titolo = str_replace("<", "", $titolo); // affinché tag HTML non possono essere inseriti nella pagina
  $titolo = str_replace(">", "", $titolo);
  $titolo = str_replace("\"", "", $titolo);
  $brano = isset($_REQUEST["brano"])?$_REQUEST["brano"]:"";
  $brano = str_replace("<", "", $brano); // affinché tag HTML non possono essere inseriti nella pagina
  $brano = str_replace(">", "", $brano);
  $brano = str_replace("\"", "", $brano);
  $testo = isset($_REQUEST["testo"])?$_REQUEST["testo"]:"";
  $testo = str_replace("<", "", $testo); // affinché tag HTML non possono essere inseriti nella pagina
  $testo = str_replace(">", "", $testo);
  $testo = str_replace("\"", "", $testo);
  $indirizzo = isset($_REQUEST["indirizzo"])?$_REQUEST["indirizzo"]:"";
  $indirizzo = str_replace("<", "", $indirizzo); // affinché tag HTML non possono essere inseriti nella pagina
  $indirizzo = str_replace(">", "", $indirizzo);
  $indirizzo = str_replace("\"", "", $indirizzo);
  $rif = "x";
  if ($s>=0)
    $rif = converti_rif($brano);
  if ($rif!="") {
    if ($s<0)
      $sql = "DELETE FROM Studi WHERE id_s=".-$s;
    if ($s==0)
      $sql = "INSERT INTO Studi(id_a,titolo,libro1,capitolo1,versetto1,libro2,capitolo2,versetto2,testo,data,indirizzo) VALUES ($cid,\"".htmlentities($titolo, ENT_QUOTES)."\",".ord($rif[0]).",".ord($rif[1]).",".ord($rif[2]).",".ord($rif[3]).",".ord($rif[4]).",".ord($rif[5]).",\"".htmlentities($testo, ENT_QUOTES)."\",CURDATE(),\"$indirizzo\")";
    if ($s>0)
      $sql = "UPDATE Studi SET titolo=\"".htmlentities($titolo, ENT_QUOTES)."\",libro1=".ord($rif[0]).",capitolo1=".ord($rif[1]).",versetto1=".ord($rif[2]).",libro2=".ord($rif[3]).",capitolo2=".ord($rif[4]).",versetto2=".ord($rif[5]).",testo=\"".htmlentities($testo, ENT_QUOTES)."\",indirizzo=\"$indirizzo\" WHERE id_s=$s";
    if (mysqli_query($conn, "$sql")) {
      if ($s<0)
        echo "Lo studio &egrave; stato rimosso.";
      if ($s==0)
        echo "Lo studio <strong>$titolo</strong> &egrave; stato aggiunto.";
      if ($s>0)
        echo "Lo studio <strong>$titolo</strong> &egrave; stato modificato.";
    }
    else
      errore2("aggiornamento database con lo studio");
  }
  else
    echo "Lo studio non &egrave; stato aggiunto, perch&egrave; non &egrave; stato possibile capire il riferimento $brano.";
}
if ($modregofatto==1)
  echo "<p>I tuoi dati personali sono stati modificati.</p>";
?>
<p><a href="regostud.php">Aggiungere un nuovo studio</a></p>
<p><a href="regonst.php">Aggiungere multipli studi</a></p>
<p><a href="modstud.php">Modificare o cancellare uno studio</a></p>
<p><a href="rego.php" title="Per cambiare la tua registrazione">Visualizzare o modificare i dati personali</a></p>
<p><a href="canrego.php" title="Tutti i tuoi studi saranno cancellati">Cancellare la propria iscrizione</a></p>
<?
}
require("../piede.php");
?>
