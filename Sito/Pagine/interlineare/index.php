<?
$descriz = "Interlineare greco-italiano del Nuovo Testamento";
$key = "interlineare, greco, italiano, nuovo testamento, bibbia";
$titolo = "Interlineare greco-italiano del Nuovo Testamento";
$sezione = "Strumenti";
require("../capo.php");
?>
<style>
    sup {
        font-size: 0.7em; /* Smaller, but not affecting the line height */
        vertical-align: 0.3em; /* Forces superscripts to stay on the baseline */
    }
    .container {
        display: flex;
        flex-wrap: wrap;
        gap: 10px;
    }
    .text-block {
        display: flex;
        flex-direction: column;
    }
    .line {
        text-align: center;
        white-space: nowrap;
        line-height: 1.3;
    }
    .line.diff {
        background-color: #FFFF00;
    }
    .line.crittest {
        background-color: #DFFFD6;
    }
    .line.crittest.diff {
    background: repeating-linear-gradient(
        to right, 
        #DFFFD6 0px, 
        #DFFFD6 5px, 
        #FFFF00 5px, 
        #FFFF00 10px
    );
    }
    .versetto {
      outline: 2px solid #000;
      padding: 0px 4px 0px 2px;
      display: inline-block;
      font-weight: bold;
    }
</style>

<h1>Interlineare greco-italiano del Nuovo Testamento</h1>
<!--
<p><b>TODO:</b> 
reverse interlineare; - cosi' controllo contro il testo italiano e anche i numeri superscript

SBL: check Jn 7-8 included (see Morph pull requests)
Morph: check errors in the discussion in the site
check errors in email messages

link to Vocabolario da rifare con nuovo vocabolario; controlla che tutte le parole esistono
righe (or hover popup?) per analisi grammaticale delle parole greche; anche per gloss italiano?;

