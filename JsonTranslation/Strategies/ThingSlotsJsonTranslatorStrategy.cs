
using System;
using System.Collections.Generic;
using Assets.Scripts.Objects;
using Newtonsoft.Json.Linq;

namespace StationeersWebApi.JsonTranslation.Strategies
{
    [JsonTranslatorStrategy]
    public sealed class ThingSlotsJsonTranslatorStrategy : IJsonTranslatorStrategy
    {
        public Type TargetType => typeof(Thing);

        public string[] SupportedProperties => new[] { "slotReferenceIds" };

        public void UpdateObjectFromJson(object target, JObject input)
        {
            if (input.ContainsKey("slotReferenceIds"))
            {
                throw new ReadOnlyPropertyException("slotReferenceIds is read only.");
            }
        }

        public void VerifyJsonUpdate(object target, JObject input)
        {
            if (input.ContainsKey("slotReferenceIds"))
            {
                throw new ReadOnlyPropertyException("slotReferenceIds is read only.");
            }
        }

        public void WriteObjectToJson(object target, JObject output)
        {
            var thing = (Thing)target;

            var refIds = new Dictionary<int, string>();
            for (var i = 0; i < thing.Slots.Count; i++)
            {
                var slot = thing.Slots[i];
                var occupant = slot.Get();
                if (occupant != null)
                {
                    refIds.Add(i, occupant.ReferenceId.ToString());
                }
            }

            output["slotReferenceIds"] = JObject.FromObject(refIds);
        }
    }
}