<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<?
$formato_rif = (isset($_POST["formato_rif"])?$_POST["formato_rif"]:"xx");
if ($formato_rif=="xx")
  $formato_rif = isset($_REQUEST["formato_rif"])?$_REQUEST["formato_rif"]:"auto";
$versioni[] = "C.E.I.";
$versioni[] = "Nuova Riveduta";
$versioni[] = "Nuova Diodati";
$versioni[] = "Riveduta 2020";
?>
<html lang="it" xmlns="http://www.w3.org/1999/xhtml"><head>
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<?
$riferimento = (isset($_REQUEST["riferimento"])?$_REQUEST["riferimento"]:"Genesi 1:1");
$riferimento = str_replace("<", "", $riferimento); // affinché tag HTML non possono essere inseriti nella pagina
$riferimento = str_replace(">", "", $riferimento);
$riferimento = str_replace("\"", "", $riferimento);
$riferimento = str_replace("+", "", $riferimento);
$titolo = $riferimento." (".implode(", ",$versioni).")";
$sezione = "Testo della Bibbia";
?>
<title>La Sacra Bibbia - <?echo $titolo?></title>
<meta name="description" content="Il testo di <?echo $riferimento?> nella Bibbia, nelle versioni C.E.I. (1974), Nuova Riveduta, Nuova Diodati e Riveduta 2020, per i link che vengono da Wikipedia" />
<style>
body{font-size:120%;}@media(min-width:768px){body{font-size:medium;}}
</style>
</head>
<body>
<h1><?echo $riferimento?></h1>
<?
include("conn.php");
include("vistesto.php");
vistesto($riferimento,$versioni,$formato_rif,"v");
?>
<hr style="width:80%" /><p><strong>Nuovo brano:</strong></p><form action="testom.php" method="post" onsubmit="if (riferimento.value.length==0) {alert('Digitare il riferimento di un brano')}; return riferimento.value.length!=0;">
<?
echo "<p>Brano da visualizzare:&nbsp;<input type=\"text\" name=\"riferimento\" value=\"$riferimento\" />";
for ($i=0; $i<count($versioni); $i++)
    echo "<input type=\"hidden\" name=\"versioni[]\" value=\"$versioni[$i]\" />";
echo "</p><p><input type=\"submit\" name=\"Submit\" value=\"Visualizza testo\" /></p>";
echo "</form>";
?>
<p><a href="/"><img src="/immagini/bibbia.gif" alt="Bibbia" />(Da https://www.laparola.net/)</a></p>

</body>
</html>
