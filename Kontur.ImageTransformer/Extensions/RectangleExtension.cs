using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Extensions
{
    public static class RectangleExtension
    {
        public static void Allign (this Rectangle r)
        {
            if (r.Height < 0)
            {
                r.Height = -r.Height;
                r.Y = Math.Max(0, r.Y - r.Height);
            }

            if (r.Width < 0)
            {
                r.Width = -r.Width;
                r.X = Math.Max(0, r.X - r.Width);
            }

            if (r.X < 0)
            {
                r.Width += r.X;
                r.X = 0;
            }

            if (r.Y < 0)
            {
                r.Height += r.Y;
                r.Y = 0;
            }
        }
    }
}
