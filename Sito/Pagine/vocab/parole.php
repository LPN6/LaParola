<?
if (isset($_REQUEST["parola"])) {
  $parola = $_REQUEST["parola"];
  $parola = str_replace("<", "", $parola); // affinché tag HTML non possono essere inseriti nella pagina
  $parola = str_replace(">", "", $parola);
  $parola = str_replace("\"", "»", $parola);
}
else
  header("Location: http://".$_SERVER['HTTP_HOST']."/vocab/");

include("../conn.php");
$sql = "SELECT Parola,Traslit,Definizione FROM Vocabolario WHERE Traslit=\"".htmlentities($parola)."\" ORDER BY Traslit";
$sql1 = $sql;
if ($ris = mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris)==0 || $parola=="oÙ" || htmlentities($parola)=="&ecirc;" || htmlentities($parola)=="&ocirc;") {
    $sql = "SELECT Parola,Traslit,Definizione FROM Vocabolario WHERE BINARY Parola=\"$parola\" ORDER BY Traslit";
    $ris = mysqli_query($conn, "$sql");
  }
  $rifare = 0;
  if ($ris) {
      if (mysqli_num_rows($ris)==0 || (strlen($parola)==1 && ord($parola[0])>=97 && ord($parola[0])<=122))
        $rifare = 1; 
  }
  else {
    $rifare = 1;
  }
  if ($rifare>0.5) {
    if ($parola=="t")
        $sql = "SELECT Parola,Traslit,Definizione FROM Vocabolario WHERE (Traslit LIKE \"t%\") AND NOT (Traslit LIKE \"th%\") ORDER BY Traslit";
    else if ($parola=="p")
        $sql = "SELECT Parola,Traslit,Definizione FROM Vocabolario WHERE (Traslit LIKE \"p%\") AND NOT (Traslit LIKE \"ps%\") ORDER BY Traslit";
    else
        $sql = "SELECT Parola,Traslit,Definizione FROM Vocabolario WHERE (Traslit LIKE \"".htmlentities($parola)."%\") OR (Traslit LIKE \"h".htmlentities($parola)."%\") ORDER BY Traslit";
    $ris = mysqli_query($conn, "$sql");
  }
}
else
  errore2("interrogazione database per il vocabolario");

$parolatras = $parola;
if (mysqli_num_rows($ris)==1) {
  $row=mysqli_fetch_array ($ris);
  $parolatras = $row["Traslit"];
}

function convperc($a) {
// bisogna anche cambiare ConvPerc in makevocab.vbp se cambi qui
  $b = "";
  for ($i=0; $i<strlen($a); $i++) {
    if (ord($a[$i])>127)
      $b .= "%".strtoupper(dechex(ord($a[$i])));
    else
      $b .= $a[$i];
  }
  return $b;
}

function convUnicode($a) {
    while (strpos($a,"<span class=\"greco\">")!==false) {
        $pos1 = strpos($a,"<span class=\"greco\">");
        $pos2 = strpos($a, "</span>", $pos1);
        $a = substr($a, 0, $pos1)."<span class=\"unimed\">".convUnicodeP(substr($a, $pos1+20, $pos2-$pos1-20)).substr($a,$pos2);
    }
    return $a;
}

