<?
header("Content-type: text/html; charset=utf-8");
$bConfMss = (isset($_REQUEST["ConfMss"])?$_REQUEST["ConfMss"]:null);
$bConfMss = str_replace("<", "", $bConfMss); // affinché tag HTML non possono essere inseriti nella pagina
$bConfMss = str_replace(">", "", $bConfMss);
$rif1 = (isset($_REQUEST["rif1"])?$_REQUEST["rif1"]:0);
$rif1 = str_replace("<", "", $rif1);
$rif1 = str_replace(">", "", $rif1);
$rif2 = (isset($_REQUEST["rif2"])?$_REQUEST["rif2"]:"");
$rif2 = str_replace("<", "", $rif2);
$rif2 = str_replace(">", "", $rif2);
$mss1 = (isset($_REQUEST["mss1"])?$_REQUEST["mss1"]:"");
$mss1 = str_replace("<", "", $mss1);
$mss1 = str_replace(">", "", $mss1);
$mss2 = (isset($_REQUEST["mss2"])?$_REQUEST["mss2"]:"");
$mss2 = str_replace("<", "", $mss2);
$mss2 = str_replace(">", "", $mss2);
$conf_libro = (isset($_REQUEST["conf_libro"])?$_REQUEST["conf_libro"]:0);
$conf_libro = str_replace("<", "", $conf_libro);
$conf_libro = str_replace(">", "", $conf_libro);
$ord = (isset($_REQUEST["ord"])?$_REQUEST["ord"]:0);
$ord = str_replace("<", "", $ord);
$ord = str_replace(">", "", $ord);
if ($ord==0) {
   $ord = (isset($_REQUEST["greco_ord"])?$_REQUEST["greco_ord"]:1);
$ord = str_replace("<", "", $ord);
$ord = str_replace(">", "", $ord);
}
$varianti = (isset($_REQUEST["varianti"])?$_REQUEST["varianti"]:null);
$varianti = str_replace("<", "", $varianti);
$varianti = str_replace(">", "", $varianti);
$fontuni = (isset($_REQUEST["fontuni"])?$_REQUEST["fontuni"]:"");
  $fontuni = str_replace("<", "", $fontuni); // affinché tag HTML non possono essere inseriti nella pagina
  $fontuni = str_replace(">", "", $fontuni);
if ($fontuni=="")
   $fontuni = (isset($_REQUEST["greco_fontuni"])?$_REQUEST["greco_fontuni"]:"");
$lin = "it";
include("../conn.php");
include("../vistesto.php");
global $libri_nomi;
global $libri_eng;
global $lin;
global $conn;

function nouni($s) {
$s2 = $s;
while (($p = strpos($s2, "<span class=\"uni\">")) !== FALSE) {
//echo $p."qq".substr($s2,0,30)."\n";
	$p2=strpos($s2, "</span>",$p);
	//$p = strpos($s2, "<span class=\"uni\">");
	$s2=substr($s2,0,$p2).substr($s2,$p2+7);
	$s2=substr($s2,0,$p).substr($s2,$p+18);
}
if (($p = strpos($s2, "<a href=\"?")) !== FALSE) {
  while (($p = strpos($s2, "<a href=\"?")) !== FALSE) {
	$p2=strpos($s2, "\">",$p);
	$s2=substr($s2,0,$p).substr($s2,$p2+2);
  }
  while (($p = strpos($s2, "</a>")) !== FALSE) {
	$s2=substr($s2,0,$p).substr($s2,$p+4);
  }
}
return "|".$s2;
}

