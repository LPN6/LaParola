<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="it" xmlns="http://www.w3.org/1999/xhtml">
<head>
<?
require("tabella_var.php");

$versioni = array();
if (isset($_REQUEST["versioni"]))
  $versionireq = $_REQUEST["versioni"];
if (isset($versionireq)) {
  if (is_array($versionireq)) {
    if (empty($versionireq)) {
        $versioni[0] = "Nuova Riveduta";
    } else {
        for ($i=0; $i<count($versionireq); $i++)
          $versioni[$i] = $versionireq[$i];
    }
  }
  else {
    $versioni[0] = $versionireq;
  }
}
else {
  $versioni[0] = "Nuova Riveduta";
}
$versioni = array_map('sanitizeVariabile', $versioni);

echo "<title>$riferimento</title>\n";
?>
<meta name="description" content="Il testo di <?echo $riferimento?> nella Bibbia, per una finestra popup" />
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<link rel="stylesheet" href="/stili/stilebase.css" type="text/css" />
</head>
<body>
<script type="text/javascript" language="JavaScript" src="/popup.js"></script>
<?
include("conn.php");
include("vistesto.php");

vistesto($riferimento, $versioni, "dv", "v", "s");

$vapp = "nr";
if (is_array($versioni)) {
  if (sizeof($versioni)>0)
    $vapp = convversionetoapp($versioni[0]);
}
else {
  if ($versioni != "")
    $vapp = convversionetoapp($versioni);
}
if (empty($riferimento))
  $riferimento = "Genesi 1:1";
$rif3 = converti_rif($riferimento);
$nBrani = strlen($rif3) / 6;
if ($nBrani>4) $nBrani = 4;
$s = "";

for ($i=1; $i<=$nBrani; ++$i) {
    if ($i>1) $s .= "&";
    $s .= "w$i=bible&t$i=local%3A$vapp&v$i=".convlibrotoapp(ord($rif3[6*$i-6])).ord($rif3[6*$i-5])."_".ord($rif3[6*$i-4]);
}

//http://laparola/app/?v1=NH11_9&t1=local%3Anr&w1=bible&w2=bible&t2=local%3Anr&v2=NH11_9
///?w1=bible&t1=local%3A".convversionetoapp($versione)."&v1=".convlibrotoapp($lib).$cap."_".$vers."\
if ($nBrani>0)
    echo "<p><a target=\"_blank\" href=\"/app/?".$s."\">Questo brano nel contesto</a></p>";
    
?>
<script type="text/javascript" charset="utf-8" src="/berea.js"></script>
<script type="text/javascript">LPNritardo = 500;</script>
</body>
</html>
