<?
function sanitizeVariabile($v) {
    $v = preg_replace_callback('/%[0-9A-F]{2}/i', function($match) {
        return strtolower($match[0]);
    }, $v);
    $v = str_replace(['<', '>', '"'], '', $v);
    $v = str_replace(['%3c', '%3e', '%22'], '', $v);
    return $v;
}

header("Content-type: text/html; charset=utf-8");
$fontuni = (isset($_REQUEST["greco_fontuni"])?$_REQUEST["greco_fontuni"]:"");
$fontuni = sanitizeVariabile($fontuni);
$p = (isset($_REQUEST["p"])?$_REQUEST["p"]:"aßßa");
$p = sanitizeVariabile($p);
$vers = (int)(isset($_REQUEST["vers"])?$_REQUEST["vers"]:0);
$lin = (isset($_REQUEST["greco_lingua"])?$_REQUEST["greco_lingua"]:"");
$lin = sanitizeVariabile($lin);
if ($lin=="")
   if (isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) $lin=substr($_SERVER['HTTP_ACCEPT_LANGUAGE'],0,2);
include("../conn.php");
include("funzioni_greco.php");
global $lin;

function MostraInVersione ($nRadice, $p, $vers="") {
global $conn;
global $lin;
	$sql = "SELECT Parola,Grammatica,Count(*) FROM Chiave$vers, GParole$vers WHERE Chiave$vers.id_p=GParole$vers.id_p AND id_r=$nRadice GROUP BY Chiave$vers.id_p, Grammatica ORDER BY ParolaPerOrdine";
  $ris = mysqli_query($conn, "$sql");
  echo "<table style=\"table-layout:fixed;width:100%;\">\n";
  $nVolte = 0;
  $VersDaRicerc = "";
  $VersDaMostrare = "";
  if ($vers!="") {
  	$VersDaRicerc = "&TrovaVers_Versione=$vers";
	$VersDaMostrare = "&".strtolower($vers)."=s";
  }
  while ($row=mysqli_fetch_array ($ris)) {
  	$parola = $row["Parola"];
  	$grammatica = $row["Grammatica"];
  	$nParola = $row[2];
  	echo "<tr><td><div style=\"overflow-wrap:break-word\"><a href=\"index.php?TrovaVers=1$VersDaRicerc&TrovaVers_Esp=$parola/$p".$VersDaMostrare."\"><span class=\"uni\">$parola</span></a></div></td><td><div style=\"overflow-wrap:break-word\"><a href=\"index.php?TrovaVers=1$VersDaRicerc&TrovaVers_Esp=/$p".urlencode("##".ConvPersona($grammatica)).$VersDaMostrare."\">".TradGram($grammatica)."</a></div></td><td align=\"right\" width=\"10%\"><a href=\"index.php?TrovaVers=1$VersDaRicerc&TrovaVers_Esp=$parola/$p".urlencode("##".ConvPersona($grammatica)).$VersDaMostrare."\">$nParola</a></td></tr>\n";
  	$nVolte += $nParola;
	}
  if ($lin=="it")
		echo "<tr><th rowspan=\"2\">Totale</th>";
	else
		echo "<tr><th rowspan=\"2\">Total</th>";
	echo "<th align=\"right\" width=\"10%\"><a href=\"index.php?TrovaVers=1$VersDaRicerc&TrovaVers_Esp=/$p".$VersDaMostrare."\">$nVolte</a></th></tr></table>\n";
}

function MostraLouwNida($radice) {
global $conn;
global $lin;
  $sql = "SELECT * FROM LNIndice WHERE id_r=$radice ORDER BY SezioneMaggiore, SezioneMinore";
  if ($ris2=mysqli_query($conn, "$sql")) {
  	if (mysqli_num_rows($ris2)>0) {
  		echo "<h3>Louw-Nida</h3>";
			if ($lin=="it") echo "<table><tr><th>Definizione</th><th>Sezione</th></tr>"; else echo "<table><tr><th>Gloss</th><th>Section</th></tr>";
  		while ($row2=mysqli_fetch_array ($ris2))
  			echo "<tr><td>".$row2["Gloss"]."</td><td><a href=\"louwnida.php#".$row2["SezioneMaggiore"]."\">".$row2["SezioneMaggiore"]."</a>.<a href=\"louwnida.php?sezmag=".$row2["SezioneMaggiore"]."&sez1=".$row2["SezioneMinore"]."&sez2=".$row2["SezioneMinore"]."\">".$row2["SezioneMinore"]."</a></td></tr>\n";
  		echo "</table>";
  	}
  }
}

