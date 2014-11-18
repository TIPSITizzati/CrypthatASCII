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
        public GestoreLogicoServer(ModalitaOperativa opMode) : base(opMode) 
        {
            Me.Name = "Server";
            Me.SessionKey = GeneraSessionKey();
            Debug.Log("Generated server key = " + Me.SessionKey, Debug.LogType.WARNING);

            Inizializza();
        }

        //Modifica il metodo di ricezione Haloha per reinviare i dati a tutti i client
        protected override void RegistraUtente(string Dati, object Source)
        {
            Debug.Log("Inizializzazione registrazione utente...");
            string[] Data = Dati.Split(';');
            string Name = Data[0];
            string SessionKey = Data[1];

            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    SerialPort port = (SerialPort)Source;

                    // Lista di host da sincronizzare
                    Identity temp = TrovaPerCOMPort(port.PortName);

                    temp.SessionKey = GeneraSessionKey();
                    temp.Name = Name;

                    // Messaggi di debug
                    Debug.Log(String.Format("Registrazione utente {0} con SessionKey = {1}", temp.Name, temp.SessionKey), Debug.LogType.INFO);

                    //Invia la key all'utente che cerca di registrarsi
                    Rs232Manager.InviaMessaggio("KEY:" + temp.SessionKey, temp);

                    //Aggiorna i dati relativi al server del client
                    Rs232Manager.InviaMessaggio("HALOHA:" + Me.Name + ":" + Me.SessionKey, temp);

                    // Comunica a tutti gli altri host dell'avvenuta connessione
                    // E' utilizzata una query linq per risparmiare alcune linee di codice
                    foreach (Identity dest in Destinatari.Where(id => id != temp))
                    {
                        //Notifica I destinatari già registrati
                        Rs232Manager.InviaMessaggio("HALOHA:" + Data, dest);

                        //Notifica l'utente registrato degli altri utenti presenti
                        Rs232Manager.InviaMessaggio("HALOHA:" + dest.SessionKey + ":" + dest.Name, temp);
                    }
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

                for (int i = 0; i < KeyLenght; i++)
                {
                    int a = rnd.Next(1000);
                    if(a < 750)
                        Key += (char)rnd.Next(60, 126);
                    else
                        Key += (char)rnd.Next(48, 58);
                }

            } while (TrovaPerSessionKey(Key) != null);

            return Key;
        }
    }
}
