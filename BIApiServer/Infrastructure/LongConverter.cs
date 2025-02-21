using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BIApiServer.Infrastructure
{
    public class LongConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken value = JToken.ReadFrom(reader);
            return value.Value<long>();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(long) == objectType;
        }
    }
}