different versions of the book with different lines (no lemma, translit x 2, grammatical analysis)
</p>
-->
    <p><a href="#" id="toggle-link">Nascondi spiegazioni</a></p>
    <div id="instructions">
    <p><i>Testo greco</i></p>
   <p>Sotto ogni parola greca nel testo del Nuovo Testamento, ci sono una traslitterazione della parola, il lemma della parola in greco e la sua traslitterazione.
   Queste tre righe possono essere nascoste o mostrate:</p>
    <p><label><input type="checkbox" id="cbTraslit" onchange="onCheckboxChange(this)" />Traslitterazione</label><br />
    <label><input type="checkbox" id="cbRad" onchange="onCheckboxChange(this)" />Lemma</label><br />
    <label><input type="checkbox" id="cbRadTraslit" onchange="onCheckboxChange(this)" />Lemma traslitterato</label>
    </p>
    <p>Il testo greco &egrave; l'edizione <a href="https://www.sblgnt.com/">SBL Greek New Testament</a> che &egrave; distribuita con licenza <a href="https://creativecommons.org/licenses/by/4.0/deed.it">Creative Commons Attribuzione 4.0 Internazionale</a>. Le parentesi quadrate [...] indicano che il testo &egrave; dubbio.<br />
    I lemmi delle parole greche sono presi da <a href="https://github.com/morphgnt/sblgnt">MorphGNT SBLGNT</a> con licenza <a href="https://creativecommons.org/licenses/by-sa/3.0/deed.it">Creative Commons Attribuzione - Condividi allo stesso modo 3.0 Unported</a>.<br />
        <p><i>Testi italiani</i></p>
        <p>Sotto le parole greche, ci sono le parole corrispondenti in tre versioni italiane.
        Le righe possono essere nascoste o mostrate, e le differenze fra i testi italiani possono essere evidenziati o non evidenziati:</p>
    <p>
    <label><input type="checkbox" id="cbNR06" onchange="onCheckboxChange(this)" />Nuova Riveduta 2006</label> (<a href="/versioni.php#Nuova%20Riveduta">quinta edizione, 2023</a>)<br />
    <label><input type="checkbox" id="cbNR94" onchange="onCheckboxChange(this)" />Nuova Riveduta 1994</label> (<a href="/versioni.php#Nuova%20Riveduta%201994">decima edizione, 2004</a>)<br />
    <label><input type="checkbox" id="cbR2" onchange="onCheckboxChange(this)" />Riveduta 2020</label> (<a href="/versioni.php#r2">testo del 2024</a>)<br />
    <label>&nbsp;&nbsp;&nbsp;<input type="checkbox" id="cbDiff" onchange="onCheckboxChange(this)" />Evidenzia differenze</label><br />
    <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" id="cbDiffPicc" onchange="onCheckboxChange(this)" />Non evidenziare differenze minori (d'/di; l'/lo; s&eacute;/se; fare/far; vangelo/evangelo; eccetera)</label>
        </p>
        <i>Simboli usati nei testi italiani</i>
        <p>&ndash; : la parola greca non &egrave; tradotta in italiano<br />
        -, + dopo una parola: la parola italiana appare nel versetto precedente o successivo al versetto nel testo italiano relativo al testo greco<br />
        &gt; : la parola greca &egrave; tradotta insieme con la parola successiva per formare il testo italiano<br />
        &lt; : la parola greca &egrave; tradotta insieme con la parola precedente per formare il testo italiano<br />
        &gt;&gt;, &lt;&lt;, eccetera: la parola greca &egrave; tradotta insieme con la parola dopo la successiva o prima della precedente; similmente per pi&ugrave; di due segni<br />
        * : vedi qui sotto<br />
        {...} : la parola italiana &egrave; stata aggiunta, non c'&egrave; una parola greca corrispondente<br />
        [...] : simboli riportati dal testo italiano cartaceo, che racchiudono parole assenti in alcuni manoscritti importanti<br />
        Numero in apice: il numero della parola nell'ordine nel testo italiano
        </p>
    
    <p>A volte le versioni italiane traducono un testo greco diverso da quello riportato qui.
    Quando le parole nella riga del testo greche hanno uno sfondo verde, vuol dire che le parole non sono nell'edizione SBL ma sono in altre edizioni.
    Quando le righe italiane contengono un asterisco * con sfondo verde, vuol dire che le parole greche corrispondenti non sono tradotte, perch&eacute; quel testo italiano segue un'altra edizione a quel punto.
    In tutti e due i casi, c'&egrave; un collegamento che porta ad un elenco dei manoscritti che contengono le diverse letture quando si clicca o si tocca la parola o l'asterisco.<br />
        <label><input type="checkbox" id="cbTC" onchange="onCheckboxChange(this)" />Evidenzia altre edizioni</label>
    </p>
    <p><i>Altri formati</i></p>
    <p>Tutti i dati che sono usati per creare questa pagina possono essere scaricati in <a href="interlineare.sql">questo foglio di calcolo</a>.</p>
    <p>L'interlineare &egrave; anche disponibile per tutte le <a href="/programma/">app gratuite di LaParola</a>.</p> 
    <p>Esiste anche un libro di 480 pagine con le tre righe con le parole greche, i lemmi, e il testo della Riveduta 2020, che pu&ograve; essere pi&ugrave; comodo per alcuni.
    Puoi scaricare un <a href="interlineareR20.pdf">file PDF</a>, oppure acquisire una <a href="https://www.amazon.it/dp/B0GNRZ9F75">copia cartacea</a>.</p>  
    </div>
<p>
<label for="firstList">Scegli un libro:</label>
<select id="firstList" onchange="updateSecondList()" class="selectInterlineare"></select>
<label for="secondList">Scegli un capitolo:</label>
<select id="secondList" onchange="handleSecondListChange()" class="selectInterlineare"></select></p>
<p><button class="button" onclick="changeChapter(-1)">Capitolo precedente</button>
    <button class="button" onclick="changeChapter(1)">Prossimo capitolo</button></p>
<p><a href="capitoli.php">Tutti i capitoli del Nuovo Testamento con link all'interlineare</a><p>
<script>
const books = [
    "Matteo", "Marco", "Luca", "Giovanni", "Atti", "Romani", "1Corinzi", "2Corinzi",
    "Galati", "Efesini", "Filippesi", "Colossesi", "1Tessalonicesi", "2Tessalonicesi",
    "1Timoteo", "2Timoteo", "Tito", "Filemone", "Ebrei", "Giacomo", "1Pietro", "2Pietro",
    "1Giovanni", "2Giovanni", "3Giovanni", "Giuda", "Apocalisse"
];

// Array of the number of chapters in each book
const chapters = [
    28, 16, 24, 21, 28, 16, 16, 13, 6, 6,
    4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22
];

