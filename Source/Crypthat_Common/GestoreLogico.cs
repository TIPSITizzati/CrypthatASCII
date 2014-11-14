using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

using Crypthat_Common;
using Crypthat_Common.Connessioni;

namespace Crypthat_Common
{
    public class GestoreLogico
    {
        public Identity Me; //Riferimento a se stesso
        public List<Identity> Destinatari { get; set; } //Lista dei destinatari memorizzati
        protected ModalitaOperativa opMode; //Modalità in cui il programma funzionerà
        protected Rs232Manager Rs232Manager; //In caso sia in modalità Rs232

        public GestoreLogico(ModalitaOperativa opMode)
        {
            this.opMode = opMode;
            this.Destinatari = new List<Identity>(10);
            this.Me = new Identity(null, null);

            Debug.Log("Inizializzazione GestoreLogico...");
            Inizializza();
        }

        public GestoreLogico(ModalitaOperativa opMode, string NomePorta)
        {
            this.opMode = opMode;
            this.Destinatari = new List<Identity>(10);
            this.Me = new Identity(null, null);

            Debug.Log("Inizializzazione GestoreLogico...");
            Inizializza(NomePorta);
        }

        //Metodo di inizializzazione globale (sia per client che per server)
        public void Inizializza()
        {
            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    //Inizializza tutte lo porte
                    foreach (string Name in SerialPort.GetPortNames())
                        Inizializza(Name);
                break;
            }
        }

        public void Inizializza(string NomePorta)
        {
            if (Rs232Manager == null)
            {
                Debug.Log("Inizializzazione manager RS232...");
                Rs232Manager = new Rs232Manager();
                Rs232Manager.OnMessaggioRicevuto += InterpretaTipoMessaggio;
            }

            Identity Ignoto = new Identity(null, null);
            Rs232Manager.InizializzaPorta(Ignoto, NomePorta);

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
                    Rs232Manager.InviaMessaggio(String.Format("MSG:{0};{1}", Me.SessionKey, Messaggio), Destinatario);
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
            string Data = msg.Remove(0, msg.IndexOf(':'));

            // Switch per i vari header
            switch (Header)
            {
                case "MSG":
                    string SessionKey = Data.Split(';')[0];
                    string Messaggio = Data.Remove(0, Data.IndexOf(';'));

                    Identity Mittente = TrovaPerSessionKey(SessionKey);

                    ElaboraMessaggio(Mittente, Messaggio);
                break;
                case "CRYPT":
                    break;
                case "KEY":
                    Debug.Log("Recived SessionKey = " + Data);
                    if(Me.SessionKey == null)
                        Me.SessionKey = Data;
                    break;
                case "HALOHA":
                    RegistraUtente(Data, args.Subject.serialPort);
                break;
            }
        }

        //Metodi per client e server
        protected virtual void ElaboraMessaggio(Identity Mittente, string Messaggio) { }
        protected virtual void RegistraUtente(string Dati, object Source) { }

        #region MetodiIdentity
        public Identity TrovaPerNome(string Nome)
        {
            foreach (Identity i in Destinatari)
                if (i.Name == Nome)
                    return i;
            return null;
        }

        public Identity TrovaPerSessionKey(string SessionKey)
        {
            foreach (Identity i in Destinatari)
                if (i.SessionKey == SessionKey)
                    return i;
            return null;
        }

        public Identity TrovaPerCOMPort(string NomePorta)
        {
            foreach (Identity i in Destinatari)
                if (i.serialPort.PortName == NomePorta)
                    return i;
            return null;
        }
        #endregion

    }
}
