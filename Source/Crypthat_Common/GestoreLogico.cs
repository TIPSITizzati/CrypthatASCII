using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;

using Crypthat_Common;
using Crypthat_Common.Connessioni;
using Crypthat_Common.Crittografia;

namespace Crypthat_Common
{
    /* Livello di astrazione appena sotto l'interfaccia grafica che si occupa di creare un collegamento
     * logico tra i vari Utenti della rete.
     */
    public class GestoreLogico
    {
        public Identity Me; // Riferimento a se stesso
        public List<Identity> Destinatari { get; set; } // Lista dei destinatari memorizzati
        protected ModalitaOperativa opMode; // Modalità in cui il programma funzionerà
        protected ConnectionInterface ConnectionManager; // Gestisce gli eventi comuni di tutte le interfacce di connessione (per ora Rs232 e Sockets)
        protected RSACipher.RSACryptoService RSACryptoService;

        //Costruttore di default che inizializza il GestoreLogico con un Identità ignota (in attesa di un'Identità dal server)
        public GestoreLogico(ModalitaOperativa opMode)
        {
            this.opMode = opMode;
            this.Destinatari = new List<Identity>(10);
            this.Me = new Identity(null, null);
            this.RSACryptoService = new RSACipher.RSACryptoService();

            // Genera le chiavi RSA necessarie
            this.Me.RSAContainer = RSACipher.GenerateKeyPair(512);
            RSACryptoService.Start(30);

            switch (opMode)
            {
                //Inizializza la parte Rs232
                case ModalitaOperativa.Rs232:
                    Debug.Log("Inizializzazione manager RS232...");
                    ConnectionManager = new Rs232Manager();                              // Inizializza RS232Manager
                break;
                //Inizializza la parte Socket
                case ModalitaOperativa.Sockets:
                    Debug.Log("Inizializzazione manager Sockets...");
                    ConnectionManager = new SocketManager();                            // Inizializza il manager dei Socket
                break;
            }

            RSACryptoService.NewKeyPair += AggiornaChiaviAsimmetriche;           // Imposta l'evento di rigenerazione delle chiavi RSA
            ConnectionManager.OnMessaggioRicevuto += InterpretaTipoMessaggio;    // Imposta l'evento OnMessaggioRicevuto in modo che chiami il metodo InterpretaTipoMessaggio

            Debug.Log("Inizializzazione GestoreLogico...");
        }

        // Metodo che in modalità Rs232 inizializza tutte le porta Rs232 disponibili
        public void Inizializza()
        {
            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    //Inizializza tutte lo porte
                    foreach (string Name in SerialPort.GetPortNames())
                        Inizializza(Name);
                break;

                case ModalitaOperativa.Sockets:
                    ((SocketManager)ConnectionManager).Ascolta();
                break;
            }
        }

        // Metodo che inizializza una porta specifica Rs232
        public void Inizializza(string NomePorta)
        {
            Identity Ignoto = new Identity(null, null); // Non sapendo a chi si è connessi, viene creata un Identità ignota
            ((Rs232Manager)ConnectionManager).InizializzaPorta(Ignoto, NomePorta); // Viene aperta la connessione sulla porta richiesta

            Destinatari.Add(Ignoto);
        }

        // Metodo che si collega ad un EndPoint definito
        public void Inizializza(System.Net.IPEndPoint serverEndpoint)
        {
            Identity Ignoto = new Identity(null, null); // Non sapendo a chi si è connessi, viene creata un Identità ignota
            ((SocketManager)ConnectionManager).Connetti(Ignoto, serverEndpoint); // Viene aperta la connessione all'EndPoint richiesto
            ((SocketManager)ConnectionManager).RiceviMessaggio(Ignoto); // Inizia ad ascoltare il server

            Destinatari.Add(Ignoto);
        }

