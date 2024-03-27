
using System.IO;
using System.Threading.Tasks;
using Assets.Scripts.Serialization;
using StationeersWebApi.Authentication;
using StationeersWebApi.Payloads;
using StationeersWebApi.Server;
using StationeersWebApi.Server.Attributes;
using StationeersWebApi.Server.Exceptions;

namespace StationeersWebApi.Controllers
{
    [WebController(Path = "api/saves")]
    public class SavesController
    {
        [WebRouteMethod(Method = "POST")]
        public async Task PostSave(IHttpContext context, PostSavePayload body)
        {
            Authenticator.VerifyAuth(context);

            if (body == null)
            {
                throw new BadRequestException("A body with a fileName must be specified.");
            }

            if (body.fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new BadRequestException("Filename contains invalid characters.");
            }

            var worldDirectory = StationSaveUtils.GetWorldSaveDirectory(body.fileName);
            await Dispatcher.RunOnMainThread(() => XmlSaveLoad.SaveGameAs(worldDirectory));
            await context.SendResponse(HttpStatusCode.OK, body);
        }
    }
}