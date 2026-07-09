<?php
ini_set('max_execution_time', 0);
ini_set('memory_limit', '1024M');

$pdo = new PDO("mysql:host=62.149.150.88;dbname=Sql237475_1", "Sql237475", "Ab95e3b76!");
$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

//$files = ["parolenoline.csv",];
//    $stmt = $pdo->prepare("INSERT INTO ParoleNew (Parola,id_r) VALUES (?,?)");
//$colonne = 2;
//$files = ["radicinoline.csv",];
//    $stmt = $pdo->prepare("INSERT INTO RadiciNew (Radice,Descrizione) VALUES (?,?)");
 //$colonne = 2;

/*
$files = [
    "nrnoline.csv",
    "ceinoline.csv",
    "ndnoline.csv",
    "luzzinoline.csv",
    "dionoline.csv",
    "commentnoline.csv",
    "rifincnoline.csv",
    "bgnoline.csv",
    "marnoline.csv",
    "r2noline.csv",
    "riccnoline.csv",
    "tintnoline.csv",
    "commentntnoline.csv",
    "commentpulpitonoline.csv",
    "commentillustratorenoline.csv",
    "commentgillnoline.csv",
    "nr94noline.csv",
    "commenthenrycompletonoline.csv",
    "commentbarnesnoline.csv",
    "commentmeyernoline.csv", 
    "commenttesorodavidenoline.csv",
    "commentcalvinonoline.csv",
    "commentginevranoline.csv",
    "volgnoline.csv",
];
    $stmt = $pdo->prepare("INSERT INTO VersettiNew (id_t,Libro,Capitolo,Versetto,Testo) VALUES (?,?,?,?,?)");
    $colonne = 5;
  */  

// nota: Apparenze funziona solo se si aggiunge una versione alla volta,
// probabilmente perché c'è un timeout con le tante righe se se ne aggiungono due
$files = [
/*    "nrcnoline.csv", // 623K righe
    "ceicnoline.csv", // 1307K
    "ndcnoline.csv", // 1940
    "luzzicnoline.csv", // 2578
    "diocnoline.csv", // 3231895
    "bgcnoline.csv", // 3401837
    "marcnoline.csv", // 4128314
    "r2cnoline.csv", // 4757954
    "ricccnoline.csv", // 5467494
    "tintcnoline.csv", // 6166176
    "nr94cnoline.csv", // 6789902
*/    "volgcnoline.csv", // 7532736
/**/
];
    $stmt = $pdo->prepare("INSERT INTO ApparenzeNew (id_p,id_v) VALUES (?,?)");
    $colonne = 2;

//    $stmt = $pdo->prepare("INSERT INTO InterlineareNew (Libro,Capitolo,Versetto,Parola,Greco, Radice,NR06, NR94,R2) VALUES (?,?,?,?,?,?,?,?,?)");
// $colonne = 9;    

$count = 0;

foreach ($files as $filename) {
    echo "Processing $filename ...<br>";

    if (($file = fopen($filename, "r")) !== false) {
         while (($line = fgets($file)) !== false) {
                 $line = trim($line);
                if ($line === '') continue; // skip blank lines
                $row = explode('|', $line);
            
            if (count($row) !== $colonne) {
                echo "Invalid column count in $filename: $line (colonne=$colonne)<br>";
                continue;
            }

            try {
                $stmt->execute($row);
            } catch (Exception $e) {
                echo "<br><b>ERROR IN FILE:</b> $filename";
                echo "<br><b>LINE:</b> $line";
                echo "<br><b>PARSED FIELDS:</b> ".count($row);
                echo "<br><b>ERROR:</b> ".$e->getMessage();
                die(); // stop so you can read the error
            }
            $count++;

            if ($count % 1000 == 0) {
                echo "$count rows inserted<br>";
                flush();
            }
        }
        fclose($file);
    } else {
        echo "Could not open $filename<br>";
    }
}
echo "Done!";
?>
