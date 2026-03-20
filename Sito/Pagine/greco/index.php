<?php
function GetCookie($nome, $predefinito) {
	$v = (isset($_REQUEST[$nome])?$_REQUEST[$nome]:"");
  $v = str_replace("<", "", $v); // affinché tag HTML non possono essere inseriti nella pagina
  $v = str_replace(">", "", $v);
  $v = str_replace("\"", "", $v);
	if ($v=="")
		$v = (isset($_COOKIE[$nome])?$_COOKIE[$nome]:$predefinito);
  $v = str_replace("<", "", $v); // affinché tag HTML non possono essere inseriti nella pagina
  $v = str_replace(">", "", $v);
  $v = str_replace("\"", "", $v);
	return $v;
}

function GetCookieVuoto($nome) {
	return GetCookie($nome, "");
}

function GetPostCookie($nome, $predefinito) {
	$v = (isset($_POST[$nome])?$_POST[$nome]:"");
  $v = str_replace("<", "", $v); // affinché tag HTML non possono essere inseriti nella pagina
  $v = str_replace(">", "", $v);
  $v = str_replace("\"", "", $v);
	if ($v=="")
		$v = (isset($_GET[$nome])?$_GET[$nome]:"");
  $v = str_replace("<", "", $v); // affinché tag HTML non possono essere inseriti nella pagina
  $v = str_replace(">", "", $v);
  $v = str_replace("\"", "", $v);
	if ($v=="")
		$v = GetCookie($nome, $predefinito);
  $v = str_replace("<", "", $v); // affinché tag HTML non possono essere inseriti nella pagina
  $v = str_replace(">", "", $v);
  $v = str_replace("\"", "", $v);
	return $v;
}

function GetPostCookieVuoto($nome) {
	return GetPostCookie($nome, "");
}

function GetCookieN($nome) {
	$v = (int)(isset($_REQUEST[$nome])?$_REQUEST[$nome]:0);
	if ($v==0)
		$v = (int)(isset($_COOKIE[$nome])?$_COOKIE[$nome]:0);
	return $v;
}

function sanitizeVariabile($v) {
    $v = preg_replace_callback('/%[0-9A-F]{2}/i', function($match) {
        return strtolower($match[0]);
    }, $v);
    $v = str_replace(['<', '>', '"'], '', $v);
    $v = str_replace(['%3c', '%3e', '%22'], '', $v);
    return $v;
}

header("Content-type: text/html; charset=utf-8");
$bConfMss = (isset($_REQUEST["ConfMss"])?$_REQUEST["ConfMss"]:"");
$bConfMss = sanitizeVariabile($bConfMss);
$bTrovaPar = (isset($_REQUEST["TrovaPar"])?$_REQUEST["TrovaPar"]:"");
$bTrovaPar = sanitizeVariabile($bTrovaPar);
$bTrovaVers = (isset($_REQUEST["TrovaVers"])?$_REQUEST["TrovaVers"]:"");
$bTrovaVers = sanitizeVariabile($bTrovaVers);
$rif1 = (int)(isset($_REQUEST["rif1"])?$_REQUEST["rif1"]:0);
$rif2 = (isset($_REQUEST["rif2"])?$_REQUEST["rif2"]:"1:1");
$rif2 = sanitizeVariabile($rif2);
$bibleworks_bk = (isset($_REQUEST["bk"])?$_REQUEST["bk"]:"");
$bibleworks_bk = sanitizeVariabile($bibleworks_bk);
$bibleworks_ch = (int)(isset($_REQUEST["ch"])?$_REQUEST["ch"]:0);
$bibleworks_vs = (int)(isset($_REQUEST["vs"])?$_REQUEST["vs"]:0);
$xml_out = (int)(isset($_REQUEST["xml"])?$_REQUEST["xml"]:0);
$mss1 = (isset($_REQUEST["mss1"])?$_REQUEST["mss1"]:"");
$mss1 = sanitizeVariabile($mss1);
$mss2 = (isset($_REQUEST["mss2"])?$_REQUEST["mss2"]:"");
$mss2 = sanitizeVariabile($mss2);
$ConfMss_Rif = (isset($_REQUEST["ConfMss_Rif"])?$_REQUEST["ConfMss_Rif"]:"");
$ConfMss_Rif = sanitizeVariabile($ConfMss_Rif);
$TrovaPar_Tipo = (isset($_REQUEST["TrovaPar_Tipo"])?$_REQUEST["TrovaPar_Tipo"]:"--");
$TrovaPar_Tipo = sanitizeVariabile($TrovaPar_Tipo);
$TrovaPar_Ordine = (int)(isset($_REQUEST["TrovaPar_Ordine"])?$_REQUEST["TrovaPar_Ordine"]:"0");
$Gram1 = (isset($_REQUEST["Gram1"])?$_REQUEST["Gram1"]:"_");
$Gram1 = sanitizeVariabile($Gram1);
$Gram2 = (isset($_REQUEST["Gram2"])?$_REQUEST["Gram2"]:"_");
$Gram2 = sanitizeVariabile($Gram2);
$Gram3 = (isset($_REQUEST["Gram3"])?$_REQUEST["Gram3"]:"_");
$Gram3 = sanitizeVariabile($Gram3);
$Gram4 = (isset($_REQUEST["Gram4"])?$_REQUEST["Gram4"]:"_");
$Gram4 = sanitizeVariabile($Gram4);
$Gram5 = (isset($_REQUEST["Gram5"])?$_REQUEST["Gram5"]:"_");
$Gram5 = sanitizeVariabile($Gram5);
$Gram6 = (isset($_REQUEST["Gram6"])?$_REQUEST["Gram6"]:"_");
$Gram6 = sanitizeVariabile($Gram6);
$Gram7 = (isset($_REQUEST["Gram7"])?$_REQUEST["Gram7"]:"_");
$Gram7 = sanitizeVariabile($Gram7);
$Gram8 = (isset($_REQUEST["Gram8"])?$_REQUEST["Gram8"]:"_");
$Gram8 = sanitizeVariabile($Gram8);
$TrovaPar_Rif1 = (isset($_REQUEST["TrovaPar_Rif1"])?$_REQUEST["TrovaPar_Rif1"]:"");
$TrovaPar_Rif1 = sanitizeVariabile($TrovaPar_Rif1);
$TrovaPar_Rif2 = (isset($_REQUEST["TrovaPar_Rif2"])?$_REQUEST["TrovaPar_Rif2"]:"");
$TrovaPar_Rif2 = sanitizeVariabile($TrovaPar_Rif2);
$TrovaVers_Esp = (isset($_REQUEST["TrovaVers_Esp"])?$_REQUEST["TrovaVers_Esp"]:"");
$TrovaVers_Esp = sanitizeVariabile($TrovaVers_Esp);
$TrovaVers_Rif = (isset($_REQUEST["TrovaVers_Rif"])?$_REQUEST["TrovaVers_Rif"]:"");
$TrovaVers_Rif = sanitizeVariabile($TrovaVers_Rif);
$TrovaVers_Versione = (isset($_REQUEST["TrovaVers_Versione"])?$_REQUEST["TrovaVers_Versione"]:"");
$TrovaVers_Versione = sanitizeVariabile($TrovaVers_Versione);
$TrovaPar_Versione = (isset($_REQUEST["TrovaPar_Versione"])?$_REQUEST["TrovaPar_Versione"]:"");
$TrovaPar_Versione = sanitizeVariabile($TrovaPar_Versione);
$ord = GetCookieN("ord");
if ($ord==0) $ord = GetCookieN("greco_ord");
$msstt = GetPostCookie("msstt", "s");
$varianti = GetPostCookie("varianti", "n");
$wh = GetPostCookie("wh", "n");
$tisch = GetPostCookie("tisch", "n");
$biz = GetPostCookie("biz", "n");
$inter = GetPostCookie("inter", "n");
$allusioni = GetPostCookie("allusioni", "n");
$direzione = GetPostCookie("direzione", "v");
$fontuni = GetCookieVuoto("fontuni");
if ($fontuni=="") $fontuni = GetCookieVuoto("greco_fontuni");
$rn = (int)(isset($_REQUEST["rn"])?$_REQUEST["rn"]:0);
$rad = (isset($_REQUEST["rad"])?$_REQUEST["rad"]:"");
$rad = sanitizeVariabile($rad);
$p = (isset($_REQUEST["p"])?$_REQUEST["p"]:"");
$p = sanitizeVariabile($p);
$g = (isset($_REQUEST["g"])?$_REQUEST["g"]:"");
$g = sanitizeVariabile($g);
$nVolteMin = (int)(isset($_REQUEST["nVolteMin"])?$_REQUEST["nVolteMin"]:1000);
if ($nVolteMin<=0) $nVolteMin=1;
$nVolteMas = (int)(isset($_REQUEST["nVolteMas"])?$_REQUEST["nVolteMas"]:99999);
if ($nVolteMas<=0) $nVolteMas=99999;
$lin = GetCookieVuoto("lin");
if ($lin=="")
	$lin = GetCookieVuoto("greco_lingua");
if ($lin=="")
	if (isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) $lin=substr($_SERVER['HTTP_ACCEPT_LANGUAGE'],0,2);
if ($lin!="")
   SetCookie("greco_lingua", $lin, time()+3600000);
SetCookie("greco_fontuni", $fontuni, time()+3600000);
SetCookie("greco_ord", $ord, time()+3600000);
SetCookie("msstt", $msstt, time()+3600000);
SetCookie("varianti", $varianti, time()+3600000);
SetCookie("allusioni", $allusioni, time()+3600000);
SetCookie("direzione", $direzione, time()+3600000);
SetCookie("wh", $wh, time()+3600000);
SetCookie("tisch", $tisch, time()+3600000);
SetCookie("biz", $biz, time()+3600000);
SetCookie("inter", $inter, time()+3600000);
include("../conn.php");
include("../vistesto.php");
include("funzioni_greco.php");
$libri_audio = array("matthew","mark","luke","john","acts","romans","1corinthians","2corinthians","galatians","ephesians","philippians","colossians","1thessalonians","2thessalonians","1timothy","2timothy","titus","philemon","hebrews","james","1peter","2peter","1john","2john","3john","jude","revelation");
$libri_audio2 = array("01Matt","02Mark","03Luke","04John","05Acts","06Roma","07_1Co","08_2Co","09Gala","10Ephe","11Phil","12Colo","13_1Th","14_2Th","15_1Ti","16_2Ti","17Titu","18Phlm","19Hebr","20Jame","21_1Pe","22_2Pe","23_1Jn","24_2Jn","25_3Jn","26Jude","27_Reve");
$libri_audio4a = array("Mat","Mark","Luk","Joh","Acts","Rom","1Cor","2Cor","Gal","Eph","Philipp","Col","1Thess","2Thess","1Tim","2Tim","Titus","Philem","Hebr","James","1Peter","2Peter","1Joh","2Joh","3Joh","Jude","Rev");
$libri_audio4b = array("Mat","Mark","Luk","Joh","Acts","Rom","CorA","CorB","Gal","Eph","Philip","Col","ThessA","ThessB","TimA","TimB","Titus","Philem","Hebr","James","PetA","PetB","JohA","JohB","JohC","Jude","Rev");
global $libri_nomi;
global $libri_eng;
global $libri_es;
global $lin;
global $conn;

function Rif_A_Sql($rif, $lin, $tab="") {
  $sqlbrano = "";
  $brano3 = converti_rif($rif, $lin);
  for ($i=0; $i<strlen($brano3); $i+=6) {
      $lib0 = ord(substr($brano3,$i,1));
      $cap0 = ord(substr($brano3,$i+1,1));
      $vers0 = ord(substr($brano3,$i+2,1));
      $lib1 = ord(substr($brano3,$i+3,1));
      $cap1 = ord(substr($brano3,$i+4,1));
      $vers1 = ord(substr($brano3,$i+5,1));
      if ($i>0)
      	$sqlbrano .= " OR ";
      $sqlbrano .= "((".$tab."Libro>$lib0 OR (".$tab."Libro=$lib0 AND ".$tab."Capitolo>$cap0) OR (".$tab."Libro=$lib0 AND ".$tab."Capitolo=$cap0 AND ".$tab."Versetto>=$vers0)) AND (".$tab."Libro<$lib1 OR (".$tab."Libro=$lib1 AND ".$tab."Capitolo<$cap1) OR (".$tab."Libro=$lib1 AND ".$tab."Capitolo=$cap1 AND ".$tab."Versetto<=$vers1)))";
  }
  if ($sqlbrano!="")
  	$sqlbrano = " AND ($sqlbrano)";
  return $sqlbrano;
}

function RicercaFrase($frase, $vers="") {
global $lin;
  $versetti = array();
  $errfrase = "";

  $frase = str_replace("(", " (", $frase);
  $frase = str_replace(")", ") ", $frase);
  $frase = trim(strtr($frase, "!^", "|~"));
  while (strpos($frase, "  ")>0)
      $frase = str_replace("  ", " ", $frase);
  $frase = str_replace(" |", "|", $frase);
  $frase = str_replace("| ", "|", $frase);
  $frase = str_replace(" ~", "~", $frase);
  $frase = str_replace("~ ", "~", $frase);
  $frase = str_replace("( ", "(", $frase);
  $frase = str_replace(" )", ")", $frase);
  $frase = str_replace("[ ", "[", $frase);
  $frase = str_replace(" ]", "]", $frase);
  $frase = str_replace("/ ", "/", $frase);
  for ($i=1; $i<=9; $i++) {
  // non cancellare lo spazio fra due numeri (che succede con i numeri LN)
  	$p = strpos($frase, $i." ");
		while ($p!==false) {
			if ($frase[$p+2]<"0" || $frase[$p+2]>"9")
				$frase = substr($frase, 0, $p+1).substr($frase, $p+2);
  	$p = strpos($frase, $i." ", $p+1);
		}
  	$p = strpos($frase, " ".$i);
		while ($p!==false) {
			if ($frase[$p-1]<"0" || $frase[$p-1]>"9")
				$frase = substr($frase, 0, $p).substr($frase, $p+1);
  	$p = strpos($frase, " ".$i, $p+1);
		}
//      $frase = str_replace($i." ", $i, $frase);
//      $frase = str_replace(" ".$i, $i, $frase);
  }
  if (strlen($frase)==0)
  	if ($lin=="it")
      $errfrase = "L'espressione da ricercare &egrave; vuota.";
    else if ($lin=="es")
      $errfrase = "La expresi&oacute;n a buscar est&aacute; vac&iacute;a.";
    else
      $errfrase = "The expresion to search for is empty.";    
  elseif (!Lettera($frase[0]) && !Lettera(strtolower($frase[0])) && $frase[0]!="ê" && $frase[0]!="ô" && $frase[0]!="/" && $frase[0]!="#" && $frase[0]!="-" && $frase[0]!="*" && $frase[0]!="?" && $frase[0]!="("&& $frase[0]!="[")
  	if ($lin=="it")
      $errfrase = "Il primo carattere deve essere una lettera, o uno dei caratteri (, [, *, ?, /, #, /.";
  	else if ($lin=="es")
      $errfrase = "El primer caracter debe ser una letra, o uno de los siguientes caracteres (, [, *, ?, /, #, /.";
    else
      $errfrase = "The first character must be a letter, or one of the characters (, [, *, ?, /, #, /.";
  if (strpos($frase, "|~")>0 || strpos($frase, "~|")>0 || strpos($frase, "1~")>0 || strpos($frase, "~1")>0 || strpos($frase, "2~")>0 || strpos($frase, "~2")>0 || strpos($frase, "3~")>0 || strpos($frase, "~3")>0 || strpos($frase, "4~")>0 || strpos($frase, "~4")>0 || strpos($frase, "5~")>0 || strpos($frase, "~5")>0 || strpos($frase, "6~")>0 || strpos($frase, "~6")>0 || strpos($frase, "7~")>0 || strpos($frase, "~7")>0 || strpos($frase, "8~")>0 || strpos($frase, "~8")>0 || strpos($frase, "9~")>0 || strpos($frase, "~9")>0)
  	if ($lin=="it")
      $errfrase = "NON non pu&ograve; essere usato con OPPURE o un numero.";
  	else if ($lin=="es")
      $errfrase = "NOT no puede ser usado con OR o un n&uacute;mero.";
    else
    	$errfrase = "NOT can not be used with OR or a number.";

  if (strlen($errfrase)>0) {
  	if ($lin=="it")
      $syntaxErrorPhrase="Errore di sintasi nell'espressione da ricercare";
  	else if ($lin=="es")
      $syntaxErrorPhrase="Error de sintaxis en la expresi&oacute;n de busqueda";
    else
      $syntaxErrorPhrase="Syntax error in the search expression";
    echo "<p><strong>".$syntaxErrorPhrase.":</strong><br />".$errfrase."</p>";
    return $versetti;
  }

  $versetti = TrovaFrase($frase, $vers);
  reset($versetti);  
  return $versetti;
}

