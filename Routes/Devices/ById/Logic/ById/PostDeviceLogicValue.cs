
using System;
using System.Collections.Generic;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using Newtonsoft.Json;
using WebAPI.API;

namespace WebAPI.Routes.Devices.ById.Logic
{
    class PostDeviceLogicValue : IWebRoute
    {
        public string Method => "POST";

        public string[] Segments => new[] { "devices", ":deviceId", "logic", ":logicType" };

        public void OnRequested(RequestEventArgs e, IDictionary<string, string> pathParams)
        {
            // TODO: Return UNPROCESSABLE_ENTITY if deviceId invalid.
            var referenceId = long.Parse(pathParams["deviceId"]);
            var device = Device.AllDevices.Find(x => x.ReferenceId == referenceId);
            if (device == null)
            {
                e.Context.SendResponse(404, new Error()
                {
                    message = "Device not found."
                });
                return;
            }

            var typeName = pathParams["logicType"];
            LogicType type;
            if (!Enum.TryParse<LogicType>(typeName, out type))
            {
                e.Context.SendResponse(404, new Error()
                {
                    message = "Unrecognized logic type."
                });
                return;
            }

            if (!device.CanLogicWrite(type))
            {
                e.Context.SendResponse(400, new Error()
                {
                    message = "Logic type is not writable."
                });
                return;
            }

            LogicValueItem item = null;
            try
            {
                item = JsonConvert.DeserializeObject<LogicValueItem>(e.Body);
            }
            catch
            {
                e.Context.SendResponse(500, new Error()
                {
                    message = "Expected body to be LogicValueItem."
                });
                return;
            }

            device.SetLogicValue(type, item.value);

            e.Context.SendResponse(200, item);
        }
    }
}