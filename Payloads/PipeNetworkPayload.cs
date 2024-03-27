
using System.Collections.Generic;
using Assets.Scripts.Networks;
using Assets.Scripts.Objects;

namespace StationeersWebApi.Payloads
{
    public class PipeNetworkPayload : IAtmosphericContentPayload
    {
        public string referenceId { get; set; }

        public List<PipeNetworkConnectionPayload> connectedDevices { get; set; } = new List<PipeNetworkConnectionPayload>();

        public float oxygen { get; set; }
        public float volatiles { get; set; }
        public float water { get; set; }
        public float carbonDioxide { get; set; }
        public float nitrogen { get; set; }
        public float nitrousOxide { get; set; }
        public float pollutant { get; set; }
        public float volume { get; set; }
        public float energy { get; set; }
        public float temperature { get; set; }
        public float pressure { get; set; }

        public static PipeNetworkPayload FromPipeNetwork(PipeNetwork network)
        {
            var payload = new PipeNetworkPayload
            {
                referenceId = network.ReferenceId.ToString()
            };

            foreach (var registration in network.DeviceRegister)
            {
                foreach (var pipe in registration.Value)
                {
                    var end = registration.Key.OpenEnds.Find(openEnd => object.ReferenceEquals(openEnd.GetPipe(), pipe));
                    if (end != null)
                    {
                        payload.connectedDevices.Add(new PipeNetworkConnectionPayload
                        {
                            referenceId = registration.Key.ReferenceId.ToString(),
                            connectionRole = end.ConnectionRole
                        });
                    }
                }
            }

            payload.CopyFromAtmosphere(network.Atmosphere);
            return payload;
        }
    }

    public class PipeNetworkConnectionPayload
    {
        public string referenceId { get; set; }
        public ConnectionRole connectionRole { get; set; }
    }
}