---
layout: post          #important: don't change this
title: "Jekyll ed il Markdown"
date: 2014-12-10 17:23:12
author: Guglielmetti Kevin
categories:
- blog                #important: leave this here
- sito
img: logo.png       #place image (850x450) with this name in /assets/img/blog/
thumb: logo.png     #place thumbnail (70x70) with this name in /assets/img/blog/thumbs/
---
<center><h3> Concetti Generali </h3></center>
<p>Jekyll è un programma per linux che permette la compilazione di siti statici, dinamicamente.</p>

<!--more-->
<p>
	Giunti alla fine del progetto il gruppo necessitava di un sito su cui caricare la documentazione. Avendo la possibilità da parte di GitHub di hostare un sito statico (quindi solo HTML, CSS o Javascript) abbiamo deciso di usare le Git Pages.<br>
	Per creare il blog, però, è stato utilizzato <b>Jekyll</b>, un programma linux ottenibile tramite <i>apt-get</i> che utilizzando un linguaggio apposito chiamato <b>markdown</b> unisce pagine, layout, post, variabili, per creare un intero sito dalla parvenza dinamica.<br>
	Jekyll è integrato nativamente su GitHub ed e possibile il suo utilizzo in locale con il comando "<b>jekyll serve</b>" nella directory dei file markdown.<br>
	In combinazione con Jekyll è stato utilizzato un tema, in seguito modificato, chiamato <a href="https://github.com/st4ple/solid-jekyll">Solid</a>, che integra il supporto a dispositivi mobile e diverse librerie CSS per icone e impaginamento dei contenuti.<br>
</p>
<center><h3>Implementazione</h3></center>
<p>
	Per la creazione di un blog post, si deve creare un file .markdown nella cartella /_posts/ di Jekyll.<br>
	Il nome del file deve rispettare la seguente sintassi "<i>yyyy-mm-dd-titolo-del-post.markdown</i>".<br>
	Il contenuto del file deve essere strutturato in questo modo:<br>
	<script src="https://gist.github.com/artumino/b30ca2aa4a7a06508f5b.js"></script><br>
	<br>	
	Una volta avviato "<i>jekyll serve</i>" vengono costruite le pagine inserendo headers, footers, stili, javascripts.<br>
	Guardando sulla documentazione presente sul sito di Jekyll è possibile scoprire di più anche sulla sintassi dei cicli, etc.<br>
	<br>
	Jekyll da quindi la possibilità di creare un sito modulare, senza l'uso di php ed è inoltre molto semplice da sincronizzare tra più persone in quanto ogni parte del sito risulta essere divisa in piccoli sottomoduli che diminuiscono le possibilità di conflitto con gli altri utenti.<br>
</p>
<center><h3>Vantaggi</h3></center>
<p>
	Il lato positivo di Jekyll è che non impegna risorse come un Database combinato a php5 e Joomla o Wordpress. Così facendo si ha un sito sicuro (non avendo componenti dinamici) e con basso consumo di risorse. Esportabile su qualsiasi servizio di hosting.<br>
</p>
