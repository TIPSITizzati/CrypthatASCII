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
            GestoreLogicoServer gestLogico = new GestoreLogicoServer(Crypthat_Common.ModalitaOperativa.Rs232);
            while (true)
            {

            }
        }
    }
}
