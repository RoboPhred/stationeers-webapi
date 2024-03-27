
#if TODO_CHAT

using System.Linq;
using StationeersWebApi.Payloads;

namespace StationeersWebApi.Models
{
    public static class ChatModel
    {
        public static ChatPayload[] GetChat()
        {
            var chatPanelText = ChatPanel.Instance.CurrentText;
            return chatPanelText.Split('\n').Select(message =>
            {
                var parts = message.Split(':');
                return new ChatPayload()
                {
                    displayName = parts[0],
                    message = string.Join(":", parts.Skip(1))
                };
            }).ToArray();
        }

        public static void SendChatMessage(string message)
        {
            NetworkManagerHudOverride.Instance.SendNoticeMessage(message);
        }
    }
}

#endif