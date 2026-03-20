<?
$formato_rif = (isset($_POST["formato_rif"])?$_POST["formato_rif"]:"xx");
if ($formato_rif=="xx")
  $formato_rif = isset($_GET["formato_rif"])?$_GET["formato_rif"]:"xx";
if ($formato_rif=="xx")
  $formato_rif = isset($_REQUEST["formato_rif"])?$_REQUEST["formato_rif"]:"auto";

$riferimento = (isset($_REQUEST["riferimento"])?$_REQUEST["riferimento"]:"");
if (isset($_REQUEST["versioni"]))
  $versioni = $_REQUEST["versioni"];
else
  $versioni[] = "Nuova Riveduta";

    include("conn.php");
    include("vistesto.php");
    vistesto($riferimento, $versioni, $formato_rif);
    echo "<p>Da <a href='https://www.laparola.net/'>LaParola</a></p>\n";
?>
