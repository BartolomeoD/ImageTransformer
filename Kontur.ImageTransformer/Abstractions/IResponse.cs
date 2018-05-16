using System.Net;

namespace Kontur.ImageTransformer.Abstractions
{
    public interface IResponse
    {
        /// <summary>
        ///  Executes response
        /// </summary>
        /// <param name="response"></param>
        void Execute(HttpListenerResponse response);
    }
}
