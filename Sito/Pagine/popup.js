function popup(rif) {
// anche nel file per i riferimenti
var vers = "";
var s;
for (var i=1; i<popup.arguments.length; i++) {
  s=popup.arguments[i].replace("/ /g", "+");
  vers = vers + '&versioni[]='+s;
}
finpopup = window.open('https://www.laparola.net/testop.php?riferimento='+rif+vers,'popup','height=400,width=300,resizable=1,scrollbars=1,screenX=0,screenY=0,left=0,top=0,toolbar=0,location=0,directories=0,status=0,menubar=0');
finpopup.focus();
}

function popupr(frase) {
var arg = "";
var s;
for (var i=1; i<popupr.arguments.length; i++) {
  s=popupr.arguments[i].replace("/ /g", "+");
  if (i==1)
    arg += '&versione='+s;
  if (i==2)
    arg += '&brano='+s;
}
finpopup = window.open('https://www.laparola.net/ricercap.php?frase='+frase+arg,'popup','height=400,width=300,resizable=1,scrollbars=1,screenX=0,screenY=0,left=0,top=0,toolbar=0,location=0,directories=0,status=0,menubar=0');
finpopup.focus();
}
