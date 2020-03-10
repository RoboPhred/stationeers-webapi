
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace WebAPI
{
    public class RequestEventArgs : EventArgs
    {
        public HttpListenerContext Context { get; private set; }

        public string Body { get; private set; }

        public RequestEventArgs(HttpListenerContext context, string body)
        {
            this.Context = context;
            this.Body = body;
        }
    }
    public class WebServer
    {
        private HttpListener _listener;
        private Thread _listenerThread;

        public event EventHandler<RequestEventArgs> OnRequest;

        public void Start()
        {
            Log("Starting web server");
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:4444/");
            _listener.Prefixes.Add("http://127.0.0.1:4444/");
            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            _listener.Start();

            _listenerThread = new Thread(ListenServer);
            _listenerThread.Start();
            Log("Server started");
        }

        void ListenServer()
        {
            while (true)
            {
                var result = _listener.BeginGetContext(OnWebRequest, _listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        void OnWebRequest(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            System.Text.Encoding encoding = context.Request.ContentEncoding;
            if (encoding == null)
            {
                encoding = System.Text.Encoding.UTF8;
            }

            var body = new StreamReader(context.Request.InputStream, encoding).ReadToEnd();

            if (OnRequest != null)
            {
                OnRequest(this, new RequestEventArgs(context, body));
            }
        }

        void Log(string message)
        {
            WebAPIPlugin.Instance.Log("[WebServer]: " + message);
        }
    }


}