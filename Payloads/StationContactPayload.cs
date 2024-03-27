#if TODO_STATION_CONTACTS

using System.Collections.Generic;
using System.Linq;

namespace StationeersWebApi.Payloads
{
    public class StationContactPayload
    {
        public Vector3Payload angle { get; set; }
        public string contactName { get; set; }

        // TODO: Serialize / deserialize as string
        public StationContactType contactType { get; set; }

        public float lifetime { get; set; }
        public float endLifetime { get; set; }
        public float initialLifeTime { get; set; }
        public int contactID { get; set; }
        public ulong humanTradingSteamID { get; set; }
        public bool currentlyTrading { get; set; }
        public string connectedPadReferenceId { get; set; }
        public Dictionary<int, TradingItemPayload> tradeInventory { get; set; }

        public static StationContactPayload FromStationContact(StationContact contact)
        {
            var payload = new StationContactPayload
            {
                angle = Vector3Payload.FromVector3(contact.Angle),
                contactName = contact.ContactName,
                contactType = contact.ContactType,
                lifetime = contact.Lifetime,
                endLifetime = contact.EndLifetime,
                initialLifeTime = contact.InitialLifeTime,
                contactID = contact.ContactID,
                humanTradingSteamID = contact.HumanTradingSteamID,
                currentlyTrading = contact.CurrentlyTrading,
                connectedPadReferenceId = contact.ConnectedPad?.ReferenceId.ToString(),
                tradeInventory = contact.TraderInventoryDict.Select(
                    item => new { Key = item.Key, Value = TradingItemPayload.FromTradingItemDat(item.Value) }
                ).ToDictionary(item => item.Key, item => item.Value)
            };
            return payload;
        }
    }
}

#endif