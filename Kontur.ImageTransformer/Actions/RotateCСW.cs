using Kontur.ImageTransformer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Actions
{
    public struct RotateCСW : IImageAction
    {
        public unsafe void Apply(byte* reqStart, byte* resPixel, int x, int y, int width, int height)
        {
            var reqX = y;
            var reqY = height - x;

            var reqPixeStart = reqStart + reqY * width * 4 + reqX * 4;

            *(resPixel) = *(reqPixeStart);
            *(resPixel + 1) = *(reqPixeStart + 1);
            *(resPixel + 2) = *(reqPixeStart + 2);
            *(resPixel + 3) = *(reqPixeStart + 3);
        }
    }
}