        // Invia i messaggi al livello sottostante in base ad opMode
        // I dati NON vengono criptati di default
        public void InviaMessaggio(string Messaggio, Identity Destinatario, bool Encrypted = false)
        {
            if(!Encrypted)
                ConnectionManager.InviaMessaggio(String.Format("MSG:{0}?{1};{2}", Me.SessionKey, Destinatario.SessionKey, Messaggio), Destinatario);
            else
            {
                byte[][] AES_DATA = AESCipher.Encrypt(Messaggio);
                string AES_KEY_RSA = Convert.ToBase64String(RSACipher.EncryptDecrypt(AES_DATA[0], Destinatario.RSAContainer.PublicKey));
                string AES_IV_RSA = Convert.ToBase64String(RSACipher.EncryptDecrypt(AES_DATA[1], Destinatario.RSAContainer.PublicKey));
                string AES_Messaggio = Convert.ToBase64String(AES_DATA[2]).Replace("<", "<\\");
                string MessaggioCifrato = String.Format("CRYPT:{0}?{1};{2}<KEY>{3}<IV>{4}", Me.SessionKey, Destinatario.SessionKey, AES_Messaggio, AES_KEY_RSA, AES_IV_RSA);

                ConnectionManager.InviaMessaggio(MessaggioCifrato, Destinatario);
            }
        }

        /*
         * Ogni Messaggio è compsto nella seguente maniera =>  {HEADER}:Messaggio
         * I tipi di HEADER sono:
         *  "MSG"   - Indica che l'header è seguito da un messaggio in chiaro
         *  "CRYPT" - Indica che l'header è seguito da una struttura dati criptata
         *  "KEY"   - Indica che l'header è seguito da una chiave
         *  "HALOHA"- Indica che l'header è seguito dai dati di un utente
         */
        protected void InterpretaTipoMessaggio(object sender, InterLevelArgs args)
        {
            string msg = args.Data.ToString();
            string Header = msg.Split(':')[0];
            string Data = msg.Substring(msg.IndexOf(':') + 1);

            // Variabili comuni
            string SessionKeys, SK_Mittente, SK_Destinatario;
            Identity Mittente, Destinatario;

            // Switch per i vari header
            switch (Header)
            {
                case "MSG":
                    SessionKeys = Data.Split(';')[0];
                    string Messaggio = Data.Remove(0, Data.IndexOf(';') + 1);

                    // Divide le due SessionKeys
                    SK_Mittente = SessionKeys.Split('?')[0];
                    SK_Destinatario = SessionKeys.Split('?')[1];

                    // Ottiene i dati di mittente e destinatario tramite il metodo TrovaPerSessionKey
                    Mittente = TrovaPerSessionKey(SK_Mittente);
                    Destinatario = TrovaPerSessionKey(SK_Destinatario);

                    //Se il messaggio ricevuto appartiene a questo client o deve essere smistato
                    if(Destinatario != null || SK_Destinatario == Me.SessionKey)
                        ElaboraMessaggio(Mittente, Destinatario, Messaggio);
                    else
                        Debug.Log("Ricevuto messaggio con SessionKey non combaciante, rifiuto del messaggio.");
                break;
                case "CRYPT":
                    // Messaggio cifrato in arrivo
                    // Sintassi: CRYPT:<SK_Mittente>?<SK_Destinatario>;<Messaggio_Cifrato_AES><Key><Chiave_Simmetrica_Cifrata_RSA><IV><Chiave_IV_Cifrata_RSA>
                    // Sintassi <Messaggio_Cifrato_AES>: <ASCII_Encyption_Attiva>;<Dati>
                    SessionKeys = Data.Split(';')[0];
                    string Dati = Data.Remove(0, Data.IndexOf(';') + 1);

                    // Divide le due SessionKeys
                    SK_Mittente = SessionKeys.Split('?')[0];
                    SK_Destinatario = SessionKeys.Split('?')[1];

                    // Ottiene i dati di mittente e destinatario tramite il metodo TrovaPerSessionKey
                    Mittente = TrovaPerSessionKey(SK_Mittente);
                    Destinatario = TrovaPerSessionKey(SK_Destinatario);

                    // Controlla che il mittente sia quello reale
                    switch (opMode)
                    {
                        case ModalitaOperativa.Rs232:
                            if (Mittente.serialPort != (System.IO.Ports.SerialPort)sender)
                            {
                                Debug.Log(((System.IO.Ports.SerialPort)sender).PortName + " sta cercando di inviare una messaggio cifrato sotto falsa identità!", Debug.LogType.WARNING);
                                return;
                            }
                            break;
                        case ModalitaOperativa.Sockets:
                            if (Mittente.Sock != ((SocketManager.StateObject)sender).Sock)
                            {
                                Debug.Log(((SocketManager.StateObject)sender).Sock.RemoteEndPoint.ToString() + " sta cercando di inviare una messaggio cifrato sotto falsa identità!", Debug.LogType.WARNING);
                                return;
                            }
                            break;
                    }

                    //Se il messaggio ricevuto appartiene a questo client o deve essere smistato
                    if (Destinatario != null || SK_Destinatario == Me.SessionKey)
                        ElaboraMessaggioCifrato(Mittente, Destinatario, Dati);
                    else
                        Debug.Log("Ricevuto messaggio con SessionKey non combaciante, rifiuto del messaggio.");
                break;
                case "CRYPTKEY":
                    // Struttura: CRYPTKEY:<SK_Mittente>;<Chiave_RSA_Pubblica>
                    SK_Mittente = Data.Split(';')[0];
                    string ChiavePubblica = Data.Remove(0, Data.IndexOf(';') + 1);

                    //Trova il mittente del messaggio
                    Mittente = TrovaPerSessionKey(SK_Mittente);

                    // Controlla che il mittente sia quello reale
                    switch(opMode)
                    {
                        case ModalitaOperativa.Rs232:
                            if (Mittente.serialPort != (System.IO.Ports.SerialPort)sender)
                            {
                                Debug.Log(((System.IO.Ports.SerialPort)sender).PortName + " sta cercando di inviare una chiave RSA che non gli appartiene!", Debug.LogType.WARNING);
                                return;
                            }
                        break;
                        case ModalitaOperativa.Sockets:
                            if (Mittente.Sock != ((SocketManager.StateObject)sender).Sock)
                            {
                                Debug.Log(((SocketManager.StateObject)sender).Sock.RemoteEndPoint.ToString() + " sta cercando di inviare una chiave RSA che non gli appartiene!", Debug.LogType.WARNING);
                                return;
                            }
                        break;
                    }

                    // Annuncia la registrazione in DEBUG
                    Debug.Log(String.Format("Registrazione di una nuova Chiave Pubblica RSA per {0}!", Mittente.Name));

                    // Chiave pubblica di un host in arrivo
                    RegistraCriptoKey(ChiavePubblica, Mittente);
                break;
                case "KEY":
                    // Alla ricezione di questo messaggio (proveniente dal server) l'utente imposta la chiave fornita
                    Debug.Log("Recived SessionKey = " + Data);
                    if (Me.SessionKey == null)
                        Me.SessionKey = Data;
                    break;
                case "HALOHA":
                    RegistraUtente(Data, sender);
                    break;
                case "DISCONNECTED":
                    UtenteDisconnesso(Data, sender);
                    break;
            }
        }

