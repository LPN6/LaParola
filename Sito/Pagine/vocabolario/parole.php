<?
if (isset($_REQUEST["parola"])) {
  $parola = trim($_REQUEST["parola"]);
  $parola = str_replace("<", "", $parola); // affinché tag HTML non possono essere inseriti nella pagina
  $parola = str_replace(">", "", $parola);
  $parola = str_replace("\"", "»", $parola);
}
else
  header("Location: https://".$_SERVER['HTTP_HOST']."/vocabolario/");

include("../conn.php");

if (strlen($parola)>=1 && preg_match('/^\p{Latin}/u', $parola)) {
    $parola = str_replace('è', 'ē', $parola);
    $parola = str_replace('é', 'ē', $parola);
    $parola = str_replace('ò', 'ō', $parola);
    $parola = str_replace('ó', 'ō', $parola);
    $parolaHtml = toHtmlEntities($parola);
    $sql = "
        SELECT *
        FROM Vocabolario2
        WHERE TraslitHtml = ?
    ";
    $ris = runQuery($conn, $sql, $parolaHtml);
    if (mysqli_num_rows($ris) !== 1) {
      $sql = "
          SELECT *
          FROM Vocabolario2
          WHERE TraslitHtml LIKE CONCAT(?, '%')
          ORDER BY Parola
      ";
      $ris = runQuery($conn, $sql, $parolaHtml);
    }
}
else {
    $parolaHtml = toHtmlEntities($parola);
// 1. Exact case-sensitive match
$sql = "
    SELECT *
    FROM Vocabolario2
    WHERE Parola = ?
    LIMIT 1
";
$ris = runQuery($conn, $sql, $parolaHtml);

if (mysqli_num_rows($ris) === 0) {
    // 2. Case-insensitive, accent-sensitive matches
    $sql = "
        SELECT *
        FROM Vocabolario2
        WHERE ParolaHtml = ?
        LIMIT 2
    ";
    $ris = runQuery($conn, $sql, $parolaHtml);

    if (mysqli_num_rows($ris) === 1) {
//
    } else {
        // 3. No unique whole-word match: find all normalized prefix matches
        $prefixHtml = toHtmlEntities(greekPrefix($parola));

      if ($prefixHtml !== '') {
          $sql = "
              SELECT *
              FROM Vocabolario2
              WHERE ParolaNorm LIKE CONCAT(?, '%')
              ORDER BY Parola
          ";
          $ris = runQuery($conn, $sql, $prefixHtml);
      }
    }
}
}

$descriz = $parola." - la definizione";
$key = $parola.",greco,vocabolario,dizionario,greco-italiano,significato,Nuovo Testamento,NT,dizionario greco";
$titolo = $parola;
$sezione = "Vocabolario greco";
$sezioneurl = "/vocabolario/";
require("../capo.php");
?>
<style>
  .note {
    position: relative;
    display: inline-block;
    cursor: help;
  }

  .popup {
    display: none;
    position: absolute;
    left: 50%;
    bottom: 1.5em;
    width: max-content;
    max-width: 240px;
    padding: 8px 10px;
    border-radius: 4px;
    background: #222;
    color: white;
    font: 14px/1.4 sans-serif;
    z-index: 10;
  }

  /* Show on hover, keyboard focus, or tap/click */
  .note:hover .popup,
  .note:focus .popup,
  .note:active .popup {
    display: block;
  }

  .note {
    outline: none;
  }
  .note.open .popup {
  display: block;
}
</style>
<script>
  document.querySelectorAll(".note").forEach(note => {
    note.tabIndex = 0; // makes it keyboard-focusable
    note.addEventListener("click", () => {
      note.classList.toggle("open");
    });
  });
</script>
<?
if (mysqli_num_rows($ris)==0) {
    echo "<h1>$parola</h1><p>Nessuna parola che inizia con '".$parola."' &egrave; stata trovata. Prova di nuovo...</p>";
}
elseif (mysqli_num_rows($ris)==1) {
  $row=mysqli_fetch_array ($ris);
  echo "<h1>".$row["ParolaHtml"]."</h1>";
  echo "<p><b>Traslitterazione:</b> ".$row["TraslitHtml"]."</p>";
  if ($row["Parte"]!="")
      echo "<p><b>Parte del discorso:</b> ".$row["Parte"]."</p>";
  echo "<p>";
  if ($row["Strong"]!="")
      echo "<b>Strong<span class='note'>*<span class='popup' role='tooltip'><i>Numero Strong</i>: usato in molti dizionari, per trovare la parola in altri testi</span></span>:</b> ".$row["Strong"]."<br />";
  if ($row["TDNT"]!="")
      echo "<b>TDNT<span class='note'>*<span class='popup' role='tooltip'><i>Theological Dictionary of the New Testament</i>: volume:pagina, pagina nella versione Abridged</span></span>:</b> ".$row["TDNT"];
  echo "</p>";
  echo "<p>".$row["Definizione"]."</p>";
  echo "<p>".$row["Forme"]."</p>";
  echo "<p>".$row["Uso"]."</p>";
}
else {
    echo "<h1>$parola</h1><p>Le seguenti parole iniziano con '".$parola."'. Scegli quella desiderata:</p><p>";
    while ($row=mysqli_fetch_array ($ris)) {
      $parolatrovata = $row["ParolaHtml"];
      echo "<a href=\"parole.php?parola=".$parolatrovata."\">".$parolatrovata."</a> (".$row["TraslitHtml"].")<br />";
    }
    echo "</p>";
}
require("indvocab.php");
require("../piede.php");

function greekPrefix(string $text): string
{
    $text = Normalizer::normalize($text, Normalizer::FORM_D);
    if ($text === false) {
        return '';
    }

    $text = preg_replace('/\p{Mn}/u', '', $text);
    if ($text === null) {
        return '';
    }

$text = mb_strtolower($text, 'UTF-8');
$text = str_replace("\u{03C2}", "\u{03C3}", $text); // ς → σ

$result = preg_replace('/[^\x{03B1}-\x{03C9}]/u', '', $text);
return $result ?? '';
}

function runQuery(mysqli $conn, string $sql, string $parola): mysqli_result
{
    $stmt = mysqli_prepare($conn, $sql);
    if (!$stmt) {
        errore2("interrogazione database per il vocabolario");
        exit;
    }

    mysqli_stmt_bind_param($stmt, "s", $parola);
    if (!mysqli_stmt_execute($stmt)) {
        errore2("interrogazione database per il vocabolario");
        exit;
    }

    return mysqli_stmt_get_result($stmt);
}

function toHtmlEntities(string $str): string
{
    $result = '';

    // Split into Unicode characters
    $chars = preg_split('//u', $str, -1, PREG_SPLIT_NO_EMPTY);

    foreach ($chars as $char) {
        $codepoint = mb_ord($char, 'UTF-8');

        // Keep printable ASCII unchanged
        if ($codepoint >= 0x20 && $codepoint <= 0x7E) {
            $result .= $char;
        } else {
            $result .= '&#x' . strtoupper(dechex($codepoint)) . ';';
        }
    }

    return $result;
}
?>
