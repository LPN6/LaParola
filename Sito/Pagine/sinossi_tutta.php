<?
header("Content-type: text/html; charset=utf-8");
$descriz = "Sinossi dei Vangeli";
$key = "sinossi,vangelo, evangelo, vangeli, evangeli";
$titolo = "Sinossi dei Vangeli";
$sezione = "Strumenti";
require("capo.php");
include("conn.php");
include("vistesto.php");

$riferimento = (isset($_REQUEST["riferimento"])?$_REQUEST["riferimento"]:"");
$riferimento = str_replace("<", "", $riferimento); // affinché tag HTML non possono essere inseriti nella pagina
$riferimento = str_replace(">", "", $riferimento);
$riferimento = str_replace("\"", "", $riferimento);
if (strlen($riferimento)>0) {
	$rif3 = converti_rif($riferimento);
	if (strlen($rif3)>0) {
		$nLibro = ord($rif3[0]);
		if ($nLibro<47 || $nLibro>50) {
			echo "<h2>Il versetto non &egrave; nei Vangeli (Matteo, Marco, Luca o Giovanni).</h2>";			
			echo "<script type='text/javascript'>alert('Il versetto non è nei Vangeli (Matteo, Marco, Luca o Giovanni).');</script>";
		}
		else {
			$nCapitolo = ord($rif3[1]);
			$nVersetto = ord($rif3[2]);
			echo "<div id=\"versetto_richiesto\" style=\"display:none\">$nLibro $nCapitolo $nVersetto</div>";
		}
	}
	else {
		echo "<h2>Il riferimento non &egrave; stato riconosciuto.</h2>";
	}
	
}
?>

<script src="https://code.jquery.com/jquery-1.10.2.js"></script>
<script type="text/javascript">
<!--
    if (window.XMLHttpRequest) {
       xhttp = new XMLHttpRequest();
    } else {    // IE 5/6
       xhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }

    xhttp.open("GET", "Harmony of the Gospels.xml", false);
    xhttp.send();
    var xmlDoc = xhttp.responseXML;

	window.onload=function(){
	  cambiaBrano();
	};

function predSucc(vangelo, direzione) {
	iBrano = Number(document.getElementById("selectBrano").value);
	nBrani = brani.length;

	if (vangelo==-1) {
		iBrano += Number(direzione);
		if (iBrano<0) iBrano = 0;
		if (iBrano>=nBrani) iBrano = nBrani - 1;
		document.getElementById("selectBrano").value = iBrano;
		cambiaBrano();
		return;
	}

	riferimento = brani[iBrano][vangelo]; // tipo 47 9:9-13
	if (riferimento.length==0)
		return;
	imigliore = -1;
	if (direzione==-1) {
		cv = primoVersetto(riferimento).split(" ");
		capitolo = Number(cv[0]);
		versetto = Number(cv[1]);
		cmigliore = 0; vmigliore = 0;
		for (i = 0; i<nBrani; i++) {
			rif = brani[i][vangelo];
			if (rif.length > 0) {
				cv = ultimoVersetto(rif).split(" ");
				c = Number(cv[0]);
				v = Number(cv[1]);
				if (c<capitolo || (c==capitolo && v<versetto)) {
					if (c>cmigliore || (c==cmigliore && v>vmigliore)) {
						cmigliore = c;
						vmigliore = v;
						imigliore = i;
					}
				}
			}
		}
	}
	else { // direzione == 1
		cv = ultimoVersetto(riferimento).split(" ");
		capitolo = Number(cv[0]);
		versetto = Number(cv[1]);
		cmigliore = 99; vmigliore = 99;
		for (i = 0; i<nBrani; i++) {
			rif = brani[i][vangelo];
			if (rif.length > 0) {
				cv = primoVersetto(rif).split(" ");
				c = Number(cv[0]);
				v = Number(cv[1]);
				if (c>capitolo || (c==capitolo && v>versetto)) {
					if (c<cmigliore || (c==cmigliore && v<vmigliore)) {
						cmigliore = c;
						vmigliore = v;
						imigliore = i;
					}
				}
			}
		}
	}
	if (imigliore>=0) {			
		document.getElementById("selectBrano").value=imigliore;
		cambiaBrano();	
	}		
}

function primoVersetto(rif) {
	rif = soloVangeli(rif);
	if (rif.length==0)
		return "0 0";
	l = rif.indexOf(" ");
	c = rif.indexOf(":");
	v = rif.length;
	v1 = rif.indexOf("-");
	if (v1>0 && v1<v) v=v1;
	v1 = rif.indexOf(";");
	if (v1>0 && v1<v) v=v1;
	
	return rif.substring(l+1, c)+" "+rif.substring(c+1,v);
}

