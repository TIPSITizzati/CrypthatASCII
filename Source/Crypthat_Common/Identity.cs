using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.IO.Ports;

namespace Crypthat_Common
{
    /*
     * Classe contenente i dati di riferimento dell'utente
     * Da utilizzare sempre per identificare una persona
     * 
     */
    public class Identity
    {
        public string Name { get; set; } //Proprietà contenente il nome dichiarato dall'utente
        public string SessionKey { get; set; } //Chiave generata random per identificare univocamente un utente (Può essere la chiave pubblica)

        #region Connessione

        //RS-232
        public SerialPort serialPort { get; set; } //Variabile utilizzata dal server per definire la porta di comunicazione con quella persona (Può essere NULL)

        //Sockets
        public IPEndPoint Address { get; set; } //Variabile utilizzata dai socket per identificare la persona (può essere NULL)
        public Socket Sock { get; set; } //Socket per comunicare con la persona (può essere NULL)

        #endregion

        //Costruttore
        public Identity(string Name, string SessionKey)
        {
            this.Name = Name;
            this.SessionKey = SessionKey;
        }



        //Metodi Utili
        public static Identity TrovaPerNome(List<Identity> Destinatari, string Nome)
        {
            foreach (Identity i in Destinatari)
                if (i.Name == Nome)
                    return i;
            return null;
        }

        public static Identity TrovaPerSessionKey(List<Identity> Destinatari, string SessionKey)
        {
            foreach (Identity i in Destinatari)
                if (i.SessionKey == SessionKey)
                    return i;
            return null;
        }
    }
}
