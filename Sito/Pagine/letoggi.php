<?php
if (isset($_REQUEST["omese"]))
  $omese = (int)$_REQUEST["omese"];
if (isset($_REQUEST["ogiorno"]))
  $ogiorno = (int)$_REQUEST["ogiorno"];

$lqvers = (isset($_REQUEST["lqvers"])?$_REQUEST["lqvers"]:"");
if ($lqvers=="")
    $lqvers = (isset($_COOKIE["lqvers"])?$_COOKIE["lqvers"]:"");
if ($lqvers=="NuovaRiveduta") $lqvers = "Nuova Riveduta";
if ($lqvers=="NuovaDiodati") $lqvers = "Nuova Diodati";
if ($lqvers=="Riveduta2020") $lqvers = "Riveduta 2020";
if ($lqvers=="NuovaRiveduta1994") $lqvers = "Nuova Riveduta 1994";
if ($lqvers=="")
    $lqvers = "Nuova Riveduta";
SetCookie("lqvers", str_replace(' ','',$lqvers), time()+3600000);

if (!isset($omese) || !isset($ogiorno) || $omese==0 || $ogiorno==0) {
    echo "<script type=\"text/javascript\" language=\"JavaScript\">\n";
    echo "<!--\n";
    echo "function DoPost() {\n";
    echo "data = new Date();\n";
    echo "mese = data.getMonth()+1;\n";
    echo "giorno = data.getDate();\n";
    echo "document.scheda.omese.value = mese;\n";
    echo "document.scheda.ogiorno.value = giorno;\n";
    echo "document.scheda.submit();\n"; 
	echo "}\n";
    echo "-->\n";
    echo "</script>\n";
    echo "<link rel=\"stylesheet\" href=\"/stili/stilebase.css\" type=\"text/css\" />\n";
    echo "</head>\n";
    echo "<body onload=\"DoPost()\">\n";
}
else {
	$descriz = "La lettura della Bibbia per oggi: $ogiorno/$omese";
	$key = "lettura quotidiana,letture,giorno,lettura del giorno";
	$titolo = "Lettura del giorno $ogiorno/$omese";
	$sezione = "Strumenti";
	require("capo.php");	
}
?>
<h1>Lettura del giorno</h1>
<p class="primalettera">Questa pagina d&agrave; quattro brani - uno dai libri storici, uno dagli scritti, uno dai profeti e uno dal Nuovo Testamento.
Fanno parte di uno <a href="letture.php" title="La Bibbia divisa in 365 letture, da leggere in un anno">schema per leggere tutta la Bibbia</a> in un anno. Puoi anche <a href="mailing_list.php#letture">ricevere un messaggio di posta elettronica</a> ogni giorno con la lettura del giorno, oppure <a href="inserire_bibbia.php#letture">inserire la lettura del giorno nel tuo sito</a>.</p>
<noscript>
<p>Per funzionare meglio, questa pagina richiede JavaScript nel browser.</p>
<?php
$oggi = getdate();
$om = $oggi["mon"];
$og = $oggi["mday"];
echo "<p>Per andare alla lettura di oggi, fa' clic su <a href=\"letoggi.php?omese=$om&ogiorno=$og\">questo link</a>.</p>";
?>
</noscript>

<script type="text/javascript">
<!--
function cambiaVersione() {
    window.location.href = window.location.href.split("#")[0]+"?lqvers="+document.getElementById('versione').value;
}

var s="<p>Seleziona la versione da usare: <select name=\"versione\" id=\"versione\" onChange=\"cambiaVersione()\">";
var vers = getCookie("lqvers");
if (!vers || (vers!="C.E.I." && vers!="NuovaDiodati" && vers!="Riveduta2020" && vers!="Riveduta" && vers!="Ricciotti" && vers!="Tintori" && vers!="Martini" && vers!="Diodati"))
        s=s+"<option selected=\"selected\" value=\"Nuova Riveduta\">Nuova Riveduta</option>";
else
        s=s+"<option value=\"Nuova Riveduta\">Nuova Riveduta</option>";
if (vers=="C.E.I.")
        s=s+"<option selected=\"selected\" value=\"C.E.I.\">C.E.I.</option>";
else
        s=s+"<option value=\"C.E.I.\">C.E.I.</option>";
if (vers=="NuovaDiodati")
        s=s+"<option selected=\"selected\" value=\"Nuova Diodati\">Nuova Diodati</option>";
else
        s=s+"<option value=\"Nuova Diodati\">Nuova Diodati</option>";
if (vers=="Riveduta2020")
        s=s+"<option selected=\"selected\" value=\"Riveduta 2020\">Riveduta 2020</option>";
else
        s=s+"<option value=\"Riveduta 2020\">Riveduta 2020</option>";
if (vers=="NuovaRiveduta1994")
        s=s+"<option selected=\"selected\" value=\"Nuova Riveduta 1994\">Nuova Riveduta (1994)</option>";
else
        s=s+"<option value=\"Nuova Riveduta 1994\">Nuova Riveduta (1994)</option>";
if (vers=="Riveduta")
        s=s+"<option selected=\"selected\" value=\"Riveduta\">Luzzi/Riveduta</option>";
else
        s=s+"<option value=\"Riveduta\">Luzzi/Riveduta</option>";
if (vers=="Ricciotti")
        s=s+"<option selected=\"selected\" value=\"Ricciotti\">Ricciotti</option>";
else
        s=s+"<option value=\"Ricciotti\">Ricciotti</option>";
if (vers=="Tintori")
        s=s+"<option selected=\"selected\" value=\"Tintori\">Tintori</option>";
else
        s=s+"<option value=\"Tintori\">Tintori</option>";
if (vers=="Martini")
        s=s+"<option selected=\"selected\" value=\"Martini\">Martini</option>";
else
        s=s+"<option value=\"Martini\">Martini</option>";
if (vers=="Diodati")
        s=s+"<option selected=\"selected\" value=\"Diodati\">Diodati</option>";
else
        s=s+"<option value=\"Diodati\">Diodati</option>";
s=s+"</select></p>";
document.write(s);
//-->
</script>

<?php
if (!isset($omese) || !isset($ogiorno)) {
    echo "<form name=\"scheda\" action=\"letoggi.php\" method=\"post\">";
    echo "<input type=\"hidden\" name=\"omese\" />\n";
    echo "<input type=\"hidden\" name=\"ogiorno\" />\n";
    echo "</form>\n";
    echo "<p>Attendere prego... Caricamento in corso.</p>";
    echo "</body></html>";
}
else {
    include("conn.php");
    include("vistesto.php");
    $sql = "SELECT Brano FROM Letture WHERE Mese=".$omese." AND Giorno=".$ogiorno;
    if ($ris=mysqli_query ($conn, "$sql")) {
        if ($ogiorno==1 || $ogiorno==8 || $ogiorno==11)
            $day = "dell'".$ogiorno;
        else
            $day = "del ".$ogiorno;
        echo "<h2>Lettura ".$day."/".$omese."</h2>";
        $row=mysqli_fetch_array ($ris);
        vistesto($row["Brano"], array($lqvers));
    }
    else
        errore2("interrogazione database per lettura");
    require("piede.php");
}
?>
