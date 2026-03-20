<?
$versione = (isset($_REQUEST["versione"])?$_REQUEST["versione"]:"Nuova Riveduta");
$brano = (isset($_REQUEST["brano"])?$_REQUEST["brano"]:"");
$frase = (isset($_REQUEST["frase"])?$_REQUEST["frase"]:"");
$formato_rif = (isset($_POST["formato_rif"])?$_POST["formato_rif"]:"xx");
if ($formato_rif=="xx") $formato_rif = isset($_REQUEST["formato_rif"])?$_REQUEST["formato_rif"]:"auto";
$nBraniInizio = (isset($_REQUEST["nBraniInizio"])?(int)($_REQUEST["nBraniInizio"]):1);
if ($nBraniInizio<1) $nBraniInizio=0;
$nBraniFine = (isset($_REQUEST["nBraniFine"])?(int)($_REQUEST["nBraniFine"]):50);
if ($nBraniFine<0) $nBraniFine=0;
$brano = str_replace("<", "", $brano); // affinché tag HTML non possono essere inseriti nella pagina
$brano = str_replace(">", "", $brano);
$brano = str_replace("\"", "", $brano);
$frase = str_replace("\\\"", "&quot;", $frase);
$frase = str_replace("<", "", $frase);
$frase = str_replace(">", "", $frase);
$frase = str_replace("\'", "' ", $frase);
$frase = str_replace("  ", " ", $frase);
$frase = str_replace("\"", "", $frase);
$versione = str_replace("<", "", $versione);
$versione = str_replace(">", "", $versione);
$versione = str_replace("\"", "", $versione);
SetCookie("RicVers",str_replace(' ','',$versione),time()+3600000);
?>
<html lang="it"><head>
<?
$titolo = str_replace("\\\\","&#92;",$frase);
$sezione = "Testo della Bibbia";
?>
<title>La Sacra Bibbia - <?echo $titolo?></title>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
</head>
<body>
<script type="text/javascript" language="JavaScript" src="/popup.js"></script>
<?
include("ricfrase.php");
ricfrase($frase,$versione,$brano,$nBraniInizio,$nBraniFine,$formato_rif);
?>
<hr width="80%" /><strong>Nuova ricerca:</strong><form action="ricercam.php" method="post" onsubmit="if (frase.value.length==0) {alert('Digitare una parola o espressione da ricercare')}; return frase.value.length!=0;">
<?
echo "<p>Espressione da ricercare:&nbsp;<input type=\"text\" name=\"frase\" value=\"".str_replace("\\\\","&#92;",$frase)."\" /><p>";
$nBraniDaMostrare = $nBraniFine - $nBraniInizio + 1;
if ($nBraniFine==0) $nBraniDaMostrare = 0;
echo "<p>Massimo numero di versetti da mostrare:&nbsp;<input type=\"text\" size=\"4\" name=\"nBraniFine\" value=\"$nBraniDaMostrare\" />&nbsp;(0 per tutti)<p>";
echo "<input type=\"hidden\" name=\"versione\" value=\"$versione\" />";
echo "<input type=\"hidden\" name=\"brano\" value=\"$brano\" />";
//echo "<input type=\"hidden\" name=\"nBraniInizio\" value=\"$nBraniInizio\" />";
echo "<p><input type=\"submit\" name=\"Submit\" value=\"Ricerca\" /></p>";
echo "</form>";
echo "<hr width=\"80%\" />";
?>
<p><a href="/">Home</a> | <a href="/min.html">Home minimo</a></p></body></html>
