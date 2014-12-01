#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

#if DEBUG
    using Crypthat_Common;
    using Crypthat_Common.Connessioni;
#endif

namespace Crypthat_Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        #if !DEBUG
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        #else
            public static GestoreLogicoClient mng;

            [STAThread]
            static void Main(string[] args)
            {
                Console.Write("Inserire nome utente --> ");
                string Nome = Console.ReadLine();
                Console.Write("Inserire IP server --> ");
                string Address = Console.ReadLine();
                if (Address == "")
                    Address = "127.0.0.1";
                mng = new GestoreLogicoClient(ModalitaOperativa.Sockets, new Identity(Nome, null), new System.Net.IPEndPoint(System.Net.IPAddress.Parse(Address), 11000));
                mng.OnMessaggioRicevuto += PrintMessaggio;
                mng.OnUtenteRegistrato += mng_OnUtenteRegistrato;

                while (true)
                {
                    string msg = Console.ReadLine();

                    switch(msg)
                    {
                        case "list":
                            foreach (Identity i in mng.Destinatari)
                                Console.WriteLine(i.Name);
                            break;
                        default:
                            Identity dest = mng.TrovaPerNome(msg.Split(':')[0]);
                            if (dest != null)
                                mng.InviaMessaggio(msg.Split(':')[1], dest);
                        break;
                    }
                }
            }

            static void mng_OnUtenteRegistrato(object sender, InterLevelArgs args)
            {
                Console.WriteLine(String.Format("[REGISTRATO] - {0}", args.Subject.Name.ToString()));
            }

            public static void PrintMessaggio(object obj, InterLevelArgs args)
            {
                Console.WriteLine(String.Format("[{0}] - {1}", args.Subject.Sock.RemoteEndPoint.ToString(), args.Data.ToString()));
            }
        #endif
    }
}
