<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="it" xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<?
$versioni = array();
if (isset($_REQUEST["versioni"]))
  $versioni = $_REQUEST["versioni"];
$riferimento = "";
if (isset($_REQUEST["riferimento"]))
  $riferimento = $_REQUEST["riferimento"];
$riferimento = str_replace("<", "", $riferimento); // affinché tag HTML non possono essere inseriti nella pagina
$riferimento = str_replace(">", "", $riferimento);
$riferimento = str_replace("+", "", $riferimento);
$riferimento = str_replace("\"", "", $riferimento);
if (empty($riferimento))
  $riferimento = "Genesi 1:1";
echo "<title>$riferimento</title>";
?>
<link rel="stylesheet" href="/stili/stilebase.css" type="text/css" />
</head>
<body>
<script type="text/javascript" language="JavaScript" src="/popup.js"></script>
<?
include("conn.php");
include("vistesto.php");
vistesto($riferimento, $versioni);
?>
</body>
</html>
