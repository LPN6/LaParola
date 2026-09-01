<?
include("../conn.php");

if (isset($_POST['libro']))
    $libro = $_POST['libro'];
else
    $libro = 1;
if (isset($_POST['capitolo']))
    $capitolo = $_POST['capitolo'];
else
    $capitolo = 1;

if (isset($_POST['righe']))
    $righe = $_POST['righe'];
else
    $righe = 127;
$nr06 = $righe & 1;
$binaryLength = strlen(decbin($righe));
$nr94 = (($binaryLength > 1 && ($righe & (1 << 1))) == 1);
$r2 = (($binaryLength > 2 && ($righe & (1 << 2))) == 1);
$rad = (($binaryLength > 3 && ($righe & (1 << 3))) == 1);
$traslit = (($binaryLength > 4 && ($righe & (1 << 4))) == 1);
$radtraslit = (($binaryLength > 5 && ($righe & (1 << 5))) == 1);
$gramm = (($binaryLength > 6 && ($righe & (1 << 6))) == 1);

if (isset($_POST['opzioni']))
    $opzioni = $_POST['opzioni'];
else
    $opzioni = 3;
$evidenziaDiff = $opzioni & 1;
$binaryLength = strlen(decbin($opzioni));
$evidenzaTC = (($binaryLength > 1 && ($opzioni & (1 << 1))) == 1);
$evidenziaDiffPicc = (($opzioni & (1 << 2)) ? 1 : 0);

$mapping = [
        "\u{03AC}" => "\u{1F71}", 
        "\u{03AD}" => "\u{1F73}", 
        "\u{03AE}" => "\u{1F75}", 
        "\u{03AF}" => "\u{1F77}", 
        "\u{03CC}" => "\u{1F79}", 
        "\u{03CD}" => "\u{1F7B}", 
        "\u{03CE}" => "\u{1F7D}", 
        "\u{0390}" => "\u{1FD3}", 
        "\u{03B0}" => "\u{1FE3}"  
    ];

$conn->set_charset("utf8");
$sql = "SELECT * FROM Interlineare WHERE (Libro=$libro) AND (Capitolo=$capitolo)";
//$sql = "SELECT * FROM Interlineare WHERE (Libro=$libro) AND (Capitolo=$capitolo)  AND (Versetto=39)";
$vlc = "rif1=".($libro+46)."&rif2=".$capitolo.":";
$diff1 = false; $diff2 = false; $diff3 = false;
$inizioVersetto = false;
$versettoAttuale = 0;
if ($ris = mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris) > 0) {
    echo "<div class=\"text-block\">";
    echo "<div class=\"line\"><b>Greco</b></div>";
    if ($traslit) echo "<div class=\"line\"><b>Traslitterazione</b></div>";
    if ($gramm) echo "<div class=\"line\"><b>Grammatica</b></div>";
    if ($rad) echo "<div class=\"line\"><b>Lemma</b></div>";
    if ($radtraslit) echo "<div class=\"line\"><b>Lemma traslitt.</b></div>";
    if ($nr06) echo "<div class=\"line\"><b>NR06</b></div>";
    if ($nr94) echo "<div class=\"line\"><b>NR94</b></div>";
    if ($r2) echo "<div class=\"line\"><b>R20</b></div>";
    echo "</div>";
      while ($row = mysqli_fetch_assoc($ris)) {
        $v = $vlc.$row["Versetto"];
        if ($row["Versetto"] != $versettoAttuale) {
            $inizioVersetto = true;
            $versettoAttuale = $row["Versetto"];
        }
        else {
            $inizioVersetto = false;
        }
        echo "<div class=\"text-block\">";
        $p = $row["Greco"];
        if ($row["Parola"] % 100 !== 0) {
            $p = "<a href=\"https://www.laparola.net/greco/index.php?varianti=s&$v\" target=\"_blank\">".$p."</a>";
            $class = "line" . ($evidenzaTC?" crittest":"");
        }
        else {
            $class = "line";
        }
          echo "<div class=\"$class\">" .($inizioVersetto?"<div class=\"versetto\">".$row["Versetto"]."</div> ":""). $p . "</div>";
          if ($traslit) {
            echo "<div class=\"line\">" . traslitterare($row["Greco"]). "</div>";
          }
          if ($gramm) {
            echo "<div class=\"line\">" . mostraGrammatica($row["Grammatica"]). "</div>";
          }
          if ($rad) {
            $radMapped = strtr($row["Radice"], $mapping);
            echo "<div class=\"line\"><a href=\"https://www.laparola.net/greco/parola.php?p=".$radMapped."&lin=it\" target=\"_blank\">" . $row["Radice"]. "</a></div>";
          }
          if ($radtraslit) {
            echo "<div class=\"line\">" . traslitterare($row["Radice"]). "</div>";
          }
          if ($evidenziaDiff) {
            $diff1 = false; $diff2 = false; $diff3 = false;
            if ($nr06 && $nr94 && $r2) {
              $d3 = compare3Strings($row["NR06"], $row["NR94"], $row["R2"], $evidenziaDiffPicc);
              $diff1 = (($d3==-1) || ($d3==1));
              $diff2 = (($d3==-1) || ($d3==2));
              $diff3 = (($d3==-1) || ($d3==3)); 
            }
            else if ($nr06 && $nr94) {
              $diff1 = ($row["NR06"] !== $row["NR94"]);
              $diff2 = $diff1;
            }
            else if ($nr06 && $r2) {
              $diff1 = ($row["NR06"] !== $row["R2"]);
              $diff3 = $diff1;
            }
            else if ($nr94 && $r2) {
              $diff2 = ($row["NR94"] !== $row["R2"]);
              $diff3 = $diff2;
            }
          }
          if ($nr06) echo divline($row["NR06"], $v, $evidenzaTC, $diff1);
          if ($nr94) echo divline($row["NR94"], $v, $evidenzaTC, $diff2);
          if ($r2) echo divline($row["R2"], $v, $evidenzaTC, $diff3);
        echo "</div>";
      }
  } else {
      echo "<p>Errore nei risultati dal database.</p>";
  }
}
else {
    echo "<p>Errore nel collegamento al database.</p>";
    echo $sql;
}

