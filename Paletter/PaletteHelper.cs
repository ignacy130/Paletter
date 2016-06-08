using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paletter
{
   public class ImageFile
    {
        public Image Image { get; set; }

        public string Location { get; set; }
    }

    public class PaletteHelper
    {
        public Dictionary<ImageFile, ImageFile> ImagePairs { get; set; }

        public double[] GetColorsVector(Image img1, Image img2)
        {
            var palette1 = GetPalette(img1, 5).ToList();
            var palette2 = GetPalette(img2, 5).ToList();

            var colorsVector = new Dictionary<int, double>();

            Console.WriteLine("p1 palette");
            palette1.Select(x => $"{x.R}, {x.G}, {x.B}").ToList().ForEach(Console.WriteLine);
            Console.WriteLine();
            Console.WriteLine("p2 palette");
            palette2.Select(x => $"{x.R}, {x.G}, {x.B}").ToList().ForEach(Console.WriteLine);
            Console.WriteLine();
            for (int i = 0; i < 5; i++)
            {
                colorsVector.Add(i, 0);
                
                colorsVector[i] = GetSmallestDifference(palette1[i], palette2);
            }

            return colorsVector.Values.ToArray();
        }

        private double GetSmallestDifference(Color color, List<Color> palette)
        {
            double smallest = 0;
            int nearest = 0;
            for (int j = 0; j < 5; j++)
            {
                if (palette[j].A != 0)
                {
                    var p = GetDeltaE(color, palette[j]);
                    if (j == 0 || p < smallest)
                    {
                        smallest = p;
                        nearest = j;
                    }
                }
            }
            return smallest;
        }

        public double GetDeltaE(Color c1, Color c2)
        {
            var lab1 = ColorHelpers.RGBToLab(c1.R, c1.G, c1.B);
            var lab2 = ColorHelpers.RGBToLab(c2.R, c2.G, c2.B);

            var deltaE = Math.Sqrt(
                Math.Pow(lab1.L - lab2.L, 2) +
                Math.Pow(lab1.A - lab2.A, 2) +
                Math.Pow(lab1.B - lab2.B, 2)
            );

            return deltaE;
        }

        public void GetPalettes(string dir = @"C:\D\Projekty\Aplikacje\StyleArt\Paletter\Images")
        {
            var images = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).ToList();
            var b = Image.FromFile(images[0]);
        }

        public IEnumerable<Color> GetPalette(Image img, int colorsNumber)
        {
            int thumbSize = 32;
            var thumbBmp = new Bitmap(img.GetThumbnailImage(thumbSize, thumbSize, ThumbnailCallback, IntPtr.Zero));
            var imgColors = GetHistogram(thumbSize, thumbBmp);

            var keyValueList = new List<KeyValuePair<Color, int>>(imgColors);

            keyValueList.Sort(
                delegate (KeyValuePair<Color, int> firstPair, KeyValuePair<Color, int> nextPair)
                {
                    return nextPair.Value.CompareTo(firstPair.Value);
                }
            );

            var top = keyValueList.Take(colorsNumber);
            return top.Select(t => t.Key);
        }

        private Dictionary<Color, int> GetHistogram(int thumbSize, Bitmap thumbBmp)
        {
            var imgColors = new Dictionary<Color, int>();
            for (int i = 0; i < thumbSize; i++)
            {
                for (int j = 0; j < thumbSize; j++)
                {
                    var color = thumbBmp.GetPixel(i, j);
                    if (imgColors.ContainsKey(color))
                    {
                        imgColors[color]++;
                    }
                    else
                    {
                        imgColors.Add(color, 1);
                    }
                }
            }
            return imgColors;
        }

        public bool ThumbnailCallback() { return false; }
    }
}
