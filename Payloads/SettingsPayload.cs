
using StationeersWebApi.Models;

namespace StationeersWebApi.Payloads
{
    public class SettingsPayload
    {
        public string name { get; set; }
        public string mapName { get; set; }
        public int? maxPlayers { get; set; }
        public string password { get; set; }
        public string startingCondition { get; set; }
        public string respawnCondition { get; set; }

        public static SettingsPayload FromServer()
        {
            var payload = new SettingsPayload()
            {
                name = SettingsModel.Name,
                mapName = SettingsModel.MapName,
                maxPlayers = SettingsModel.MaxPlayers,
                password = SettingsModel.Password,
                startingCondition = SettingsModel.StartingCondition,
                respawnCondition = SettingsModel.RespawnCondition
            };
            return payload;
        }
    }
}