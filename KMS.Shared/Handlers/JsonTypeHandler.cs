using Dapper;
using System.Data;
using System.Text.Json;

namespace KMS.Shared.Handlers
{
    public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T> where T : class
    {
        public override void SetValue(IDbDataParameter parameter, T? value)
        {
            parameter.Value = value == null ? DBNull.Value : JsonSerializer.Serialize(value);
        }

        public override T? Parse(object value)
        {
            if (value is string json)
            {
                return JsonSerializer.Deserialize<T>(json);
            }

            return null;
        }
    }
}
