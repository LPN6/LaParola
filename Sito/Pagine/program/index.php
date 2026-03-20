<html lang="en">
<head>
<title>LaParola</title>
<meta name="description" content="A public domain program to study the Bible" />
<meta name="keywords" content="bible,the bible,holy bible,the holy bible,bible on line,bible online,bible on-line,laparola,Italian bible,italian,italy,gospel,psalm,gospels,psalms,jesus,christ,jesus christ,christ jesus,new testament,old testament,program,programme,christianity,religion,free,spirituality,catholic,christian" />
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="robots" content="index,follow" />
<meta http-equiv="content-language" content="en" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />

<link rel="SHORTCUT ICON" href="/favicon.ico" />
<link rel="stylesheet" href="/stili/stilebase.css" type="text/css" />
<link rel="stylesheet" href="/stili/stampa.css" type="text/css" media="print" />
</head>

<body>

<table width="100%"><tr align="center" valign="middle"><td width="32">
<a href="/english.html" title="Bible"><img src="/immagini/bibbia.gif" width="32" height="32" alt="Bible" border="0" /></a>
</td><td>
<img src="/immagini/la_parola.gif" height="125" width="290" alt="The Word on Internet" border="0" vspace="1" />
</td></tr></table>

<p>(<a href="/programma/">Questa pagina in italiano</a>)</p>

<h1>LaParola 7.20.6: A Free Computer Program to Study the Bible</h1>

<h2>Why another Bible study program?</h2>
<p class="primalettera">There are already many free Bible study programs available. <i>LaParola</i> has a few characteristics that few or no others have, that may make it worth trying.
<ul>
<li>Not only is it free, but it is <a href="#source">open source</a> and the file format is open (see the help file), making it easier for others to help develop it.</li>
<li>There are four different Greek texts of the New Testament, as well as a listing of the manuscript support for all the major variants (useful for textual criticism).</li>
<li>It automatically updates itself and its components, and finds new components that can be installed.</li>
<li>It can import ThML, OSIS, Zefania, BibleWorks and e-Sword files.<!-- That means for example that there are over 800 files at the <a href="http://www.ccel.org/">Christian Classics Ethereal Library</a> that can be imported into the program.--></li>
<li>It can perform searches in the Bible not only for words, but also for all forms of the same word. So for example you can search for &quot;love&quot; if you want to, but also for all the forms of that word: love, loved, loves, loving, etcetera. Not just in English, but also in other languages.</li>
<li>Links to external sites give extra information on Bible verses or words, for example extra Bible translations, readings in manuscripts like Aleppo, Sinaiticus and Bezae, sermons, definitions and Internet searches of words.</i>
<li>You can create personal bookmarks, external links, parallel passages and daily reading schemes, and distribute them to others.</li>
<li>You can create personal notes on verses and themes, which are automatically fully searchable just like the components distributed along with the program.</li>
<li>The program can create a concordance of any passage of any Bible or collection of notes.</li>
<li>The program can look up all the installed resources (Bibles, notes, pictures, parallel passages, external links, ...) to find all the information in the program on a certain passage or word.</li>
<li>The same program and data files work in both Windows and Linux.</li>   
</ul> 
</p>

<h2>Installation</h2>
<p class="primalettera">
The program requires Windows 7 or later, or Linux.</p>
<h3>Windows</h3>
<p>
You can download the program <a href="/file/laparola-en.exe">from this site</a>.
It is an installation program; run it and follow the instructions to install <i>LaParola</i>.
You can then run the Bible program.
</p>

<p>Windows has a system called <i>User Account Control</i> (UAC),
that means that certain activities require an explicit authorization even when executed
by an administrator of the computer. This means that the program requires permission to perform
an update, as it substitutes and runs other files. For this reason the program has been created
to be run with administrator rights, and thus when it is run when UAC is active
(as it is by default in Windows Vista, although UAC can be turned off in the <i>Users</i> section
of the Control Panel) a window will pop up asking if you agree to the running of the program.
When you agree, the program will run without further problems.
</p>

<h3>Linux</h3>
<p>For Linux, the latest version of Mono is required, which runs .NET application in Linux.
Mono might already by installed on your copy of Linux, but it may not be a recent enough version. At least the version 1.2.5 is required, that was released in August 2007; however the program works a lot better with the version 2.0 of Mono.
Mono for many different distributions can be downloaded from <a href="http://www.go-mono.com/mono-downloads/download.html">Mono's site</a>;
if your distribution is not listed then click on the link &quot;unsupported downloads&quot;.</p>

