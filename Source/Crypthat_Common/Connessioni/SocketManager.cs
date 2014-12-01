using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Crypthat_Common.Connessioni
{
    /* Classe posta al livello di astrazione più basso che si occupa solamente
     * di inviare e ricevere messaggi tramite Sockets fornendo al GestoreLogico
     * i messaggi interi
     * 
     * Basato sulla documentazione Microsoft alle seguenti pagine:
     * Client - http://msdn.microsoft.com/it-it/library/fx6588te(v=vs.110).aspx
     * Server - http://msdn.microsoft.com/it-it/library/5w7b7x5f(v=vs.110).aspx
     */

    class SocketManager
    {
        // Classe per mantenere il riferimento ad ogni dato ricevuto
        public class StateObject
        {
            public Socket Sock = null;                      // Socket di riferimento
            public const int BufferSize = 1024;             // Dimensione del buffer di ricezione
            public byte[] buffer = new byte[BufferSize];    // Buffer di ricezione
            public string str = "";                         // Stringa di dati ricevuti (Sarebbe meglio StringBuilder)
        }

        // Evento per la ricezione di un messaggio
        public delegate void MessaggioRicevuto(object sender, InterLevelArgs args);
        public event MessaggioRicevuto OnMessaggioRicevuto;

        // Semafori Client
        ManualResetEvent connessioneRiuscita = new ManualResetEvent(false);

        // Metodo di connessione per il client
        public void Connetti(Identity Server, IPEndPoint ipAddress)
        {
            Server.Sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Server.Sock.BeginConnect(ipAddress,
                new AsyncCallback(Connesso), Server); 
            
            //Aspetta che la connessione sia avvenuta
            connessioneRiuscita.WaitOne();
        }

        // Callback di avvenuta connessione per il client
        private void Connesso(IAsyncResult ar)
        {
            try
            {
                Identity dest = (Identity)ar.AsyncState;

                // Connessione conclusa
                dest.Sock.EndConnect(ar);

                //Avviso di debug
                Debug.Log(String.Format("Connesso con successo a {0}!", dest.Sock.RemoteEndPoint.ToString()), Debug.LogType.INFO);

                // Sblocca il Thread principale
                connessioneRiuscita.Set();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString(), Debug.LogType.ERROR);
            }
        }

        // Metodo standard per l'invio di Dati ad un destinatario
        public void InviaMessaggio(string Dati, Identity Destinatario)
        {
            // Converte la stringa in formata Unicode
            byte[] byteData = Encoding.Unicode.GetBytes(Dati + "<eof>");

            // Inizia ad inviare i dati ad il dispositivo connesso
            Destinatario.Sock.BeginSend(byteData, 0, byteData.Length, SocketFlags.None,
                new AsyncCallback(Inviato), Destinatario);
        }

        // Callback per l'avennuto trasferimento dei dati
        public void Inviato(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Identity dest = (Identity)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = dest.Sock.EndSend(ar);
                Debug.Log(String.Format("Sent {0} bytes to server.", bytesSent));
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString(), Debug.LogType.ERROR);
            }
        }

    }
}