function ultimoVersetto(rif) {
	rif = soloVangeli(rif);
	if (rif.length==0)
		return "99 99";
		
	v = 0;
	v1 = rif.lastIndexOf("-");
	if (v1>0 && v1>v) v=v1;
	v1 = rif.lastIndexOf(":");
	if (v1>0 && v1>v) v=v1;
	c = rif.lastIndexOf(":");
	l = rif.lastIndexOf(" ", c);
	l1 = rif.lastIndexOf("-", c);
	if (l1>0 && l1>s) l=l1;
	return rif.substring(l+1, c)+" "+rif.substring(v+1);
}

function cambiaVersione() {
	cambiaBrano();
	setCookie("SinossiVers", versione = document.getElementById("selectVersione").value);
}

function loadURL(divcol, u, data) { 
	$(divcol).load(u, data);
}

function cambiaBrano() {
	iBrano = document.getElementById("selectBrano").value;
	if (brani[iBrano][0].length > 0) {
		//document.getElementById("col_matteo").innerHTML = '<object type="text/html" data="sinossi_testo.php?sin_riferimento='+brani[iBrano][0].replace(/47 /g,"Matteo")+'&versione='+document.getElementById("selectVersione").value+'"></object>';
		loadURL("#col_matteo", "sinossi_testo.php", { sin_riferimento: brani[iBrano][0].replace(/47 /g,"Matteo"), versione: document.getElementById("selectVersione").value });
		pulsanteStile("matteo", 0);
		if (brani[iBrano][0]=="47 1:1-17") {
			document.getElementById("matteo_prec").disabled = true;
			document.getElementById("matteo_prec").style.background = "#939393";
		}
		if (brani[iBrano][0]=="47 28:16-20") {
			document.getElementById("matteo_succ").disabled = true;
			document.getElementById("matteo_succ").style.background = "#939393";
		}
	}
	else {
		document.getElementById("col_matteo").innerHTML = "";
		pulsanteStile("matteo", 1);
	}
	
	if (brani[iBrano][1].length > 0) {
		loadURL("#col_marco", "sinossi_testo.php", { sin_riferimento: brani[iBrano][1].replace(/48 /g,"Marco"), versione: document.getElementById("selectVersione").value });
		pulsanteStile("marco", 0);
		if (brani[iBrano][1]=="48 1:1") {
			document.getElementById("marco_prec").disabled = true;
			document.getElementById("marco_prec").style.background = "#939393";
		}
		if (brani[iBrano][1]=="48 16:19-20") {
			document.getElementById("marco_succ").disabled = true;
			document.getElementById("marco_succ").style.background = "#939393";
		}
	}
	else {
		document.getElementById("col_marco").innerHTML = "";
		pulsanteStile("marco", 1);
	}

	if (brani[iBrano][2].length > 0) {
		loadURL("#col_luca", "sinossi_testo.php", { sin_riferimento: brani[iBrano][2].replace(/49 /g,"Luca"), versione: document.getElementById("selectVersione").value });
		pulsanteStile("luca", 0);
		if (brani[iBrano][2]=="49 1:1-4") {
			document.getElementById("luca_prec").disabled = true;
			document.getElementById("luca_prec").style.background = "#939393";
		}
		if (brani[iBrano][2]=="49 24:50-53") {
			document.getElementById("luca_succ").disabled = true;
			document.getElementById("luca_succ").style.background = "#939393";
		}
	}
	else {
		document.getElementById("col_luca").innerHTML = "";	
		pulsanteStile("luca", 1);
	}
	
	if (brani[iBrano][3].length > 0) {
		brano3 = brani[iBrano][3].replace(/50 /g,"Giov");
		brano3 = brano3.replace(/51 /g, "Atti");
		brano3 = brano3.replace(/53 /g, "1Cor");
		loadURL("#col_giovanni", "sinossi_testo.php", { sin_riferimento: brano3, versione: document.getElementById("selectVersione").value });
		pulsanteStile("giovanni", soloVangeli(brani[iBrano][3]).length>0?0:1);
		if (brani[iBrano][3]=="50 1:1-18") {
			document.getElementById("giovanni_prec").disabled = true;
			document.getElementById("giovanni_prec").style.background = "#939393";
		}
		if (brani[iBrano][3]=="50 21:1-25") {
			document.getElementById("giovanni_succ").disabled = true;
			document.getElementById("giovanni_succ").style.background = "#939393";
		}
	}
	else {
		document.getElementById("col_giovanni").innerHTML = "";
		pulsanteStile("giovanni", 1);
	}
	
	branoSelezionato = document.getElementById("selectBrano").value;
	document.getElementById("precedente").disabled = (branoSelezionato==0);
	document.getElementById("precedente").style.background = (branoSelezionato==0?"#939393":"#0000ff");
	document.getElementById("successivo").disabled = (branoSelezionato==brani.length-1);
	document.getElementById("successivo").style.background = (branoSelezionato==brani.length-1?"#939393":"#0000ff");
	setCookie("SinossiBrano", brano = branoSelezionato);
}

