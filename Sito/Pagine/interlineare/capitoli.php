<?
$descriz = "Interlineare greco-italiano del Nuovo Testamento, diviso per capitoli";
$key = "interlineare, greco, italiano, nuovo testamento, bibbia";
$titolo = "Interlineare greco-italiano del Nuovo Testamento, diviso per capitoli";
$sezione = "Strumenti";
require("../capo.php");
?>
<h1>Interlineare greco-italiano del Nuovo Testamento</h1>
<p>In questo elenco ci sono tutti i capitoli del Nuovo Testamento. Clicca su un capitolo per aprire l'interlineare di quel capitolo.</p>
<?
$books = [
                "Matteo", "Marco", "Luca", "Giovanni", "Atti", "Romani", "1Corinzi", "2Corinzi",
                "Galati", "Efesini", "Filippesi", "Colossesi", "1Tessalonicesi", "2Tessalonicesi",
                "1Timoteo", "2Timoteo", "Tito", "Filemone", "Ebrei", "Giacomo", "1Pietro", "2Pietro",
                "1Giovanni", "2Giovanni", "3Giovanni", "Giuda", "Apocalisse"
            ];

            $chapters = [
                28, 16, 24, 21, 28, 16, 16, 13, 6, 6,
                4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22
            ];

            foreach ($books as $index => $book) {
                for ($chapter = 1; $chapter <= $chapters[$index]; $chapter++) {
                    echo "<p><a href='index.php?libro=" . ($index + 1) . "&capitolo=$chapter'>$book $chapter</a></p>";
                }
            }
?>
<?
require("../piede.php");
?>