function TraduciMss($mss) {
global $conn;
$mss2 = htmlspecialchars($mss, ENT_QUOTES);
if ($mss[0]=="p" && strlen($mss)>1)
   if ($mss[1]>="0" && $mss[1]<="9") $mss2 = "p<sup>".substr($mss,1)."</sup>";
if ($mss[0]=="l" && strlen($mss)>1)
   if (($mss[1]>="0" && $mss[1]<="9") || $mss=="lAD") $mss2 = "l<sup>".substr($mss,1)."</sup>";
if (substr($mss,0,2)=="it" && strlen($mss)>2)
   $mss2 = "it<sup>".substr($mss,2)."</sup>";
if ((substr($mss,0,3)=="sir" || substr($mss,0,3)=="syr") && strlen($mss)>3)
   $mss2 = "sir<sup>".substr($mss,3)."</sup>";
if (substr($mss,0,3)=="cop" && strlen($mss)>3)
   $mss2 = "cop<sup>".substr($mss,3)."</sup>";
if (substr($mss,0,3)=="eth" && strlen($mss)>3)
   $mss2 = "et<sup>".substr($mss,3)."</sup>";
else if (substr($mss,0,2)=="et" && strlen($mss)>2)
   $mss2 = "et<sup>".substr($mss,2)."</sup>";
if (substr($mss,0,3)=="geo" && strlen($mss)>3)
   $mss2 = "geo<sup>".substr($mss,3)."</sup>";
if (substr($mss,0,11)=="Diatessaron" && strlen($mss)>11)
   $mss2 = "Diatessaron<sup>".substr($mss,11)."</sup>";
if ($mss=="alef" || $mss=="aleph") $mss2 = '<span class=\"uni\">&#8237;&#1488;</span>';
if ($mss=="gamma") $mss2 = '<span class=\"uni\">&#915;</span>';
if ($mss=="delta") $mss2 = '<span class=\"uni\">&#916;</span>';
if ($mss=="theta") $mss2 = '<span class=\"uni\">&#920;</span>';
if ($mss=="lambda") $mss2 = '<span class=\"uni\">&#923;</span>';
if ($mss=="xi") $mss2 = '<span class=\"uni\">&#926;</span>';
if ($mss=="pi") $mss2 = '<span class=\"uni\">&#928;</span>';
if ($mss=="sigma") $mss2 = '<span class=\"uni\">&#931;</span>';
if ($mss=="phi" || $mss=="fi") $mss2 = '<span class=\"uni\">&#934;</span>';
if ($mss=="psi") $mss2 = '<span class=\"uni\">&#936;</span>';
if ($mss=="omega") $mss2 = '<span class=\"uni\">&#937;</span>';
if ($mss=="TR") $mss2 = '<span class=\"uni\">&#962;</span>';
if ($mss2=="it<sup>beta</sup>") $mss2 = 'it<sup><span class=\"uni\">&#946;</span></sup>';
if ($mss2=="it<sup>delta</sup>") $mss2 = 'it<sup><span class=\"uni\">&#948;</span></sup>';
if ($mss2=="it<sup>lambda</sup>") $mss2 = 'it<sup><span class=\"uni\">&#955;</span></sup>';
if ($mss2=="it<sup>mu</sup>") $mss2 = 'it<sup><span class=\"uni\">&#956;</span></sup>';
if ($mss2=="it<sup>fi</sup>" || $mss2=="it<sup>phi</sup>") $mss2 = 'it<sup><span class=\"uni\">&#960;</span></sup>';
if ($mss2=="it<sup>pi</sup>") $mss2 = 'it<sup><span class=\"uni\">&#966;</span></sup>';
if ($mss2=="it<sup>ro</sup>" || $mss2=="it<sup>rho</sup>") $mss2 = 'it<sup><span class=\"uni\">&#961;</span></sup>';
$sql = "SELECT id_mss FROM Mss WHERE (Mss_nome_it=\"".$mss2."\" OR Mss_nome_ing=\"".$mss2."\")";
$n_mss = 0;
if ($ris=mysqli_query($conn, "$sql")) {
   if (mysqli_num_rows($ris)>0) {
       $row = mysqli_fetch_array ($ris);
       $n_mss = $row["id_mss"];
   }
}
else {
     errore2("interrogazione database per trovare il manuscritto $mss ($mss2)");
}
if ($mss=="text" || $mss=="testo" || $mss=="UBS") $n_mss = -1;
return $n_mss;
}

function MostraVarianti($ris,$ord,$lin) {
global $libri_nomi;
global $libri_eng;
      $nDiff = mysqli_num_rows($ris);
      if ($nDiff<20) {
         while ($row=mysqli_fetch_array($ris))
               MostraBrano($row["Libro"],$row["Capitolo"].":".$row["Versetto"], $ord, "n");
      }
      else if ($nDiff<200) {
           echo "<p>";
           while ($row=mysqli_fetch_array($ris)) {
              if ($lin=="it")
                 echo '<a href="?rif1='.$row["Libro"].'&rif2='.$row["Capitolo"].'%3A'.$row["Versetto"].'">'.$libri_nomi[$row["Libro"]]." ".$row["Capitolo"].":".$row["Versetto"]."</a><br />";
              else
                 echo '<a href="?rif1='.$row["Libro"].'&rif2='.$row["Capitolo"].'%3A'.$row["Versetto"].'">'.$libri_eng[$row["Libro"]]." ".$row["Capitolo"].":".$row["Versetto"]."</a><br />";
           }
           echo "</p>";
      }
}

