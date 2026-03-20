<?
function errore2($commando) {
global $conn;
    echo "Errore ".mysqli_errno($conn).": ".mysqli_error($conn)." in $commando.<br />\n";
    echo "Prova di nuovo pi&ugrave; tardi; se l'errore persiste, scrivere a <a href=\"mailto:&#105;&#110;&#102;&#111;&#64;&#108;&#97;&#112;&#97;&#114;&#111;&#108;&#97;&#46;&#110;&#101;&#116;\">Richard Wilson</a>.<br />";
    echo "Try again later; if the error remains, write to <a href=\"mailto:&#105;&#110;&#102;&#111;&#64;&#108;&#97;&#112;&#97;&#114;&#111;&#108;&#97;&#46;&#110;&#101;&#116;\">Richard Wilson</a>.";
}

$conn=mysqli_connect("127.0.0.1","laparola_testo","","laparola_testo");

if (mysqli_connect_errno())
  errore2('collegamento');
?>
