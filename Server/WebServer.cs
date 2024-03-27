namespace StationeersWebApi.Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using StationeersWebApi.Payloads;
    using StationeersWebApi.Server.Exceptions;

    /// <summary>
    /// A web server for receiving HTTP requests.
    /// </summary>
    public class WebServer : IDisposable
    {
        private readonly Func<IHttpContext, Task<bool>> requestHandler;

        private HttpListener listener;
        private Thread listenerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServer"/> class.
        /// </summary>
        /// <param name="requestHandler">The web request handler to use.</param>
        public WebServer(Func<IHttpContext, Task<bool>> requestHandler)
        {
            this.requestHandler = requestHandler;
        }

        /// <summary>
        /// Starts the HTTP server.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public void Start(int port)
        {
            if (this.listener != null)
            {
                throw new InvalidOperationException("Server already started.");
            }

            Logging.LogTrace("Starting web server");

            this.listener = new HttpListener();
            this.listener.Prefixes.Add($"http://*:{port}/");
            this.listener.AuthenticationSchemes = AuthenticationSchemes.None;
            this.listener.Start();

            this.listenerThread = new Thread(this.Listen);
            this.listenerThread.Start();

            Logging.LogInfo($"Server started on port {port}");
        }

        /// <summary>
        /// Stops the HTTP server.
        /// </summary>
        public void Dispose()
        {
            this.listener.Stop();
            this.listenerThread.Abort();
            Logging.LogTrace("Server stopped");
        }

        private async void Listen()
        {
            while (true)
            {
                var context = await this.listener.GetContextAsync();
                this.HandleRequest(context);
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.OnRequest(context);
                }
                catch (Exception e)
                {
                    Logging.LogError($"Failed to handle request: {e}");
                }
            });
        }

        private async Task OnRequest(HttpListenerContext context)
        {
            // Need to get this ahead of time as we loose access to it when the context is disposed.
            var remoteEndPoint = context.Request.RemoteEndPoint.ToString();

            var httpContext = new HttpListenerHttpContext(context);
            try
            {
                var handled = await this.requestHandler(httpContext);
                if (!handled)
                {
                    throw new NotFoundException();
                }
            }
            catch (Exception e)
            {
                var webException = e.GetInnerException<Exceptions.WebException>();
                if (webException != null)
                {
                    Logging.LogError(
                        new Dictionary<string, string>()
                        {
                        { "RequestMethod", httpContext.Method },
                        { "RequestPath", httpContext.Path },
                        { "StatusCode", webException.StatusCode.ToString() },
                        { "RemoteEndpoint", remoteEndPoint },
                        }, $"Failed to handle request: {webException.Message}\n{webException.StackTrace}");

                    await httpContext.TrySendResponse(webException.StatusCode, new ErrorPayload
                    {
                        message = webException.Message,
                    });
                }
                else
                {
                    var unwrapped = e.GetInnerException<Exception>() ?? e;
                    Logging.LogError(
                        new Dictionary<string, string>()
                        {
                        { "RequestMethod", httpContext.Method },
                        { "RequestPath", httpContext.Path },
                        { "RemoteEndpoint", remoteEndPoint },
                        }, $"Failed to handle request: {unwrapped}\n{unwrapped.StackTrace}");

                    await httpContext.TrySendResponse(HttpStatusCode.InternalServerError, new ErrorPayload
                    {
                        message = unwrapped.Message,
                    });
                }
            }
            finally
            {
                // TODO: We don't know whether the websocket was upgraded or not.
                if (!context.Request.IsWebSocketRequest)
                {
                    try
                    {
                        context.Response.Close();
                    }
                    catch (ObjectDisposedException)
                    {
                        // This is fine, it just means the connection was already handled.
                    }
                }
            }
        }
    }
}
