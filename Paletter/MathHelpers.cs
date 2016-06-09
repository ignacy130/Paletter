using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paletter
{
    public class MathHelpers
    {
        public static double GetVectorLength(double[] vector)
        {
            var squaresSum = vector.Select(x => x * x).Sum();
            var result = Math.Sqrt(squaresSum);
            return result;
        }
    }
}
