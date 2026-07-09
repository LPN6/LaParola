<?
function errore2($commando) {
global $conn;
    echo "Errore ".mysqli_errno($conn).": ".mysqli_error($conn)." in $commando.<br />\n";
    echo "Prova di nuovo pi&ugrave; tardi; se l'errore persiste, scrivere a <a href=\"mailto:&#105;&#110;&#102;&#111;&#64;&#108;&#97;&#112;&#97;&#114;&#111;&#108;&#97;&#46;&#110;&#101;&#116;\">Richard Wilson</a>.<br />";
    echo "Try again later; if the error remains, write to <a href=\"mailto:&#105;&#110;&#102;&#111;&#64;&#108;&#97;&#112;&#97;&#114;&#111;&#108;&#97;&#46;&#110;&#101;&#116;\">Richard Wilson</a>.";
}

$db_host = getenv('DB_HOST') ?: '127.0.0.1';
$db_user = getenv('DB_USER') ?: 'laparola_testo';
$db_pass = getenv('DB_PASS') ?: '';
$db_name = getenv('DB_NAME') ?: 'laparola_testo';
$db_port = (int)(getenv('DB_PORT') ?: '3306');

$conn = mysqli_connect($db_host, $db_user, $db_pass, $db_name, $db_port);

if (mysqli_connect_errno())
  errore2('collegamento');

// Use utf8mb4 for modern MySQL compatibility
mysqli_set_charset($conn, "utf8mb4");

?>
