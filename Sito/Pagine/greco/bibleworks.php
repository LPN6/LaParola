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
<title><?if ($lin=="it") echo "La Sacra Bibbia - Bibleworks"; else echo "New Testament Manuscripts - Bibleworks";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Come inserire le pagine di questo sito in BibleWorks"; else echo "How to insert this site into BibleWorks";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "BibleWorks,Nuovo Testamento,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante"; else echo "BibleWorks,New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants";?>" />
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
<h1>How to insert this site into the <a href="http://www.bibleworks.com/">BibleWorks</a> program</h1>
<h2>Method 1: Linking to the site</h2>
<ol>
<li>Open BibleWorks.</li>
<li>Open the External Links Manager by pressing the ELM button <img src="elm_button.jpg" />.</li>
<li>Inside the External Links Manager (ELM), click on the "New" Button. This will add a new item to the list of links on the left and create a new name in the "menu entry text" and "Description" section on the left.<br /> <img src="elm.jpg" /></li>
<li>Then in the "Menu entry text" enter "Look up verse in LaParola.net apparatus". You can also enter the same thing in the "Description" area.</li>
<li>Now select "Verse Reference" in the "BibleWorks provides..." list box and choose "Bible Verse" in the "...with this type" option. This will tell BibleWorks that it needs to provide a Bible Verse when you click on the text of a New Testament verse in the Browse Window.</li>
<li>In the "Menu Location" choose "Browse Window Greek". This will cause a menu entry to be added to the Browse Window context menu when you have a Greek text like BNT or BGT.</li>
<li>This is a Web query so enter "NULL" in the parameters section. Choose BGT for the "Map verses using" list box.</li>
<li>All that remains is the entry in the "Web page, File to open, or executable to run" section. There you will enter:<br />

http://www.laparola.net/greco/index.php?varianti=s&amp;bk=&lt;book&gt;&amp;ch=&lt;chapter&gt;&amp;vs=&lt;verse&gt;<br />

The &lt;book&gt;, &lt;chapter&gt; and &lt;verse&gt; sections will be filled in by BibleWorks with the name of the New Testament book, the chapter and the verse numbers of the verse you are displaying in the Browse Window when you click on the menu entry.</li>
<li>Make sure that the "Enable this Link" check box has a check in it.<br /><img src="elm_laparola.jpg" /></li>
<li>Now click on the "OK" button. Now when you right click on a verse reference in the Browse Window there will be an option to look up the verse in the LaParola.net website.</li>
<h2>Method 2: Using a module</h2>
<p>Pasquale Amicarelli has created two modules with the manuscript data of this site: with <a href="http://www.webalice.it/pasgil/mantext.zip">the manuscripts in the usual order</a>, or <a href="http://www.webalice.it/pasgil/mantext2.zip">in the order of the text type</a>.<br />
There is also a <a href="/file/ManuscriptsBibleworks.zip">version in Italian/versione italiana</a>.</p>
</body>
</html>
