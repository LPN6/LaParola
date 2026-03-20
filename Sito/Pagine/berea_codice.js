/*
BEREA Atti 17,10-11

Questo programma si chiama 'Berea', perché vuole essere un servizio per quanti
desiderano imitare i credenti di quella città anche nell'era informatica.
La preghiera è che possa essere utile a qualcuno per conoscere la giustificazione
per fede in Gesù Cristo.

Atti 17,10-11:
Essi erano di sentimenti più nobili perché ricevettero la Parola con ogni premura,
esaminando ogni giorno le Scritture per vedere se le cose stavano così.

Il programma riconosce i riferimenti a testi biblici nella pagina HTML e inserisce dei collegamenti 
a www.laparola.net. Per utilizzarlo è sufficiente includere il file berea.js immediatamente prima 
del tag </body>.
Esempio:
<script src="https://www.laparola.net/berea.js" type="text/javascript" charset="utf-8"></script></body>

Licenza: Creative Commons Attribuzione - Non opere derivate 3.0 Italia (CC BY-ND 3.0) 
http://creativecommons.org/licenses/by-nd/3.0/deed.it

http://dean.edwards.name/packer/ è usato per comprimere il codice
*/

function aggiungiTestoAlDiv(testo) {
	LPNDivRif.innerHTML = '<div onmouseout="mout(event);" style="border:1px solid; width:'+LPNlarghezza+'px; height:'+LPNaltezza+'px; background-color:' + LPNcolore + ';"><div style="position:relative; top:1px; left:1px;overflow:auto; margin-right:1px; height:'+(LPNaltezza-30)+'px;">' + testo + '</div><div style="background-color:' + LPNcolore2 + ';height:30px"><a style="font-family: Verdana;font-size: x-small;" href="https://www.laparola.net/" target="_blank" class="LPN">Un servizio di <em>LaParola.Net</em></a></div></div>'; 
	LPNDivRif.style.zIndex = '9999998';
}

function getVersioneDaMostrare() {
	var versioneDaMostrare = "";
	if (LPNversione != "") {
		versioneDaMostrare = '&versioni[]=' + LPNversione;		
	}
	return versioneDaMostrare;	
}

function testoBrano(riferimento)
{
    var nuovoScript = document.createElement('script');
    nuovoScript.setAttribute('type', 'text/javascript');
    nuovoScript.setAttribute('charset', 'utf-8');
    //nuovoScript.setAttribute('src', "/testojsonp.php?cb=scriptCallback&riferimento=" + escape(riferimento) + getVersioneDaMostrare());
    nuovoScript.setAttribute('src', "https://www.laparola.net/testojsonp.php?cb=scriptCallback&riferimento=" + escape(riferimento) + getVersioneDaMostrare());
    document.getElementsByTagName("head")[0].appendChild(nuovoScript);
}

function scriptCallback (data) {
	if (document.getElementsByTagName("head")[0].lastChild) {
		aggiungiTestoAlDiv(data.Testo);
		document.getElementsByTagName("head")[0].removeChild(document.getElementsByTagName("head")[0].lastChild);
	}
}

function svuotaDiv () {
	LPNDivRif.innerHTML = "";
	LPNDivRif.style.zIndex = '-1';
	LPNlink = null;
}

function schermoTop() {
	return typeof window.pageYOffset != 'undefined' ?  window.pageYOffset : document.documentElement && document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop ? document.body.scrollTop : 0;
}

function browserLarghezza() {
  var myWidth = 0;
  if( typeof( window.innerWidth ) == 'number' ) {
    //Non-IE
    myWidth = window.innerWidth;
  } else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
    //IE 6+ in 'standards compliant mode'
    myWidth = document.documentElement.clientWidth;
  } else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {
    //IE 4 compatible
    myWidth = document.body.clientWidth;
  }
  return myWidth;
}

function browserAltezza() {
  var myHeight = 0;
  if( typeof( window.innerHeight ) == 'number' ) {
    myHeight = window.innerHeight;
  } else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
    myHeight = document.documentElement.clientHeight;
  } else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {
    myHeight = document.body.clientHeight;
  }
  return myHeight;
}