        //Metodi diversificati per client e server
        protected virtual void ElaboraMessaggio(Identity Mittente, Identity Destinatario, string Messaggio) { }
        protected virtual void ElaboraMessaggioCifrato(Identity Mittente, Identity Destinatario, string Data) { }
        protected virtual void RegistraUtente(string Dati, object Source) { }
        protected virtual void UtenteDisconnesso(string Dati, object Source) { }
        protected virtual void RegistraCriptoKey(string Dati, Identity Mittente) { }
        protected virtual void AggiornaChiaviAsimmetriche(RSAContainer newContainer) { }

        //Metodi di ricerca delle Identity nella lista dei destinatari
        #region MetodiIdentity
        // Non usare, può dare risultati inconsistenti
        public Identity TrovaPerNome(string Nome)
        {
            foreach (Identity i in Destinatari)
                if (i.Name == Nome)
                    return i;
            return null;
        }

        // Restituisce un Identity grazie alla chiave di identificazione univoca dei vari client
        public Identity TrovaPerSessionKey(string SessionKey)
        {
            foreach (Identity i in Destinatari)
                if (i.SessionKey == SessionKey)
                    return i;
            return null;
        }

        // Utilizzata su Destinatari di cui non si conoscono ne Nome ne SessionKey
        public Identity TrovaPerCOMPort(string NomePorta)
        {
            foreach (Identity i in Destinatari)
                if (i.serialPort.PortName == NomePorta)
                    return i;
            return null;
        }

        // Utilizzata per Destinatari di cui si conosce solo il Socket di provenienza
        public Identity TrovaPerSocket(Socket sock)
        {
            foreach (Identity i in Destinatari)
                if (i.Sock == sock)
                    return i;
            return null;
        }

        #endregion

    }
}
