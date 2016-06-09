using Newtonsoft.Json;
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
            var ph = new PaletteHelper(10);

            var ordered = new DirectoryInfo("../../Images/ordered/");
            
            foreach (FileInfo file in ordered.GetFiles())
            {
                file.Delete();
            }

            var files = Directory.GetFiles("../../Images/photos/");

            var trios = new List<ImageFile>();

            var orderedData = new List<dynamic>();

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
            current.OriginalImage.Save("../../Images/ordered/" + 0 + ".jpg");
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

                orderedData.Add(new {
                    img1Location = current.Location,
                    img1Palette = current.Palette,
                    img2Location = nearest.Location,
                    img2Palette = nearest.Palette,
                    distance = shortestDistance
                });

                Console.WriteLine(current.Location);
                Console.WriteLine(nearest.Location);
                Console.WriteLine(shortestDistance);
                nearest.OriginalImage.Save(@"../../Images/ordered/" + (i) + ".jpg");
                i++;
                trios.Remove(nearest);
                current = nearest;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            var json = File.CreateText(@"../../Images/ordered/data.json");
            json.Write(JsonConvert.SerializeObject(orderedData));
            json.Close();

            Console.WriteLine("sorted");

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