function convUnicodeP($a) {
    //$a = str_replace(array("&egrave;", "&Ograve;"), array("è", "Ò"), $a);
    $a = html_entity_decode($a, ENT_QUOTES | ENT_HTML401, 'ISO8859-15');
    $b = "";
    for ($i=0; $i<strlen($a); $i++) {
    switch ($a[$i]) {
    case "a":
        $b .= "&#945;";
        break;
    case "b":
        $b .= "&#946;";
        break;
    case "g":
        $b .= "&#947;";
        break;
    case "d":
        $b .= "&#948;";
        break;
    case "e":
        $b .= "&#949;";
        break;
    case "z":
        $b .= "&#950;";
        break;
    case "h":
        $b .= "&#951;";
        break;
    case "q":
        $b .= "&#952;";
        break;
    case "i":
        $b .= "&#953;";
        break;
    case "k":
        $b .= "&#954;";
        break;
    case "l":
        $b .= "&#955;";
        break;
    case "m":
        $b .= "&#956;";
        break;
    case "n":
        $b .= "&#957;";
        break;
    case "x":
        $b .= "&#958;";
        break;
    case "o":
        $b .= "&#959;";
        break;
    case "p":
        $b .= "&#960;";
        break;
    case "r":
        $b .= "&#961;";
        break;
    case "j":
        $b .= "&#962;";
        break;
    case "s":
        $b .= "&#963;";
        break;
    case "t":
        $b .= "&#964;";
        break;
    case "u":
        $b .= "&#965;";
        break;
    case "f":
        $b .= "&#966;";
        break;
    case "c":
        $b .= "&#967;";
        break;
    case "y":
        $b .= "&#968;";
        break;
    case "w":
        $b .= "&#969;";
        break;
    case "/":
        $b .= "&#884;"; // '
        break;
    case ":":
        $b .= "&#903;"; // ;
        break;
    case "A":
        $b .= "&#913;";
        break;
    case "B":
        $b .= "&#914;";
        break;
    case "G":
        $b .= "&#915;";
        break;
    Case "D":
        $b .= "&#916;";
        break;
    Case "E":
        $b .= "&#917;";
        break;
    Case "Z":
        $b .= "&#918;";
        break;
    Case "H":
        $b .= "&#919;";
        break;
    Case "Q":
        $b .= "&#920;";
        break;
    Case "I":
        $b .= "&#921;";
        break;
    Case "K":
        $b .= "&#922;";
        break;
    Case "L":
        $b .= "&#923;";
        break;
    Case "M":
        $b .= "&#924;";
        break;
    Case "N":
        $b .= "&#925;";
        break;
    Case "X":
        $b .= "&#926;";
        break;
    Case "O":
        $b .= "&#927;";
        break;
    Case "P":
        $b .= "&#928;";
        break;
    Case "R":
        $b .= "&#929;";
        break;
    Case "S":
        $b .= "&#931;";
        break;
    Case "T":
        $b .= "&#932;";
        break;
    Case "U":
        $b .= "&#933;";
        break;
    Case "F":
        $b .= "&#934;";
        break;
    Case "C":
        $b .= "&#935;";
        break;
    Case "Y":
        $b .= "&#936;";
        break;
    Case "W":
        $b .= "&#937;";
        break;
    Case "Ž":
        $b .= "&#970;"; // i di
        break;
    Case "ã":
        $b .= "&#971;"; // u di
        break;
    Case "¢":
        $b .= "&#7936;"; // a close
        break;
    Case "¡":
        $b .= "&#7937;"; // a open
        break;
    Case "¨":
        $b .= "&#7938;"; // a close down
        break;
    Case "§":
        $b .= "&#7939;"; // a open down
        break;
    Case "¥":
        $b .= "&#7940;"; // a close up
        break;
    Case "¤":
        $b .= "&#7941;"; // a open up
        break;
    Case "«":
        $b .= "&#7942;"; // a close hat
        break;
    Case "qq":
        $b .= "&#7943;"; // a open hat
        break;
    Case "™":
        $b .= "&#7952;"; // e close
        break;
    Case "˜":
        $b .= "&#7953;"; // e open
        break;
    Case "ž":
        $b .= "&#7955;"; // e open down
        break;
    Case "œ":
        $b .= "&#7956;"; // e close up
        break;
    Case "›":
        $b .= "&#7957;"; // e open up
        break;
    Case "º":
        $b .= "&#7968;"; // eta close
        break;
    Case "¹":
        $b .= "&#7969;"; // eta open
        break;
    Case "À":
        $b .= "&#7970;"; // eta close down
        break;
    Case "¿":
        $b .= "&#7971;"; // eta open down
        break;
    Case "½":
        $b .= "&#7972;"; // eta close up
        break;
    Case "¼":
        $b .= "&#7973;"; // eta open up
        break;
    Case "Ã":
        $b .= "&#7974;"; // eta close hat
        break;
    Case "Â":
        $b .= "&#7975;"; // eta open hat
        break;
    Case "„":
        $b .= "&#7984;"; // i close
        break;
    Case "ƒ":
        $b .= "&#7985;"; // i open
        break;
    Case "qqq":
        $b .= "&#7986;"; // i close down
        break;
    Case "‰":
        $b .= "&#7987;"; // i open down
        break;
    Case "‡":
        $b .= "&#7988;"; // i close up
        break;
    Case "†":
        $b .= "&#7989;"; // i open up
        break;
    Case "":
        $b .= "&#7990;"; // i hat close
        break;
    Case "Œ":
        $b .= "&#7991;"; // i hat open
        break;
    Case "Ñ":
        $b .= "&#8000;"; // o close
        break;
    Case "Ð":
        $b .= "&#8001;"; // o open
        break;
    Case "×":
        $b .= "&#8002;"; // o close down
        break;
    Case "Ö":
        $b .= "&#8003;"; // o open down
        break;
    Case "Ô":
        $b .= "&#8004;"; // o close up
        break;
    Case "Ó":
        $b .= "&#8005;"; // o open up
        break;
    Case "Ù":
        $b .= "&#8016;"; // u close
        break;
    Case "Ø":
        $b .= "&#8017;"; // u open
        break;
    Case "ß":
        $b .= "&#8018;"; // u close down
        break;
    Case "Þ":
        $b .= "&#8019;"; // u open down
        break;
    Case "Ü":
        $b .= "&#8020;"; // u close up
        break;
    Case "Û":
        $b .= "&#8021;"; // u open up
        break;
    Case "â":
        $b .= "&#8022;"; // u hat close
        break;
    Case "á":
        $b .= "&#8023;"; // u hat open
        break;
    Case "ç":
        $b .= "&#8032;"; // omega close
        break;
    Case "æ":
        $b .= "&#8033;"; // omega open
        break;
    Case "í":
        $b .= "&#8034;"; // omega close down
        break;
    Case "qqq2":
        $b .= "&#8035;"; // omega open down
        break;
    Case "ê":
        $b .= "&#8036;"; // omega close up
        break;
    Case "é":
        $b .= "&#8037;"; // omega open up
        break;
    Case "ð":
        $b .= "&#8038;"; // omega close hat
        break;
    Case "ï":
        $b .= "&#8039;"; // omega open hat
        break;
    Case "¦":
        $b .= "&#8048;"; // a down
        break;
    Case "£":
        $b .= "&#8049;"; // a up
        break;
    Case "":
        $b .= "&#8050;"; // e down
        break;
    Case "š":
        $b .= "&#8051;"; // e up
        break;
    Case "¾":
        $b .= "&#8052;"; // eta down
        break;
    Case "»":
        $b .= "&#8053;"; // eta up
        break;
    Case "ˆ":
        $b .= "&#8054;"; // i down
        break;
    Case "…":
        $b .= "&#8055;"; // i up
        break;
    Case "Õ":
        $b .= "&#8056;"; // o down
        break;
    Case "Ò":
        $b .= "&#8057;"; // o up
        break;
    Case "Ý":
        $b .= "&#8058;"; // u down
        break;
    Case "Ú":
        $b .= "&#8059;"; // u up
        break;
    Case "ë":
        $b .= "&#8060;"; // omega down
        break;
    Case "è":
        $b .= "&#8061;"; // omega up
        break;
    Case "°":
        $b .= "&#8068;"; // a close up i
        break;
    Case "¯":
        $b .= "&#8069;"; // a open up i
        break;
    Case "qqq":
        $b .= "&#8070;"; // a close hat i
        break;
    Case "µ":
        $b .= "&#8071;"; // a open hat i
        break;
    Case "Æ":
        $b .= "&#8080;"; // eta close i
        break;
    Case "Å":
        $b .= "&#8081;"; // eta open i
        break;
    Case "É":
        $b .= "&#8084;"; // eta close up i
        break;
    Case "qqq":
        $b .= "&#8085;"; // eta open up i
        break;
    Case "Ï":
        $b .= "&#8086;"; // eta close hat i
        break;
    Case "Î":
        $b .= "&#8087;"; // eta open hat i
        break;
    Case "ò":
        $b .= "&#8096;"; // omega close i
        break;
    Case "ñ":
        $b .= "&#8097;"; // omega open i
        break;
    Case "ú":
        $b .= "&#8103;"; // omega open hat i
        break;
    Case "v":
        $b .= "&#8115;"; // a i
        break;
    Case "®":
        $b .= "&#8116;"; // a i up
        break;
    Case "©":
        $b .= "&#8118;"; // a hat
        break;
    Case "´":
        $b .= "&#8119;"; // a hat i
        break;
    Case "qqq":
        $b .= "&#8131;"; // eta down i
        break;
    Case "V":
        $b .= "&#8131;"; // eta i
        break;
    Case "Ç":
        $b .= "&#8132;"; // eta up i
        break;
    Case "Á":
        $b .= "&#8134;"; // eta i
        break;
    Case "Í":
        $b .= "&#8135;"; // eta i cap
        break;
    Case "":
        $b .= "&#8146;"; // i dots down
        break;
    Case "":
        $b .= "&#8147;"; // i dots up
        break;
    Case "‹":
        $b .= "&#8150;";
        break;
    Case "å":
        $b .= "&#8163;"; // u, .. down
        break;
    Case "ä":
        $b .= "&#8163;"; // u, .. up
        break;
    Case "¸":
        $b .= "&#8164;"; // r close
        break;
    Case "·":
        $b .= "&#8165;"; // r open
        break;
    Case "à":
        $b .= "&#8166;"; // u hat
        break;
    Case "J":
        $b .= "&#8179;"; // omega i
        break;
    Case "ó":
        $b .= "&#8180;"; // omega up i
        break;
    Case "î":
        $b .= "&#8182;"; // omega hat
        break;
    Case "ù":
        $b .= "&#8183;"; // omega hat i
        break;
    Case "”": // close up
        switch ($a[$i+1]) {
        Case "A":
            $b .= "&#7948;";
            break;
        Case "E":
            $b .= "&#7964;";
            break;
        Case "H":
            $b .= "&#7980;";
            break;
        }
        $i++;
        break;
    Case "–": // close down
        switch ($a[$i+1]) {
        Case "H":
            $b .= "&#7978;";
            break;
        default: // trattino normale
            $b .= $a[$i];
            $i--;
            break;
        }
        $i++;
        break;
    Case "`":
        switch ($a[$i+1]) {
        Case "A":
            $b .= "&#7945;";
            break;
        Case "E":
            $b .= "&#7961;";
            break;
        Case "H":
            $b .= "&#7977;";
            break;
        Case "I":
            $b .= "&#7993;";
            break;
        Case "O":
            $b .= "&#8009;";
            break;
        Case "R":
            $b .= "&#8172;";
            break;
        Case "U":
            $b .= "&#8025;";
            break;
        Case "W":
            $b .= "&#8041;";
            break;
        }
        $i++;
        break;
    Case "'":
        switch ($a[$i+1]) {
        Case " ":
        case "":
        case "]":
        case ".":
        case ",":
        //Chr$(2)
            $b .= "'";
            $i--;
            break;
        Case "A":
            $b .= "&#7944;";
            break;
        Case "E":
            $b .= "&#7960;";
            break;
        Case "H":
            $b .= "&#7976;";
            break;
        Case "I":
            $b .= "&#7992;";
            break;
        Case "O":
            $b .= "&#8008;";
            break;
        Case "W":
            $b .= "&#8040;";
            break;
        }
        $i++;
        break;
    Case "“": // open up
        switch ($a[$i+1]) {
        Case "A":
            $b .= "&#7949;";
            break;
        Case "E":
            $b .= "&#7965;";
            break;
        Case "O":
            $b .= "&#8013;";
            break;
        }
        $i++;
        break;
    Case "’": // close hat
        switch ($a[$i+1]) {
        Case "A":
            $b .= "&#7950;";
            break;
        Case "W":
            $b .= "&#8046;";
            break;
        }
        $i++;
        break;
    default:
        $b .= $a[$i];
        break;
    }
    }
    return $b;
}

