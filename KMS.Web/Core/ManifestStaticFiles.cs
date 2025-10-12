using System.Text.Json;

namespace KMS.Web.Core
{
    public interface IManifestStaticFiles
    {
        string get(string originalFile);
    }

    public class ManifestStaticFiles : IManifestStaticFiles
    {
        private readonly Dictionary<string, string> _map;

        public ManifestStaticFiles(IWebHostEnvironment env, JsonConfigCacheService<Dictionary<string, string>> jsonConfig)
        {
            _map = jsonConfig.GetConfig();

            if (_map.Count == 0)
            {
                var manifestPath = Path.Combine(env.WebRootPath, "public", "manifest", "manifest.json");

                if (File.Exists(manifestPath))
                {
                    var json = File.ReadAllText(manifestPath);
                    _map = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                    jsonConfig.SaveConfig(_map);
                }
                else
                {
                    _map = new Dictionary<string, string>();
                }
            }
        }

        public string get(string originalFile)
        {
            if (_map.TryGetValue(originalFile, out var hashed))
            {
                return hashed;
            }

            return originalFile;
        }
    }
}