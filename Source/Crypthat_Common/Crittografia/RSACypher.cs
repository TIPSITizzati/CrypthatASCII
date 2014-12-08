using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

using System.Threading;

namespace Crypthat_Common.Crittografia
{
    public class RSACypher
    {
        // Metodo molto debole che usa poche cifre
        public static RSAContainer GenerateKeyPair(int Length)
        {
            Debug.Log("Generazione di una nuova coppia di chiavi asimmetriche...", Debug.LogType.WARNING);
            //Genera due numeri "primi" P, Q (Per motivi di tempo verranno utilizzati dei numeri completamente casuali)
            BigInteger p = GetProbablePrime(Length / 2);
            BigInteger q = GetProbablePrime(Length / 2);

            // Genera il modulo ed il phi(modulo)
            BigInteger n = p * q;
            BigInteger v = (p - 1) * (q - 1);

            // Ottiene i valori della chiave pubblica(il numero dispari per cui mcd(e,v) = 1 ) e la chiave privata (l'inverso nel modulo tra e,v)
            BigInteger e = GetSmallOddInteger(v);
            BigInteger d = ModularInverse(e, v);

            // Ritorna i valori in un apposito container
            RSAContainer Container = new RSAContainer();
            Container.PrivateKey = String.Format("{0},{1}", e.ToString(), n.ToString());
            Container.PublicKey = String.Format("{0},{1}", d.ToString(), n.ToString());

            return Container;
        }

        // Restituisce un array di byte sempre pari (Unicode usa 2 byte per carattere)
        public static byte[] EncryptDecrypt(byte[] Message, string Key)
        {
            BigInteger C = new BigInteger(Message);
            BigInteger D = BigInteger.Parse(Key.Split(',')[0]);
            BigInteger N = BigInteger.Parse(Key.Split(',')[1]);

            return BigInteger.ModPow(C, D, N).ToByteArray();
        }

        // Ritorna un numero grande non divisibile per i primi 65000 numeri primi
        public static BigInteger GetProbablePrime(int Length)
        {
            // Genera un numero casuale di lungezza richiesta
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[Length / 8];

            BigInteger number;
            do
            {
                rng.GetBytes(bytes);
                number = BigInteger.Abs(new BigInteger(bytes));
            } while (!IsProbabilyPrime(number, 1)); // Continua a generare finchè non è un numero pseudo-primo

            return number;
        }

        public static bool IsProbabilyPrime(BigInteger n, int k)
        {
            bool result = false;
            if (n < 2)
                return false;
            if (n == 2)
                return true;
            // return false if n is even -> divisbla by 2
            if (n % 2 == 0)
                return false;
            //writing n-1 as 2^s.d
            BigInteger d = n - 1;
            BigInteger s = 0;
            while (d % 2 == 0)
            {
                d >>= 1;
                s = s + 1;
            }
            for (int i = 0; i < k; i++)
            {
                BigInteger a;
                do
                {
                    a = RandomIntegerBelow(n - 2);
                }
                while (a < 2 || a >= n - 2);

                if (BigInteger.ModPow(a, d, n) == 1) return true;
                for (int j = 0; j < s - 1; j++)
                {
                    if (BigInteger.ModPow(a, 2 * j * d, n) == n - 1)
                        return true;
                }
                result = false;
            }
            return result;
        }

        public static BigInteger RandomIntegerBelow(BigInteger bound)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //Get a byte buffer capable of holding any value below the bound
            byte[] buffer = (bound << 16).ToByteArray(); // << 16 adds two bytes, which decrease the chance of a retry later on

            //Compute where the last partial fragment starts, in order to retry if we end up in it
            BigInteger generatedValueBound = BigInteger.One << (buffer.Length * 8 - 1); //-1 accounts for the sign bit
            BigInteger validityBound = generatedValueBound - generatedValueBound % bound;

            while (true)
            {
                //generate a uniformly random value in [0, 2^(buffer.Length * 8 - 1))
                rng.GetBytes(buffer);
                BigInteger r = BigInteger.Abs(new BigInteger(buffer));

                //return unless in the partial fragment
                if (r >= validityBound) continue;
                return r % bound;
            }
        }

        // Ritorna il numero dispari più piccolo (i) per il quale il minimo comune divisore tra i ed Number è uguale a 1
        public static BigInteger GetSmallOddInteger(BigInteger Number)
        {
            for (BigInteger i = 3; i < Number; i += 2)
                if (BigInteger.GreatestCommonDivisor(i, Number) == 1)
                    return i;
            return 3;
        }

        public static BigInteger ModularInverse(BigInteger a, BigInteger b)
        {
            BigInteger inv, u1, u3, v1, v3, t1, t3, q, iter;
            /* Step X1. Initialise */
            u1 = 1;
            u3 = a;
            v1 = 0;
            v3 = b;
            /* Remember odd/even iterations */
            iter = 1;
            /* Step X2. Loop while v3 != 0 */
            while (v3 != 0)
            {
                /* Step X3. Divide and "Subtract" */
                q = u3 / v3;
                t3 = u3 % v3;
                t1 = u1 + q * v1;
                /* Swap */
                u1 = v1; v1 = t1; u3 = v3; v3 = t3;
                iter = -iter;
            }
            /* Make sure u3 = gcd(u,v) == 1 */
            if (u3 != 1)
                return 0;   /* Error: No inverse exists */
            /* Ensure a positive result */
            if (iter < 0)
                inv = b - u1;
            else
                inv = u1;
            return inv;
        }
    
        // Servizio in background che gestisce la generazione delle chiavi
        public class RSACryptoService
        {
            public Thread RefreshThread;

            public delegate void RSACallBack(RSAContainer newKeys);
            public event RSACallBack NewKeyPair;

            public void Start(int Timer)
            {
                this.RefreshThread = new Thread(() => ManageKeys(Timer));
                this.RefreshThread.IsBackground = true;
                this.RefreshThread.Start();
            }

            public void Stop()
            {
                if(this.RefreshThread.IsAlive)
                {
                    this.RefreshThread.Abort();
                }
            }

            public void ManageKeys(int Timer)
            {
                while(true)
                {
                    DateTime nextAlarm = DateTime.Now.AddSeconds(Timer);
                    while (DateTime.Now < nextAlarm) { }    //Attende che siano trascorsi i secondi richiesti
                    if(NewKeyPair != null)
                        NewKeyPair(GenerateKeyPair(512));
                }
            }
        }
    }
}
