using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypthat_Common
{
    //Enum utilizzato per distinguere tra l'utilizzo di socket o Rs232
    public enum ModalitaOperativa
    {
        Rs232,
        Sockets
    }

    //Oggetto utilizzato per la comunicazione tra strati di astrazione
    public class InterLevelArgs
    {
        public Identity Subject { get; set; }   //Mittente
        public object Data { get; set; }        //Dati da condividere

        public InterLevelArgs(Identity Subject, object Data)
        {
            this.Subject = Subject;
            this.Data = Data;
        }
    }

    // Delegate per la ricezione di un messaggio
    public delegate void MessaggioRicevuto(object sender, InterLevelArgs args);

    // Interfaccia per unire alcuni metodi comuni
    public interface ConnectionInterface
    {
        event MessaggioRicevuto OnMessaggioRicevuto;

        void InviaMessaggio(string Dati, Identity Destinatario);
    }
}
