using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Abstractions
{
    interface IImageAction
    {
        IResponse Apply(byte[] imagePixels);
    }
}