function pulsanteStile(libro, impostato) {
	document.getElementById(libro+"_prec").disabled = (impostato==1?true:false);
	document.getElementById(libro+"_succ").disabled = (impostato==1?true:false);
	document.getElementById(libro+"_prec").style.background = (impostato==1?"#939393":"#0000ff");
	document.getElementById(libro+"_succ").style.background = (impostato==1?"#939393":"#0000ff");
}

function soloVangeli(rif) {
	if (rif.substring(0,3)=="51 " || rif.substring(0,3)=="53 ") rif = "";
	if (rif.indexOf("; 53")>0) rif = rif.substring(0,rif.indexOf("; 53"));
	return rif;
}
//-->
</script>

<h1>Sinossi dei Vangeli</h1>
<noscript>
<p><strong>Nota:</strong> Questa pagina del sito richiede Javascript per funzionare. Per usare la sinossi, &egrave; necessario abilitare Javascript. Qui ci sono tutte le <a href="https://www.enable-javascript.com/it/" target="_blank"> istruzioni su come abilitare JavaScript nel tuo browser</a>.</p>
</noscript>
<p class="primalettera">Questa sinossi dei Vangeli elenca e visualizza i brani paralleli nei quattro Vangeli.
Prima scegli la versione della Bibbia che vuoi usare per visualizzare il testo, e poi scegli il racconto dall'elenco oppure digita il riferimento di un versetto in un Vangelo per vedere i brani paralleli. &Egrave; anche possibile passare al brano precedente o successivo nella lista, oppure in un Vangelo, cliccando sui pulsanti.</p>
<p>La divisione dei brani e i titoli sono stati presi dal libro <i>Harmony of the Gospels</i> (1923) di A. T. Robertson.</p>

<script type="text/javascript">
<!--
document.write('<p style="text-align:left"><label>Versione da mostrare:&nbsp;&nbsp;');
var s="<select name=\"versione\" size=\"1\" id=\"selectVersione\" onchange=\"cambiaVersione()\">";
var vers = getCookie("SinossiVers");
//alert(vers);
if (!vers) vers = getCookie("RicVers");
if (!vers || (vers!="C.E.I." && vers!="Nuova Diodati" && vers!="Riveduta 2020" && vers!="Nuova Riveduta 1994" && vers!="Bibbia della Gioia" && vers!="Riveduta" && vers!="Ricciotti" && vers!="Tintori" && vers!="Martini" && vers!="Diodati"))
        s=s+"<option selected=\"selected\" value=\"Nuova Riveduta\">Nuova Riveduta</option>";
else
        s=s+"<option value=\"Nuova Riveduta\">Nuova Riveduta</option>";
if (vers=="C.E.I.")
        s=s+"<option selected=\"selected\" value=\"C.E.I.\">C.E.I. (1974)</option>";
else
        s=s+"<option value=\"C.E.I.\">C.E.I. (1974)</option>";
if (vers=="Nuova Diodati")
        s=s+"<option selected=\"selected\" value=\"Nuova Diodati\">Nuova Diodati</option>";
else
        s=s+"<option value=\"Nuova Diodati\">Nuova Diodati</option>";
if (vers=="Riveduta 2020")
        s=s+"<option selected=\"selected\" value=\"Riveduta 2020\">Riveduta 2020</option>";
else
        s=s+"<option value=\"Riveduta 2020\">Riveduta 2020</option>";
if (vers=="Nuova Riveduta 1994")
        s=s+"<option selected=\"selected\" value=\"Nuova Riveduta 1994\">Nuova Riveduta (1994)</option>";
else
        s=s+"<option value=\"Nuova Riveduta 1994\">Nuova Riveduta (1994)</option>";
if (vers=="Bibbia della Gioia")
        s=s+"<option selected=\"selected\" value=\"Bibbia della Gioia\">La Parola &egrave; Vita</option>";
else
        s=s+"<option value=\"Bibbia della Gioia\">La Parola &egrave; Vita</option>";
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
s=s+"</select>";
document.write(s);
//-->
</script>
</label></p>