function mostraGrammatica(string $text): string
{
    $ann="";
    if ($text[0]=='C') {
        $text="Cong.";$ann="Congiunzione";
    }
    if ($text[0]=='P') {
        $text="Prep.";$ann="Preposizione";
    }
    if ($text[0]=='X') {
        $text="Part.";$ann="Particella";
    }
    if ($text[0]=='D') {
        $comp=0;if ($text[9]=='C') $comp=1;
        $text="Avv.";$ann="Avverbio";
        if ($comp==1) {$text.=" comp.";$ann.=" comparativo";}
    }
    if ($text[0]=='I') {
        $text="Inter.";$ann="Interiezione";
    }
    if (substr($text,0,2)=="V-") {
        if ($text[5]=='N') {
            $ann="Verbo ".tempo($text[3])." ".forma($text[4])." infinito";
            $text="Vb. ".$text[3].$text[4]." inf.";
        }
        if ($text[5]=='I' || $text[5]=='S' || $text[5]=='D' || $text[5]=='O') {
            $ann="Verbo ".persona($text[2])." ".numero($text[7])." ".tempo($text[3])." ".forma($text[4])." ".tense($text[5]);
            $text="Vb. ".$text[2].$text[7].$text[3].$text[4]." ".tense2($text[5]);
        }
        if ($text[5]=='P') {
            $ann="Verbo ".tempo($text[3])." ".forma($text[4])." ".genere($text[8])." ".numero($text[7])." ".caso($text[6])." participio";
            $text="Vb. ".$text[3].$text[4].$text[8].$text[7].$text[6]." part.";
        }
    }
    if (substr($text,0,2)=="N-") {
        $ann="Sostantivo ".genere($text[8])." ".numero($text[7])." ".caso($text[6]);
        $text="Sost. ".$text[8].$text[7].$text[6];
    }
    if (substr($text,0,2)=="A-") {
        $comp=0;if ($text[9]=='C') $comp=1;
        $sup=0;if ($text[9]=='S') $sup=1;
        $ann="Aggettivo ".genere($text[8])." ".numero($text[7])." ".caso($text[6]);
        $text="Agg. ".$text[8].$text[7].$text[6];
        if ($comp==1) {$text.=" comp.";$ann.=" comparativo";}
        if ($sup==1) {$text.=" sup.";$ann.=" superlativo";}
    }
    if (substr($text,0,2)=="RA") {
        $ann="Articolo definitivo ".genere($text[8])." ".numero($text[7])." ".caso($text[6]);
        $text="Art. def. ".$text[8].$text[7].$text[6];
    }
    if (substr($text,0,2)=="RD") {
        $ann="Pronome dimostrativo ".genere($text[8])." ".numero($text[7])." ".caso($text[6]);
        $text="Pron. dim. ".$text[8].$text[7].$text[6];
    }
    if (substr($text,0,2)=="RI") {
        $ann="Pronome interrogativo ".genere($text[8])." ".numero($text[7])." ".caso($text[6]);
        $text="Pron. int. ".$text[8].$text[7].$text[6];
    }
    if (substr($text,0,2)=="RR") {
        $ann="Pronome relativo ".genere($text[8])." ".numero($text[7])." ".caso($text[6]);
        $text="Pron. rel. ".$text[8].$text[7].$text[6];
    }
    if (substr($text,0,2)=="RP") {
        $ann="Pronome personale ".($text[8]!="-"?genere($text[8])." ":"").numero($text[7])." ".caso($text[6]);
        $text="Pron. pers. ".($text[8]!="-"?$text[8]:"").$text[7].$text[6];
    }
    $text="<span class='analysis-code' data-analysis='".$ann."'>".$text."</span>";
    return $text;
}

