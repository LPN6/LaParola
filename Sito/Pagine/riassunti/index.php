<?
include("../conn.php");
include("../vistesto.php");

$descriz = "Riassunti dei capitoli della Bibbia";
$key = "bibbia, riassunti, intelligenza artificiale";
$titolo = "Riassunti dei capitoli della Bibbia";
$sezione = "Strumenti";
require("../capo.php");
?>
<h1>Riassunti dei capitoli della Bibbia</h1>
<p class="primalettera">Per ogni capitolo della Bibbia, ci sono un riassunto del contenuto del capitolo, alcuni spunti per metterlo in pratica, e alcune domande per la riflessione.</p> 
<p>Nota che tutto il contenuto di questa parte del sito &egrave; stato generato da un'intelligenza artificiale.
Naturalmente, l'intelligenza artificiale non &egrave; una persona, e non pu&ograve; sperimentare spiritualmente quello che scrive n&eacute; essere guidata dallo Spirito Santo.
Tuttavia, i riassunti potrebbero essere utili ad alcuni.</p>

<p>Scegli il libro e poi il capitolo di cui vuoi un riassunto:</p>
<label for="firstList">Scegli un libro:</label>
<select id="firstList" onchange="updateSecondList()"></select>
<label for="secondList">Scegli un capitolo:</label>
<select id="secondList" onchange="handleSecondListChange()"></select>

<script>
const books = [ "",
    "Genesi", "Esodo", "Levitico", "Numeri", "Deuteronomio", "Giosuè", "Giudici", "Rut",
    "1Samuele", "2Samuele", "1Re", "2Re", "1Cronache", "2Cronache", "Esdra", "Neemia",
    "Tobia","Giuditta",
    "Ester",
    "1Maccabei", "2Maccabei",
    "Giobbe", "Salmi", "Proverbi", "Ecclesiaste", "Cantico dei Cantici",
    "Sapienza", "Siracide",
    "Isaia",
    "Geremia", "Lamentazioni",
    "Baruc",
    "Ezechiele", "Daniele", "Osea", "Gioele", "Amos", "Abdia",
    "Giona", "Michea", "Naum", "Abacuc", "Sofonia", "Aggeo", "Zaccaria", "Malachia",
    "Matteo", "Marco", "Luca", "Giovanni", "Atti", "Romani", "1Corinzi", "2Corinzi",
    "Galati", "Efesini", "Filippesi", "Colossesi", "1Tessalonicesi", "2Tessalonicesi",
    "1Timoteo", "2Timoteo", "Tito", "Filemone", "Ebrei", "Giacomo", "1Pietro", "2Pietro",
    "1Giovanni", "2Giovanni", "3Giovanni", "Giuda", "Apocalisse"
];

// Array of the number of chapters in each book
const chapters = [ 0,
    50, 40, 27, 36, 34, 24, 21, 4,
    31, 24, 22, 25, 29, 36, 10, 13,
    14, 16,
    10,
    16, 15,
    42, 150, 31, 12, 8,
    19, 51,
    66, 52, 5,
    6,
    48, 12, 14, 3, 9, 1, 4, 7, 3, 3, 3, 2, 14, 4, 28, 16, 24, 21, 28, 16, 16, 13, 6, 6,
    4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22
];

// Populate the first list with book names
const firstList = document.getElementById('firstList');
books.forEach((book, index) => {
    const option = document.createElement('option');
    option.value = index; // Use the index as the value
    option.textContent = book;
    firstList.appendChild(option);
});

function updateSecondList() {
    const secondList = document.getElementById('secondList');
    const firstList = document.getElementById('firstList');
    const selectedIndex = firstList.options[firstList.selectedIndex].value;
    secondList.innerHTML = '';

    // Get the number of chapters for the selected book
    const numChapters = chapters[selectedIndex];

    if (numChapters == 1) {
        const url = `riassunto.php?libro=${selectedIndex}&capitolo=1`;
        window.location.href = url;
    }
    else {
      // Populate the second list with chapter numbers
      for (let i = 0; i <= numChapters; i++) {
          const option = document.createElement('option');
          option.value = i;
          option.textContent = (i==0 ? "" : i);
          secondList.appendChild(option);
      }
    }
}

function handleSecondListChange() {
  const firstList = document.getElementById('firstList');
  const secondList = document.getElementById('secondList');
  const selectedBookIndex = firstList.selectedIndex;
  const selectedChapter = secondList.value;
  const url = `riassunto.php?libro=${selectedBookIndex}&capitolo=${selectedChapter}`;
  window.location.href = url;
}

firstList.addEventListener('change', updateSecondList);
const secondList = document.getElementById('secondList');
secondList.addEventListener('change', handleSecondListChange);
</script>

<?
require("../piede.php");
?>
