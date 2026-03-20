<?
$descriz = "File per il programma della Bibbia per Macintosh, Linux, Android";
$key = "java, android, linux, macintosh";
$titolo = "File per il programma per Android, Linux, Macintosh";
$sezione = "Programma";
require("../capo.php");
?>
<h1>File per il programma per Android, Linux, Macintosh</h1>
<p class="primalettera">Per installare uno di questi testi, clicca sul nome del testo per scaricare il file, scompatta il file,
e copia il file contenuto alla cartella che contiene i file del programma:</p>
<p>
<b><a href="/programma/android.php">Android</a>:</b> la cartella <i>laparola</i> della scheda SD<br />
<!--<b><a href="#">Macintosh</a>:</b> ..<br />
<b><a href="#">Linux</a>:</b> ..<br />-->
</p>
<table style="table-layout:fixed;width:100%;" cellspacing="10">
<tr>
<th>Nome</th>
<th>Descrizione</th>
<th align="center">Versione</th>
<th align="center">Dimensione (Kb)</th>
<th align="left">Tipo di testo</th>
</tr>
<?
$xml=new SimpleXMLElement(file_get_contents("aggiorna2.xml"));
foreach ($xml->file as $f) {
echo "<td><div style=\"overflow-wrap:break-word\"><a href=\"".htmlentities(str_replace("ã","a",$f->url2), 0,'UTF-8')."\">".htmlentities($f->componente, 0,'UTF-8')."</a></div></td>\n";
echo "<td><div style=\"overflow-wrap:break-word\">".htmlentities($f->descrizione, 0,'UTF-8')."</div></td>\n";
echo "<td align=\"center\">".$f->versione."</td>\n";
echo "<td align=\"center\">".(intval($f->dimensione2/1024)+1)."</td>\n";
echo "<td><div style=\"overflow-wrap:break-word\">".$f->tipo."</div></td>\n";
echo "</tr>\n";
}
?>
</table>
<?
require("../piede.php");
?>