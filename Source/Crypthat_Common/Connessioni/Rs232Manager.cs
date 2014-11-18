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
        //Evento per la ricezione di un messaggio
        public delegate void MessaggioRicevuto(object sender, InterLevelArgs args);
        public event MessaggioRicevuto OnMessaggioRicevuto;

        public void InviaMessaggio(string Dati, Identity Destinatario)
        {
            if (Destinatario.serialPort != null)
                Destinatario.serialPort.Write(Dati + (char)243);
            else
                throw new Exception("Porta non inizializzata");

            Destinatario.serialPort.DataReceived += RiceviMessaggio;
        }

        void RiceviMessaggio(object sender, SerialDataReceivedEventArgs e)
        {
            //Porta seriale di provenienza
            SerialPort porta = (SerialPort)sender;

            string Dati = porta.ReadExisting();

            if (Dati.Contains((char)243))
            {
                //Rimuove il carattere di escape
                Dati.Remove(Dati.Length - 1);

                porta.DiscardInBuffer();

                Identity temp = new Identity(null, null);
                temp.serialPort = porta;

                //Richiama l'evento
                if (OnMessaggioRicevuto != null)
                    OnMessaggioRicevuto(this, new InterLevelArgs(temp, Dati));
                else
                    throw new Exception("Evento di ricezione messaggio non impostato!");
            }
        }

        public void InizializzaPorta(Identity destinatario, string PortName)
        {
            destinatario.serialPort = new SerialPort(PortName);
            destinatario.serialPort.BaudRate = 9600;
            destinatario.serialPort.DataBits = 8;
            destinatario.serialPort.Parity = Parity.Even;
            destinatario.serialPort.StopBits = StopBits.One;
            destinatario.serialPort.DataReceived += RiceviMessaggio;

            //Controllo per i client (in cui la porta di destinazione è sempre quella del server)
            if (SerialPort.GetPortNames().Contains(PortName))
            {
                try
                {
                    destinatario.serialPort.Open();
                    Debug.Log("Inizializzata porta " + PortName + "!");
                }
                catch
                {
                    Debug.Log(PortName + " doesn't exist or it's already open, skipping port!", Debug.LogType.ERROR);
                }
            }
            else
                Debug.Log(PortName + " doesn't exist or it's already open, skipping port!", Debug.LogType.ERROR);
        }
    }
}
