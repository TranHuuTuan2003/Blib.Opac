using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KMS.Api.Infrastructure.Swagger
{
    public class SwaggerExcludeFilter : ISchemaFilter
    {
        private static readonly List<string> excludes = new List<string>()
        {

        };

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context == null)
                return;

            schema.Properties
                  .Where(prp => excludes.Any(exc => string.Equals(exc, prp.Key, StringComparison.OrdinalIgnoreCase)))
                  .Select(prExclude => prExclude.Key)
                  .ToList()
                  .ForEach(key => schema.Properties.Remove(key));
        }
    }
}
