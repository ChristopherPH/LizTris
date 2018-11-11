using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liztris
{
    public static class IntExtensions
    {
        public static int GetHeight(this int[,] intArray)
        {
            return intArray.GetLength(0);
        }

        public static int GetWidth(this int[,] intArray)
        {
            return intArray.GetLength(1);
        }
    }
}
