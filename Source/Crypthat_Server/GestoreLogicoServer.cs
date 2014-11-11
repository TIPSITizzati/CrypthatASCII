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

        public List<Identity> Ignoti { get; set; } //Lista degli host ignoti


        public GestoreLogicoServer(ModalitaOperativa opMode) : base(opMode) 
        {
            this.Ignoti = new List<Identity>();
        }

        //Inizializza il server su tutte le porte
        public override void Inizializza()
        {
            switch(opMode)
            {
                case ModalitaOperativa.Rs232:
                    Rs232Manager = new Rs232Manager(this); 

                    //Inizializza tutte lo porte
                    foreach (string Name in SerialPort.GetPortNames())
                    {
                        Identity Ignoto = new Identity(null, null);
                        Rs232Manager.InizializzaPorta(Ignoto, Name);

                        Ignoti.Add(Ignoto);
                    }
                break;
            }
        }

        //Modifica il metodo di ricezione Haloha per reinviare i dati a tutti i client
        protected override void RegistraUtente(string Dati, object Source)
        {
            string[] Data = Dati.Split(';');
            string SessionKey = Data[0];
            string Name = Data[1];

            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    SerialPort port = (SerialPort)Source;

                    //Lista di host da sincronizzare
                    Identity temp = TrovaIgnotoPerCOM(port.PortName);

                    temp.SessionKey = SessionKey;
                    temp.Name = Name;

                    //Rimuove l'utente dalla lista ignoti
                    Ignoti.Remove(temp);

                    //Comunica a tutti gli altri host dell'avvenuta connessione
                    foreach (Identity dest in Destinatari)
                        InviaMessaggio("HALOHA:" + Data, dest);

                    //Aggiunge l'utente alla propria lista di destinatari
                    Destinatari.Add(temp);

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


        public Identity TrovaIgnotoPerCOM(string COMName)
        {
            foreach (Identity Ignoto in Ignoti)
                if (Ignoto.serialPort.PortName == COMName)
                    return Ignoto;
            return null;
        }
    }
}
