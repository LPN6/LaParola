<?
require("funzioni.php");

$RIF_NON_ESISTE = "Non ci sono versetti che hanno questo riferimento.";
$formato_rif_trovato = "";

$libri_nomi = array("","Genesi","Esodo","Levitico","Numeri","Deuteronomio","Giosu&egrave;","Giudici","Rut","1Samuele","2Samuele","1Re","2Re","1Cronache","2Cronache","Esdra","Neemia","Tobia","Giuditta","Ester","1Maccabei","2Maccabei","Giobbe","Salmi","Proverbi","Ecclesiaste","Cantico","Sapienza","Siracide","Isaia","Geremia","Lamentazioni","Baruc","Ezechiele","Daniele","Osea","Gioele","Amos","Abdia","Giona","Michea","Naum","Abacuc","Sofonia","Aggeo","Zaccaria","Malachia","Matteo","Marco","Luca","Giovanni","Atti","Romani","1Corinzi","2Corinzi","Galati","Efesini","Filippesi","Colossesi","1Tessalonicesi","2Tessalonicesi","1Timoteo","2Timoteo","Tito","Filemone","Ebrei","Giacomo","1Pietro","2Pietro","1Giovanni","2Giovanni","3Giovanni","Giuda","Apocalisse");
$libri_eng = array("","Genesis","Exodus","Leviticus","Numbers","Deuteronomy","Joshua","Judges","Ruth","1Samuel","2Samuel","1Kings","2Kings","1Chronicles","2Chronicles","Ezra","Nehemiah","Tobit","Judith","Esther","1Maccabees","2Maccabees","Job","Psalms","Proverbs","Ecclesiastes","Song of Solomon","Wisdom","Sirach","Isaiah","Jeremiah","Lamentations","Baruch","Ezekiel","Daniel","Hosea","Joel","Amos","Obadiah","Jonah","Micah","Nahum","Habakkuk","Zephaniah","Haggai","Zechariah","Malachi","Matthew","Mark","Luke","John","Acts","Romans","1Corinthians","2Corinthians","Galatians","Ephesians","Philippians","Colossians","1Thessalonians","2Thessalonians","1Timothy","2Timothy","Titus","Philemon","Hebrews","James","1Peter","2Peter","1John","2John","3John","Jude","Revelation");
$libri_es = array("","Genesis","Exodus","Leviticus","Numbers","Deuteronomy","Joshua","Judges","Ruth","1Samuel","2Samuel","1Kings","2Kings","1Chronicles","2Chronicles","Ezra","Nehemiah","Tobit","Judith","Esther","1Maccabees","2Maccabees","Job","Psalms","Proverbs","Ecclesiastes","Song of Solomon","Wisdom","Sirach","Isaiah","Jeremiah","Lamentations","Baruch","Ezekiel","Daniel","Hosea","Joel","Amos","Obadiah","Jonah","Micah","Nahum","Habakkuk","Zephaniah","Haggai","Zechariah","Malachi","Mateo","Marcos","Lucas","Juan","Hechos","Romanos","1Corintios","2Corintios","Galatias","Efesios","Filipenses","Colosenses","1Tesalonicenses","2Tesalonicenses","1Timoteo","2Timoteo","Tito","Filem&oacute;n","Hebreos","Santiago","1Pedro","2Pedro","1Juan","2Juan","3Juan","Judas","Apocalipsis");
$libri_abb = array("","Gen","Eso","Le","Nu","De","Gios","Giudic","Ru","1Sam","2Sam","1Re","2Re","1Cr","2Cr","Esd","Ne","Tob","Giudit","Est","1Macc","2Macc","Giob","Sal","Prov","Ec","CC","Sap","Sir","Is","Ger","Lam","Bar","Ez","Da","Os","Gioe","Am","Abd","Gion","Mi","Na","Abac","So","Ag","Zac","Mal","Mat","Mar","Lu","Giov","At","Ro","1Co","2Co","Ga","Ef","Fili","Col","1Te","2Te","1Ti","2Ti","Tit","File","Eb","Giac","1P","2P","1G","2G","3G","Giuda","Ap");
$libri_abbr = array("ge"=>1,"gn"=>1,"eo"=>2,"es"=>2,"le"=>3,"lv"=>3,"nm"=>4,"nu"=>4,"de"=>5,"dt"=>5,"gios"=>6,"gs"=>6,"gc"=>7,"gdc"=>7,"giudic"=>7,"rt"=>8,"ru"=>8,"1s"=>9,"isam"=>9,"2s"=>10,"iis"=>10,"1r"=>11,"ir"=>11,"2r"=>12,"iir"=>12,"1cr"=>13,"icr"=>13,"2cr"=>14,"iicr"=>14,"ed"=>15,"esd"=>15,"ne"=>16,"tb"=>17,"to"=>17,"giudit"=>18,"est"=>19,"et"=>19,"1m"=>20,"im"=>20,"2m"=>21,"iim"=>21,"gb"=>22,"giob"=>22,"sal"=>23,"sl"=>23,"p"=>24,"ec"=>25,"q"=>25,"ca"=>26,"cc"=>26,"ct"=>26,"sap"=>27,"si"=>28,"is"=>29,"ger"=>30,"gr"=>30,"la"=>31,"b"=>32,"ez"=>33,"da"=>34,"dn"=>34,"o"=>35,"gioe"=>36,"gl"=>36,"am"=>37,"abd"=>38,"ad"=>38,"gion"=>39,"mi"=>40,"na"=>41,"aba"=>42,"ac"=>42,"h"=>42,"so"=>43,"ag"=>44,"z"=>45,"mal"=>46,"ml"=>46,"mat"=>47,"mt"=>47,"mar"=>48,"mc"=>48,"mr"=>48,"lc"=>49,"lu"=>49,"giov"=>50,"gv"=>50,"at"=>51,"rm"=>52,"ro"=>52,"1co"=>53,"ico"=>53,"2co"=>54,"iico"=>54,"ga"=>55,"ef"=>56,"fili"=>57,"fl"=>57,"cl"=>58,"co"=>58,"1te"=>59,"1ts"=>59,"ite"=>59,"its"=>59,"2te"=>60,"2ts"=>60,"iite"=>60,"iits"=>60,"1ti"=>61,"1tm"=>61,"iti"=>61,"itm"=>61,"2ti"=>62,"2tm"=>62,"iiti"=>62,"iitm"=>62,"ti"=>63,"tt"=>63,"file"=>64,"fm"=>64,"eb"=>65,"gia"=>66,"gm"=>66,"1p"=>67,"ip"=>67,"2p"=>68,"iip"=>68,"1g"=>69,"ig"=>69,"2g"=>70,"iig"=>70,"3g"=>71,"iiig"=>71,"gd"=>72,"giuda"=>72,"ap"=>73);
$libri_abbr_eng = array("ge"=>1,"gn"=>1,"eo"=>2,"ex"=>2,"le"=>3,"lv"=>3,"nm"=>4,"nu"=>4,"de"=>5,"dt"=>5,"jos"=>6,"js"=>6,"judg"=>7,"jg"=>7,"rt"=>8,"ru"=>8,"1s"=>9,"is"=>9,"2s"=>10,"iis"=>10,"1k"=>11,"ik"=>11,"2k"=>12,"iik"=>12,"1ch"=>13,"ich"=>13,"2ch"=>14,"iich"=>14,"ez"=>15,"ne"=>16,"est"=>19,"et"=>19,"jb"=>22,"job"=>22,"ps"=>23,"pr"=>24,"pv"=>24,"ec"=>25,"q"=>25,"ss"=>26,"sg"=>26,"so"=>26,"is"=>29,"jer"=>30,"jr"=>30,"la"=>31,"ez"=>33,"da"=>34,"dn"=>34,"ho"=>35,"joe"=>36,"jl"=>36,"am"=>37,"ob"=>38,"jon"=>39,"mi"=>40,"na"=>41,"ha"=>42,"hb"=>42,"zep"=>43,"zp"=>43,"hag"=>44,"hg"=>44,"zec"=>45,"zc"=>45,"mal"=>46,"ml"=>46,"mat"=>47,"mt"=>47,"mar"=>48,"mk"=>48,"mr"=>48,"lk"=>49,"lu"=>49,"joh"=>50,"jn"=>50,"ac"=>51,"rm"=>52,"ro"=>52,"1co"=>53,"ico"=>53,"2co"=>54,"iico"=>54,"ga"=>55,"ep"=>56,"phili"=>57,"cl"=>58,"co"=>58,"1th"=>59,"1ts"=>59,"ith"=>59,"its"=>59,"2th"=>60,"2ts"=>60,"iith"=>60,"iits"=>60,"1ti"=>61,"1tm"=>61,"iti"=>61,"itm"=>61,"2ti"=>62,"2tm"=>62,"iiti"=>62,"iitm"=>62,"ti"=>63,"tt"=>63,"phile"=>64,"phm"=>64,"phlm"=>64,"heb"=>65,"ja"=>66,"jm"=>66,"1p"=>67,"ip"=>67,"2p"=>68,"iip"=>68,"1j"=>69,"ij"=>69,"2j"=>70,"iij"=>70,"3j"=>71,"iiij"=>71,"jude"=>72,"jd"=>72,"re"=>73,"rv"=>73);
// nota non c'è l'apocrifa in inglese

