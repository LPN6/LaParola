<?
function formatta_data($d) {
  $a = substr($d,8,2);
  if ($a[0]=="0") $a = substr($a,1,1);
  $b = substr($d,5,2);
  if ($b[0]=="0") $b = substr($b,1,1);
  return $a."/".$b."/".substr($d,0,4);
}

function voti($s, $c) {
  $sql = "SELECT voto FROM Voti WHERE id_s=".$s;
  $nvoti = 0;
  $totalevoto = 0;
  if ($ris2 = mysqli_query($c, "$sql")) {
    while ($row2=mysqli_fetch_array($ris2)) {
      $nvoti += 1;
      $totalevoto += $row2["voto"];
    }
  }
  $ret = $nvoti."|";
  if ($nvoti>0)
   $ret .= round($totalevoto/$nvoti*10)/10;
  return $ret;
}
?>