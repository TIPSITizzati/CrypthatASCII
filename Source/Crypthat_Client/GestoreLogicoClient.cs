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

        // Delegate per L'evento grafico
        public delegate void MessaggioRicevuto(Identity Mittente, string Messaggio);
        public event MessaggioRicevuto OnMessaggioRicevuto;


        public GestoreLogicoClient(ModalitaOperativa opMode) : base(opMode) { }

        //Modifica il metodo di ricezione Haloha per reinviare i dati a tutti i client
        protected override void RegistraUtente(string Dati, object Source)
        {
            string[] Data = Dati.Split(';');
            string SessionKey = Data[0];
            string Name = Data[1];

            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    Identity temp = new Identity(Name, SessionKey);
                    temp.serialPort = (System.IO.Ports.SerialPort)Source;
                    Destinatari.Add(temp);
                break;
            }
        }

        //Richiama l'evento per l'interfaccia grafica
        protected override void ElaboraMessaggio(Identity Mittente, string Messaggio)
        {
            //Richiama l'evento per lo strato superiore
            if (OnMessaggioRicevuto != null)
                OnMessaggioRicevuto(Mittente, Messaggio);
            else
                throw new Exception("Evento di ricezione messaggio non impostato!");
        }
    }
}
