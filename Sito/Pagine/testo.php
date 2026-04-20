<?
require("tabella_var.php");
 
$vers_mult = (isset($_POST["vers_mult"])?$_POST["vers_mult"]:"x");
if ($vers_mult=="x")
  $vers_mult = isset($_REQUEST["vers_mult"])?$_REQUEST["vers_mult"]:"x";
if ($vers_mult=="x")
  $vers_mult = isset($_COOKIE["vers_mult"])?$_COOKIE["vers_mult"]:"x";
if ($vers_mult!="x") {
  SetCookie("vers_mult",$vers_mult,time()+3600000);
  $parametri .= "&vers_mult=$vers_mult";
}
else
  $vers_mult="v";

$versioni = array();
if (isset($_REQUEST["versioni"]))
  $versionireq = $_REQUEST["versioni"];
if (isset($versionireq)) {
  if (is_array($versionireq)) {
    if (empty($versionireq)) {
        $versioni[0] = "Nuova Riveduta";
    } else {
        for ($i=0; $i<count($versionireq); $i++)
          $versioni[$i] = $versionireq[$i];
    }
  }
  else {
    $versioni[0] = $versionireq;
  }
}
else {
  $versioni[0] = "Nuova Riveduta";
}
$versioni = array_map('sanitizeVariabile', $versioni);

SetCookie("nVisVers",count($versioni),time()+3600000);
for ($i=0; $i<count($versioni); $i++) {
  SetCookie("VisVers".$i,str_replace(' ','',$versioni[$i]),time()+3600000);
  $parametri .= "&versioni[]=".urlencode($versioni[$i]);
}

$descriz = "Il testo di ".$riferimento.", un versetto o brano della Bibbia, nella/e versione/i ".implode(", ",$versioni).". Puoi visualizzare il testo anche in altre versioni e leggere diversi commentari.";
$key = $riferimento.",versetto,brano";
$titolo = $riferimento." (".implode(", ",$versioni).")";
$sezione = "Testo della Bibbia";
require("capo.php");
include("conn.php");
include("vistesto.php");
vistesto($riferimento, $versioni, $formato_rif, $vers_mult);

// questo codice era per mettere link ai capitoli precedenti e successivi, ma il sito non sa quale sia l'ultimo capito del libro precedente
// l'unico modo per farlo sarebbe di fare una query SQL sul libro in questa versione per trovare tutti i capitoli, e prendere il numero di capitolo più alto
// ma non funzionerebbe per saltare un libro dell'apocrifa'
/*
  if (!empty($riferimento)) {
    $rif3 = converti_rif($riferimento);
    if (strlen($rif3)==6) {
    	$capPrec="";
    	$libro = ord($rif3[0]);
    	$cap = ord($rif3[1]);
    	if ($libro>1 || $cap>1) {
    		$cap = $cap - 1;
    		if ($cap==0) {
				$lib = $lib - 1;
				
			}
			$rifP = converti_rif3($libro, $cap, 1, $libro, $cap, 177);
			$capPrec = "<a href=\"".$rifP."\">Capitolo precedente</a>&nbsp;";
		}
		echo "<p>".$capPrec."<a href=\"\">Capitolo successivo</a>.</p>";
	}
  }
*/

// il seguente codice quasi uguale anche in ricerca.php
include("geoip.inc");
$gi = geoip_open("GeoIP.dat",GEOIP_STANDARD);
$country_code = geoip_country_code_by_addr($gi, $_SERVER['REMOTE_ADDR']);
if ($country_code=="")
	$country_code = "XX";
geoip_close($gi);

