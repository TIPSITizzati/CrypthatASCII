using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void InviaMessaggio(string Messaggio, Identity Destinatario)
        {

        }

        public void RiceviMessaggio(string Dati)
        {

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