document.addEventListener('DOMContentLoaded', function () {
 var instructions = document.getElementById("instructions");
            var toggleLink = document.getElementById("toggle-link");

// Check the cookie to set the initial state
            var instructionsVisible = getCookie("interlineareInstructionsVisible") || "1";
            if (instructionsVisible === "0") {
                document.getElementById("instructions").style.display = "none";
                toggleLink.innerText = "Mostra spiegazioni";
            } else {
                document.getElementById("instructions").style.display = "block";
                toggleLink.innerText = "Nascondi spiegazioni";
            }
            
            const savedStateTraslit = getCookie('interlinearecbTraslit');
            const checkboxTraslit = document.getElementById('cbTraslit');
            if (savedStateTraslit != "") {
                checkboxTraslit.checked = (savedStateTraslit === 'true');
            } else {
                checkboxTraslit.checked = false; // Default state OFF
            }
            const savedStateRad = getCookie('interlinearecbRad');
            const checkboxRad = document.getElementById('cbRad');
            if (savedStateRad != "") {
                checkboxRad.checked = (savedStateRad === 'true');
            } else {
                checkboxRad.checked = true; // Default state ON
            }
            const savedStateRadTraslit = getCookie('interlinearecbRadTraslit');
            const checkboxRadTraslit = document.getElementById('cbRadTraslit');
            if (savedStateRadTraslit != "") {
                checkboxRadTraslit.checked = (savedStateRadTraslit === 'true');
            } else {
                checkboxRadTraslit.checked = false; // Default state OFF
            }

            const savedStateNR06 = getCookie('interlinearecbNR06');
            const checkboxNR06 = document.getElementById('cbNR06');
            if (savedStateNR06 != "") {
                checkboxNR06.checked = (savedStateNR06 === 'true');
            } else {
                checkboxNR06.checked = true;
            }
            const savedStateNR94 = getCookie('interlinearecbNR94');
            const checkboxNR94 = document.getElementById('cbNR94');
            if (savedStateNR94 != "") {
                checkboxNR94.checked = (savedStateNR94 === 'true');
            } else {
                checkboxNR94.checked = true;
            }
            const savedStateR2 = getCookie('interlinearecbR2');
            const checkboxR2 = document.getElementById('cbR2');
            if (savedStateR2 != "") {
                checkboxR2.checked = (savedStateR2 === 'true');
            } else {
                checkboxR2.checked = true;
            }
            const savedStateDiff = getCookie('interlinearecbDiff');
            const checkboxDiff = document.getElementById('cbDiff');
            if (savedStateDiff != "") {
                checkboxDiff.checked = (savedStateDiff === 'true');
            } else {
                checkboxDiff.checked = true;
            }
            const savedStateDiffPicc = getCookie('interlinearecbDiffPicc');
            const checkboxDiffPicc = document.getElementById('cbDiffPicc');
            if (savedStateDiffPicc != "") {
                checkboxDiffPicc.checked = (savedStateDiffPicc === 'true');
            } else {
                checkboxDiffPicc.checked = false;
            }
            const savedStateTC = getCookie('interlinearecbTC');
            const checkboxTC = document.getElementById('cbTC');
            if (savedStateTC != "") {
                checkboxTC.checked = (savedStateTC === 'true');
            } else {
                checkboxTC.checked = true;
            }
            
            // Attach event listener to toggle link
            toggleLink.addEventListener("click", function(event) {
                event.preventDefault();
                if (document.getElementById("instructions").style.display == "none") {
                //if (instructions.classList.contains("hidden")) {
                //alert(document.getElementById("instructions").style.display);
                    //instructions.classList.remove("hidden");
                    document.getElementById("instructions").style.display = "block";
                    //alert(document.getElementById("instructions").style.display);
                    toggleLink.innerText = "Nascondi spiegazioni";
                    setCookie("interlineareInstructionsVisible", "1", 365);
                } else {
                //alert(document.getElementById("instructions").style.display);
                document.getElementById("instructions").style.display = "none";
                    //instructions.classList.add("hidden");
                    //alert(document.getElementById("instructions").style.display);
                    toggleLink.innerText = "Mostra spiegazioni";
                    setCookie("interlineareInstructionsVisible", "0", 365);
                }
            });
            
            const firstList = document.getElementById('firstList');
            const secondList = document.getElementById('secondList');

            books.forEach((book, index) => {
                const option = document.createElement('option');
                option.value = index+1;
                option.textContent = book;
                firstList.appendChild(option);
            });

            firstList.addEventListener('change', updateSecondList);
            secondList.addEventListener('change', handleSecondListChange);

            loadSelections();
        });

        function onCheckboxChange(checkbox) {
            const isChecked = checkbox.checked;
            checkboxId = checkbox.id;
            setCookie("interlineare" + checkboxId, isChecked, 365);
            handleSecondListChange();
        }

