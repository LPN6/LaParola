<?
$p = (isset($_REQUEST["p"])?$_REQUEST["p"]:"αββα");
$p = str_replace("<", "", $p); // affinché tag HTML non possono essere inseriti nella pagina
$p = str_replace(">", "", $p);
//$p = utf8_encode($p);
include("../conn.php");
$sql = "SELECT Strong FROM GVocab WHERE Radice='$p'";
if ($ris=mysqli_query($conn, "$sql")) {
   if (mysqli_num_rows($ris)>0) {
      $row = mysqli_fetch_array ($ris);
      echo "<p><span class=\"uni\">$p:</span><br />".$row["Strong"]."</p>";
    }
    else {
//      echo "<p>3</p>";
    }
}
?>
