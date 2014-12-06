using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

namespace Crypthat_Common.Crittografia
{
    class RSACypher
    {
        // Metodo molto debole che usa poche cifre
        public static BigInteger GenerateKeyPair(int Length)
        {
            //Genera due numeri "primi" P, Q (Per motivi di tempo verranno utilizzati dei numeri completamente casuali)
            BigInteger p = GetProbablePrime(Length / 2);
            Debug.Log(String.Format("Generated P : {0}", p.ToString()));
            BigInteger q = GetProbablePrime(Length / 2);
            Debug.Log(String.Format("Generated Q : {0}", q.ToString()));

            BigInteger n = p * q;
            Debug.Log(String.Format("Generated N : {0}", n.ToString()));
            BigInteger v = (p - 1) * (q - 1);
            Debug.Log(String.Format("Generated V : {0}", v.ToString()));

            BigInteger e = GetSmallOddInteger(v);
            Debug.Log(String.Format("Generated E : {0}", e.ToString()));

            BigInteger d = ModularInverse(e, v);
            Debug.Log(String.Format("Generated D : {0}", d.ToString()));

            BigInteger M = new BigInteger(Encoding.Unicode.GetBytes(Console.ReadLine()));
            Debug.Log(String.Format("Message is : {0}", M.ToString()));

            BigInteger C = BigInteger.ModPow(M, e, n);
            Debug.Log(String.Format("Generated Chyper : {0}", C.ToString()));

            BigInteger BM = BigInteger.ModPow(C, d, n);
            Debug.Log(String.Format("Original Message value is : {0}", BM.ToString()));
            Debug.Log(String.Format("Original Message is : {0}", Encoding.Unicode.GetString(BM.ToByteArray())));

            return 0;
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

        // Ritorna il numero dispari più piccolo (i) per il quale il minimo comune divisore tra i ed Number
        // è uguale a 1
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
    }
}
