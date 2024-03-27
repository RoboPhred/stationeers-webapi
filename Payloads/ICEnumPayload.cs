#if TODO_IC_ENUMS

using System.Collections.Generic;
using Assets.Scripts.Objects.Electrical;
using System;

namespace StationeersWebApi.Payloads
{
    public class ICEnumPayload
    {
        public Dictionary<string, Dictionary<string, int>> enums { get; set; }

        public static ICEnumPayload FromGame()
        {
            var item = new ICEnumPayload();
            item.enums = new Dictionary<string, Dictionary<string, int>>();

            foreach (KeyValuePair<string, System.Type> pair in ProgrammableChip.EnumLookUp)
            {
                try
                {
                    var enumDict = new Dictionary<string, int>();

                    foreach (string key in Enum.GetNames(pair.Value))
                    {

                        var val = (int)Enum.Parse(pair.Value, key);
                        enumDict[key] = val;

                    }

                    item.enums[pair.Key] = enumDict;
                }
                catch
                {
                    // Handles MineableType which has a value which can not be cast to int.
                }
            }

            return item;
        }
    }
}
#endif