<?
$frase = (isset($_REQUEST["frase"])?$_REQUEST["frase"]:"");
$versione = (isset($_REQUEST["versione"])?$_REQUEST["versione"]:"");
$brano = (isset($_REQUEST["brano"])?$_REQUEST["brano"]:"");
$nBraniInizio = (isset($_REQUEST["nBraniInizio"])?(int)($_REQUEST["nBraniInizio"]):1);
$brano = str_replace("<", "", $brano); // affinché tag HTML non possono essere inseriti nella pagina
$brano = str_replace(">", "", $brano);
$frase = str_replace("\\\"", "&quot;", $frase);
$frase = str_replace("<", "", $frase);
$frase = str_replace(">", "", $frase);
$frase = str_replace("\'", "' ", $frase);
$frase = str_replace("  ", " ", $frase);
$versione = str_replace("<", "", $versione);
$versione = str_replace(">", "", $versione);

if ($nBraniInizio<1) $nBraniInizio=0;
if (isset($_REQUEST["nBraniFine"])) {
  if ($_REQUEST["nBraniFine"]=="")
    $nBraniFine = 50;
  else {
    $nBraniFine = (int)($_REQUEST["nBraniFine"]);
    if ($nBraniFine<0) $nBraniFine=0;
  }
}
else
  $nBraniFine = 50;
if ($nBraniFine-$nBraniInizio>50)
  $nBraniFine = nBraniInizio + 50;

    include("ricfrase.php");
    ricfrase($frase,$versione,$brano,$nBraniInizio,$nBraniFine,"auto",0);
    echo "<p>Da <a href='https://www.laparola.net/'>LaParola</a></p>\n";
?>