function TrovaFrase($frase, $vers="", $InFrase=0) {
global $lin;
global $conn;
  	$frase = str_replace("\'","'",$frase);
    $versetti = array();
    $errfrase = "";
    if (strlen($frase)==0)
        return $versetti;
    if (!Lettera($frase[0]) && !Lettera(strtolower($frase[0])) && $frase[0]=="ê" && $frase[0]=="ô" && $frase[0]!="/" && $frase[0]!="#" && $frase[0]!="-" && $frase[0]!="*" && $frase[0]!="?" && $frase[0]!="§" && $frase[0]!="(" && $frase[0]!="[")
	  	if ($lin=="it")
        $errfrase = "Il primo carattere dopo una parentesi deve essere una lettera, o uno dei caratteri (, [, *, ?, /, #, /, §.";
	  	else if ($lin=="es")
        $errfrase = "El primer caracter luego del par&eacute;ntesis debe ser una letra, o uno de los siguientes caracteres (, [, *, ?, /, #, /.";
      else
      	$errfrase = "The first character after parenthesis must be a letter, or one of the characters (, [, *, ?, /, #, /, §.";
    if ($InFrase>=1 && strpos($frase,"~")>0)
	  	if ($lin=="it")
        $errfrase = "NON non pu&ograve; essere usato entro le parentesi quadrate.";
	  	else if ($lin=="es")
        $errfrase = "NOT no puede ser usado dentro de corchetes.";
      else
      	$errfrase = "NOT can not be used inside square brackets.";
    if ($frase[0]=="[") {
        $i = 1;
        $nPar = 1;
        while ($i<strlen($frase)) {
            if ($frase[$i]=="[")
					  	if ($lin=="it")
    		        $errfrase = "Le parentesi quadrate non sono giuste.";
					  	else if ($lin=="es")
    		        $errfrase = "Los corchetes no son correctos.";
        		  else
          			$errfrase = "The square brackets are not correct.";            
            if ($frase[$i]=="]") $nPar--;
            if ($nPar==0) break;
            $i++;
        }
        if ($i == strlen($frase)) {
			  	if ($lin=="it")
    		  	$errfrase = "Le parentesi quadrate non sono giuste.";
			  	else if ($lin=="es")
    		  	$errfrase = "Los corchetes no son correctos.";
        	else
          	$errfrase = "The square brackets are not correct.";            
        }
        else {        
            $versetti = TrovaFrase(substr($frase,1,$i-1), $vers, 1);
            $frase = substr($frase,$i+1);
        }
    }
    else if ($frase[0]=="(") {
        $i = 1;
        $nPar = 1;
        while ($i<strlen($frase)) {
            if ($frase[$i]=="(") $nPar++;
            if ($frase[$i]==")") $nPar--;
            if ($nPar==0) break;
            $i++;
        }
        if ($i == strlen($frase)) {
			  	if ($lin=="it")
            $errfrase = "Le parentesi non sono giuste.";
			  	else if ($lin=="es")
            $errfrase = "Los par&eacute;ntesis no son correctos.";
          else
          	$errfrase = "The parenthesis are not correct.";
        }
        else {
            $versetti = TrovaFrase(substr($frase,1,$i-1), $vers, ($InFrase==0?0:2));
            $frase = substr($frase,$i+1);
        }
    }
    else {
        $i = 0;
        while ($i<strlen($frase) && (Lettera($frase[$i]) || Lettera(strtolower($frase[$i])) || $frase[$i]=="'" || $frase[$i]=="ê" || $frase[$i]=="ô" || ord($frase[$i])>=128 || $frase[$i]=="*" || $frase[$i]=="?" || $frase[$i]=="/" || $frase[$i]=="#" || $frase[$i]=="-" || $frase[$i]=="§"))
            $i++;
        if ($i>0 && $frase[$i-1]=="§") {
        	while ($i<strlen($frase) && ($frase[$i]=="." || ($frase[$i]>="0" && $frase[$i]<="9")))
        		$i++;
        }
        $versetti = TrovaParola(substr($frase,0,$i), $vers, $InFrase);
        $frase = substr($frase,$i);
    }
    while (strlen($frase)>0 && strlen($errfrase)==0) {
        $punteg = $frase[0];
        $frase = substr($frase,1);
		    $verspar = array();
        if ($frase[0]=="[") {
            $i = 1;
            $nPar = 1;
            while ($i<strlen($frase)) {
                if ($frase[$i]=="[")
    					  	if ($lin=="it")
        		        $errfrase = "Le parentesi quadrate non sono giuste.";
        			  	else if ($lin=="es")
                    $errfrase = "Los corchetes no son correctos.";
            		  else
              			$errfrase = "The square brackets are not correct.";            
                if ($frase[$i]=="]") $nPar--;
                if ($nPar==0) break;
                $i++;
            }
            if ($i == strlen($frase)) {
    			  	if ($lin=="it")
        		  	$errfrase = "Le parentesi quadrate non sono giuste.";
              else if ($lin=="es")
                $errfrase = "Los corchetes no son correctos.";
            	else
              	$errfrase = "The square brackets are not correct.";            
            }
            else {
                $verspar = TrovaFrase(substr($frase,1,$i-1), $vers, 1);                
                $frase = substr($frase,$i+1);
            }
        }
        else if ($frase[0]=="(") {
            $i = 1;
            $nPar = 1;
            while ($i<strlen($frase)) {
                if ($frase[$i]=="(") $nPar++;
                if ($frase[$i]==")") $nPar--;
                if ($nPar==0) break;
                $i++;
            }
            if ($i == strlen($frase)) {
					  	if ($lin=="it")
    		        $errfrase = "Le parentesi non sono giuste.";
    			  	else if ($lin=="es")
                $errfrase = "Los par&eacute;ntesis no son correctos.";
        		  else
          			$errfrase = "The parenthesis are not correct.";
              break;
            }
            $verspar = TrovaFrase(substr($frase,1,$i-1), $vers, ($InFrase==0?0:2));
            $frase = substr($frase,$i+1);
        }
        else {
            $i = 0;
            while ($i<strlen($frase) && (Lettera($frase[$i]) || Lettera(strtolower($frase[$i])) || $frase[$i]=="'" || $frase[$i]=="ê" || $frase[$i]=="ô" || ord($frase[$i])>=128 || $frase[$i]=="*" || $frase[$i]=="?" || $frase[$i]=="/" || $frase[$i]=="#" || $frase[$i]=="-"))
                $i++;
		        if ($i>0 && $frase[$i-1]=="§") {
    		    	while ($i<strlen($frase) && ($frase[$i]=="." || ($frase[$i]>="0" && $frase[$i]<="9")))
        				$i++;
        		}
            $verspar = TrovaParola(substr($frase,0,$i), $vers, $InFrase);
            $frase = substr($frase, $i);
        }
//        if ($punteg=="~")
//            $versetti = array_diff($versetti,$verspar);
        if ($InFrase>=1 && $punteg==" ")
					$punteg="1";
        switch ($punteg) {
        case " ":
            $versetti = array_intersect($versetti,$verspar);
            break;
        case "|":        
            $versetti = array_merge($versetti,$verspar);
            break;
        case "~":
            $versetti = array_diff($versetti,$verspar);
            break;
        case "1":
        case "2":
        case "3":
        case "4":
        case "5":
        case "6":
        case "7":
        case "8":
        case "9":
//            $versetti = array_unique($versetti);
//            $versetti = array_values($versetti);
            sort($versetti);
            sort($verspar);
            $versetti2 = array();
            $diff = ord($punteg)-ord("0");
            $diff_neg = ($InFrase>=1?0:$diff); // in una frase, la seconda parola deve essere dopo la prima
            for ($i=0; $i<count($versetti); $i++) {
                for ($j=0; $j<count($verspar); $j++) {
                    if ($verspar[$j]<$versetti[$i]-$diff_neg) {
                    }
                    elseif ($verspar[$j]>$versetti[$i]+$diff) {
                        break;
                    }
                    else {
                        $versetti2[] = $versetti[$i];
                        break;
                    }
                }
            }
            $versetti = array();
            $versetti = $versetti2;
            break;
        default:
            if ($lin=="it")
							$errfrase = "Non riconosco il carattere '$punteg' a questo punto nell'espressione.";
            else if ($lin=="es")
							$errfrase = "No entiendo el caracter '$punteg' en este punto en la expresi&oacute;n.";
						else
							$errfrase = "I do not understand the character '$punteg' at this point in the expression.";
            break;
        }
    }

    if (strlen($errfrase)>0) {
        if ($lin=="it")
        	echo "<p><strong>Errore di sintasi nell'espressione da ricercare:</strong><br />".$errfrase."</p>";
        else if ($lin=="es")
        	echo "<p><strong>Error de sintaxis en la expresi&oacute;n de busqueda:</strong><br/>".$errfrase."<p>";
        else
        	echo "<p><strong>Syntax error in the search expression:</strong><br />".$errfrase."</p>";
        $versetti = array();
    }
    else {
//        $versetti = array_unique($versetti);
//        $versetti = array_values($versetti);
        sort($versetti);
    }
    if ($InFrase==1) {
			$sql = "SELECT id_v FROM Chiave$vers WHERE id_c=-1";
			for ($i=0; $i<count($versetti); ++$i)
				$sql .= " OR id_c=$versetti[$i]";
			$sql .= " GROUP BY id_v";
      $versetti = array();
		  if ($ris=mysqli_query($conn, "$sql")) {
   			while ($row=mysqli_fetch_array ($ris))
      		$versetti[] = $row[0];
  		}
  		else {
      	 errore2("interrogazione database per ricerca $sql");
  		}
    }
    return $versetti;
}

function TrovaParola($TrovaVers_Esp, $vers="", $InFrase=0) {
global $conn;
  	$alfabeta_vecchia = array("ch","ph","ps","th","ê","ãª","Ãª","ô","ã´","Ã´","f","g","o","p","y","x","z"); // ãª = lower(eta), ã´ = lower(omega) in UTF-8
  	$alfabeta_nuova = array("X","V","Y","H","G","G","G","Z","Z","Z","V","C","P","Q","U","O","F");
  // abdeiklmnrst(u) rimangono uguale
  	$esp2 = str_replace("*", "%", $TrovaVers_Esp);
  	$esp2 = str_replace("?", "_", $esp2);
	$esp2 = str_replace("'", "\'", $esp2);
	$esp2 = str_replace("Â§", "§", $esp2);

	$sql_where = "";
	$ln_table = "";
	$ln_where = "";
	$gparoleusato = 0;
	$gvocabusato = 0;
	while (strlen($esp2)>0) {
		$i = 0;
		$primo_car = " ";
		$modificatore = "";
		do {
			$primo_car = $esp2[$i];
			$i += 1;
			if ($primo_car=="/" || $primo_car=="#" || $primo_car=="§")
				$modificatore .= $primo_car;
		} while (($primo_car=="/" || $primo_car=="#" || $primo_car=="§" || $primo_car=="_") && $i<strlen($esp2));
		$posProssimoMod = strpos($esp2, "/", $i);
		if ($posProssimoMod===false)
			$posProssimoMod = strlen($esp2);
		$pos2 = strpos($esp2, "#", $i);
		if ($pos2===false)
			$pos2 = strlen($esp2);
		if ($pos2<$posProssimoMod)
			$posProssimoMod = $pos2;
		$pos2 = strpos($esp2, "§", $i);
		if ($pos2===false)
			$pos2 = strlen($esp2);
		else {
			if ($pos2>0 && $esp2[$pos2-1]=="Î") // perché chi maiuscola è Î§; forse un problema se c'è un § dopo un chi maiuscola
				$pos2 = strlen($esp2);
			else if ($pos2>1 && $esp2[$pos2-2]=="á" && $esp2[$pos2-1]=="¼") // perché eta open tilde è á¼§
				$pos2 = strlen($esp2);							
		}
		if ($pos2<$posProssimoMod)
			$posProssimoMod = $pos2;
		$prossimaParola = substr($esp2, $i-1, $posProssimoMod - $i + 1);
		$esp2 = substr($esp2, $posProssimoMod);
		$parolaUtf8 = 0;
		if (ord($primo_car)>=192) {
    		if (strlen($prossimaParola)>=2) {
      			$primaLetteraUtf8 = substr($prossimaParola, 0, 2);
      			if ($primaLetteraUtf8!="ãª" && $primaLetteraUtf8!="Ãª" && $primaLetteraUtf8!="ã´" && $primaLetteraUtf8!="Ã´")
        			$parolaUtf8 = 1;
    		}
    		else
      			$parolaUtf8 = 1;
  		}
		if ($parolaUtf8==1) { // utf-8 unicode
			$prossimaParolaConv = $prossimaParola;   	
			$tabella_add = "";
		}
		else {
			$prossimaParolaConv = strtolower(str_replace($alfabeta_vecchia, $alfabeta_nuova, strtolower($prossimaParola)));
			$tabella_add = "PerOrdine";	
		}

		if ($modificatore=="/") {
			$sql_where .= " AND GVocab.Radice$tabella_add LIKE '$prossimaParolaConv'";
			$gvocabusato = 1;
//		echo "<p>$sql_where</p>";
//  	    echo "<p>Spiacente, la ricerca per radice &egrave; stata temporaneamente bloccata.<br />Sorry, the search for root word has been momentarily blocked.</p>";
		//return array();
		}

		else if ($modificatore=="#") {
			$sql_where .= " AND Chiave$vers.Grammatica LIKE '".EncodeGram($prossimaParola)."'";	
		}
		else if ($modificatore=="##")
			$sql_where .= " AND Chiave$vers.Grammatica LIKE '".ConvPersona($prossimaParola)."'";
		else if ($modificatore=="§") {
			$pos = strpos($prossimaParola, ".");
			if ($pos===false)
				$sql_where .= " AND LNParole.SezioneMaggiore=$prossimaParola";
			else
				$sql_where .= " AND LNParole.SezioneMaggiore=".substr($prossimaParola, 0, $pos)." AND LNParole.SezioneMinore=".substr($prossimaParola, $pos + 1);
			$ln_table = ", LNParole";
			$ln_where = " AND Chiave$vers.id_c=LNParole.id_c";
		}
		else {
			$sql_where .= " AND GParole$vers.Parola$tabella_add LIKE '$prossimaParolaConv'";
			$gparoleusato = 1;
		}
	}

	$sql_where = ($gvocabusato==1?" AND Chiave$vers.id_r=GVocab.id_r":"").($gparoleusato==1?" AND Chiave$vers.id_p=GParole$vers.id_p":"")."$ln_where$sql_where";
	$sql_where = substr($sql_where, 5);
	if ($InFrase>=1)
		$sql = "SELECT Chiave$vers.id_c";
	else
		$sql = "SELECT Chiave$vers.id_v";
	$sql .= " FROM Chiave$vers ".($gparoleusato==1 || $gvocabusato==1?", ":" ").($gparoleusato==1?"GParole$vers":"").($gparoleusato==1 && $gvocabusato==1?", ":" ").($gvocabusato==1?"GVocab ":"")."$ln_table WHERE $sql_where";
		
//  	echo "<p>$sql</p>";
	$VersParola = array();
//  $start = microtime();
	if ($ris=mysqli_query($conn, "$sql")) {
//$end = microtime();
//$parseTime = $end-$start;
//echo "<p>$parseTime $start $end</p>";
		while ($row=mysqli_fetch_array ($ris))
			$VersParola[] = $row[0];
  	}
	else {
		errore2("interrogazione database per ricerca $sql");
	}

	return $VersParola;
}

