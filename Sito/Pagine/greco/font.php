<?
header("Content-type: text/html; charset=utf-8");
$fontuni = (isset($_REQUEST["greco_fontuni"])?$_REQUEST["greco_fontuni"]:"");
  $fontuni = str_replace("<", "", $fontuni); // affinch� tag HTML non possono essere inseriti nella pagina
  $fontuni = str_replace(">", "", $fontuni);
$lin = (isset($_REQUEST["greco_lingua"])?$_REQUEST["greco_lingua"]:"");
  $lin = str_replace("<", "", $lin); // affinch� tag HTML non possono essere inseriti nella pagina
  $lin = str_replace(">", "", $lin);
if ($lin=="")
   if (isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) $lin=substr($_SERVER['HTTP_ACCEPT_LANGUAGE'],0,2);
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<?if ($lin=="it") echo "it"; else echo "en";?>">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title><?if ($lin=="it") echo "La Sacra Bibbia - Manoscritti del Nuovo Testamento - Font"; else echo "New Testament Manuscripts - Fonts";?></title>
<meta name="description" content="<?if ($lin=="it") echo "I font da usare per visualizzare le letture varianti dei manoscritti del Nuovo Testamento, per la critica testuale"; else echo "The font to use to see the variant readings of the manuscripts of the New Testamento, for textual criticism";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "font,unicode,Nuovo Testamento,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante"; else echo "font,unicode,New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<!-- da fare da canc se capo.php � inserito -->
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
font-size: small;
}
</style>
</head>
<body>
<?if ($lin=="it") {?>
<h1>Come visualizzare i caratteri greci</h1>
<p>Il testo greco &egrave; visualizzato con Unicode.
Alcune versioni vecchie dei browser non possono visualizzare tale testo, quindi &egrave; necessario prima di tutto
aggiornare il browser se non &egrave; la versione pi&ugrave; recente.</p>
<p>Poi &egrave; necessario aver installato sul tuo computer uno di 11 diversi font unicode con lettere greche che
questo sito riconosce. Questi 11 font sono elencati in fondo a questa pagina, con un campione del font. Se il testo non
&egrave; visualizzato correttamente, quel font non &egrave; installato sul tuo computer.</p>
<p>Windows 2000 e dopo contengono <i>Palatino Linotype</i>; Windows XP e dopo contengono anche il font <i>Tahoma</i>.
Gli altri font possono essere scaricati facendo clic sul nome del font.</p>
<p>Per installare uno di questi font in Windows, copialo alla cartella dei font (di solito c:\windows\fonts\).
A volte basta cos&igrave;;
se non funziona ancora riavvia il computer, in modo che il font sia riconosciuto automaticamente da Windows.</p>
<p>Questo sito prova prima di tutto ad usare il primo font elencato. Se non &egrave; installato, usa il secondo, e
cos&igrave; via fino all'ultimo font. Questa &egrave; la scelta predefinta di font. Puoi modificare questa
scelta nelle opzioni per visualizzare il testo.</p>
<p>Indietro al <a href="index.php">Nuovo Testamento greco</a>.</p>
<?}else{?>
<h1>How to see the Greek letters</h1>
<p>The Greek text is created with Unicode.
Some older versions of browsers can not view such text, and so it is necessary first of all to update your browser if it
is not the most recent version.</p>
<p>Then it is necessary to have installed on your computer one of 11 different Unicode fonts with Greek letters that
this site recognises. These 11 fonts are listed at the end of the page, along with a sample of the font. If the text is not
visualised correctly, then that font is not installed on your computer.</p>
<p>Windows 2000 and later contain <i>Palatino Linotype</i>; Windows XP and later also contain the font <i>Tahoma</i>.
The other fonts can be downloaded clicking on the name of the font.</p>
<p>To install one of these font in Windows, copy it to the font directory (usually c:\windows\fonts\). Sometimes this
is all that is needed. If it still doesn't work, reboot the computer, so that the font is automatically recognised by Windows.</p>
<p>This site tries first of all to use the first font listed. If it is not installed, it uses the second, and so on
down to the last. This is the default choice of font. You can override this choice in the options for viewing the text.</p>
<p>Back to the <a href="index.php">Greek New Testament</a>.</p>
<?}?>
<table>
<tr><td><a href="http://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&item_id=SILgrkuni">Galatia SIL</a></td><td><span class="unigs">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&item_id=Gentium">Gentium</a></td><td><span class="unig">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://scholarsfonts.net/cardofnt.html">Cardo</a></td><td><span class="unic">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<!--<tr><td><a href="http://semata.delendis.com/fuentes.html">Oxoniensis</a></td><td><span class="unio">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>-->
<tr><td><a href="http://www.users.dircon.co.uk/~hancock/vudown.htm">Vusillus Old Face</a></td><td><span class="univof">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://greekbible.com/">Athena</a></td><td><span class="unia">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://bibliofile.mc.duke.edu/gww/fonts/Caslon/Caslon.html">Caslon</a></td><td><span class="unic2">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://dartcanada.tripod.com/Objets/Old/hh/hindhist.html">Hindsight Unicode</a></td><td><span class="unihu">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://everywitchway.net/linguistics/fonts/chrysuni.html">Chrysanthi Unicode</a></td><td><span class="unicu">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://bibliofile.mc.duke.edu/gww/fonts/Monospace/index.html">Monospace</a></td><td><span class="unim">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://www.thessalonica.org.ru/en/fonts.html">OldStandard</a></td><td><span class="unios">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td>Palatino Linotype</td><td><span class="unipl">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td>Tahoma</td><td><span class="unit">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<!--
<tr><td><a href="http://socrates.berkeley.edu/~pinax/greekkeys/NAUdownload.html">New Athena</a></td><td><span class="unina">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://home.kabelfoon.nl/~slam/fonts/fonts.html">Garogier</a></td><td><span class="unig2">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://home.kabelfoon.nl/~slam/fonts/fonts.html">Legendum</a></td><td><span class="unil">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://faculty.bbc.edu/rdecker/galileeunicode.htm">Galilee Unicode</a></td><td><span class="unigu">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://www.io.com/~hmiller/lang/">Thryomanes</a></td><td><span class="unit2">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
<tr><td><a href="http://www.geocities.com/greekfonts/">Porson</a></td><td><span class="unip">&#945;&#946;&#947;&#948; &#913;&#914;&#915;&#916; &#7936;&#7937;&#7938;&#7939;&#8115;&#8118;&#8119; &#7944;&#7945; &#903;,.;<br />βίβλος γενέσεως Ἰησοῦ Χριστοῦ υἱοῦ Δαυὶδ υἱοῦ Ἀβραάμ</span></td></tr>
-->
</table>
</body>
</html>

