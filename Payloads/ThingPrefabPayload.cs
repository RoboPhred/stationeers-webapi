#if TODO_THING_PREFABS
using System;
using System.Collections.Generic;
using Assets.Scripts.Objects.Entities;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Objects.Clothing;
using Assets.Scripts.Util;
using Reagents;
using Newtonsoft.Json;

namespace StationeersWebApi.Payloads
{
    public class ThingPrefabPayload
    {
        public Dictionary<string, ThingPayload> things { get; set; }

        public static ThingPrefabPayload FromGame()
        {
            var item = new ThingPrefabPayload();
            item.things = new Dictionary<string, ThingPayload>();

            foreach (Thing thing in Thing.AllPrefabs)
            {
                var payload = ThingPayload.FromThing(thing);
                item.things[thing.name] = payload;
            }

            return item;
        }

        public class ThingPayload
        {
            [JsonIgnore]
            public string name { get; set; }
            public int hash { get; set; }
            public int stackSize { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public Nutrition nutrition { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public Meltable meltable { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            private TemperatureLimits temperatureLimits { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            private LogicTypes logicTypes { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            private List<string> slots { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public Dictionary<string, double> createdReagents { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public Dictionary<string, double> spawnedGases { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public List<String> modes { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public List<String> constructs { get; set; }
            public List<String> flags { get; set; }

            public static ThingPayload FromThing(Thing prefab)
            {
                var item = new ThingPayload();

                item.name = prefab.name;
                item.hash = prefab.GetPrefabHash();

                if (prefab is IQuantity quantity)
                {
                    item.stackSize = (int)quantity.GetMaxQuantity;
                }
                else
                {
                    item.stackSize = 1;
                }

                if (prefab is INutrition nutrition)
                {
                    item.nutrition = Nutrition.FromINutrition(nutrition);
                }

                if (prefab is Ice ice)
                {
                    item.meltable = Meltable.FromIce(ice);
                }

                if (prefab is ILogicable iLogicable)
                {
                    item.logicTypes = LogicTypes.FromILogicable(iLogicable);
                }

                // TODO: Paintable

                item.temperatureLimits = TemperatureLimits.FromThing(prefab);

                if (prefab.Slots.Count > 0)
                {
                    item.slots = SlotsFromThing(prefab);
                }

                if (prefab is Item thingItem)
                {
                    item.createdReagents = ReagentsFromItem(thingItem);
                }

                if (prefab is Ore ore)
                {
                    item.spawnedGases = SpawnedGasFromOre(ore);
                }

                if (prefab.HasModeState && prefab.ModeStrings != null)
                {
                    item.modes = new List<string>(prefab.ModeStrings);
                }

                if (prefab is IConstructionKit kit)
                {
                    item.constructs = ConstructsFromKit(kit);
                }

                item.flags = FlagsFromThing(prefab);

                return item;
            }

            public static List<string> FlagsFromThing(Thing thing)
            {
                var flags = new List<string>();

                if (thing.PaintableMaterial != null)
                {
                    flags.Add("paintable");
                }

                if (thing is Item)
                {
                    flags.Add("item");
                }

                if (thing is IConstructionKit)
                {
                    flags.Add("constructor");
                }

                if (thing is Tool)
                {
                    flags.Add("tool");
                }

                if (thing is Plant)
                {
                    flags.Add("plant");
                }

                if (thing is INutrition)
                {
                    flags.Add("edible");
                }

                if (thing is Structure)
                {
                    flags.Add("structure");
                }

                if (thing is SmallGrid)
                {
                    flags.Add("smallgrid");
                }

                if (thing is ILogicable)
                {
                    flags.Add("logicable");
                }

                if (thing is Entity)
                {
                    flags.Add("entity");
                }

                if (thing is Npc)
                {
                    flags.Add("npc");
                }

                if (thing is Pipe || thing is DeviceAtmospherics)
                {
                    flags.Add("atmospherics");
                }

                if (thing is IWearable)
                {
                    flags.Add("wearable");
                }

                return flags;
            }

            public static Dictionary<string, double> SpawnedGasFromOre(Ore ore)
            {
                if (ore.SpawnContents.Count > 0)
                {
                    var spawnedGases = new Dictionary<string, double>();

                    foreach (SpawnGas gas in ore.SpawnContents)
                    {
                        spawnedGases.Add(gas.Name, gas.Quantity);
                    }


                    return spawnedGases;
                }

                return null;
            }

            public static Dictionary<string, double> ReagentsFromItem(Item item)
            {
                if (item.CreatedReagentMixture.TotalReagents > 0)
                {
                    var createdReagents = new Dictionary<string, double>();

                    foreach (Reagent reagent in Reagent.AllReagents)
                    {
                        var reagentQuantity = item.CreatedReagentMixture.Get(reagent);

                        if (reagentQuantity > 0)
                        {
                            createdReagents.Add(reagent.DisplayName, reagentQuantity);
                        }
                    }

                    return createdReagents;
                }

                return null;
            }

            public static List<string> SlotsFromThing(Thing thing)
            {
                var slots = new List<string>();

                for (int i = 0; i < thing.Slots.Count; i++)
                {
                    var slot = thing.Slots[i];
                    slots.Add(string.IsNullOrEmpty(slot.DisplayName) ? ("Slot " + i) : slot.DisplayName);
                }

                return slots;
            }

            public static List<string> ConstructsFromKit(IConstructionKit kit)
            {
                var constructedPrefabs = kit.GetConstructedPrefabs();

                if (constructedPrefabs != null)
                {
                    var constructs = new List<string>();

                    foreach (Thing constructed in constructedPrefabs)
                    {
                        if (constructed != null)
                        {
                            constructs.Add(constructed.PrefabName);
                        }
                    }

                    return constructs;
                }

                return null;
            }
        }

        public class Nutrition
        {
            public float perQuantity { get; set; }

            public static Nutrition FromINutrition(INutrition iNutrition)
            {
                var nutrition = new Nutrition();
                nutrition.perQuantity = iNutrition.Nutrition(1f);

                return nutrition;
            }
        }

        public class Meltable
        {
            public float temperature { get; set; }
            public float pressure { get; set; }

            public static Meltable FromIce(Ice ice)
            {
                var meltable = new Meltable();
                meltable.temperature = ice.MeltTemperature;
                meltable.pressure = ice.MeltPressure * RocketMath.Thousand;

                return meltable;
            }
        }

        public class TemperatureLimits
        {
            public float shatter { get; set; }
            public float flashpoint { get; set; }
            public float autoignition { get; set; }

            public static TemperatureLimits FromThing(Thing thing)
            {
                var limits = new TemperatureLimits();

                limits.shatter = thing.ShatterTemperature;
                limits.flashpoint = thing.FlashpointTemperature;
                limits.autoignition = thing.AutoignitionTemperature;

                return limits;
            }
        }

        public class LogicTypes
        {
            public List<string> read { get; set; }
            public List<string> write { get; set; }

            public static LogicTypes FromILogicable(ILogicable logicable)
            {
                var logicTypes = new LogicTypes();
                logicTypes.read = new List<string>();
                logicTypes.write = new List<string>();

                foreach (LogicType logicType in Enum.GetValues(typeof(LogicType)))
                {
                    if (logicable.CanLogicRead(logicType))
                    {
                        logicTypes.read.Add(logicType.ToString());
                    }

                    if (logicable.CanLogicWrite(logicType))
                    {
                        logicTypes.write.Add(logicType.ToString());
                    }
                }

                return logicTypes;
            }
        }

    }
}
#endif