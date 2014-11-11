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
                        Identity Ignoto = new Identity("Unknown", "Unknown");
                        Rs232Manager.InizializzaPorta(Ignoto, Name);

                        Ignoti.Add(Ignoto);
                    }
                break;
            }
        }
    }
}
