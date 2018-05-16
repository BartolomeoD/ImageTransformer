using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Extensions
{
    public static class BitmapExtension
    {
        public static Rectangle getRectangle(this Bitmap bitmap)
        {
            return new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        } 
    }
}
