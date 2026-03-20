<?
$non_includere_quot = 1;
include("../conn.php");
include("../vistesto.php");
include("funzionis.php");
$sezione = "Studi";
$sezioneurl = "/studi/";
?>

<?
//$cid=1;$cnome="rmw";
$sql = "SELECT * FROM Studi, Autori";
$cond = "Studi.id_a=Autori.id_a";
$autore = "";
if (isset($_REQUEST["autore"])) {
  $autore = $_REQUEST["autore"];
  $autore = str_replace("<", "", $autore); // affinché tag HTML non possono essere inseriti nella pagina
  $autore = str_replace(">", "", $autore);
  $autore = str_replace("\"", "", $autore);
}
if ($autore!="") {
  $cond .= " AND ";
  $cond .= "nome LIKE '%$autore%'";
}
$rif="x";
$brano = "";
if (isset($_REQUEST["brano"])) {
  $brano = $_REQUEST["brano"];
  $brano = str_replace("<", "", $brano); // affinché tag HTML non possono essere inseriti nella pagina
  $brano = str_replace(">", "", $brano);
  $brano = str_replace("\"", "", $brano);
}
if ($brano!="") {
  $rif = converti_rif($brano);
  if ($rif!="") {
    $cond .= " AND ";
    $l1=ord($rif[0]);
    $c1=ord($rif[1]);
    $v1=ord($rif[2]);
    $l2=ord($rif[3]);
    $c2=ord($rif[4]);
    $v2=ord($rif[5]);
    $cond .= "(libro1<$l2 OR (libro1=$l2 AND (capitolo1<$c2 OR (capitolo1=$c2 AND versetto1<=$v2)))) AND (libro2>$l1 OR (libro2=$l1 AND (capitolo2>$c1 OR (capitolo2=$c1 AND versetto2>=$v1))))";
  }
}
$data = "";
if (isset($_REQUEST["data"])) {
  $data = $_REQUEST["data"];
  $data = str_replace("<", "", $data); // affinché tag HTML non possono essere inseriti nella pagina
  $data = str_replace(">", "", $data);
  $data = str_replace("\"", "", $data);
}
if ($data!="") {
  if ($cond!="")
    $cond .= " AND ";
  $cond .= "TO_DAYS(NOW())-TO_DAYS(data)<=".$data;
}
$s = 0;
if (isset($_REQUEST["s"]))
  $s = (int)$_REQUEST["s"];
if ($s!=0) {
  if ($cond!="")
    $cond .= " AND ";
  $cond .= "id_s=\"$s\"";
}
if ($cond!="")
  $sql .= " WHERE ".$cond;
$sql .= " ORDER BY libro1,capitolo1,versetto1,libro2,capitolo2,versetto2";

$cid = 0;
if (isset($_REQUEST["cid"]))
  $cid = (int)$_REQUEST["cid"];
$cnome = "";
if (isset($_REQUEST["cnome"])) {
  $cnome = $_REQUEST["cnome"];
  $cnome = str_replace("<", "", $cnome); // affinché tag HTML non possono essere inseriti nella pagina
  $cnome = str_replace(">", "", $cnome);
  $cnome = str_replace("\"", "", $cnome);
}
$votovecchio = 0;
if (isset($_REQUEST["votovecchio"]))
  $votovecchio = (int)$_REQUEST["votovecchio"];
$votofatto = 0;
if (isset($_REQUEST["votofatto"]))
  $votofatto = (int)$_REQUEST["votofatto"];

