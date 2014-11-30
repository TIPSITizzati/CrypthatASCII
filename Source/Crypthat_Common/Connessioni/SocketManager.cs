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
     * i messaggi interi.
     */

    class SocketManager
    {
        //Evento per la ricezione di un messaggio
        public delegate void MessaggioRicevuto(object sender, InterLevelArgs args);
        public event MessaggioRicevuto OnMessaggioRicevuto;

        // Metodo standard per l'invio di Dati ad un destinatario
        public void InviaMessaggio(string Dati, Identity Destinatario)
        {

        }

        public void Connetti(Identity Server, IPEndPoint ipAddress)
        {
            Server.Sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Server.Sock.BeginConnect(ipAddress, 
                new AsyncCallback(Connesso), Server);

        }

        private void Connesso(IAsyncResult ar)
        {
            try
            {
               
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString(), Debug.LogType.ERROR);
            }
        }
    }
}
