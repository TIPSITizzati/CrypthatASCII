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
        public delegate void MessaggioRicevuto(object sender, InterLevelArgs args);
        public event MessaggioRicevuto OnMessaggioRicevuto;


        public GestoreLogicoClient(ModalitaOperativa opMode, Identity Me, string NomePorta) : base(opMode) 
        {
            this.Me = Me;

            Inizializza(NomePorta);

            //Autenticazione con il server
            if (Destinatari.Count > 0)
                InviaMessaggio("HALOHA:" + Me.Name + ";" + Me.SessionKey, Destinatari[0]);
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

                break;
            }
        }

        //Richiama l'evento per l'interfaccia grafica
        protected override void ElaboraMessaggio(Identity Mittente, string Messaggio)
        {
            //Richiama l'evento per lo strato superiore
            if (OnMessaggioRicevuto != null)
                OnMessaggioRicevuto(this, new InterLevelArgs(Mittente, Messaggio));
            else
                throw new Exception("Evento di ricezione messaggio non impostato!");
        }
    }
}
