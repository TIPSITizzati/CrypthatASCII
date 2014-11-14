﻿using System;
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
                Destinatario.serialPort.Write(Dati);
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

            //Richiama l'evento
            if (OnMessaggioRicevuto != null)
                OnMessaggioRicevuto(this, new InterLevelArgs(null, Dati));
            else
                throw new Exception("Evento di ricezione messaggio non impostato!");
        }

        public void InizializzaPorta(Identity destinatario, string PortName)
        {
            Debug.Log("Inizializzata porta " + PortName + "!");
            destinatario.serialPort = new SerialPort(PortName);
            destinatario.serialPort.DataBits = 8;
            destinatario.serialPort.DataReceived += RiceviMessaggio;
        }
    }
}
