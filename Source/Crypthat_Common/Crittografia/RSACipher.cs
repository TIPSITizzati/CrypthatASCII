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
    public class RSACipher
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
            Container.PrivateKey = String.Format("{0},{1}", d.ToString(), n.ToString());
            Container.PublicKey = String.Format("{0},{1}", e.ToString(), n.ToString());

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

        // Ritorna un numero grande che è probabilmente primo
        public static BigInteger GetProbablePrime(int Length)
        {
            // Genera un numero casuale di lungezza richiesta (RNGCryptoServiceProvider fornisce una serie sicura di byte random)
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

        // Metodo che applica l'algoritmo di Miller–Rabin
        public static bool IsProbabilyPrime(BigInteger n, int k)
        {
            // Gestisce i valori ovvi
            if (n < 2)
                return false;
            if (n == 2)
                return true;

            // Se il numero è pari, allora non è primo
            if (n.IsEven)
                return false;

            // Restituisce un numero che rappresenta n - 1 come (2^s)*d con d dispari
            BigInteger d = BigInteger.Subtract(n, BigInteger.One);
            BigInteger s = 0;
            while (d % 2 == 0)
            {
                d >>= 1; // Divisione veloce per 2 (Utilizza lo shift)
                s = BigInteger.Add(s, BigInteger.One);
            }

            // Per tutti i tentativi
            for (int i = 0; i < k; i++)
            {
                // Genera un numero a compreso tra [2, n - 2]
                BigInteger a;
                do
                {
                    a = RandomIntegerBelow(n - 2);
                }
                while (a < 2 || a >= n - 2);

                // Se (a^d)%n è 1 allora probabilmente il numero è primo
                if (BigInteger.ModPow(a, d, n) == 1) return true;

                // Finchè r < s-1
                for (int r = 0; r < s - 1; r++)
                {
                    // Se il [a^(2*r*d)]%n è uguale a n-1 allora probabilmente n è un numero primo
                    if (BigInteger.ModPow(a, 2 * r * d, n) == n - 1)
                        return true;
                }
            } 
            return false;
        }

        // Genera un numero intero sotto un determinato numero (Essendo ad infinite cifre non è possibile usare la funzione random)
        public static BigInteger RandomIntegerBelow(BigInteger bound)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            // Crea un buffer in grado di contenere un numero minore al numero dato
            byte[] buffer = (bound << 16).ToByteArray(); // << 16 aggiunge 2 byte, è un trucco per ridurre i tentativi successivi

            // Calcola dove era arrivato l'ultimo frammento parziale inizia (oltre il valore bound)
            BigInteger generatedValueBound = BigInteger.One << (buffer.Length * 8 - 1); //-1 modifica il bit del segno
            BigInteger validityBound = generatedValueBound - generatedValueBound % bound;

            while (true)
            {
                // Genera un valore random tra [0, 2^(buffer.Length * 8 - 1))
                rng.GetBytes(buffer);
                BigInteger r = BigInteger.Abs(new BigInteger(buffer));

                // Ritorna il valore nel caso non si è nel frammento parziale
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

        // Applica il Teorema di Esteso di Euclide (Massimo Comune Divisore esteso) per calcolare il modulo inverso
        public static BigInteger ModularInverse(BigInteger a, BigInteger b)
        {
            // Dichiarazione ed inizializzazione dei valori utilizzati
            BigInteger inv, lastY, newA, lastX, newB, x, modulus, q, iter;
            lastY = 1;
            newA = a;   // Per evitare di modificare a
            lastX = 0;
            newB = b;   // Per evitare di modificare b
            iter = 1;   // Utilizzato per ricordare le iterazione pari o dispari

            // Finche b != 0 (nel nostro caso newB)
            while (newB != 0)
            {
                q = newA / newB;        // Divide A/B (Quoziente)
                modulus = newA % newB;  // Ottiene il modulo (Resto)
                x = lastY + q * lastX;  // Calcola il nuovo X
                
                // Inverte i valori
                lastY = lastX; lastX = x; newA = newB; newB = modulus;
                iter = -iter;
            }

            // Si assicura che newA = gcd(u,v) == 1
            if (newA != 1)
                return 0;   // Se non è così non esiste l'inverso

            // Si assicura che il risultato sia positivo
            if (iter < 0)
                inv = b - lastY;
            else
                inv = lastY;
            return inv;
        }
    
        // Servizio in background che gestisce la generazione delle chiavi
        public class RSACryptoService
        {
            // Thread utilizzato come timer
            public Thread RefreshThread;

            // Evento chiamato al cambio di chiavi
            public delegate void RSACallBack(RSAContainer newKeys);
            public event RSACallBack NewKeyPair;

            // Avvia il timer al numero di secondi specificato
            public void Start(int Timer)
            {
                this.RefreshThread = new Thread(() => ManageKeys(Timer));
                this.RefreshThread.IsBackground = true;
                this.RefreshThread.Start();
            }

            // Se il timer è attivo, lo killa
            public void Stop()
            {
                if(this.RefreshThread.IsAlive)
                {
                    this.RefreshThread.Abort();
                }
            }

            // Loop del thread che genera le chiavi ogni X secondi
            public void ManageKeys(int Timer)
            {
                while(true)
                {
                    // Genera la data da cui può iniziare a generare le chiavi (Ora + Timer secondi)
                    DateTime nextAlarm = DateTime.Now.AddSeconds(Timer);

                    while (DateTime.Now < nextAlarm) { }    //Attende che siano trascorsi i secondi richiesti
                    
                    // Se è già connesso alla rete (quindi è presente almeno una chiave), genera la nuova coppia di chiavi
                    if(NewKeyPair != null)
                        NewKeyPair(GenerateKeyPair(512));
                }
            }
        }
    }
}
