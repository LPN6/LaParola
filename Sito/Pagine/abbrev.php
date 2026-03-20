<?
$descriz = "Le abbreviazioni usate e riconosciute per i libri della Bibbia";
$key = "libri,abbreviazioni";
$titolo = "Abbreviazioni";
$sezione = "Testo della Bibbia";
require("capo.php");
include("conn.php");
?>
<h1>Abbreviazioni dei libri</h1>
<p>Per visualizzare i riferimenti, il sito usa sempre le stesse abbreviazioni per i libri della Bibbia. Per&ograve; quando un riferimento &egrave; digitato, il sito riesce a riconoscere molte altre abbreviazioni. Questa pagina elenca le abbreviazioni usate e quelle riconosciute dal sito.</p>
<p>Per le abbreviazioni riconosciute, qualsiasi abbreviazioni che inizia con una delle abbreviazioni qui elencate &egrave; riconosciuta. Per esempio, &quot;Gen&quot; &egrave; riconosciuta per Genesi, perch&eacute; inizia con &quot;Ge&quot;, un'abbreviazione elencata. Maiuscola/minuscola non importa quando digiti un riferimento, sono riconsciute tutte e due, quindi tutte le abbreviazioni sono in lettere minuscole qui.</p>
<table>
<tr><th>Libro</th><th>Abbreviazione<br />usata</th><th>Abbreviazioni<br />riconosciute</th></tr>
<?
    $sql = "SELECT * FROM Abbreviazioni,Libri WHERE Abbreviazioni.Numero=Libri.Numero ORDER BY Libri.Numero ASC, AbbR ASC";
    if ($ris=mysqli_query($conn, "$sql")) {
        $vecchiolibro = 0;
        while ($row=mysqli_fetch_array ($ris)) {
        if ($row["Numero"]!=$vecchiolibro) {
            if ($vecchiolibro!=0)
                echo "</td></tr>\n";
            $vecchiolibro = $row["Numero"];
            echo "<tr><td>".str_replace("è","&egrave;",$row["Nome"])."</td><td>".$row["Abb"]."</td><td>".$row["AbbR"];
        }
        else
            echo ", ".$row["AbbR"];
             }
        echo "</td></tr>\n";
       }
       else
            errore2("interrogazione database per abbreviazioni");
echo "</table>";
require("piede.php");
?>