//if (!isset($non_includere_quot))
//  include("quot.php");

function val3as($x) {
  return 256 * 256 * ord($x[0]) + 256 * ord($x[1]) + ord($x[2]);
}

function analizza_versione($ver) {
// Restituisce una string, il primo carattere è il tipo della versione (v=versione o c=commentario), gli altri il numero della versione nel db
global $conn;
  $sql = "SELECT id_t,tipo FROM Versioni WHERE nome=\"$ver\"";
  $risultato = "v0";
  if ($ris = mysqli_query($conn, "$sql")) {
    if (mysqli_num_rows($ris))
      $risultato = mysqli_result_lpn($ris, 0, "tipo").mysqli_result_lpn($ris, 0, "id_t");
  }
  else
    errore2("interrogazione database per versioni");
  return $risultato;
}

function converti_rif($a, $lin="it") {
global $libri_abb;
global $libri_abbr;
global $libri_abbr_eng;
global $formato_rif_trovato;
global $utf8;
    $utf8 = (isset($utf8)?$utf8:0);
    if ($utf8==0 && mb_detect_encoding($a, 'UTF-8, ISO-8859-1')=='UTF-8')
        $a = utf8_decode($a);

$libri_usati = array();
if ($lin=="en")
	$libri_usati = $libri_abbr_eng;
else
	$libri_usati = $libri_abbr;

$formato_rif_trovato="";
$RifLungo = strtolower(trim($a));
$RifLungo = str_replace(" ", "", $RifLungo);
if (strlen($RifLungo)==0)
  return "";

for ($i=strlen($RifLungo)-1; $i>=1; $i--)
  if ((/*$RifLungo[$i]=="," ||*/ $RifLungo[$i]==".") && (Lettera($RifLungo[$i-1])))
    $RifLungo = substr($RifLungo,0,$i).substr($RifLungo,$i+1);

while (strlen($RifLungo)>0 && (!Lettera($RifLungo[0]) && $RifLungo[0]<"0" && $RifLungo[0]>"9"))
  $RifLungo = substr($RifLungo,1);
if (strlen($RifLungo)==0)
  return "";

$posdp = strpos($RifLungo, ":");
if ($posdp==0)
  $posdp=strlen($RifLungo);
else if ($RifLungo[$posdp-1]<"0" || $RifLungo[$posdp-1]>"9") // solo se preceduto da un numero è un vero divisore
	$posdp=strlen($RifLungo);
$posvir = strpos($RifLungo, ",");
if ($posvir==0)
  $posvir=strlen($RifLungo);
else if ($RifLungo[$posvir-1]<"0" || $RifLungo[$posvir-1]>"9") // solo se preceduto da un numero è un vero divisore
	$posvir=strlen($RifLungo);
$TipoRif = ($posvir<$posdp?2:1); // 2 per 2P 3,4.7; 1 per 2P 3:4,7
if ($TipoRif==1 && $posvir>$posdp) // in casi come 2P3, che può essere tutti e due i tipi, non diamo valore al formato trovato
  $formato_rif_trovato = "dv";
if ($TipoRif==2)
  $formato_rif_trovato = "vp";
if ($TipoRif==2 && $posdp==strlen($RifLungo)) { // per casi come File 1,4
// problema ancora con riferimenti come File 1,4, 2g 3,5, ma sono molto rari
  $LettereStato = 0;
  $TestoIniz = "";
  for ($i=0; $i<strlen($RifLungo); $i++) {
    switch ($LettereStato) {
    case 0:
      if (Lettera($RifLungo[$i]) || ($i==0 && $RifLungo[$i]>="1" && $RifLungo[$i]<="3"))
        $TestoIniz .= $RifLungo[$i];
      else
        $LettereStato = 1;
      break;
    case 1:
      if (Lettera($RifLungo[$i]))
        $LettereStato = 2;
      break;
    }
  }
  if ($LettereStato==1) {
    $iLibro=0;
    for ($i=strlen($TestoIniz); $i>=1 && $iLibro==0; $i--) {
      foreach ($libri_usati as $key=>$val)
      {
        if ($key==substr($TestoIniz,0,$i)) {
          $iLibro = $val;
          break;
        }
      }
      //reset ($libri_usati);
      //while (list ($key, $val) = each ($libri_usati)) {
      //  if ($key==substr($TestoIniz,0,$i)) {
      //    $iLibro = $val;
      //    break;
      //  }
      //}
    }
    if ($iLibro==72 || $iLibro==71 ||$iLibro==70 || $iLibro==64 || $iLibro==38)
      $TipoRif = 1;
  }
}
if ($TipoRif==2 && strpos($RifLungo,":")==0) {
  while (strpos($RifLungo,",")>0) {
    $p = strpos($RifLungo, ",");
    $RifLungo=substr($RifLungo,0,$p).":".substr($RifLungo,$p+1);
  }
  while (strpos($RifLungo,".")>0) {
    $p = strpos($RifLungo, ".");
    $RifLungo=substr($RifLungo,0,$p).",".substr($RifLungo,$p+1);
  }
  while (strpos($RifLungo,";")>0) {
    $bpos = strpos($RifLungo,";")+1; // controlla Is 7,1-10;12 che viene tradotto in modo diverso
    while ($bpos<strlen($RifLungo) && (($RifLungo[$bpos]>="0" && $RifLungo[$bpos]<="9") || $RifLungo[$bpos]==" "))
      $bpos++;
    if ($bpos>=strlen($RifLungo) || ($RifLungo[$bpos]!=":" && $RifLungo[$bpos]!="." && (!Lettera($RifLungo[$bpos])) && $RifLungo[$bpos]<="~"))
      $RifLungo=substr($RifLungo,0,$bpos).":1-177".substr($RifLungo,$bpos);
    $RifLungo=substr($RifLungo,0,strpos($RifLungo,";")).",".substr($RifLungo,strpos($RifLungo,";")+1);
  }
}

$sTemp=0; $Capitolo=0; $TrattinoVecchio=0;
$bTrattino=0; $noverse=0;
$r="";
$RiferimentoBrano="";
$sLibro="";
$RiferimentoBranoPrecedente="";
$RiferimentoOut="";
do {
  $sTemp = strpos($RifLungo,",");
  if ($sTemp==0 || (strpos($RifLungo,";") < $sTemp && strpos($RifLungo,";") > 0))
    $sTemp = strpos($RifLungo,";");
  if ($sTemp==0 || (strpos($RifLungo,"-") < $sTemp && strpos($RifLungo,"-") > 0)) {
    $sTemp = strpos($RifLungo,"-");
    if ($sTemp > 0)
      $bTrattino = 1;
  }
  if ($sTemp == 0)
    $sTemp = strlen($RifLungo);
  $r = substr($RifLungo,0,$sTemp);
  $RifLungo = trim(substr($RifLungo,$sTemp+1));
  $RiferimentoBrano = converti_versetto($r, 1-$TrattinoVecchio, $TipoRif, $lin);
  if (strlen($RiferimentoBrano)==0 && strlen($r)>0 && !Lettera($r[0])) {
    if ((strpos($r,":") + strpos($r,".") == 0) && $noverse==0)
      $r = $Capitolo . ":" . $r;
    $r = $sLibro.$r;
    $RiferimentoBrano = converti_versetto($r, 1-$TrattinoVecchio, $TipoRif, $lin);
  }
  $noverse = 0;

  if (strlen($RiferimentoBrano)>0) {
    if (strpos($r,":") + strpos($r,".") == 0) {
      $noverse = 1;
      if ($bTrattino) {
        if (!Lettera($RifLungo[0]) &&  (strlen($RifLungo)==1 || !Lettera($RifLungo[1])))
          $RifLungo = $libri_abb[ord($RiferimentoBrano)] . $RifLungo;
      }
      else {
        if ($TrattinoVecchio==0) {
          $bTrattino = 1;
          $RifLungo = $r . ";" . $RifLungo;
        }
      }
    }
    $sLibro = $libri_abb[ord($RiferimentoBrano)];
    $Capitolo = ord(substr($RiferimentoBrano,1));
  }
  if ($TrattinoVecchio) {
    $RiferimentoBrano = $RiferimentoBranoPrecedente . $RiferimentoBrano;
    $TrattinoVecchio = 0;
  }
  else {
    if ($bTrattino) {
      $TrattinoVecchio = 1;
      $RiferimentoBranoPrecedente = $RiferimentoBrano;
      $bTrattino = 0;
    }
    else
      $RiferimentoBrano .= $RiferimentoBrano;
  }
  if (strlen($RiferimentoBrano) == 6) {
    if (val3AS(substr($RiferimentoBrano,0,3)) <= val3AS(substr($RiferimentoBrano,3,3)))
      $RiferimentoOut .= $RiferimentoBrano;
  }
} while (strlen($RifLungo)>0);

return $RiferimentoOut;
}

