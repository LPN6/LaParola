<?
$descriz = "Commentari sulla Bibbia";
$key = "nuovo testamento,bibbia,commentario,capitoli";
$titolo = "Commentari sulla Bibbia";
$sezione = "Commentari sulla Bibbia";
require("../capo.php");
?>
<h1>Commentari sulla Bibbia</h1>
<p>Qui ci sono dei link a tutti i libri della <strong>Bibbia</strong>, da cui ci sono dei link ad ogni capitolo dei libri.
I libri del Nuovo Testamento hanno due commenti, quelli dell'Antico Testamento ne hanno uno.
I commentari sono descritti <a href="/versioni.php#Commentario">qui</a>.</p><p>
<?
include("../conn.php");
global $conn;
$sql = "SELECT Nome,Numero FROM Libri";
if ($ris=mysqli_query ($conn, "$sql")) {
  while ($row=mysqli_fetch_array ($ris)) {
    $n = $row["Numero"];
    if ($n<=16 || $n==19 || ($n>=22 && $n<=26) || ($n>=29 && $n<=31) || $n>=33) {
      $libro = $row["Nome"];
      $libroHTML = htmlentities($libro, 0, "ISO-8859-1");      
      $v = "versioni[]=Commentario";
      if ($n >= 47)
        $v = $v."&versioni[]=CommentarioNT";
      if ($n==72 || $n==71 || $n==70 || $n==64 || $n==42)
        echo "<a href=\"/testo.php?".$v."&riferimento=".$libro."\" title=\"".$libro."\">".$libro."</a><br />";
      else
        echo "<a href=\"libro.php?n=".$n."\" title=\"".$libroHTML."\">".$libroHTML."</a><br />";
    }
  }
}
echo "</p>";
require("../piede.php");
?>
