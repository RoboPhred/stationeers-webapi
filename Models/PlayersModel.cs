
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Networking;
using StationeersWebApi.Payloads;

namespace StationeersWebApi.Models
{
    public static class PlayersModel
    {
        public static PlayerPayload GetPlayer(ulong clientId)
        {
            var client = NetworkBase.Clients.FirstOrDefault(x => x.ClientId == clientId);
            if (client == null)
            {
                return null;
            }
            return PlayerPayload.FromPlayerConnection(client);

        }
        public static IList<PlayerPayload> GetPlayers()
        {
            return NetworkBase.Clients.Select(x => PlayerPayload.FromPlayerConnection(x)).ToArray();
        }

        public static PlayerPayload KickPlayer(ulong clientId, string reason)
        {
            var client = NetworkBase.Clients.FirstOrDefault(x => x.ClientId == clientId);
            if (client == null)
            {
                return null;
            }

            var playerPayload = PlayerPayload.FromPlayerConnection(client);
            client.Disconnect();
            return playerPayload;
        }
    }
}