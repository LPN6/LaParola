<?
$fontuni = (isset($_REQUEST["greco_fontuni"])?$_REQUEST["greco_fontuni"]:"");
  $fontuni = str_replace("<", "", $fontuni); // affinché tag HTML non possono essere inseriti nella pagina
  $fontuni = str_replace(">", "", $fontuni);
$lin = (isset($_REQUEST["greco_lingua"])?$_REQUEST["greco_lingua"]:"");
  $lin = str_replace("<", "", $lin); // affinché tag HTML non possono essere inseriti nella pagina
  $lin = str_replace(">", "", $lin);
if ($lin=="")
   if (isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) $lin=substr($_SERVER['HTTP_ACCEPT_LANGUAGE'],0,2);

function titolo($l) {
if ($l=="it")
   $s = "<table border=\"1\"><tr><th>Nome</th><th>Data</th><th>Tipo</th><th>Contenuto</th><th>Commenti</th></tr>";
else
   $s = "<table border=\"1\"><tr><th>Name</th><th>Date</th><th>Type</th><th>Contents</th><th>Comments</th></tr>";
return $s;
}
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="<?if ($lin=="it") echo "it"; else echo "en";?>">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title><?if ($lin=="it") echo "La Sacra Bibbia - Manoscritti del Nuovo Testamento"; else echo "New Testament Manuscripts";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Tutti (quasi) i manoscritti del Nuovo Testamento, con data, tipo di testo, contenuto"; else echo "All (almost) the New Testament manuscripts, with date, text type and contents";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "data,tipo di testo,contenuto,alessandrino,bizantino,occidentale,cesareano,papiri,papiro,onciali,onciale,minuscoli,lezionari,diatessaron,padri,Nuovo Testamento,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante"; else echo "date,text type,contents,Alexandrian,Byzantine,Caesarean,Western,papyrus,papyri,uncials,uncial,minuscule,minuscules,lectionary,diatessaron,fathers,New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<!-- da fare da canc se capo.php e' inserito -->
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
}
</style>
</head>
<body>
<h1><?if ($lin=="it") echo "Manoscritti del Nuovo Testamento"; else echo "New Testament Manuscripts";?></h1>
<?if ($lin=="it") {?>
<h3>Note:</h3>
<p>La collana <b>Tipo</b> indica il tipo del testo dei manoscritti. Usa le seguenti lettere:<br />
A = alessandrino<br />
B = bizantino<br />
O = occidentale<br />
C = cesareano<br />
M = misto<br />
A volte anche manoscritti simili sono elencati.</p>
<p>Nella collana <b>Contenuto</b> le seguenti lettere sono usate:<br />
e: Vangeli<br />
a: Atti e epistole cattoliche nei minuscoli; Atti e epistole nei lezionari (che non includevano Apocalisse); Atti nel vecchio italiano<br />
p: epistole di Paolo (inclusa Ebrei)<br />
c: lettere cattoliche<br />
r: Apocalisse<br />
Un'eucologia era un libro liturgico, con solo alcune letture.</p>
<?}else{?>
<p>The <b>Type</b> column gives the text type of the manuscripts. It uses the following letters:<br />
A = Alexandrian<br />
B = Byzantine<br />
C = C&aelig;sarean<br />
W = Western<br />
M = Mixed<br />
Sometimes also similiar manuscripts are listed.</p>
<p>In the <b>Contents</b> column the following letters are used:<br />
e: Gospels<br />
a: Acts and the Catholic letters in the minuscules; Acts and the letters in the Lectionaries (that did not include Revelation); Acts in the Old Italian<br />
p: letters of Paul (including Hebrews)<br />
c: Catholic letters<br />
r: Revelation<br />
A eucologia was a liturgical book, with only some readings.</p>
<?}?>
<?
include("../conn.php");
if ($lin=="it")
   $sql = "SELECT Data_it, Tipo_it, Mss_nome_it, Commento_it, Contenuto_it ";
else
    $sql = "SELECT Data_ing, Tipo_ing, Mss_nome_ing, Commento_ing, Contenuto_ing ";
$sql .= "FROM Mss, MssTipo, MssData WHERE id_mssdata=id_data AND id_msstipo=id_tipo";
if ($lin!="it")
   $sql .= " ORDER BY Categoria,IF(Categoria=7,Mss_nome_ing,null),id_mss"; // sette sono i Padre, che vanno messo in ordine alfabetico in inglese
if ($ris=mysqli_query($conn, "$sql")) {
   $row=mysqli_fetch_array ($ris); // per non visualizzare la prima riga, che è il ms dummy
   while ($row=mysqli_fetch_array ($ris)) {
         if ($row[2]=="p<sup>1</sup>")
            echo '<h3>'.($lin=="it"?"Papiri":"Papyri").'</h3>'.titolo($lin);
         elseif ($row[2]=="<span class=\"uni\">&#8237;&#1488;</span>")
            echo '</table><h3>'.($lin=="it"?"Onciali":"Uncials").'</h3>'.titolo($lin);
         elseif ($row[2]=="f1")
            echo '</table><h3>'.($lin=="it"?"Minuscoli":"Minuscules").'</h3>'.titolo($lin);
         elseif ($row[2]=="l<sup>1</sup>")
            echo '</table><h3>'.($lin=="it"?"Lezionari":"Lectionaries").'</h3>'.titolo($lin);
         elseif ($row[2]=="it")
            echo '</table><h3>'.($lin=="it"?"Vecchio italiano":"Old Italian").'</h3>'.titolo($lin);
         elseif ($row[2]=="vg")
            echo '</table><h3>'.($lin=="it"?"Versioni":"Early Versions").'</h3>'.titolo($lin);
         elseif ($row[2]=="Diatessaron")
            echo '</table><h3>'.($lin=="it"?"Padri":"Fathers").'</h3>'.titolo($lin);
         elseif ($row[2]=="<span class=\"uni\">&#962;</span>")
            echo '</table><h3>'.($lin=="it"?"Edizioni del NT greco":"Greek NT Editions").'</h3>'.titolo($lin);
         elseif ($row[2]=="NR")
            echo '</table><h3>'.($lin=="it"?"Versioni italiane":"Italian Translations").'</h3>'.titolo($lin);
         echo "<tr><td>".$row[2]."</td><td>".$row[0]."</td><td>".$row[1]."</td><td>".$row[4]."</td><td>".$row[3]."</td></tr>";
   }
}
?>
</table>
<?if ($lin=="it") {?>
<p>Indietro al <a href="index.php">Nuovo Testamento greco</a>.</p>
<?}else{?>
<p>Back to the <a href="index.php">Greek New Testament</a>.</p>
<?}?>
</body>
</html>

