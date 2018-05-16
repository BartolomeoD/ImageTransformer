using Kontur.ImageTransformer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

namespace Kontur.ImageTransformer.Responses
{
    class ImageResponse : IResponse
    {
        private Bitmap b;

        public ImageResponse(Bitmap responseBitmap)
        {
            b = responseBitmap;
        }

        public void Execute(HttpListenerResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "image/png";
            b.Save(response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            response.OutputStream.Close();
            b.Dispose();
        }
    }
}
