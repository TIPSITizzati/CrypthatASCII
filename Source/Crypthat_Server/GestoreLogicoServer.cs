using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crypthat_Common;
using Crypthat_Common.Connessioni;

using System.IO.Ports;

namespace Crypthat_Server
{
    class GestoreLogicoServer : GestoreLogico
    {
        public GestoreLogicoServer(ModalitaOperativa opMode) : base(opMode) { }

        //Modifica il metodo di ricezione Haloha per reinviare i dati a tutti i client
        protected override void RegistraUtente(string Dati, object Source)
        {
            Debug.Log("Inizializzazione registrazione utente...", Debug.LogType.INFO);
            string[] Data = Dati.Split(';');
            string SessionKey = Data[0];
            string Name = Data[1];

            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    SerialPort port = (SerialPort)Source;

                    // Lista di host da sincronizzare
                    Identity temp = TrovaPerCOMPort(port.PortName);

                    temp.SessionKey = SessionKey;
                    temp.Name = Name;

                    // Messaggi di debug
                    Debug.Log(String.Format("Registrazione utente {0} con SessionKey = {1}", Name, SessionKey), Debug.LogType.INFO);

                    // Comunica a tutti gli altri host dell'avvenuta connessione
                    // E' utilizzata una query linq per risparmiare alcune linee di codice
                    foreach (Identity dest in Destinatari.Where(id => id != temp))
                        InviaMessaggio("HALOHA:" + Data, dest);
                break;
            }
        }

        //Smista i messaggi ricevuti ad i corrispettivi destinatari
        protected override void ElaboraMessaggio(Identity Mittente, string Messaggio)
        {
            //TODO: Per ora effettua solo un semplice smistamento
            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    Rs232Manager.InviaMessaggio(Messaggio, Mittente);
                break;
            }
        }
    }
}