function persona(string $g): string
{
switch($g) {
case '1': return "1a";
case '2': return "2a";
case '3': return "3a";
}
return "";
}

function tempo(string $g): string
{
switch($g) {
case 'P': return "presente";
case 'A': return "aorista";
case 'X': return "perfetto";
case 'F': return "futuro";
case 'I': return "imperfetto";
case 'Y': return "piuccheperfetto";
}
return "";
}

function tense(string $g): string
{
switch($g) {
case 'I': return "indicativo";
case 'S': return "congiuntivo";
case 'D': return "imperativo";
case 'O': return "ottativo";
}
return "";
}

function tense2(string $g): string
{
switch($g) {
case 'I': return "ind.";
case 'S': return "cong.";
case 'D': return "imp.";
case 'O': return "ott.";
}
return "";
}

function forma(string $g): string
{
switch($g) {
case 'A': return "attivo";
case 'M': return "medio";
case 'P': return "passivo";
}
return "";
}

function genere(string $g): string
{
switch($g) {
case 'M': return "maschile";
case 'F': return "femminile";
case 'N': return "neutro";
}
return "";
}

function numero(string $g): string
{
switch($g) {
case 'S': return "singolare";
case 'P': return "plurale";
}
return "";
}

function caso(string $g): string
{
switch($g) {
case 'N': return "nominativo";
case 'V': return "vocativo";
case 'A': return "accusativo";
case 'G': return "genitivo";
case 'D': return "dativo";
}
return "";
}