function mover(evento, riferimento)
{
	if (!evento) {
		var evento = window.event;
	}

	var targ;
	if (evento.target) {
		targ = evento.target;		
	}
	else {
		if (evento.srcElement) {
			targ = evento.srcElement;			
		}
	}
	if (targ.nodeType == 3) { // defeat Safari bug
		targ = targ.parentNode;
	}
		
	if (LPNlink && LPNlink==targ) {
		return;		
	}
		
	LPNlink = targ;
	LPNmout = false;

    var curtop = 0;
	var curleft = 0;
    if (targ.offsetParent) {
            do {
                  curtop += targ.offsetTop;
				  curleft += targ.offsetLeft;
            } while (targ = targ.offsetParent);
    }
	
    var x = evento.clientX-12;
	if (x < 8) {
		x = evento.clientX;
	}
	var y = curtop-LPNaltezza;
	if (LPNlink.offsetWidth+curleft>document.width && evento.clientX<LPNlink.offsetLeft) { // link su due righe, nella parte a sinistra
		y = y + LPNlink.offsetHeight / 2;
	}

	if (x+LPNlarghezza > browserLarghezza()-20) { // 20 per la barra di scorrimento
		x = browserLarghezza()-20-LPNlarghezza;
	}
	if (y < schermoTop()) {		
		y = curtop + LPNlink.offsetHeight; // "targ" è stato cambiato, quindi usiamo il valore salvato
		if (LPNlink.offsetWidth+curleft>document.width && evento.clientX>=LPNlink.offsetLeft) { // link su due righe, nella parte a destra
			y = y - LPNlink.offsetHeight / 2;
		}
	}

	LPNDivRif.style.top = y + "px";
	LPNDivRif.style.left = x + "px";

	setTimeout(function () {
		if (LPNmout) {
			svoutDiv();
			return;
		}
	   	aggiungiTestoAlDiv("<h3 class=\"LPNBerea\">Caricamento</h3><p>Attendere...</p>");
		if (LPNmout) {
			svoutDiv();
			return;
		}
		testoBrano(riferimento);
		if (LPNmout) {
			svoutDiv();
			return;
  		}
	}, LPNritardo);
}

function mout(e) {
	if (!e) {
		var e = window.event;
	}
	var relTarg = e.relatedTarget || e.toElement;
	var relTargNN = relTarg.nodeName.toLowerCase();
	var relTargCLS = relTarg.className.toLowerCase();
	while (relTarg != LPNDivRif && relTargNN != 'body' && relTargNN != 'a' && relTargNN != 'head' && relTargNN != 'html') {
		relTarg = relTarg.parentNode;
		relTargNN = relTarg.nodeName.toLowerCase();
		relTargCLS = relTarg.className.toLowerCase();
	}
	if (relTarg != LPNDivRif && relTargCLS != 'lpn') {
		LPNmout = true;
		svuotaDiv();
	}
}

function popupRif(riferimento) {
// anche nel file popup.js
	finpopup = window.open('https://www.laparola.net/testop.php?riferimento=' + escape(riferimento) + getVersioneDaMostrare(),'popup','height=400,width=300,resizable=1,scrollbars=1,screenX=0,screenY=0,left=0,top=0,toolbar=0,location=0,directories=0,status=0,menubar=0');
	finpopup.focus();
}

function arrayIndexOf(array, search) {
var indice;
	for (indice in array) {
		if (array[indice] == search) {
			return indice;			
		}
	}	
	return -1;
}

function addEvent(obj, evt, fn) {
    if (obj.addEventListener) {
        obj.addEventListener(evt, fn, false);
    }
    else if (obj.attachEvent) {
        obj.attachEvent("on" + evt, fn);
    }
}

// codice da eseguire all'apertura della pagina

