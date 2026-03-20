<?
header("Content-type: text/html; charset=utf-8");
$fontuni = (isset($_REQUEST["greco_fontuni"])?$_REQUEST["greco_fontuni"]:"");
  $fontuni = str_replace("<", "", $fontuni); // affinché tag HTML non possono essere inseriti nella pagina
  $fontuni = str_replace(">", "", $fontuni);
$letter = (int)(isset($_REQUEST["letter"])?$_REQUEST["letter"]:945);
if ($letter<945 || $letter>969 || $letter==962) $letter = 945;
$lin = (isset($_REQUEST["greco_lingua"])?$_REQUEST["greco_lingua"]:"");
  $lin = str_replace("<", "", $lin); // affinché tag HTML non possono essere inseriti nella pagina
  $lin = str_replace(">", "", $lin);
if ($lin=="")
   if (isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) $lin=substr($_SERVER['HTTP_ACCEPT_LANGUAGE'],0,2);
include("../conn.php");
include("funzioni_greco.php");
global $lin;

function unichr($dec) {
  if ($dec < 128) {
   $utf = chr($dec);
  } else if ($dec < 2048) {
   $utf = chr(192 + (($dec - ($dec % 64)) / 64));
   $utf .= chr(128 + ($dec % 64));
  } else {
   $utf = chr(224 + (($dec - ($dec % 4096)) / 4096));
   $utf .= chr(128 + ((($dec % 4096) - ($dec % 64)) / 64));
   $utf .= chr(128 + ($dec % 64));
  }
  return $utf;
}
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<?if ($lin=="it") echo "it"; else echo "en";?>">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title><?if ($lin=="it") echo "La Sacra Bibbia - Nuovo Testamento greco - Concordanza"; else echo "Greek New Testament - Concordance";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Una concordanza (chiave biblica) di parole greche nel Nuovo Testamento"; else echo "A concordance of words in the New Testament";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "concordanza,chiave,Nuovo Testamento,Nuovo Testamento greco,greco,bibbia"; else echo "concordance,New Testament,Greek New Testament,Bible,Greek";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
font-size: small;
}
.unih {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
}
</style>
</head>
<body>
<h1><?if ($lin=="it") echo "Concordanza nel Nuovo Testamento greco - <span class=\"unih\">".unichr($letter)."</span>"; else echo "Concordance of the Greek New Testament - <span class=\"unih\">".unichr($letter)."</span>";?></h1>
<?
$alfabeto = "abcdefghiklmnopqrsstuvxyz";
$lettord = $alfabeto[$letter-945];
$sql = "SELECT Radice FROM GVocab WHERE RadicePerOrdine LIKE \"$lettord%\" ORDER BY RadicePerOrdine";
if ($ris=mysqli_query($conn, "$sql")) {
	while ($row = mysqli_fetch_array ($ris)) {
		echo "<p><a href=\"parola.php?p=".$row["Radice"]."\"><span class=\"uni\">".$row["Radice"]."</span></a></p>";
	}
}

?>
</body>
</html>