function converti_versetto($r, $flag, $TipoRif, $lin="it") {
// convertire a 3 byte un riferimento di un versetto
// $TipoRif: 2 per 2P 3,4.7; 1 per 2P 3:4,7
global $libri_abbr;
global $libri_abbr_eng;

$libri_usati = array();
if ($lin=="en")
	$libri_usati = $libri_abbr_eng;
else
	$libri_usati = $libri_abbr;

$i = 0;
$r2 = strtolower(trim($r));

$b = "";
if ($r2[0]>="1" && $r2[0]<="3") {
  $b = substr($r2,0,1);
  $r2 = trim(substr($r2,1));
}
do
  $i++;
while ($i<strlen($r2) && Lettera($r2[$i]));

$RifRimanente="";
$iCapitolo=0;
$iVersetto=0;
if ($i==strlen($r2) && Lettera($r2[0]))
  $b .= $r2;
else {
  $b .= substr($r2,0,$i);
  $RifRimanente = trim(substr($r2,$i));
  $sCapitoloNumerico="";
  for ($j=0; $j<strlen($RifRimanente) && $RifRimanente[$j]>="0" && $RifRimanente[$j]<="9"; $j++)
    $sCapitoloNumerico .= $RifRimanente[$j];
  if (strlen($sCapitoloNumerico)>0)
    $iCapitolo = $sCapitoloNumerico + 0;
}

if (strlen($RifRimanente)>0) {
  $t = strpos($RifRimanente,":");
  if ($t==0 || (strpos($RifRimanente,".")<$t && strpos($RifRimanente,".")>0))
    $t = strpos($RifRimanente,".");
  if (($TipoRif==2) && ($t==0 || (strpos($RifRimanente,",")<$t && strpos($RifRimanente,",")>0)))
    $t = strpos($RifRimanente,",");
  if ($t==0)
    $t = strlen($RifRimanente)-1;
  $RifRimanente = trim(substr($RifRimanente,$t+1));
  $sVersettoNumerico="";
  for ($j=0; $j<strlen($RifRimanente) && $RifRimanente[$j]>="0" && $RifRimanente[$j]<="9"; $j++)
    $sVersettoNumerico .= $RifRimanente[$j];
  $iVersetto=((strlen($sVersettoNumerico)==0)?0:$sVersettoNumerico+0);
}

$s="";
$iLibro=0;
for ($i=strlen($b);$i>=1 && $iLibro==0; $i--) {
//  reset ($libri_usati);
//  while (list ($key, $val) = each ($libri_usati)) {
//    if ($key==substr($b,0,$i)) {
//      $iLibro = $val;
//      break;
//    }
//  }
  foreach ($libri_usati as $key=>$val) {
    if ($key==substr($b,0,$i)) {
      $iLibro = $val;
      break;
    }
  }
}
if ($iLibro>0) {
  $s = chr($iLibro);
  if (($iLibro==72 || $iLibro==71 ||$iLibro==70 || $iLibro==64 || $iLibro==38) && $iVersetto==0) {
    $iVersetto = $iCapitolo;
    $iCapitolo = 1;
  }
}

if (strlen($s)>0) {
  if ($iCapitolo == 0) {
    if ($flag==1)
      $s .= chr(1).chr(1);
    else {
      if ($flag == 0) {
        $iCapitolo = 151;
        $s .= chr($iCapitolo).chr(177);
      }
    }
  } // if ($iCapitolo==0)
  else {
    $s .= chr($iCapitolo);
    $t = 177;
    if ($iVersetto > $t)
      $iVersetto = $t;
    if ($iVersetto == 0) {
      if ($flag == 1)
        $iVersetto = 1;
      else {
        if ($flag == 0)
          $iVersetto = $t;
      }
    }
    $s .= chr($iVersetto);
  } // else
} // if (strlen($s)>0)
return $s;
}