if (typeof(LPNnoscript) == "undefined") {

	addEvent(window,"load",function(e) {
	    addEvent(document, "mouseout", function(e) {
	        e = e ? e : window.event;
	        var from = e.relatedTarget || e.toElement;
	        if (!from || from.nodeName == "HTML") {
				LPNmout = true;
				svuotaDiv();
	        }
	    });
	});
	
	LPNversione = "";
	LPNlarghezza = 400;
	LPNaltezza = 300;
	LPNcolore = "yellow";
	LPNcolore2 = "#ffcc33";
	LPNritardo = 0;
	
	LPNmout = false;
	LPNlink = null;
	var tagDaNonFare = new Array("a", "input", "h1", "h2", "h3", "code");
	var codiceMagico = "LPN~@q";
    var codiceMagicoLunghezza = codiceMagico.length;
	
	var codiceHtml = document.body.innerHTML;
	
	var versetti = "\\s*\\d{1,3}(?:\\s*-\\s*(?:\\d{1,3}\\s*\\1)?\\d{1,3})?(?!:)";
	var brano = "\\s*(?:\\d{1,3}\\s*([:,]))?" + versetti + "(?:\\s*[,\\.]" + versetti + ")*";
	//var libri = "[12] ?(?:S(?:am(?:uele)?)?|R(?:e)?|C(?:r(?:on(?:ache)?)?|o(?:r(?:inzi)?)?)|M(?:a(?:c(?:cabei)?)?)?|T(?:(?:e(?:ss(?:alonicesi)?)?)|i(?:m(?:oteo)?)?)|P(?:i(?:et(?:ro)?)?)?|G(?:(?:io(?:vanni)?)?|v))|3G ?(?:(?:io(?:vanni)?)?|v)|G(?:e(?:n(?:esi)?|r(?:emia)?)?|a(?:l(?:ati)?)?|io(?:s(?:uè)?|v(?:anni)?|b(?:be)?|el(?:e)?|n(?:a)?)|iud(?:a|i(?:c(?:i)?|t(?:ta)?))|ia(?:c(?:omo)?)?|n|s|c|v|b|r|l|m|d)|E(?:s(?:(?:o(?:do)?)?|d(?:ra)?|t(?:er)?)|c(?:cl(?:esiaste)?)?|z(?:ec(?:h(?:iele)?)?)?|f(?:es(?:ini)?)?|b(?:r(?:ei)?)?|o|d|t)|L(?:e(?:v(?:itico)?)?|a(?:m(?:entazioni)?)?|u(?:ca)?|v|c)|N(?:u(?:m(?:eri)?)?|e(?:h)?(?:em(?:ia)?)?|a(?:um|hum)?|m)|D(?:e(?:ut(?:eronomio)?)?|a(?:n(?:iele)?)?|t|n)|R(?:u(?:t(?:h)?)?|o(?:m(?:ani)?)?|t|m)|T(?:o(?:b(?:i(?:a)?)?)?|i(?:t(?:o)?)?|b|t)|S(?:al(?:m[oi])?|ap(?:ienza)?|ir(?:acide)?|o(?:f(?:onia)?)?|l)|Pr(?:ov(?:erbi)?)?|Qo(?:h)?(?:elet)?|C(?:ant(?:ico(?: dei Cantici)?)?|o(?:l(?:ossesi)?)?|C|t|l)|Is(?:a(?:ia)?)?|Bar(?:uc)?|Os(?:ea)?|A(?:m(?:os)?|b(?:d(?:ia)?|a(?:c(?:uc)?)?)|g(?:g(?:eo)?)?|t(?:ti)?|p(?:oc(?:alisse)?)?|d|c)|Hab(?:a[ck]u[ck])?|Z(?:a(?:c(?:c(?:aria)?)?)?|c)|M(?:a(?:r(?:c(?:o)?)?|l(?:achia)?|t(?:t(?:eo)?)?)|i(?:c(?:h(?:ea)?)?)?|l|t|c|r)|F(?:il(?:i(?:ppesi)?|e(?:mone)?)|l|m)";
	var libri = "[12] ?(?:S(?:[Aa][Mm](?:[Uu][Ee][Ll][Ee])?)?|R(?:[Ee])?|C(?:[Rr](?:[Oo][Nn](?:[Aa][Cc][Hh][Ee])?)?|[Oo](?:[Rr](?:[Ii][Nn][Zz][Ii])?)?)|M(?:[Aa](?:[Cc](?:[Cc][Aa][Bb][Ee][Ii])?)?)?|T(?:(?:[Ee](?:[Ss][Ss](?:[Aa][Ll][Oo][Nn][Ii][Cc][Ee][Ss][Ii])?)?)|[Ii](?:[Mm](?:[Oo][Tt][Ee][Oo])?)?)|P(?:[Ii](?:[Ee][Tt](?:[Rr][Oo])?)?)?|G(?:(?:[Ii][Oo](?:[Vv][Aa][Nn][Nn][Ii])?)?|[Vv]))|3G ?(?:(?:[Ii][Oo](?:[Vv][Aa][Nn][Nn][Ii])?)?|[Vv])|G(?:[Ee](?:[Nn](?:[Ee][Ss][Ii])?|[Rr](?:[Ee][Mm][Ii][Aa])?)?|[Aa](?:[Ll](?:[Aa][Tt][Ii])?)?|[Ii][Oo](?:[Ss](?:[Uu]è)?|[Vv](?:[Aa][Nn][Nn][Ii])?|[Bb](?:[Bb][Ee])?|[Ee][Ll](?:[Ee])?|[Nn](?:[Aa])?)|[Ii][Uu][Dd](?:[Aa]|[Ii](?:[Cc](?:[Ii])?|[Tt](?:[Tt][Aa])?))|[Ii][Aa](?:[Cc](?:[Oo][Mm][Oo])?)?|[Nn]|[Ss]|[Cc]|[Vv]|[Bb]|[Rr]|[Ll]|[Mm]|[Dd])|E(?:[Ss](?:(?:[Oo](?:[Dd][Oo])?)?|[Dd](?:[Rr][Aa])?|[Tt](?:[Ee][Rr])?)|[Cc](?:[Cc][Ll](?:[Ee][Ss][Ii][Aa][Ss][Tt][Ee])?)?|[Zz](?:[Ee][Cc](?:[Hh](?:[Ii][Ee][Ll][Ee])?)?)?|[Ff](?:[Ee][Ss](?:[Ii][Nn][Ii])?)?|[Bb](?:[Rr](?:[Ee][Ii])?)?|[Oo]|[Dd]|[Tt])|L(?:[Ee](?:[Vv](?:[Ii][Tt][Ii][Cc][Oo])?)?|[Aa](?:[Mm](?:[Ee][Nn][Tt][Aa][Zz][Ii][Oo][Nn][Ii])?)?|[Uu](?:[Cc][Aa])?|[Vv]|[Cc])|N(?:[Uu](?:[Mm](?:[Ee][Rr][Ii])?)?|[Ee](?:[Hh])?(?:[Ee][Mm](?:[Ii][Aa])?)?|[Aa](?:[Uu][Mm]|[Hh][Uu][Mm])?|[Mm])|D(?:[Ee](?:[Uu][Tt](?:[Ee][Rr][Oo][Nn][Oo][Mm][Ii][Oo])?)?|[Aa](?:[Nn](?:[Ii][Ee][Ll][Ee])?)?|[Tt]|[Nn])|R(?:[Uu](?:[Tt](?:[Hh])?)?|[Oo](?:[Mm](?:[Aa][Nn][Ii])?)?|[Tt]|[Mm])|T(?:[Oo](?:[Bb](?:[Ii](?:[Aa])?)?)?|[Ii](?:[Tt](?:[Oo])?)?|[Bb]|[Tt])|S(?:[Aa][Ll](?:[Mm][OoIi])?|[Aa][Pp](?:[Ii][Ee][Nn][Zz][Aa])?|[Ii][Rr](?:[Aa][Cc][Ii][Dd][Ee])?|[Oo](?:[Ff](?:[Oo][Nn][Ii][Aa])?)?|[Ll])|P[Rr](?:[Oo][Vv](?:[Ee][Rr][Bb][Ii])?)?|Q[Oo](?:[Hh])?(?:[Ee][Ll][Ee][Tt])?|C(?:[Aa][Nn][Tt](?:[Ii][Cc][Oo](?: [Dd][Ee][Ii] C[Aa][Nn][Tt][Ii][Cc][Ii])?)?|[Oo](?:[Ll](?:[Oo][Ss][Ss][Ee][Ss][Ii])?)?|C|[Tt]|[Ll])|I[Ss](?:[Aa](?:[Ii][Aa])?)?|B[Aa][Rr](?:[Uu][Cc])?|O[Ss](?:[Ee][Aa])?|A(?:[Mm](?:[Oo][Ss])?|[Bb](?:[Dd](?:[Ii][Aa])?|[Aa](?:[Cc](?:[Uu][Cc])?)?)|[Gg](?:[Gg](?:[Ee][Oo])?)?|[Tt](?:[Tt][Ii])?|[Pp](?:[Oo][Cc](?:[Aa][Ll][Ii][Ss][Ss][Ee])?)?|[Dd]|[Cc])|H[Aa][Bb](?:[Aa][CcKk][Uu][CcKk])?|Z(?:[Aa](?:[Cc](?:[Cc](?:[Aa][Rr][Ii][Aa])?)?)?|[Cc])|M(?:[Aa](?:[Rr](?:[Cc](?:[Oo])?)?|[Ll](?:[Aa][Cc][Hh][Ii][Aa])?|[Tt](?:[Tt](?:[Ee][Oo])?)?)|[Ii](?:[Cc](?:[Hh](?:[Ee][Aa])?)?)?|[Ll]|[Tt]|[Cc]|[Rr])|F(?:[Ii][Ll](?:[Ii](?:[Pp][Pp][Ee][Ss][Ii])?|[Ee](?:[Mm][Oo][Nn][Ee])?)|[Ll]|[Mm])";
	var reRiferimento = new RegExp("\\b(?:" + libri + ")\\.?" + brano + "(?:\\s*[,;]" + brano + ")*\\b", "g");
	
	var riferimentiTrovati = codiceHtml.match(reRiferimento);
    if (riferimentiTrovati === null) {riferimentiTrovati = [];}
	var conLink, tagDelRiferimento, riferimentoConMagia, i;
    var pos = -1, trovato, p2, p3;
	for (i=0; i<riferimentiTrovati.length; i++) {
		tagDelRiferimento = codiceHtml.match(new RegExp("<([A-Z][A-Z0-9]*)(?:[^<]*?)" + riferimentiTrovati[i] + "(?!(" + codiceMagico + "))(?:[\\s\\S]*?</\\1>|[^<]*?>)", "i"));
		riferimentoConMagia = riferimentiTrovati[i].substring(0,2) + codiceMagico + riferimentiTrovati[i].substring(2) + codiceMagico;
		if (tagDelRiferimento==null || arrayIndexOf(tagDaNonFare, tagDelRiferimento[1].toLowerCase())==-1) {
            trovato = 0;
            // bisogna capire se il riferimento è un attributo di un tag HTML, cioè > prima di < dopo il riferimento
            // non possiamo semplicemente cercare "<...rif...>", perché forse esiste dopo il riferimento numero i che stiamo considerano
            // quindi cerchiamo il riferimento i (cioè la prima volta c'è quel riferimento senza il codice magico)  
            do {
                pos = codiceHtml.indexOf(riferimentiTrovati[i], pos+1);
                if (pos>=0 && codiceHtml.substr(pos+2, codiceMagicoLunghezza)!=codiceMagico)
                    trovato = -1;
            }
            while (pos>=0 && trovato==0);
            p2 = codiceHtml.indexOf(">", pos);
            p3 = codiceHtml.indexOf("<", pos);
            if (p2<0 || p3<p2) {
			 conLink = '<a href=\"javascript:popupRif(\'' + riferimentoConMagia + '\');\" onmouseover=\"javascript:mover(event,\'' + riferimentoConMagia + '\');\" onmouseout=\"javascript:mout(event);\" class=\"LPN\">' + riferimentoConMagia + '</a>';
			 codiceHtml = codiceHtml.replace(new RegExp(riferimentiTrovati[i]+"(?!(" + codiceMagico + "))"), conLink);
            }
            else {
                codiceHtml = codiceHtml.replace(new RegExp(riferimentiTrovati[i]+"(?!(" + codiceMagico + "))"), riferimentoConMagia);
            }
		}
		else {
			codiceHtml = codiceHtml.replace(new RegExp(riferimentiTrovati[i]+"(?!(" + codiceMagico + "))"), riferimentoConMagia);
		}
	}
	
	codiceHtml = codiceHtml.replace(new RegExp(codiceMagico, "g"), "");
	document.body.innerHTML = codiceHtml;
	LPNDivRif = document.createElement('div');
	LPNDivRif.style.position = 'absolute';
	LPNDivRif.id = 'LPNDivRif';
	
	if (document.body.appendChild) {
		document.body.appendChild(LPNDivRif);	
	}
	else {
		if (document.body.innerHTML) {
			document.body.innerHTML += LPNDivRif.innerHTML;
		}
	}
}

var head = document.getElementsByTagName( 'head')[0],
    style = document.createElement('style'),
    rules = document.createTextNode('h3.LPNBerea { margin: 0px 0px 0px 0px;}');

style.type = 'text/css';
if (style.styleSheet) {
    style.styleSheet.cssText = rules.nodeValue;	
}
else {
	style.appendChild(rules);
}
head.appendChild(style);
