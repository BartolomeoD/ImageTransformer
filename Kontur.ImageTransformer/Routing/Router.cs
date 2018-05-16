using Kontur.ImageTransformer.Abstractions;
using Kontur.ImageTransformer.Actions;
using Kontur.ImageTransformer.Responses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Routing
{
    class Router
    {
        private static readonly Regex ActionRegex =
           new Regex(@"^\/process\/(?<action>[-\w]+)?\/(?<x>[-]?\d*),(?<y>[-]?\d*),(?<w>[-]?\d*),(?<h>[-]?\d*)[\/]*$",
               RegexOptions.Compiled);

        private static readonly string[] actions = new string[4] {
            "rotate-cw",
            "rotate-ccw",
            "flip-h",
            "flip-v"
           };

        public static IResponse Route(HttpListenerRequest request)
        {
            if (request.HttpMethod.ToUpperInvariant() != "POST")
                return new StatusCode((int)HttpStatusCode.BadRequest);
            Match urlMatch = ActionRegex.Match(request.RawUrl);

            if (!urlMatch.Success)
                return new StatusCode((int)HttpStatusCode.BadRequest);

            if (!Int32.TryParse(urlMatch.Groups["x"].Value, out var x) ||
                !Int32.TryParse(urlMatch.Groups["y"].Value, out var y) ||
                !Int32.TryParse(urlMatch.Groups["w"].Value, out var w) ||
                !Int32.TryParse(urlMatch.Groups["h"].Value, out var h))
            {
                return new StatusCode((int)HttpStatusCode.BadRequest);
            }

            if (!actions.Contains(urlMatch.Groups["action"].Value))
                return new StatusCode((int)HttpStatusCode.NotFound);

            Rectangle imageArea = new Rectangle(x, y, w, h);
            if (imageArea.IsEmpty)
                return new StatusCode((int)HttpStatusCode.BadRequest);
            
            //if (request.ContentLength64 > 1024 * 100)
              //  return new StatusCode((int)HttpStatusCode.BadRequest);

            return ActionManager.ApplyAction(urlMatch.Groups["action"].Value, request.InputStream, imageArea);
        }
    }
}
