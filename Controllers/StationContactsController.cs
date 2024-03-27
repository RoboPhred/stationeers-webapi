#if TODO_STATION_CONTACTS

using System.Threading.Tasks;
using StationeersWebApi.Authentication;
using StationeersWebApi.Server.Attributes;
using StationeersWebApi.Server.Exceptions;
using StationeersWebApi.Payloads;
using StationeersWebApi.Server;

namespace StationeersWebApi.Controllers
{
    [WebController(Path = "api/station-contacts")]
    public class StationContactsController
    {
        [WebRouteMethod(Method = "GET")]
        public async Task GetAllStationContacts(IHttpContext context)
        {
            Authenticator.VerifyAuth(context);
            var payload = await Dispatcher.RunOnMainThread(() => StationContactsModel.GetStationContacts());
            await context.SendResponse(HttpStatusCode.OK, payload);
        }

        [WebRouteMethod(Method = "GET", Path = ":contactId")]
        public async Task GetStationContact(IHttpContext context, int contactId)
        {
            Authenticator.VerifyAuth(context);
            var payload = await Dispatcher.RunOnMainThread(() => StationContactsModel.GetStationContact(contactId));
            if (payload == null)
            {
                throw new NotFoundException("StationContact not found.");
            }
            await context.SendResponse(HttpStatusCode.OK, payload);
        }

        [WebRouteMethod(Method = "POST", Path = ":contactId")]
        public async Task PostStationContact(IHttpContext context, int contactId, StationContactPayload updates)
        {
            Authenticator.VerifyAuth(context);
            var payload = await Dispatcher.RunOnMainThread(() => StationContactsModel.UpdateStationContact(contactId, updates));
            if (payload == null)
            {
                throw new NotFoundException("StationContact not found.");
            }
            await context.SendResponse(HttpStatusCode.OK, payload);
        }

        [WebRouteMethod(Method = "DELETE", Path = ":contactId")]
        public async Task DeleteStationContact(IHttpContext context, int contactId)
        {
            Authenticator.VerifyAuth(context);
            var payload = await Dispatcher.RunOnMainThread(() => StationContactsModel.RemoveStationContact(contactId));
            if (payload == null)
            {
                throw new NotFoundException("StationContact not found.");
            }
            await context.SendResponse(HttpStatusCode.OK);
        }
    }
}
#endif