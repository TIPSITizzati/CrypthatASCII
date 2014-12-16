using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.IO;

namespace Crypthat_Common.Crittografia
{
    /* Classe per la crittografia simmetrica AES basata sulla documentazione Microsoft:
     * http://msdn.microsoft.com/it-it/library/system.security.cryptography.aes(v=vs.110).aspx
     */
    public class AESCipher
    {
        public static byte[][] Encrypt(string Message)
        {
            // Genera le chiavi simmetriche random e cripta il testo
            byte[][] AES_DATA = new byte[3][];
            AES_DATA[0] = new byte[128 / 8];
            AES_DATA[1] = new byte[128 / 8];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(AES_DATA[0]);  // Genera la chiave AES a 128bit
            rng.GetBytes(AES_DATA[1]);  // Genera le IV AES a 128bit

            // Crea un oggetto AES per la cifratura
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = AES_DATA[0];
                aesAlg.IV = AES_DATA[1];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Crea le stream per la codifica del messaggio
                using (MemoryStream msEncrypt = new MemoryStream()) // Stream in memoria, per velocizzare l'operazione
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) // Stream che applica la crittografia AES
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))    // Oggetto standard StreamWriter per la scrittura nello stream
                        {
                            // Scrive tutti i dati del messaggio
                            swEncrypt.Write(Message);
                        }

                        // Riceve i dati cifrati
                        AES_DATA[2] = msEncrypt.ToArray();
                    }
                }
            }

            return AES_DATA;
        }

        // Basata sulla documentazione microsoft
        public static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Controlla che i parametri inseriti siano validi
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // plainText conterrà il messaggio decifrato
            string plaintext = "";

            // Crea un oggetto Aes con le chiavi specificate
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Crea un oggetto decryptor per decifrare il messaggio
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
                // Crea le stream per la codifica del messaggio
                using (MemoryStream msDecrypt = new MemoryStream(cipherText)) // Stream in memoria, per velocizzare l'operazione
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) // Stream che decifra i dati letti
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))    // Oggetto standard StreamReader per la lettura dello stream
                        {
                            // Legge la serie di byte decifrati che indica il messaggio e la aggiunge al testo
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
