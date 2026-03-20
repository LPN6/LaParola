<?
if (isset($_REQUEST["nome"])) {
  $nome = $_REQUEST["nome"];
  $nome = str_replace("<", "", $nome); // affinché tag HTML non possono essere inseriti nella pagina
  $nome = str_replace(">", "", $nome);
}
else
  header("Location: http://".$_SERVER['HTTP_HOST']."/nomi/");

$iniziale=0;
if (isset($_REQUEST["i"]))
  $iniziale=1;
?>
<?
$descriz = $nome." nella Bibbia";
$key = $nome.",dizionario";
$titolo = $nome;
$sezione = "Dizionario biblico";
$sezioneurl = "/nomi/";
include("../conn.php");
require("../capo.php");

        $problema = 0;
          $nomeSQL=utf8_decode(urldecode($nome));
        if ($iniziale==1) {
          $Tipo = "Radice";
          $sql = "SELECT Radice,Descrizione FROM Radici WHERE (Radice LIKE \"".$nomeSQL."%\") AND NOT (Descrizione = \"\") ORDER BY Radice";
          if ($ris = mysqli_query($conn, "$sql")) {
                  //$rad="radsi";
          }
          else {
                  $problema = 1;
            }
        }
        else {
        $Tipo = "Nome";
        $sql = "SELECT Descrizione,Radice FROM Radici WHERE Radice=\"$nomeSQL\" AND NOT (Descrizione = \"\") ORDER BY Radice";
        if ($ris = mysqli_query($conn, "$sql")) {
                if (mysqli_num_rows($ris)==0) {
                        $sql = "SELECT Parola,Descrizione, Radice FROM Parole, Radici WHERE Parole.id_r = Radici.id_r AND Parola = BINARY \"$nomeSQL\" AND NOT (Descrizione = \"\") ORDER BY Parola";
                        if ($ris = mysqli_query($conn, "$sql")) {
                                if (mysqli_num_rows($ris)==0) {
                                        $Tipo = "Parola";
                                        $sql = "SELECT Radice,Descrizione FROM Radici WHERE (Radice LIKE \"$nomeSQL%\") AND NOT (Descrizione = \"\") ORDER BY Radice";
                                        if ($ris = mysqli_query($conn, "$sql")) {
                                                if (mysqli_num_rows($ris)==0) {
                                                        $sql = "SELECT Parola,Descrizione,Radice FROM Parole, Radici WHERE Parole.id_r = Radici.id_r AND (Parola LIKE \"$nomeSQL%\") AND NOT (Descrizione = \"\") ORDER BY Parola";
                                                        if ($ris = mysqli_query($conn, "$sql")) {
                                                             $Tipo = "Parola";
                                                        }
                                                        else
                                                                $problema = 1;
                                                }
                                                else {
                                                        $Tipo = "Radice";
                                                }
                                        }
                                        else
                                                $problema = 1;
                                }
                        }
                        else
                                $problema = 1;
                }
        }
        else
                $problema = 1;
        }
        if ($problema==0) {
                if (mysqli_num_rows($ris)==0) {
                        echo "<h1>$nome</h1>";
                        echo "<p>Nessun nome simile a o che inizia con '$nome' trovato. Prova di nuovo...</p>";
                }
                if (mysqli_num_rows($ris)==1) {
                        $row=mysqli_fetch_array ($ris);
                        $radstring = "";
                        if (strtolower($nome)!=strtolower($row["Radice"]))
                            $radstring = " (".$row["Radice"].")";
                        if ($Tipo=="Nome") {
                            if (strtolower($nome)==strtolower($row["Radice"]) && $nome!=$row["Radice"])
                                echo "<h1>".$row["Radice"].$radstring."</h1>";
                            else
                                echo "<h1>".$nome.$radstring."</h1>";
                        }
                        elseif ($Tipo=="Parola") {
                            if (strtolower($nome)==strtolower($row["Parola"]) && $nome!=$row["Parola"])
                                echo "<h1>".$nome.$radstring."</h1>";
                            else
                                echo "<h1>".$row["Parola"].$radstring."</h1>";
                        }
                        else
                                echo "<h1>".$row["Radice"]."</h1>";
                        echo "<p>".StripSlashes($row["Descrizione"])."</p>";
                }
                if (mysqli_num_rows($ris)>1) {
                        echo "<h1>$nome</h1>";
                        echo "<p>I seguenti nomi iniziano con '$nome'. Scegli quello desiderato:</p><p>";
                        while ($row=mysqli_fetch_array ($ris)) {
                            $nometrovato = $nome;
                            if ($Tipo=="Radice")
                                $nometrovato = $row["Radice"];
                            if ($Tipo=="Parola")
                                $nometrovato = $row["Parola"];
                            $nometrovato = utf8_encode($nometrovato);
                            echo "<a href=\"/nomi/nomi.php?nome=".urlencode($nometrovato)."\">".$nometrovato."</a><br />";
                        }
                }
        }
        else
                errore2("interrogazione database per nomi");
require("indnomi.php");
require("../piede.php");
?>