function converti_rif3($lib1, $cap1, $vers1, $lib2, $cap2, $vers2, $ling="it", $formato_rif="dv") {
// convertire al formato testuale un riferimento a 3 byte
if ($ling=="it") {
   global $libri_nomi;
   $libro = $libri_nomi;
}
else {
   global $libri_eng;
   $libro = $libri_eng;
}

$sep = ($formato_rif=="vp"?",":":");
$v1testo = ($vers2==177?"":$sep.$vers1);
$v2testo = ($vers2==177?"":$sep.$vers2);
$c1testo = ($cap2==151?"":"$cap1");
$c2testo = ($cap2==151?"":"$cap2");
if ($lib1==72 || $lib1==71 ||$lib1==70 || $lib1==64 || $lib1==38) {
  $c1testo="";
  if (strlen($v1testo)>0)
    $v1testo = substr($v1testo,1);
}
$rif = "$libro[$lib1] $c1testo$v1testo";
if ($lib2==72 || $lib2==71 ||$lib2==70 || $lib2==64 || $lib2==38) {
  $c2testo="";
  if (strlen($v2testo)>0)
    $v2testo = substr($v2testo,1);
}
if ($lib1!=$lib2)
  $rif .= "-$libri_nomi[$lib2] $c2testo$v2testo";
elseif ($cap1!=$cap2 && $cap2!=151)
  $rif .= "-$cap2$v2testo";
elseif ($vers1!=$vers2 && $vers2!=177)
  $rif .= "-$vers2";
return $rif;
}

