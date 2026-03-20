<?
function TradGram($g) {
global $lin;
$g2 = "";
switch (substr($g,0,2)) {
case "A-":
   $g2=($lin=="it"?"aggettivo":"adjective:");
   break;
case "C-":
   $g2=($lin=="it"?"congiunzione":"conjunction");
   break;
case "D-":
   $g2=($lin=="it"?"avverbio":"adverb:");
   break;
case "I-":
   $g2=($lin=="it"?"interiezione":"interjection");
   break;
case "N-":
   $g2=($lin=="it"?"sostantivo":"noun:");
   break;
case "P-":
   $g2=($lin=="it"?"preposizione":"preposition:");
   break;
case "RA":
   $g2=($lin=="it"?"articolo":"article:");
   break;
case "RD":
   $g2=($lin=="it"?"pronome dimostrativo":"demonstrative pronoun:");
   break;
case "RI":
   $g2=($lin=="it"?"pronome interrogativo/indefinito":"interrogative/indefinite pronoun:");
   break;
case "RP":
   $g2=($lin=="it"?"pronome personale/possessivo":"personal/possessive pronoun:");
   break;
case "RR":
   $g2=($lin=="it"?"pronome relativo":"relative pronoun:");
   break;
case "V-":
   $g2=($lin=="it"?"verbo":"verb:");
   break;
case "X-":
   $g2=($lin=="it"?"particella":"particle");
   break;
}
switch (substr($g,2,1)) {
case "1":
	$g2 .= ($lin=="it"?" 1a persona":" 1st person");
	break;
case "2":
	$g2 .= ($lin=="it"?" 2a persona":" 2nd person");
	break;
case "3":
	$g2 .= ($lin=="it"?" 3a persona":" 3rd person");
	break;
}
switch (substr($g,3,1)) {
case "A":
	$g2 .= ($lin=="it"?" aoristo":" aorist");
	break;
case "F":
	$g2 .= ($lin=="it"?" futuro":" future");
	break;
case "I":
	$g2 .= ($lin=="it"?" imperfetto":" imperfect");
	break;
case "P":
	$g2 .= ($lin=="it"?" presente":" present");
	break;
case "X":
	$g2 .= ($lin=="it"?" perfetto":" perfect");
	break;
case "Y":
	$g2 .= ($lin=="it"?" piuccheperfetto":" pluperfect");
	break;
}
switch (substr($g,4,1)) {
case "A":
	$g2 .= ($lin=="it"?" attivo":" active");
	break;
case "M":
	$g2 .= ($lin=="it"?" medio":" middle");
	break;
case "P":
	$g2 .= ($lin=="it"?" passivo":" passive");
	break;
}
switch (substr($g,5,1)) {
case "D":
	$g2 .= ($lin=="it"?" imperativo":" imperative");
	break;
case "I":
	$g2 .= ($lin=="it"?" indicativo":" indicative");
	break;
case "N":
	$g2 .= ($lin=="it"?" infinito":" infinitive");
	break;
case "O":
	$g2 .= ($lin=="it"?" ottativo":" optative");
	break;
case "P":
	$g2 .= ($lin=="it"?" participio":" participle");
	break;
case "S":
	$g2 .= ($lin=="it"?" congiuntivo":" subjunctive");
	break;
}
switch (substr($g,6,1)) {
case "A":
	$g2 .= ($lin=="it"?" accusativo":" accusative");
	break;
case "D":
	$g2 .= ($lin=="it"?" dativo":" dative");
	break;
case "G":
	$g2 .= ($lin=="it"?" genitivo":" genitive");
	break;
case "N":
	$g2 .= ($lin=="it"?" nominativo":" nominative");
	break;
case "V":
	$g2 .= ($lin=="it"?" vocativo":" vocative");
	break;
}
switch (substr($g,7,1)) {
case "P":
	$g2 .= ($lin=="it"?" plurale":" plural");
	break;
case "S":
	$g2 .= ($lin=="it"?" singolare":" singular");
	break;
}
switch (substr($g,8,1)) {
case "F":
	$g2 .= ($lin=="it"?" femminile":" feminine");
	break;
case "M":
	$g2 .= ($lin=="it"?" maschile":" masculine");
	break;
case "N":
	$g2 .= ($lin=="it"?" neutro":" neuter");
	break;
}
switch (substr($g,9,1)) {
case "C":
	$g2 .= ($lin=="it"?" comparativo":" comparative");
	break;
case "S":
	$g2 .= ($lin=="it"?" superlativo":" superlative");
	break;
}

return $g2;
}

function ConvPersona($g) {
switch ($g[2]) {
case "1":
	$g[2]="F";
	break;
case "2":
	$g[2]="S";
	break;	
case "3":
	$g[2]="T";
	break;	
case "F":
	$g[2]="1";
	break;
case "S":
	$g[2]="2";
	break;	
case "T":
	$g[2]="3";
	break;	
}
return $g;
}
?>