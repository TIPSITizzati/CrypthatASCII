---
layout: post          #important: don't change this
title: "L'Interfaccia di Crpyhat"
date: 2014-11-22 19:45:09
author: Cippelletti Alberto
categories:
- blog                #important: leave this here
- Programmazione
- Interfaccia
img: logo.png       #place image (850x450) with this name in /assets/img/blog/
thumb: logo.png     #place thumbnail (70x70) with this name in /assets/img/blog/thumbs/
---
<center><h3> Concetti Generali </h3></center>
<p>L'interfaccia di Crypthat prevede componenti grafiche elaborate che sono legate in alcuni casi ad un Utente e devono agire in base a determinati eventi.</p>

<!--more-->
<p>
	Durante la definizione del progetto si è definito il funzionamento delle parti logiche e di connessione del progetto, ma non si è analizzato in maniera dettagliata l'interfaccia grafica.<br>
	Dopo la conclusione dello sviluppo delle classi Rs232 è stato chiaro però che l'interfaccia doveva presentare i seguenti elementi:<br>
	<ul>
		<li>Schermata iniziale contenente: Modalità operativa, Nome utente, Info sulla connessione
		<li>Lista degli utenti connessi alla Chat</li>
		<li>Chat vera e propria contenente le informazione di un utente</li>
	</ul><br>
	<br>
	Era inoltre necessario che all'arrivo di un messaggio, fosse aperta la chat corrispondente all'utente da cui era arrivato il messaggio.<br>
</p>
<center><h3>Implementazione nel Progetto</h3></center>
<h4>Main From</h4><br>
<center><img class="img-responsive" src="{{ "/assets/img/posts/mainform-interfaccia.png" | prepend: site.baseurl }}" alt=""></center><br>
<p>
	La schermata principale è stata relativamente semplice da implementare ed una volta inseriti i dati di configurazione per la connessione, crea una nuova istanza del Form della lista utenti (<b>UserList</b>) passandogli i parametri inseriti dall'utente.<br>
	La scelta della modalità operativa determina l'attivazione della parte Socket o Rs232 dell'interfaccia.<br>
	Una volta terminata la configurazione l'utente preme Connect e si dovrebbe aprire la lista degli utenti.<br>
	In caso di errore di connessione, l'utente verrà notificato e l'applicazione chiusa.<br>
</p><br>
<br>
<h4>User List</h4><br>
<center><img class="img-responsive" src="{{ "/assets/img/posts/userlist-interfaccia.png" | prepend: site.baseurl }}" alt=""></center><br>
<p>
	La <i>UserList</i> è il Form principale dell'applicazione in quanto gestisce tutti gli eventi provenienti dal gestore logico e deve essere in grado di associare ad ogni persona collegata una Form contenente la chat.<br>
	La realizzazione dell'interfaccia è stata semplice in quanto presenta solo una lista degli utenti connessi (con un contatore nella parte superiore) e, nella status bar, il nome scelto dall'utente con cui gli altri lo vedranno.<br>
	<script src="https://gist.github.com/artumino/1770248ff1cc8144a860.js"></script><br>
	Nel costruttore viene inizializzato il GestoreLogico a cui il Form sarà connesso. Vengono inoltre assegnati i vari eventi da gestire come la registrazione di un utente, l'arrivo di un messaggio o la disconnessione forzata.<br>
	Il form presenta 2 variabili importanti che sono:<br>
	<ul>
		<li><b>chatAttive</b>, un Dictionary che dato un utente ritorna il Form che gli corrisponde (o <i>null</i> se il form non è ancora stato aperto)
		<li><b>gestoreClient</b>, il riferimento alla classe logica del programma (per inviare messaggi o controllare la lista dei connessi)
	</ul><br>
	<br>
	La gestione degli utenti viene effettuata tramite gli eventi della classe Gestore Logico Client.<br>
	<script src="https://gist.github.com/artumino/c02d5b39c97b7d3617de.js"></script><br>
	Alla registrazione di un utente viene semplicemente inserito questo nella lista degli utenti connessi.<br>
	In caso si disconnetta un utente, invece, viene anche controllato se esiste una chat aperta con quell'utente e viene disabilitata.<br>
	<br>
	La ricezione dei messaggi avviene con il comune evento:<br>
	<script src="https://gist.github.com/artumino/98a50ec8ca2de14449f9.js"></script><br>
	<b>OnMessaggioRicevuto</b> controlla se la chat con il mittente è già aperta oppure ne apre una nuova legata al mittente e vi scrive il messaggio ricevuto.<br>
	E' inoltre necessario sapere che il testo ricevuto sarà sempre in chiaro, quindi già decifrato dai livelli sottostanti.<br>
	<br>
	Al doppio click su una qualsiasi persona nella lista degli utenti connessi, verrà aperta una nuova Form legata all'utente selezionato (se non già aperta in precedenza).<br>
	All'apertura di una nuova <b>ChatForm</b> sono passati i riferimenti all'utente selezionato ed al GestoreLogicoClient (utilizzato per inviare i messaggi).<br>
