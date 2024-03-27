
#if TODO_CHAT

using System.Threading.Tasks;
using StationeersWebApi.Authentication;
using StationeersWebApi.Payloads;
using StationeersWebApi.Server;
using StationeersWebApi.Server.Attributes;

namespace StationeersWebApi.Controllers
{

    [WebController(Path = "api/chat")]
    public class ChatController
    {
        [WebRouteMethod(Method = "GET")]
        public async Task GetChat(IHttpContext context)
        {
            Authenticator.VerifyAuth(context);

            var chat = await Dispatcher.RunOnMainThread(() => ChatModel.GetChat());

            await context.SendResponse(HttpStatusCode.OK, chat);
        }

        [WebRouteMethod(Method = "POST")]
        public async Task SendChatMessage(IHttpContext context, ChatMessagePayload body)
        {
            Authenticator.VerifyAuth(context);

            if (body.message == null || body.message.Length == 0)
            {
                await context.SendResponse(HttpStatusCode.BadRequest, new ErrorPayload()
                {
                    message = "Expected a message."
                });
                return;
            }

            await Dispatcher.RunOnMainThread(() => ChatModel.SendChatMessage(body.message));

            await context.SendResponse(HttpStatusCode.OK);
        }
    }
}

#endif