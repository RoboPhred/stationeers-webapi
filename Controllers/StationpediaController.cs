
using System.Threading.Tasks;
using StationeersWebApi.Authentication;
using StationeersWebApi.Payloads;
using StationeersWebApi.Server;
using StationeersWebApi.Server.Attributes;

namespace StationeersWebApi.Controllers
{
    [WebController(Path = "api/stationpedia")]
    class StationpediasController
    {
        [WebRouteMethod(Method = "GET", Path = "ic/instructions")]
        public async Task GetICInstructions(IHttpContext context)
        {
            Authenticator.VerifyAuth(context);
            await context.SendResponse(HttpStatusCode.OK, ICInstructionPayload.FromGame());
        }

#if TODO_IC_ENUMS
        [WebRouteMethod(Method = "GET", Path = "ic/enums")]
        public async Task GetICEnums(IHttpContext context)
        {
            Authenticator.VerifyAuth(context);
            await context.SendResponse(HttpStatusCode.OK, ICEnumPayload.FromGame());
        }
#endif

        [WebRouteMethod(Method = "GET", Path = "logic/slottypes")]
        public async Task GetLogicSlotTypes(IHttpContext context)
        {
            Authenticator.VerifyAuth(context);
            await context.SendResponse(HttpStatusCode.OK, LogicSlotTypesPayload.FromGame());
        }

        [WebRouteMethod(Method = "GET", Path = "logic/types")]
        public async Task GetLogicTypes(IHttpContext context)
        {
            Authenticator.VerifyAuth(context);
            await context.SendResponse(HttpStatusCode.OK, LogicTypesPayload.FromGame());
        }

#if TODO_THING_PREFABS
        [WebRouteMethod(Method = "GET", Path = "things")]
        public async Task GetThings(IHttpContext context)
        {
            Authenticator.VerifyAuth(context);
            var things = await Dispatcher.RunOnMainThread(() => ThingPrefabPayload.FromGame());
            await context.SendResponse(HttpStatusCode.OK, things);
        }
#endif
    }
}