if ($ris = mysqli_query($conn, "$sql")) {
  if (mysqli_num_rows($ris)==0) {
    $titolo = "Nessuno trovato";
    $descriz = "Studio biblico non trovato";
    $key = "studio biblico,studi biblici,studio";
    require("../capo.php");
    echo "<!-- RESULT LIST START -->";
    if ($rif=="")
      echo "<p><strong>Nota:</strong> Il riferimento $brano non &egrave; stato riconosciuto.</p>";
    echo "<h1>Nessuno studio trovato</h1>";
    echo "<p>Purtroppo, non c'&egrave; nessuno studio con i criteri di questa ricerca. <a href=\"/studi/\">Ritorna alla pagina principale</a> degli studi e prova un'altra ricerca.</p>";
    if ($brano!="" && (ord($rif[0])>=47 || ord($rif[0])==47))
      echo "<p>C&egrave; invece un <a href=\"../testo.php?riferimento=".$brano."&versioni[]=Commentario\">commentario su $brano</a>.</p>";
    echo "<!-- RESULT LIST END -->";
  }
  if (mysqli_num_rows($ris)==1) {
    $row=mysqli_fetch_array($ris);
    if ($cid>0 && $cnome!=$row["nome"]) {
      $sql = "";
      if ($s>0 && $votofatto!=0) {
        if ($votovecchio<0)
          $sql = "INSERT INTO Voti(id_s,id_a,voto) VALUES ($s,$cid,$votofatto)";
        else
          $sql = "UPDATE Voti SET voto=$votofatto WHERE id_s=$s AND id_a=$cid";
        $risvoto = mysqli_query($conn, "$sql");
      }
    }

    $titolo = $row["titolo"];
    $descriz = $titolo."- uno studio biblico";
    $key = "studio biblico,studi biblici,studio";
    require("../capo.php");
    echo "<!-- RESULT LIST START -->";
    echo "<!-- RESULT ITEM START -->";
    if ($rif=="")
      echo "<p><strong>Nota:</strong> Il riferimento $brano non &egrave; stato riconosciuto.</p>";
    echo "<h1>".$row["titolo"]."</h1>";
    echo "<p>".converti_rif3($row["libro1"],$row["capitolo1"],$row["versetto1"],$row["libro2"],$row["capitolo2"],$row["versetto2"])."</p>";
    if ($row["indirizzo"]!="") {
        $ind = $row["indirizzo"];
        if (strlen($ind)<35)
            $ind = "a <a target=\"_blank\" href=\"".$ind."\">".$ind."</a>";
        else // non si visualizza bene su cellulare quindi...
            $ind = "<a target=\"_blank\" href=\"".$ind."\">qui</a>";             
      echo "<p>Questo studio si trova $ind.</p>";
    }
    $n = $row["nome"];
    echo "<p>Scritto da <a href=\"autori.php?nome=".urlencode($n)."\">".$n."</a>.</p>";
    echo "<p>".formatta_data($row["data"])."</p>";
    $nStudio = $row["id_s"];
    $v = voti($nStudio, $conn);
    $pos = strpos($v,"|");
    $nvoti = substr($v,0,$pos);
    echo "<p>Numero di voti per questo studio: ".$nvoti;
    if ($nvoti>0)
      echo "&nbsp;&nbsp;&nbsp;Media: ".substr($v,$pos+1);
    echo "</p>";
    if ($cid>0 && $cnome!=$n) {
      $sql = "SELECT Voto FROM Voti WHERE id_s=".$nStudio." AND id_a=".$cid;
      $voto = -1;
      if ($ris2 = mysqli_query($conn, "$sql")) {
        if (mysqli_num_rows($ris2)==1) {
          $row2 = mysqli_fetch_array($ris2);
          $voto = $row2["Voto"];
        }
      }
      echo "<form action=\"studi.php\" method=\"post\"><label>Da' un voto a questo studio:&nbsp";
      echo "<select name=\"votofatto\" style=\"background:#68ffff\" >";
      if ($voto<0)
        echo "<option selected> ";
      for ($i=1; $i<11; $i++) {
        echo "<option";
        if ($i==$voto)
          echo " selected";
        echo ">".$i;
      }
      echo "</select></label>&nbsp;";
      echo "<input class=\"submit\" type=\"submit\" value=\"Vota\" />";
      echo "<input type=\"hidden\" name=\"s\" value=".$nStudio." />";
      echo "<input type=\"hidden\" name=\"votovecchio\" value=".$voto." />";
      echo "</form>";
    }
    if ($row["testo"]!="") {
      echo "<hr />";
      echo "<p>".str_replace("&amp;", "&", utf8_encode(nl2br($row["testo"])))."</p>";
    }
    echo "<!-- RESULT ITEM END -->";
    echo "<!-- RESULT LIST END -->";
  }
  if (mysqli_num_rows($ris)>1) {
    $titolo = "Studi trovati";
    $descriz = "Studio biblico";
    $key = "studio biblico,studi biblici,studio";
    require("../capo.php");
    echo "<!-- RESULT LIST START -->";
    if ($rif=="")
      echo "<p><strong>Nota:</strong> Il riferimento $brano non &egrave; stato riconosciuto.</p>";
    echo "<h1>Studi</h1>";
    echo "<p>I seguenti studi sono stati trovati. Scegli quello desiderato.</p>";
    echo "<table style=\"table-layout:fixed;width:100%;\"><tr><th>Titolo</th><th>Brano</th><th>Autore</th><th>Data</th><th width=\"10%\"><div style=\"text-align:right\">Voto</div></th></tr>";
    while ($row=mysqli_fetch_array ($ris)) {
      echo "<!-- RESULT ITEM START -->";
      echo "<tr><td><div style=\"overflow-wrap:break-word\"><a href=\"studi.php?s=".$row["id_s"]."\">".utf8_encode($row["titolo"])."</a></div></td>";
      echo "<td>".converti_rif3($row["libro1"],$row["capitolo1"],$row["versetto1"],$row["libro2"],$row["capitolo2"],$row["versetto2"])."</td>";
      echo "<td><div style=\"overflow-wrap:break-word\"><a href=\"autori.php?nome=".urlencode($row["nome"])."\">".utf8_encode($row["nome"])."</a></div></td>";
      echo "<td><div style=\"overflow-wrap:break-word\">".formatta_data($row["data"])."</div></td>";
      $v = voti($row["id_s"], $conn);
      $pos = strpos($v,"|");
      $nvoti = substr($v,0,$pos);
      if ($nvoti>0)
        $v = substr($v,$pos+1);
      else
        $v = "s.v.";
      echo "<td><div style=\"text-align:right\">".$v."</div></td></tr>\n";
      echo "<!-- RESULT ITEM END -->";
    }
    echo "</table>";
    echo "<!-- RESULT LIST END -->";
    if ($brano!="" && $rif!="" && (ord($rif[0])>=47 || ord($rif[0])==1))
      echo "<p>Vedi anche il <a href=\"../testo.php?riferimento=".$brano."&versioni[]=Commentario\">commentario su $brano</a>.</p>";
  }
}
else {
  $titolo = "Errore";
  require("../capo.php");
    errore2("interrogazione database per studi");
}

require("../piede.php");
?>
