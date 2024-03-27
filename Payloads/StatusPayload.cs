
using StationeersWebApi.Models;

namespace StationeersWebApi.Payloads
{
    public class StatusPayload
    {
        public int playerCount { get; set; }
        public string lastSave { get; set; }
        public string gameStatus { get; set; }

        public static StatusPayload FromServer()
        {
            var payload = new StatusPayload()
            {
                playerCount = PlayersModel.GetPlayers().Count,
                gameStatus = SettingsModel.GameStatus.ToString()
            };
            return payload;
        }
    }
}