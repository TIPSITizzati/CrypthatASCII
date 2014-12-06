using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crypthat_Common;
using Crypthat_Common.Connessioni;

namespace Crypthat_Client
{
    public class GestoreLogicoClient : GestoreLogico
    {

        // Delegate per gli eventi grafici
        public delegate void EventoRicevuto(object sender, InterLevelArgs args);

        // Eventi
        public event EventoRicevuto OnMessaggioRicevuto;    // Evento scatenato quando un messaggio viene ricevuto
        public event EventoRicevuto OnUtenteRegistrato;     // Evento ricevuto quando un utente si è registrato
        public event EventoRicevuto OnUtenteDisconnesso;     // Evento ricevuto quando un utente si è registrato


        public GestoreLogicoClient(ModalitaOperativa opMode, Identity Me, string NomePorta)
            : base(opMode)
        {
            this.Me = Me;

            Inizializza(NomePorta);

            Autenticazione();
        }

        public GestoreLogicoClient(ModalitaOperativa opMode, Identity Me, System.Net.IPEndPoint serverEndpoint)
            : base(opMode)
        {
            this.Me = Me;

            Inizializza(serverEndpoint);

            Autenticazione();
        }

        private void Autenticazione()
        {
            //Autenticazione con il server
            if (Destinatari.Count > 0)
            {
                ConnectionManager.InviaMessaggio("HALOHA:" + Me.Name + ";" + Me.SessionKey, Destinatari[0]);
                Destinatari.Remove(Destinatari[0]);
            }
        }

        //Modifica il metodo di ricezione Haloha per reinviare i dati a tutti i client
        protected override void RegistraUtente(string Dati, object Source)
        {
            string[] Data = Dati.Split(';');
            string Name = Data[0];
            string SessionKey = Data[1];

            Identity temp = new Identity(Name, SessionKey);
            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    temp.serialPort = (System.IO.Ports.SerialPort)Source;
                    break;
                case ModalitaOperativa.Sockets:
                    temp.Sock = ((SocketManager.StateObject)Source).Sock;
                    break;
            }


            Identity Ignoto = null;
            //Controlla che la SessionKey utilizzata non sia già presente
            if ((Ignoto = TrovaPerSessionKey(SessionKey)) == null)
                Destinatari.Add(temp);
            else
            {
                Ignoto.SessionKey = SessionKey;
                Ignoto.Name = Name;
            }

            //Richiama l'evento di registrazione di un utente (Per la parte grafica)
            if (OnUtenteRegistrato != null)
                OnUtenteRegistrato(this, new InterLevelArgs(temp, null));
            else
                throw new Exception("Evento di registrazione utente non impostato!");
        }

        //Richiama l'evento per l'interfaccia grafica
        protected override void ElaboraMessaggio(Identity Mittente, Identity Destinatario, string Messaggio)
        {
            //Richiama l'evento di ricezione di un messaggio (Per la parte grafica)
            if (OnMessaggioRicevuto != null)
                OnMessaggioRicevuto(this, new InterLevelArgs(Mittente, Messaggio));
            else
                throw new Exception("Evento di ricezione messaggio non impostato!");
        }

        // Gestisce la disconnessione di un utente dalla chat
        protected override void UtenteDisconnesso(string Dati, object Source)
        {
            // Trova il riferimento all'utente disconnesso
            Identity utenteDisconnesso = TrovaPerSessionKey(Dati);

            int index = -1;
            // Se l'utente disconnesso è registrato nella lista dei destinatari
            if(utenteDisconnesso != null)
            {
                // Ottiene un riferimento per la parte grafica alla posizione dell'utente
                // nella lista destinatari e cancella l'utente disconnesso dalla lista.
                index = Destinatari.IndexOf(utenteDisconnesso);
                Destinatari.Remove(utenteDisconnesso);
            }

            // Passa le informazioni al livello grafico
            if (OnUtenteDisconnesso != null)
                OnUtenteDisconnesso(this, new InterLevelArgs(utenteDisconnesso, index));
            else
                throw new Exception("Evento di disconnession utente non impostato!");
        }
    }
}
