<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="it" xmlns="http://www.w3.org/1999/xhtml">
<head>
<?
require("tabella_var.php");

$versione = (isset($_REQUEST["versione"])?$_REQUEST["versione"]:"");
$versione = sanitizeVariabile($versione);
if ($versione=="")
	$versione = "Nuova Riveduta";

echo "<title>$frase</title>\n";
?>
<link rel="stylesheet" href="/stili/stilebase.css" type="text/css" />
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
</head>
<body>
<script type="text/javascript" language="JavaScript" src="/popup.js"></script>
<?
include("ricfrase.php");
if (empty($versione))
    $versione = "Nuova Riveduta";
if (empty($brano))
    $brano = "";
if (empty($frase))
    $frase = "";
ricfrase($frase, $versione, $brano, 1,0,"dv",1,"s");
?>
</body>
</html>