<script type="text/javascript">
<!--
	var nBrani = xmlDoc.getElementsByTagName("passage").length;
	var brani = new Array(nBrani);

	for (i = 0; i<nBrani; i++) {
		brani[i] = new Array(4);
		for (j=0; j<4; j++)
			brani[i][j] = "";
		for (j=5; j<xmlDoc.getElementsByTagName("passage")[i].childNodes.length; j+=2) {
			col = xmlDoc.getElementsByTagName("passage")[i].childNodes[j].attributes.getNamedItem("column").value;
			brani[i][col-1] = xmlDoc.getElementsByTagName("passage")[i].childNodes[j].textContent;
		}
        
        document.write("<table><tr>")
	}


    document.write("<p><select name=\"brani\" id=\"selectBrano\" style=\"width:90%\" onChange=\"cambiaBrano()\">");
	if (!b) b = getCookie("SinossiBrano");
	if (!b) b = 0;
	var sel = "";
	for (i = 0; i<nBrani; i++) {
		titolo = xmlDoc.getElementsByTagName("passage")[i].childNodes[3].textContent;
		sel = ((i==b)?"selected=\"selected\" ":"");
		document.write("<option "+sel+"value=\""+i+"\">"+titolo+"</option>");
	}
	document.write("<\select></p><p>");
	document.write("<input class=\"submit\" type=\"submit\" name=\"precedente\" id=\"precedente\" value=\"Precedente\" onclick=\"predSucc(-1,-1)\" />&nbsp;");
    document.write("<input class=\"submit\" type=\"submit\" name=\"successivo\" id=\"successivo\" value=\"Successivo\" onclick=\"predSucc(-1,1)\" /></p>");
//-->
</script>

<form action="#" method="post" id="sin_versetto_form" onsubmit="if (riferimento.value.length==0) {alert('Digitare il riferimento di un versetto nei Vangeli')}; return riferimento.value.length!=0;">
	<p style="text-align:left">
	<label>Versetto da visualizzare:&nbsp;</label>
	<input class="text" name="riferimento" title="Digita qui il riferimento di un versetto nei Vangeli" />
	<input class="submit" type="submit" name="Submit" value="Visualizza versetto" />
	</p>
</form>

<!--<p></p><hr /><p></p>-->

<style>
table#sin {
    border: 1px solid black;
    border-collapse: collapse;
}
table#sin th {
    border: 1px solid black;
    border-collapse: collapse;	
}
table#sin td {
    border: 1px solid black;
    border-collapse: collapse;	
    vertical-align:top;
}
</style>
<table id="sin">
	<tr>
		<th>Matteo</th>
		<th>Marco</th>
		<th>Luca</th>
		<th>Giovanni (ed altri)</th>		
	</tr>
	<tr>
		<td>
		<div id ="col_matteo"> </div>
		</td>
		<td>
		<div id ="col_marco"> </div>
		</td>
		<td>
		<div id ="col_luca"> </div>
		</td>
		<td>
		<div id ="col_giovanni"> </div>
		</td>
	</tr>
	<tr>
		<td style="text-align:center">
			<br />
			<input class="submit" type="submit" name="matteo_prec" id="matteo_prec" value="Precedente" onclick="predSucc(0,-1)" />
			<br /><br />
			<input class="submit" type="submit" name="matteo_succ" id="matteo_succ" value="Successivo" onclick="predSucc(0,1)" />
			<br />
		</td>
		<td style="text-align:center">
			<br />
			<input class="submit" type="submit" name="marco_prec" id="marco_prec" value="Precedente" onclick="predSucc(1,-1)" />
			<br /><br />
			<input class="submit" type="submit" name="marco_succ" id="marco_succ" value="Successivo" onclick="predSucc(1,1)" />
			<br />
		</td>
		<td style="text-align:center">
			<br />
			<input class="submit" type="submit" name="luca_prec" id="luca_prec" value="Precedente" onclick="predSucc(2,-1)" />
			<br /><br />
			<input class="submit" type="submit" name="luca_succ" id="luca_succ" value="Successivo" onclick="predSucc(2,1)" />
			<br />
		</td>
		<td style="text-align:center">
			<br />
			<input class="submit" type="submit" name="giovanni_prec" id="giovanni_prec" value="Precedente" onclick="predSucc(3,-1)" />
			<br /><br />
			<input class="submit" type="submit" name="giovanni_succ" id="giovanni_succ" value="Successivo" onclick="predSucc(3,1)" />
			<br />
		</td>
	</tr>
</table>

<?
require("piede.php");
?>
