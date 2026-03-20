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
<title><?if ($lin=="it") echo "La Sacra Bibbia - Manoscritti del Nuovo Testamento - Sigle"; else echo "New Testament Manuscripts - Manuscript codes";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Le sigle che si possono usare per confrontare due manoscritti del Nuovo Testamento"; else echo "The codes that can be used to compare two New Testament manuscripts";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "sigla,sigle,Nuovo Testamento,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante"; else echo "code,codes,New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<!-- da fare da canc se capo.php e' inserito -->
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
font-size: small;
}
</style>
</head>
<body>
<h1><?if ($lin=="it") echo "Sigle permesse per il confronto dei manoscritti"; else echo "Allowed codes for the comparison of manuscripts";?></h1>
<?if ($lin=="it"){?>
<p>Per confrontare due manoscritti, devi usare le seguenti sigle per ogni manoscritti:</p>
<ul>
<li>p + numero per i papiri (per esempio p46)</li>
<li>alef, una lettera maiuscola, Dabs, gamma, delta, theta, lambda, xi, pi, sigma, fi, psi, omega, o un numero che inizia con uno zero per gli onciali</li>
<li>f1 o f13 per le famiglie</li>
<li>un numero per i minuscoli, oppure Biz</li>
<li>l + numero per i lezionari (per esempio l1331), oppure Lez o lAD</li>
<li>it per il vecchio italiano, it + lettere (o beta, delta, lambda, mu, fi, pi, ro) per un manoscritto (per esempio ita, itar, itbeta)</li>
<li>la sigla di una versione, seguita dalle lettere in apice (per esempio sir, sirp, sirpal)</li>
<li>il nome di un padre</li>
<li>TR per il Textus receptus, WH, ECM, NA, UBS per le altre edizioni del NT</li>
<li>l'abbreviazione di una versione italiana (per esempio NR, Nv)</li>
</ul>
<p>&Egrave; anche possibile mettere <i>testo</i> per paragonare un manoscritto con il testo del NT utilizzato.</p>
<?}else{?>
<p>To compare two manuscripts, you need to use the following codes for each manuscript:</p>
<ul>
<li>p + number for the papyri (for example p46)</li>
<li>aleph, a capital letter, Dabs, gamma, delta, theta, lambda, xi, pi, sigma, phi, psi, omega, or a number that begins with zero for the uncials</li>
<li>f1 or f13 for the families</li>
<li>a number for the minuscules, or Byz</li>
<li>l + number for the lectionaries (for example l331), or Lect or lAD</li>
<li>it for the Old Italian, it + letters (or beta, delta, lambda, mu, phi, pi, rho) for a manuscript (for example ita, itar)</li>
<li>the code of an early version, followed by the letters in superscript (for example syr, syrp, syrpal)</li>
<li>the name of a father</li>
<li>TR for the Textus receptus, WH, ECM, NA, UBS for the other NT editions</li>
<li>the abbreviation of an Italian translation (for example NR, Nv)</li>
</ul>
<p>It is also possible to put <i>text</i> to compare a manuscript with the text of the NT that is used.</p>
<?}?>
</body>
</html>

