namespace StationeersWebApi.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A web router that routes requests in the form of <see href="IHttpContext"> objects to <see cref="IWebRoute"/> objects.
    /// </summary>
    public class WebRouter
    {
        private List<IWebRoute> routes = new();

        /// <summary>
        /// Gets the number of routes in this router.
        /// </summary>
        public int Count
        {
            get
            {
                return this.routes.Count;
            }
        }

        /// <summary>
        /// Adds a route to this router.
        /// </summary>
        /// <param name="route">The route to add.</param>
        public void AddRoute(IWebRoute route)
        {
            // Put the longest paths first, to help ** path params.
            this.routes = this.routes.Concat(new[] { route }).OrderByDescending(x => this.GetPathSegments(x.Path).Length).ToList();
        }

        /// <summary>
        /// Routes a request to a route in this router.
        /// </summary>
        /// <param name="context">The context to route.</param>
        /// <returns>A task for the web request.  Resolves to true if the request was handled, or false if it was not.</returns>
        public async Task<bool> HandleRequest(IHttpContext context)
        {
            foreach (var route in this.routes)
            {
                if (context.Method != route.Method)
                {
                    continue;
                }

                var pathParameters = this.MatchRoute(context.Path, route.Path);
                if (pathParameters == null)
                {
                    continue;
                }

                var routeContext = new WebRouteHttpContext(context, pathParameters);
                await route.OnRequested(routeContext);
                return true;
            }

            return false;
        }

        private IDictionary<string, string> MatchRoute(string requestPath, string routePath)
        {
            if (requestPath.StartsWith("/"))
            {
                requestPath = requestPath.Substring(1);
            }

            if (requestPath.EndsWith("/"))
            {
                requestPath = requestPath.Substring(0, requestPath.Length - 1);
            }

            if (routePath.StartsWith("/"))
            {
                routePath = routePath.Substring(1);
            }

            if (routePath.EndsWith("/"))
            {
                routePath = routePath.Substring(0, routePath.Length - 1);
            }

            var requestSegments = this.GetPathSegments(requestPath);
            var routeSegments = this.GetPathSegments(routePath);

            return this.RecursiveMatch(requestSegments, routeSegments, 0, 0);
        }

        private Dictionary<string, string> RecursiveMatch(string[] requestSegments, string[] routeSegments, int requestIndex, int routeIndex)
        {
            var pathParams = new Dictionary<string, string>();

            while (requestIndex < requestSegments.Length && routeIndex < routeSegments.Length)
            {
                if (routeSegments[routeIndex].StartsWith("**"))
                {
                    var param = routeSegments[routeIndex].Substring(2);
                    var value = new StringBuilder();
                    var tempRequestIndex = requestIndex;

                    // Attempt to include the current segment in the **
                    while (tempRequestIndex < requestSegments.Length &&
                           (routeIndex == routeSegments.Length - 1 || requestSegments[tempRequestIndex] != routeSegments[routeIndex + 1]))
                    {
                        value.Append(requestSegments[tempRequestIndex]);
                        value.Append('/');
                        tempRequestIndex++;
                    }

                    // Recursively check the rest
                    var restParams = this.RecursiveMatch(requestSegments, routeSegments, tempRequestIndex, routeIndex + 1);
                    if (restParams != null)
                    {
                        if (value.Length > 0)
                        {
                            value.Length--;  // Trim last "/"
                            pathParams.Add(param, value.ToString());
                        }

                        foreach (var kvp in restParams)
                        {
                            pathParams.Add(kvp.Key, kvp.Value);
                        }

                        return pathParams;
                    }
                    else
                    {
                        return null;  // If recursive match failed, then entire path can't match
                    }
                }
                else if (routeSegments[routeIndex].StartsWith(":"))
                {
                    pathParams.Add(routeSegments[routeIndex].Substring(1), requestSegments[requestIndex]);
                    requestIndex++;
                    routeIndex++;
                }
                else
                {
                    if (requestSegments[requestIndex].ToLower() != routeSegments[routeIndex].ToLower())
                    {
                        return null;
                    }

                    requestIndex++;
                    routeIndex++;
                }
            }

            if (requestIndex < requestSegments.Length || routeIndex < routeSegments.Length)
            {
                return null;
            }

            return pathParams;
        }

        private string[] GetPathSegments(string path)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            return path.Split('/');
        }
    }
}
