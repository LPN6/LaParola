<?
include("conn.php");
include("vistesto.php");

function ricfrase($frase,$versione,$brano,$nBraniInizio=1,$nBraniFine=0,$formato_rif="dv",$altrilink=1, $popup="n", $homepage="n") {
global $libri_abb;

		if ($formato_rif=="auto" && $versione=="C.E.I.")
			$formato_rif = "vp";
    if (empty($versione))
        $versione = "Nuova Riveduta";
    if (strlen($versione)==0)
        $versione = "Nuova Riveduta";
    $branospeciale = 0;
    if (empty($brano)) {
        $brano = "gen1:1-ap22:21";
        $branospeciale = -1;
    }
    if (strlen($brano)==0) {
        $brano = "gen1:1-ap22:21";
        $branospeciale = -1;
    }
    if (strtolower($brano)=="vt") {
        $brano = "gen1:1-mal4:6";
        $branospeciale = 1;
    }
    if (strtolower($brano)=="nt") {
        $brano = "mt1:1-ap22:21";
        $branospeciale = 2;
    }
    $brano3 = converti_rif($brano);
    if (strlen($brano3)==0) {
        echo "Non capisco il riferimento ".$brano.".";
        $brano = "gen1:1-ap22:21";
        $brano3 = converti_rif($brano);
        if (strlen($brano3)==0) {
            echo "Ci sono problemi con il collegamento al database.";
            return;
        }
    }
    $versione_id = analizza_versione($versione);
    if ($versione_id[1]=='0') {
        echo "Non capisco la versione ".$versione.".";
        $versione = "Nuova Riveduta";
        $versione_id = analizza_versione($versione);
        if ($versione_id[1]=='0') {
            echo "Ci sono problemi con il collegamento al database.";
            return;
        }
    }
    echo "<h2>".str_replace("\\\\","&#92;",$frase)."</h2>";
    if ($branospeciale==0) {
        if ($homepage=="s")
            echo "<h3>in ".$brano." ($versione)</h3>";
        else
            echo "<h3>in ".utf8_decode($brano)." ($versione)</h3>";
    }
    if ($branospeciale==1)
        echo "<h3>nell'Antico Testamento ($versione)</h3>";
    if ($branospeciale==2)
        echo "<h3>nel Nuovo Testamento ($versione)</h3>";

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
    $frase = str_replace("/ ", "/", $frase);
    $frase = str_replace("\\ ", "\\", $frase);
    $frase = str_replace("* ", "*", $frase);
    $frase = str_replace("# ", "#", $frase);
    $frase = strtolower($frase);
    for ($i=1; $i<=9; $i++) {
        $frase = str_replace($i." ", $i, $frase);
        $frase = str_replace(" ".$i, $i, $frase);
    }
    if (strlen($frase)==0)
        $errfrase = "L'espressione da ricercare &egrave; vuota.";
    elseif (!Lettera($frase[0]) && $frase[0]!="/" && $frase[0]!="*" && $frase[0]!="#" && $frase[0]!="\\" && $frase[0]!="(")
        $errfrase = "Il primo carattere deve essere una lettera, o uno dei caratteri (,*, #, / o \\.";
    if (strpos($frase, "|~")>0)
        $errfrase = "NON non pu&ograve; seguire O.";

    if (strlen($errfrase)>0) {
        echo "<p><strong>Errore di sintassi nell'espressione da ricercare:</strong><br />".$errfrase."</p>";
        return;
    }

    $versetti = array();
    $versetti = trova_frase($frase,substr($versione_id,1));
    reset($versetti);
    $versettib = array();
    if ($branospeciale==-1)
        $versettib = $versetti;
    else {
      for ($i=0; $i<strlen($brano3); $i+=6) {
          $lib0 = ord(substr($brano3,$i,1));
          $cap0 = ord(substr($brano3,$i+1,1));
          $vers0 = ord(substr($brano3,$i+2,1));
          $lib1 = ord(substr($brano3,$i+3,1));
          $cap1 = ord(substr($brano3,$i+4,1));
          $vers1 = ord(substr($brano3,$i+5,1));
          for ($j=count($versetti)-1; $j>=0; $j--) {
              $lib = ord(substr($versetti[$j],0,1));
              $cap = ord(substr($versetti[$j],1,1));
              $vers = ord(substr($versetti[$j],2,1));
              if ($lib<$lib0 || ($lib==$lib0 && $cap<$cap0) || ($lib==$lib0 && $cap==$cap0 && $vers<$vers0)) {
                break;
              }
              elseif ($lib>$lib1 || ($lib==$lib1 && $cap>$cap1) || ($lib==$lib1 && $cap==$cap1 && $vers>$vers1)) {
              }
              else {
                $versettib[] = $versetti[$j];
              }
          }
      }
      $versettib = array_unique($versettib);
      $versettib = array_values($versettib);
      sort($versettib);
    }

    $nBrani = count($versettib);
    if ($nBrani==0) {
        echo "<p>Questa espressione non appare nella versione <i>$versione</i>";
        if ($branospeciale!=-1)
            echo " nel brano selezionato";
        echo ".</p>";
    }
    else {
        echo "<p>Questa espressione appare in ".$nBrani." versett".(($nBrani>1)?"i":"o");
        $ultimoversetto = $nBraniFine;
        if ($nBraniFine==0 || $nBraniFine>$nBrani)
            $ultimoversetto = $nBrani;
        if ($nBraniInizio>1 || ($nBraniFine<$nBrani && $nBraniFine>0))
            echo " (i versetti ".$nBraniInizio."-".$ultimoversetto." sono mostrati)";

        echo ":</p>\n";
    }
    
    $imax = $nBraniFine;
    if ($nBrani<$nBraniFine || $nBraniFine==0)
        $imax = $nBrani;

    // bisogna modificare questo codice anche altrove in questo file se è modificato qui
    if ($nBraniInizio>1) {
        $nuovonBraniInizio = 2*$nBraniInizio - $nBraniFine - 1;
        $nuovonBraniInizioMostrato = $nuovonBraniInizio;
        if ($nuovonBraniInizio<1)
            $nuovonBraniInizioMostrato = 1;
        $nuovonBraniFine = $nBraniInizio - 1;
        echo "<h3><a href=\"".$_SERVER["PHP_SELF"]."?frase=".urlencode(str_replace("\\\\","\\",$frase))."&versione=$versione&brano=".urlencode($brano)."&nBraniInizio=$nuovonBraniInizio&nBraniFine=$nuovonBraniFine\">Mostra i versetti ".$nuovonBraniInizioMostrato."-".$nuovonBraniFine."</a></h3>";
    }
    if ($imax<$nBrani) {
        $nuovonBraniInizio = $nBraniFine + 1;
        $nuovonBraniFine = 2*$nBraniFine - $nBraniInizio + 1;
        $nuovonBraniFineMostrato = $nuovonBraniFine;
        if ($nuovonBraniFine>$nBrani)
            $nuovonBraniFineMostrato = $nBrani;
        if ($altrilink>0) {
          echo "<h3><a href=\"".$_SERVER["PHP_SELF"]."?frase=".urlencode(str_replace("\\\\","\\",$frase))."&versione=$versione&brano=".urlencode($brano)."&nBraniInizio=$nuovonBraniInizio&nBraniFine=$nuovonBraniFine\">Mostra i versetti ".$nuovonBraniInizio."-".$nuovonBraniFineMostrato."</a></h3>";
       }
    }

    echo "<!-- RESULT LIST START --><div id=\"brano\">";
    $dim_testo = isset($_COOKIE["dim_testo"])?$_COOKIE["dim_testo"]:0;
    if ($dim_testo>1)
        echo "<script type=\"text/javascript\">$('#brano').css('font-size',$dim_testo);</script>";

    for ($i=$nBraniInizio-1; $i<$imax; $i++) {
        $lib = ord(substr($versettib[$i],0,1));
        $cap = ord(substr($versettib[$i],1,1));
        $vers = ord(substr($versettib[$i],2,1));
        echo "<!-- RESULT ITEM START -->\n";
        visualizza_brano(substr($versione_id,1),$versione_id[0],$lib,$cap,$vers,$lib,$cap,$vers,$formato_rif);
        $rifcap = $libri_abb[$lib];
		if ($lib<>38 && $lib<>64 && $lib<>70 && $lib<>71 && $lib<>72)
			$rifcap = $rifcap." ".$cap;
        if ($altrilink>0) {
          echo "<p align=\"right\"><a href=\"/testo.php?riferimento=$rifcap:$vers&versioni[]=Nuova+Riveduta&versioni[]=C.E.I.&versioni[]=Nuova+Diodati&versioni[]=Riveduta+2020&versioni[]=Nuova+Riveduta+1994&versioni[]=Bibbia+della+Gioia&versioni[]=Riveduta&versioni[]=Ricciotti&versioni[]=Tintori&versioni[]=Martini&versioni[]=Diodati&versioni[]=CommentarioHenry&versioni[]=CommentarioNT&versioni[]=Commentario&versioni[]=CommentarioCalvino&versioni[]=CommentarioBarnes&versioni[]=CommentarioGinevra&versioni[]=CommentarioGill&versioni[]=CommentarioPulpito&versioni[]=CommentarioIllustratore&versioni[]=CommentarioMeyer&versioni[]=CommentarioTesoro&versioni[]=Riferimenti+incrociati\">$rifcap".($formato_rif=="vp"?",":":")."$vers in tutte le versioni</a> | ";
          echo "<a href=\"JavaScript:popup('$rifcap','$versione');\" onMouseOver=\"window.status='Visualizza contesto'; return true\" onMouseOut=\"window.status=''; return true\">Mostra capitolo</a> | ";
          echo "<a href=\"/app/?w1=bible&t1=local%3A".convversionetoapp($versione)."&v1=".convlibrotoapp($lib).$cap."_".$vers."\">Mostra contesto</a></p>"; // http://laparola/app/?w1=bible&t1=local%3Anr&v1=JL1_1
        }
        echo "\n<!-- RESULT ITEM END -->\n";
    }
    echo "</div><!-- RESULT LIST END -->";
    if ($nBrani>0 && $versione=="Bibbia della Gioia")
      echo "La Parola &egrave; Vita<br />Copyright &copy; 1981, 1994 di <a href=\"http://www.biblica.com/\">Biblica</a>, Inc.&reg;<br />Usato con permesso. Tutti i diritti riservati in tutto il mondo.";

    if ($popup=="n")
        echo pulsanti_dim();
     
// bisogna modificare questo codice anche altrove in questo file se è modificato qui
    if ($nBraniInizio>1) {
        $nuovonBraniInizio = 2*$nBraniInizio - $nBraniFine - 1;
        $nuovonBraniInizioMostrato = $nuovonBraniInizio;
        if ($nuovonBraniInizio<1)
            $nuovonBraniInizioMostrato = 1;
        $nuovonBraniFine = $nBraniInizio - 1;
        echo "<h3><a href=\"".$_SERVER["PHP_SELF"]."?frase=".urlencode(str_replace("\\\\","\\",$frase))."&versione=$versione&brano=".urlencode($brano)."&nBraniInizio=$nuovonBraniInizio&nBraniFine=$nuovonBraniFine\">Mostra i versetti ".$nuovonBraniInizioMostrato."-".$nuovonBraniFine."</a></h3>";
    }
    if ($imax<$nBrani) {
        $nuovonBraniInizio = $nBraniFine + 1;
        $nuovonBraniFine = 2*$nBraniFine - $nBraniInizio + 1;
        $nuovonBraniFineMostrato = $nuovonBraniFine;
        if ($nuovonBraniFine>$nBrani)
            $nuovonBraniFineMostrato = $nBrani;
        if ($altrilink>0) {
          echo "<h3><a href=\"".$_SERVER["PHP_SELF"]."?frase=".urlencode(str_replace("\\\\","\\",$frase))."&versione=$versione&brano=".urlencode($brano)."&nBraniInizio=$nuovonBraniInizio&nBraniFine=$nuovonBraniFine\">Mostra i versetti ".$nuovonBraniInizio."-".$nuovonBraniFineMostrato."</a></h3>";
       }
    }
}

