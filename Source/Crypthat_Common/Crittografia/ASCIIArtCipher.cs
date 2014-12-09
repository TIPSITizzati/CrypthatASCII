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
        public static string GenerateASCIIArt(string Text, int fontSize)
        {
            string ASCII_Art = "";

            // (8/6) è il rapporto tra pt e px
            Bitmap bmp = new Bitmap(fontSize * Text.Length * (8 / 6), fontSize * (8 / 6));
            Graphics g = Graphics.FromImage((Image)bmp);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.Word;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.FillRectangle(Brushes.Red, 0, 0, bmp.Width, bmp.Height);
            g.DrawString(Text, new Font("Courier", fontSize), Brushes.Black, new Rectangle(0, 0, bmp.Width, bmp.Height), format);
            g.Flush();

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                    ASCII_Art += bmp.GetPixel(j, i).R != 255 ? "□" : "■";

                ASCII_Art += "\n";
            }

            return ASCII_Art;
        }
    }
}
