using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Abstractions
{
    public  interface IImageAction
    {
        unsafe void  Apply(byte* reqStart, byte* resPixel, int x, int y, int width, int height);
    }
}
