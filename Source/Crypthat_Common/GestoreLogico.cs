using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;

using Crypthat_Common;
using Crypthat_Common.Connessioni;

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
        protected Rs232Manager Rs232Manager; // In caso sia in modalità Rs232
        protected SocketManager SocketManager; // In caso sia in modalità Socket

        //Costruttore di default che inizializza il GestoreLogico con un Identità ignota (in attesa di un'Identità dal server)
        public GestoreLogico(ModalitaOperativa opMode)
        {
            this.opMode = opMode;
            this.Destinatari = new List<Identity>(10);
            this.Me = new Identity(null, null);

            switch (opMode)
            {
                //Inizializza la parte Rs232
                case ModalitaOperativa.Rs232:
                    Debug.Log("Inizializzazione manager RS232...");
                    Rs232Manager = new Rs232Manager();                              // Inizializza RS232Manager
                    Rs232Manager.OnMessaggioRicevuto += InterpretaTipoMessaggio;    // Imposta l'evento OnMessaggioRicevuto in modo che chiami il metodo InterpretaTipoMessaggio
                break;
                //Inizializza la parte Socket
                case ModalitaOperativa.Sockets:
                    Debug.Log("Inizializzazione manager Sockets...");
                    SocketManager = new SocketManager();
                    SocketManager.OnMessaggioRicevuto += InterpretaTipoMessaggio;
                break;
            }

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
                    SocketManager.Ascolta();
                break;
            }
        }

        // Metodo che inizializza una porta specifica Rs232
        public void Inizializza(string NomePorta)
        {
            Identity Ignoto = new Identity(null, null); // Non sapendo a chi si è connessi, viene creata un Identità ignota
            Rs232Manager.InizializzaPorta(Ignoto, NomePorta); // Viene aperta la connessione sulla porta richiesta

            Destinatari.Add(Ignoto);
        }

        // Invia i messaggi al livello sottostante in base ad opMode
        // I dati vengono criptati di default
        public void InviaMessaggio(string Messaggio, Identity Destinatario, bool Encrypted = true)
        {
            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    //TODO: Aggiungere crittografia
                    Rs232Manager.InviaMessaggio(String.Format("MSG:{0}?{1};{2}", Me.SessionKey, Destinatario.SessionKey, Messaggio), Destinatario);
                break;
                case ModalitaOperativa.Sockets:

                    break;
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

            // Switch per i vari header
            switch (Header)
            {
                case "MSG":
                    string SessionKeys = Data.Split(';')[0];
                    string Messaggio = Data.Remove(0, Data.IndexOf(';'));

                    // Divide le due SessionKeys
                    string SK_Mittente = Data.Split('?')[0];
                    string SK_Destinatario = Data.Split('?')[0];

                    // Ottiene i dati di mittente e destinatario tramite il metodo TrovaPerSessionKey
                    Identity Mittente = TrovaPerSessionKey(SK_Mittente);
                    Identity Destinatario = TrovaPerSessionKey(SK_Destinatario);

                    ElaboraMessaggio(Mittente, Destinatario, Messaggio);
                break;
                case "CRYPT":
                    break;
                case "KEY":
                    // Alla ricezione di questo messaggio (proveniente dal server) l'utente imposta la chiave fornita
                    Debug.Log("Recived SessionKey = " + Data);
                    if(Me.SessionKey == null)
                        Me.SessionKey = Data;
                    break;
                case "HALOHA":
                    RegistraUtente(Data, args.Subject.serialPort);
                break;
            }
        }

        //Metodi diversificati per client e server
        protected virtual void ElaboraMessaggio(Identity Mittente, Identity Destinatario, string Messaggio) { }
        protected virtual void RegistraUtente(string Dati, object Source) { }

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
