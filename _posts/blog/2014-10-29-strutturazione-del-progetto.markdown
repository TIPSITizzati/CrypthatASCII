---
layout: post          #important: don't change this
title: "Strutturazione di Crypthat"
date: 2014-10-29 17:11:34
author: Jacopo Libè
categories:
- blog                #important: leave this here
- Programmazione
- Organizzazione
img: rs232.jpg       #place image (850x450) with this name in /assets/img/blog/
thumb: rs232.png     #place thumbnail (70x70) with this name in /assets/img/blog/thumbs/
---
<p>
	Data la natura <b>complessa</b> di Crypthat i primi giorni di produzione del progetto sono stati dedicati alla definizione di una struttura precisa del progetto, utile a dividere il lavoro in parti più piccole e "stagne" permettendo così una maggiora collaborazione all'interno del team di sviluppo.
</p><br>

<!--more-->
<p>
	Il concetto di sviluppo per "<i>Punti di Incontro</i>" è stato adottato basandosi sul funzionamento della pila ISO/OSI dove ogni strato comunica con gli adiacenti tramite dei <b>SAP</b>(<i>Service Access Point</i>) che permettono la libertà totale all'interno di ogni livello di astrazione ma allo stesso tempo garantiscono la funzionalità definendo, nel caso di Crypthat, metodi o eventi di accesso tra diversi livelli.<br>
</p><br>
<p>La struttura di Crypthat adoperata è visibile nella seguente immagine:</p>

<p>
	
</p>
<center><h3> Esempio C# </h3></center>
<p>
	<script src="https://gist.github.com/artumino/30ca0c1bd2f3b45166da.js"></script><br>
	<p>Il codice incluso indica come l'inizializzazione delle porte Rs232 è gestita in Crypthat.</p>
	<p>Il metodo utilizzato è infatti "InizializzaPorta" dove viene specificato a che persona (Identity) attribuire un porta Rs232.</p>
	<p>Come prima cosa si inizializza la proprietà Identity.serialPort ad un nuovo oggetto SerialPort con il nome specificato nei parametri del metodo.</p>
	<p>Seguono poi i settaggi dei parametri della porta, in questo caso sono stati utilizzati:</p>
		<ul>
			<li>Velocità di 9600bps, questo perchè il cavo utilizzato presentava un forte deterioramento e velocità superiori avrebbero probabilmente causato perdita di dati.
			<li>8 bit per codificare i caratteri, per includere anche i caratteri speciali ASCII.
			<li>Controllo di Parità in modalità Pari, per garantire un minimo controllo sugli errori di trasmissione.
			<li>1 StopBit
		</ul>
</p>
<br>
<center><h3> Considerazioni Finali </h3></center>
<p>
	Durante lo sviluppo della classe di gestione di Rs232 abbiamo riscontrato un problema inaspettato che ha ritardato il progetto.<br>
	Crypthat, infatti, utilizza la porta Rs232 ad eventi, ovvero ogni volta che un dato viene ricevuto, viene chiamato l'evento Data_Received della porta che restituisce la porta Rs232 di origine ed i dati letti (presenti nel buffer di ricezione). Nella fase di testing, però, il gruppo ha scoperto che l'evento veniva chiamato alla ricezione di un certo numero di bit, non necessariamente alla fine del messaggio.<br>
	E' stato quindi necessario modificare i metodi di comunicazione, inserendo alla fine di ogni messaggio un carattere di escape (nel nostro caso ~) ed in ricezzione usando il metodo dell'oggetto SerialPort, "ReadTo(char carattere)" che legge il buffer corrente fino ad un determinato carattere e cancella solo i byte letti.<br>
</p>
<script src="https://gist.github.com/artumino/c6d2511c180c4faeb5fc.js"></script><br>
