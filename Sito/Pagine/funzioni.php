<?
function errore($commando) {
global $conn;
    echo "Errore ".mysqli_errno($conn).": ".mysqli_error($conn)." in $commando.<br />\n";
    echo "Se l'errore persiste, scrivere a <a href=\"mailto:&#105;&#110;&#102;&#111;&#64;&#108;&#97;&#112;&#97;&#114;&#111;&#108;&#97;&#46;&#110;&#101;&#116;\">Richard Wilson</a>.";
}

function linkbib($riferimento) {
echo "<a href=\"JavaScript:popup('$riferimento');\" onmouseover=\"window.status='Visualizza $riferimento'; return true\" onmouseout=\"window.status=''; return true\">$riferimento</a>";
}

function domanda($r) {
global $conn;
  $titolo = "";
  $v2 = -1;
  if (strlen($r)<3) {
    $l1=0;$c1=0;$v1=0;$l2=0;$c2=0;
    $v2=$r;
  }
  else {
    $rif = converti_rif($r);
    if ($rif!="") {
        $l1=ord($rif[0]);
        $c1=ord($rif[1]);
        $v1=ord($rif[2]);
        $l2=ord($rif[3]);
        $c2=ord($rif[4]);
        $v2=ord($rif[5]);
    }
  }

	// vediamo se c'è un brano con esattamente questo riferimento (utile quando c'è un link al brano da un altro brano),
	// altrimenti scegliamo tutti i brani che contengono il riferimento
  if ($v2>=0) {
	$sql="SELECT * FROM Brani WHERE (Libro1=$l1 AND Capitolo1=$c1 AND Versetto1=$v1 AND Libro2=$l2 AND Capitolo2=$c2 AND Versetto2=$v2)";
	if ($ris2 = mysqli_query($conn, "$sql")) {
  		if (mysqli_num_rows($ris2)<>1) {
    		$sql="SELECT * FROM Brani WHERE (Libro1<$l2 OR (Libro1=$l2 AND (Capitolo1<$c2 OR (Capitolo1=$c2 AND Versetto1<=$v2)))) AND (Libro2>$l1 OR (Libro2=$l1 AND (Capitolo2>$c1 OR (Capitolo2=$c1 AND Versetto2>=$v1))))";
		}
	}
	else {
  		$sql="SELECT * FROM Brani WHERE (Libro1<$l2 OR (Libro1=$l2 AND (Capitolo1<$c2 OR (Capitolo1=$c2 AND Versetto1<=$v2)))) AND (Libro2>$l1 OR (Libro2=$l1 AND (Capitolo2>$c1 OR (Capitolo2=$c1 AND Versetto2>=$v1))))";
	}
    if ($ris = mysqli_query($conn, "$sql")) {
        if (mysqli_num_rows($ris)==1) {
            $row=mysqli_fetch_array($ris);
            $titolo = $row["Domanda"];
        }
    }
  }
  return $titolo;
}

function bd($rif) {
if (strlen($rif)==0)
    echo "<span class=\"B\">BD</span>";
else if (strlen($rif)<3)
    echo "<span class=\"B\"><a href=\"/brani/brani.php?b=".($rif+1)."\" target=\"_blank\">BD</a><span class=\"B2\">".domanda($rif)."</span></span>";
else
    echo "<span class=\"B\"><a href=\"/brani/brani.php?r=$rif\" target=\"_blank\">BD</a><span class=\"B2\">".domanda($rif)."</span></span>";    
}

function Lettera($c) {
  return (($c>="a" && $c<="z") || ord($c)>191);
}

function mysqli_result_lpn($res, $row, $field=0) {
    $res->data_seek($row);
    $datarow = $res->fetch_array();
    return $datarow[$field];
}

