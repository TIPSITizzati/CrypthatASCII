using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crypthat_Common;
using Crypthat_Common.Connessioni;

namespace Crypthat_Client
{
    class GestoreLogicoClient : GestoreLogico
    {

        // Delegate per gli eventi grafici
        public delegate void EventoRicevuto(object sender, InterLevelArgs args);

        // Eventi
        public event EventoRicevuto OnMessaggioRicevuto;    // Evento scatenato quando un messaggio viene ricevuto
        public event EventoRicevuto OnUtenteRegistrato;     // Evento ricevuto quando un utente si è registrato


        public GestoreLogicoClient(ModalitaOperativa opMode, Identity Me, string NomePorta) : base(opMode) 
        {
            this.Me = Me;

            Inizializza(NomePorta);

            //Autenticazione con il server
            if (Destinatari.Count > 0)
                Rs232Manager.InviaMessaggio("HALOHA:" + Me.Name + ";" + Me.SessionKey, Destinatari[0]);
        }

        //Modifica il metodo di ricezione Haloha per reinviare i dati a tutti i client
        protected override void RegistraUtente(string Dati, object Source)
        {
            string[] Data = Dati.Split(';');
            string Name = Data[0];
            string SessionKey = Data[1];

            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    Identity temp = new Identity(Name, SessionKey);
                    temp.serialPort = (System.IO.Ports.SerialPort)Source;

                    //Controlla che la SessionKey utilizzata non sia già presente
                    Identity Ignoto;
                    if ((Ignoto = TrovaPerSessionKey(SessionKey)) == null)
                        Destinatari.Add(temp);
                    else
                    {
                        Ignoto.SessionKey = SessionKey;
                        Ignoto.Name = Name;
                    }

                    //Richiama l'evento di registrazione di un utente (Per la parte grafica)
                    if (OnUtenteRegistrato != null)
                        OnUtenteRegistrato(this, new InterLevelArgs(Ignoto, null));
                    else
                        throw new Exception("Evento di registrazione utente non impostato!");
                break;
            }
        }

        //Richiama l'evento per l'interfaccia grafica
        protected override void ElaboraMessaggio(Identity Mittente, Identity Destinatario, string Messaggio)
        {
            //Se il messaggio ricevuto appartiene a questo client
            if (Destinatario.SessionKey == Me.SessionKey)
            {
                //Richiama l'evento di ricezione di un messaggio (Per la parte grafica)
                if (OnMessaggioRicevuto != null)
                    OnMessaggioRicevuto(this, new InterLevelArgs(Mittente, Messaggio));
                else
                    throw new Exception("Evento di ricezione messaggio non impostato!");
            }
            else
                Debug.Log("Ricevuto messaggio con SessionKey non combaciante, rifiuto del messaggio.");
        }
    }
}
