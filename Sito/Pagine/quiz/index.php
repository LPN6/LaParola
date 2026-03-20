<?
//$tema = (int)(isset($_REQUEST["tema"])?$_REQUEST["tema"]:0);
$numero = (int)(isset($_REQUEST["numero"])?$_REQUEST["numero"]:0);
$giusto = (int)(isset($_REQUEST["giusto"])?$_REQUEST["giusto"]:0);
$sbagliato = (int)(isset($_REQUEST["sbagliato"])?$_REQUEST["sbagliato"]:0);
$giustoSempre = (int)(isset($_COOKIE["giustoSempre"])?$_COOKIE["giustoSempre"]:0);
$sbagliatoSempre = (int)(isset($_COOKIE["sbagliatoSempre"])?$_COOKIE["sbagliatoSempre"]:0);
$difficolta = (int)(isset($_REQUEST["difficolta"])?$_REQUEST["difficolta"]:-1);
if ($difficolta<0)
    $difficolta = (int)(isset($_COOKIE["difficolta"])?$_COOKIE["difficolta"]:0); 
$r1 = (isset($_REQUEST["r1"])?$_REQUEST["r1"]:"");
  $r1 = str_replace("<", "", $r1); // affinché tag HTML non possono essere inseriti nella pagina
  $r1 = str_replace(">", "", $r1);
$r2 = (isset($_REQUEST["r2"])?$_REQUEST["r2"]:"");
  $r2 = str_replace("<", "", $r2); // affinché tag HTML non possono essere inseriti nella pagina
  $r2 = str_replace(">", "", $r2);
$r3 = (isset($_REQUEST["r3"])?$_REQUEST["r3"]:"");
  $r3 = str_replace("<", "", $r3); // affinché tag HTML non possono essere inseriti nella pagina
  $r3 = str_replace(">", "", $r3);
$r4 = (isset($_REQUEST["r4"])?$_REQUEST["r4"]:"");
  $r4 = str_replace("<", "", $r4); // affinché tag HTML non possono essere inseriti nella pagina
  $r4 = str_replace(">", "", $r4);

include("../conn.php");

if ($numero > 0) {
	$sql = "SELECT * FROM Quiz WHERE id_d=$numero";
	if ($ris = mysqli_query($conn, "$sql")) {
		$row = mysqli_fetch_array ($ris);
		$rispostaGiusta = $row["risposta"];
		switch ($rispostaGiusta) {
			case 1:
				$risposta = $row["risposta1"];
				break;
			case 2:
				$risposta = $row["risposta2"];
				break;
			case 3:
				$risposta = $row["risposta3"];
				break;
			case 4:
				$risposta = $row["risposta4"];
				break;
		}
		$giustoTutti = $row["giuste"];
		$sbagliatoTutti = $row["sbagliate"];
		if ((!empty($r1) && $rispostaGiusta==1) || (!empty($r2) && $rispostaGiusta==2) || (!empty($r3) && $rispostaGiusta==3) || (!empty($r4) && $rispostaGiusta==4)) {
			++$giustoSempre;
			++$giusto;
			++$giustoTutti;
			$sql_update = "UPDATE Quiz SET giuste=".$giustoTutti." WHERE id_d=".$numero;
		}
		else {
			++$sbagliatoSempre;
			++$sbagliato;
			++$sbagliatoTutti;
			$sql_update = "UPDATE Quiz SET sbagliate=".$sbagliatoTutti." WHERE id_d=".$numero;
		}
		$ris_update = mysqli_query($conn, $sql_update);

		SetCookie("giustoSempre", $giustoSempre, time()+3600000);
		SetCookie("sbagliatoSempre", $sbagliatoSempre, time()+3600000);
        SetCookie("difficolta",$difficolta, time()+3600000);
	}
}

$descriz = "Un quiz biblico";
$key = "quiz,biblico,quiz biblici";
$titolo = "Quiz";
$sezione = "Strumenti";
require("../capo.php");
?>

<h1>Quiz biblico</h1>
<?
function perc($g, $s) {
	$r = "$g giust".($g==1?"a":"e")." e $s sbagliat".($s==1?"a":"e");
	if ($g+$s>0)
		$r .= " (".round(100.0*$g/($g+$s))."%)";
	$r .= "</p>\n";
	return $r;
}

if ($numero == 0)
	echo "<p class=\"primalettera\">Questo quiz biblico ti propone diverse domande sulla Bibbia, e dovrai scegliere la risposta giusta dalle quattro proposte. Clicca sul pulsante della risposta giusta.</p>";
