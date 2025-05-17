using Amqp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Core
{
    /// <summary>
    /// Generic deserializer
    /// Handles when the broker encodes the payload differently
    /// </summary>
    /// <typeparam name="t"></typeparam>
    public class ExtensibilityDeserializer<t>
    {
        public t DeserializeObject(object body)
        {
            t eventMessage = default(t);
            // Depending on AMQP broker used the message payload maybe a byte array or just a string
            if (body.GetType().Name == "Byte[]")
            {
                eventMessage = JsonConvert.DeserializeObject<t>(Encoding.UTF8.GetString((Byte[])body));
            }
            else
            {
                eventMessage = JsonConvert.DeserializeObject<t>(body.ToString());
            }

            return eventMessage;
        }
    }
}
