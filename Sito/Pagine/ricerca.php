<?
require("tabella_var.php");

$versione = (isset($_REQUEST["versione"])?$_REQUEST["versione"]:"");
$versione = sanitizeVariabile($versione);
if ($versione=="")
	$versione = "Nuova Riveduta";
else {
	SetCookie("RicVers",str_replace(' ','',str_replace(' ','',$versione)),time()+3600000);
	$parametri .= "&versione=".urlencode($versione);
}

$nBraniInizio = (isset($_REQUEST["nBraniInizio"])?(int)($_REQUEST["nBraniInizio"]):0);
if ($nBraniInizio<1)
	$nBraniInizio=1;
else
	$parametri .= "&nBraniInizio=$nBraniInizio";
if (isset($_REQUEST["nBraniFine"])) {
  if ($_REQUEST["nBraniFine"]=="")
    $nBraniFine = 50;
  else {
    $nBraniFine = (int)($_REQUEST["nBraniFine"]);
    if ($nBraniFine<0)
		$nBraniFine=0;
	else
		$parametri .= "&nBraniFine=$nBraniFine";  }
}
else
  $nBraniFine = 50;

$descriz = $frase.", i versetti della Bibbia che contengono questa parola o frase";
$key = $frase.",ricerca,parola,frase,bibbia,parola,italiana";
$titolo = str_replace("\\\\","&#92;",utf8_encode($frase));
$sezione = "Testo della Bibbia";
require("capo.php");
require("ricfrase.php");

//if ($homepage=="s")
    ricfrase(utf8_encode($frase),$versione,utf8_encode($brano),$nBraniInizio,$nBraniFine,$formato_rif,1,"n",$homepage);
//else
//    ricfrase($frase,$versione,$brano,$nBraniInizio,$nBraniFine,$formato_rif,1,"n",$homepage);

// il seguente codice quasi uguale anche in testo.php
include("geoip.inc");
$gi = geoip_open("GeoIP.dat",GEOIP_STANDARD);
$country_code = geoip_country_code_by_addr($gi, $_SERVER['REMOTE_ADDR']);
if ($country_code=="")
	$country_code = "XX";
geoip_close($gi);

$nr=0;$cei=0;$nd=0;$r2=0;$nr94=0;$bdg=0;$riv=0;$ricc=0;$tint=0;$mar=0;$dio=0;
$comm=0;$commnt=0;$rif=0;$commpulpito=0;$commillustratore=0;$commgill=0;$commbarnes=0;$commmeyer=0;$commtesoro=0;$commhenry=0;$commcalvino=0;$commginevra=0;
	if ($versione=="Nuova Riveduta")
		$nr = 1;
	else if ($versione=="C.E.I.")
		$cei = 1;
	else if ($versione=="Nuova Diodati")
		$nd = 1;
    else if ($versione=="Riveduta 2020")
        $r2 = 1;
    else if ($versione=="Nuova Riveduta 1994")
        $nr94 = 1;
	else if ($versione=="Bibbia della Gioia")
		$bdg = 1;
	else if ($versione=="Riveduta")
		$riv = 1;
    else if ($versione=="Ricciotti")
        $ricc = 1;
    else if ($versione=="Tintori")
        $tint = 1;
	else if ($versione=="Martini")
		$mar = 1;
	else if ($versione=="Diodati")
		$dio = 1;
	else if ($versione=="Commentario")
		$comm = 1;
	else if ($versione=="CommentarioNT")
		$commnt = 1;
	else if ($versione=="Riferimenti incrociati")
		$rif = 1;
	else if ($versione=="CommentarioPulpito")
		$commpulpito = 1;
	else if ($versione=="CommentarioIllustratore")
		$commillustratore = 1;
	else if ($versione=="CommentarioGill")
		$commgill = 1;
	else if ($versione=="CommentarioBarnes")
		$commbarnes = 1;
	else if ($versione=="CommentarioMeyer")
		$commmeyer = 1;
	else if ($versione=="CommentarioTesoro")
		$commtesoro = 1;
	else if ($versione=="CommentarioHenry")
		$commhenry = 1;
    else if ($versione=="CommentarioCalvino")
		$commcalvino = 1;
	else if ($versione=="CommentarioGinevra")
		$commginevra = 1;

$sql = "INSERT INTO Statistiche (nr,cei,nd,r2,nr94,bdg,riv,ricc,tint,mar,dio,comm,commnt,rif,commpulpito,commillustratore,commgill,commbarnes,commmeyer,commtesoro,commhenry,commcalvino,commginevra,tipo,paese,mese,anno) VALUES ($nr,$cei,$nd,$r2,$nr94,$bdg,$riv,$ricc,$tint,$mar,$dio,$comm,$commnt,$rif,$commpulpito,$commillustratore,$commgill,$commbarnes,$commmeyer,$commtesoro,$commhenry,$commcalvino,$commginevra,'r','$country_code',".date('n').",".date('y').")";
mysqli_query($conn, "$sql");
?>
<hr />
<?
//if ($homepage=="s")
    $brano = utf8_encode($brano);
require("tabella.php");
if ($parametri!="") {
	$url = "https://www.laparola.net/ricerca.php?".substr($parametri,1);
	echo "<div style=\"word-wrap:break-word;word-break:break-all;\"><p><i>Indirizzo di questa pagina:</i><br /><a href=\"".$url."\">$url</a></p></div>";
}

require("piede.php");
?>
