using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace P3DStreamer.Extensions
{
    public static class ImageExtensions
    {
        public static byte[] ToByteArray(this Bitmap image, int Quality = 50)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                 
                if (ms.Length > 65535 - 100)  // 0x10000 max size udp ?
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.SetLength(0);

                    var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    var encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(Encoder.Quality, (long)Quality) } };
                    image.Save(ms, encoder, encParams);
                }

                return ms.ToArray();
            }
        }

        static public Bitmap Copy(Bitmap srcBitmap, Rectangle section)
        {
            var bmp = new Bitmap(section.Width, section.Height);
            var g = Graphics.FromImage(bmp);

            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);
            g.Dispose();

            return bmp;
        }
    }
}
