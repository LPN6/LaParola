<?
$descriz = "Aiuto per cercare una parola o frase nella Bibbia";
$key = "ricerca,aiuto";
$titolo = "Aiuto per la ricerca";
$sezione = "Testo della Bibbia";
require("capo.php");
?>
<h1>Come ricercare un'espressione nella Bibbia</h1>
<p>1. Digita l'espressione da ricercare. 
<?include("ricerca_simboli.php");?>
</p>
<p>2. Scegli quale versione della Bibbia vuoi visualizzare - &egrave; possibile scegliere solo una versione.</p>
<p>3. Digita il riferimento del brano in cui vuoi ricercare la frase. Questo &egrave; facoltativo - se non &egrave; digitato nessun riferimento, l'espressione &egrave; cercata in tutta la Bibbia. &Egrave; anche possibile digitare <em>nt</em> per tutto il Nuovo Testamento, o <em>vt</em> per tutto il Vecchio Testamento.<br />Per un riferimento, si pu&ograve; usare il formato Giovanni&nbsp;3:16-17,19, oppure Giovanni&nbsp;3,16-17.19. &Egrave; anche possibile includere diversi brani, separandoli con una virgola o punto e virgola; per esempio Giovanni&nbsp;3:16-17,19;&nbsp;4:5,&nbsp;Atti 2:14. Non &egrave; necessario digitare l'intero nome del libro, perch&eacute; il sito riconosce quasi tutte le abbreviazioni comunemente usate per i libri. Per un elenco completo, fa' clic <a href="abbrev.php">qui</a>. Spazi e lettere maiuscole sono facoltativi; il sito riconosce quasi qualsiasi tipo di riferimento possibile.</p>
<p>4. Di solito il sito visualizza solo i primi 50 versetti che contengono la frase. Dopo aver eseguito la ricerca, dalla pagina che mostra i versetti, &egrave; possibile mostrare i prossimi versetti che contengono la frase, oppure mostrare pi&ugrave; versetti ogni volta, oppure tutti i versetti.</p>
<p>5. Nella <a href="/ricerca_avanzata.php">ricerca avanzata</a> &egrave; possibile cambiare quanti brani sono visualizzati su ogni pagina, oppure mettere 0 per visualizzare tutti. &Egrave; anche possibile decidere il formato da usare per visualizzare i riferimenti. Se il formato non &egrave; scelto, il sito cerca di decidere il formato da usare in base alla versione da visualizzare.</p>
<h2>Esempi di ricerche</h2>
<table border="1">
        <tr>
                <th>Espressione:</th>
                <th>Trova i versetti con:</th>
        </tr>
        <tr>
                <td>perdona</td>
                <td>"perdona"</td>
        </tr>
        <tr>
                <td>perdona peccatore</td>
                <td>"perdona" e "peccatore"</td>
        </tr>
        <tr>
                <td>perdona | peccatore</td>
                <td>"perdona" o "peccatore"</td>
        </tr>
        <tr>
                <td>/perdonare /peccare</td>
                <td>una parola con radice "perdonare" e una parola con radice "peccare"</td>
        </tr>
        <tr>
                <td>\perdona \peccatore</td>
                <td>uguale all'esempio precedente</td>
        </tr>
        <tr>
                <td>/pentire&nbsp;(/perdonare|/peccare)</td>
                <td>"/pentire" e o "/perdonare" o "/peccare"</td>
        </tr>
        <tr>
                <td>/pentire&nbsp;/perdonare&nbsp;|&nbsp;/peccare</td>
                <td>o "/pentire" e "/perdonare", o "/peccare" (non &egrave; uguale all'esempio precedente)</td>
        </tr>
        <tr>
                <td>/perdonare 1 /peccare</td>
                <td>"/perdonare" e nello stesso versetto o in quello primo o in quello dopo "/peccare"</td>
        </tr>
        <tr>
                <td>/peccare 1 /perdonare</td>
                <td>"/peccare" e nello stesso versetto o in quello primo o in quello dopo "/perdonare" (i versetti saranno diversi da quelli elencati nell'esempio precedente, ma si riferiranno alle stesse paia di versetti)</td>
        </tr>
        <tr>
                <td>/perdonare ^ /peccare</td>
                <td>"/perdonare" ma non "/peccare"</td>
        </tr>
        <tr>
                <td>/peccare7#pen</td>
                <td>"/peccare" e entro 7 versetti una parola che contiene "pen" (ce ne sono molti, ma nota soprattutto Rom 2:12 con 2:5)</td>
        </tr>
</table>
<?
require("piede.php");
?>