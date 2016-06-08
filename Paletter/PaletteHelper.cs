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

        public double[] GetColorsVector(ImageFile img1, ImageFile img2)
        {
            var p1 = GetPalette(img1.Image, 5).ToList();
            var p2 = GetPalette(img2.Image, 5).ToList();

            var colorsVector = new Dictionary<int, double>();

            for (int i = 0; i < 5; i++)
            {
                colorsVector.Add(i, 0);
                double smallest = 0;
                int nearest = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (p2[j].A != 0)
                    {
                        var p = GetDeltaE(p1[i], p2[j]);
                        if (p < smallest)
                        {
                            smallest = p;
                            nearest = j;
                        }
                    }
                }
                p2[nearest] = Color.FromArgb(0, 0, 0, 0);
                colorsVector[i] = smallest;
            }

            return colorsVector.Values.ToArray();
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
            Dictionary<Color, int> colors = new Dictionary<Color, int>();
            var bitmap = img;
            Bitmap thumbBmp =
            new Bitmap(bitmap.GetThumbnailImage(thumbSize, thumbSize, ThumbnailCallback, IntPtr.Zero));

            //just for test
            //pictureBox2.Image = thumbBmp;

            for (int i = 0; i < thumbSize; i++)
            {
                for (int j = 0; j < thumbSize; j++)
                {
                    Color col = thumbBmp.GetPixel(i, j);
                    if (colors.ContainsKey(col))
                        colors[col]++;
                    else
                        colors.Add(col, 1);
                }
            }

            var keyValueList = new List<KeyValuePair<Color, int>>(colors);

            keyValueList.Sort(
                delegate (KeyValuePair<Color, int> firstPair,
                KeyValuePair<Color, int> nextPair)
                {
                    return nextPair.Value.CompareTo(firstPair.Value);
                });

            var top = keyValueList.Take(colorsNumber);
            return top.Select(t => t.Key);
        }

        public bool ThumbnailCallback() { return false; }
    }
}
