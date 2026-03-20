// Sincronizzazione scroll

function getElementPosition(el) {
    var lx = 0, ly = 0;
    while (el != null) {
        lx += el.offsetLeft;
        ly += el.offsetTop;
        el = el.offsetParent;
    }
    return {x: lx, y: ly};
}

function setupOnScroll () {
    var scrollTimer = null;
    document.onscroll = function() { 
        clearTimeout(scrollTimer);
        scrollTimer = setTimeout(onStoppedScroll, 500); 
    };
}

var lastAnchor = null;
function onStoppedScroll () {
    var els = document.getElementsByClassName("posizione_versetto");
    
    for (var i = 0; i < els.length; i++) {
        var pos = getElementPosition(els[i]).y;
        pos -= document.body.scrollTop;
    
        var name = els[i].name;
    
        if (pos >= -8) {
            if (name != lastAnchor) {
                lastAnchor = name;
                LaParola.notificaPrimoSegnalibroVisible(lastAnchor);
            }
            return;
        }
    }
}


// Tocco su spazio vuoto

function getWordAtPoint(elem, x, y) {
    if (elem.nodeType == elem.TEXT_NODE) {
        var range = elem.ownerDocument.createRange();
        range.selectNodeContents(elem);
        var currentPos = 0;
        var endPos = range.endOffset;
        while (currentPos+1 < endPos) {
            range.setStart(elem, currentPos);
            range.setEnd(elem, currentPos+1);
            if (range.getBoundingClientRect() != null &&
                range.getBoundingClientRect().left <= x && range.getBoundingClientRect().right  >= x &&
                range.getBoundingClientRect().top  <= y && range.getBoundingClientRect().bottom >= y) {

                //range.expand("word");   // deprecato, tanto non mi serve davvero l'intera parola ma solo sapere se c'è o meno testo sotto il cursore
                var ret = range.toString();
                range.detach();
                return ret;
            }
            currentPos += 1;
        }
    } else {
        for (var i = 0; i < elem.childNodes.length; i++) {
            var range = elem.childNodes[i].ownerDocument.createRange();
            try {
                range.selectNodeContents(elem.childNodes[i]);
            } catch (err) {}
            if (range.getBoundingClientRect() != null &&
                range.getBoundingClientRect().left <= x && range.getBoundingClientRect().right  >= x &&
                range.getBoundingClientRect().top  <= y && range.getBoundingClientRect().bottom >= y) {

                range.detach();
                return getWordAtPoint(elem.childNodes[i], x, y);
            } else {
                range.detach();
            }
        }
    }
    return null;
}

function checkLongTouchOnBackground (normx, normy) {
    var x = normx * window.innerWidth;
    var y = normy * window.innerHeight;

    var D = 5;

    w0 = getWordAtPoint(document, x, y);
    w1 = getWordAtPoint(document, x + D, y);
    w2 = getWordAtPoint(document, x - D, y);
    w3 = getWordAtPoint(document, x, y + D);
    w4 = getWordAtPoint(document, x, y - D);
    
    //LaParola.logd(x);
    //LaParola.logd(y);
    //LaParola.logd("" + w);

    if (w0 == null && w1 == null && w2 == null && w3 == null && w4 == null) {
        LaParola.toccoLungoSuSfondo();
    }
}


// Evidenziatore

var evidenziatoreAttivo = false;

function versettoSpan_onclick (event) {
    if (evidenziatoreAttivo) {
        var el = event.target
        if (el.tagName == "A")
    		return;
        
        while (el != null && !el.hasAttribute("data-versetto"))
            el = el.parentNode;
            
        if (el != null) {
            LaParola.cambiaEvidenziatore(el.getAttribute("data-versetto"));
            return false;
        }
    }
}

function attivaEvidenziatore (attivo) {
    evidenziatoreAttivo = attivo;

    if (attivo) {
        var els = document.querySelectorAll("[data-versetto]");
        for (var i = 0; i < els.length; i++) {
             els[i].onclick = versettoSpan_onclick;
        }
    }
}

function evidenziaVersetto (versetto, colore, modoNotte) {
	var v = document.querySelector("[data-versetto=\"" + versetto + "\"]");
	if (v == null) {
	    return;
	}

    if (!modoNotte) {
        v.style.backgroundColor = colore;
        v.style.color = null;
    } else {
        v.style.backgroundColor = null;
        v.style.color = colore;
    }
}


// Gruppi

function setGroupArrow (element, text) {
    while (element != null && element.tagName != "P") {
        element = element.previousSibling;
    }
    
    if (element == null)
        return;
    
    element = element.firstChild.firstChild;
    
    element.nodeValue = text;
}

function closeGroup (id) {
    var element = document.getElementById(id);
    element.style.display = 'none';
    setGroupArrow(element, "\u25ba ");
}

function openGroup (id) {
    var element = document.getElementById(id);
    
    while (element != null) {
        if (element.tagName == "DIV" && element.className == "gruppo_div") {
            element.style.display = 'block';
            setGroupArrow(element, "\u25bc ");
        }
        
        element = element.parentNode;
    }
}

function getGroupId(p) {
    while (p != null) {
        if (p.className == "gruppo_div")
            return p.getAttribute("id");
        p = p.nextSibling;
    }
    return null;
}

