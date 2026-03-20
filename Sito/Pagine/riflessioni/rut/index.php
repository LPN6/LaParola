<?
include("../../conn.php");
include("../../vistesto.php");

$descriz = "Rut";
$key = "rut, riflessioni";
$titolo = "Rut";
$sezione = "Strumenti";
require("../../capo.php");
?>
<h1>Rut</h1>
<p class="primalettera">
Questa &egrave; una serie di riflessioni sul libro di Rut, con il titolo: <b>Una vita di amore in un mondo di relazioni difficili</b>.</p>

<?
$n=0;
require("indice.php");
require("../../piede.php");
?>
