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
        private ModalitaOperativa opMode; //Modalità in cui il programma funzionerà

        public enum ModalitaOperativa
        {
            Rs232,
            Sockets
        }

        public GestoreLogico(ModalitaOperativa opMode)
        {
            this.opMode = opMode;
            this.Destinatari = new List<Identity>(10);
        }

        // Invia i messaggi al livello sottostante in base ad opMode
        // I dati vengono criptati di default
        public void InviaMessaggio(string Messaggio, Identity Destinatario, bool Encrypted = true)
        {

        }

        // Riceve i dati dallo strato inferiore (Indipendentemente dal tipo RS232 o Sockets)
        public void RiceviMessaggio(string Dati)
        {

        }

        /*
         * Ogni Messaggio è compsto nella seguente maniera =>  {HEADER}:Messaggio
         * I tipi di HEADER sono:
         *  "MSG"   - Indica che l'header è seguito da un messaggio in chiaro
         *  "CRYPT" - Indica che l'header è seguito da una struttura dati criptata
         *  "KEY"   - Indica che l'header è seguito da una chiave
         *  "HALOHA"- Indica che l'header è seguito dai dati di un utente
         */
        private void InterpretaMessaggio(string msg)
        {
            string Header = msg.Split(':')[0];
            string Message = msg.Remove(0, msg.IndexOf(':'));

            // Switch per i vari header
            switch (Header)
            {
                case "MSG":
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
        #endregion

    }
}