function traslitterare(string $text): string
{
static $mapdt = [
    "αἱ" => "hai", "αἷ" => "hai", "αἵ" => "hai", "αἳ" => "hai",
    "Αἱ" => "Hai", "Αἵ" => "Hai",
    "αὑ" => "hau", "αὕ" => "hau", "αὗ" => "hau",
    "Αὕ" => "Hau",
    "εἱ" => "hei", "εἷ" => "hei", "εἵ" => "hei",
    "εὑ" => "heu", "εὗ" => "heu", "εὕ" => "heu",
    "Εἱ" => "Hei", "Εἷ" => "Hei",
    "Εὑ" => "Heu", "Εὗ" => "Heu", "Εὕ" => "Heu",
    "ηὑ" => "hêu", "ηὕ" => "hêu",
    "οἱ" => "hoi", "οἵ" => "hoi", "οἷ" => "hoi", "οἳ" => "hoi",
    "Οἱ" => "Hoi", "Οἷ" => "Hoi",
    "οὗ" => "hou", "οὕ" => "hou", "οὓ" => "hou",
    "Οὕ" => "Hou", "Οὗ" => "Hou", "Οὓ" => "Hou",
    "υἱ" => "hui",
    "Υἱ" => "Hui",
];

$t2 = mb_substr($text, 0, 2, 'UTF-8');
if (isset($mapdt[$t2])) {
    $text = $mapdt[$t2] . mb_substr($text, 2, null, 'UTF-8');
}

static $mapdt3 = [
    "(αὕ" => "(hau",
    "(οἱ" => "(hoi",
    "(οἵ" => "(hoi",
];

$t3 = mb_substr($text, 0, 3, 'UTF-8');
if (isset($mapdt3[$t3])) {
    $text = $mapdt3[$t3] . mb_substr($text, 3, null, 'UTF-8');
}

static $map = [
    // Lowercase
    'α' => 'a',  'β' => 'b',  'γ' => 'g',  'δ' => 'd',
    'ε' => 'e',  'ζ' => 'z',  'η' => 'ê', 'θ' => 'th',
    'ι' => 'i',  'κ' => 'k',  'λ' => 'l',  'μ' => 'm',
    'ν' => 'n',  'ξ' => 'x',  'ο' => 'o',  'π' => 'p',
    'ρ' => 'r',  'σ' => 's',  'ς' => 's',  'τ' => 't',
    'υ' => 'u',  'φ' => 'f', 'χ' => 'ch', 'ψ' => 'ps',
    'ω' => 'ô',

    // Uppercase
    'Α' => 'A',  'Β' => 'B',  'Γ' => 'G',  'Δ' => 'D',
    'Ε' => 'E',  'Ζ' => 'Z',  'Η' => 'Ê', 'Θ' => 'Th',
    'Ι' => 'I',  'Κ' => 'K',  'Λ' => 'L',  'Μ' => 'M',
    'Ν' => 'N',  'Ξ' => 'X',  'Ο' => 'O',  'Π' => 'P',
    'Ρ' => 'R',  'Σ' => 'S',  'Τ' => 'T',  'Υ' => 'U',
    'Φ' => 'F', 'Χ' => 'Ch', 'Ψ' => 'Ps', 'Ω' => 'Ô',
        
    // Accented lowercase (acute ´)
    'ά' => 'a', 'έ' => 'e', 'ή' => 'ê', 'ί' => 'i',
    'ό' => 'o', 'ύ' => 'u', 'ώ' => 'ô',

    // Accented lowercase (grave `)
    'ὰ' => 'a', 'ὲ' => 'e', 'ὴ' => 'ê', 'ὶ' => 'i',
    'ὸ' => 'o', 'ὺ' => 'u', 'ὼ' => 'ô',

    // Accented uppercase (acute ´)
    'Ά' => 'A', 'Έ' => 'E', 'Ή' => 'Ê', 'Ί' => 'I',
    'Ό' => 'O', 'Ύ' => 'U', 'Ώ' => 'Ô',

    // Accented uppercase (grave `)
    'Ὰ' => 'A', 'Ὲ' => 'E', 'Ὴ' => 'Ê', 'Ὶ' => 'I',
    'Ὸ' => 'O', 'Ὺ' => 'U', 'Ὼ' => 'Ô',

    // Iota subscript (lowercase)
    'ᾳ' => 'a', 'ῃ' => 'ê', 'ῳ' => 'ô',

    // Iota subscript (uppercase)
    'ᾼ' => 'A', 'ῌ' => 'Ê', 'ῼ' => 'Ô',
    
    'ᾶ' => 'a', 'ᾴ' => 'a', 'ᾷ' => 'a', 
    'ῆ' => 'ê', 'ῇ' => 'ê', 'ἦ' => 'ê', 'ἤ' => 'ê', 'ῄ' => 'ê',
    'ΐ' => 'i', 'ῖ' => 'i', 'ἶ' => 'i', 'ῒ' => 'i', 'ἴ' => 'i', 'ϊ'=> 'i',
    'ϋ' => 'u', 'ῦ' => 'u',
    'ῶ' => 'ô', 'ῷ' => 'ô', 'ῴ' => 'ô',

    'ἀ' => 'a', 'ἐ' => 'e', 'ἠ' => 'ê', 'ἰ' => 'i', 'ὀ' => 'o', 'ὐ' => 'u', 'ὡ' => 'ô',
    'ἄ' => 'a', 'ἆ' => 'a', 'ἂ' => 'a', 'ᾄ' => 'a',
    'ἔ' => 'e',
    'ᾔ' => 'ê', 'ῆ' => 'ê', 'ἢ' => 'ê', 'ᾖ' => 'ê', 'ᾐ' => 'ê',
    'ὄ' => 'o', 'ὂ' => 'o',
    'ὖ' => 'u', 'ὔ' => 'u', 'ὒ' => 'u', 'ΰ' => 'u', 'ῢ' => 'u',
    'ὠ' => 'ô', 'ὢ' => 'ô', 'ὤ' => 'ô', 'ὦ' => 'ô', 'ᾠ' => 'ô',

    'Ἀ' => 'A', 'Ἄ' => 'A', 'Ἆ' => 'A',
    'Ἐ' => 'E', 'Ἔ' => 'E',
    'Ἠ' => 'Ê', 'Ἤ' => 'Ê', 'Ἦ' => 'Ê',
    'Ἰ' => 'I', 'Ἴ' => 'I',
    'Ὀ' => 'O', 'Ὄ' => 'O',
    'Ὦ' => 'Ô', 'Ὤ' => 'Ô',
    
    'ἁ' => 'ha',
    'ἑ' => 'he', 'ἕ' => 'he', 'ἓ' => 'he',
    'ἡ' => 'hê', 'ἧ' => 'hê', 'ᾑ' => 'hê',
    'ἱ' => 'hi', 'ὁ' => 'ho', 'ὑ' => 'hu',
    'ἅ' => 'ha', 'ἃ' => 'ha', 'ᾅ' => 'ha',
    'ἥ' => 'hê', 'ᾗ' => 'hê', "ἣ" => 'hê',
    'ἵ' => 'hi',
    'ὃ' => 'ho', 'ὅ' => 'ho',
    'ὕ' => 'hu', 'ὗ' => 'hu',
    'ὥ' => 'hô', 'ᾧ' => 'hô', 'ὧ' => 'hô',
    'ῥ' => 'rh', 'Ῥ' => 'Rh',
    'Ἁ' => 'Ha', 'Ἅ' => 'Ha', 'Ἃ' => 'Ha',
    'Ἑ' => 'He', 'Ἕ' => 'He', 'Ἓ' => 'He',
    'Ἡ' => 'Hê', 'Ἢ' => 'Hê', 'Ἥ' => 'Hê',
    'Ἱ' => 'Hi', 'Ἵ' => 'Hi',
    'Ὁ' => 'Ho', 'Ὅ' => 'Ho', 'Ὃ' => 'Ho',
    'Ὑ' => 'Hu', 'Ὕ' => 'Hu', 'Ὗ' => 'Hu',
    'Ὡ' => 'Hô', 'Ὧ' => 'Hô', 'Ὥ' => 'Hô',
];

    $text = strtr($text, $map);
/*    
$hasNonAscii = false;
$char="";
for ($i = 0; $i < mb_strlen($text, 'UTF-8'); $i++) {
    $oldchar=$char;
    $char = mb_substr($text, $i, 1, 'UTF-8');
    $codePoint = mb_ord($char, 'UTF-8'); // Get Unicode code point
    if ($codePoint > 127 && $char!="ê" && $char!="ô" && $char!="Ê" && $char!="Ô" && $char!="·" && $char!="’" && $char!="—" && $char!="⟦" && $char!="⟧") { // caratteri non visibili sono doppio ] in Marco 16:9,20
        $hasNonAscii = true;
        break;
    }
    if (($char=="h" || $char=="H") && $i>0 && $oldchar!="c" && $oldchar!="C" && $oldchar!="t" && $oldchar!="T" && $oldchar!="r" && $oldchar!="R" && $oldchar!="(")
    echo "QQX";
}

if ($hasNonAscii) {
    echo "QQ";
}
*/
    return $text;
}