function EncodeGram($g) {
	if ($g[0]=="#")
		return str_replace("-","_",substr($g,1));
	$g = strtolower($g);
	$gram = "__________";
	$tok = strtok($g, "-");
	while ($tok !== false) {
		if (substr($tok,0,2)=="ag" || substr($tok,0,3)=="adj") $gram=substr_replace($gram,"A-",0,2);
		if (substr($tok,0,8)=="congiunz" || substr($tok,0,4)=="conj") $gram=substr_replace($gram,"C-",0,2);
		if (substr($tok,0,2)=="av" || substr($tok,0,3)=="adv") $gram=substr_replace($gram,"D-",0,2);
		if (substr($tok,0,3)=="int") $gram=substr_replace($gram,"I-",0,2);
		if (substr($tok,0,2)=="so" || substr($tok,0,3)=="nou") $gram=substr_replace($gram,"N-",0,2);
		if (substr($tok,0,4)=="prep") $gram=substr_replace($gram,"P-",0,2);
		if (substr($tok,0,2)=="ar") $gram=substr_replace($gram,"RA",0,2);
		if (substr($tok,0,2)=="pd" || substr($tok,0,2)=="dp") $gram=substr_replace($gram,"RD",0,2);
		if (substr($tok,0,3)=="pin" || substr($tok,0,2)=="ip") $gram=substr_replace($gram,"RI",0,2);
		if (substr($tok,0,2)=="pp") $gram=substr_replace($gram,"RP",0,2);
		if (substr($tok,0,4)=="prel" || substr($tok,0,2)=="rp") $gram=substr_replace($gram,"RR",0,2);
		if (substr($tok,0,2)=="ve") $gram=substr_replace($gram,"V-",0,2);
		if (substr($tok,0,7)=="partice" || substr($tok,0,7)=="particl") $gram=substr_replace($gram,"X-",0,2);
		if (substr($tok,0,2)=="fi" || substr($tok,0,3)=="pri") $gram=substr_replace($gram,"1",2,1);
		if (substr($tok,0,2)=="se") $gram=substr_replace($gram,"2",2,1);
		if (substr($tok,0,1)=="t") $gram=substr_replace($gram,"3",2,1);
		if (substr($tok,0,2)=="ao") $gram=substr_replace($gram,"A",3,1);
		if (substr($tok,0,2)=="fu") $gram=substr_replace($gram,"F",3,1);
		if (substr($tok,0,6)=="imperf") $gram=substr_replace($gram,"I",3,1);
		if (substr($tok,0,4)=="pres") $gram=substr_replace($gram,"P",3,1);
		if (substr($tok,0,2)=="pe") $gram=substr_replace($gram,"X",3,1);
		if (substr($tok,0,3)=="piu" || substr($tok,0,4)=="plup") $gram=substr_replace($gram,"Y",3,1);
		if (substr($tok,0,2)=="at" || substr($tok,0,3)=="act") $gram=substr_replace($gram,"A",4,1);
		if (substr($tok,0,2)=="me" || substr($tok,0,2)=="mi") $gram=substr_replace($gram,"M",4,1);
		if (substr($tok,0,3)=="pas") $gram=substr_replace($gram,"P",4,1);
		if (substr($tok,0,6)=="impera") $gram=substr_replace($gram,"D",5,1);
		if (substr($tok,0,3)=="ind") $gram=substr_replace($gram,"I",5,1);
		if (substr($tok,0,3)=="inf") $gram=substr_replace($gram,"N",5,1);
		if (substr($tok,0,1)=="o") $gram=substr_replace($gram,"O",5,1);
		if (substr($tok,0,7)=="partici") $gram=substr_replace($gram,"P",5,1);
		if (substr($tok,0,8)=="congiunt" || substr($tok,0,3)=="sub") $gram=substr_replace($gram,"S",5,1);
		if (substr($tok,0,3)=="acc") $gram=substr_replace($gram,"A",6,1);
		if (substr($tok,0,2)=="da") $gram=substr_replace($gram,"D",6,1);
		if (substr($tok,0,1)=="g") $gram=substr_replace($gram,"G",6,1);
		if (substr($tok,0,3)=="nom") $gram=substr_replace($gram,"N",6,1);
		if (substr($tok,0,2)=="vo") $gram=substr_replace($gram,"V",6,1);
		if (substr($tok,0,4)=="plur") $gram=substr_replace($gram,"P",7,1);
		if (substr($tok,0,2)=="si") $gram=substr_replace($gram,"S",7,1);
		if (substr($tok,0,2)=="fe") $gram=substr_replace($gram,"F",8,1);
		if (substr($tok,0,2)=="ma") $gram=substr_replace($gram,"M",8,1);
		if (substr($tok,0,2)=="ne") $gram=substr_replace($gram,"N",8,1);
		if (substr($tok,0,3)=="com") $gram=substr_replace($gram,"C",9,1);
		if (substr($tok,0,3)=="sup") $gram=substr_replace($gram,"S",9,1);
  	$tok = strtok("-");
	}
	return $gram;
}

function CreaTestoGreco($r1, $cap1, $cap2, $sqlcv, $campo) {
global $libri_nomi;
global $conn;
   $sql = "SELECT $campo, Capitolo, Versetto FROM GVersetti WHERE Libro=$r1 AND ".$sqlcv." ORDER BY Capitolo ASC, Versetto ASC";
   $brano = "";
   if ($ris=mysqli_query($conn, "$sql")) {/*
        // modo diverso quando non c'erano paragrafi (cioè punteggiatura)
      while ($row=mysqli_fetch_array ($ris)) {
        if ($solo1versetto==1)
           echo "<p><b>".$row["Capitolo"].":".$row["Versetto"]."</b> ";
        echo "<span class=\"uni\">".$row["Testo"]."</span></p>\n";
      }*/
      $PrimaRiga = 1;
      while ($row=mysqli_fetch_array ($ris)) {
            $testo_versetto = $row[0];
            if (strlen($brano)>0)
               $brano .= " ";
            if (mysqli_num_rows($ris)>1) {
               if ($row["Versetto"]==1 && $brano!="" && $cap1!=$cap2)
                  $brano .= "</p><p>";
               $brano .= "<strong>";
               if ($campo=="Inter")
               		$brano .= "<div class=\"i\"><p class=\"ig\">";
               if (($row["Versetto"]==1 || $PrimaRiga==1) && $cap1!=$cap2)
                  $brano .= $row["Capitolo"].":";
               $brano .= $row["Versetto"]."</strong>&nbsp;";
               if ($campo=="Inter")
               		$brano .= "</p><p class=\"ii\"></p></div>";
            }
            $brano .= "<span class=\"uni\">".$testo_versetto."</span>\n";
            $PrimaRiga = 0;
      }
      $brano = "<p>".$brano."</p>\n";
  }
  else {
    errore2("interrogazione database per visualizzare il testo: $sql");
  }
  return $brano;
}

function TraduciMss($mss) {
global $conn;
$mss2 = mysqli_real_escape_string($conn, htmlspecialchars($mss, ENT_QUOTES));
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
       $n_mss = $row["id_mss"];}
}
else {
     errore2("interrogazione database per trovare il manuscritto $mss($mss2)");
}
if ($mss=="text" || $mss=="testo") $n_mss = -1;
return $n_mss;
}

function MostraVarianti($ris, $ord, $lin, $varianti, $msstt, $wh, $tisch, $biz, $inter, $allusioni, $direzione="v") {
global $libri_nomi;
global $libri_eng;
global $libri_es;
      $nDiff = mysqli_num_rows($ris);
      if ($lin=="it")
  	    echo "<p><b>$nDiff</b> versetti sono stati trovati.</p>";
      else if ($lin=="es")
  	    echo "<p><b>$nDiff</b> versículos encontrados.</p>";
    	else
      	echo "<p><b>$nDiff</b> verses were found.</p>";
      if ($nDiff==0 || $nDiff>=300) {
      //
      }
      else if ($nDiff<30) {
      	$rif = "";
      	while ($row=mysqli_fetch_array($ris))
      		$rif .= $row["Libro"]." ".$row["Capitolo"].":".$row["Versetto"]."|";
//      	$rif = substr($rif, 0, -1); // togliere ultimo |
      	MostraBrano($rif, $ord, $direzione, $varianti, $msstt, $wh, $tisch, $biz, $inter, $allusioni);
      }
      else if ($nDiff<300) {
           echo "<p>";
           while ($row=mysqli_fetch_array($ris)) {
              if ($lin=="it")
                 echo '<a href="?rif1='.$row["Libro"].'&rif2='.$row["Capitolo"].'%3A'.$row["Versetto"].'">'.$libri_nomi[$row["Libro"]]." ".$row["Capitolo"].":".$row["Versetto"]."</a>";
              else if ($lin=="es")
                 echo '<a href="?rif1='.$row["Libro"].'&rif2='.$row["Capitolo"].'%3A'.$row["Versetto"].'">'.$libri_es[$row["Libro"]]." ".$row["Capitolo"].":".$row["Versetto"]."</a>";
              else
                echo '<a href="?rif1='.$row["Libro"].'&rif2='.$row["Capitolo"].'%3A'.$row["Versetto"].'">'.$libri_eng[$row["Libro"]]." ".$row["Capitolo"].":".$row["Versetto"]."</a>";                 
							if ($row["Count"]>1)
								echo " (x".$row["Count"].")";
							echo "<br />";
           }
           echo "</p>";
      }
}

