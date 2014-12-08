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
    public class AESCypher
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

                // Crea uno stream per la codifica del messaggio
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
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

        public static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