// https://www.laparola.net/greco/index.php?rif1=49&rif2=7:2&varianti=s
function divline($a, $v, $tc, $diff) {
    if ($a=="-") $a = "&ndash;";
    $class = ($tc && ($a === "*")) ? "line crittest" : "line";
    if ($diff) $class = $class." diff";
    if ($a=="*") {
        $a = "<a href=\"https://www.laparola.net/greco/index.php?varianti=s&$v\" target=\"_blank\">".$a."</a>";
    }
    echo "<div class=\"$class\">$a</div>";
}

function compare3Strings($str1, $str2, $str3, $evidenziaDiffPicc) {
    if ($evidenziaDiffPicc==1) {
      $str1 = normalizeString($str1);
      $str2 = normalizeString($str2);
      $str3 = normalizeString($str3);
    }
    
    if ($str1 === $str2 && $str2 === $str3) {
        return 0; // All are the same
    } elseif ($str1 !== $str2 && $str2 !== $str3 && $str1 !== $str3) {
        return -1; // All are different
    } elseif ($str1 === $str2) {
//    echo "<p>$str1"."xxx".$str3."x</p>";
        return 3; // Third string is different
    } elseif ($str2 === $str3) {
        return 1; // First string is different
    } else {
//    echo "<p>$str1"."xxx".$str2."x</p>";
        return 2; // Second string is different
    }
}

