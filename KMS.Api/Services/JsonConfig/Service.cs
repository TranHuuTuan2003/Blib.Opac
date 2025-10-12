using System.Text.Encodings.Web;
using System.Text.Json;

using KMS.Api.Helpers;

namespace KMS.Api.Services.JsonConfig
{
    public class Service<T> where T : new()
    {
        private readonly string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, ".."));
        private readonly string _configPath;

        public Service(AppConfigHelper appConfigHelper, string filename)
        {
            var path = Path.Combine(
                basePath,
                "web", "wwwroot", "configs",
                appConfigHelper.GetTenantCodes().FirstOrDefault() ?? "default",
                filename
            );
            _configPath = path;
        }

        public T Get()
        {
            if (!File.Exists(_configPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
                return new T();
            }

            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }

        public void Save(T config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(_configPath, json);
        }
    }
}