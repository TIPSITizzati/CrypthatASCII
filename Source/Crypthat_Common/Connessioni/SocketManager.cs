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

    public class SocketManager
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

        ManualResetEvent tuttoPronto = new ManualResetEvent(false);

        // Metodi per l'inizializzazione del client
        #region InizializzazioneClient

            // Metodo di connessione per il client
            public void Connetti(Identity Server, IPEndPoint ipAddress)
            {
                Server.Sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
                Server.Sock.BeginConnect(ipAddress,
                    new AsyncCallback(Connesso), Server); 
            
                //Aspetta che la connessione sia avvenuta
                tuttoPronto.WaitOne();
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
                    tuttoPronto.Set();
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString(), Debug.LogType.ERROR);
                }
            }

        #endregion

        // Metodi per l'inizializzazione del server
        #region InizializzazioneServer

            // Metodo per impostare il server in modalità ascolto
            public void Ascolta()
            {
                //Ascolta su ogni IP disponibile
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 11000);

                Debug.Log(String.Format("Ascolto all'endpoint : {0}", localEP.ToString()));

                Socket listener = new Socket(localEP.Address.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    listener.Bind(localEP);
                    listener.Listen(10);

                    while (true)
                    {
                        tuttoPronto.Reset();

                        Debug.Log("Attendendo connessione...");
                        listener.BeginAccept(
                            new AsyncCallback(ConnessioneAccettata),
                            listener);

                        tuttoPronto.WaitOne();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString(), Debug.LogType.ERROR);
                }

                Console.WriteLine("Chiusura del listener...");
            }

            // Callback chiamato una colta che un client si è connesso
            public void ConnessioneAccettata(IAsyncResult ar)
            {
                // Ottiene il socket che si occupa della ricezione
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Segnala all thread in ascolto di continuare ad ascoltare
                tuttoPronto.Set();

                Debug.Log("Accettata connessione da: " + handler.RemoteEndPoint.ToString());

                // Crea l'oggetto per gestire la connessione
                StateObject state = new StateObject();
                state.Sock = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(RicevutoMessaggio), state);
            }

        #endregion


        // Metodo standard per l'invio di Dati ad un destinatario
        public void InviaMessaggio(string Dati, Identity Destinatario)
        {
            // Converte la stringa in formata Unicode
            byte[] byteData = Encoding.Unicode.GetBytes(Dati.Replace("<", "<\\") + "<eof>");

            // Inizia ad inviare i dati ad il dispositivo connesso
            Destinatario.Sock.BeginSend(byteData, 0, byteData.Length, SocketFlags.None,
                new AsyncCallback(Inviato), Destinatario);
        }

        // Callback per l'avennuto trasferimento dei dati
        public void Inviato(IAsyncResult ar)
        {
            try
            {
                // Ottiene l'identity dall'AsyncState
                Identity dest = (Identity)ar.AsyncState;

                // Invia i dati all'host remoto
                int bytesSent = dest.Sock.EndSend(ar);
                Debug.Log(String.Format("Inviati {0} a {1}.", bytesSent, dest.Sock.RemoteEndPoint.ToString()));
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString(), Debug.LogType.ERROR);
            }
        }

        // Metodo per mettere in ascolto il client di possibili richieste
        public void RiceviMessaggio(Identity dest)
        {
            try
            {
                // Crea un oggetto temporaneo per gestire la connessione tra gli host
                StateObject state = new StateObject();
                state.Sock = dest.Sock;

                // Inizia a ricevere i dati dall'host remoto
                dest.Sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(RicevutoMessaggio), state);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString(), Debug.LogType.ERROR);
            }
        }

        // Callback chiamato alla ricezione di dati
        private void RicevutoMessaggio(IAsyncResult ar)
        {
            try
            {
                // Riceve l'oggetto per gestire la connessione (creato in precedenza)
                // ed il Socket da cui arriva.
                StateObject state = (StateObject)ar.AsyncState;
                Socket from = state.Sock;
                // Legge i dati nel buffer dei dati ricevuti
                int bytesRead = from.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // Siccome ci possono essere ancora dati da leggere, memorizza quelli ricevuti fino ad ora
                    state.str += Encoding.Unicode.GetString(state.buffer, 0, bytesRead);
                    //  e riceve i restanti.
                    from.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(RicevutoMessaggio), state);

                    // Una volta letti tutti i dati, controlla che sia finito il comando e chiama l'evento di ricezione messaggio
                    if (state.str.Length > 1 && state.str.Contains("<eof>"))
                    {
                        if (OnMessaggioRicevuto != null)
                            OnMessaggioRicevuto(state, new InterLevelArgs(null, state.str.Replace("<eof>", "\n").Replace("<\\", "<")));
                        else
                            throw new Exception("Evento di ricezione messaggio non impostato!");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
