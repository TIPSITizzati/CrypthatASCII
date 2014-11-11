using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crypthat_Common.Connessioni;

namespace Crypthat_Common
{
    public class GestoreLogico
    {
        public List<Identity> Destinatari { get; set; } //Lista dei destinatari memorizzati
        protected ModalitaOperativa opMode; //Modalità in cui il programma funzionerà
        protected Rs232Manager Rs232Manager; //In caso sia in modalità Rs232

        // Delegate per L'evento
        public delegate void MessaggioRicevuto(Identity Mittente,  string Messaggio);
        public event MessaggioRicevuto OnMessaggioRicevuto;

        public enum ModalitaOperativa
        {
            Rs232,
            Sockets
        }

        public GestoreLogico(ModalitaOperativa opMode)
        {
            this.opMode = opMode;
            this.Destinatari = new List<Identity>(10);
            Inizializza();
        }

        public GestoreLogico(ModalitaOperativa opMode, string NomePorta)
        {
            this.opMode = opMode;
            this.Destinatari = new List<Identity>(10);
            Inizializza(NomePorta);
        }

        //Metodo che differenzia le inizializzazioni tra client e server
        public abstract void Inizializza();
        public abstract void Inizializza(string NomePorta);

        // Invia i messaggi al livello sottostante in base ad opMode
        // I dati vengono criptati di default
        public void InviaMessaggio(string Messaggio, Identity Destinatario, bool Encrypted = true)
        {
            switch (opMode)
            {
                case ModalitaOperativa.Rs232:

                    break;
                case ModalitaOperativa.Sockets:

                    break;
            }
        }

        // Riceve i dati dallo strato inferiore (Indipendentemente dal tipo RS232 o Sockets)
        public void RiceviMessaggio(string Dati)
        {
            InterpretaTipoMessaggio(Dati);
        }

        /*
         * Ogni Messaggio è compsto nella seguente maniera =>  {HEADER}:Messaggio
         * I tipi di HEADER sono:
         *  "MSG"   - Indica che l'header è seguito da un messaggio in chiaro
         *  "CRYPT" - Indica che l'header è seguito da una struttura dati criptata
         *  "KEY"   - Indica che l'header è seguito da una chiave
         *  "HALOHA"- Indica che l'header è seguito dai dati di un utente
         */
        protected void InterpretaTipoMessaggio(string msg)
        {
            string Header = msg.Split(':')[0];
            string Data = msg.Remove(0, msg.IndexOf(':'));

            // Switch per i vari header
            switch (Header)
            {
                case "MSG":
                    string SessionKey = Data.Split(';')[0];
                    string Messaggio = Data.Remove(0, Data.IndexOf(';'));

                    Identity Mittente = TrovaPerSessionKey(SessionKey);

                    //Richiama l'evento per lo strato superiore
                    if (OnMessaggioRicevuto != null)
                        OnMessaggioRicevuto(Mittente, Messaggio);
                    else
                        throw new Exception("Evento di ricezione messaggio non impostato!");
                break;
                case "CRYPT":
                    break;
                case "KEY":
                    break;
                case "HALOHA":
                    break;
            }
        }

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
