USE laparola_testo;

DROP TABLE IF EXISTS Vocabolario;
CREATE TABLE Vocabolario (id_p MEDIUMINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, Parola TINYTEXT NOT NULL, Traslit TINYTEXT NOT NULL, Definizione TEXT NOT NULL);
ALTER TABLE Vocabolario ADD KEY key_p(Parola(4));
ALTER TABLE Vocabolario ADD KEY key_t(Traslit(4));
LOAD DATA LOCAL INFILE "c:\\users\\richa\\onedrive\\siti\\laparola\\mysql\\vocab.txt" INTO TABLE Vocabolario FIELDS TERMINATED BY '|' (Parola,Traslit,Definizione);
