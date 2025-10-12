using KMS.Shared.DTOs.Document;
using Newtonsoft.Json.Linq;

namespace KMS.Shared.Converters
{
    public class JsonTypeNewtonsoftConverter : Newtonsoft.Json.JsonConverter<Ext?>
    {
        public override Ext? ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Ext? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);

            return jsonObject.ToObject<Ext>();
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Ext? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value != null)
            {
                var json = JObject.FromObject(value);
                json.WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
