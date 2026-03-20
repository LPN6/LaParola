<?
$v = (isset($_REQUEST["v"])?$_REQUEST["v"]:"");
$descriz = "Lezioni a video per usare il programma della Bibbia";
$key = "lezioni, video";
$titolo = "Lezione";
$sezione = "Programma";
$sezioneurl = "/programma/";
require("../capo.php");
if ($v!="") {
  $v = "https://www.laparola.net/programma/$v.swf";
?>
<center>
<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" width="800" height="586" codebase="http://active.macromedia.com/flash5/cabs/swflash.cab#version=7,0,0,0">
<param name="movie" value="<? echo $v;?>">
<param name="play" value="true">
<param name="loop" value="false">
<param name="wmode" value="transparent">
<param name="quality" value="low">
<embed src="<? echo $v;?>" width="800" height="586" quality="low" loop="false" wmode="transparent" type="application/x-shockwave-flash" pluginspace="http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash">
</embed>
</object>
</center>
<script src="video.js"></script>
<?
}
require("../piede.php");
?>