function converti_riferimento_brano($riferimento, $ling="it", $formato_rif="dv") {
  $riferimento_leggibile = "";
  for ($j=0; $j<strlen($riferimento)/6; $j++) {
    $k = 6*$j;
    if ($j>0)
      $riferimento_leggibile .= "; ";
    $riferimento_leggibile .= converti_rif3(ord($riferimento[$k]), ord($riferimento[$k+1]), ord($riferimento[$k+2]), ord($riferimento[$k+3]), ord($riferimento[$k+4]), ord($riferimento[$k+5]),$ling, $formato_rif);
  }
  return $riferimento_leggibile;
}

function visualizza_brano($versione, $vtipo, $l1, $c1, $v1, $l2, $c2, $v2, $formato_rif="dv") {
  echo get_brano($versione, $vtipo, $l1, $c1, $v1, $l2, $c2, $v2, $formato_rif);
}

function get_brano($versione, $vtipo, $l1, $c1, $v1, $l2, $c2, $v2, $formato_rif="dv", $xml=0, $titoli="s") {
global $libri_nomi, $libri_abb, $libri_eng;
global $conn;
global $RIF_NON_ESISTE;

$testo = "";
$sql = "SELECT Libro,Capitolo,Versetto,Testo FROM Versetti WHERE id_t=$versione AND ";
if ($l1==$l2) {
  $sql .= "Libro=$l1 AND ";
  if ($c1==$c2)
    $sql .= "Capitolo=$c1 AND Versetto>=$v1 AND Versetto<=$v2";
  else
    $sql .= "((Capitolo=$c1 AND Versetto>=$v1) OR (Capitolo>$c1 AND Capitolo<$c2) OR (Capitolo=$c2 AND Versetto<=$v2))";
}
else {
  $sql .= "(Libro=$l1 AND ((Capitolo=$c1 AND Versetto>=$v1) OR Capitolo>$c1) OR (Libro>$l1 AND Libro <$l2) OR (Libro=$l2 AND ((Capitolo<$c2 OR (Capitolo=$c2 AND Versetto<=$v2)))))";
}
$sql .= " ORDER BY Libro ASC,Capitolo ASC,Versetto ASC";
//echo $sql;
$brano = "";
$rif = "";
if ($ris=mysqli_query($conn, "$sql")) {
  $PrimaRiga = 1;
  $num_rows_sql2 = 0;
  $versetto_precedente_trovato = 0;
  if (mysqli_num_rows($ris)==0 && $vtipo=='c') {
    $sql = "SELECT Libro,Capitolo,Versetto,Testo FROM Versetti WHERE id_t=$versione AND Libro=$l1 AND Capitolo=$c1 AND Versetto<$v1 ORDER BY Versetto ASC";
    $ris=mysqli_query($conn, "$sql");
    if (mysqli_num_rows($ris)>0)
      mysqli_data_seek($ris, mysqli_num_rows($ris)-1);
    $versetto_precedente_trovato = 1;
  }
  $libro_precedente = -1;
  $capitolo_precendente = -1;
  $primo_versetto = 1;
  $numero_sezioni = 0;
  while ($row=mysqli_fetch_array ($ris)) {
    $testo_versetto = $row["Testo"];
    $rif_versetto = $row["Versetto"];
    if ($PrimaRiga==1 && $vtipo=='c' && $versetto_precedente_trovato==0 && ($row["Libro"]!=$l1 || $row["Capitolo"]!=$c1 || $row["Versetto"]!=$v1)) {
       // in un commentario, dove il commento è per un brano, trova l'ultimo commento prima del primo versetto richiesto se non ha un commento
       $sql = "SELECT Libro,Capitolo,Versetto,Testo FROM Versetti WHERE id_t=$versione AND Libro=$l1 AND Capitolo=$c1 AND Versetto<$v1 ORDER BY Versetto DESC";
       $ris2=mysqli_query($conn, "$sql");
       $row2=mysqli_fetch_array($ris2);
       $testo_versetto = $row2["Testo"];
       $num_rows_sql2 = 1;
       $rif_versetto = $row2["Versetto"];
       mysqli_data_seek($ris, 0);
    }
    $titolo_versetto = "";
    $titolo_versetto0 = "";
	$testo_versetto_inizio = "";
    $pos = strrpos ($testo_versetto, "^");
    $testo_versetto = str_replace("§", "@", $testo_versetto);
    if (!$pos === false) {
      $titolo_versetto = substr($testo_versetto, 0, $pos);
      $testo_versetto = substr($testo_versetto, $pos+1);
	  $pos = strrpos($titolo_versetto, "@");
	  if (!$pos === false) {
	      $testo_versetto_inizio = substr($titolo_versetto, 0, $pos);
    	  $titolo_versetto = substr($titolo_versetto, $pos+1);
          $pos = strrpos ($testo_versetto_inizio, "^");
          if (!$pos === false) { // alcuni casi in Riv2020 con 2 titoli in un versetto, per esempio CC 5:1
            $titolo_versetto0 = substr($testo_versetto_inizio, 0, $pos);
            $testo_versetto_inizio = substr($testo_versetto_inizio, $pos+1);
          }
	  }
    }
    if ($titoli=="n") {
        $titolo_versetto = "";
        $titolo_versetto0 = "";
    }
    
    $pos = strpos ($testo_versetto, "_______________________________________________________");
    if (!$pos === false) {
        $testo_versetto = substr($testo_versetto,0,$pos).substr($testo_versetto,$pos+50);
        // altrimenti riga troppo lunga su cellulari
    }
    
    if ($xml==0) {
      if (strlen($titolo_versetto) > 0) {
        if ($row["Versetto"]!=1) {
          $titolo_versetto = "</p><p>".$titolo_versetto;
          if (strlen($titolo_versetto0) > 0)
            $titolo_versetto0 = "</p><p>".$titolo_versetto0;
        }
        else {
          if (strlen($titolo_versetto0) > 0)
            $titolo_versetto = "</p><p>".$titolo_versetto;
        }
        $titolo_versetto .= "<br />";
        if (strlen($titolo_versetto0) > 0)
            $titolo_versetto0 .= "<br />";
      }
      if (strlen($brano)>0)
        $brano .= " ";

      if (mysqli_num_rows($ris) + $num_rows_sql2>1) {
        if ((($rif_versetto==1 && $brano!="") && ($l1!=$l2 || $c1!=$c2) || ($vtipo=='c')))
          $brano .= "</p><p>";
        if ($formato_rif != "nn") {
//          $brano .= $titolo_versetto."<strong>";
		  $riferimento = "<strong>";
          if ((($rif_versetto==1 && $row["Capitolo"]==1) || $PrimaRiga==1) && $l1!=$l2)
            $riferimento .= $libri_abb[$row["Libro"]]." ";
          if (($rif_versetto==1 || $PrimaRiga==1) && ($c1!=$c2 || $l1!=$l2)) {
            $riferimento .= $row["Capitolo"];
            if ($formato_rif=="vp")
              $riferimento .= ",";
            else
              $riferimento .= ":";
          }
		  $riferimento .= $rif_versetto."</strong>&nbsp;";
		  if (strlen($testo_versetto_inizio) > 0) {
		  	$brano .= $titolo_versetto0.$riferimento.$testo_versetto_inizio.$titolo_versetto;
		  }
		  else
          	$brano .= $titolo_versetto.$riferimento;
        }
      }
      else // solo un versetto, quindi non mettiamo il riferimento, solo l'eventuale titolo
        $brano .= $titolo_versetto0.$testo_versetto_inizio.$titolo_versetto;
      $brano .= $testo_versetto."";
    }
    else {
      $indica_libro = 0;
			if ($row["Libro"]!=$libro_precedente) {
			  if ($libro_precedente>0)
          $brano .= "    </book>\n";
			  $brano .= "    <book name=\"".$libri_eng[$row["Libro"]]."\" number=\"".$row["Libro"]."\">\n";
        $libro_precedente = $row["Libro"];
			}
			if ($row["Capitolo"]!=$capitolo_precedente) {
			  if ($capitolo_precedente>0) {
			    for ($i=1; $i<=$numero_sezioni; ++$i)
            $brano .= "        </section>\n";
          $numero_sezioni = 0;
          $brano .= "      </chapter>\n";
          $primo_versetto = 1;
  			}
			  $brano .= "      <chapter name=\"".$row["Capitolo"]."\">\n";
        $capitolo_precedente = $row["Capitolo"];
			}
			if ($primo_versetto==1 || $titolo_versetto!="") {
			  if ($primo_versetto!=1) {
			    for ($i=1; $i<=$numero_sezioni; ++$i)
            $brano .= "        </section>\n";
          $numero_sezioni = 0;
			  }
			  if ($titolo_versetto!="") {
			    if ($versione=="1") { // Nuova Riveduta
			      $righe = explode("<br />", $titolo_versetto);
			      foreach ($righe as $riga) {
			        if (substr($riga, 0, 3)=="<i>") { // titolo normale
      			    $riga = str_replace("<i>", "", $riga);
			          $riga = str_replace("</i>", "", $riga);
			          $brano .= "        <section name=\"".utf8_encode(html_entity_decode($riga))."\">\n";
			          ++$numero_sezioni;
			        }
			        else { // elenco di riferimenti
  			        $collegamenti = extract_link($riga);
                $riferimenti_xml = "          <references>\n";              
                foreach ($collegamenti as $riferimenti)
                {
                     $riferimenti_xml .= "            <reference>\n";
                     $riferimenti_xml .= "              <target>".implode("              </target>\n              <target>", $riferimenti)."              </target>\n";
                     $riferimenti_xml .= "            </reference>\n";
                }
                $riferimenti_xml .= "          </references>\n";
                $brano .= $riferimenti_xml;
			        }
			      }
			    }
			    else {
			      $brano .= $titolo_versetto;
			    }
			  }
			  else {
			    $brano .= "        <section>\n";
			    ++$numero_sezioni;
			  }
			  $primo_versetto = 0;
			}
      $brano .= "          <verse id=\"$rif_versetto\">".utf8_encode(html_entity_decode($testo_versetto))."</verse>\n";
    }

    $PrimaRiga = 0;
  }
  if ($brano=="")
    $brano = $RIF_NON_ESISTE;
  if ($xml==0) {
    $testo .= "<h1>";
    $testo .= converti_rif3($l1,$c1,$v1,$l2,$c2,$v2, "it", $formato_rif);
    $brano = str_replace("<<", "&lt;&lt;", $brano);
    $brano = str_replace(">>", "&gt;&gt;", $brano);
    $testo .= "</h1><p>".StripSlashes($brano)."</p>";
  }
  else {
  	    for ($i=1; $i<=$numero_sezioni; ++$i)
          $brano .= "        </section>\n";
        $numero_sezioni = 0;
        $brano .= "      </chapter>\n";
        $brano .= "    </book>\n";
    $testo .= $brano;
  }
}
else
  errore2("interrogazione database per versetti");
return str_replace("»", "&raquo;", str_replace("«","&laquo;", str_replace("î","&icirc;",$testo)));
}

