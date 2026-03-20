<?
$fontuni = (isset($_REQUEST["greco_fontuni"])?$_REQUEST["greco_fontuni"]:"");
  $fontuni = str_replace("<", "", $fontuni); // affinché tag HTML non possono essere inseriti nella pagina
  $fontuni = str_replace(">", "", $fontuni);
$lin = (isset($_REQUEST["greco_lingua"])?$_REQUEST["greco_lingua"]:"");
  $lin = str_replace("<", "", $lin); // affinché tag HTML non possono essere inseriti nella pagina
  $lin = str_replace(">", "", $lin);
if ($lin=="")
   if (isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) $lin=substr($_SERVER['HTTP_ACCEPT_LANGUAGE'],0,2);
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<?if ($lin=="it") echo "it"; else echo "en";?>">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title><?if ($lin=="it") echo "La Sacra Bibbia - Logos"; else echo "New Testament Manuscripts - Logos";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Come inserire le pagine di questo sito in Logos"; else echo "How to insert this site into Logos";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "Logos,Nuovo Testamento,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante"; else echo "Logos,New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<!-- da fare da canc se capo.php e' inserito -->
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,Palatino Linotype,Tahoma;
font-size: small;
}
</style>
</head>
<body>
<h1>How to insert this site into the <a href="http://www.logos.com/">Logos</a> program</h1>
<p>You can create a personal book in Logos 4 with the variant readings listed in this site.</p>
<ol>
<li>Download <a href="/file/Variant Readings of the New Testament.docx">this docx file</a>.</li>
<li>Open Logos.</li>
<li>Choose the <i>Personal Books</i> item from the <i>Tools</i> menu.</li>
<li>Click <i>Add book</i>.</li>
<li>Type "Variant Readings" in the first box of the library information, and choose the type "Bible Apparatus".</li>
<li>Click the <i>Add file</i> button, and go to the directory where you downloaded the file and choose the file.</li>
<li>Click the <i>Build book</i> button, and wait.</li>
</ol>
</body>
</html>
