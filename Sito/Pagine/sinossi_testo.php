<?
include("conn.php");
include("vistesto.php");

$formato_rif = isset($_COOKIE["formato_rif"])?$_COOKIE["formato_rif"]:"auto";

$versione = isset($_REQUEST["versione"]) ? $_REQUEST["versione"] : "";
$versioni[] = $versione;
$versioni[] = "Nuova Riveduta";
if (count($versioni)>1)
  unset($versioni[count($versioni)-1]);

$sin_riferimento = (isset($_REQUEST["sin_riferimento"])?$_REQUEST["sin_riferimento"]:"Lu 1:1");
$sin_riferimento = str_replace("<", "", $sin_riferimento); // affinché tag HTML non possono essere inseriti nella pagina
$sin_riferimento = str_replace(">", "", $sin_riferimento);

echo "<html><body>\n";
echo "<link rel=\"stylesheet\" href=\"/stili/stilebase6.css\" type=\"text/css\" />";
//echo "<p>rif=".$sin_riferimento.".</p>";
if (strlen($sin_riferimento)>0)
	vistesto($sin_riferimento, $versioni, $formato_rif);
echo "\n</body></html>";

?>
