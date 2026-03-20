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
<title><?if ($lin=="it") echo "La Sacra Bibbia - Manoscritti del Nuovo Testamento - idee per il futuro"; else echo "New Testament Manuscripts - Ideas for the Future";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Idee future per il sviluppo del sito dei manoscritti del Nuovo Testamento"; else echo "Ideas for the future development of the site of the manuscripts of the New Testamento";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "Nuovo Testamento,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante"; else echo "New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants";?>" />
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
<?if ($lin=="it") {?>
<h1>Manoscritti del Nuovo Testamento - Idee per il futuro</h1>
<p>Alcune idee per sviluppare questo sito ulteriormente:</p>
<ul>
<li>ricercare parole e frasi nel testo greco</li>
<li>nell'ordine per il tipo di testo, mettere ogni manoscritto nel tipo che &egrave; in quel brano, invece del tipo che &egrave; di solito
(per esempio, A &egrave; sempre messo con i manoscritti alessandrini, ma &egrave; bizantino nei Vangeli).
Similmente, i correttori di un manoscritto hanno spesso una data diversa che andrebbe usata nell'ordine per data</li>
</ul>
<p>Se hai altri suggerimenti o idee, scrivimi a <i>info</i> a questo dominio.</p>

<p>Indietro al <a href="index.php">Nuovo Testamento greco</a>.</p>
<?}else{?>
<h1>New Testamento manuscripts - Ideas for the Futuro</h1>
<p>Some ideas to further develop this site:</p>
<ul>
<li>search words and phrases in the Greek text</li>
<li>in the order by text type, put each manuscript in the type that it is in that passage, rather than the type it usually is
(for example, A is always put with the Alexandrian manuscripts, but it is Byzantine in the Gospels).
Similarly, the correctors of a manuscripts often have a different date, that should be used in the ordering by date.</li>
</ul>
<p>If you have other suggestions or ideas, write to me at <i>info</i> at this domain.</p>

<p>Back to the <a href="index.php">Greek New Testament</a>.</p>
<?}?>
</body>
</html>
<?
/*
da fare - altre idee segrete

come index.html:
Visualizza: lingua orig, traslit, interlineare in 2 direzioni (simile a formato di rtf; con link al vocab?), mss : aiuto
Ricerca: lingua orig, traslit : aiuto ricerca, aiuto greek/ebr
Info sulle versioni

concetti/sinonomi/Louw
a ogni versetto mostra significati (e attributi)
Sign. in ordine alfabetico (e sinonomi)
sign. in ordine logico (cioè di Louw)

id_p id_v parola_no parola punt id_rad gram LN#
id_ln ln# sign_ing sign_it (attr_tipo attr_ln#)  [parte in () diverse volte x ogni parola; interpretazioni diverse? testi diversi?]
id_rad parola_greca id_voc def_ing Strong
*/
?>