</p><br>
<br>
<h4>Chat Form</h4><br>
<center><img class="img-responsive" src="{{ "/assets/img/posts/chat-interfaccia.png" | prepend: site.baseurl }}" alt=""></center><br>
<p>
	L'interfaccia della chat per ogni utente è semplice.<br>
	Come titolo della chat vi è il nome dell'utente con cui si sta chattando. Sono presenti 2 RichTextBox per l'invio e la ricezione di messaggi.<br>
	A scopo dimostrativo ho incluso diverse possibilità di cifratura dato che lo strato logico permetteva di escluderne alcune.<br>
	Scrivendo testo senza attivare l'opzione "Cifra", il testo verrà trasferito in chiaro ed il messaggio risulterà di colore nero.<br>
	I messaggi con l'opzione <b>Cifra</b> abilitata, invece, risultano verdi e sono cifrati solamente con RSA e AES.<br>
	Premendo il bottone <b>Invia ASCII Art</b>, invece, viene generata l'ASCII Art da inviare e viene cifrata ulteriormente con le altre 2 techniche di cifratura.<br>
</p>
<center><h3>Annotazioni</h3></center>
<h4>Operazione Cross-Thread</h4>
<p>
	Data la natura multi-threaded dei Socket, è possibile che alcuni eventi vengano scatenati da Threads diversi da quello grafico, scatenando così eccezioni per operazione cross thread.<br>
	Un trucchetto comodo per risolvere il problema si è rivelato invocare (tramite <i>Invoke</i>) pezzi di codice da eseguire sul thread grafico.<br>
	Così facendo il codice eseguito apparterrà sempre al thread grafico evitando eccezzioni.<br>
</p><br>
<h4>Testo Colorato</h4>
<p>
	Per la creazione di testi ricchi e colorati nei Form può essere utilizzato il componente RichTextBox, che permette di modificare il colore a parti di testo e di affettuare formattazioni speciali sul testo.<br>
	Nel progetto è stato utilizzato per differenziare i vari tipi di messaggi.<br>
</p>
<h4>Corretta visualizzazione ASCII</h4>
<p>
	Durante la fase di testi abbiamo scoperto che per avere una rappresentazione corretta di un messaggio in ASCII Art occorre utilizzare font detti <b>Monospaced</b> ovvero che mantangono le stesse dimensione per ogni carattere. Arial, ad esempio, cambia la dimensione dei caratteri mentre Courier le mantiene eguali.<br>
</p>
<h4>Errori di Inconsistenza</h4>
<p>
	Durante lo sviluppo abbiamo incontrato molti problemi di "Inconsistenza" con il passaggio di parametri tra Form diverse. Questo problema si è rivelato poi semplice da risolvere.<br>
	Ogni Form infatti è una classe <b>pubblica</b>, passando le nostre classi nei costruttori (<p>private</p> di default) generavamo questi errori di inconsistenza.<br>
</p>