function MostraBrano($rif_brano, $ord, $direzione="v", $varianti="n", $msstt="s", $wh="n", $tisch="n", $biz="n", $inter="n", $allusioni="n", $count=1) {
global $libri_nomi;
global $libri_eng;
global $libri_es;
global $libri_audio;
global $libri_audio2;
global $libri_audio4a;
global $libri_audio4b;
global $lin;
global $conn;
	 $rif = "";
	 $tok = strtok($rif_brano, "|");
	 while ($tok!==false) {
	 		$r1 = (int)(substr($tok, 0, strpos($tok," ")));
	 		$r2 = substr($tok, strpos($tok, " ")+1);
	    	$rif .= converti_rif($libri_nomi[$r1].$r2);
	 		$tok = strtok("|");
	 }
   if (strlen($rif)==0 && $varianti!="x") {
      if ($lin=="it")
         echo "<p>Non ho capito il riferimento $rif_brano.</p>";
      else if ($lin=="es")
         echo "<p>No pude entender la referencia $rif_brano.</</p>";
      else
          echo "<p>I could not understand the reference $rif_brano.</p>";
//      $rif=chr(0)+chr(0)+chr(0)+chr(0)+chr(0)+chr(0);
   }
  
	$larghezza_col = 100;
	if ($direzione=="o" && $varianti!="x") {
	  $colonne = 1;
		echo "<table style=\"table-layout: fixed\"><tr>";
		echo "<th>SBL</th>";
    if ($wh=="s") {echo "<th>WH</th>"; $colonne += 1;}
    if ($tisch=="s") {echo "<th>Tischendorf</th>"; $colonne += 1;}
    if ($biz=="s") {if ($lin=="it" || $lin=="es") echo "<th>Bizantino</th>"; else echo "<th>Byzantine</th>"; $colonne += 1;}
    if ($inter=="s") {if ($lin=="it") echo "<th>Interlineare</th>"; else if ($lin=="es") echo "<th>Interlineal</th>"; else echo "<th>Interlinear</th>"; $colonne += 1;}
    if ($varianti=="s") {$colonne += 1; if ($lin=="it") echo "<th>Varianti</th>"; else if ($lin=="es") echo "<th>Variantes</th>"; else echo "<th>Variants</th>";}
    if ($allusioni=="s") {$colonne += 1; if ($lin=="it") echo "<th>Allusioni</th>"; else if ($lin=="es") echo "<th>Alusiones</th>"; else echo "<th>Allusions</th>";}
		echo "</tr>";
		$larghezza_col = 100 / $colonne;
	}
	for ($j=0; $j<strlen($rif)/6; $j++) {
	  $r1=ord($rif[$j*6]);
      $cap1=ord($rif[1+$j*6]);
      $vers1=ord($rif[2+$j*6]);
      $cap2=ord($rif[4+$j*6]);
      $vers2=ord($rif[5+$j*6]);
      if ($cap2!=$cap1) { //perché non più di un capitolo
      	$cap2 = $cap1; $vers2 = 177;
      }
      $titrif = "<h3>";
      if ($lin=="it")
        $titrif .= $libri_nomi[$r1]." ";
      else if ($lin=="es")
        $titrif .= $libri_es[$r1]." ";
      else
        $titrif .= $libri_eng[$r1]." ";
      $titrif_capvers = "";
      $solo1versetto = 0;
      if ($cap1!=$cap2 || $vers1!=$vers2) {
        $solo1versetto = 1;
        if ($vers2==177) {
              if ($cap1==$cap2)
                 $titrif_capvers .= $cap1;
              else {
                   if ($cap2!=151)
                   $titrif_capvers .= "$cap1-$cap2";
              }
        }
        else {
          if ($cap1==$cap2)
                $titrif_capvers .= "$cap1:$vers1-$vers2";          
          else
                $titrif_capvers .= "$cap1:$vers1-$cap2:$vers2";
        }
      }
      else
         $titrif_capvers .= $cap1.":".$vers1;
      $titrif .= $titrif_capvers;
      if ($count>1)
      	$titrif .= " (x".$count.")";
      $cap1_2cifre = ($cap1>9?$cap1:"0".$cap1);
      $abb_audio = substr($libri_audio[$r1-47],0,3);
      switch ($r1-47) {
      case 1:
        $abb_audio = "mrk";
        break;
      case 17:
        $abb_audio = "phe";
        break;
      }
      $nomefile_audio = $abb_audio.$cap1_2cifre."g";
      $cap_audio2 = $cap1_2cifre;
      if ($r1==64 || $r1==70 || $r1==71 || $r1==72)
        $cap_audio2 = "00";
      $titrif .= " (<a href=\"http://prototypes.openscriptures.org/manuscript-comparator/?passage=$libri_eng[$r1]+$titrif_capvers&view=parallel&ins[]=1&ins[]=2&ins[]=3&ins[]=4&del[]=5&del[]=6&del[]=7&strongs=1\">Manuscript Comparator</a>)";
      $libri_audio3 = $libri_audio[$r1-47]."/";
      $inizio = ($r1-46>9?$r1-46:"0".($r1-46))."%20";
      if (substr($libri_audio3,0,1)>="1" && substr($libri_audio3,0,1)<="3") {
        $inizio .= substr($libri_audio3,0,1)."%20";
        $libri_audio3 = substr($libri_audio3,1);
      }
      $libri_audio3 = $inizio.ucfirst($libri_audio3);
      $numeroLib_audio4 = $r1-46;
      if ($numeroLib_audio4==25) $numeroLib_audio4=24;
      if ($numeroLib_audio4==26) $numeroLib_audio4=25;

      $titrif .= " (<a href=\"http://www.helding.net/greeklatinaudio/greek/".$libri_audio[$r1-47]."/".$nomefile_audio.".mp3\" target=\"_blank\">Audio</a> ";
      if ($lin=="it"||$lin=="es") $titrif .= "Biz"; else $titrif .="Byz";
      $titrif .= "</a>)";
      
      if ($cap1==$cap2)
        $sqlcv = "Capitolo=$cap1 AND Versetto>=$vers1 AND Versetto<=$vers2";
      else
         $sqlcv = "((Capitolo=$cap1 AND Versetto>=$vers1) OR (Capitolo>$cap1 AND Capitolo<$cap2) OR (Capitolo=$cap2 AND Versetto<=$vers2))";

	  if ($varianti!="x") {
   		if ($direzione=="o") {
			echo "<tr><td colspan=$colonne>$titrif</h3></td></tr>\n";
			echo "<tr valign=\"top\"><td style=\"width: ".$larghezza_col."%\">";
		}
		else {
	    	echo $titrif."</h3>\n";			
		}
	  
      $brano = CreaTestoGreco($r1, $cap1, $cap2, $sqlcv, "Testo");
      echo "$brano";
   		if ($direzione=="o") echo "</td>\n";      

      if ($wh=="s") {
   			 if ($direzione=="o")
						echo "<td style=\"width: ".$larghezza_col."%\">";
				 else
         		if ($lin=="it") echo "<h4>Westcott e Hort</h4><p>"; else echo "<h4>Westcott and Hort</h4><p>";
         $brano = CreaTestoGreco($r1, $cap1, $cap2, $sqlcv, "WH");
         echo $brano;
   			 if ($direzione=="o") echo "</td>\n";
      }
  
      if ($tisch=="s") {
   			 if ($direzione=="o")
						echo "<td style=\"width: ".$larghezza_col."%\">";
				 else
         		echo "<h4>Tischendorf</h4><p>";
         $brano = CreaTestoGreco($r1, $cap1, $cap2, $sqlcv, "Tisch");
         echo $brano;
   			 if ($direzione=="o") echo "</td>\n";         
      }
  
      if ($biz=="s") {
   			 if ($direzione=="o")
						echo "<td style=\"width: ".$larghezza_col."%\">";
				 else
						if ($lin=="it" || $lin=="es") echo "<h4>Bizantino</h4><p>"; else echo "<h4>Byzantine</h4><p>";
         $brano = CreaTestoGreco($r1, $cap1, $cap2, $sqlcv, "Biz");
         echo $brano;
   			 if ($direzione=="o") echo "</td>\n";         
      }

      if ($inter=="s") {
   			 if ($direzione=="o")
						echo "<td style=\"width: ".$larghezza_col."%\">";
				 else
						if ($lin=="it") echo "<h4>Interlineare</h4><p>"; else if ($lin=="es") echo "<h4>Interlineal</h4>"; else echo "<h4>Interlinear</h4><p>";
         $brano = CreaTestoGreco($r1, $cap1, $cap2, $sqlcv, "Inter");
         echo $brano;
   			 if ($direzione=="o")
						echo "</td>\n";
      }
	  } // if ($varianti!="x")

   if ($varianti=="s" || $varianti=="x") {
   		if ($direzione=="o" && $varianti!="x") echo "<td style=\"width: ".$larghezza_col."%\"><p>";   
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
       $tipo_prec = -1; $cap_prec = 0; $vers_prec = 0;
       $libro_nel_NT = $r1 - 46;
       if ($ris2=mysqli_query($conn, "$sql")) {
          while ($row2=mysqli_fetch_array($ris2)) {
            if ($row2["VarInVers"]!=$iVarInVers || $row2["Capitolo"]!=$cap_prec || $row2["Versetto"]!=$vers_prec) {
               if ($iVarInVers!=-1) {
			      if ($varianti!="x")
                    echo $sSos."</p>\n";
				  else
				    echo $sSos."\n      </alternative>\n      </variant>\n";
                  $sSos = "";
                  $tipo_prec = -1;
               }
               else
               	if ($direzione!="o" && $varianti!="x") {
                  if ($lin=="it") echo "<h4>Varianti</h4><p>"; else if ($lin=="es") echo "<h4>Lecturas variantes</h4><p>"; else echo "<h4>Variant readings</h4><p>";
				}
               if ($row2["Capitolo"]!=$cap_prec || $row2["Versetto"]!=$vers_prec) {
//                  if ($solo1versetto==1) {
// bisogna mostrare sempre il riferimento, per aver il link a Munster
                    if ($cap_prec>0) echo $varianti!="x"?"<p>":"    </variants>\n    </verse>\n";
					if ($varianti!="x") {
                      echo "<b>".$row2["Capitolo"].":".$row2["Versetto"]."</b> (<a href=\"http://nttranscripts.uni-muenster.de/AnaServer?NTtranscripts+0+wordcoll.anv+book=$libro_nel_NT&chapter=".$row2["Capitolo"]."&verse=".$row2["Versetto"]."\">M&uuml;nster</a>)";
                      //if ($libro_nel_NT==4) // solo in Giovanni
                        //echo " (<a href=\"http://arts-itsee.bham.ac.uk/AnaServer?majuscule+0+appframe.anv&chapter=".$row2["Capitolo"]."&verse=".$row2["Versetto"]."\">IGNT Majuscules</a>)";
                      echo "<br />\n";											
					}
					else {
					  echo "    <verse chapterNumber=\"".$row2["Capitolo"]."\" verseNumber=\"".$row2["Versetto"]."\">\n    <variants>\n    <variant>\n";
					}
//                  }
                  $cap_prec = $row2["Capitolo"];
                  $vers_prec = $row2["Versetto"];
               }
               else
                   echo $varianti!="x"?"<p>":"    <variant>\n";
               $iVarInVers = $row2["VarInVers"];
            }
            if ($row2["id_var"]!=$iVar) {
               $iVar = $row2["id_var"];
               if ($sSos!="")
                  echo $varianti!="x"?$sSos."<br />\n":"      $sSos\n      </alternative>\n";
               if ($row2["VarInVar"]>=200) {
			     if ($varianti!="x")
                   $sSos = "<i>$row2[4]</i><a href=\"".$row2["VarCommenti"]."\" target=\"_blank\">A Student's Guide to New Testament Textual Variants</a>";
				 else
				   $sSos = "";
               }
               else {
			     if ($varianti!="x")
                   $sSos = $row2[4]."]";
				 else
				   $sSos = "      <alternative>\n        <text>".strip_tags($row2[4])."</text>\n"; // per togliere <span>
                 $VarComm = $row2["VarCommenti"]; // qqq
                 if ($VarComm=="p") {
					if ($varianti!="x")				 	
                   		if ($lin=="it") $sSos.=" (<i>vedi brano parallelo</i>)"; else if ($lin=="es") $sSos.=" (<i>ver pasaje paralelo</i>)"; else $sSos.=" (<i>see parallel passage)</i>";
					else
						echo "        <see>See parallel passage</see>\n"; // qqq da fare controllare
				 }
                 elseif ($VarComm!="") {
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
				   if ($varianti!="x") {
	                   $sSos.=" (<i>";
	                   if ($lin=="it")
	                     $sSos .= "vedi";
	                   else if ($lin=="es")
	                     $sSos .= "ver";
	                   else
	                     $sSos .= "see";
	                   $sSos.="</i> ".$rif_nuovo.")";
				   }
				   else {
				   		echo "        <see>See $rif_nuovo</see>\n"; // qqq da fare controllare - ma usa nome libro in lingua non in inglese
				   }
                 } // elseif ($VarComm!="") {
                 $tipo_prec = -1;
               }
            }
            if ($ord==3 && $row2["Tipo_db"]!=$tipo_prec && $row2[2]!="") { // non indicare il tipo del dummy
               switch ($row2["Tipo_db"]) {
               case 50:
                    if ($lin=="it") $tipovar = "Ales"; else if ($lin=="es") $tipovar = "Alej"; else $tipovar = "Alex";
                    break;
               case 60:
                    if ($lin=="it") $tipovar = "Ales/Ces"; else if ($lin=="es") $tipovar = "Alej/Ces"; else $tipovar = "Alex/C&aelig;s";
                    break;
               case 70:
                    if ($lin=="it") $tipovar = "Ales/Occ"; else if ($lin=="es") $tipovar = "Alej/Occ"; else $tipovar = "Alex/West";
                    break;
               case 80:
                    if ($lin=="it") $tipovar = "Ales/Biz"; else if ($lin=="es") $tipovar = "Alej/Biz"; else $tipovar = "Alex/Byz";
                    break;
               case 100:
                    if ($lin=="it") $tipovar = "Ces"; else if ($lin=="es") $tipovar = "Ces"; else $tipovar = "C&aelig;s";
                    break;
               case 120:
                    if ($lin=="it") $tipovar = "Occ/Biz"; else if ($lin=="es") $tipovar = "Occ/Biz"; else $tipovar = "West/Byz";
                    break;
               case 130:
                    if ($lin=="it") $tipovar = "Ces/Biz"; else if ($lin=="es") $tipovar = "Ces/Biz"; else $tipovar = "C&aelig;s/Byz";
                    break;
               case 150:
                    if ($lin=="it") $tipovar = "Occ"; else if ($lin=="es") $tipovar = "Occ"; else $tipovar = "West";
                    break;
               case 200:
                    if ($lin=="it") $tipovar = "Biz"; else if ($lin=="es") $tipovar = "Biz"; else $tipovar = "Byz";
                    break;
               case 250:
                    if ($lin=="it") $tipovar = "Misto"; else if ($lin=="es") $tipovar = "Otros"; else $tipovar = "Mixed";
                    break;
               case 255:
                    $tipovar = "?";
                    break;
               default:
                    $tipovar = "X";
               }
               if ($varianti!="x")
			     $sSos .= " <b>".$tipovar.":</b>&nbsp;";
               $tipo_prec = $row2["Tipo_db"];
            }

            if (substr($sSos,strlen($sSos)-1,1)=="]")
            	$sSos .= "&nbsp;";
            if (substr($sSos,strlen($sSos)-6,6)!="&nbsp;")
               $sSos .= " ";

            $ms_mod = $row2[2];
            $ms_mod2 = "";
            $modifiche = $row2["Modifiche"];
            while (strpos($modifiche, "!")!==false) {
               $modifica_2lingue = substr($modifiche, 0, strpos($modifiche, "!"));
               if ($modifica_2lingue[strlen($modifica_2lingue)-1]=="<")
                  if ($lin!="it")
                     $modifica_2lingue = "";
                  else
                      $modifica_2lingue = substr($modifiche, 0, strlen($modifica_2lingue)-1);
               elseif ($modifica_2lingue[strlen($modifica_2lingue)-1]==">")
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
            if (strpos($modifiche, "3")!==false)
               $ms_mod .= "<sup>3</sup>";
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
            if (strpos($modifiche, "b")!==false)
               $ms_mod .= "<sup>arab</sup>";
            if (strpos($modifiche, "z")!==false)
               $ms_mod .= "<sup>arm</sup>";
            if (strpos($modifiche, "a")!==false)
               $ms_mod .= "<sup>slav</sup>";
            if (strpos($modifiche, "i")!==false)
               if ($lin=="it" || $lin=="es") $ms_mod .= "<sup>sir</sup>"; else $ms_mod .= "<sup>syr</sup>";
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
            if (strpos($modifiche, "¢")!==false)
               $ms_mod .= "<sup>(c1)</sup>";
            if (strpos($modifiche, "ç")!==false)
               $ms_mod .= "<sup>(c2)</sup>";
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
            
			if ($varianti!="x") {
	            $titolo = $row2[0]!=""?($lin=="it"?"Data=":($lin=="es"?"Fecha=":"Date=")).$row2[0]:"";
	            if ($row2[0]!="" && $row2[1]!="") $titolo.="; ";
	            if ($row2[1]!="") $titolo.=($lin=="it"?"Tipo di testo=":($lin=="es"?"Tipo de texto=":"Text type=")).$row2[1];
	            if ($msstt=="n")
	              $sSos .= '<b>'.$ms_mod.$ms_mod2.'</b> {'.$titolo.'}';
	            else
	              $sSos .= '<span title="'.$titolo.'">'.$ms_mod.$ms_mod2."</span>";
			}
			else { // qqq $mod_mod(2), sSos with (,[,[[
				echo "        <witness>\n        <name>".$ms_mod.$ms_mod2."</name>\n        <date>$row2[0]</date>\n        <type>$row2[1]</type>\n        </witness>\n";
			} // type has to be in English, mss name has span

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
            echo $sSos."</p>\n";
       } // if ($ris2=mysqli_query($conn, "$sql")) {
       else {
            errore2("interrogazione database per visualizzare le varianti di $libri_nomi[$r1] $cap1:$vers1-$cap2:$vers2");
       }
   		if ($direzione=="o") echo "</td>";       
    } // if ($varianti=="s")

    if ($allusioni=="s") {
   		 if ($direzione=="o")
				echo "<td style=\"width: ".$larghezza_col."%\"><p>";
			 else
       	if ($lin=="it") echo "<h4>Allusioni</h4><p>"; else if ($lin=="es") echo "<h4>Alusiones</h4><p>"; else echo "<h4>Allusions</h4><p>";
       $cap_prec=0; $vers_prec=0;
       $sql = "SELECT Capitolo, Versetto, Titolo, Citazione, Indirizzo FROM Allusioni WHERE Libro=$r1 AND ".$sqlcv." ORDER BY Capitolo ASC, Versetto ASC";
       if ($ris=mysqli_query($conn,"$sql")) {
       		if (mysqli_num_rows($ris)==0)
       			if ($lin=="it") echo "Nessuna"; else if ($lin=="es") echo "Ninguna"; else echo "None";
          while ($row=mysqli_fetch_array ($ris)) {
          	if ($row["Capitolo"]!=$cap_prec || $row["Versetto"]!=$vers_prec) {
            	if ($solo1versetto==1) {
              	if ($cap_prec>0) echo "</p>\n<p>";
                echo "<b>".$row["Capitolo"].":".$row["Versetto"]."</b><br />\n";
              }
            }
          	else
          		echo "<br />";
          	echo "<a href=\"".$row["Indirizzo"]."\">".$row["Titolo"]."</a>: ".$row["Citazione"];
	          $cap_prec = $row["Capitolo"];
  	        $vers_prec = $row["Versetto"];
          }
          echo "</p>\n";
      }
      else {
        errore2("interrogazione database per visualizzare le allusioni di $libri_nomi[$r1] $cap1:$vers1-$cap2:$vers2");
      }
   		if ($direzione=="o") echo "</td>";      
    }
   	if ($direzione=="o") echo "</tr>";    
  } // for ($j=0; $j<strlen($rif)/6; $j++) {
  if ($direzione=="o") echo "</table>";
  if ($varianti=="x") echo "    </variants>\n    </verse>\n";
}

if ($xml_out!=0) {
	echo "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
	echo "<passage>\n";
	echo "  <reference>".$libri_eng[$rif1]." ".$rif2."</reference>\n  <verses>\n";
	if ($rif1>0)
		MostraBrano($rif1." ".$rif2."|", $ord, $direzione, "x", $msstt);
	echo "  </verses>\n</passage>";
}
else {
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="https://www.w3.org/1999/xhtml" lang="<?if ($lin=="it" || $lin=="es") echo $lin; else echo "en";?>">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<?if (empty($bConfMss) && empty($bTrovaPar) && empty($bTrovaVers)) {?>
<title><?if ($lin=="it") echo "La Sacra Bibbia - Nuovo Testamento greco"; else if ($lin=="es") echo "Nuevo Testamento Griego"; else echo "New Testament Greek";?></title>
<meta name="description" content="<?if ($lin=="it") echo "Le principali letture varianti dei manoscritti del Nuovo Testamento, per la critica testuale; si possono fare ricerche in diversi testi greci"; else echo "The main variant readings of the manuscripts of the New Testament, for textual criticism; there are various fully searchable Greek texts";?>" />
<meta name="keywords" content="<?if ($lin=="it") echo "Nuovo Testamento,greco,Nuovo Testamento greco,bibbia,critica testuale,manoscritti,manoscritto,varianti,variante,Nestle,Aland,Nestle Aland,United Bible Societies,Westcott,Hort,Westcott e Hort,Tischendorf,Byzantino,interlineare"; else echo "New Testament,Greek,Greek New Testament,Bible,text criticism,manuscript,manuscripts,variant,variants,Nestle,Aland,Nestle Aland,United Bible Societies,Westcott,Hort,Westcott and Hort,Tischendorf,Bizantino,interlinear";?>" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<?}else{?>
<title><?if ($lin=="it") echo "Ricerca nel Nuovo Testamento"; else if ($lin=="es") echo "Buscar en el Nuevo Testamento"; else echo "Search in the New Testament";?></title>
<?}?>
<link rel="shortcut icon" type="image/png" href="/favicon.png" />
<link rel="stylesheet" href="/stili/stilebase_old.css" type="text/css" />
<style type="text/css">
.uni {
font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,OldStandard,Palatino Linotype,Tahoma;
font-size: small;
}
.uni a:visited {text-decoration: none; color:black;}
.uni a:link {text-decoration: none; color:black;}
.uni a:hover {text-decoration: underline;}
.uni a:active {text-decoration: underline;}
div.i {
 float: left;
 margin-bottom: 1em;
 color: black;
}
p.ig {
 font-family: <?if ($fontuni!="") echo $fontuni.","?>Galatia SIL,Gentium,Cardo,Oxoniensis,Vusillus Old Face,Athena,Caslon,Hindsight Unicode,Chrysanthi Unicode,Monospace,Palatino Linotype,Tahoma;
 font-size: small;
 margin: 0em;
 padding: 0em 0.5em;
}
p.ii {
 font-family: Verdana, Arial, Helvetica, sans-serif;
 font-size: x-small;
 margin: 0em;
 padding: 0em 0.5em;
}
</style>
<script language="JavaScript" type="text/javascript">
function cv(e,p,g) {
var targ;
if (!e) var e = window.event;
if (e.target)
	targ = e.target;
else
	if (e.srcElement) targ = e.srcElement;
if (targ.nodeType == 3) // Safari bug
   targ = targ.parentNode;

switch (g.substring(0,2)) {
case "A-":
   g2="<?if($lin=="it")echo "aggettivo";else if ($lin=="es") echo "adjectivo:"; else echo "adjective:";?>";
   break;
case "C-":
   g2="<?if($lin=="it")echo "congiunzione";else if ($lin=="es") echo "conjunci&ograve;n"; else echo "conjunction";?>";
   break;
case "D-":
   g2="<?if($lin=="it")echo "avverbio";else if ($lin=="es") echo "adverbio:"; else echo "adverb";?>";
   break;
case "I-":
   g2="<?if($lin=="it")echo "interiezione";else if ($lin=="es") echo "interjeci&ograve;n"; else echo "interjection";?>";
   break;
case "N-":
   g2="<?if($lin=="it")echo "sostantivo";else if ($lin=="es") echo "sustantivo:"; else echo "noun:";?>";
   break;
case "P-":
   g2="<?if($lin=="it")echo "preposizione";else if ($lin=="es") echo "preposici&ograve;n"; else echo "preposition";?>";
   break;
case "RA":
   g2="<?if($lin=="it")echo "articolo";else if ($lin=="es") echo "articulo:"; else echo "article:";?>";
   break;
case "RD":
   g2="<?if($lin=="it")echo "pronome dimostrativo";else if ($lin=="es") echo "pronombre demonstrativo:"; else echo "demonstrative pronoun:";?>";
   break;
case "RI":
   g2="<?if($lin=="it")echo "pronome interrogativo/indefinito";else if ($lin=="es") echo "pronombre interrogativo/indefinido:"; else echo "interrogative/indefinite pronoun:";?>";
   break;
case "RP":
   g2="<?if($lin=="it")echo "pronome personale/possessivo";else if ($lin=="es") echo "pronombre personal/possesivo:"; else echo "personal/possessive pronoun:";?>";
   break;
case "RR":
   g2="<?if($lin=="it")echo "pronome relativo";else if ($lin=="es") echo "pronombre relative:"; else echo "relative pronoun:";?>";
   break;
case "V-":
   g2="<?if($lin=="it")echo "verbo";else if ($lin=="es") echo "verbo:"; else echo "verb:";?>";
   break;
case "X-":
   g2="<?if($lin=="it")echo "particella";else if ($lin=="es") echo "particule"; else echo "particle";?>";
   break;
case "--":
   g2="";
   break;
}
switch (g.substring(2,3)) {
case "1":
	g2+="<?if($lin=="it")echo " 1a persona";else if ($lin=="es") echo " 1 persona"; else echo " 1st person";?>";
	break;
case "2":
	g2+="<?if($lin=="it")echo " 2a persona";else if ($lin=="es") echo " 2 persona"; else echo " 2nd person";?>";
	break;
case "3":
	g2+="<?if($lin=="it")echo " 3a persona";else if ($lin=="es") echo " 3 persona"; else echo " 3rd person";?>";
	break;
}
switch (g.substring(3,4)) {
case "A":
	g2+="<?if($lin=="it")echo " aoristo";else if ($lin=="es") echo " aoristo"; else echo " aorist";?>";
    break;
case "F":
	g2+="<?if($lin=="it")echo " futuro";else if ($lin=="es") echo " futuro"; else echo " future";?>";
	break;
case "I":
	g2+="<?if($lin=="it")echo " imperfetto";else if ($lin=="es") echo " imperfecto"; else echo " imperfect";?>";
	break;
case "P":
	g2+="<?if($lin=="it")echo " presente";else if ($lin=="es") echo " presente"; else echo " present";?>";
	break;
case "X":
	g2+="<?if($lin=="it")echo " perfetto";else if ($lin=="es") echo " perfecto"; else echo " perfect";?>";
	break;
case "Y":
	g2+="<?if($lin=="it")echo " piuccheperfetto";else if ($lin=="es") echo " pluscuamperfecto"; else echo " pluperfect";?>";
	break;
}
switch (g.substring(4,5)) {
case "A":
	g2+="<?if($lin=="it")echo " attivo";else if ($lin=="es") echo " activo"; else echo " active";?>";
	break;
case "M":
	g2+="<?if($lin=="it")echo " medio";else if ($lin=="es") echo " medio"; else echo " middle";?>";
	break;
case "P":
	g2+="<?if($lin=="it")echo " passivo";else if ($lin=="es") echo " pasivo"; else echo " passive";?>";
	break;
}
switch (g.substring(5,6)) {
case "D":
	g2+="<?if($lin=="it")echo " imperativo";else if ($lin=="es") echo " imperativo"; else echo " imperative";?>";
	break;
case "I":
	g2+="<?if($lin=="it")echo " indicativo";else if ($lin=="es") echo " indicativo"; else echo " indicative";?>";
	break;
case "N":
	g2+="<?if($lin=="it")echo " infinito";else if ($lin=="es") echo " infinitivo"; else echo " infinitive";?>";
	break;
case "O":
	g2+="<?if($lin=="it")echo " ottativo";else if ($lin=="es") echo " optativo"; else echo " optative";?>";
	break;
case "P":
	g2+="<?if($lin=="it")echo " participio";else if ($lin=="es") echo " participio"; else echo " participle";?>";
	break;
case "S":
	g2+="<?if($lin=="it")echo " congiuntivo";else if ($lin=="es") echo " subjunctivo"; else echo " subjunctive";?>";
	break;
}
switch (g.substring(6,7)) {
case "A":
	g2+="<?if($lin=="it")echo " accusativo";else if ($lin=="es") echo " acusativo"; else echo " accusative";?>";
	break;
case "D":
	g2+="<?if($lin=="it")echo " dativo";else if ($lin=="es") echo " dativo"; else echo " dative";?>";
	break;
case "G":
	g2+="<?if($lin=="it")echo " genitivo";else if ($lin=="es") echo " genitivo"; else echo " genitive";?>";
	break;
case "N":
	g2+="<?if($lin=="it")echo " nominativo";else if ($lin=="es") echo " nominativo"; else echo " nominative";?>";
	break;
case "V":
	g2+="<?if($lin=="it")echo " vocativo";else if ($lin=="es") echo " vocativo"; else echo " vocative";?>";
	break;
}
switch (g.substring(7,8)) {
case "P":
	g2+="<?if($lin=="it")echo " plurale";else if ($lin=="es") echo " plural"; else echo " plural";?>";
	break;
case "S":
	g2+="<?if($lin=="it")echo " singolare";else if ($lin=="es") echo " singular"; else echo " singular";?>";
	break;
}
switch (g.substring(8,9)) {
case "F":
	g2+="<?if($lin=="it")echo " femminile";else if ($lin=="es") echo " femenino"; else echo " feminine";?>";
	break;
case "M":
	g2+="<?if($lin=="it")echo " maschile";else if ($lin=="es") echo " masculino"; else echo " masculine";?>";
	break;
case "N":
	g2+="<?if($lin=="it")echo " neutro";else if ($lin=="es") echo " neutro"; else echo " neuter";?>";
	break;
}
switch (g.substring(9,10)) {
case "C":
	g2+="<?if($lin=="it")echo " comparativo";else if ($lin=="es") echo " comparativo"; else echo " comparative";?>";
	break;
case "S":
	g2+="<?if($lin=="it")echo " superlativo";else if ($lin=="es") echo " superlativo"; else echo " superlative";?>";
	break;
}

targ.title=p+" "+g2;
ajax("strong.php?p="+p, "def");
}

function Cambia_Tipo(t) {
window.document.form1.Gram1.disabled = true;
window.document.form1.Gram2.disabled = true;
window.document.form1.Gram3.disabled = true;
window.document.form1.Gram4.disabled = true;
window.document.form1.Gram5.disabled = true;
window.document.form1.Gram6.disabled = true;
window.document.form1.Gram7.disabled = true;
window.document.form1.Gram8.disabled = true;
switch (t.substring(0,1)) {
case "V":
	window.document.form1.Gram1.disabled = false;
	window.document.form1.Gram2.disabled = false;
	window.document.form1.Gram3.disabled = false;	
	window.document.form1.Gram4.disabled = false;
case "N":
case "R":
case "A":
	window.document.form1.Gram5.disabled = false;
	window.document.form1.Gram6.disabled = false;
	window.document.form1.Gram7.disabled = false;
}
if (t.substring(0,1)=="A")
	window.document.form1.Gram8.disabled = false;
}
</script>
<script type="text/javascript">

//<![CDATA[

	var xmlhttp=false;
	
	/*@cc_on @*/
	/*@if (@_jscript_version >= 5)
	try {
	xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
	} catch (e) {
	try {
	xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
	} catch (E) {
	xmlhttp = false;
	}
	}
	@end @*/
	
	if (!xmlhttp && typeof XMLHttpRequest != 'undefined') 
	{
		xmlhttp = new XMLHttpRequest();
	}
		
	// Funzione che si occupa di fare la richiesta AJAX allo script che materialmente effettuera' l'inserimento nel db
	function ajax(serverPage, objID) 
	{
		// Div dove finira' il risultato
		var obj = document.getElementById(objID);
		
		// Apre connessione
		xmlhttp.open("GET", serverPage);
		
		// Stampa risultato se tutto e' ok
		xmlhttp.onreadystatechange = function() {
							if (xmlhttp.readyState == 4 && xmlhttp.status == 200) 
							{
								//obj.innerHTML = "<p>123</p>";//xmlhttp.responseText;
                                obj.innerHTML = xmlhttp.responseText;
							}
					}
					
		xmlhttp.send(null);
		return obj.innerHTML;
	}
//]]>
</script>
</head>
<body>
<?
if ($bibleworks_bk!="") {
	$bibleworks_libri = array(47=>"Mat","Mar","Luk","Joh","Act","Rom","1Co","2Co","Gal","Eph","Phi","Col","1Th","2Th","1Ti","2Ti","Tit","Phm","Heb","Jam","1Pe","2Pe","1Jo","2Jo","3Jo","Jud","Rev");
	for ($i=47; $i<=73; ++$i)
		if ($bibleworks_libri[$i]==$bibleworks_bk)
			$rif1 = $i;
}
if ($bibleworks_ch>0 && $bibleworks_vs>0)
	$rif2 = $bibleworks_ch.":".$bibleworks_vs;
	
if (empty($bConfMss) && empty($bTrovaPar) && empty($bTrovaVers)) {
if ($lin=="it")
     echo '<h1>Nuovo Testamento greco <a href="/"><img src="/immagini/bibbia.gif" width="32" height="32" alt="Bibbia" style="border:0px" /></a></h1>';
else if ($lin=="es")
    echo '<h1>Nuevo Testamento Griego<a href="/english.html"><img src="/immagini/bibbia.gif" height="32" width="32" alt="Biblia" style="border: 0px"></a></h1>';
else
    echo '<h1>Greek New Testament <a href="/english.html"><img src="/immagini/bibbia.gif" width="32" height="32" alt="Bible" style="border:0px" /></a></h1>';
if ($rif1==0) {
   if ($lin=="it") {?>
<p>In questo sito si pu&ograve; leggere il testo del Nuovo Testamento greco con le varianti (letture alternative) pi&ugrave; importanti.
Cliccando su una parola greca nel testo si possono trovare delle informazioni sulla parola; lasciando il mouse sopra una parola (senza cliccare) ne d&agrave; un riassunto.</p>
<p>Lo stesso contenuto pu&ograve; essere letto senza Internet <a href="/programma/scaricamento_net.php">scaricando ed installando il programma della Bibbia e il Nuovo Testamento in greco</a> da questo sito,
oppure scaricando l'elenco di letture varianti (in inglese) <a href="/file/tc_rtf.zip">in formato RTF</a> per Word e programmi simili.</p>
   <?} else if ($lin=="es") {?>
<p>En este sitio puede leer los textos del Griego del Nuevo Testamento con las variantes textuales m&aacute;s importantes.</p>
<p>Haciendo click en una palabra Griega en el texto podr&aacute; encontrar informaci&oacute;n sobre la palabra; al pasar el cursor por encima de una palabra muestra un resumen.</p>
<p>Las lecturas variantes tambi&eacute;n se pueden leer sin estar conectado descargando e instalando el programa gratuito <a href="/program/">LaParola</a>.
Tambi&eacute;n hay listas (no siempre actualizadas) <a href="/file/tc_rtf.zip">en formato RTF</a> para Word y otros procesadores de texto.</p>   
      <?}else{?>
<p>In this site you can read the text of the Greek New Testament with the most important textual variants (alternative readings).
Clicking on a Greek word in the text you can find some information about the word; hovering the cursor above a word gives a summary.</p>
<p>The variant readings can also be read off line downloading and installing the free program <a href="/program/">LaParola</a>. 
There are also lists (not always updated)<br />
<ul>
<li><a href="/file/tc_rtf.zip">in RTF format</a> for Word and other word processors;</li>
<li><a href="/file/Variant Readings of the New Testament.docx">in docx format</a> for Word and other word processors, and that can be imported into <a href="logos.php">Logos</a>;</li>
</ul>
</p>
<?}}
   if ($rif1>0) {
      MostraBrano($rif1." ".$rif2."|", $ord, $direzione, $varianti, $msstt, $wh, $tisch, $biz, $inter, $allusioni);
   }
}
if (!empty($bConfMss)) {
    if ($lin=="it")
    	$tit_pagina = "<h1>Confronto di $mss1 e $mss2";
    else
      $tit_pagina = "<h1>Comparison of $mss1 and $mss2";
  	if ($TrovaVers_Rif!="")
  		$tit_pagina .= " in $ConfMss_Rif";
		echo $tit_pagina."</h1>";

    if (1==1) {
      if ($lin=="it") {
         echo "<p>Questo comando non &egrave; pi&ugrave; disponibile.</p>";
      }
      else {
         echo "<p>This comand is no longer available.</p>";
      }
    }
    else {
     $n_mss1=TraduciMss($mss1);
     $n_mss2=TraduciMss($mss2);
     if ($n_mss1==0)
        if ($lin=="it") echo "<p>Non ho capito la sigla $mss1 - vedi l'elenco di <a href=\"sigle.php\">sigle permesse</a>.</p>"; else echo "<p>I did not understand the code $mss1 - see the list of <a href=\"sigle.php\">permitted codes</a>.</p>";
     else if ($n_mss2==0)
        if ($lin=="it") echo "<p>Non ho capito la sigla $mss2 - vedi l'elenco di <a href=\"sigle.php\">sigle permesse</a>.</p>"; else echo "<p>I did not understand the code $mss2 - see the list of <a href=\"sigle.php\">permitted codes</a>.</p>";
     else {
          if ($lin=="it") {
             echo "<p>Non tutti i versetti quando i manoscritti sono uguali o diversi sono elencati, perch&eacute; spesso la lettura di un manoscritto non &egrave; elencata, soprattutto se &egrave; la lettura pi&ugrave; comune. ";
             echo "Se ci sono varianti in meno di 30 versetti, sono visualizzati. Se ci sono in meno di 300, solo i riferimenti sono elencati con un link alla pagina che d&agrave; le varianti. ";
             echo "Altrimenti, solo il numero di versetti &egrave; dato.</p>";
          }
          else {
             echo "<p>Not all the verses where the manuscripts are equal or different are listed, because often the reading of a manuscript is not given, especially when it is the more common reading. ";
             echo "If there are variants in less than 30 verses, they are shown. If there are in less than 300, only the references are listed with a link to the page with the variants. ";
             echo "Otherwise, only the number of verses is given.</p>";
          }
          if ($n_mss2==-1) {$n_mss2=$n_mss1; $n_mss1=-1;}
          if ($n_mss2==-1)
             $sql = "SELECT DISTINCT Libro,Capitolo,Versetto,VarInVers, COUNT(*) AS Count FROM Varianti WHERE (VarInVar=1";
          else if ($n_mss1==-1)
             $sql = "SELECT DISTINCT Libro,Capitolo,Versetto,VarInVers, COUNT(*) AS Count FROM Varianti, Sostegno WHERE (id_var=id_sosvar AND id_sosmss=$n_mss2 AND VarInVar=1";
          else // nota: questa riga funziona con MySQL 4 su LaParola, ma non con MySQL 5 sul mio computer 
              $sql = "SELECT DISTINCT Libro,Capitolo,Versetto,VarInVers, COUNT(*) AS Count FROM Varianti AS v, Sostegno AS s1 INNER JOIN Sostegno AS s2 ON (id_var=s1.id_sosvar AND s1.id_sosmss=$n_mss1 AND s2.id_sosmss=$n_mss2 AND s1.id_sosvar=s2.id_sosvar) WHERE (1=1";
					$sql .= Rif_A_Sql($ConfMss_Rif, $lin);
          $sql .= ") GROUP BY Libro, Capitolo, Versetto";
//        echo $sql;
          if ($ris=mysqli_query($conn, "$sql")) {
             $nDiff = mysqli_num_rows($ris);
             if ($lin=="it") {
                echo "<h2>".($nDiff==1?"C'&egrave ":"Ci sono ").$nDiff.($nDiff==1?" variante":" varianti")." in cui hanno lo stesso testo</h2>";
             } else {
               echo "<h2>".($nDiff==1?"There is ":"There are ").$nDiff.($nDiff==1?" variant":" variants")." in which they have the same text</h2>";
             }
             MostraVarianti($ris,$ord,$lin, "s", $msstt, $wh, $tisch, $biz, $inter, $allusioni, $direzione);
          }
          else {
               errore2("interrogazione database per confrontare $mss1 e $mss2");
          }
          if ($n_mss1==-1) // se $n_mss2==-1, non ci sono differenze fra UBS e UBS, e la seguente riga dà la risposta giusta anche se la query non è quella che dovrebbe essere
             $sql = "SELECT DISTINCT Libro,Capitolo,Versetto,VarInVers, COUNT(*) AS Count FROM Varianti AS v1, Sostegno WHERE (id_var=id_sosvar AND id_sosmss=$n_mss2 AND VarInVar>1";
          else
              $sql = "SELECT v1.*, COUNT(*) AS Count FROM Varianti AS v1, Sostegno AS s1 INNER JOIN Varianti as v2, Sostegno AS s2 ON (v1.Libro=v2.Libro AND v1.Capitolo=v2.Capitolo AND v1.Versetto=v2.Versetto AND v1.VarInVers=v2.VarInVers AND v1.VarInVar!=v2.VarInVar AND s1.id_sosmss=$n_mss1 AND s2.id_sosmss=$n_mss2 AND s1.id_sosvar=v1.id_var AND s2.id_sosvar=v2.id_var) WHERE (1=1";
  				$sql .= Rif_A_Sql($ConfMss_Rif, $lin, "v1.");
          $sql .= ") GROUP BY Libro, Capitolo, Versetto";
          if ($ris=mysqli_query($conn, "$sql")) {
             $nDiff = mysqli_num_rows($ris);
             if ($lin=="it") {
                echo "<h2>Ci sono varianti in $nDiff ".($nDiff==1?"versetto":"versetti")." in cui c'&egrave; un testo diverso</h2>";
             } else {
               echo "<h2>There are variants in $nDiff ".($nDiff==1?"verse":"verses")." in which there is a different text</h2>";
             }
             MostraVarianti($ris, $ord, $lin, "s", $msstt, $wh, $tisch, $biz, $inter, $allusioni, $direzione);
          }
          else {
               errore2("interrogazione database per confrontare $mss1 e $mss2");
          }
       }
      }
} // if (!empty($bConfMss))
if (!empty($bTrovaVers)) {
	if ($lin=="it")
		echo "<h1>Trova versetti</h1>";
	else if ($lin=="es")
		echo "<h1>Buscar vers&iacute;culos</h1>";
  else
  	echo "<h1>Find verses</h1>";
  $TrovaVers_Titolo = stripslashes($TrovaVers_Esp);
  $tit_pagina = $TrovaVers_Titolo;
  if ($TrovaVers_Rif!="")
  	$tit_pagina .= " in $TrovaVers_Rif";
  echo "<h2>$tit_pagina</h2>";

  $TrovaVers_Esp = trim(mysqli_real_escape_string($conn, $TrovaVers_Titolo));  
	$versetti = RicercaFrase($TrovaVers_Esp, $TrovaVers_Versione);
	$versetti = array_values(array_unique($versetti));
  reset($versetti);

  if ($lin=="it") {
     echo "<p>Se l'espressione da ricercare appare in meno di 30 versetti, tutti i versetti sono visualizzati. Se appare in meno di 300 versetti, solo i riferimenti sono elencati con un link alla pagina che d&agrave; il testo del versetto. ";
     echo "Altrimenti, solo il numero di volte che appare &egrave; dato.</p>";
  }
  else if ($lin=="es") {
     echo "<p>Si la expresi&oacute;n de busqueda aparece en menos de 30 versículos, se muestran todos los vers&iacute;culos. Si aparece en menos de 300 vers&iacute;culos, solo se muestran las referencias con un enlace a la p&aacute;gina del texto del vers&iacute;culo. ";
     echo "De lo contrario, solo el n&uacute;mero de veces que aparece es mostrado.</p>";
  }
  else {
     echo "<p>If the search expression appears in less than 30 verses, all the verses are shown. If it appears in less than 300 verses, only the references are listed with a link to the page with the text of the verse. ";
     echo "Otherwise, only the number of times that it appears is given.</p>";
  }
	if (count($versetti)<300) {
		$sqlbrano = Rif_A_Sql($TrovaVers_Rif, $lin);
		$sql = "SELECT *, COUNT(*) AS Count FROM GVersetti WHERE (id_v=-1";
		for ($i=0; $i<count($versetti); ++$i)
			$sql .= " OR id_v=$versetti[$i]";
		$sql .= ")$sqlbrano GROUP BY Libro, Capitolo, Versetto";
  	if ($ris=mysqli_query($conn, "$sql")) {
  			MostraVarianti($ris, $ord, $lin, $varianti, $msstt, $wh, $tisch, $biz, $inter, $allusioni, $direzione);
  	}
  	else {
       errore2("interrogazione database per visualizzare i risultati della ricerca $sql");
  	}
  }
  else {
  	if ($lin=="it")
  		echo "<p><b>".count($versetti)."</b> versetti sono stati trovati.</p>";
  	else if ($lin=="es")
  		echo "<p><b>".count($versetti)."</b> vers&iacute;culos encontrados.</p>";
  	else
    	echo "<p><b>".count($versetti)."</b> verses were found.</p>";
  }
} // if (!empty($bTrovaVers))
if (!empty($bTrovaPar)) {
	if ($lin=="it")
		echo "<h1>Trova parole</h1>";
	else if ($lin=="es")
		echo "<h1>Buscar palabras</h1>";
  else
  	echo "<h1>Find words</h1>";
	$Gram18 = $Gram1.$Gram2.$Gram3.$Gram4.$Gram5.$Gram6.$Gram7.$Gram8;
	$Gram18 = mysqli_real_escape_string($conn, $Gram18);
	$TrovaPar_Tipo = mysqli_real_escape_string($conn, $TrovaPar_Tipo);

	$sqlbrano1 = Rif_A_Sql($TrovaPar_Rif1, $lin);
	$sqlbrano2 = Rif_A_Sql($TrovaPar_Rif2, $lin);

	$totale = 0;
	$sql = "SELECT Count(*) AS Apparenze, Radice";
	if ($Gram18!="________")
		$sql .= ", Parola, Grammatica";
	$sql .= " FROM GVocab, Chiave$TrovaPar_Versione";
	if ($Gram18!="________")
		$sql .= ", GParole$TrovaPar_Versione";
	if ($sqlbrano2!="")
		$sql .= ", GVersetti";
	if ($sqlbrano1!="")
		$sql.= ", (SELECT DISTINCT id_r FROM GVersetti, Chiave$TrovaPar_Versione WHERE GVersetti.id_v=Chiave$TrovaPar_Versione.id_v$sqlbrano1) AS t1";
	$sql .= " WHERE GVocab.id_r=Chiave$TrovaPar_Versione.id_r";
	if ($Gram18!="________")
		$sql .= " AND GParole$TrovaPar_Versione.id_p=Chiave$TrovaPar_Versione.id_p";
	if ($sqlbrano2!="")
		$sql .= " AND GVersetti.id_v=Chiave$TrovaPar_Versione.id_v$sqlbrano2";
	if ($sqlbrano1!="")
		$sql .= " AND GVocab.id_r=t1.id_r";
	if ($TrovaPar_Tipo!="--")
		$sql .= " AND Grammatica LIKE '$TrovaPar_Tipo$Gram18'";
	$sql .= " GROUP BY Radice";
	if ($Gram18!="________")
		$sql .= ", Parola, Grammatica";
	$sql .= " HAVING (Apparenze>=$nVolteMin AND Apparenze<=$nVolteMas)";
	$sql .= " ORDER BY ";
	if ($TrovaPar_Ordine==0) {
		if ($Gram18!="________")
			$sql .= "RadicePerOrdine, ParolaPerOrdine";
		else
			$sql .= "RadicePerOrdine";
	}
	if ($TrovaPar_Ordine==1) {
		if ($Gram18!="________")
			$sql .= "ParolaPerOrdine, RadicePerOrdine";
		else
			$sql .= "RadicePerOrdine";
	}
	if ($TrovaPar_Ordine==2) {
		$sql .= "Apparenze DESC, ";
		if ($Gram18!="________")
			$sql .= "RadicePerOrdine, ParolaPerOrdine";
		else
			$sql .= "RadicePerOrdine";
	}

  if ($ris=mysqli_query($conn, "$sql")) {
  	if (mysqli_num_rows($ris)>0) {
  		echo "<table><tr><th>";
			if ($Gram18!="________")
				echo "Parola</th><th>";
			if ($lin=="it") echo "Radice"; else if ($lin=="es") echo "Ra&iacute;z"; else echo "Root";
			echo "</th><th align=\"right\">";
			if ($TrovaPar_Rif2=="")
				if ($lin=="it") echo "Volte nel NT"; else if ($lin=="es") echo "Veces en el NT"; else echo "Times in the NT";
			else
				if ($lin=="it") echo "Volte in $TrovaPar_Rif2"; else if ($lin=="es") echo "Veces en $TrovaPar_Rif2"; else echo "Times in $TrovaPar_Rif2";
			echo "</th></tr>";
			while ($row=mysqli_fetch_array($ris)) {
				$rad = $row["Radice"];
  			echo "<tr><td>";
				if ($Gram18!="________") {
					$par = $row["Parola"];
					$g = $row["Grammatica"];
					echo "<a href=\"index.php?TrovaVers=1&TrovaVers_Esp=$par/$rad##".ConvPersona($g)."\"><span class=\"uni\">$par</span></a> (".TradGram($g).")</td><td>";
				}
				echo "<a href=\"parola.php?p=$rad\"><span class=\"uni\">$rad</span></a></td><td align=\"right\">".$row["0"]."</td></tr>\n";
				$totale += $row["0"];
  		}
  		if ($lin=="it") echo "<tr><th>Totale:</th>"; else if ($lin=="es") echo "<tr><th>Total:</th>"; else echo "<tr><th>Total:</th>";
			if ($Gram18!="________")
				echo "<th></th>";
  		echo "<th align=\"right\">$totale</th></tr>";				
  		echo "</table>";
  	}
  	else {
  		if ($lin=="it") echo "<p>Nessuna parola &egrave; stata trovata con queste caratteristiche.</p>"; else if ($lin=="es") echo "<p>No se ha encontrado ninguna palabra con estas caracter&iacute;sticas</p>"; else echo "<p>No word was found with these characteristics.</p>";
  	}
  }
  else {
       errore2("interrogazione database per trovare parola $sql");
  }
} // if (!empty($TrovaPar))
echo "<div id=\"def\"></div>";
if ($rif1>0 || !empty($bConfMss) || !empty($bTrovaPar) || !empty($bTrovaVers)) echo "<hr />";
?>

<form method="post" action="index.php" name="form1">
<p><label>
<?if ($lin=="it") echo "Brano"; else if ($lin=="es") echo "Pasaje"; else echo "Passage";?>:&nbsp;
<select name="rif1">
<?
for ($i=47; $i<=73; $i++) {
  echo "<option value='$i'";
  if ($rif1==$i) echo " selected=\"selected\"";
  echo ">";
  if ($lin=="it") echo $libri_nomi[$i]; else if ($lin=="es") echo $libri_es[$i]; else echo $libri_eng[$i];
  echo "</option>\n";
}
?>
</select></label>
&nbsp;
<input class="text" name="rif2" value="<?echo $rif2;?>" title="<?if ($lin=="it") echo "Digita qui il riferimento di un brano"; else if ($lin=="es") echo "Escriba la referencia del pasaje"; else echo "Type here the reference of a passage";?>" />&nbsp;
<input class="submit" type="submit" name="VisTesto" value="<?if ($lin=="it") echo "Visualizza testo"; else if ($lin=="es") echo "Ver Texto"; else echo "View text";?>" />
<?if ($lin=="it") echo "(Non pi&ugrave; di un capitolo)"; else if ($lin=="es") echo "(No m&aacute;s de un cap&iacute;tulo)"; else echo "(No more than one chapter)";?></p>

<!--
<h3><?if ($lin=="it") echo "Confronta manoscritti"; else echo "Compare manuscripts";?></h3>

<p><label><?if ($lin=="it") echo "Manoscritto 1"; else echo "Manuscript 1";?>:&nbsp;
<input class="text" name="mss1" value="<?echo $mss1;?>" title="<?if ($lin=="it") echo "Digita qui la sigla del primo manoscritto"; else echo "Type here the symbol of the first manuscript";?>" /></label>
&nbsp;<label><?if ($lin=="it") echo "Manoscritto 2"; else echo "Manuscript 2";?>:&nbsp;
<input class="text" name="mss2" value="<?echo $mss2;?>" title="<?if ($lin=="it") echo "Digita qui la sigla del secondo manoscritto"; else echo "Type here the symbol of the secondo manuscript";?>" /></label>
<?if ($lin=="it") echo " nel brano "; else echo " in the passage ";?><input class="text" name="ConfMss_Rif" value="<?echo $ConfMss_Rif;?>" title="<?if ($lin=="it") echo "Il riferimento del brano in cui fare il confronto; lascia vuoto per tutto il NT"; else echo "The reference of the passage in which to do the comparision; leave blank for all the NT";?>" />
<input class="submit" type="submit" name="ConfMss" value="<?if ($lin=="it") echo "Confronta manoscritti"; else echo "Compare manuscripts";?>" /></p>
<p><a href="sigle.php"><?if ($lin=="it") echo "Sigle dei manoscritti permesse"; else echo "Allowed codes for the manuscripts";?></a></p>
-->

<h3><?if ($lin=="it") echo "Trova versetti"; else if ($lin=="es") echo "Encontrar vers&iacute;culos"; else echo "Find verses";?></h3>
<p>
<?if ($lin=="it") echo "Versetti che contengono "; else if ($lin=="es") echo "Vers&iacute;culos que contengan "; else echo "Verses that contain ";?><input class="text" name="TrovaVers_Esp" value="<?echo stripslashes($TrovaVers_Esp);?>" title="<?if ($lin=="it") echo "Digita qui l'espressione da ricercare"; else if ($lin=="es") echo "Escriba la expresi&oacute;n a buscar"; else echo "Type here the expression to search for";?>" />
<?if ($lin=="it") echo " nel brano "; else if ($lin=="es") echo " en el pasaje "; else echo " in the passage ";?><input class="text" name="TrovaVers_Rif" value="<?echo $TrovaVers_Rif;?>" title="<?if ($lin=="it") echo "Digita qui il riferimento del brano da ricercare; lascia vuoto per tutto il NT"; else if ($lin=="es") echo "Escriba aqui la referencia del pasaje a buscar; deje en blanco para buscar en todo el NT"; else echo "Type here the reference of the passage to search in; leave blank for all the NT";?>" />
<?if ($lin=="it") echo " in "; else if ($lin=="es") echo " en "; else echo " in ";?><select name="TrovaVers_Versione"><option value=""<?if ($TrovaVers_Versione=="") echo " selected=\"selected\"";?>>SBL</option><option value="WH"<?if ($TrovaVers_Versione=="WH") echo " selected=\"selected\"";?>>Westcott/Hort</option><option value="Tisch"<?if ($TrovaVers_Versione=="Tisch") echo " selected=\"selected\"";?>>Tischendorf</option><option value="Biz"<?if ($TrovaVers_Versione=="Biz") echo " selected=\"selected\"";?>><?if ($lin=="it" || $lin=="es") echo "Bizantino"; else echo "Byzantine";?></option></select>
<input class="submit" type="submit" name="TrovaVers" value="<?if ($lin=="it") echo "Trova versetti"; else if ($lin=="es") echo "Encontrar vers&iacute;culos"; else echo "Find verses";?>" /></p>
<p><a href="istruzioni.php#TrovaVersetti"><?if ($lin=="it") echo "Spiegazione ed esempi"; else if ($lin=="es") echo "Explicaci&oacute;n y ejemplos"; else echo "Explanation and examples";?></a>
</p>

<h3><?if ($lin=="it") echo "Trova parole"; else if ($lin=="es") echo "Encontrar palabras"; else echo "Find words";?></h3>

<p><?if ($lin=="it") echo "Ogni "; else if ($lin=="es") echo "Cada "; else echo "Every ";?>
<select name="TrovaPar_Tipo" onchange="Cambia_Tipo(window.document.form1.TrovaPar_Tipo.options[selectedIndex].value)">
<?
echo '<option value="--"';
if ($TrovaPar_Tipo=="--") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "radice"; else if ($lin=="es") echo "palabra ra&iacute;z"; else echo "root word";
echo "</option>\n";
echo '<option value="V-"';
if ($TrovaPar_Tipo=="V-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "verbo"; else if ($lin=="es") echo "verbo"; else echo "verb";
echo "</option>\n";
echo '<option value="N-"';
if ($TrovaPar_Tipo=="N-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "sostantivo"; else if ($lin=="es") echo "sustantivo"; else echo "noun";
echo "</option>\n";
echo '<option value="D-"';
if ($TrovaPar_Tipo=="D-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "avverbio"; else if ($lin=="es") echo "adverbio"; else echo "adverb";
echo "</option>\n";
echo '<option value="A-"';
if ($TrovaPar_Tipo=="A-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "aggettivo"; else if ($lin=="es") echo "adjectivo"; else echo "adjective";
echo "</option>\n";
echo '<option value="RA"';
if ($TrovaPar_Tipo=="RA") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "articolo"; else if ($lin=="es") echo "articulo"; else echo "article";
echo "</option>\n";
echo '<option value="RD"';
if ($TrovaPar_Tipo=="RD") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "pronome dimostrativo"; else if ($lin=="es") echo "pronombre demonstrativo"; else echo "demonstrative pronoun";
echo "</option>\n";
echo '<option value="RI"';
if ($TrovaPar_Tipo=="RI") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "pronome interrogativo/indefinito"; else if ($lin=="es") echo "pronombre interrogativo/indefinido"; else echo "interrogative/indefinite pronoun";
echo "</option>\n";
echo '<option value="RP"';
if ($TrovaPar_Tipo=="RP") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "pronome personale/possessivo"; else if ($lin=="es") echo "pronombre personal/possesivo"; else echo "personal/possessive pronoun";
echo "</option>\n";
echo '<option value="RR"';
if ($TrovaPar_Tipo=="RR") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "pronome relativo"; else if ($lin=="es") echo "pronombre relative"; else echo "relative pronoun";
echo "</option>\n";
echo '<option value="P-"';
if ($TrovaPar_Tipo=="P-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "preposizione"; else if ($lin=="es") echo "preposici&oacute;n"; else echo "preposition";
echo "</option>\n";
echo '<option value="C-"';
if ($TrovaPar_Tipo=="C-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "congiunzione"; else if ($lin=="es") echo "conjunci&oacute;n"; else echo "conjunction";
echo "</option>\n";
echo '<option value="I-"';
if ($TrovaPar_Tipo=="I-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "interiezione"; else if ($lin=="es") echo "interjeci&oacute;n"; else echo "interjection";
echo "</option>\n";
echo '<option value="X-"';
if ($TrovaPar_Tipo=="X-") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "particella"; else if ($lin=="es") echo "particule"; else echo "particle";
echo "</option>\n</select> ";

echo '<select name="Gram1"';
if ($TrovaPar_Tipo!="V-") echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram1=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="1"';
if ($Gram1=="1") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "1a persona"; else if ($lin=="es") echo "1 persona"; else echo "1st person";
echo "</option>\n";
echo '<option value="2"';
if ($Gram1=="2") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "2a persona"; else if ($lin=="es") echo "2 persona"; else echo "2nd person";
echo "</option>\n";
echo '<option value="3"';
if ($Gram1=="3") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "3a persona"; else if ($lin=="es") echo "3 persona"; else echo "3rd person";
echo "</option>\n</select>";

echo '<select name="Gram2"';
if ($TrovaPar_Tipo!="V-") echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram2=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="P"';
if ($Gram2=="P") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "presente"; else if ($lin=="es") echo "presente"; else echo "present";
echo "</option>\n";
echo '<option value="I"';
if ($Gram2=="I") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "imperfetto"; else if ($lin=="es") echo "imperfecto"; else echo "imperfect";
echo "</option>\n";
echo '<option value="F"';
if ($Gram2=="F") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "futuro"; else if ($lin=="es") echo "futuro"; else echo "future";
echo "</option>\n";
echo '<option value="A"';
if ($Gram2=="A") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "aoristo"; else if ($lin=="es") echo "aoristo"; else echo "aorist";
echo "</option>\n";
echo '<option value="X"';
if ($Gram2=="X") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "perfetto"; else if ($lin=="es") echo "perfecto"; else echo "perfect";
echo "</option>\n";
echo '<option value="Y"';
if ($Gram2=="Y") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "piuccheperfetto"; else if ($lin=="es") echo "pluscuamperfecto"; else echo "pluperfect";
echo "</option>\n</select>";

echo '<select name="Gram3"';
if ($TrovaPar_Tipo!="V-") echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram3=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="A"';
if ($Gram3=="A") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "attivo"; else if ($lin=="es") echo "activo"; else echo "active";
echo "</option>\n";
echo '<option value="M"';
if ($Gram3=="M") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "medio"; else if ($lin=="es") echo "medio"; else echo "middle";
echo "</option>\n";
echo '<option value="P"';
if ($Gram3=="P") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "passivo"; else if ($lin=="es") echo "pasivo"; else echo "passive";
echo "</option>\n</select>";

echo '<select name="Gram4"';
if ($TrovaPar_Tipo!="V-") echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram4=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="I"';
if ($Gram4=="I") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "indicativo"; else if ($lin=="es") echo "indicativo"; else echo "indicative";
echo "</option>\n";
echo '<option value="D"';
if ($Gram4=="D") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "imperativo"; else if ($lin=="es") echo "imperativo"; else echo "imperative";
echo "</option>\n";
echo '<option value="S"';
if ($Gram4=="S") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "congiuntivo"; else if ($lin=="es") echo "subjunctivo"; else echo "subjunctive";
echo "</option>\n";
echo '<option value="O"';
if ($Gram4=="O") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "ottativo"; else if ($lin=="es") echo "optativo"; else echo "optative";
echo "</option>\n";
echo '<option value="N"';
if ($Gram4=="N") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "infinito"; else if ($lin=="es") echo "infinitivo"; else echo "infinitive";
echo "</option>\n";
echo '<option value="P"';
if ($Gram4=="P") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "participio"; else if ($lin=="es") echo "participio"; else echo "participle";
echo "</option>\n</select>";

echo '<select name="Gram5"';
if ($TrovaPar_Tipo=="--" || $TrovaPar_Tipo=="C-" || $TrovaPar_Tipo=="D-" || $TrovaPar_Tipo=="I-" || $TrovaPar_Tipo=="P-" || $TrovaPar_Tipo=="X-")
	echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram5=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="N"';
if ($Gram5=="N") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "nominativo"; else if ($lin=="es") echo "nominativo"; else echo "nominative";
echo "</option>\n";
echo '<option value="V"';
if ($Gram5=="V") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "vocativo"; else if ($lin=="es") echo "vocativo"; else echo "vocative";
echo "</option>\n";
echo '<option value="A"';
if ($Gram5=="A") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "accusativo"; else if ($lin=="es") echo "acusativo"; else echo "accusative";
echo "</option>\n";
echo '<option value="G"';
if ($Gram5=="G") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "genitivo"; else if ($lin=="es") echo "genitivo"; else echo "genitive";
echo "</option>\n";
echo '<option value="D"';
if ($Gram5=="D") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "dativo"; else if ($lin=="es") echo "dativo"; else echo "dative";
echo "</option>\n</select>";

echo '<select name="Gram6"';
if ($TrovaPar_Tipo=="--" || $TrovaPar_Tipo=="C-" || $TrovaPar_Tipo=="D-" || $TrovaPar_Tipo=="I-" || $TrovaPar_Tipo=="P-" || $TrovaPar_Tipo=="X-")
	echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram6=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="S"';
if ($Gram6=="S") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "singolare"; else if ($lin=="es") echo "singular"; else echo "singular";
echo "</option>\n";
echo '<option value="P"';
if ($Gram6=="P") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "plurale"; else if ($lin=="es") echo "plural"; else echo "plural";
echo "</option>\n</select>";

echo '<select name="Gram7"';
if ($TrovaPar_Tipo=="--" || $TrovaPar_Tipo=="C-" || $TrovaPar_Tipo=="D-" || $TrovaPar_Tipo=="I-" || $TrovaPar_Tipo=="P-" || $TrovaPar_Tipo=="X-")
	echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram7=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="M"';
if ($Gram7=="M") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "maschile"; else if ($lin=="es") echo "masculino"; else echo "masculine";
echo "</option>\n";
echo '<option value="F"';
if ($Gram7=="F") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "femminile"; else if ($lin=="es") echo "femenino"; else echo "feminine";
echo "</option>\n";
echo '<option value="N"';
if ($Gram7=="N") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "neutro"; else if ($lin=="es") echo "neutro"; else echo "neuter";
echo "</option>\n</select>";

echo '<select name="Gram8"';
if ($TrovaPar_Tipo!="A-") echo ' disabled="disabled"';
echo '><option value="_"';
if ($Gram8=="_") echo " selected=\"selected\"";
echo ">";
echo "</option>\n";
echo '<option value="C"';
if ($Gram8=="C") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "comparativo"; else if ($lin=="es") echo "comparativo"; else echo "comparative";
echo "</option>\n";
echo '<option value="S"';
if ($Gram8=="S") echo " selected=\"selected\"";
echo ">";
if ($lin=="it") echo "superlativo"; else if ($lin=="es") echo "superlativo"; else echo "superlative";
echo "</option>\n</select>";

?>
</p><p>
<?if ($lin=="it") echo "che appare nel brano "; else if ($lin=="es") echo "que aparece "; else echo "that appears in the passage ";?>
<input class="text" name="TrovaPar_Rif1" value="<?echo $TrovaPar_Rif1;?>" title="<?if ($lin=="it") echo "Digita qui il riferimento del brano da ricercare; lascia vuoto per tutto il NT"; else if ($lin=="es") echo "Escriba aqui la referencia a buscar, deje en blanco para buscar en todo el NT"; else echo "Type here the reference of the passage to search in; leave blank for all the NT";?>" />
<?if ($lin=="it") echo "e che appare almeno "; else if ($lin=="es") echo "y que aparece al menos "; else echo "and that appears at least ";?>
<input class="text" name="nVolteMin" value="<?echo $nVolteMin;?>" title="<?if ($lin=="it") echo "Digita qui il minimo numero di volte che la parola deve apparire"; else if ($lin=="es") echo "Escriba aqui la minima cantidad de veces que debe aparecer la palabra"; else echo "Type here the minimum number of times that the word must appear";?>" />
<?if ($lin=="it") echo "e non pi&ugrave; di "; else if ($lin=="es") echo "y no m&aacute;s de "; else echo "and not more than ";?>
<input class="text" name="nVolteMas" value="<?echo $nVolteMas;?>" title="<?if ($lin=="it") echo "Digita qui il massimo numero di volte che la parola deve apparire"; else if ($lin=="es") echo "Escriba aqui la m&aacute;xima cantidad de veces que debe aparecer la palabra"; else echo "Type here the maximum number of times that the word must appear";?>" />
<?if ($lin=="it") echo "volte nel brano "; else if ($lin=="es") echo "veces en el pasaje "; else echo "times in the passage ";?>
<input class="text" name="TrovaPar_Rif2" value="<?echo $TrovaPar_Rif2;?>" title="<?if ($lin=="it") echo "Digita qui il riferimento del brano da ricercare; lascia vuoto per tutto il NT"; else if ($lin=="es") echo "Escriba aqui la referencia del pasaje a buscar; d&eacute;jelo en blanco para buscar en todo el NT"; else echo "Type here the reference of the passage to search in; leave blank for all the NT";?>" />
<?if ($lin=="it") echo " in "; else if ($lin=="es") echo " en "; else echo " in ";?><select name="TrovaPar_Versione"><option value=""<?if ($TrovaPar_Versione=="") echo " selected=\"selected\"";?>>SBL</option><option value="WH"<?if ($TrovaPar_Versione=="WH") echo " selected=\"selected\"";?>>Westcott/Hort</option><option value="Tisch"<?if ($TrovaPar_Versione=="Tisch") echo " selected=\"selected\"";?>>Tischendorf</option><option value="Biz"<?if ($TrovaPar_Versione=="Biz") echo " selected=\"selected\"";?>><?if ($lin=="it" || $lin=="es") echo "Bizantino"; else echo "Byzantine";?></option></select>
</p><p>
<?if ($lin=="it") echo "Ordine"; else if ($lin=="es") echo "Ordenar por"; else echo "Order";?>: <select name="TrovaPar_Ordine">
<option value="0"
<?if ($TrovaPar_Ordine==0) echo " selected=\"selected\""?>
><?if ($lin=="it") echo "alfabetico delle radici"; else if ($lin=="es") echo "orden alfab&eacute;tico de las raices"; else echo "alphabetical of the roots";?></option>
<option value="1"
<?if ($TrovaPar_Ordine==1) echo " selected=\"selected\""?>
><?if ($lin=="it") echo "alfabetico delle parole"; else if ($lin=="es") echo "orden alfab&eacute;tico de las palabras"; else echo "alphabetical of the words";?></option>
<option value="2"
<?if ($TrovaPar_Ordine==2) echo " selected=\"selected\""?>
><?if ($lin=="it") echo "numero di volte che appare"; else if ($lin=="es") echo "n&uacute;mero de apariciones"; else echo "number of appearances";?></option>
</select>
<input class="submit" type="submit" name="TrovaPar" value="<?if ($lin=="it") echo "Trova parole"; else if ($lin=="es") echo "Encontrar palabras"; else echo "Find words";?>" />
</p>

<p><?if ($lin=="it") echo "Cerca per prima lettera"; else if ($lin=="es") echo "Buscar por primera letra"; else echo "Search by first letter";?>:
<?
for ($i=945; $i<=969; ++$i)
	if ($i!=962)
		echo " <a href=\"concordanza.php?letter=$i\"><span class=\"uni\">&#$i;</span></a>";
?>
</p>

<p><a href="louwnida.php"><?if ($lin=="it") echo "Cerca per categoria Louw-Nida o definizione inglese"; else if ($lin=="es") echo "Buscar por categoria Louw-Nida o inglesa"; else echo "Search by Louw-Nida category or English gloss";?></a>.</p>

<h3><?if ($lin=="it") echo "Opzioni"; else if ($lin=="es") echo "Opciones"; else echo "Options";?></h3>

<table>
<tr><td><label><?if ($lin=="it") echo "Visualizzare le varianti"; else if ($lin=="es") echo "Ver las lecturas variantes"; else echo "View the variant readings";?>:</label></td><td>
<input type="radio" name="varianti" value="s"<?if ($varianti=="s") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "S&igrave;"; else if ($lin=="es") echo "Si"; else echo "Yes";?>
<input type="radio" name="varianti" value="n"<?if ($varianti=="n") echo " checked=\"checked\"";?> />No
</td></tr><tr><td>

<label><?if ($lin=="it") echo "L'ordine dei manoscritti per ogni variante"; else if ($lin=="es") echo "El orden de los manuscritos en cada lectura variante"; else echo "The order of the manuscripts in each variant reading";?>:</label></td><td>
<select name="ord">
<option value="1" <?if ($ord==1) echo "selected=\"selected\"";?>><?if ($lin=="it") echo "Per tipo di manoscritto"; else if ($lin=="es") echo "por tipo de manuscrito"; else echo "By type of manuscript";?></option>
<option value="2" <?if ($ord==2) echo "selected=\"selected\"";?>><?if ($lin=="it") echo "Per data"; else if ($lin=="es") echo "por fecha"; else echo "By date";?></option>
<option value="3" <?if ($ord==3) echo "selected=\"selected\"";?>><?if ($lin=="it") echo "Per tipo di testo"; else if ($lin=="es") echo "por tipo textual"; else echo "By text type";?></option>
</select>
</td></tr><tr><td>

<label><?if ($lin=="it") echo "Mostra informazioni sui manoscritti"; else if ($lin=="es") echo "Mostrar la informaci&oacute;n en los manuscritos"; else echo "Show the information on the manuscripts";?>:</label></td><td>
<select name="msstt">
<option value="s" <?if ($msstt=="s") echo "selected=\"selected\"";?>><?if ($lin=="it") echo "Come tooltip"; else if ($lin=="es") echo "como un cuadro emergente"; else echo "As a tooltip";?></option>
<option value="n" <?if ($msstt=="n") echo "selected=\"selected\"";?>><?if ($lin=="it") echo "Dopo il manoscritto"; else if ($lin=="es") echo "luego del manuscrito"; else echo "After the manuscripts";?></option>
</select>
</td></tr><tr><td>

<label><?if ($lin=="it") echo "Visualizzare il testo di Westcott e Hort"; else if ($lin=="es") echo "Mortrar el texto de Westcott and Hort"; else echo "Show the text of Westcott and Hort";?>:</label></td><td>
<input type="radio" name="wh" value="s"<?if ($wh=="s") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "S&igrave;"; else if ($lin=="es") echo "Si"; else echo "Yes";?>
<input type="radio" name="wh" value="n"<?if ($wh=="n") echo " checked=\"checked\"";?> />No
</td></tr><tr><td>

<label><?if ($lin=="it") echo "Visualizzare il testo di Tischendorf"; else if ($lin=="es") echo "Mortrar el texto de Tischendorf"; else echo "Show the text of Tischendorf";?>:</label></td><td>
<input type="radio" name="tisch" value="s"<?if ($tisch=="s") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "S&igrave;"; else if ($lin=="es") echo "Si"; else echo "Yes";?>
<input type="radio" name="tisch" value="n"<?if ($tisch=="n") echo " checked=\"checked\"";?> />No
</td></tr><tr><td>

<label><?if ($lin=="it") echo "Visualizzare il testo bizantino"; else if ($lin=="es") echo "Mostar el texto Bizantino"; else echo "Show the Byzantine text";?>:</label></td><td>
<input type="radio" name="biz" value="s"<?if ($biz=="s") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "S&igrave;"; else if ($lin=="es") echo "Si"; else echo "Yes";?>
<input type="radio" name="biz" value="n"<?if ($biz=="n") echo " checked=\"checked\"";?> />No
</td></tr><tr><td>

<label><?if ($lin=="it") echo "Visualizzare l'interlineare greco-Nuova Riveduta"; else if ($lin=="es") echo "Mostrar el interlineal Griego-Italiano"; else echo "Show the Greek-Italian interlinear";?>:</label></td><td>
<input type="radio" name="inter" value="s"<?if ($inter=="s") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "S&igrave;"; else if ($lin=="es") echo "Si"; else echo "Yes";?>
<input type="radio" name="inter" value="n"<?if ($inter=="n") echo " checked=\"checked\"";?> />No
</td></tr><tr><td>

<label><?if ($lin=="it") echo "Visualizzare le allusioni nei Padri"; else if ($lin=="es") echo "Mostrar las alusiones de los Padres"; else echo "Show the allusions in the Fathers";?>:</label></td><td>
<input type="radio" name="allusioni" value="s"<?if ($allusioni=="s") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "S&igrave;"; else if ($lin=="es") echo "Si"; else echo "Yes";?>
<input type="radio" name="allusioni" value="n"<?if ($allusioni=="n") echo " checked=\"checked\"";?> />No
<?if ($lin=="it") echo "(da"; else if ($lin=="es") echo "(de"; else echo "(from";?> <a href="http://www.earlychristianwritings.com/e-catena/">Early Christian Writings</a>)
</td></tr><tr><td>

<label><?if ($lin=="it") echo "Visualizzare i testi"; else if ($lin=="es") echo "Mostrar los"; else echo "Show the texts";?>:</label></td><td>
<input type="radio" name="direzione" value="v"<?if ($direzione=="v") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "Verticalmente"; else if ($lin=="es") echo "Verticalmente"; else echo "Vertically";?>
<input type="radio" name="direzione" value="o"<?if ($direzione=="o") echo " checked=\"checked\"";?> /><?if ($lin=="it") echo "Orizzontalmente"; else if ($lin=="es") echo "Horizontalmente"; else echo "Horizontally";?>
</td></tr><tr><td>

<label><?if ($lin=="it") echo "La lingua della pagina"; else if ($lin=="es") echo "Languaje de la p&aacute;gina"; else echo "The language of the page";?>:</label></td><td>
<select name="lin">
<option value="en"<?if ($lin=="en") echo " selected=\"selected\"";?>>English/inglese/ingl&eacute;s</option>
<option value="it"<?if ($lin=="it") echo " selected=\"selected\"";?>>Italiano/Italian</option>
<option value="es"<?if ($lin=="es") echo " selected=\"selected\"";?>>Espa&ntilde;ol/Spanish/spagnolo</option>
</select>
</td></tr><tr><td>

<label><a href="font.php"><?if ($lin=="it") echo "Font unicode per il testo greco"; else if ($lin=="es") echo "Fuente unicode para el texto griego"; else echo "Unicode font for the Greek text";?></a>:</label></td><td>
<select name="fontuni">
<option value="" <?if ($fontuni=="") echo "selected=\"selected\"";?>><?if ($lin=="it") echo "Predefinito"; else if ($lin=="es") echo "por defecto"; else echo "Default";?></option>
<option value="Galatia SIL" <?if ($fontuni=="Galatia SIL") echo "selected=\"selected\"";?>>Galatia SIL</option>
<option value="Gentium" <?if ($fontuni=="Gentium") echo "selected=\"selected\"";?>>Gentium</option>
<option value="Cardo" <?if ($fontuni=="Cardo") echo "selected=\"selected\"";?>>Cardo</option>
<option value="Vusillus Old Face" <?if ($fontuni=="Vusillus Old Face") echo "selected=\"selected\"";?>>Vusillus Old Face</option>
<option value="Athena" <?if ($fontuni=="Athena") echo "selected=\"selected\"";?>>Athena</option>
<option value="Caslon" <?if ($fontuni=="Caslon") echo "selected=\"selected\"";?>>Caslon</option>
<option value="Hindsight Unicode" <?if ($fontuni=="Hindsight Unicode") echo "selected=\"selected\"";?>>Hindsight Unicode</option>
<option value="Chrysanthi Unicode" <?if ($fontuni=="Chrysanthi Unicode") echo "selected=\"selected\"";?>>Chrysanthi Unicode</option>
<option value="Monospace" <?if ($fontuni=="Monospace") echo "selected=\"selected\"";?>>Monospace</option>
<option value="OldStandard" <?if ($fontuni=="OldStandard") echo "selected=\"selected\"";?>>OldStandard</option>
<option value="Palatino Linotype" <?if ($fontuni=="Palatino Linotype") echo "selected=\"selected\"";?>>Palatino Linotype</option>
<option value="Tahoma" <?if ($fontuni=="Tahoma") echo "selected=\"selected\"";?>>Tahoma</option>
</select>
</td></tr></table>

</form>

<h3><?if ($lin=="it") echo "Informazioni"; else if ($lin=="es") echo "Informaci&ograve;n"; else echo "Information";?></h3>

<?if ($lin=="it") {?>
<p><a href="istruzioni.php">Istruzioni e spiegazioni</a></p>
<p><a href="font.php">Alcune lettere greche non sono visualizzate? Problemi con il font?</a></p>
<p><a href="manoscritti.php">Tutti i manoscritti</a></p>
<p><a href="idee.php">Idee per il futuro</a></p>
<p><a href="bibleworks.php">Come inserire questo sito nel programma <i>BibleWorks</i></a></p>
<p><a href="logos.php">Come inserire questo sito nel programma <i>Logos</i></a></p>

<p>Il testo greco &egrave; il <a href="https://www.sblgnt.com/">SBL Greek New Testamento</a>, con l'analisi grammaticale di ogni parola di MorphGNT.<br />
Il testo di Westcott e Hort (1881) &egrave; l'unione del testo del <a href="https://www.ccel.org/w/westcott/gnt/toc.htm">Christian Classics Ethereal Library</a> (con correzioni), il testo di <a href="https://www.perseus.tufts.edu/hopper/text.jsp?doc=Perseus:text:1999.01.0155">Perseus</a> con correzioni da Charles Hill, il testo di <a href="http://faculty.gordon.edu/hu/bi/Ted_Hildebrandt/New_Testament_Greek/Text/00-GreekArticlesWebBib.htm">Ted Hildebrandt</a> corretto e migliorato da Harmai G&aacute;bor, e l'analisi grammaticale di Maurice A.&nbsp;Robinson.
<!--G&aacute;bor ha questo testo come <a href="http://www.ehf.hu/ujszov/letolthetok/GNT_WH_e_book.zip">libro elettronico</a>.--><br />
Il testo di Tischendorf (8a edizione; 1869-1872) &egrave; di Ulrik Petersen.<br />
Il testo bizantino &egrave; di Maurice A.&nbsp;Robinson (edizione 2005).<br />
I link ai file audio sono di Greeklatinaudio (testo di Westcott e Hort, pronuncia moderna).</p>

<p>Per commenti e domande, scrivi a <i>info</i> a questo dominio.</p>
<?}else if ($lin=="es") {?>
<p><a href="istruzioni.php">Instrucciones y explicaci&oacute;n</a></p>
<p><a href="font.php">&iquest;Algunas letras griegas no est&aacute;n correctas? Problemas con la fuente?</a></p>
<p><a href="manoscritti.php">Todos los manuscritos</a></p>
<p><a href="idee.php">Ideas para el futuro</a></p>
<p><a href="bibleworks.php">C&oacute;mo insertar este sitio en el programa<i>BibleWorks</i> program</a></p>

<p>The Greek text is the <a href="https://www.sblgnt.com/">SBL Greek New Testamento</a>, with the parsing information by MorphGNT.<br />
El texto de Westcott y Hort (1881) es la unipin del texto de <a href="https://www.ccel.org/w/westcott/gnt/toc.htm">Christian Classics Ethereal Library</a> (con correcciones), el texto por <a href="https://www.perseus.tufts.edu/hopper/text.jsp?doc=Perseus:text:1999.01.0155">Perseus</a> con correcciones por Charles Hill, el texto por <a href="http://faculty.gordon.edu/hu/bi/Ted_Hildebrandt/New_Testament_Greek/Text/00-GreekArticlesWebBib.htm">Ted Hildebrandt</a> con correcciones por Harmai G&aacute;bor, e informaci&oacute;n interpretada por Maurice A.&nbsp;Robinson.<br />
El texto de Tischendorf (8va edici&oacute;n; 1869-1872) es por Ulrik Petersen.<br />
El texto Bizantino es por Maurice A.&nbsp;Robinson (edici&oacute;n 2005).<br />
Los enlaces a los archivos de audio son de Greeklatinaudio (texto Westcott y Hort, pronunciaci&oacute;n moderna).</p>

<p>Para comentarios y preguntas, escriba a <i>info</i> en este dominio.</p>
<?}else{?>
<p><a href="istruzioni.php">Instructions and explanations</a></p>
<p><a href="font.php">Some of the Greek letters are not correct? Problems with the font?</a></p>
<p><a href="manoscritti.php">All the manuscripts</a></p>
<p><a href="idee.php">Ideas for the future</a></p>
<p><a href="bibleworks.php">How to insert this site into the <i>BibleWorks</i> program</a></p>
<p><a href="logos.php">How to insert this site into the <i>Logos</i> program</a></p>

<p>The Greek text is the <a href="https://www.sblgnt.com/">SBL Greek New Testamento</a>, with the parsing information by MorphGNT.<br />
The text of Westcott and Hort (1881) is the union of the text of <a href="https://www.ccel.org/w/westcott/gnt/toc.htm">Christian Classics Ethereal Library</a> (with corrections), the text by <a href="https://www.perseus.tufts.edu/hopper/text.jsp?doc=Perseus:text:1999.01.0155">Perseus</a> with corrections by Charles Hill, the text of <a href="http://faculty.gordon.edu/hu/bi/Ted_Hildebrandt/New_Testament_Greek/Text/00-GreekArticlesWebBib.htm">Ted Hildebrandt</a> corrected and improved by Harmai G&aacute;bor, and the parsing information of Maurice A.&nbsp;Robinson.<br />
The text of Tischendorf (8th edition; 1869-1872) is by Ulrik Petersen.<br />
The Byzantine text is by Maurice A.&nbsp;Robinson (2005 edition).<br />
The links to the audio files are by Greeklatinaudio (Westcott and Hort text, modern pronunciation).</p>

<p>For comments and questions, write to <i>info</i> at this domain.</p>
<?}?>

</body>
</html>
<?
}
?>
