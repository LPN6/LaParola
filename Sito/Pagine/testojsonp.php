<?
$formato_rif = (isset($_POST["formato_rif"])?$_POST["formato_rif"]:"xx");
if ($formato_rif=="xx")
  $formato_rif = isset($_GET["formato_rif"])?$_GET["formato_rif"]:"xx";
if ($formato_rif=="xx")
  $formato_rif = isset($_REQUEST["formato_rif"])?$_REQUEST["formato_rif"]:"auto";
$vers_mult = (isset($_POST["vers_mult"])?$_POST["vers_mult"]:"x");
if ($vers_mult=="x")
  $vers_mult = isset($_REQUEST["vers_mult"])?$_REQUEST["vers_mult"]:"x";
if ($vers_mult=="x")
  $vers_mult="v";
if (isset($_REQUEST["versioni"]))
  $versioni = $_REQUEST["versioni"];
$versioni[] = "Nuova Riveduta";
if (count($versioni)>1)
  unset($versioni[count($versioni)-1]);

$riferimento = (isset($_REQUEST["riferimento"])?$_REQUEST["riferimento"]:"Genesi 1:1");
$riferimento = str_replace("<", "", $riferimento); // affinché tag HTML non possono essere inseriti nella pagina
$riferimento = str_replace(">", "", $riferimento);

$cb = $_GET['cb'];
$cb = str_replace("<", "", $cb); // affinché tag HTML non possono essere inseriti nella pagina
$cb = str_replace(">", "", $cb);

include("conn.php");
include("vistesto.php");
echo $cb . "({\"Testo\":\"" . str_replace(array("«","»","\"","<h3>","\n"), array("&laquo;","&raquo;","&quot;","<h3 class='LPNBerea'>", ""), gettesto($riferimento,$versioni,0,$formato_rif,$vers_mult)) . "\"});";
?>
