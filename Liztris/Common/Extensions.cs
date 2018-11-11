using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
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

    public static class RectExtensions
    {
        public static Vector2 GetCenter(this Rectangle rect)
        {
            return new Vector2(rect.Left + rect.Width / 2,
                             rect.Top + rect.Height / 2);
        }
    }
}
