    <table style="border:1px solid #0000ff;margin-left: auto; margin-right: auto; background-color:#68ffff" cellspacing="0">
    <tr><td style="background-color:#0000ff"><p class="contrario" style="text-align:center">Trova nome:</p></td></tr>
        <tr><td>
<form action="nomi.php" method="post" onsubmit="if (nome.value.length==0) {alert('Digitare un nome da ricercare')}; return nome.value.length!=0;">
<p><label>Nome: <input class="text" type="text" name="nome" size="10" style="background:#ffffff" />
<input class="submit" type="submit" name="Submit" value="Trova" /></label></p></form>
</td></tr>
        <tr><td>
<?
    for ($i=1; $i<=26; $i++)
        echo "<a href=\"/nomi/nomi.php?i=1&nome=".chr($i+96)."\" style=\"display: inline-block;min-width: 24px;min-height: 24px;\">".chr($i+64)."</a> ";
?>
        </td></tr>
    </table>
<p></p>