function convversionetoapp($versione) {
    $vapp = "nr"; // predefinito, se $v non trovato
	if ($versione=="Nuova Riveduta")
		$vapp = "nr";
	else if ($versione=="C.E.I.")
		$vapp = "cei1974";
	else if ($versione=="Nuova Diodati")
		$vapp = "nd";
    else if ($versione=="Riveduta 2020")
        $vapp = "re";
    else if ($versione=="Nuova Riveduta 1994")
        $vapp = "nr94";
	else if ($versione=="Bibbia della Gioia")
		$vapp = "lpv";
	else if ($versione=="Riveduta")
		$vapp = "luzzi";
    else if ($versione=="Ricciotti")
        $vapp = "ricciotti";
    else if ($versione=="Tintori")
        $vapp = "tintori";
	else if ($versione=="Martini")
		$vapp = "martini";
	else if ($versione=="Diodati")
		$vapp = "dio";
	else if ($versione=="Volgare")
		$vapp = "volg";
    return $vapp;
}

function converti_linkTestoContinuto($riferimento, $versioni) {
    $url3 = "https://www.laparola.net/app/?";
    $rifapp = "GN1_1";
    if (!empty($riferimento)) {
        $rif3 = converti_rif($riferimento);
        if (strlen($rif3)>=3) {
            $rifapp = convlibrotoapp(ord($rif3[0])).ord($rif3[1])."_".ord($rif3[2]);
        }
    }
    for ($i=1; $i<=count($versioni); $i++) {
        $abb=""; $tipo = "bible";
        if ($versioni[$i-1] == "Nuova Riveduta") {
            $abb = "nr";
        } elseif ($versioni[$i-1] == "C.E.I.") {
            $abb = "cei1974";
        } elseif ($versioni[$i-1] == "Nuova Diodati") {
            $abb = "nd";
        } elseif ($versioni[$i-1] == "Riveduta 2020") {
            $abb = "r2";
        } elseif ($versioni[$i-1] == "Nuova Riveduta 1994") {
            $abb = "nr94";
        } elseif ($versioni[$i-1] == "Bibbia della Gioia") {
            $abb = "lpv";
        } elseif ($versioni[$i-1] == "Riveduta") {
            $abb = "luzzi";
        } elseif ($versioni[$i-1] == "Ricciotti") {
            $abb = "ricciotti";
        } elseif ($versioni[$i-1] == "Tintori") {
            $abb = "tintori";
        } elseif ($versioni[$i-1] == "Martini") {
            $abb = "martini";
        } elseif ($versioni[$i-1] == "Diodati") {
            $abb = "dio";
        } elseif ($versioni[$i-1] == "Volgare") {
            $abb = "volgare";
        } elseif ($versioni[$i-1] == "Commentario") {
            $abb = "commabbrmh";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioNT") {
            $abb = "commnuovotest";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "Riferimenti incrociati") {
            $abb = "rifinc";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioPulpito") {
            $abb = "commpulpito";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioIllustratore") {
            $abb = "commillustratore";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioGill") {
            $abb = "commgill";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioBarnes") {
            $abb = "commbarnes";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioMeyer") {
            $abb = "commmeyer";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioTesoro") {
            $abb = "commtesoro";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioHenry") {
            $abb = "commhenrycompleto";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioCalvino") {
            $abb = "commcalvino";
            $tipo ="commentary";
        } elseif ($versioni[$i-1] == "CommentarioGinevra") {
            $abb = "commginevra";
            $tipo ="commentary";
        }
        $url3 .= "w".$i."=".$tipo."&t".$i."=local%3A".$abb;
        if ($rifapp != "")
            $url3 .= "&v".$i."=".$rifapp;
        $url3 .= "&";
    }
    if (count($versioni)>0)
        $url3 = substr($url3, 0, -1);
    return $url3;
}

function convlibrotoapp($b) {
        $libriapp = array(
"GN","EX","LV","NU","DT","JS","JG","RT","S1","S2","K1","K2","R1","R2","ER","NH","TB","JT","ET","M1","M2","JB","PS","PR","EC","SS","WS","SR","IS","JR","LM","BR","EK","DN","HS","JL","AM","OB","JH","MC","NM","HK","ZP","HG","ZC","ML",
"MT","MK","LK","JN","AC","RM","C1","C2","GL","EP","PP","CL","H1","H2","T1","T2","TT","PM","HB","JM","P1","P2","J1","J2","J3","JD","RV"
);
    return $libriapp[$b-1];
}

?>
