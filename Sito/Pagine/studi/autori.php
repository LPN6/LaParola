<?
$nome="";
if (isset($_REQUEST["nome"])) {
  $nome=$_REQUEST["nome"];
  $nome = str_replace("<", "", $nome); // affinché tag HTML non possono essere inseriti nella pagina
  $nome = str_replace(">", "", $nome);
}

$descriz = "Autori degli studi biblici: $nome";
$key = "autori,studi biblici,studio biblico,studio";
include("../conn.php");
include("../funzioni.php");
$sezione = "Studi";
$sezioneurl = "/studi/";

if ($nome=="") {
  $sql = "SELECT DISTINCT Autori.nome, Autori.emailpubblico, Autori.sito, Autori.email, COUNT(id_s) AS nstudi FROM Autori, Studi WHERE Autori.id_a=Studi.id_a GROUP BY Autori.id_a ORDER BY nome";
$titolo = "Autori";
require("../capo.php");
  if ($ris = mysqli_query($conn, "$sql")) {
    echo "<h1>Autori</h1>";
    echo "<table style=\"table-layout:fixed;width:100%;\"><tr><th>Nome</th><th>E-mail</th><th>Sito</th><th width=\"10%\"><div style=\"text-align:right\">Studi</div></th></tr>";
    while ($row=mysqli_fetch_array ($ris)) {
      echo "<tr><td><div style=\"overflow-wrap:break-word\"><a href=\"autori.php?nome=".urlencode($row["nome"])."\">".utf8_encode($row["nome"])."</a></div></td>";
      if ($row["emailpubblico"]=="S")
        echo "<td><div style=\"overflow-wrap:break-word\"><a href=\"mailto:".$row["email"]."\">".$row["email"]."</a></div></td>";
      else
        echo "<td></td>";
      $sito2 = utf8_encode($row["sito"]);
      if (substr($sito2,0,4)!="http" && $sito2!="")
        $sito2 = "http://".$sito2;
      echo "<td><div style=\"overflow-wrap:break-word\"><a href=\"".$sito2."\">".$sito2."</a></div></td>";
      echo "<td><div style=\"text-align:right\"><a href=\"studi.php?autore=".urlencode($row["nome"])."\">".$row["nstudi"]."</a></div></td>";
      echo "</tr>";
    }
    echo "</table>";
  }
  else
    errore2("interrogazione database per studi");
}
else {
  $sql = "SELECT * FROM Autori WHERE nome=\"$nome\"";

$titolo = "Autore - ".$nome;
require("../capo.php");
  if ($ris = mysqli_query($conn, "$sql")) {
    if (mysqli_num_rows($ris)==0) {
      echo "<h1>Errore</h1>";
      echo "<p>L'autore ".$nome." non &egrave; stato trovato.</p>";
    }
    else {
      $row=mysqli_fetch_array($ris);
      echo "<h1>".utf8_encode($row["nome"])."</h1>";
      if ($row["emailpubblico"]=="S")
        echo "<p>E-mail: <a href=\"mailto:".$row["email"]."\">".$row["email"]."</a></p>";
      $sito2 = utf8_encode($row["sito"]);
      if (substr($sito2,0,4)!="http" && $sito2!="")
        $sito2 = "http://".$sito2;
      if ($sito2!="")
        echo "<p>Sito: <a href=\"".$sito2."\">".$sito2."</a></p>";
      echo "<p>".utf8_encode(nl2br($row["descrizione"]))."</p>";
      echo "<p><a href=\"studi.php?autore=".urlencode($row["nome"])."\">Studi pubblicati su questo sito</a></p>";
    }
  }
  else {
    errore2("interrogazione database per studi");
  }
}

require("../piede.php");
?>
