<? 
    include("conn.php");
    include("vistesto.php");
    $oggi = getdate(strtotime("+0 hours"));
    $omese = $oggi["mon"];
    $ogiorno = $oggi["mday"];
    $sql="SELECT Brano FROM Letture WHERE Mese=".$omese." AND Giorno=".$ogiorno;
    if ($ris=mysqli_query ($conn, "$sql")) {
        if ($ogiorno==1 || $ogiorno==8 || $ogiorno==11)
            $day = "dell\'".$ogiorno;
        else
            $day = "del ".$ogiorno;
        echo "document.write('<h2>Lettura ".$day."/".$omese."</h2>');";
        echo "document.write('<p>Da <a href=\"https://www.laparola.net/\" target=\"_blank\">LaParola</a></p>');";
        $row=mysqli_fetch_array ($ris);
        echo 'document.write("';
        $testo = htmlentities(html_entity_decode(str_replace('"', '\"', gettesto($row["Brano"],array("Nuova Riveduta")))));
        $testo = str_replace("&gt;", ">", $testo);
        $testo = str_replace("&lt;", "<", $testo);
        $testo = str_replace("\n", "", $testo);
       	echo $testo;
		echo '");';
    }
?>
