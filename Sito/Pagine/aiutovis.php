<?
$descriz = "Aiuto per visualizzare un versetto o brano della Bibbia";
$key = "versetto,brano,visualizza,mostra,visualizzare,mostrare";
$titolo = "Aiuto per visualizzare";
$sezione = "Testo della Bibbia";
require("capo.php");
?>
<h1>Come visualizzare il testo della Bibbia</h1>
<p class="primalettera">Prima di tutto, digita il riferimento del brano desiderato. Si pu&ograve; usare il formato Giovanni 3:16-17,19, oppure Giovanni 3,16-17.19. &Egrave; anche possibile includere diversi brani, separandoli con una virgola o punto e virgola; per esempio Giovanni 3:16-17,19; 4:5, Atti 2:14. Non &egrave; necessario digitare l'intero nome del libro, perch&eacute; il sito riconosce quasi tutte le abbreviazioni comunemente usate per i libri. Per un elenco completo, fa' clic <a href="abbrev.php">qui</a>. Spazi e lettere maiuscole sono facoltativi; il sito riconosce quasi qualsiasi tipo di riferimento possibile.</p>
<p>Secondo, scegli quali versioni della Bibbia o quali commentari vuoi visualizzare. Si possono scegliere pi&ugrave; di una versione o commentario. Il modo di farlo dipende dal sistema operativo. Per esempio, in Windows bisogna cliccare sui nomi mentre il tasto CTRL oppure il tasto MAISC &egrave; tenuto.</p>
<p>Quando il pulsante <strong>Visualizza testo</strong> o <strong>Visualizza commento</strong> &egrave; schiacciato, il sito creer&agrave; una pagina con il testo del brano selezionato in tutte le versioni o tutti i commentari desiderati. Se il versetto non esiste in una delle versioni o in un commentario, non sar&agrave; visualizzato.</p>
<p>Nella <a href="/ricerca_avanzata.php">ricerca avanzata</a> &egrave; anche possibile, quando pi&ugrave; di una versione della Bibbia &egrave; scelta, decidere di visualizzare le versioni in colonne paralleli (impostazione predefinita) oppure una sotto l'altra. I commentari sono in ogni caso visualizzati sotto. &Egrave; anche possibile decidere il formato da usare per visualizzare i riferimenti. Se il formato non &egrave; scelto, il sito cerca di decidere il formato da usare in base al formato del riferimento digitato e la versione da visualizzare.</p>
<?
require("piede.php");
?>