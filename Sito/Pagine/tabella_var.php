<?
function sanitizeVariabile($v) {
    $v = preg_replace_callback('/%[0-9A-F]{2}/i', function($match) {
        return strtolower($match[0]);
    }, $v);
    $v = str_replace(['<', '>', '"'], '', $v);
    $v = str_replace(['%3c', '%3e', '%22'], '', $v);
    return $v;
}

$parametri = "";
$utf8 = (isset($_REQUEST["utf8"])?(int)($_REQUEST["utf8"]):0);
$homepage = (isset($_REQUEST["homepage"])?($_REQUEST["homepage"]):"");
$homepage = sanitizeVariabile($homepage);

$brano = (isset($_REQUEST["brano"])?$_REQUEST["brano"]:"");
$brano = sanitizeVariabile($brano);
if ($brano!="")  {
  if ($homepage=="s")
	$parametri .= "&brano=".urlencode(utf8_encode($brano));
  else
	$parametri .= "&brano=".urlencode($brano);
}

$frase = (isset($_REQUEST["frase"])?$_REQUEST["frase"]:"");
$frase = str_replace("\\\"", "&quot;", $frase);
$frase = sanitizeVariabile($frase);
$frase = str_replace("\'", "' ", $frase);
$frase = str_replace("  ", " ", $frase);
if ($frase!="") {
  if ($homepage=="s")
	$parametri .= "&frase=".urlencode(str_replace("\\\\","\\",utf8_encode($frase)));
  else
	$parametri .= "&frase=".urlencode(str_replace("\\\\","\\",$frase));
}
if ($utf8==1 || $homepage!="s")
//if ($utf8==1)
  $frase = utf8_decode($frase);

$riferimento_predefinito = "Genesi 1:1";
$riferimento = (isset($_REQUEST["riferimento"])?$_REQUEST["riferimento"]:"");
$riferimento = sanitizeVariabile($riferimento);
if ($utf8==1)
  $riferimento = utf8_decode($riferimento);
if ($riferimento=="")
	$riferimento = $riferimento_predefinito;
else {
  if ($homepage=="s")
	$parametri .= "&riferimento=".urlencode(utf8_encode($riferimento));
  else
	$parametri .= "&riferimento=".urlencode($riferimento);
}
	
$formato_rif = (isset($_POST["formato_rif"])?$_POST["formato_rif"]:"xx");
if ($formato_rif=="xx")
  $formato_rif = isset($_GET["formato_rif"])?$_GET["formato_rif"]:"xx";
if ($formato_rif=="xx")
  $formato_rif = isset($_REQUEST["formato_rif"])?$_REQUEST["formato_rif"]:"xx";
if ($formato_rif=="xx")
  $formato_rif = isset($_COOKIE["formato_rif"])?$_COOKIE["formato_rif"]:"auto";
if ($formato_rif!="auto" && $formato_rif!="nn") {
  SetCookie("formato_rif", $formato_rif, time()+3600000);
  $parametri .= "&formato_rif=$formato_rif";
}

if ($utf8==1)
  $parametri .= "&utf8=1";
?>
