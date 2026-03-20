<?
$v = (isset($_REQUEST["v"])?$_REQUEST["v"]:"");
?>
<html lang="en">
<head>
<title>LaParola</title>
<meta name="description" content="Video tutorials for a public domain program to study the Bible" />
<meta name="keywords" content="video, tutorial,bible,the bible,holy bible,the holy bible,bible on line,bible online,bible on-line,laparola,Italian bible,italian,italy,gospel,psalm,gospels,psalms,jesus,christ,jesus christ,christ jesus,new testament,old testament,program,programme,christianity,religion,free,spirituality,catholic,christian" />
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="robots" content="index,follow" />
<meta http-equiv="content-language" content="en" />

<link rel="SHORTCUT ICON" href="/favicon.ico" />
<link rel="stylesheet" href="/stili/stilebase.css" type="text/css" />
<link rel="stylesheet" href="/stili/stampa.css" type="text/css" media="print" />
</head>
<body>
<table width="100%"><tr align="center" valign="middle"><td width="32">
<a href="/program/" title="Bible"><img src="/immagini/bibbia.gif" width="32" height="32" alt="Bible" border="0" /></a>
</td><td>
<img src="/immagini/la_parola.gif" height="125" width="290" alt="The Word on Internet" border="0" vspace="1" />
</td></tr></table>
<?
if ($v!="") {
  $v = "http://richardwilson.interfree.it/versione7/$v.swf";
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
?>
</body>
</html>
