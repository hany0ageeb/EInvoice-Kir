using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace EInvoice.Model
{
    public class DecimalFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return null;
                case JsonToken.Integer:
                    return Convert.ToDecimal(int.Parse(reader.Value.ToString()));
                case JsonToken.Float:
                    return Convert.ToDecimal((double)reader.Value);
                default:
                    return null;
            }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            decimal? dec = value as decimal?;
            if (dec != null)
            {
                string decstr = dec.Value.ToString("0.#####");
                if (decstr.Contains("."))
                    writer.WriteValue(Convert.ToDecimal(dec.Value.ToString("0.#####")));
                else
                    writer.WriteValue(Convert.ToInt64(decstr));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
