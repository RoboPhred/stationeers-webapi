
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using StationeersWebApi.Server.Attributes;
using StationeersWebApi.Server;
using System.Threading.Tasks;
using StationeersWebApi.Server.Exceptions;
using StationeersWebApi.Payloads;
using StationeersWebApi.JsonTranslation;

namespace StationeersWebApi
{
    [BepInPlugin("net.sunsetfidev.stationeers.StationeersWebApi", "Web API for Stationeers", "2.1.1.0")]
    public class StationeersWebApiPlugin : BaseUnityPlugin
    {
        public static StationeersWebApiPlugin Instance;

        private WebServer _webServer;
        private WebRouter _router = new WebRouter();

        public static string AssemblyDirectory
        {
            get
            {
                var assemblyLocation = typeof(StationeersWebApiPlugin).Assembly.Location;
                var assemblyDir = Path.GetDirectoryName(assemblyLocation);
                return assemblyDir;
            }
        }

        public void AddRoute(IWebRoute route)
        {
            this._router.AddRoute(route);
        }

        public void RegisterControllers(Assembly assembly)
        {
            var controllerTypes = (from type in assembly.GetTypes()
                                   where type.GetCustomAttribute(typeof(WebControllerAttribute)) != null
                                   select type).ToArray();

            var controllerRoutes = (from type in controllerTypes
                                    let controller = Activator.CreateInstance(type)
                                    let routes = WebControllerFactory.CreateRoutesFromController(controller)
                                    from route in routes
                                    select route).ToArray();

            foreach (var route in controllerRoutes)
            {
                this._router.AddRoute(route);
            }

            Logging.Log("Loaded {0} routes from {1} controllers in {2}", controllerRoutes.Length, controllerTypes.Length, assembly.FullName);
        }

        public void StartServer()
        {
            if (!StationeersWebApi.Config.Instance.Enabled)
            {
                return;
            }

            if (this._webServer != null)
            {
                return;
            }

            // TODO: Dispatcher game object is getting destroyed somewhere.
            Dispatcher.Initialize();

            Logging.Log("Starting server on port {0}", StationeersWebApi.Config.Instance.Port);
            this._webServer = new WebServer(this.OnRequest);
            this._webServer.Start(StationeersWebApi.Config.Instance.Port);
        }

        public void StopServer()
        {
            if (this._webServer != null)
            {
                Logging.Log("Stopping server");
                this._webServer.Dispose();
                this._webServer = null;
            }
        }

        void Awake()
        {
            StationeersWebApiPlugin.Instance = this;

            StationeersWebApi.Config.LoadConfig();
            Dispatcher.Initialize();

            if (StationeersWebApi.Config.Instance.Enabled)
            {
                try
                {
                    this.ApplyPatches();
                    Logging.Log("Patches applied successfully");
                }
                catch (Exception ex)
                {
                    Logging.Log("Failed to apply patches: {0}", ex.Message);
                }

                var ownAssembly = typeof(StationeersWebApiPlugin).Assembly;

                try
                {
                    this.RegisterControllers(ownAssembly);
                }
                catch (Exception ex)
                {
                    Logging.Log("Failed to register default controllers: {0}", ex.Message);
                }

                try
                {
                    JsonTranslator.LoadJsonTranslatorStrategies(ownAssembly);
                }
                catch (Exception ex)
                {
                    Logging.Log("Failed to load JsonTranslator strategies: {0}", ex.Message);
                }
            }
            else
            {
                Logging.Log("StationeersWebApi is disabled.");
            }
        }

        private void ApplyPatches()
        {
            var harmony = new Harmony("net.robophreddev.stationeers.StationeersWebApi");
            harmony.PatchAll();
            Logging.Log("Patch succeeded");
        }

        private async Task<bool> OnRequest(IHttpContext context)
        {
            // For a proper implementation of CORS, see https://github.com/expressjs/cors/blob/master/lib/index.js#L159

            if (context.Method == "OPTIONS")
            {
                context.SetResponseHeader("Access-Control-Allow-Origin", "*");
                // TODO: Choose based on available routes at this path
                context.SetResponseHeader("Access-Control-Allow-Methods", "GET, POST, DELETE");
                context.SetResponseHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");
                context.SetResponseHeader("Access-Control-Max-Age", "1728000");
                context.SetResponseHeader("Access-Control-Expose-Headers", "Authorization");
                context.SetResponseHeader("Content-Length", "0");
                await context.SendResponse(HttpStatusCode.NoContent);
                return true;
            }
            else
            {
                context.SetResponseHeader("Access-Control-Allow-Origin", "*");
                context.SetResponseHeader("Access-Control-Expose-Headers", "Authorization");
            }

            try
            {
                return await _router.HandleRequest(context);
            }
            catch (WebException e)
            {
                await context.SendResponse(e.StatusCode, new ErrorPayload
                {
                    message = e.Message
                });
                return true;
            }
        }
    }
}