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
    /*
     * Classe derivante da GestoreLogico per differenziare i metodi di Ricezione di messaggi
     * e di Registrazione degli utenti dal Client.
     * */

    //TODO: Aggiungere interfaccia per la gestione della connessione
    class GestoreLogicoServer : GestoreLogico
    {
        // Costruttore iniziale che mette in ascolto il server su più porte
        public GestoreLogicoServer(ModalitaOperativa opMode) : base(opMode) 
        {
            // All'avvio il server si imposta con nome = "Server" e si genera una chiave univoca per iniziare a comunicare.
            Me.Name = "Server";
            Me.SessionKey = GeneraSessionKey();
            Debug.Log("Generated server key = " + Me.SessionKey, Debug.LogType.WARNING);

            // TODO: Modificare per definire da console che modalità usare
            // Chiama il metodo Inizializza di "GestoreLogico" che mette il server in ascolto su tutte le porte COM / sul Socket predefinito
            Inizializza();
        }

        // Modifica il metodo di ricezione degli "Haloha" per sincronizzare le liste dei Destinatari tra tutti gli utenti connessi
        protected override void RegistraUtente(string Dati, object Source)
        {
            Debug.Log("Inizializzazione registrazione utente...");
            string[] Data = Dati.Split(';');
            string Name = Data[0];
            string SessionKey = Data[1];

            switch (opMode)
            {
                // In caso si stia usando la modalità RS232
                case ModalitaOperativa.Rs232:
                    SerialPort port = (SerialPort)Source;

                    // Trova l'host che si deve registrare vedendo da quale porta proviene la richiesta
                    Identity temp = TrovaPerCOMPort(port.PortName);

                    temp.SessionKey = GeneraSessionKey();   // Genera una chiave per il nuovo utente
                    temp.Name = Name;                       // Ottiene il nome dichiarato dall'utente

                    // Messaggi di debug
                    Debug.Log(String.Format("Registrazione utente {0} con SessionKey = {1}", temp.Name, temp.SessionKey), Debug.LogType.INFO);

                    //Invia la key all'utente che cerca di registrarsi
                    Rs232Manager.InviaMessaggio("KEY:" + temp.SessionKey, temp);

                    //Aggiorna i dati relativi al server del client
                    Rs232Manager.InviaMessaggio("HALOHA:" + Me.Name + ":" + Me.SessionKey, temp);

                    // Comunica a tutti gli altri host dell'avvenuta connessione
                    // E' utilizzata una query linq per risparmiare alcune linee di codice
                    // Equivalente in SQL = "SELECT * FROM Destinatari WHERE temp.Id <> Destinatari.Id;"
                    foreach (Identity dest in Destinatari.Where(id => id != temp))
                    {
                        //Notifica I destinatari già registrati dalla connessione di "temp"
                        Rs232Manager.InviaMessaggio("HALOHA:" + Data, dest);

                        //Notifica l'utente registrato degli altri utenti presenti
                        Rs232Manager.InviaMessaggio("HALOHA:" + dest.SessionKey + ":" + dest.Name, temp);
                    }
                break;
                // In caso si stia usando la modalità Sockets
                case ModalitaOperativa.Sockets:
                    SocketManager.StateObject state = (SocketManager.StateObject)Source;

                    // Se qualcuno è già registrato con quel socket
                    if (TrovaPerSocket(state.Sock) != null)
                    {
                        Debug.Log(String.Format("Utente {0} già registrato!", state.Sock.RemoteEndPoint.ToString()), Debug.LogType.ERROR);
                        return;
                    }

                    //Crea una identity per l'utente registrato
                    Identity nuovoUtente = new Identity(Name, GeneraSessionKey());
                    nuovoUtente.Sock = state.Sock;

                    // Messaggi di debug
                    Debug.Log(String.Format("Registrazione utente {0} con SessionKey = {1}", nuovoUtente.Name, nuovoUtente.SessionKey), Debug.LogType.INFO);

                    //Invia la key all'utente che cerca di registrarsi
                    SocketManager.InviaMessaggio("KEY:" + nuovoUtente.SessionKey, nuovoUtente);

                    //Aggiorna i dati relativi al server del client
                    SocketManager.InviaMessaggio("HALOHA:" + Me.Name + ":" + Me.SessionKey, nuovoUtente);

                    // Comunica a tutti gli altri host dell'avvenuta connessione
                    // E' utilizzata una query linq per risparmiare alcune linee di codice
                    // Equivalente in SQL = "SELECT * FROM Destinatari WHERE temp.Id <> Destinatari.Id;"
                    foreach (Identity dest in Destinatari.Where(id => id != nuovoUtente))
                    {
                        //Notifica I destinatari già registrati dalla connessione di "temp"
                        SocketManager.InviaMessaggio("HALOHA:" + Data, dest);

                        //Notifica l'utente registrato degli altri utenti presenti
                        SocketManager.InviaMessaggio("HALOHA:" + dest.SessionKey + ":" + dest.Name, nuovoUtente);
                    }
                break;
            }
        }

        //Smista i messaggi ricevuti ad i corrispettivi destinatari
        protected override void ElaboraMessaggio(Identity Mittente, Identity Destinatario, string Messaggio)
        {
            //TODO: Per ora effettua solo un semplice smistamento
            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    //Inoltra il messaggio al destinatario richiesto, mantanendo il mittente originario
                    Rs232Manager.InviaMessaggio(String.Format("MSG:{0}?{1};{2}", Mittente.SessionKey, Destinatario.SessionKey, Messaggio), Destinatario);
                break;
                case ModalitaOperativa.Sockets:
                    //Inoltra il messaggio al destinatario richiesto, mantanendo il mittente originario
                    SocketManager.InviaMessaggio(String.Format("MSG:{0}?{1};{2}", Mittente.SessionKey, Destinatario.SessionKey, Messaggio), Destinatario);
                break;
            }
        }

        //Metodo per generare una stringa alfanumerica valida
        private string GeneraSessionKey()
        {
            string Key; //Chiave finale
            Random rnd = new Random();

            //Fino a quando non trova una chiave non utilizzata
            do
            {
                Key = "";
                int KeyLenght = rnd.Next(64, 128);

                //Genera una chiave alfanumerica di KeyLenght caratteri
                for (int i = 0; i < KeyLenght; i++)
                {
                    int a = rnd.Next(1000);
                    if(a < 750)
                        Key += (char)rnd.Next(97, 122);
                    else
                        Key += (char)rnd.Next(48, 58);
                }

            } while (TrovaPerSessionKey(Key) != null);

            return Key;
        }
    }
}
