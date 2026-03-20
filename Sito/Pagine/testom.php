<?
$formato_rif = (isset($_POST["formato_rif"])?$_POST["formato_rif"]:"xx");
if ($formato_rif=="xx")
  $formato_rif = isset($_REQUEST["formato_rif"])?$_REQUEST["formato_rif"]:"auto";
if (isset($_REQUEST["versioni"])) {
  $versioni = $_REQUEST["versioni"];
  if (empty($versioni)) {
    $versioni = [];
  }}
$versioni[] = "Nuova Riveduta";
if (count($versioni)>1)
  unset($versioni[count($versioni)-1]);
SetCookie("nVisVers",count($versioni),time()+3600000);
for ($i=0; $i<count($versioni); $i++) {
  SetCookie("VisVers".$i,str_replace(' ','',$versioni[$i]),time()+3600000);
}
?>
<html lang="it"><head>
<?
$riferimento = (isset($_REQUEST["riferimento"])?$_REQUEST["riferimento"]:"Genesi 1:1");
$riferimento = str_replace("<", "", $riferimento); // affinché tag HTML non possono essere inseriti nella pagina
$riferimento = str_replace(">", "", $riferimento);
$riferimento = str_replace("\"", "", $riferimento);
$titolo = $riferimento;
$sezione = "Testo della Bibbia";
?>
<title>La Sacra Bibbia - <?echo $titolo?></title>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
</head><body>
<?
include("conn.php");
include("vistesto.php");
vistesto($riferimento,$versioni,$formato_rif,"o");
?>
<hr width="80%" /><strong>Nuovo brano:</strong><form action="testom.php" method="post" onsubmit="if (riferimento.value.length==0) {alert('Digitare il riferimento di un brano')}; return riferimento.value.length!=0;">
<?
echo "<p>Brano da visualizzare:&nbsp;<input type=\"text\" name=\"riferimento\" value=\"$riferimento\" /><p>";
for ($i=0; $i<count($versioni); $i++)
    echo "<input type=\"hidden\" name=\"versioni[]\" value=\"$versioni[$i]\" />";
echo "<p><input type=\"submit\" name=\"Submit\" value=\"Visualizza testo\" /></p>";
echo "</form>";
?>
<p><a href="/">Home</a> | <a href="/min.html">Home minimo</a></p></body></html>
