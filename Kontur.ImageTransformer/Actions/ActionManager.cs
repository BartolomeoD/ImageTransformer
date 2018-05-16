using Kontur.ImageTransformer.Abstractions;
using Kontur.ImageTransformer.Helpers;
using Kontur.ImageTransformer.Responses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Extensions;

namespace Kontur.ImageTransformer.Actions
{
    public static class ActionManager
    {
       
        public static  IResponse ApplyAction(string actionName, Stream inputStream, Rectangle responseArea)
        {
            if (!BitmapHelper.TryLoad(inputStream, out Bitmap requestImage))
                return new StatusCode((int)HttpStatusCode.BadRequest);
            using (requestImage)
            {
                if (requestImage.PixelFormat != PixelFormat.Format32bppArgb)
                    return new StatusCode((int)HttpStatusCode.BadRequest);

                if (requestImage.Height > 1000 || requestImage.Width > 1000)
                    return new StatusCode((int)HttpStatusCode.BadRequest);

                Rectangle responseImageArea;
                if (actionName.Contains("rotate"))
                {
                    responseImageArea = new Rectangle(0, 0, requestImage.Height, requestImage.Width);
                } else
                {
                    responseImageArea = new Rectangle(0, 0, requestImage.Width, requestImage.Height);
                    
                }

                responseArea.Allign();
                responseArea.Intersect(responseImageArea);

                if (responseArea.IsEmpty)
                    return new StatusCode((int)HttpStatusCode.NoContent);

                Bitmap responseImage = new Bitmap(responseArea.Width, responseArea.Height, PixelFormat.Format32bppArgb);
                BitmapData responseData = responseImage.LockBits(
                    new Rectangle(0, 0, responseImage.Width, responseImage.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                BitmapData requestData = requestImage.LockBits(
                    new Rectangle(0, 0, requestImage.Width, requestImage.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var requestArea = requestImage.getRectangle();

                switch (actionName)
                {
                    case "rotate-ccw":
                        apply<RotateCW>(requestData, responseData, requestArea, responseArea);
                        break;
                    case "rotate-cw":
                        apply<RotateCСW>(requestData, responseData, requestArea, responseArea);
                        break;
                    case "flip-h":
                        apply<FlipH>(requestData, responseData, requestArea, responseArea);
                        break;
                    case "flip-v":
                        apply<FlipV>(requestData, responseData, requestArea, responseArea);
                        break;
                }
                requestImage.UnlockBits(requestData);
                responseImage.UnlockBits(responseData);
                return new ImageResponse(responseImage);
            }

            
        }

        private static unsafe void apply<TAction>(BitmapData requestData, BitmapData responseData, Rectangle requestArea, Rectangle  responseArea)
            where TAction : struct, IImageAction
        {
            var action = new TAction();

            byte* reqStart= (byte*)requestData.Scan0;
            byte* resStart = (byte*)responseData.Scan0;
            int lastH;
            int lastW;
            for (int y = responseArea.Y, h = 0; y < responseArea.Height+ responseArea.Y; y++, h++)
			{
                for (int x = responseArea.X, w = 0; x < responseArea.Width + responseArea.X; x++, w++)
                {
                    action.Apply(reqStart, resStart + w * 4 + h * responseArea.Width*4, x, y, requestArea.Width, requestArea.Height);
                    lastW = w;
                }
                lastH = h;
			}
        }
    }
}