/**
 * Estrae i collegamenti ad altri brani dal formato "(Mt 18:3; Ga 6:15; 2Co 5:17)(Gv 1:12-13; 1P 1:3, 23) Ez 36:25-27"
 *
 * @param string $link_text
 * @return array();
 */
function extract_link($link_text)
{
    $link_text = trim($link_text);
    $references = array();
    while(strpos($link_text,'(') !== false)
    {
        if(strpos($link_text,'(') == 0)
        {
            $ref1 = substr($link_text,1,strpos($link_text,')')-2);
            $references[] = explode('; ',$ref1);
           
            $link_text = substr($link_text,strpos($link_text,')')+1);
        }
        else
        {
            $ref0 = substr($link_text,0,strpos($link_text,'(')-1);
            $ref1 = substr($link_text,strpos($link_text,'(')+1,strpos($link_text,')')-strpos($link_text,'(')-1);
            $references[] = explode('; ',$ref1);
           
            if(strlen($ref0))
            {
                $link_text = trim($ref0).';'.trim(substr($link_text,strpos($link_text,')')+1));
            }
            else
            {
                $link_text = substr($link_text,strpos($link_text,')')+1);
            }
        }
    }
    if($link_text)
    {
        $verses = explode('; ',$link_text);
       
        $references[] = $verses;
    }
   
    return $references;
}

