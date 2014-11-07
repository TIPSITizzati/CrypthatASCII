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
            SerialPort mittente = (SerialPort)sender;


        }
    }
}
