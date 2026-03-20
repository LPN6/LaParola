<html>
<body>
<?

include("../conn.php");
global $conn;

//esegui("SELECT * FROM Versetti WHERE id_t=1 AND Libro=1 AND Capitolo=1");

esegui("DROP TABLE IF EXISTS Brani");
esegui("CREATE TABLE Brani (id_b MEDIUMINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, Libro1 SMALLINT UNSIGNED NOT NULL,Capitolo1 SMALLINT UNSIGNED NOT NULL, Versetto1 SMALLINT UNSIGNED NOT NULL,Libro2 SMALLINT UNSIGNED NOT NULL,Capitolo2 SMALLINT UNSIGNED NOT NULL, Versetto2 SMALLINT UNSIGNED NOT NULL,Domanda TEXT, Risposta TEXT)");
esegui("LOAD DATA LOCAL INFILE \"brani.sql\" INTO TABLE Brani FIELDS TERMINATED BY '|' (Libro1,Capitolo1,Versetto1,Libro2,Capitolo2,Versetto2,Domanda,Risposta)");

//esegui("DROP TABLE IF EXISTS Quiz");
//esegui("CREATE TABLE Quiz (id_d MEDIUMINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, id_t MEDIUMINT UNSIGNED NOT NULL, domanda VARCHAR(255) NOT NULL, risposta1 VARCHAR(255) NOT NULL, risposta2 VARCHAR(255) NOT NULL, risposta3 VARCHAR(255) NOT NULL, risposta4 VARCHAR(255) NOT NULL, risposta TINYINT UNSIGNED NOT NULL, spiegazione VARCHAR(5000) NOT NULL, giuste INT UNSIGNED NOT NULL DEFAULT 0, sbagliate INT UNSIGNED NOT NULL DEFAULT 0)");

//esegui("DROP TABLE IF EXISTS QuizTemi");
//esegui("CREATE TABLE QuizTemi (id_t MEDIUMINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, tema VARCHAR(255) NOT NULL)");

//esegui("LOAD DATA LOCAL INFILE \"quiz.sql\" INTO TABLE Quiz FIELDS TERMINATED BY ';' OPTIONALLY ENCLOSED BY '\"' (id_t,domanda,risposta1,risposta2,risposta3,risposta4,risposta,spiegazione)");

//esegui("LOAD DATA LOCAL INFILE \"quiztemi.sql\" INTO TABLE QuizTemi (tema)");


echo "fatto";

function esegui($sql) {
global $conn;
echo "<p>$sql</p>";
      if ($ris=mysqli_query($conn, "$sql")) {
    //    echo "Result: ".$ris."<p>";
      }
      else {
        errore2("errore database $sql");
      }
}

?>

<p>done</p>
</body>
</html>
