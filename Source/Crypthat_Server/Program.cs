using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypthat_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // In partenza il server inizializza subito un GestoreLogicoServer per mettersi in ascolto per eventuali connessioni
            GestoreLogicoServer gestLogico = new GestoreLogicoServer(Crypthat_Common.ModalitaOperativa.Sockets);
            while (true)
            {
                //Ciclo infinito per mantenere il server in ascolto
            }
            Console.ReadKey();
        }
    }
}
