<?php
header("Content-Type: application/json; charset=UTF-8");
include("conn.php");

$obj = json_decode($_GET["q"], false);
include("vistesto.php");
$versioni[] = $obj->versione;
$out = gettesto($obj->riferimento, $versioni);

//$out = str_replace("\\/", "/", $out);
echo json_encode($out);
?>