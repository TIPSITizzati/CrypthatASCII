using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Crypthat_Common.Crittografia
{
    public class ASCIIArtCipher
    {
        // Metodo che utilizza le funzioni grafiche GDI per generare l'ASCII Art
        public static string GenerateASCIIArt(string Text, int fontSize)
        {
            // Stringa contenente la rappresentazione in ASCII Art del testo inserito
            string ASCII_Art = "";

            // Inizializza una immagine delle dimensioni del testo, per poi disegnarvi sopra il testo
            // (8/6) è il rapporto tra pt e px
            Bitmap bmp = new Bitmap(fontSize * Text.Length * (8 / 6), fontSize * (8 / 6));
            Graphics g = Graphics.FromImage((Image)bmp);    // Inizializza la componente grafica GDI sull'immagine
            
            // Imposta il formato con cui verrà disegnata la stringa
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.Word;

            //  Imposta i parametri grafici per la rappresentazione del testo
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Riempie l'immagine di rosso
            g.FillRectangle(Brushes.Red, 0, 0, bmp.Width, bmp.Height);

            //Disegna il testo in nero
            g.DrawString(Text, new Font("Courier", fontSize), Brushes.Black, new Rectangle(0, 0, bmp.Width, bmp.Height), format);
            g.Flush();

            //Per ogni pixel dell'immagine
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                    ASCII_Art += bmp.GetPixel(j, i).R != 255 ? "□" : "■"; // Se il colore è rosso, scrive ■ sennò scrive □

                ASCII_Art += "\n"; // Alla fine di ogni riga di pixel va a capo
            }

            return ASCII_Art;
        }
    }
}
