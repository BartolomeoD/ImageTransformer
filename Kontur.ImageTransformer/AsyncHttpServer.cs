using Kontur.ImageTransformer.Responses;
using Kontur.ImageTransformer.Routing;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            QueueSize = Environment.ProcessorCount*3; 
            listener = new HttpListener();
            RequestsQueue = new ConcurrentQueue<HttpListenerContext>();
            ExecuterThreads = new Thread[Environment.ProcessorCount];
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

                    listenerThread.Start();

                    for (int i = 0; i < ExecuterThreads.Length; i++)
                    {
                        ExecuterThreads[i] = new Thread(ExecuteFromRequestsQueue)
                        {
                            IsBackground = true,
                            Priority = ThreadPriority.BelowNormal
                        };
                        ExecuterThreads[i].Start();
                    }
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
                    if (listener.IsListening) {
                        var context = listener.GetContext();
                        if (queueLength < QueueSize)
                        {
                            RequestsQueue.Enqueue(context);
                            Interlocked.Increment(ref queueLength);
                        }
                        else
                        {
                            var response = new StatusCode(429);  //429
                            response.Execute(context.Response);
                        }
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
                    if (queueLength > 0 && listener.IsListening)
                    {
                        if (!RequestsQueue.TryDequeue(out var context))
                            continue;
                        Interlocked.Decrement(ref queueLength);
                        Task.Run(() =>
                        {
                            var response = Router.Route(context.Request);
                            response.Execute(context.Response);
                        });
                    }
                    else
                        Thread.Sleep(10);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }
        }

        public int QueueSize;

        private readonly HttpListener listener;

        private int queueLength = 0;

        private ConcurrentQueue<HttpListenerContext> RequestsQueue;
        private Thread listenerThread;
        private Thread[] ExecuterThreads;
        private bool disposed;
        private volatile bool isRunning;
    }
}