function vistesto($riferimento, $versioni, $formato_rif="dv", $vers_mult="v", $popup="n") {
  echo gettesto($riferimento, $versioni, 0, $formato_rif, $vers_mult);
  if ($popup=="n")
    echo pulsanti_dim();
}

function visualizzaXML($riferimento, $versioni, $formato_rif="dv") 
{
    echo gettesto($riferimento, $versioni, 1, $formato_rif);
}

function pulsanti_dim() {
    $testo = "<div style=\"text-align:center\">";
    $testo .= "<p><i>Dimensione testo:</i> ";
    $testo .= "<input class=\"grandezzatesto\" type=\"button\" name=\"diminuisci\" value=\"Pi&ugrave; piccolo\" onclick=\"curSize=parseInt($('#brano').css('font-size'))-2;if(curSize>=10){\$('#brano').css('font-size',curSize);var d=new Date();d.setTime(d.getTime()+(1000*24*60*60*1000));document.cookie='dim_testo='+curSize+';expires='+d.toUTCString()+';path=/';}\" />\n";
    $testo .= "<input class=\"grandezzatesto\" type=\"button\" name=\"risetta\" value=\"Predefinito\" onClick=\"$('#brano').css('font-size',16);document.cookie='dim_testo=0; path=/;';\" />\n";
	$testo .= "<input class=\"grandezzatesto\" type=\"button\" name=\"aumenta\" value=\"Pi&ugrave; grande\" onClick=\"curSize=parseInt($('#brano').css('font-size'))+2;if(curSize<=48){\$('#brano').css('font-size',curSize);var d=new Date();d.setTime(d.getTime()+(1000*24*60*60*1000));document.cookie='dim_testo='+curSize+';expires='+d.toUTCString()+';path=/';}\" />\n";
    $testo .= "</p></div>";
    return $testo;
}

