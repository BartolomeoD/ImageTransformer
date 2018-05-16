using Kontur.ImageTransformer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Responses
{
    class StatusCode : IResponse
    {
        private int code; 

        public StatusCode(int code)
        {
            this.code = code; 
        }

        public void Execute(HttpListenerResponse response)
        {
            try
            {
                response.StatusCode = code;
                response.OutputStream.Close();
            }
            catch
            {

            }
        }
    }
}