$nr=0;$cei=0;$nd=0;$r2=0;$nr94=0;$bdg=0;$riv=0;$mar=0;$ricc=0;$tint=0;$dio=0;
$comm=0;$commnt=0;$rif=0;$commpulpito=0;$commillustratore=0;$commgill=0;$commbarnes=0;$commmeyer=0;$commtesoro=0;$commhenry=0;$commcalvino=0;$commginevra=0;
for ($i=0; $i<count($versioni); $i++) {
	if ($versioni[$i]=="Nuova Riveduta")
		$nr = 1;
	else if ($versioni[$i]=="C.E.I.")
		$cei = 1;
	else if ($versioni[$i]=="Nuova Diodati")
		$nd = 1;
    else if ($versioni[$i]=="Riveduta 2020")
        $r2 = 1;
    else if ($versioni[$i]=="Nuova Riveduta 1994")
        $nr94 = 1;
	else if ($versioni[$i]=="Bibbia della Gioia")
		$bdg = 1;
	else if ($versioni[$i]=="Riveduta")
		$riv = 1;
    else if ($versioni[$i]=="Ricciotti")
        $ricc = 1;
    else if ($versioni[$i]=="Tintori")
        $tint = 1;
	else if ($versioni[$i]=="Martini")
		$mar = 1;
	else if ($versioni[$i]=="Diodati")
		$dio = 1;
	else if ($versioni[$i]=="Commentario")
		$comm = 1;
	else if ($versioni[$i]=="CommentarioNT")
		$commnt = 1;
	else if ($versioni[$i]=="Riferimenti incrociati")
		$rif = 1;
	else if ($versioni[$i]=="CommentarioPulpito")
		$commpulpito = 1;
	else if ($versioni[$i]=="CommentarioIllustratore")
		$commillustratore = 1;
	else if ($versioni[$i]=="CommentarioGill")
		$commgill = 1;
	else if ($versioni[$i]=="CommentarioBarnes")
		$commbarnes = 1;
	else if ($versioni[$i]=="CommentarioMeyer")
		$commmeyer = 1;
	else if ($versioni[$i]=="CommentarioTesoro")
		$commtesoro = 1;
	else if ($versioni[$i]=="CommentarioHenry")
		$commhenry = 1;
	else if ($versioni[$i]=="CommentarioCalvino")
		$commcalvino = 1;
	else if ($versioni[$i]=="CommentarioGinevra")
		$commginevra = 1;
}
$sql = "INSERT INTO Statistiche (nr,cei,nd,r2,nr94,bdg,riv,ricc,tint,mar,dio,comm,commnt,rif,commpulpito,commillustratore,commgill,commbarnes,commmeyer,commtesoro,commhenry,commcalvino,commginevra,tipo,paese,mese,anno) VALUES ($nr,$cei,$nd,$r2,$nr94,$bdg,$riv,$ricc,$tint,$mar,$dio,$comm,$commnt,$rif,$commpulpito,$commillustratore,$commgill,$commbarnes,$commmeyer,$commtesoro,$commhenry,$commcalvino,$commginevra,'v','$country_code',".date('n').",".date('y').")";
mysqli_query($conn, "$sql");
?>
<hr />
<?
require("tabella.php");
if ($parametri!="") {
	$url = "https://www.laparola.net/testo.php?".substr($parametri,1);
    $url2 = $url;
//	$userAgent = strtolower($_SERVER['HTTP_USER_AGENT']); 
//	if (strpos($userAgent, "applewebkit") !== false)
//		$url2=str_replace("&","<br />&",$url);
	echo "<div style=\"word-wrap: break-word;word-break: break-all;\"><p><i>Indirizzo di questa pagina:</i><br /><a href=\"".$url."\">$url2</a></p></div>";
    
    //http://laparola/app/?w1=bible&t1=local%3Anr&v1=EX1_1&w2=commentary&t2=local%3Acommabbrmh&v2=EX1_1
    $url3 = converti_linkTestoContinuto($riferimento, $versioni);
    $url4 = $url3;
    echo "<div style=\"word-wrap: break-word;word-break: break-all;\"><p><i>Indirizzo del testo continuo:</i><br /><a href=\"".$url3."\" style=\"display: inline-block;min-height: 24px;\">$url4</a></p></div>";
}
?>
<!--
<script>
    window.onload = function() {
     var scrollX = window.scrollX;
      var scrollY = window.scrollY;
      document.getElementById("libri").focus();
       setTimeout(function() {
        window.scrollTo(scrollX, scrollY);
      }, 0);
    }
  </script>
  -->
<?
require("piede.php");
?>