function toggleGroup (e) {
    var id = getGroupId(e.target);
    if (id == null)
        return;
        
    var element = document.getElementById(id);
    if (element.style.display == 'block') {
        closeGroup(id);
    } else {
        openGroup(id);
    }
}

function closeAllGroups () {
    var els = document.getElementsByClassName("gruppo_nome");
    for (var i = 0; i < els.length; i++) {
        var id = getGroupId(els[i]);
        if (id != null)
            closeGroup(id);
    }
}

function prepareGroups () {
    var els = document.getElementsByClassName("gruppo_nome");
    for (var i = 0; i < els.length; i++) {
        var buttonspan = document.createElement('span');
        
        // var img = document.createElement("img");
        // img.setAttribute("src", "errore");
        // buttonspan.appendChild(img);
        
        var text = document.createTextNode("");
        buttonspan.appendChild(text);
        els[i].insertBefore(buttonspan, els[i].firstChild);

        els[i].onclick = toggleGroup;
    }
    closeAllGroups();
    
    var id = window.location.hash.substring(1);
    window.setTimeout(function () {openGroupAndScroll(id);}, 1);
}

function openGroupAndScroll (id) {
    var element = document.getElementById(id);
    if (element != null) {
        openGroup(id);
        while (element != null && element.tagName != "P") {
            element = element.previousSibling;
        }
        if (element != null) {
            element.scrollIntoView();
            //window.scrollBy(0, -100);
        }
    }
}


// Liturgia cattolica

function setSpan (id, html) {
    var s = document.getElementById(id);
    s.innerHTML = html;
}

function setVisible (id, visible) {
    var s = document.getElementById(id);
    if (visible)
        s.style.display = "";
    else
        s.style.display = "none";
}

function ottieniLinkPerLiturgia (anno, mese, giorno) {
    var lit = liturgia_dati[anno][mese][giorno];

    //lit.length = 5;
    if (lit.length != 7)
        return;   // già convertito

    var giornoDellaSettimana = lit[0];
    var descrizione = lit[1];
    var versetto = lit[2];
    
    for (var k = 5; k < 7; k++) {
        if (lit[k] == "giorno aliturgico") {
            lit.push(lit[k]);
            continue;
        }
        
        letture = "";
        var rif = lit[k].split("|");
        for (var i in rif) {
            var t = rif[i].split(" oppure ");
            for (var j in t) {
                if (LaParola.isRiferimento(t[j])) {
                    var rifstd = LaParola.convertiRiferimentoAStandardVirgola(t[j], "C.E.I.2008")
                    letture += "<a href='laparola:" + rifstd + "@*bibbia'>" + LaParola.normalizzaRiferimento(rifstd) + "</a>";
                } else {
                    letture += t[j];
                }
                
                if (j != t.length - 1)
                    letture += " oppure ";
            }
            if (i != rif.length - 1)
                letture += ", ";
        }
        if (letture.indexOf(", ", letture.length - 2) != -1)
            letture = letture.substring(0, letture.length - 2)
            
        lit.push(letture);
    }               
    
    liturgia_dati[anno][mese][giorno] = lit;
}

function convertiLiturgiaAStandard () {
    for (var anno in liturgia_dati) {
        for (var mese in liturgia_dati[anno]) {
            for (var giorno in liturgia_dati[anno][mese]) {
                ottieniLinkPerLiturgia(anno, mese, giorno);
            }
        }
    }

    LaParola.scriviFile("liturgia-standard.json", "liturgia_dati=" + JSON.stringify(liturgia_dati, null, 4));
}

function liturgia (dataint) {
    // attivare per generare il file con i riferimenti standard
    //convertiLiturgiaAStandard();

    var data = new Date(dataint);
    var giorno = data.getDate();
    var mese = "" + (data.getMonth() + 1);
    if (mese.length == 1)
        mese = "0" + mese;
    var anno = "" + data.getFullYear();
    
    ottieniLinkPerLiturgia(anno, mese, giorno);
    
    setSpan("liturgia_data", giorno + "/" + mese + "/" + anno);

    if (!(anno in liturgia_dati) || !(mese in liturgia_dati[anno])) {
        setVisible("liturgia_errore", true);
        setVisible("liturgia_contenuto", false);
    } else {
        setVisible("liturgia_errore", false);
        setVisible("liturgia_contenuto", true);
        
        var lit = liturgia_dati[anno][mese][giorno];
        var giornoDellaSettimana = lit[0];
        var descrizione_r = lit[1];
        var versetto_r = lit[2];
        var descrizione_a = lit[3];
        var versetto_a = lit[4];
        var romana = lit[7];
        var ambrosiana = lit[8];
        
        setSpan("liturgia_giorno_romana", giornoDellaSettimana);
        setSpan("liturgia_descrizione_romana", descrizione_r);
        setSpan("liturgia_versetto_romana", versetto_r);
        setSpan("liturgia_letture_romana", romana);

        setSpan("liturgia_giorno_ambrosiana", giornoDellaSettimana);
        setSpan("liturgia_descrizione_ambrosiana", descrizione_a);
        setSpan("liturgia_versetto_ambrosiana", versetto_a);
        setSpan("liturgia_letture_ambrosiana", ambrosiana);
    }
}


// Inizializzazione pagina

window.onload = function () {
    prepareGroups();
}
