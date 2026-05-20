<?
//error_reporting(E_ALL);
include("conn.php");
include("vistesto.php");
$mandare = true;

//$oggi = mktime(date("H")+6,1,0,date("m"),date("j"),date("Y"));
$oggi = mktime(date("H"),1,0,date("m"),date("j"),date("Y"));
$oggigiorno = date("j", $oggi);
$oggimese = date("n", $oggi);
$oggianno = date("Y", $oggi)+0;

if ($mandare) {
  $mandato = mktime(1,0,0,$oggimese,$oggigiorno,$oggianno);
  $mandatogiorno = date("j", $mandato);
  $mandatomese = date("n", $mandato);
  $mandatoanno = date("Y", $mandato)+0;
global $conn;
  $sql="SELECT Brano FROM Letture WHERE Mese=".$mandatomese." AND Giorno=".$mandatogiorno;
//  echo "<p>qui1</p>";
  if ($ris=mysqli_query ($conn, "$sql")) {
//  	  echo "<p>qui2</p>";
    $row=mysqli_fetch_array($ris);
    $messaggio = gettesto($row["Brano"],array("Nuova Riveduta"));
    $messaggio = str_replace("<h3>","\n\n--- ",$messaggio);
    $messaggio = str_replace("</h3>"," ---",$messaggio);
    $messaggio = str_replace("<p></p>","",$messaggio);
    $messaggio = str_replace("<p>","\n",$messaggio);
    $messaggio = str_replace("<br />","\n",$messaggio);
    $messaggio = html_entity_decode($messaggio);
    $messaggio = strip_tags($messaggio);
    $messaggio = str_replace("\n "," ",$messaggio);
    $messaggio = str_replace("\n ","\n",$messaggio);
    $messaggio = "La lettura per il giorno ".$mandatogiorno."/".$mandatomese.".\nPer non ricevere più questi messaggi, leggi le istruzioni in fondo a questo messaggio.\n".$messaggio;

    $messdaspedire = "";
    $lungriga = 0;
    $tok = strtok($messaggio," ");
    while ($tok) {
      if ($lungriga+strlen($tok)>76) {
        $lungriga = 0;
        $messdaspedire .= "\n";
      }
      elseif ($lungriga>0)
        $messdaspedire .= " ";
      $messdaspedire .= $tok;

      $lungriga += strlen($tok)+1;
      $acapo = strrpos($tok, "\n");
      if (!($acapo===false))
        $lungriga = strlen($tok)-$acapo-1;

      $tok = strtok(" ");
    }

    $headers = "From: LaParola.Net <info@laparola.net>";
    $headers .= "\nContent-Type: text/plain; charset=iso-8859-1";
    $headers .= "\nContent-Transfer-Encoding: 8bit";
	$headers .= "\nX-Mailer: PHP " . phpversion();

// metodo alternativo
/*
    if ($_SERVER["HTTP_HOST"]!="localhost") {
      for ($i=1; $i<sizeof($arr_abbonati); $i++) {
        mail($arr_abbonati[$i], "Lettura quotidiana", $messdaspedire, $headers);
      }
    }
*/

//      $headersBcc = "\nBcc: ";
//      $headersBcc .= "\n";
      if ($_SERVER["HTTP_HOST"]!="localhost") {
//      echo $_SERVER["HTTP_HOST"];
//      ini_set("SMTP", "laparola");
//	  	ini_set("sendmail_from", "info@laparola.net");
        if (!mail ("lettura-quotidiana2@googlegroups.com", "Lettura quotidiana ".$mandatogiorno."/".$mandatomese, $messdaspedire, $headers, "-finfo@laparola.net")) {
// per provare
//echo "<p>"."Lettura quotidiana ".$mandatogiorno."/".$mandatomese."</p>";
//echo "<p>$messdaspedire</p>";
//echo "<p>$headers</p>";
//        if (!mail ("info@laparola.net", "Lettura quotidiana ".$mandatogiorno."/".$mandatomese, $messdaspedire, $headers, "-finfo@laparola.net")) {
          mail("info@laparola.net", "Errore in LQ", "Non spedito", "From: LaParola.Net <info@laparola.net>");
		  //echo "errore";
		}
      //else {
      //  echo "<html><body><p>Messaggio spedito<br>";
      //  echo $mandatogiorno."/".$mandatomese."</p></body></html>";
      //}
      }
      else {
        echo "Messaggio spedito<br>";
        echo $mandatogiorno."/".$mandatomese;
      }
  }
}
?>