function changeChapter(direction) {
    const firstList = document.getElementById('firstList');
    const secondList = document.getElementById('secondList');
    const currentBookIndex = parseInt(firstList.value) - 1; // Get the current book index (0-based)
    const currentChapter = parseInt(secondList.value);
    const maxChapter = chapters[currentBookIndex];
//    alert(currentBookIndex+"q"+currentChapter+"q"+maxChapter);

    let newBookIndex = currentBookIndex;
    let newChapter = currentChapter + direction;

   // Handle next chapter navigation
    if (newChapter > maxChapter) {
        if (currentBookIndex < books.length - 1) {
            newBookIndex += 1;
            newChapter = 1;
        } else {
            newChapter = maxChapter; // Stay on Apocalisse 22
        }
    }

    // Handle previous chapter navigation
    if (newChapter < 1) {
        if (currentBookIndex > 0) {
            newBookIndex -= 1;
            newChapter = chapters[newBookIndex];
        } else {
            newChapter = 1; // Stay on Matteo 1
        }
    }
//    alert(newBookIndex+"q"+newChapter);

      // Update the select lists and content
    firstList.value = newBookIndex + 1;
    updateSecondList(false);
    secondList.value = newChapter;
    handleSecondListChange();
}

function loadSelections() {
    const selectedBookFromUrl = getUrlParameter("libro");
    const selectedChapterFromUrl = getUrlParameter("capitolo");
    
                const selectedBookFromCookie = getCookie("interlineareLibro") || "1";
            const selectedChapterFromCookie = getCookie("interlineareCapitolo") || "1";

 const selectedBook = selectedBookFromUrl || selectedBookFromCookie;
    const selectedChapter = selectedChapterFromUrl || selectedChapterFromCookie;

            const firstList = document.getElementById('firstList');
            const secondList = document.getElementById('secondList');

            firstList.value = selectedBook;
            updateSecondList();
            secondList.value = selectedChapter;

    generaContenuto(selectedBook, selectedChapter);
        }

        function setCookie(name, value, days) {
            const d = new Date();
            d.setTime(d.getTime() + (days * 24 * 60 * 60 * 1000));
            const expires = "expires=" + d.toUTCString();
            document.cookie = name + "=" + value + ";" + expires + ";path=/";
        }

        function getCookie(name) {
            const nameEQ = name + "=";
            const ca = document.cookie.split(';');
            for (let i = 0; i < ca.length; i++) {
                let c = ca[i];
                while (c.charAt(0) === ' ') c = c.substring(1);
                if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
            }
            return "";
        }

function getUrlParameter(name) {
    name.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
    const regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    const results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

function updateSecondList(callGeneraContenuto = true) {
    const secondList = document.getElementById('secondList');
    const firstList = document.getElementById('firstList');
    const selectedIndex = firstList.options[firstList.selectedIndex].value;
    secondList.innerHTML = '';

    setCookie("interlineareLibro", selectedIndex, 365);

    // Get the number of chapters for the selected book
    const numChapters = chapters[selectedIndex-1];

      for (let i = 1; i <= numChapters; i++) {
          const option = document.createElement('option');
          option.value = i;
          option.textContent = i;
          secondList.appendChild(option);
      }
            secondList.selectedIndex = 0;
        if (callGeneraContenuto)
           handleSecondListChange();
}

function handleSecondListChange() {
  const firstList = document.getElementById('firstList');
  const secondList = document.getElementById('secondList');
  const selectedBookIndex = firstList.selectedIndex+1;
  const selectedChapter = secondList.value;

    setCookie("interlineareCapitolo", selectedChapter, 365);

    generaContenuto(selectedBookIndex, selectedChapter);
}

function generaContenuto(nLibro, nCapitolo) {
    var righe = (document.getElementById('cbNR06').checked?1:0) + (document.getElementById('cbNR94').checked?2:0) + (document.getElementById('cbR2').checked?4:0) + (document.getElementById('cbRad').checked?8:0) + (document.getElementById('cbTraslit').checked?16:0) + (document.getElementById('cbRadTraslit').checked?32:0);
    var opzioni = (document.getElementById('cbDiff').checked?1:0) + (document.getElementById('cbTC').checked?2:0) + (document.getElementById('cbDiffPicc').checked?4:0);    
//    alert(righe);
    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'contenuto.php', true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
//alert(xhr.responseText);
            document.getElementById('container').innerHTML = xhr.responseText;
        }
    };
    xhr.send('libro=' + nLibro + '&capitolo=' + nCapitolo + '&righe=' + righe + '&opzioni=' + opzioni);

}

firstList.addEventListener('change', updateSecondList);
const secondList = document.getElementById('secondList');
secondList.addEventListener('change', handleSecondListChange);
</script>

<div class="container" id="container"></div>

<?
require("../piede.php");
?>