//function CambiaAccenti($p) {
//  $p2= strtr($p, "?x", "?x");
//  echo $p."w".$p2;
//  return $p2;
//}
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<?if ($lin=="it") echo "it"; else echo "en";?>">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title><?if ($lin=="it") echo "La Sacra Bibbia - Nuovo Testamento greco - $p"; else echo "Greek New Testament - $p";?></title>
<meta name="description" content="<?if ($lin=="it") echo "La parola $p nel Nuovo Testamento"; else echo "The word $p in the New Testament";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "$p,Nuovo Testamento,Nuovo Testamento greco,greco,bibbia"; else echo "$p,New Testament,Greek New Testament,Bible,Greek";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<meta name="robots" content="nofollow" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
font-size: small;
}
.unih {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
}
.link_no_sotto a:visited {text-decoration: none; color:black}
.link_no_sotto a:link {text-decoration: none; color:black}
.link_no_sotto a:hover {text-decoration: underline;}
.link_no_sotto a:active {text-decoration: underline;}
</style>
</head>
<body>
<h1><?if ($lin=="it") echo "<span class=\"unih\">$p</span> nel Nuovo Testamento"; else echo "<span class=\"unih\">$p</span> in the New Testament";?></h1>
<?
$radice_font = "";
$sql = "SELECT * FROM GVocab WHERE Radice=\"$p\"";
if ($ris=mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris)==0) {
    // radice non esiste, cerchiamo una parola con la stessa radice
    $sql2 = "SELECT * FROM GParole, Chiave, GVocab WHERE GParole.id_p=Chiave.id_p AND GVocab.id_r=Chiave.id_r AND Parola=\"$p\"";
    if ($ris2=mysqli_query($conn, "$sql2")) {
      if (mysqli_num_rows($ris2)>0) {
        $row = mysqli_fetch_array ($ris2);
        $p = $row["Radice"];
        $sql = "SELECT * FROM GVocab WHERE Radice=\"$p\"";
        $ris=mysqli_query($conn, "$sql");
      }
    }
    else {
      errore2("interrogazione database per trovare la parola $p");
    }
  }
}
else {
     errore2("interrogazione database per trovare la parola $p");
}

   if (mysqli_num_rows($ris)>0) {
      $row = mysqli_fetch_array ($ris);
      $radice_font = $row["RadiceFont"];
      $nRadice = $row["id_r"];
      $thayer = strtolower($row["RadicePerOrdine"]);
      // alphabet$ = "abxdevcgi.klmnpqhrstu.zoyf"
	  $thayer = str_replace(array("x","v","c","g","p","q","h","z","o","f"), array("CH","PH","G","E","O","P","TH","O","X","Z"), $thayer);
	  $thayer = strtolower($thayer);
	  $primocarattere = ord($radice_font[0]);
	  if ($primocarattere==131 || $primocarattere==152 || $primocarattere==161 || $primocarattere==185 || $primocarattere==208 || $primocarattere==216 || $primocarattere==230)
	  	$thayer = "h".$thayer;
	  if ($primocarattere==96 || $primocarattere==183) // rho con h
	  	$thayer = "rh".substr($thayer,1);
	  if ($thayer=="dauid") $thayer="dabid";

      if ($lin=="it") {
      	echo "<h2>Definizioni</h2>";
      	if ($radice_font!="") {
      		echo "<h3>LaParola</h3>";
      		echo "<p><a href=\"/vocab/parole.php?parola=".urlencode($radice_font)."\">Dal vocabolario di questo sito</a> (in italiano)</p>\n";
      	}
      	echo "<h3>Thayer</h3>";
      	echo "<p>".$row["Thayer"]."</p>\n";
      	echo "<h3>Strong</h3>";
        echo "<p>".$row["Strong"]."</p>\n";
      	MostraLouwNida($nRadice);        	
      	echo "<h3>Dizionari di greco classico</h3>";
		echo "<p>Questi due siti restituiscono informazioni simili, con le definizioni da diversi dizionari e statistiche sull'uso della parola.</p>\n";
      	echo "<p><a href=\"http://www.perseus.tufts.edu/hopper/morph?l=$p\">Perseus Digital Library</a></p>\n";
		echo "<p><a href=\"http://logeion.uchicago.edu/index.html#$p\">University of Chicago's Logion lexicon</a></p>\n";
//      	echo "<h3>ZHubert</h3>";
//      	echo "<p><a href=\"http://www.zhubert.com/word?root=$p\">Un dizionario inglese</a>, con alcune informazioni mancanti per il sito non è più attivo</p>\n";
      	if ($row["NumeroStrong"]!="" && $row["NumeroStrong"]!="0") {
      		echo "<h3>Ulrik Sandborg-Petersen</h3>";
      		echo "<p><a href=\"http://greeklexicon.org/lexicon/strongs/".$row["NumeroStrong"]."\">Vocabolario di Strong</a></p>\n";
      		echo "<h3>Crosswalk</h3>";
      		echo "<p><a href=\"http://www.biblestudytools.net/lexicons/greek/nas/".$thayer.".html\">Vocabolario di Thayer</a> pi&ugrave; altre informazioni.</p>\n";
				}
      } else {
      	echo "<h2>Definitions</h2>";
      	echo "<h3>Thayer</h3>";
      	echo "<p>".$row["Thayer"]."</p>\n";
      	echo "<h3>Strong</h3>";
      	echo "<p>".$row["Strong"]."</p>\n";
      	MostraLouwNida($nRadice);
      	echo "<h3>Classical Greek Dictionaries</h3>";
		echo "<p>These two sites give similar information, with the definition from several dictionaries and statistics on the use of the word.</p>\n";
      	echo "<p><a href=\"http://www.perseus.tufts.edu/hopper/morph?l=$p\">Perseus Digital Library</a></p>\n";
		echo "<p><a href=\"http://logeion.uchicago.edu/index.html#$p\">University of Chicago's Logion lexicon</a></p>\n";
      	if ($row["NumeroStrong"]!="" && $row["NumeroStrong"]!="0") {
      		echo "<h3>Ulrik Sandborg-Petersen</h3>";
      		echo "<p><a href=\"http://greeklexicon.org/lexicon/strongs/".$row["NumeroStrong"]."\">Strong's dictionary</a></p>\n";
      		echo "<h3>Crosswalk</h3>";
      		echo "<p><a href=\"http://www.biblestudytools.net/lexicons/greek/nas/".$thayer.".html\">Thayer's dictionary</a> plus other information.</p>\n";
				}      		
//      	echo "<h3>ZHubert</h3>";
//      	echo "<p><a href=\"http://www.zhubert.com/word?root=$p\">A dictionary</a>, but with some missing information because the site is no longer active.</p>\n";
      	if ($radice_font!="") {
      		echo "<h3>LaParola</h3>";
      		echo "<p><a href=\"/vocab/parole.php?parola=".urlencode($radice_font)."\">From this site's dictionary</a> (in Italian)</p>\n";
      	}
      }
      if ($lin=="it")
      	echo "<h2>Nel Nuovo Testamento</h2>";
      else
      	echo "<h2>In the New Testament</h2>";
//      mysqli_query($conn, "SET CHARACTER SET 'utf8'");
//      $sql = "SELECT Parola,Grammatica,Count(*) FROM Chiave WHERE id_r=$nRadice GROUP BY Parola COLLATE utf8_unicode_ci, Grammatica";
      if ($vers==0) {
				echo "<h3>SBL (<a href=\"parola.php?p=$p&vers=1\">";
				if ($lin=="it") echo "anche Westcott e Hort; Tischendorf; Bizantino"; else echo "also Westcott and Hort; Tischendorf; Byzantine";
				echo "</a>)</h3>";
	      echo "<span class='link_no_sotto'>";
      	MostraInVersione($nRadice, $p);
      }
      else {
	      echo "<span class='link_no_sotto'>";
      	echo "<table style=\"width:95%;\"><tr><th width=\"25%\">SBL</th><th></th><th>Westcott/Hort</th><th></th><th>Tischendorf</th><th></th>";
				if ($lin=="it") echo "<th>Bizantino</th>"; else echo "<th>Byzantine</th>";      	
				echo "</tr><tr valign=\"top\"><td>";
      	MostraInVersione($nRadice, $p);
      	echo "</td><td></td><td>";
      	MostraInVersione($nRadice, $p, "WH");
      	echo "</td><td></td><td>";
      	MostraInVersione($nRadice, $p, "Tisch");
      	echo "</td><td></td><td>";
      	MostraInVersione($nRadice, $p, "Biz");
      	echo "</td></tr></table>";
      }
			echo "</span>";
      if ($lin=="it")
				echo "<p>Clicca sulla prima colonna per cercare quella parola come forma della radice <span class=\"uni\">$p</span>; clicca sulla seconda colonna per cercare quella forma grammaticale della radice <span class=\"uni\">$p</span>; clicca sulla terza colonna per cercare quella parola e forma grammaticale; clicca sul totale per cercare la radice <span class=\"uni\">$p</span>.</p>\n";
			else
				echo "<p>Click on the first column to search for that word as a form of the root <span class=\"uni\">$p</span>; click on the second column to search for that grammatical form of the root <span class=\"uni\">$p</span>; click on the third column to search for that word and grammatical form; click on the total to search for the root <span class=\"uni\">$p</span>.</p>\n";
   }
   else {
      if ($lin=="it") {
      	echo "<p>La parola <span class=\"uni\">$p</span> non &egrave; stata trovata.</p>";
      }
      else {
      	echo "<p>The word <span class=\"uni\">$p</span> was not found.</p>";
      }
   }

?>
</body>
</html>
