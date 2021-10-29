using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.Model
{
    public class ReceiverTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumstring = (string)reader.Value;
            return ReceiverTypeExtensions.ToReceiverType(enumstring);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ReceiverType rtype = (ReceiverType)value;
            writer.WriteValue(ReceiverTypeExtensions.ToString(rtype));
        }
    }
}
