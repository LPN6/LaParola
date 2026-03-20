<?
header("Content-type: text/html; charset=utf-8");
$sezmag = (int)(isset($_REQUEST["sezmag"])?$_REQUEST["sezmag"]:0);
$sez1 = (int)(isset($_REQUEST["sez1"])?$_REQUEST["sez1"]:0);
$sez2 = (int)(isset($_REQUEST["sez2"])?$_REQUEST["sez2"]:0);
$LNGloss = (isset($_REQUEST["LNGloss"])?$_REQUEST["LNGloss"]:"");
  $LNGloss = str_replace("<", "", $LNGloss); // affinché tag HTML non possono essere inseriti nella pagina
  $LNGloss = str_replace(">", "", $LNGloss);
include("../conn.php");
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="en" xmlns="https://www.w3.org/1999/xhtml">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Greek New Testament - Louw-Nida lexicon</title>
<meta name="description" content="Table of contents of the Louw - Nida lexicon, with links to the words in each section" />
<meta name="keywords" content="Louw Nida,Louw-Nida,Louw,Nida,dictionary,lexicon,New Testament,Greek New Testament,Bible,Greek" />
<meta name="robots" content="nofollow" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
font-size: small;
}
</style>
</head><body>
<h1>Louw-Nida Lexicon</h1>
<form><p>Search for the Greek words that contain an English word in the gloss: 
<input class="text" name="LNGloss" value="" title="English word to search for" />&nbsp;
<input class="submit" type="submit" value="Find words" /></p></form>
<?if ($LNGloss!="") {
echo "<h3>$LNGloss</h3>";
$sql = "SELECT * FROM LNIndice LEFT JOIN GVocab ON LNIndice.id_r=GVocab.id_r WHERE Gloss LIKE \"%$LNGloss%\" ORDER BY SezioneMaggiore, SezioneMinore";
if ($ris=mysqli_query($conn, "$sql")) {
	if (mysqli_num_rows($ris)>0) {
		echo "<table><tr><th>Word</th><th>Gloss</th><th>Section</th></tr>";
		while ($row=mysqli_fetch_array ($ris))
		{
			$radice = $row["Radice"];
			$sezione = $row["SezioneMaggiore"].".".$row["SezioneMinore"];
			echo "<tr><td><span class=\"uni\"><a href=\"parola.php?p=$radice\">$radice</a></span></td><td>".$row["Gloss"]."</td><td><a href=\"index.php?TrovaVers=1&TrovaVers_Esp=/".$radice."&sect;$sezione\">$sezione</a></td></tr>\n";
		}
		echo "</table>";
	}
else
	echo "<p>No Greek word was found that contains $LNGloss in its gloss in the <a href=\"louwnida.php\">Louw-Nida lexicon</a>.";
}
}
else if ($sezmag==0){
	include("louwnidasommario.php");
}else{
echo "<h3>";
if ($sez2==0) {
  $sez2=9999;
  echo "Section <a href=\"louwnida.php#".$sezmag."\">".$sezmag."</a>";
}
else {
  if ($sez1==$sez2)
    echo "Section <a href=\"louwnida.php#".$sezmag."\">".$sezmag."</a>.".$sez1;
  else
    echo "Sections <a href=\"louwnida.php#".$sezmag."\">".$sezmag."</a>.".$sez1."-".$sez2;
}
echo "</h3>";

$sql="SELECT * FROM LNSommMag WHERE Numero=$sezmag";
if ($ris=mysqli_query($conn, "$sql")) {
  $row=mysqli_fetch_array($ris);
  echo "<h3>".$row["Titolo"]."</h3>";
}

$sql="SELECT * FROM LNSommMin WHERE id_lnsg=$sezmag AND SezioneMin=$sez1";
if ($ris=mysqli_query($conn, "$sql")) {
  $row=mysqli_fetch_array($ris);
  echo "<h4>".$row["Lettera"]." ".$row["Titolo"]."</h4>";
}

$sql="SELECT * FROM LNIndice LEFT JOIN GVocab ON LNIndice.id_r=GVocab.id_r WHERE SezioneMaggiore=$sezmag AND SezioneMinore>=$sez1 AND SezioneMinore<=$sez2 ORDER BY SezioneMinore";
$sezMinoreTrovata=99999;
$sezMaggioreTrovata=-1;
if ($ris=mysqli_query($conn, "$sql")) {
  echo "<table><tr><th>Word</th><th>Gloss</th><th>Section</th></tr>";
  while ($row=mysqli_fetch_array ($ris)) {
	  $radice = $row["Radice"];
	  $sezione = $row["SezioneMaggiore"].".".$row["SezioneMinore"];
	  echo "<tr><td><span class=\"uni\"><a href=\"parola.php?p=$radice\">$radice</a></span></td><td>".$row["Gloss"]."</td><td><a href=\"index.php?TrovaVers=1&TrovaVers_Esp=/".$radice."&sect;$sezione\">$sezione</a></td></tr>\n";
	  if ($row["SezioneMinore"]<$sezMinoreTrovata)
	    $sezMinoreTrovata=$row["SezioneMinore"];
	  if ($row["SezioneMinore"]>$sezMaggioreTrovata)
	    $sezMaggioreTrovata=$row["SezioneMinore"];
  }	
  echo "</table>";

  echo "<p>All the words in section:";
  for ($i=$sezMinoreTrovata; $i<=$sezMaggioreTrovata; ++$i)
    echo " <a href=\"index.php?TrovaVers=1&TrovaVers_Esp=&sect;$sezmag.$i\">$sezmag.$i</a>";
    echo "</p>";
    echo "<p><b>Note:</b> Only the words that are only in one section of Louw-Nida are included in the searches by section. In other words, those searches only work when there is no letter before the word(s) in the gloss.</p>";
  }
}
?>
</body>
</html>
