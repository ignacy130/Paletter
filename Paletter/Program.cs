using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paletter
{
    class Program
    {
        public static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        [STAThread]
        static void Main(string[] args)
        {
            var ph = new PaletteHelper();

            var ordered = new DirectoryInfo(@"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images\ordered\");

            foreach (FileInfo file in ordered.GetFiles())
            {
                file.Delete();
            }

            var files = Directory.GetFiles(@"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images\photos\");

            var trios = new List<ImageFile>();

            var imferator = 0;
            foreach (var item in files)
            {
                var img = new Bitmap(item);
                var resized = ResizeImage(img, img.Width / 5, img.Height / 5);
                var imf = new ImageFile()
                {
                    Image = resized,
                    OriginalImage = img,
                    Location = item,
                    Palette = ph.GetPalette(resized, 5).ToList()
                };

                Console.WriteLine("got file #" + imferator);
                imferator++;
                trios.Add(imf);
            }

            Console.WriteLine("files to imfs");
            var current = trios[0];
            current.OriginalImage.Save(@"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images\ordered\" + 0 + ".jpg");
            trios.Remove(current);
            var i = 1;
            while(trios.Count>0)
            {
                ImageFile nearest = null;
                var shortestDistance = double.MaxValue;
                foreach (var item in trios)
                {
                    var dist = MathHelpers.GetVectorLength(ph.GetColorsVector(current.Palette, item.Palette));
                    if(dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        nearest = item;
                    }
                }

                nearest.OriginalImage.Save(@"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images\ordered\" + (i) + ".jpg");
                i++;
                trios.Remove(nearest);
                current = nearest;
            }


            Console.WriteLine("sorted");

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