function MostraBrano($r1, $r2, $ord, $varianti) {
global $libri_nomi;
global $libri_eng;
global $lin;
global $conn;
   $rif = converti_rif($libri_nomi[$r1].$r2);
   if (strlen($rif)==0) {
      if ($lin=="it")
         echo "<p>Non ho capito il riferimento $libri_nomi[$r1]$r2.</p>";
      else
          echo "<p>I could not understand the reference $libri_eng[$r1]$r2.</p>";
//      $rif=chr(0)+chr(0)+chr(0)+chr(0)+chr(0)+chr(0);
   }
   for ($j=0; $j<strlen($rif)/6; $j++) {
       $cap1=ord($rif[1+$j*6]);
       $vers1=ord($rif[2+$j*6]);
       $cap2=ord($rif[4+$j*6]);
       $vers2=ord($rif[5+$j*6]);

          if ($cap1==$cap2)
          $sqlcv = "Capitolo=$cap1 AND Versetto>=$vers1 AND Versetto<=$vers2";
       else
           $sqlcv = "((Capitolo=$cap1 AND Versetto>=$vers1) OR (Capitolo>$cap1 AND Capitolo<$cap2) OR (Capitolo=$cap2 AND Versetto<=$vers2))";
if ($varianti!="s") {
       if ($lin=="it")
          $sql = "SELECT Data_it, Tipo_it, Mss_nome_it, AltVar_it, TestoVar_it";
       else
           $sql = "SELECT Data_ing, Tipo_ing, Mss_nome_ing, AltVar_ing, TestoVar_ing";
       $sql .= ", id_var, VarInVers, VarInVar, Tipo_db, Modifiche, Capitolo, Versetto, VarCommenti FROM Mss, Sostegno, Varianti, MssTipo, MssData WHERE id_mssdata=id_data AND id_msstipo=id_tipo AND id_sosmss=id_mss AND id_sosvar=id_var AND Libro=$r1 AND ".$sqlcv;
       switch ($ord) {
       case 2:
            $sql .= " ORDER BY id_sosvar,Data_db,id_sosmss";
            break;
       case 3:
            $sql .= " ORDER BY id_sosvar,Tipo_db,id_sosmss";
            break;
       default: // predefinito è tipo 1 cioè per tipo di manoscritto
            $sql .= " ORDER BY id_sosvar";
       }
       $iVarInVers = -1;
       $iVar = -1;
       $sSos = "";
       $tipo_prec = -1;
       $cap_prec=0; $vers_prec=0;
       $NuovaVariante = 0;
       if ($ris2=mysqli_query($conn, "$sql")) {
          while ($row2=mysqli_fetch_array($ris2)) {
            if ($row2["VarInVers"]!=$iVarInVers || $row2["Capitolo"]!=$cap_prec || $row2["Versetto"]!=$vers_prec) {
               if ($iVarInVers!=-1) {
                  echo nouni($sSos);
                  $sSos = "";
                  $tipo_prec = -1;
               }
               if ($row2["Capitolo"]!=$cap_prec || $row2["Versetto"]!=$vers_prec) {
/*                  if ($solo1versetto==1) {
                     if ($cap_prec>0) echo "<p>";
                     echo "<b>".$row2["Capitolo"].":".$row2["Versetto"]."</b>\n";
                  }*/
                  $cap_prec = $row2["Capitolo"];
                  $vers_prec = $row2["Versetto"];
               }
               else
	                echo "";
               $iVarInVers = $row2["VarInVers"];
               if ($iVarInVers==1)
              		$NuovaVariante = 1;
            }
            if ($row2["id_var"]!=$iVar) {
            	if ($row2["VarInVar"]==2) {
            		$sSosVecchio = $sSos;
            		$NuovaVariante = 0;
            	}
               $iVar = $row2["id_var"];
               if ($sSos!="")
                  echo nouni($sSos);
                if ($row2["VarInVar"]==1)
	              echo "\r\n$r1|".$row2["Capitolo"]."|".$row2["Versetto"];
               if ($row2["VarInVar"]>=200) {
                 $sSos = "<i>$row2[4]</i><a href=\"".$row2["VarCommenti"]."\" target=\"_blank\">A Student's Guide to New Testament Textual Variants</a>";
               }
               else {
               $sSos = $row2[4]."]";
               $VarComm = $row2["VarCommenti"];
               if ($VarComm=="p")
                  if ($lin=="it") $sSos.=" (<i>vedi brano parallelo</i>)"; else $sSos.=" (<i>see parallel passage)</i>";
               elseif ($VarComm!="") {
                  $sSos.=" (<i>".($lin=="it"?"vedi":"see");
                  $tok = strtok($VarComm, ";");
                  $rif_nuovo = "";
                  while ($tok) {
                     $tok = trim($tok);
                     $rif3 = converti_rif($tok);
                     if ($rif_nuovo!="") $rif_nuovo.="; ";
                     if (ord($rif3[0])>=47)
                        $rif_nuovo .= '<a href="?rif1='.ord($rif3[0]).'&rif2='.ord($rif3[1]).'%3A'.ord($rif3[2]).'">'.converti_rif3(ord($rif3[0]),ord($rif3[1]),ord($rif3[2]),ord($rif3[3]),ord($rif3[4]),ord($rif3[5]),$lin).'</a>';
                     else
                        $rif_nuovo .= converti_rif3(ord($rif3[0]),ord($rif3[1]),ord($rif3[2]),ord($rif3[3]),ord($rif3[4]),ord($rif3[5]),$lin);
                     if (strpos($tok, " ", strpos($tok, " ")+1)===false) {
                        //
                     }
                     else {
                          $rif_nuovo .= substr($tok, strpos($tok, " ", strpos($tok, " ")+1));
                     }
                     $tok = strtok(";");
                  }
                  $sSos.="</i> ".$rif_nuovo.")";
               }
               $tipo_prec = -1;
               }
            }

            $titolo = $row2[0]!=""?($lin=="it"?"Data=":"Date=").$row2[0]:"";
            if ($row2[0]!="" && $row2[1]!="") $titolo.="; ";
            if ($row2[1]!="") $titolo.=($lin=="it"?"Tipo di testo=":"Text type=").$row2[1];
            if (substr($sSos,strlen($sSos)-6,6)!="&nbsp;")
               $sSos .= ' ';

            $ms_mod = $row2[2];
            $ms_mod2 = "";
            $modifiche = $row2["Modifiche"];
            while (strpos($modifiche, "!")!==false) {
               $modifica_2lingue = substr($modifiche, 0, strpos($modifiche, "!"));
               if ($modifica_2lingue{strlen($modifica_2lingue)-1}=="<")
                  if ($lin!="it")
                     $modifica_2lingue = "";
                  else
                      $modifica_2lingue = substr($modifiche, 0, strlen($modifica_2lingue)-1);
               elseif ($modifica_2lingue{strlen($modifica_2lingue)-1}==">")
                  if ($lin=="it")
                     $modifica_2lingue = "";
                  else
                      $modifica_2lingue = substr($modifiche, 0, strlen($modifica_2lingue)-1);
               $ms_mod2 .= $modifica_2lingue;
               $modifiche = substr($modifiche, strpos($modifiche, "!")+1);
            }
            if (strpos($modifiche, "(")!==false || ((strpos($modifiche, ")")!==false || strpos($modifiche, "O")!==false) && $ord>1))
               $sSos .= "(";
            if (strpos($modifiche, "[")!==false)
               $sSos .= "[";
            if (strpos($modifiche, "]")!==false)
               $sSos .= "[[";
            if (strpos($modifiche, "u")!==false)
               $ms_mod .= "<sup>supp</sup>"; // prima di 2
            if (strpos($modifiche, "U")!==false)
               $ms_mod .= "<sup>(supp)</sup>";
            if (strpos($modifiche, "1")!==false)
               $ms_mod .= "<sup>1</sup>";
            if (strpos($modifiche, "2")!==false)
               $ms_mod .= "<sup>2</sup>";
            if (strpos($modifiche, "t")!==false)
               $ms_mod .= "<sup>lat</sup>";
            if (strpos($modifiche, "T")!==false)
               if ($ms_mod2!="")
                  $ms_mod2 .= "(lat)"; // per es. mss^secondo Origene(lat)
               else
                   $ms_mod .= "<sup>(lat)</sup>";
            if (strpos($modifiche, "k")!==false)
               $ms_mod .= "<sup>gr</sup>";
            if (strpos($modifiche, "K")!==false)
               if ($ms_mod2!="")
                  $ms_mod2 .= "(gr)"; // per es. mss^secondo Origene(gr)
               else
                   $ms_mod .= "<sup>(gr)</sup>";
            if ($ms_mod2!="") $ms_mod2 = "<sup>".$ms_mod2."</sup>";
            if (strpos($modifiche, "z")!==false)
               $ms_mod .= "<sup>arm</sup>";
            if (strpos($modifiche, "a")!==false)
               $ms_mod .= "<sup>slav</sup>";
            if (strpos($modifiche, "i")!==false)
               if ($lin=="it") $ms_mod .= "<sup>sir</sup>"; else $ms_mod .= "<sup>syr</sup>";
            if (strpos($modifiche, "o")!==false)
               $ms_mod .= "<sup>comm</sup>";
            if (strpos($modifiche, "r")!==false)
               if ($lin=="it") $ms_mod .= "<sup>l.v.</sup>"; else $ms_mod .= "<sup>v.r.</sup>";
            if (strpos($modifiche, "R")!==false)
               if ($lin=="it") $ms_mod .= "<sup>(l.v.)</sup>"; else $ms_mod .= "<sup>(v.r.)</sup>";
            if (strpos($modifiche, "s")!==false)
               if ($lin=="it") $ms_mod .= "<sup>testo</sup>"; else $ms_mod .= "<sup>text</sup>";
            if (strpos($modifiche, "S")!==false)
               if ($lin=="it") $ms_mod .= "<sup>(testo)</sup>"; else $ms_mod .= "<sup>(text)</sup>";
            if (strpos($modifiche, "y")!==false)
               $ms_mod .= "<sup>s</sup>";
            if (strpos($modifiche, "e")!==false)
               $ms_mod .= "<sup>m</sup>";
            if (strpos($modifiche, "m")!==false)
               $ms_mod .= "<sup>ms</sup>";
            if (strpos($modifiche, "M")!==false)
               $ms_mod .= "<sup>(ms)</sup>";
            if (strpos($modifiche, "n")!==false)
               $ms_mod .= "<sup>mss</sup>";
            if (strpos($modifiche, "N")!==false)
               $ms_mod .= "<sup>(mss)</sup>";
            if (strpos($modifiche, "p")!==false)
               $ms_mod .= "<sup>pt</sup>";
            if (strpos($modifiche, "P")!==false)
               $ms_mod .= "<sup>(pt)</sup>";
            if (strpos($modifiche, "d")!==false) // prima di lem
               $ms_mod .= "<sup>dub</sup>";
            if (strpos($modifiche, "D")!==false)
               $ms_mod .= "<sup>(dub)</sup>";
            if (strpos($modifiche, "l")!==false) // dopo dub
               $ms_mod .= "<sup>lem</sup>";
            if (strpos($modifiche, "L")!==false)
               $ms_mod .= "<sup>(lem)</sup>";
            if (strpos($modifiche, "*")!==false) // dopo ms
               $ms_mod .= "*";
            if (strpos($modifiche, "#")!==false)
               $ms_mod .= "**";
            if (strpos($modifiche, "c")!==false) // dopo ms
               $ms_mod .= "<sup>c</sup>";
            if (strpos($modifiche, "C")!==false)
               $ms_mod .= "<sup>(c)</sup>";
            if (strpos($modifiche, "g")!==false) // dopo ms e (c)
               $ms_mod .= "<sup>mg</sup>";
            if (strpos($modifiche, "G")!==false)
               $ms_mod .= "<sup>(mg)</sup>";
            if (strpos($modifiche, "v")!==false) // dopo * e (c)
               $ms_mod2 .= "<sup><i>vid</i></sup>";
            if (strpos($modifiche, "V")!==false)
               $ms_mod2 .= "<sup><i>(vid)</i></sup>";
            if (strpos($modifiche, "?")!==false) // dopo vid, (c)
               $ms_mod2 .= "?";
            $sSos .= '<span title="'.$titolo.'">'.$ms_mod.$ms_mod2."</span>";
            if (strpos($modifiche, ")")!==false || ((strpos($modifiche, "(")!==false || strpos($modifiche, "O")!==false) && $ord>1)) {
               if ($ord==1) {
                  if ($row2[3]!="") $sSos .= " ";
                  $sSos .= $row2[3];
               }
               $sSos .= ")";
            }
            if (strpos($modifiche, "[")!==false)
               $sSos .= "]";
            if (strpos($modifiche, "]")!==false)
               $sSos .= "]]";
         } // while ($row2=mysqli_fetch_array($ris2)) {
         if ($sSos!="")
            echo nouni($sSos)."\r\n";
       } // if ($ris2=mysqli_query($conn, "$sql")) {
       else {
            errore2("interrogazione database per visualizzare le varianti di $libri_nomi[$r1].$r2");
       }
    } // if ($varianti!="s")
  }
}

for ($i=47; $i<=73; $i++)
//for ($i=47; $i<=50; $i++)
//for ($i=51; $i<=73; $i++)
//for ($i=69; $i<=71; $i++)
	MostraBrano($i, "", 1, "n");
?>
