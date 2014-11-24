﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace Crypthat_Common.Connessioni
{
    /* Classe posta al livello di astrazione più basso che si occupa solamente
     * di inviare e ricevere messaggi sulle porte Rs232 fornendo al GestoreLogico
     * i messaggi interi.
     */

    public class Rs232Manager
    {
        //Evento per la ricezione di un messaggio
        public delegate void MessaggioRicevuto(object sender, InterLevelArgs args);
        public event MessaggioRicevuto OnMessaggioRicevuto;

        // Metodo standard per l'invio di Dati ad un destinatario
        public void InviaMessaggio(string Dati, Identity Destinatario)
        {
            // Se la porta del destinatario è inizializzata allora scrive i dati
            if (Destinatario.serialPort != null)
                Destinatario.serialPort.Write(Dati + (char)126);
            else
                throw new Exception("Porta non inizializzata");
        }

        void RiceviMessaggio(object sender, SerialDataReceivedEventArgs e)
        {
            //Porta seriale di provenienza
            SerialPort porta = (SerialPort)sender;

            string Dati = porta.ReadExisting();

            if (Dati.Contains((char)126))
            {
                //Rimuove carattere di escape
                Dati = Dati.Remove(Dati.Length - 1);

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

            // Controlla che la porta non sia già stata aperta
            if (SerialPort.GetPortNames().Contains(PortName))
            {
                try
                {
                    destinatario.serialPort.Open();
                    destinatario.serialPort.DataReceived += RiceviMessaggio;
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
