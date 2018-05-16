using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer
{
    internal class AsyncHttpServer : IDisposable
    {
        public AsyncHttpServer()
        {
            listener = new HttpListener();
            RequestsQueue = new Queue();
        }
        
        public void Start(string prefix)
        {
            lock (listener)
            {
                if (!isRunning)
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();

                    listenerThread = new Thread(AddToRequestsQueue)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };

                    ExecuterThread = new Thread(ExecuteFromRequestsQueue)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };

                    listenerThread.Start();
                    ExecuterThread.Start();
                    
                    isRunning = true;
                }
            }
        }

        public void Stop()
        {
            lock (listener)
            {
                if (!isRunning)
                    return;

                listener.Stop();

                listenerThread.Abort();
                listenerThread.Join();
                
                isRunning = false;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            Stop();

            listener.Close();
        }
        
        private void AddToRequestsQueue()
        {
            while (true)
            {
                try
                {
                    if (listener.IsListening && RequestsQueue.Count < QueueSize)
                    {
                        var context = listener.GetContext();
                        RequestsQueue.Enqueue(context);
                    }
                    else
                    {

                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        private void ExecuteFromRequestsQueue()
        {

            while (true)
            {
                try
                {
                    if (listener.IsListening && RequestsQueue.Count > 0)
                    {
                        var context = listener.GetContext();
                        Task.Run(() => HandleContextAsync((HttpListenerContext)RequestsQueue.Dequeue()));
                    }
                    else Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        private async Task HandleContextAsync(HttpListenerContext listenerContext)
        {
            // TODO: implement request handling

            listenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
            using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
                writer.WriteLine("Hello, world!");
        }

        public int QueueSize;


        private readonly HttpListener listener;

        private Queue RequestsQueue;
        private Thread listenerThread;
        private Thread ExecuterThread;
        private bool disposed;
        private volatile bool isRunning;
    }
}