$descriz = $parolatras." - la definizione";
$key = $parolatras.",greco,vocabolario,dizionario,greco-italiano,significato,Nuovo Testamento,NT,dizionario greco";
$titolo = $parolatras;
$sezione = "Vocabolario greco";
$sezioneurl = "/vocab/";
require("../capo.php");
//echo "<p>$sql1</p>";
  if (mysqli_num_rows($ris)==0) {
    echo "<p>Nessuna parola simile a o che inizia con '".htmlentities($parola)."' &egrave; stata trovata. Prova di nuovo...</p>";
  }
  if (mysqli_num_rows($ris)==1) {
    echo StripSlashes(convUnicode($row["Definizione"]));
  }
  if (mysqli_num_rows($ris)>1) {
    //echo htmlentities($parola);
    echo "<p>Le seguenti parole iniziano con '".htmlentities($parola)."'. Scegli quella desiderata:</p><p>";
    while ($row=mysqli_fetch_array ($ris)) {
      $parolatrovata = $row["Parola"];
      echo "<a href=\"parole.php?parola=".convperc($parolatrovata)."\"><span class=\"unimed\">".convUnicodeP($parolatrovata)."</span></a> (".$row["Traslit"].")<br />";
    }
    echo "</p>";
  }
require("indvocab.php");
require("../piede.php");
?>