function trova_parola($parola,$versione_id) {
global $conn;
$VersParola=array();
if ($parola=="") return $VersParola;

$sql = "SELECT Libro,Capitolo,Versetto FROM Parole,Apparenze,Versetti WHERE Parole.id_p=Apparenze.id_p AND Apparenze.id_v=Versetti.id_v AND id_t=$versione_id AND ";
switch ($parola[0]) {
case "/":
    $id = 0;
    $sql = "SELECT id_r FROM Radici WHERE Radice=\"".substr($parola,1)."\"";
    if ($ris=mysqli_query($conn, "$sql")) {
        if ($row=mysqli_fetch_array ($ris))
           $id = $row["id_r"];
    }
    else {
        errore2("interrogazione database per radice ($parola)");
    }
    $sql = "SELECT Libro,Capitolo,Versetto FROM Parole,Apparenze,Versetti WHERE Parole.id_p=Apparenze.id_p AND Parole.id_r=$id AND Apparenze.id_v=Versetti.id_v AND id_t=$versione_id";
    break;
case "\\":
    $id = 0;
    $sql = "SELECT id_r FROM Parole WHERE Parola=\"".substr($parola,1)."\"";
	// una volta, su Aruba (ma non localhost) bisognava mettere substr($parola,2) , ma non più. Non so perché, probabilmente un cambio in versione PHP
//	echo "<p>$parola $sql</p>";
    if ($ris=mysqli_query ($conn, "$sql")) {
        if ($row=mysqli_fetch_array ($ris))
          $id = $row["id_r"];
    }
    else {
        errore2("interrogazione database per radice ($parola)");
    }
    $sql = "SELECT Libro,Capitolo,Versetto FROM Parole,Apparenze,Versetti WHERE Parole.id_p=Apparenze.id_p AND id_r=$id AND Apparenze.id_v=Versetti.id_v AND id_t=$versione_id";
    break;
case "*":
    $sql .= "Parola LIKE \"".substr($parola,1)."%\"";
    break;
case "#":
    $sql .= "Parola LIKE \"%".substr($parola,1)."%\"";
    break;
default:
    $sql .= "Parola=\"$parola\"";
}

//$sql .= " ORDER BY Libro ASC,Capitolo ASC,Versetto ASC";
//echo "<p>$sql</p>";
if ($ris=mysqli_query ($conn, "$sql")) {
    while ($row=mysqli_fetch_array ($ris)) {
//      echo "<p>".$row["Libro"].$row["Capitolo"].$row["Versetto"]."</p>";
        $VersParola[] = chr($row["Libro"]).chr($row["Capitolo"]).chr($row["Versetto"]);
    }
}
else {
    errore2("interrogazione database per ricerca della parola $parola");
}
return $VersParola;
}

