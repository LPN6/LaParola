<?
$descriz = "Come fare ricerche in un programma della Bibbia per Macintosh, Linux, Android";
$key = "ricerca, macintosh, linux, unix, android, java";
$titolo = "Ricerche per Macintosh, Linux, Android";
$sezione = "Programma";
require("../capo.php");
?>
<h1>Ricerche</h1>
<p>L'espressione da ricercare contiene un elenco di parole (composte da lettere, trattini e apostrofi) e alcuni simboli fra le parole che specificano il rapporto per le parole nella ricerca. Le parole possono anche contiene i simboli * e ?: * corrisponde a qualsiasi numero di lettere e ? corrisponde ad una lettere qualsiasi.</p>

<p>Siccome le cifre sono usate per indicare il rapporto fra le parole ricercate, non possono essere ricercate come parole. Per&ograve;, se il numero o la parola che contiene un numero &egrave; messo fra i segni di minore e di maggiore (per esempio, <12000>), &egrave; possibile cercarli.</p>

<p>Spazi (a parte di quelli fra le parole) e lettere maiuscole sono facoltativi.</p>

<h2>Significato dei simboli</h2>
  
<h3>Fra le parole:</h3>
<table border="1">
<tr><td>spazio, 0</td><td>E</td></tr>
<tr><td>0, 1, 2, 3, 4, 5, 6, 7, 8, 9</td><td>E ENTRO: la seconda parola deve apparire entro questo numero di versetti<br />
SALTA (dentro [...]): il numero massimo di parole che pu&ograve; essere saltato in una frase</td></tr>
<tr><td>|, !</td><td>OPPURE</td></tr>
<tr><td>:</td><td>SALTA QUALSIASI NUMERO (dentro [...]): qualsiasi numero di parole pu&ograve; essere fra le due parole</td></tr>
</table>

<h3>Prima di una parola:</h3>
<table border="1">
<tr><td>/</td><td>Qualsiasi parola che ha questa radice</td></tr>
<tr><td>\</td><td>Qualsiasi parola che ha la stessa radice di questa parola</td></tr>
<tr><td>^, ~</td><td>NON questa parola (o parole se seguito da \ o /; deve seguire E o E ENTRO)</td></tr>
</table>

<h3>Intorno ad espressioni:</h3>
<table border="1">
<tr><td>(...)</td><td>Trova prima i versetti che contengono l'espressione dentro le parentesi. Sono necessarie quando l'ordine in cui le parole sono analizzate (da sinistra a destra) non &egrave; l'ordine desiderato</td></tr>
<tr><td>[...], "..."</td><td>Trova i versetti che contengono questa frase, cio&egrave;, queste parole in questo ordine</td></tr>
</table>

<h2>Esempi</h2>
<table border="1">
<tr><td>perdona peccatore</td><td>Un versetto che contiene le parole "perdona" e "peccatore"</td></tr>
<tr><td>perdona | peccatore</td><td>Un versetto che contiene o la parola "perdona" o la parola "peccatore"</td></tr>
<tr><td>/perdonare /peccare</td><td>Un versetto che contiene una parola con radice "perdonare" e una parola con radice "peccare"</td></tr>
<tr><td>\perdona \peccatore</td><td>uguale all'esempio precedente</td></tr>
<tr><td>/pentire (/perdonare|/peccare)</td><td>Un versetto che contiene una parola da "pentire", e una parola da "perdonare" o una da "peccare"</td></tr>
<tr><td>/pentire /perdonare | /peccare</td><td>Un versetto che contiene sia una parola da "pentire" sia una da "perdonare", oppure una parola da "peccare" (non &egrave; uguale all'esempio precedente)</td></tr>
<tr><td>/perdonare 1 /peccare</td><td>Un versetto che contiene una parola da "perdonare", e nello stesso versetto o in quello primo o in quello dopo una parola da "peccare"</td></tr>
<tr><td>/peccare 1 /perdonare</td><td>Un versetto che contiene una parola da "peccare", e nello stesso versetto o in quello primo o in quello dopo una parola da "perdonare" (i versetti saranno diversi da quelli elencati dall'esempio precedente, ma si riferiranno alle stesse paia di versetti)</td></tr>
<tr><td>/perdonare ^ /peccare</td><td>Un versetto che contiene una parola da "perdonare" ma non una da "peccare"</td></tr>
<tr><td>[/perdonare 2 /peccare]</td><td>Un versetto che contiene una parola da "perdonare" e non pi&ugrave; di due parole dopo una parola da "peccare"</td></tr>
<tr><td>[(peccato|peccati):/perdonare]</td><td>Un versetto che contiene la parola "peccato" o la parola "peccati" e pi&ugrave; tardi nello stesso versetto una parola da "perdonare"</td></tr>
<tr><td>/peccare7*pen*</td><td>Un versetto che contiene una parola da "peccare" ed entro sette versetti una parola che contiene le lettere "pen"</td></tr>
</table>
  
<?
require("../piede.php");
?>
