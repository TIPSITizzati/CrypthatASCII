---
layout: post          #important: don't change this
title: "Cifratura AES"
date: 2014-11-25 22:57:26
author: Guglielmetti Kevin
categories:
- blog                #important: leave this here
- Crittografia
img: aes.jpg       #place image (850x450) with this name in /assets/img/blog/
thumb: aes.png     #place thumbnail (70x70) with this name in /assets/img/blog/thumbs/
---
<center><h3> Concetti Generali </h3></center>
<p>AES (advanced encryption standard) è una tecnica di cifratura a blocchi, conosciuta anche come Rijndael.</p>

<!--more-->
<p>
	È veloce, semplice da implementare e offre un buon livello di sicurezza.<br>
	Nell' AES il blocco è di dimensione fissa (<b>128 bit</b>) e la chiave può essere di 128, 192 o 256 bit.<br>
	AES opera utilizzando matrici di 4×4 byte chiamate stati (<b>states</b>). Funzionamento:<br>
	Quando l'algoritmo ha blocchi di 128 bit in input, la matrice State ha 4 righe e 4 colonne; se il numero di blocchi in input diventa di 32 bit più lungo, viene aggiunta una colonna allo State, e così via fino a 256 bit.<br>
	Si divide il numero di bit del blocco in input per 32 e il quoziente specifica il numero di colonne.<br>
	<h4>Operazioni</h4><br>
	<b>AddRoundKey</b><br> 
	Ogni byte della tabella viene combinato con la chiave di sessione, la chiave di sessione viene calcolata dal gestore delle chiavi.<br>
	<br>
	<b>SubBytes</b><br>
	Ogni byte della matrice viene modificato tramite la S-box (tabelle fisse utilizzate per oscurare le relazioni tra testo in chiaro e testo cifrato) a 8 bit. Questa operazione provvede a fornire la non linearità all'algoritmo.<br>
	<img class="img-responsive" src="http://upload.wikimedia.org/wikipedia/commons/a/a4/AES-SubBytes.svg" alt=""><br>
	<b>ShiftRows</b><br>
	Trasla verso sinistra le righe della matrice di un parametro dipendente dal numero di riga. La prima riga resta invariata, la seconda viene spostata di un posto, la terza di due posti e così via. L’ ultima colonna dei dati in ingresso andrà a formare la diagonale della matrice in uscita.<br>
	<img class="img-responsive" src="http://upload.wikimedia.org/wikipedia/commons/6/66/AES-ShiftRows.svg" alt=""><br>
	<b>MixColumns</b><br>
	Prende i quattro byte di ogni colonna e li combina utilizzando una trasformazione lineare invertibile. Ogni colonna è trattata come un polinomio in GF() e viene moltiplicata modulo   per un polinomio fisso.<br>
	<img class="img-responsive" src="http://upload.wikimedia.org/wikipedia/commons/7/76/AES-MixColumns.svg" alt=""><br>
	<b>AddRoundKey</b><br>
	Combina con uno XOR la chiave di sessione con la matrice ottenuta dai passaggi precedenti (State).<br>
	<img class="img-responsive" src="http://upload.wikimedia.org/wikipedia/commons/a/ad/AES-AddRoundKey.svg" alt=""><br>
</p>
<center><h3>Implementazione nel Progetto</h3></center>
<p>
	A causa di ritardi al progetto e della presenza in secondo piano della cifratura AES, utilizzata solamente come ulteriore livello di sicurezza oltre alla cifratura RSA + ASCII Art, il gruppo ha optato per utilizzare la classe AES di sistema.<br>
	Durante la fase di documentazione è inoltre emerso che una implementazione AES in C# avrebbe condotto a tempi di elaborazione eccessivi. In molti casi, infatti, AES è implementata con librerie scritte in linguaggio <b>assembly</b> per diminuire al minimo i tempi di elaborazione.<br>
	<br>
	Nel progetto è stato combinato ad RSA perchè avendo una chiave di lunghezza fissa (128bit) risulta più semplice da codificare rispettando le limitazioni di RSA.<br>
	<br>
	La classe predefinita di C# per l'utilizzo di AES è <b>System.Security.Cryptography.Aes</b> che, data in input le 2 chiavi random generate di lunghezza fissa (di solito 128bit) ed il messaggio in byte, restituisce un array di byte contenente i dati cifrati.<br>
	La classe AES Chiper, utilizza la classe di sistema ma presenta 2 metodi, <b>Encrypt</b> che genera le chiavi e cifra il messaggio in input restituendo un <b>jagged-array</b> contenente le 2 chiavi ed il messaggio cifrato.<br>
	<b>Decrypt</b> invece si affida alla funzione di decifratura standard di AES.<br>
<script src="https://gist.github.com/artumino/640c270af1d530faf39a.js"></script><br>
</p>
