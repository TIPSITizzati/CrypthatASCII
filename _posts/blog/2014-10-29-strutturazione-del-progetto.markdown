---
layout: post          #important: don't change this
title: "Strutturazione di Crypthat"
date: 2014-10-29 17:11:34
author: Jacopo Libè
categories:
- blog                #important: leave this here
- Programmazione
- Organizzazione
img: struttura-crypthat.png       #place image (850x450) with this name in /assets/img/blog/
thumb: vs2012.png     #place thumbnail (70x70) with this name in /assets/img/blog/thumbs/
---
<p>
	Data la natura <b>complessa</b> di Crypthat i primi giorni di produzione del progetto sono stati dedicati alla definizione di una struttura precisa del progetto, utile a dividere il lavoro in parti più piccole e "stagne" permettendo così una maggiora collaborazione all'interno del team di sviluppo.
</p><br>

<!--more-->
<p>
	Il concetto di sviluppo per "<i>Punti di Incontro</i>" è stato adottato basandosi sul funzionamento della pila ISO/OSI dove ogni strato comunica con quelli adiacenti tramite dei <b>SAP</b>(<i>Service Access Point</i>) che permettono la libertà totale all'interno di ogni livello di astrazione ma allo stesso tempo garantiscono la funzionalità definendo, nel caso di Crypthat, metodi o eventi di accesso tra diversi livelli.<br>
	<br>
	Crypthat sfrutterà inoltre il modello client/server per le connessioni.<br>
	Si è deciso quindi di creare 3 progetti distinti:<br>
		<ul>
			<li>Crypthat_Server
			<li>Crpyhtat_Client
			<li><b>Crypthat_Common</b>
		</ul><br>
	Così facendo le classi utilizzate solo dal client saranno nel progetto client, viceversa per il server, mentre invece le classi comune saranno localizzate nel progetto Common a cui faranno riferimento sia Client che Server.<br>
</p><br>
<p>La struttura di Crypthat adoperata è visibile nella seguente immagine:</p>
<img class="img-responsive" src="{{ "/assets/img/posts/Struttura-Crypthat.png" | prepend: site.baseurl }}" alt="">
<p>
	Come è possibile notare dalle immagini, il progetto è composto da 4 parti principali:<br>
	<ul>
		<li>L'interfaccia Grafica
		<li>Il gestore logi
		<li>Il gestore di connessione
		<li>Il set di classi per la crittografia
	<ul>
	<br>
	L'insieme di questi classi comunicano tra di loro con alcune strutture dati (<i>DataStructures.cs</i>). La struttura dati più importante è l'<b>Identity</b> che memorizza informazioni su un utente provenienti da tutti i componenti del progetto, quindi dal Nome, alla SessionKey(una chiave che genera il server univoca per ogni client) alle informazioni sulla connessione come il Socket di provenienza o la porta Rs232 attribuita.<br>
</p>
<p>
	Il gestore logico è il cuore del progetto. In questa classe sono inseriti i metodi che mettono in comunicazione l'interfaccia con la parte di connettività, formulando messaggi strutturati che possono essere interpretati dai vari Gestori Logici degli utenti connessi alla chat.<br>
	Il gestore logico, inoltre, si occupa di attribuire un messaggio ad una Identity registrata nella lista degli utenti connessi e di verificarne l'identità.<br>
	Dovendo svolgere lavori diversi tra Client e Server, gestore logico è <b>derivato</b> in <i>GestoreLogicoServer</i> e <i>GestoreLogicoClient</i>.<br>
	<br>
	Nel seguente codice è possibile vedere come un messaggio in arrivo dallo strato di connessione è interpretato dal GestoreLogico.<br>
	<script src="https://gist.github.com/artumino/81005a8cb584c9ce140a.js"></script><br>
</p>
<p>
	Le classi di Cifratura, invece, contengono una serie di algoritmi comuni di cifratura che vengono utlizzati dal GestoreLogico, come RSA, AES e ASCII Art.<br>
	In RSAManager è inoltre presente un servizio che si occupa del rinnovo delle chiavi pubbliche ogni N secondi.<br>
</p>
<p>
	L'interfaccia grafica utilizza gli eventi del GestoreLogicoClient per mostrare all'utente lo stato della chat e per interagire con il GestoreLogico (ad esempio per l'invio di Messaggi).<br>
	Il programma server non presenta alcuna interfaccia dato che deve essere performante e si occupa solamente di smistare i messaggi e gestire gli utenti connessi.<br>
</p>
<p>
	Il livello più basso di Crypthat è composto dalle classi di gestione della connessione che grazie ad un livello astratto (<b>ConnectionManager</b>) permettono al progetto di comunicare con praticamente ogni tipologia di rete. Queste classi, inoltre, si occupano solo del trasferimento dei dati e non verificano identità o semantica dei messaggi.
</p>
