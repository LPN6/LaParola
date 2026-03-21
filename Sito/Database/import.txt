DROP DATABASE IF EXISTS laparola_testo;
CREATE DATABASE laparola_testo;
USE laparola_testo;

DROP TABLE IF EXISTS Versetti;
CREATE TABLE Versetti (id_v MEDIUMINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL,id_t SMALLINT UNSIGNED NOT NULL,Libro SMALLINT UNSIGNED NOT NULL,Capitolo SMALLINT UNSIGNED NOT NULL, Versetto SMALLINT UNSIGNED NOT NULL,Testo MEDIUMTEXT);
ALTER TABLE Versetti ADD UNIQUE KEY key_v(id_t,Libro,Capitolo,Versetto);
DELETE FROM Versetti;
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\nr.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\cei.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\nd.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\luzzi.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\dio.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\comment.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\rifinc.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\bg.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\mar.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\r2.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\ricc.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\tint.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentnt.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentpulpito.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentillustratore.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentgill.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\nr94.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commenthenrycompleto.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentbarnes.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentmeyer.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commenttesorodavide.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentcalvino.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\commentginevra.txt" INTO TABLE Versetti FIELDS TERMINATED BY '|' (id_t,Libro,Capitolo,Versetto,Testo);

DROP TABLE IF EXISTS Versioni;
CREATE TABLE Versioni (id_t SMALLINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, Nome TINYTEXT NOT NULL, Libri SMALLINT UNSIGNED NOT NULL DEFAULT 66, Lingua CHAR(2) NOT NULL DEFAULT 'it', Tipo CHAR(1) NOT NULL DEFAULT 'v');
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\versioni.txt" INTO TABLE Versioni FIELDS TERMINATED BY '|' (Nome,Libri,Lingua,Tipo);

DROP TABLE IF EXISTS Letture;
CREATE TABLE Letture (Mese TINYINT UNSIGNED NOT NULL, Giorno TINYINT UNSIGNED NOT NULL, Brano TINYTEXT NOT NULL);
ALTER TABLE Letture ADD UNIQUE KEY key_g(Mese,Giorno);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\letture.txt" INTO TABLE Letture FIELDS TERMINATED BY '|' (Giorno,Mese,Brano);

DROP TABLE IF EXISTS Libri;
CREATE TABLE Libri (Numero TINYINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, Nome TINYTEXT NOT NULL, Abb TINYTEXT NOT NULL);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\libri.txt" INTO TABLE Libri FIELDS TERMINATED BY '|' (Nome,Abb);

DROP TABLE IF EXISTS Abbreviazioni;
CREATE TABLE Abbreviazioni (Numero TINYINT UNSIGNED NOT NULL, AbbR TINYTEXT NOT NULL);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\abbreviazioni.txt" INTO TABLE Abbreviazioni FIELDS TERMINATED BY '|' (Numero,AbbR);

DROP TABLE IF EXISTS Parole;
CREATE TABLE Parole (id_p MEDIUMINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, Parola TINYTEXT NOT NULL, id_r MEDIUMINT UNSIGNED NOT NULL);
ALTER TABLE Parole ADD KEY key_p(Parola(5));
#LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\parole.txt" INTO TABLE Parole FIELDS TERMINATED BY '|' (Parola,id_r);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\paroleutf8.txt" INTO TABLE Parole FIELDS TERMINATED BY '|' (Parola,id_r);

DROP TABLE IF EXISTS Radici;
CREATE TABLE Radici (id_r MEDIUMINT UNSIGNED AUTO_INCREMENT PRIMARY KEY NOT NULL, Radice TINYTEXT NOT NULL, Descrizione TEXT NOT NULL);
ALTER TABLE Radici ADD KEY key_r(Radice(4));
#LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\radici.txt" INTO TABLE Radici FIELDS TERMINATED BY '|' (Radice,Descrizione);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\radiciutf8.txt" INTO TABLE Radici FIELDS TERMINATED BY '|' (Radice,Descrizione);

DROP TABLE IF EXISTS Apparenze;
CREATE TABLE Apparenze (id_p MEDIUMINT UNSIGNED NOT NULL, id_v MEDIUMINT UNSIGNED NOT NULL);
ALTER TABLE Apparenze ADD KEY key_pa(id_p);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\nrc.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\ceic.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\ndc.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\luzzic.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\dioc.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\bgc.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\marc.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\r2c.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\riccc.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\tintc.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
LOAD DATA LOCAL INFILE "c:\\users\\richa\\OneDrive\\siti\\laparola\\mysql\\nr94c.txt" INTO TABLE Apparenze FIELDS TERMINATED BY '|' (id_p,id_v);
