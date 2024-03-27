
#if TODO_STATION_CONTACTS

using System.Collections.Generic;
using System.Linq;
using StationeersWebApi.Payloads;

namespace StationeersWebApi.Models
{
    public static class StationContactsModel
    {
        public static IList<StationContactPayload> GetStationContacts()
        {
            return StationContact.AllStationContacts.Select(contact => StationContactPayload.FromStationContact(contact)).ToList();
        }

        public static StationContactPayload GetStationContact(int contactId)
        {
            var contact = StationContact.AllStationContacts.FirstOrDefault(x => x.ContactID == contactId);
            if (contact == null)
            {
                return null;
            }
            return StationContactPayload.FromStationContact(contact);
        }

        public static StationContactPayload UpdateStationContact(int contactId, StationContactPayload updates)
        {
            var contact = StationContact.AllStationContacts.FirstOrDefault(x => x.ContactID == contactId);
            if (contact == null)
            {
                return null;
            }

            StationContactsModel.WriteStationContactProperties(contact, updates);
            return StationContactPayload.FromStationContact(contact);
        }

        public static void WriteStationContactProperties(StationContact contact, StationContactPayload payload)
        {
            contact.Angle = Vector3Payload.ToVector3(payload.angle);
            contact.ContactName = payload.contactName;
            contact.ContactType = payload.contactType;
            contact.Lifetime = payload.lifetime;
            contact.EndLifetime = payload.endLifetime;
            contact.InitialLifeTime = payload.initialLifeTime;
            // TODO: Deserialize inventory data.
            //contact.TraderInventoryDict = payload.traderInventoryDict;
            //contact.SerializedTraderInventory = payload.serializedTraderInventory;
        }

        public static StationContactPayload RemoveStationContact(int contactId)
        {
            var contact = StationContact.AllStationContacts.FirstOrDefault(x => x.ContactID == contactId);
            if (contact == null)
            {
                return null;
            }
            StationContact.AllStationContacts.Remove(contact);
            return StationContactPayload.FromStationContact(contact);
        }
    }
}
#endif