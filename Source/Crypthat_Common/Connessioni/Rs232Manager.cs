using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace Crypthat_Common.Connessioni
{
    public class Rs232Manager
    {
        //TODO: Aggiungere evento come nel gestore logico (molto meglio)
        //Riferimento al gestore logico attivo
        private GestoreLogico GestoreLogico;


        public Rs232Manager(GestoreLogico GestoreLogico)
        {
            this.GestoreLogico = GestoreLogico;
        }

        public void InviaMessaggio(string Messaggio, Identity Destinatario)
        {
            if (Destinatario.serialPort != null)
                Destinatario.serialPort.Write(Messaggio);
            else
                throw new Exception("Porta non inizializzata");

            Destinatario.serialPort.DataReceived += RiceviMessaggio;
        }

        void RiceviMessaggio(object sender, SerialDataReceivedEventArgs e)
        {
            //Porta seriale di provenienza
            SerialPort porta = (SerialPort)sender;

            string Dati = porta.ReadExisting();
            porta.DiscardInBuffer();

            GestoreLogico.RiceviMessaggio(Dati, sender);
        }

        public void InizializzaPorta(Identity destinatario, string PortName)
        {
            destinatario.serialPort = new SerialPort(PortName);
            destinatario.serialPort.DataReceived += RiceviMessaggio;
        }
    }
}