else {
	echo "<h2>Risposta</h2>";
//    echo "<p>$difficolta</p>";
	echo "<p><i>".$row["domanda"]."</i></p>\n";
	if ((!empty($r1) && $rispostaGiusta==1) || (!empty($r2) && $rispostaGiusta==2) || (!empty($r3) && $rispostaGiusta==3) || (!empty($r4) && $rispostaGiusta==4)) {
		echo "<p>RISPOSTA GIUSTA - ".$risposta."</p>\n";
	}
	else {
		echo "<p>RISPOSTA SBAGLIATA - la risposta giusta &egrave;: ".$risposta."</p>\n";		
	}
	echo "<p>".$row["spiegazione"]."</p>";
	echo "<h3>Punteggio</h3>\n";
	echo "<p><i>Questa sessione:</i> ".perc($giusto, $sbagliato);
	echo "<p><i>Sempre:</i> ".perc($giustoSempre, $sbagliatoSempre);
	echo "<p><i>Questa domanda per tutti:</i> ".perc($giustoTutti, $sbagliatoTutti);
}

echo "<h2>Domanda</h2>";

$sql = "SELECT * FROM Quiz";
//if ($tema != 0)
//	$sql .= " WHERE id_t=".$tema;
//	$sql .= " WHERE id_d=787";	
$sql .= " ORDER BY RAND() LIMIT 1";

if ($ris = mysqli_query($conn, "$sql")) {
	$row = mysqli_fetch_array($ris);
    $min = 0; $max = 1;
    if ($difficolta==1) {
        $min=0.623;
    }
    if ($difficolta==2) {
        $min=0.422;
        $max=0.623;
    }
    if ($difficolta==3) {
        $max=0.422;
    }
    
    if ($row["giuste"] + $row["sbagliate"]>0) {
      $p = $row["giuste"] / ($row["giuste"] + $row["sbagliate"]);
      while ($p<$min || $p>$max) {
          $ris = mysqli_query($conn, "$sql");
          $row = mysqli_fetch_array($ris);
          $p = $row["giuste"] / ($row["giuste"] + $row["sbagliate"]);
      }
    }
     
	echo "<form method=\"post\" action=\"/quiz/\" name=\"form_domanda\">\n";
	$nTema = $row["id_t"];
	if ($ris2 = mysqli_query($conn, "SELECT * FROM QuizTemi WHERE id_t=".$nTema)) {
		$row2 = mysqli_fetch_array($ris2);
		echo "<h3>(Categoria: ".$row2["tema"].")</h3>\n";	
	}
    echo "<p><i>".$row["domanda"]."</i></p>\n";
	echo "<p><input class=\"submit\" type=\"submit\" name=\"r1\" value=\"1.\" />&nbsp;".$row["risposta1"]."</p>\n";
	echo "<p><input class=\"submit\" type=\"submit\" name=\"r2\" value=\"2.\" />&nbsp;".$row["risposta2"]."</p>\n";
	echo "<p><input class=\"submit\" type=\"submit\" name=\"r3\" value=\"3.\" />&nbsp;".$row["risposta3"]."</p>\n";
	echo "<p><input class=\"submit\" type=\"submit\" name=\"r4\" value=\"4.\" />&nbsp;".$row["risposta4"]."</p>\n";
    echo "<p>Difficolt&agrave; della prossima domanda:<select name=\"difficolta\">\n";
    echo "<option ".($difficolta==0?"selected=\"selected\" ":"")."value=\"0\">Qualsiasi</option>\n";
    echo "<option ".($difficolta==1?"selected=\"selected\" ":"")."value=\"1\">Facile</option>\n";
    echo "<option ".($difficolta==2?"selected=\"selected\" ":"")."value=\"2\">Media</option>\n";
    echo "<option ".($difficolta==3?"selected=\"selected\" ":"")."value=\"3\">Difficile</option>\n</select></p>\n";
	echo "<p><input type=\"hidden\" name=\"numero\" value=\"".$row["id_d"]."\" />\n";
//	echo "<input type=\"hidden\" name=\"tema\" value=\"".$tema."\" />\n";
	echo "<input type=\"hidden\" name=\"giusto\" value=\"".$giusto."\" />\n";
	echo "<input type=\"hidden\" name=\"sbagliato\" value=\"".$sbagliato."\" /></p>\n";
	echo "</form>\n";
}

// TODO
// convertire a HTML &egrave; i temi
// libro l'ultima tema, controllare gli altri nomi
// unire i temi più piccoli

require("../piede.php");
?>
