<?
$descriz = "Delle mailing list per ricevere automaticamente le letture quotidiane della Bibbia, o per essere informato delle novit&agrave; del sito";
$key = "mailing list,lettura,lettura quotidiana,letture,letture quotidiane";
$titolo = "Mailing list";
$sezione = "Informazioni sul sito";
require("capo.php");
?>
<script type="text/javascript">
function isEmailAddress (string) {
  var addressPattern = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
  return addressPattern.test(string);
}
function checkEmail (field) {
  var giusto = "OK";
  if (!isEmailAddress(field.value)) {
    alert('Indirizzo sbagliato - correggere per favore!');
    field.focus();
    field.select();
    giusto = "sbagliato";
  }
  return (giusto=="OK");
}
</script>

<h1>Le mailing list di LaParola.Net</h1>
<p class="primalettera">Ci sono due mailing list (gruppi di discussione) a cui ci si pu&ograve; iscrivere.</p>
<p>La prima &egrave; solo per dare informazioni sulle novit&agrave; del sito e del programma, e solo l'autore del programma pu&ograve; mandare messaggi.<br /><!--La seconda mailing list &egrave; aperta a tutti, ed &egrave; un modo per interagire con gli altri utenti del sito e del programma su temi biblici.<br />-->La seconda spedisce un messaggio ogni giorno con il testo di quattro brani della Bibbia, gli stessi testi della <a href="letture.php">lettura quotidiana</a>. Seguendo questi testi, si legge tutta la Bibbia in un anno.</p>

<p>Tutte e due le liste sono gestite da Google Groups, per cui ci vuole un account di Google per iscriversi.</p>

<a name="novita"></a>
<h2>Per essere informato delle novit&agrave; del sito e dei programmi e app</h2>

<p>Questa mailing list viene usata solo per informare gli utenti dei programmi e del sito di futuri aggiornamenti. Solo il gestore del sito pu&ograve; mandare messaggi tramite la lista, e lo far&agrave; solo per annunciare le novit&agrave; - circa 3 volte in tutto l'anno.</p>

<p>Per iscriverti al gruppo, vai alla <a href="https://groups.google.com/g/laparolainfo">pagina del gruppo</a> e clicca il pulsante <i>Iscriviti al gruppo</i>.<br />
Alla domanda "Abbonamento", &egrave; meglio scegliere "Ogni nuovo messaggio".<br />
Se non hai un account di Google, o sei disconnesso dal tuo account, bisogna prima cliccare il pulsante <i>Accedi</i>.</p>

<p>Per essere rimosso dalla mailing list, manda un messaggio all'indirizzo laparolainfo+unsubscribe@googlegroups.com.</p>

<p>Alcuni modi alternativi per essere informato delle novit&agrave; sono:</p>
<ul>
<li>utilizzare il <a type="application/rss+xml" href="/feed.xml">feed RSS del sito. <img src="/immagini/feed.gif" width="12" height="12" alt="Feed RSS in formato XML" /></a><br />(Per una spiegazione, vedi la pagina delle <a href="/novita.php">novit&agrave;</a>.)</li>
<li>iscriversi alla <a href="https://www.facebook.com/LaParolaBibbia?ref=bookmarks">pagina Facebook</a> del sito</li>
<!--<li>leggere <a href="http://www.laparola.net/blog/">Bibbia blog</a> - il blog del sito</li>-->
</ul>
<a name="letture"></a>
<h2>Letture quotidiane</h2>
<p>Qui ti puoi iscrivere alla mailing list delle letture quotidiane. Quando iscritto, ogni giorno riceverai un messaggio con quattro brani della Bibbia, in modo di leggere tutta la Bibbia in un anno.
Il tuo indirizzo non sar&agrave; dato a nessun altro, e non sar&agrave; usato da questo sito per nessun altro motivo.</p>

<p>Per iscriverti al gruppo, vai alla <a href="https://groups.google.com/g/lettura-quotidiana2">pagina del gruppo</a> e clicca il pulsante <i>Iscriviti al gruppo</i>.<br />
Alla domanda "Abbonamento", &egrave; meglio scegliere "Ogni nuovo messaggio".<br />
Se non hai un account di Google, o sei disconnesso dal tuo account, bisogna prima cliccare il pulsante <i>Accedi</i>.</p>

<p>Per essere rimosso dalla mailing list, manda un messaggio all'indirizzo lettura-quotidiana2+unsubscribe@googlegroups.com.</p>

<p class="piccolo">I dati da te forniti saranno utilizzati solo per le comunicazioni interne tra il
mittente ed il destinatario e non verranno ceduti ad altri. Avrai in ogni momento il diritto,
ex D.L.vo 196/03, di potere avere notizia dei dati che ti riguardano e che sono in nostro
possesso, di chiederne la correzione, la cancellazione o l'aggiornamento.</p>

<?
require("piede.php");
?>