function trova_frase($frase,$versione_id) {
global $utf8;
    $utf8 = (isset($utf8)?$utf8:0);
    if ($utf8==0 && mb_detect_encoding($frase, 'UTF-8, ISO-8859-1')=='UTF-8')
        $frase = utf8_decode($frase);

    $versetti = array();
    $errfrase = "";
    if (strlen($frase)==0)
        return $versetti;
    if (!Lettera($frase[0]) && $frase[0]!="/" && $frase[0]!="*" && $frase[0]!="#" && $frase[0]!="\\" && $frase[0]!="(")
        $errfrase = "Il primo carattere dopo una parentesi deve essere una lettera, o uno dei caratteri (,*, #, / o \\.";
    if ($frase[0]=="(") {
        $i = 1;
        $nPar = 1;
        while ($i<strlen($frase)) {
            if ($frase[$i]=="(") $nPar++;
            if ($frase[$i]==")") $nPar--;
            if ($nPar==0) break;
            $i++;
        }
        if ($i == strlen($frase)) {
            $errfrase = "Le parentesi non sono giuste.";
        }
        else {
            $versetti = trova_frase(substr($frase,1,$i-1),$versione_id);
            $frase = substr($frase,$i+1);
        }
    }
    else {
        $i = 0;
        if ($frase[0]=="/" || $frase[0]=="*" || $frase[0]=="#")
            $i++;
        if ($frase[0]=="\\")
            $i += 2;
//        while ($i<strlen($frase) && (Lettera($frase[$i]) || $frase[$i]=="'" || $frase[$i]=="-"))
        while ($i<strlen($frase) && (Lettera($frase[$i]) || $frase[$i]=="-"))
            $i++;
        if ($i<strlen($frase)) { // per quelli che fanno una ricerca per l'uomo per esempio
            if ($frase[$i]=="'") {
                $i++;
                if ($i<strlen($frase) && $frase[$i]!=" ")
                    $frase = substr($frase,0,$i)." ".substr($frase,$i);
            }
        }
        $versetti = trova_parola(substr($frase,0,$i),$versione_id);
        $frase = substr($frase,$i);
    }
    while (strlen($frase)>0 && strlen($errfrase)==0) {
        $punteg = $frase[0];
        $frase = substr($frase,1);
        $verspar = array();
        if ($frase[0]=="(") {
            $i = 1;
            $nPar = 1;
            while ($i<strlen($frase)) {
                if ($frase[$i]=="(") $nPar++;
                if ($frase[$i]==")") $nPar--;
                if ($nPar==0) break;
                $i++;
            }
            if ($i == strlen($frase)) {
                $errfrase = "Le parentesi non sono giuste.";
                break;
            }
            $verspar = trova_frase(substr($frase,1,$i-1),$versione_id);
            $frase = substr($frase,$i+1);
        }
        else {
            $i = 0;
            if ($frase[0]=="/" || $frase[0]=="*" || $frase[0]=="#")
                $i++;
            if ($frase[0]=="\\")
                $i += 2;
            while ($i<strlen($frase) && (Lettera($frase[$i]) || $frase[$i]=="-"))
                $i++;
            if ($i<strlen($frase)) { // per quelli che fanno una ricerca per l'uomo per esempio
                if ($frase[$i]=="'") {
                    $i++;
                    if ($i<strlen($frase) && $frase[$i]!=" ")
                        $frase = substr($frase,0,$i)." ".substr($frase,$i);
                }
            }
            $verspar = trova_parola(substr($frase,0,$i),$versione_id);
            $frase = substr($frase,$i);
        }
        if ($punteg=="~")
            $versetti = array_diff($versetti,$verspar);
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
            $versetti = array_unique($versetti);
            $versetti = array_values($versetti);
            sort($versetti);
            sort($verspar);
            $versetti2 = array();
            $diff = ord($punteg)-ord("0");
            for ($i=0; $i<count($versetti); $i++) {
                $lib = ord(substr($versetti[$i],0,1));
                $cap = ord(substr($versetti[$i],1,1));
                $vers = ord(substr($versetti[$i],2,1));
                for ($j=0; $j<count($verspar); $j++) {
                    $libp = ord(substr($verspar[$j],0,1));
                    $capp = ord(substr($verspar[$j],1,1));
                    $versp = ord(substr($verspar[$j],2,1));
                    if ($libp<$lib || ($libp==$lib && $capp<$cap) || ($libp==$lib && $capp==$cap && $versp<$vers-$diff)) {
                    }
                    elseif ($libp>$lib || ($libp==$lib && $capp>$cap) || ($libp==$lib && $capp==$cap && $versp>$vers+$diff)) {
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
            $errfrase = "Non riconosco il carattere '".$punteg."' a questo punto nell'espressione.";
            break 2;
        }
    }

    if (strlen($errfrase)>0) {
        echo "<p><strong>Errore di sintassi nell'espressione da ricercare:</strong><br />".$errfrase."</p>";
        $versetti = array();
    }
    else {
        $versetti = array_unique($versetti);
        $versetti = array_values($versetti);
        sort($versetti);
    }

    return $versetti;
}

?>