<p>You can then install the program in one of two ways:</p>
<ul>
<li>If you have already installed the Windows version, you can copy the program and data files to Linux.
You have to copy the contents of the directory in which you installed the the program (usually c:\program files\LaParola\) to any directory of Linux.
Then you need to copy the contents of the data directory (usually  c:\users\&lt;user&nbsp;name&gt;\AppData\Roaming\LaParola\ in Windows Vista; c:\Documents&nbsp;and&nbsp;Settings\&lt;user&nbsp;name&gt;\Application&nbsp;Data\LaParola\ in Windows XP)
to the directory /home/&lt;user&nbsp;nome&gt;/.config/LaParola/ of Linux.</li>
<li>Otherwise, you can <a href="/file/LaParola.tar.gz">download the version for Linux</a>. The files are exactly the same as those for Windows, but in a tar.gz archive rather than an installation program.
You can then use the command <i>tar xzvf LaParola.tar.gz</i> or a graphical program to unpack the file into any directory.</li>
</ul>
<p>In both cases, to open the program you need to use the command <i>mono /path/to/program/LaParola.exe</i></p>
<p>Note however that Mono is not yet a complete substitute for NET and has some errors, and so the program in Mono is not yet perfect. I am still improving this version of the program.</p>

<p>There are three errors that might appear, that are caused by problems in the installation of Mono in some distributions of Linux:</p>
<ol>
<li>Not all the necessary components to run the program and installed. To correct this problem, install the program <em>MonoDevelop</em>, using the usual method for adding programs to Linux.</li>
<li>An error message about codepage 1252. To correct this problem, install the file libmono-i18n2.0-cil using the usual method for downloading additional components to your version of Linx.</li>
<li>An error messagge <code>no implementation for interface method Atk.TableImplementor</code> when the program is run. To correct this problem, run the command <code>sudo apt-get remove .uia.</code> in a terminal window.</li>
</ol>

<h3>Post installation</h3>
<p>The first time that the program is run, it checks the availability of updates and other components on Internet.
In fact the downloaded file contains only one Bible translation (the <i>American Standard Version</i>).
The program, if you are connected to Internet, will list many others; you just have to select which ones you want and they will be downloaded and installed automatically.
You can also look for other components later doing a manual check for updates using the <i>Update</i> command of the <i>Tools</i> menu.
If you use a proxy to connect to Internet, you will have to first enter the details of the proxy in the <i>Options</i>.
The program also checks regularly if there are updates to the program itself or the components, so you will never have to download it again. The frequency of the checks can be set in the <i>Options</i> of the program. 
</p>
<p>Alternatively, you can download all the extra files that you want at the page with the <a href="addins.php">list of all the available extra components</a>. Each file that is downloaded needs to be unzipped, and its contents placed in the directory that the program was installed in.</p>
<p>It is also suggested to read the <i>How Do I...</i> section of the Help file to get an idea of the main tasks that you can do with the program.
</p>
<!--<p>Another way to learn about the program is to watch the <a href="video.php">video tutorials</a>. The same videos can be watched inside the program (if they are downloaded with the <i>Update</i> command) from the <i>Help</i> menu.</p>-->

<p>Click on the image to enlarge this example of the use of the program.<br />
<a href="/immagini/example7.jpg"><img src="/immagini/example7a.jpg" width="431" height="389" alt="An example of the use of this Bible program" title="An example of the use of this Bible program"></a>
</p>

<a name="source"></a>
<h3>Source code</h3>
<p>
For programmers, not only is the source code available <a href="/file/laparolacode.zip">here</a>, but also the development environment is free, namely <a href="https://visualstudio.microsoft.com/vs/">Visual C# Express</a>.
If you are asked for a password to open the file, it is <em>bibbia</em>.
</p>

<h2>This site</h2>

<p class="primalettera">
Most of the rest of this site contains resources to study the Bible in Italian.
If you are interested in what else is available, see <a href="/english.html">this page</a>.
There is however one bilingual section which might interested you, that on the <a href="/greco/">Greek New Testament</a>.
</p>

<p>For information about the site or the program, write to info at this domain.</p>

</body>
</html>