function normalizeString($s) {
    static $equiv = [
        '+'    => '',
        '-'    => '',
        'gl\''    => 'gli',
        'l\''    => 'loq',
        'il<'    => 'loq<',
        'la<'    => 'loq<',
        '{lo}'    => '{loq}',
        '{lo<'    => '{loq<',
        'un\''    => 'unq',
        'una'    => 'unq',
        'uno'    => 'unq',
        'qualcos\'' => 'qualcosa',
        'quel<' => 'quello<',
        'quel}' => 'quello}',
        'quel ' => 'quello ',
        'nunzi' => 'nunci',
        'nunce' => 'nuncie',
        'viene' => 'vien',
        'ancora' => 'ancor',
        'maggiore' => 'maggior',
        'signore' => 'signor',
        'quale' => 'qual',
        'amore' => 'amor',
        'eppure' => 'eppur',
        'odore' => 'odor',
        'cuore' => 'cuor',
        'fiore' => 'fior',
        'fuori' => 'fuor',
        'quest\'' => 'questo',
        'quand\'' => 'quando',
        'questa}' => 'questo}',
        'anch\'' => 'anche',
        'nient\'' => 'niente',
        'mentr\'' => 'mentre',
        '{ch\'' => '{che',
        ' d\'' => ' di',
        '{d\'' => '{di',
        'd\'<' => 'di<',
        'n\'}' => 'ne}',
        's\'<' => 'si<',
        'bòc' => 'boc',
        'bóc' => 'boc',
        'mós' => 'mos',
        'òs' => 'os', //mos, pos
        'òr' => 'or', // cor, for
        'òl' => 'ol', // col, sol
        'àpp' => 'app',
        'àl' => 'al', // dàl, càl
        'làv' => 'lav',
        'ràd' => 'rad',
        'sài' => 'sai',
        'uàr' => 'uar',
        'án' => 'an', //tan, ian
        'àn' => 'an',
        'èr' => 'er', // sèr, tèr
        'èg' => 'eg', // leg, veg
        'vég' => 'veg',
        'sèn' => 'sen',
        'ès' => 'es', // fes, ves
        'rèt' => 'ret',
        'rìc' => 'ric',
        'rìf' => 'rif',
        'ìn' => 'in', // rin, cin
        'tìa' => 'tia',
        'tìf' => 'tif',
        'bìt' => 'bit',
        'dùl' => 'dul',
        '{lì'    => '{là',
        ' lì'    => ' là',
        'dette<' => 'dé<', // passato remoto
        'evangelo' => 'vangelo',
        'vedut' => 'vist',
        'sé ' => 'se ',
        'sé<' => 'se<',
        'fra' => 'tra',
        '{tra} {di}' => '{tra}',
        'tra di' => 'tra',
        'sopra di' => 'sopra',
        'dov\'' => 'dove',
        'ricup' => 'recup',
        'rical' => 'recal',        
        'un po\'' => 'un poco',
        ' po\'<' => ' poco<',
        'cuot' => 'cot',
        'fuoc' => 'foc',
        'sona' => 'suona',
        'sonò' => 'suonò',
        'mov' => 'muov',
        'com\'' => 'come',
        'dev\'' => 'deve',
        '{ve<' => '{ce<',
        '{vi<' => '{ci<',
        '{vi}' => '{ci}',
        '{v\'}' => '{ci}',
        '{c\'<' => '{ci<',
        '{c\'}' => '{ci}',
        'son<' => 'sono<',
        'son}' => 'sono}',
        'son ' => 'sono ',
        'sol}' => 'solo}',
        'ella' => 'lei',
        ' tale' => ' tal',
        'tutte e ' => 'tutt\' e ',
        'grande' => 'gran',
        'qua<' => 'qui<',
        'qua}' => 'qui}',
        'pur}' => 'pure}',
        'ar}' => 'are}',
        'ar<' => 'are<',
        'ar ' => 'are ',
        'er<' => 'ere<',
        'er ' => 'ere ',
        'stridore' => 'stridor', // ora, che finisce con or(a), fatto qui sotto
        'fino ad' => 'fino a',
        ' ad<' => ' a<',
        'ad}' => 'a}',
        'comper' => 'compr',        
        'ir<' => 'ire<',
        'ir ' => 'ire ',
        'cosicché' => 'così che',
        'oramai' => 'ormai',
        'difatti' => 'infatti',
        'ministerio' => 'ministero',        
        '666' => 'seicentosessantasei',
    ];

    $s = mb_strtolower($s, 'UTF-8');

    foreach ($equiv as $search => $replace) {
        $s = str_replace($search, $replace, $s);
    }

    if (mb_substr($s, 0, 6, 'UTF-8') === "àlzati") $s = "alzati" . mb_substr($s, 6, null, 'UTF-8');
    if ($s == "ed") $s = "e";
    if (substr($s, 0, 3) === "ad<") $s = "a<" . substr($s, 3);
    if (substr($s, 0, 3) === "ed<") $s = "e<" . substr($s, 3);
    if (substr($s, 0, 4) === "ora<") $s = "or<" . substr($s, 4);
    if (substr($s, 0, 5) === "{ora<") $s = "{or<" . substr($s, 5);
    if (mb_substr($s, 0, 3, 'UTF-8') === "ciò") $s = "quello" . mb_substr($s, 3, null, 'UTF-8');
    if (substr($s, -2) === "ar") $s .= "e";
    if (substr($s, -2) === "er") $s .= "e";
    if (substr($s, -3) === "er}") $s = mb_substr($s, 0, mb_strlen($s, 'UTF-8') - 1, 'UTF-8') . 'e}';
    if (substr($s, -2) === "ir") $s .= "e";
    if (substr($s, -2) === "or") $s .= "a"; // ora
    if (mb_substr($s, 0, 2, 'UTF-8') === "dà") $s = "da'" . mb_substr($s, 2, null, 'UTF-8');
    if ($s == "su di") $s = "su";
    if ($s == "son") $s = "sono";
    if ($s == "cos'") $s = "che";
    if ($s == "vuol") $s = "vuole";
    if ($s == "qual") $s = "quale";
    if ($s == "qua") $s = "qui";
    if ($s == "ad") $s = "a";
    if ($s == "lo") $s = "loq";
    if ($s == "il") $s = "loq";
    if ($s == "sino") $s = "fino";
    if ($s == "sin") $s = "fino";
    if ($s == "fin") $s = "fino";
    if (substr($s, 0, 5) === "sino ") $s = "fino " . substr($s, 5);
    if (substr($s, 0, 4) === "fin<") $s = "fino<" . substr($s, 4);
    if (substr($s, 0, 6) === "persin") $s = "perfin" . substr($s, 6);
    if (substr($s, 0, 3) === "lo<") $s = "loq<" . substr($s, 3);
    if (substr($s, 0, 4) === "{lo<") $s = "{loq<" . substr($s, 4);
    if ($s == "quel che") $s = "quello che";
    if ($s == "dello") $s = "del"; // dell' a dello prima
    if ($s == "alloq" || $s == "alla") $s = "al"; // per all', che viene cambiato in alloq
    if ($s == "va'") $s = "vai";
    if (substr($s, 0, 2) === "d'") $s = "di" . substr($s, 2);
    if (substr($s, 0, 3) === "ch'") $s = "che" . substr($s, 3);
    if (substr($s, 0, 2) === "m'") $s = "mi" . substr($s, 2);
    if (substr($s, 0, 2) === "s'") $s = "si" . substr($s, 2);
    if ($s == "se") $s = "si"; // s' può essere se o si, ma s'->se è solo quando è la stringa intera
    if (substr($s, 0, 2) === "t'") $s = "ti" . substr($s, 2);
    if (substr($s, 0, 2) === "v'") $s = "vi" . substr($s, 2);
    
    return $s;
}
?>
