
using UnityEngine;

namespace StationeersWebApi.Payloads
{
    public class Vector3Payload
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public static Vector3Payload FromVector3(Vector3 v)
        {
            return new Vector3Payload
            {
                x = v.x,
                y = v.y,
                z = v.z
            };
        }

        public static Vector3 ToVector3(Vector3Payload v)
        {
            return new Vector3
            {
                x = v.x,
                y = v.y,
                z = v.z
            };
        }
    }
}