<?
$descriz = "Uno schema per leggere tutta la Bibbia in un anno";
$key = "letture quotidiane,calendario,liturgia,anno";
$titolo = "Letture quotidiane";
$sezione = "Strumenti";
require("capo.php");
include("funzioni.php");
include("conn.php");
?>
<h1>Letture quotidiane per tutto l'anno</h1>
<p class="primalettera">Questa pagina d&agrave; quattro brani per ogni giorno dell'anno - uno dai libri storici, uno dagli scritti, uno dai profeti e uno dal Nuovo Testamento.
Se segui queste letture, leggerai tutta la Bibbia in un anno. Puoi anche andare ogni giorno alla pagina con la <a href="letoggi.php" title="4 brani biblici da leggere">lettura del giorno</a> per seguire le letture, oppure iscriverti a una <a href="mailing_list.php#letture">mailing list</a> per ricevere tutte le letture ogni giorno automaticamente.</p>
<?
$sql="SELECT * FROM Letture ORDER BY Mese,Giorno";
if ($ris=mysqli_query ($conn, "$sql")) {
  echo "<table><tr><th>Data</th><th>Brani</th></tr>";
  while ($row=mysqli_fetch_array ($ris)) {
    echo "<tr><td>".$row["Giorno"]."/".$row["Mese"]."</td><td>";
    linkbib($row["Brano"]);
    echo "</td></tr>";
  }
  echo "</table>";
}
else
  errore2("interrogazione database per letture");
?>
<script type="text/javascript">LPNnoscript = 1;</script>
<?
require("piede.php");
?>
