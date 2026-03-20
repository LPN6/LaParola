<?
$libro = 1;
if (isset($_REQUEST["libro"])) {
  $libro = (int)$_REQUEST["libro"];
}
if ($libro==0) $libro = 1;
$capitolo = 1;
if (isset($_REQUEST["capitolo"])) {
  $capitolo = (int)$_REQUEST["capitolo"];
}
if ($capitolo==0) $capitolo = 1;

include("../conn.php");
include("../vistesto.php");

$capitoliInLibri = array(0, 50,
    40,
    27,  // Leviticus
    36,  // Numbers
    34,  // Deuteronomy
    24,  // Joshua
    21,  // Judges
    4,   // Ruth
    31,  // 1 Samuel
    24,  // 2 Samuel
    22,  // 1 Kings
    25,  // 2 Kings
    29,  // 1 Chronicles
    36,  // 2 Chronicles
    10,  // Ezra
    13,  // Nehemiah
    14,  // Tobit
    16,  // Judith
    10,  // Esther
    16, 15, // 1M, 2M
    42,  // Job
    150, // Psalms
    31,  // Proverbs
    12,  // Ecclesiastes
    8,   // Song of Solomon
    19, 51, // Wisdom, Sirach
    66,  // Isaiah
    52,  // Jeremiah
    5,   // Lamentations
    6,   // Baruch
    48,  // Ezekiel
    14,  // Daniel
    14,  // Hosea
    3,   // Joel
    9,   // Amos
    1,   // Obadiah
    4,   // Jonah
    7,   // Micah
    3,   // Nahum
    3,   // Habakkuk
    3,   // Zephaniah
    2,   // Haggai
    14,  // Zechariah
    4,   // Malachi
    28,  // Matthew
    16, // Mark
    24, // Luke
    21, // John
    28, // Acts
    16, // Romans
    16, // 1 Corinthians
    13, // 2 Corinthians
    6, // Galatians
    6, // Ephesians
    4, // Philippians
    4, // Colossians
    5, // 1 Thessalonians
    3, // 2 Thessalonians
    6, // 1 Timothy
    4, // 2 Timothy
    3, // Titus
    1, // Philemon
    13, // Hebrews
    5, // James
    5, // 1 Peter
    3, // 2 Peter
    5, // 1 John
    1, // 2 John
    1, // 3 John
    1, // Jude
    22 // Revelation
);

$libri = array("",
    "Genesi", "Esodo", "Levitico", "Numeri", "Deuteronomio", "Giosuè", "Giudici", "Rut",
    "1Samuele", "2Samuele", "1Re", "2Re", "1Cronache", "2Cronache", "Esdra", "Neemia",
    "Tobia","Giuditta",
    "Ester",
    "1Maccabei", "2Maccabei",
    "Giobbe", "Salmi", "Proverbi", "Ecclesiaste", "Cantico dei Cantici",
    "Sapienza", "Siracide",
    "Isaia",
    "Geremia", "Lamentazioni",
    "Baruc",
    "Ezechiele", "Daniele", "Osea", "Gioele", "Amos", "Abdia",
    "Giona", "Michea", "Naum", "Abacuc", "Sofonia", "Aggeo", "Zaccaria", "Malachia",
    "Matteo", "Marco", "Luca", "Giovanni", "Atti", "Romani", "1Corinzi", "2Corinzi",
    "Galati", "Efesini", "Filippesi", "Colossesi", "1Tessalonicesi", "2Tessalonicesi",
    "1Timoteo", "2Timoteo", "Tito", "Filemone", "Ebrei", "Giacomo", "1Pietro", "2Pietro",
    "1Giovanni", "2Giovanni", "3Giovanni", "Giuda", "Apocalisse");
$nomeCapitolo = ($libro==6 ? "Giosu&egrave;" : $libri[$libro])." ".$capitolo;
$descriz = "Un riassunto di $nomeCapitolo, generato dall'intelligenza artificiale";
$key = $libri[$libro].", riassunto";
$titolo = "Un riassunto di $nomeCapitolo, generato dall'intelligenza artificiale";
$sezione = "Strumenti";
require("../capo.php");

$sql = "SELECT Testo FROM Riassunti WHERE (Libro=$libro) AND (Capitolo=$capitolo)";
if ($ris = mysqli_query($conn, "$sql")) {
//echo $sql;
    if (mysqli_num_rows($ris)==1) {
        echo "<p>Questo riassunto del capitolo &egrave; stato generato da un'intelligenza artificiale.</p>";
        echo "<h1>$nomeCapitolo</h1>";
        $row=mysqli_fetch_array ($ris);
        echo $row["Testo"];
        if ($libro>1 || $capitolo>1) {
            $libropred = $libro;
            $capitolopred = $capitolo - 1;
            if ($capitolopred==0) {
                $libropred = $libropred - 1;
                $capitolopred = $capitoliInLibri[$libropred];
            }
            echo "<p><input class=\"seleziona\" type=\"button\" name=\"Capitolo precedente\" value=\"Capitolo precedente\" onclick=\"window.location.href='riassunto.php?libro=$libropred&capitolo=$capitolopred'\" /></p>";
        }
        echo "<p><input class=\"seleziona\" type=\"button\" name=\"Indice\" value=\"Indice\" onclick=\"window.location.href='/riassunti/';\" /></p>";
        if ($libro<73 || $capitolo<22) {
            $librosucc = $libro;
            $capitolosucc = $capitolo + 1;
            if ($capitolosucc > $capitoliInLibri[$librosucc]) {
                $librosucc = $librosucc + 1;
                $capitolosucc = 1;
            }
            echo "<p><input class=\"seleziona\" type=\"button\" name=\"Capitolo successivo\" value=\"Capitolo successivo\" onclick=\"window.location.href='riassunto.php?libro=$librosucc&capitolo=$capitolosucc'\" /></p>";
        }
    }
    else {
        echo "<h1>$nomeCapitolo</h1>";
        echo "<p>Nessuno riassunto per $nomeCapitolo &egrave; stato trovato. Prova di nuovo a selezionare un libro e un capitolo dalla <a href='/riassunti/'>pagina dei riassunti</a>.</p>";
    }
}
?>

<?
require("../piede.php");
?>
