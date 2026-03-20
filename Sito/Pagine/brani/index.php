<?
$non_includere_quot = 1;
include("../conn.php");
include("../vistesto.php");

$descriz = "Un elenco di brani difficili nella Bibbia, con una spiegazione";
$key = "difficolt&agrave;, brani, brani difficili, spiegazione, interpretazione, commentario";
$titolo = "Brani difficili nella Bibbia";
$sezione = "Strumenti";
require("../capo.php");
?>
<script language="JavaScript" src="https://www.laparola.net/popup.js"></script>
<h1>Brani difficili nella Bibbia</h1>
<p class="primalettera">Ci sono alcune domande comuni sulla Bibbia, perch&eacute; certi brani sono pi&ugrave; difficili da capire che altri. Ho raccolto le mie risposte a queste domande in questa parte del sito, per aiutare quelli che porranno queste domande nel futuro. Per iniziare, puoi leggere l'introduzione, e puoi cliccare su una delle domande qui sotto oppure digitare il riferimento di un brano.</p>
<p>Ci sono alcuni altri modi per leggere questo testo:</p>
<ul>
<li>scaricare un <a href="Brani difficili nella Bibbia.pdf">file PDF</a> che contiene tutte le domande e risposte in un unico documento;</li>
<li>installare le risposte come un commentario del <a href="/programma/windows.php">programma della Bibbia per Windows</a> (usa il comando <i>Aggiorna</i> del menu <i>Strumenti</i> per scaricarli e installarli) o come un commentario del <a href="/programma/android.php">programma della Bibbia per Android</a> (usa il comando <i>Gestione libreria</i>);</li>
<li>comprare il testo per <a href="https://www.amazon.it/dp/B07KT3SVTG">Kindle</a> oppure come <a href="https://www.amazon.it/dp/1790243548">libro cartaceo</a>;</li>
<li>comprare la raccolta completa di 4 libri per <a href="https://www.amazon.it/dp/B0965RGT3G">Kindle</a> oppure come <a href="https://www.amazon.it/dp/B095PDTBSJ">libro cartaceo</a>.</li>
</ul>

<form action="brani.php" method="post">
<p><label for="brano">Brano:</label>&nbsp;<input class="text" type="text" name="r" id="brano" maxlength="255" size="30" />
<!--</p><p>-->
<input class="submit" type="submit" name="Submit" value="Ricercare" />
<!--<input class="reset" type="reset" name="Reset" value="Annulla" />-->
</p>
</form>
<?
$sql = "SELECT * FROM Brani";
if ($ris = mysqli_query($conn, "$sql")) {
	echo "<table>";
	while ($row=mysqli_fetch_array ($ris)) {
	  if ($row["Libro1"]==0) {
	  	if ($row["Versetto2"]>0)
        	echo "<tr><td>Generale ".$row["Versetto2"]."</td>";
		else
			echo "<tr><td></td>";
        echo "<td><a href=\"brani.php?b=".$row["id_b"]."\">".$row["Domanda"]."</a></td></tr>\n";	  		  	
	  }
	  else {
  	    $rif = converti_rif3($row["Libro1"],$row["Capitolo1"],$row["Versetto1"],$row["Libro2"],$row["Capitolo2"],$row["Versetto2"]);
        echo "<tr><td><a href=\"JavaScript:popup('$rif');\">$rif</a></td>";
        echo "<td><a href=\"brani.php?b=".$row["id_b"]."\">".$row["Domanda"]."</a></td></tr>\n";	  	
	  }
    }
	echo "</table>";
}
?>
<!--
<p><strong>Nota:</strong> &Egrave; pi&ugrave; veloce cercare uno studio su un brano senza visitare questo sito installando la <a href="/toolbar.php">toolbar</a> di LaParola.net.<br />
<a href="/toolbar.php"><img src="/immagini/toolbar.jpg" width="850" height="112" alt="La toolbar del sito" style="border:0px" /></a></p>
-->

<script type="text/javascript">LPNnoscript = 1;</script>
<?
require("../piede.php");
?>
