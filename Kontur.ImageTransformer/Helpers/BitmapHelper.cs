using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Helpers
{
    public static class BitmapHelper
    {
        public static bool TryLoad(Stream input, out Bitmap b)
        {
            try
            {
                b = new Bitmap(input);
                return true;
            }
            catch
            {
                b = null;
                return false;
            }
        }
    }
}
