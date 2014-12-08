using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crypthat_Common;
using Crypthat_Common.Connessioni;
using Crypthat_Common.Crittografia;

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


        public GestoreLogicoClient(ModalitaOperativa opMode, string Name, string NomePorta)
            : base(opMode)
        {
            this.Me.Name = Name;

            Inizializza(NomePorta);

            Autenticazione();
        }

        public GestoreLogicoClient(ModalitaOperativa opMode, string Name, System.Net.IPEndPoint serverEndpoint)
            : base(opMode)
        {
            this.Me.Name = Name;

            Inizializza(serverEndpoint);

            Autenticazione();
        }

        private void Autenticazione()
        {
            // Autenticazione con il server
            if (Destinatari.Count > 0)
            {
                ConnectionManager.InviaMessaggio("HALOHA:" + Me.Name + ";" + Me.SessionKey, Destinatari[0]);
                Destinatari.Remove(Destinatari[0]);
            }
        }

        // Modifica il metodo di ricezione Haloha per reinviare i dati a tutti i client
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
            // Controlla che la SessionKey utilizzata non sia già presente
            if ((Ignoto = TrovaPerSessionKey(SessionKey)) == null)
                Destinatari.Add(temp);
            else
            {
                Ignoto.SessionKey = SessionKey;
                Ignoto.Name = Name;
            }

            // Se si è appena registrato il server
            if(Destinatari.Count == 1)
                // Simula la creazione di una chiave
                AggiornaChiaviAsimmetriche(Me.RSAContainer);

            // Richiama l'evento di registrazione di un utente (Per la parte grafica)
            if (OnUtenteRegistrato != null)
                OnUtenteRegistrato(this, new InterLevelArgs(temp, null));
            else
                throw new Exception("Evento di registrazione utente non impostato!");
        }

        // Richiama l'evento per l'interfaccia grafica
        protected override void ElaboraMessaggio(Identity Mittente, Identity Destinatario, string Messaggio)
        {
            // Richiama l'evento di ricezione di un messaggio (Per la parte grafica)
            if (OnMessaggioRicevuto != null)
                OnMessaggioRicevuto(false, new InterLevelArgs(Mittente, Messaggio));
            else
                throw new Exception("Evento di ricezione messaggio non impostato!");
        }

        // Decifra il messaggio cifrato e lo manda all'interfaccia
        protected override void ElaboraMessaggioCifrato(Identity Mittente, Identity Destinatario, string Data)
        {
            // Ricompone le parti del messaggio cifrato con AES
            string[] Parts = Data.Split(new string[] { "<KEY>" }, StringSplitOptions.RemoveEmptyEntries);
            string[] Keys = Parts[1].Split(new string[] { "<IV>" }, StringSplitOptions.RemoveEmptyEntries);
            byte[] AES_Messaggio = Convert.FromBase64String(Parts[0].Replace("<\\", "<"));

            byte[] AES_KEY = RSACypher.EncryptDecrypt(Convert.FromBase64String(Keys[0]), Me.RSAContainer.PrivateKey);
            byte[] AES_IV = RSACypher.EncryptDecrypt(Convert.FromBase64String(Keys[1]), Me.RSAContainer.PrivateKey);

            // Decifra il messaggio con la chiave simmetrica ottenuta
            string Messaggio = AESCypher.Decrypt(AES_Messaggio, AES_KEY, AES_IV);

            // Richiama l'evento di ricezione di un messaggio (Per la parte grafica)
            if (OnMessaggioRicevuto != null)
                OnMessaggioRicevuto(true, new InterLevelArgs(Mittente, Messaggio));
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

        // Metodo chiamato allo scadere della coppia di chiavi RSA attuali
        protected override void AggiornaChiaviAsimmetriche(RSAContainer newContainer)
        {
            if (Destinatari.Count > 0)
            {
                ConnectionManager.InviaMessaggio(String.Format("CRYPTKEY:{0};{1}", Me.SessionKey, newContainer.PublicKey), Destinatari[0]);
                this.Me.RSAContainer = newContainer;
            }
        }

        // Registra la nuova chiave ricevuta
        protected override void RegistraCriptoKey(string Dati, Identity Mittente)
        {
            if(Mittente.RSAContainer == null)
                Mittente.RSAContainer = new RSAContainer();

            Mittente.RSAContainer.PublicKey = Dati;
        }
    }
}
