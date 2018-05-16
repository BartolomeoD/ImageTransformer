using System;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            using (var server = new AsyncHttpServer())
            {
                server.Start("http://+:8080/");
                server.QueueSize = Environment.ProcessorCount * 3;
                Console.ReadKey(true);
            }
        }
    }
}