function gettesto($riferimento, $versioni_in, $xml=0, $formato_rif="dv", $vers_mult="v", $titoli="s") {
global $RIF_NON_ESISTE;
global $formato_rif_trovato;
  $versioni = array();
  if (is_array($versioni_in))
    $versioni = $versioni_in;
  else {
    if ($versioni_in=="")
      $versione[0] = "Nuova Riveduta";
    else
      $versione[0] = $versioni_in;
  }
  $versioni[count($versioni)] = "Nuova Riveduta";
  if (count($versioni)>1)
    unset($versioni[count($versioni)-1]);
  $nVersioni = count($versioni);

  if (empty($riferimento))
    $riferimento = "Genesi 1:1";
  $rif3 = converti_rif($riferimento);
  if ($formato_rif_trovato!="" && $formato_rif=="auto")
  	$formato_rif=$formato_rif_trovato;
  if ($formato_rif=="auto")
    if (count($versioni)==1 && $versioni[0]=="C.E.I.")
    	$formato_rif = "vp";
    else
    	$formato_rif = "dv";
  if ($xml==0) {
    $testo = "<!-- RESULT LIST START --><div id=\"brano\">";
    $dim_testo = isset($_COOKIE["dim_testo"])?$_COOKIE["dim_testo"]:0;
    if ($dim_testo>1)
        $testo .= "<script type=\"text/javascript\">$('#brano').css('font-size',$dim_testo);</script>";
  }
  else {
    $testo  = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
    $testo .= "<!DOCTYPE results PUBLIC \"-//LaParola//DTD for Biblical Search Results//EN\" \"https://www.laparola.net/testo_biblico.dtd\">\n";
    $testo .= "<results for=\"$riferimento\" name=\"".converti_riferimento_brano($rif3, "it", $formato_rif)."\">\n";
  }
  
  if (strlen($rif3)==0) {
    if ($xml==0) {
      $testo .= "<p>Non capisco il riferimento $riferimento.</p>";
    }
    else
      $testo .= "<error>Non capisco il riferimento $riferimento.</error>\n";
  }
  else {
    $versioni_id_CEI = analizza_versione("C.E.I.");
    $nVersioni_v = 0;
    for ($i=0; $i<$nVersioni; $i++) {
      $versioni_id[$i] = analizza_versione($versioni[$i]);
      if (substr($versioni_id[$i],0,1)=="v")
        $nVersioni_v += 1;
    }

    if ($xml==0) {
      if ($nVersioni_v<2) $vers_mult="x"; // non mettiamo il table se c'è solo una versione della Bibbia
      if ($vers_mult=="v") {
        $testo .= "<table style=\"width:100%\" rules=\"cols\" cellpadding=\"5px\" border=\"1\" frame=\"void\"><tr valign=\"top\">";
//        $testo .= "<table rules=\"cols\" cellpadding=\"5px\" border=\"1\" frame=\"void\" style=\"table-layout: fixed\"><tr valign=\"top\">";
//        table-layout:fixed dà problemi con IE6,7 in modalità strict (che la pagina è con questo DOCTYPE)
        $larghezza_col = 100 / $nVersioni_v;
    }
      $comm_fatto = 0;
    }
    else
      $vers_mult = "x";

    for ($i=0; $i<$nVersioni; $i++) {
      if (substr($versioni_id[$i],1)!="0") {
         if ($vers_mult=="v") {
           if ($versioni_id[$i][0]=="v")
             $testo .= "<td style=\"max-width:1px;width: ".$larghezza_col."%\"><div style=\"overflow-wrap:break-word\">";
           else if ($comm_fatto==0) {
             $testo .= "</tr></table>";
             $comm_fatto = 1;
           }
         }

         $urlversione = $versioni[$i];
         if ($urlversione=="Bibbia della Gioia") $urlversione="La Parola &egrave; Vita"; // perché il nome della versione è stato cambiato
         if ($urlversione=="Volgare") $urlversione="Bibbia in Volgare";
         if ($urlversione=="Nuova Riveduta 1994") $urlversione="Nuova Riveduta (1994)";
         if ($urlversione=="CommentarioNT") $urlversione="Commentario del Nuovo Testamento";
         if ($urlversione=="Commentario") $urlversione="Commentario abbreviato di Matthew Henry";
         if ($urlversione=="CommentarioPulpito") $urlversione="Commentario del Pulpito";
         if ($urlversione=="CommentarioIllustratore") $urlversione="Illustratore biblico";
         if ($urlversione=="CommentarioGill") $urlversione="Esposizione della Bibbia di John Gill";
         if ($urlversione=="CommentarioBarnes") $urlversione="Note di Albert Barnes sulla Bibbia";
         if ($urlversione=="CommentarioMeyer") $urlversione="Commento di Frederick Brotherton Meyer";
         if ($urlversione=="CommentarioTesoro") $urlversione="Tesoro di Davide";
         if ($urlversione=="CommentarioHenry") $urlversione="Commentario completo di Matthew Henry";
         if ($urlversione=="CommentarioCalvino") $urlversione="Commentario di Giovanni Calvino";
         if ($urlversione=="CommentarioGinevra") $urlversione="Note della Bibbia di Ginevra";
         $url = "<a href=\"?riferimento=$riferimento&versioni[]=$versioni[$i]\">".$urlversione;
         if ($xml==0) {
           $testo .= "<!-- RESULT ITEM START -->";
           if (count($versioni)>1)
             $testo .= "<h2>$url</a>:</h2>";
         }
         else
           $testo .= "  <version name=\"$versioni[$i]\">\n";

         for ($j=0; $j<strlen($rif3)/6; $j++) {
           $k = 6*$j;
           $libro_inizio = ord($rif3[$k]);
           $capitolo_inizio = ord($rif3[$k+1]);
           $versetto_inizio = ord($rif3[$k+2]);
           $libro_fine = ord($rif3[$k+3]);
           $capitolo_fine = ord($rif3[$k+4]);
           $versetto_fine = ord($rif3[$k+5]);
           $limitato = 0;
           if ($versioni[$i]=="Bibbia della Gioia") {
		   		if ($libro_inizio!=$libro_fine || $capitolo_inizio!=$capitolo_fine)	{
		   			$libro_fine = $libro_inizio;
		   			$capitolo_fine = $capitolo_inizio;
		   			$versetto_fine = 177;
					$limitato = 1;	
				}
		   }
$testobrano = get_brano(substr($versioni_id[$i],1), $versioni_id[$i][0], $libro_inizio, $capitolo_inizio, $versetto_inizio, $libro_fine, $capitolo_fine, $versetto_fine, $formato_rif, $xml, $titoli);
           if (strpos($testobrano,$RIF_NON_ESISTE)>0 && $nVersioni == 0) {
// nel caso che nessuna versione fu data, si usa la NR. Ma per l'apocrifa, bisogna usare la CEI
$testobrano = get_brano(substr($versioni_id_CEI,1), $versioni_id_CEI[0], $libro_inizio, $capitolo_inizio, $versetto_inizio, $libro_fine, $capitolo_fine, $versetto_fine, $formato_rif, $xml, $titoli);
           }
           $testo .= $testobrano;
         }
         if ($xml==0) {
            if ($versioni[$i]=="Bibbia della Gioia") {
            	if ($limitato==1)
            		$testo .= "<p><i>Non &egrave; possibile visualizzare pi&ugrave; di un capitolo di questa versione.</i></p><br /><br />";
            	$testo .= "<p><span class=\"normale\">La Parola &egrave; Vita<br />Copyright &copy; 1981, 1994 di <a href=\"http://www.biblica.com/\">Biblica</a>, Inc.&reg;<br />Usato con permesso. Tutti i diritti riservati in tutto il mondo.</span></p>";
            }
           $testo .= "<!-- RESULT ITEM END -->";
           if ($vers_mult=="v" && $versioni_id[$i][0]=="v")
             $testo .= "</div></td>";
         }
         else
           $testo .= "  </version>\n";
      }
    }
    if ($vers_mult=="v" && $comm_fatto==0)
	   $testo .= "</tr></table>";
  }


  if ($xml==0) {
    $testo .= "</div><!-- RESULT LIST END -->\n";
  }
  else
    $testo .= "</results>\n";
    $testo = str_replace("»", "&raquo;", str_replace("«", "&laquo;", $testo));
//    $testo = str_replace("’", "'", $testo);
    return $testo;
}
?>
