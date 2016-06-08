using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paletter
{
    class Program
    {
        static void Main(string[] args)
        {
            var image = new Bitmap(@"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images\middle_img_8553.jpg");
            var image2 = new Bitmap(@"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images\middle_img_8554.jpg");
            var image3 = new Bitmap(@"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images\middle_img_8555.jpg");
            var ph = new PaletteHelper();
            //ph.GetPalette(image, 5).Select(x => $"{x.R}, {x.G}, {x.B}").ToList().ForEach(Console.WriteLine);
            ph.GetColorsVector(image, image2).Select(x => x).ToList().ForEach(Console.WriteLine);
            Console.WriteLine();
            ph.GetColorsVector(image, image3).Select(x => x).ToList().ForEach(Console.WriteLine);
            Console.ReadLine();
        }
    }
}
