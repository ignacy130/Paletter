using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paletter
{
   public class ImageFile
    {
        public Bitmap Image { get; set; }

        public Bitmap OriginalImage { get; set; }

        public string Location { get; set; }

        public List<Color> Palette { get; set; }
    }

    public class PaletteHelper
    {
        public Dictionary<ImageFile, ImageFile> ImagePairs { get; set; }

        public double[] GetColorsVector(List<Color> palette1, List<Color> palette2)
        {
            var colorsVector = new double[5];

            for (int i = 0; i < Math.Min(palette1.Count, palette2.Count); i++)
            {
                colorsVector[i] = GetSmallestDifference(palette1[i], palette2);
            }

            Console.WriteLine(MathHelpers.GetVectorLength(colorsVector));

            return colorsVector.ToArray();
        }

        private double GetSmallestDifference(Color color, List<Color> palette)
        {
            double smallest = 0;
            int nearest = 0;
            for (int j = 0; j < palette.Count; j++)
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

        public IEnumerable<Color> GetPalette(Bitmap img, int colorsNumber)
        {
            var imgColors = GetHistogram(img);

            var keyValueList = new List<KeyValuePair<Color, int>>(imgColors);

            keyValueList.Sort(
                delegate (KeyValuePair<Color, int> firstPair, KeyValuePair<Color, int> nextPair)
                {
                    return nextPair.Value.CompareTo(firstPair.Value);
                }
            );

            var top = keyValueList;
            return top.Select(t => t.Key);
        }

        private Dictionary<Color, int> GetHistogram(Bitmap thumbBmp)
        {
            var imgColors = new Dictionary<Color, int>();
            for (int i = 0; i < thumbBmp.Width; i++)
            {
                for (int j = 0; j < thumbBmp.Height; j++)
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

            
            var sorted = from pair in imgColors
                         orderby pair.Value descending
                         select pair;
            var dict = sorted.ToDictionary(t => t.Key, v => v.Value);
            foreach (var item in dict.Select(x=>x.Key).ToList())
            {
                var similarColors = dict.Where(c => GetDeltaE(item, c.Key) < 13).ToList();
                foreach (var similar in similarColors.Skip(1))
                {
                    dict.Remove(similar.Key);
                }
            }

            return dict.Take(5).ToDictionary(t => t.Key, v => v.Value);
        }

        public bool ThumbnailCallback() { return false; }
    }
}
