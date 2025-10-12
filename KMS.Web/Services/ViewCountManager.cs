using System.Text.Json;

using KMS.Web.Helpers;

namespace KMS.Web.Services
{
    public class DocumentViewInfo
    {
        public DateTime LastViewed { get; set; }
        public int Count { get; set; }
    }

    public class DocumentView
    {
        public int id { get; set; }
        public int? view { get; set; }
    }

    public class ViewCountManager
    {
        private readonly string _filePath;
        private readonly string _baseUrlApi;
        private readonly object _lock = new();
        private readonly ApiHelper _apiHelper;
        private readonly ILogger<ViewCountManager> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewCountManager(IWebHostEnvironment env, AppConfigHelper appConfigHelper, ApiHelper apiHelper, ILogger<ViewCountManager> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _baseUrlApi = appConfigHelper.GetApiApp();
            _apiHelper = apiHelper;
            _filePath = Path.Combine(env.WebRootPath, "configs", appConfigHelper.GetTenantCode(), "view_counts/view_counts.json");
            _httpContextAccessor = httpContextAccessor;
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_filePath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(_filePath)))
            {
                File.WriteAllText(_filePath, "{}");
            }
            else
            {
                try
                {
                    // Thử parse JSON xem có hợp lệ không, nếu không thì reset
                    var content = File.ReadAllText(_filePath);
                    JsonSerializer.Deserialize<Dictionary<string, DocumentViewInfo>>(content);
                }
                catch
                {
                    File.WriteAllText(_filePath, "{}");
                }
            }
        }

        public void Increment(string documentId)
        {
            lock (_lock)
            {
                var sessionId = _httpContextAccessor.HttpContext?.Session?.Id
                             ?? _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                             ?? "unknown";

                var data = LoadData();
                var now = DateTime.UtcNow;

                var uniqueKey = $"{documentId}_{sessionId}";

                if (data.TryGetValue(uniqueKey, out var info))
                {
                    if ((now - info.LastViewed).TotalMinutes >= 5)
                    {
                        info.LastViewed = now;
                        info.Count += 1;
                    }
                }
                else
                {
                    data[uniqueKey] = new DocumentViewInfo
                    {
                        LastViewed = now,
                        Count = 1
                    };
                }

                SaveData(data);
            }
        }

        public List<DocumentView> GetAndReset()
        {
            lock (_lock)
            {
                var data = GetDocViewData();
                ResetData();
                return data;
            }
        }

        public List<DocumentView> GetDocViewData()
        {
            var data = LoadData();
            var docViews = data
            .Select(kvp => new
            {
                DocumentIdStr = kvp.Key.Split('_')[0],
                Count = kvp.Value.Count
            })
            .Where(x => int.TryParse(x.DocumentIdStr, out _))
            .GroupBy(x => int.Parse(x.DocumentIdStr))
            .Select(g => new DocumentView
            {
                id = g.Key,
                view = g.Sum(x => x.Count)
            })
            .ToList();
            return docViews;
        }

        private Dictionary<string, DocumentViewInfo> LoadData()
        {
            EnsureFileExists();
            try
            {
                string json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new Dictionary<string, DocumentViewInfo>();
                }

                return JsonSerializer.Deserialize<Dictionary<string, DocumentViewInfo>>(json)
                    ?? new Dictionary<string, DocumentViewInfo>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading view count data");
                return new Dictionary<string, DocumentViewInfo>();
            }
        }

        private void SaveData(Dictionary<string, DocumentViewInfo> data)
        {
            EnsureFileExists();
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private void ResetData()
        {
            EnsureFileExists();
            SaveData(new Dictionary<string, DocumentViewInfo>());
        }

        public async Task SyncData()
        {
            try
            {
                var data = GetDocViewData();
                var url = _baseUrlApi + "Document/SyncDocumentViews";
                var response = await _apiHelper.PutApiResponseAsync<object>(url, data);
                if (response.Success)
                {
                    ResetData();
                    return;
                }
                _logger.LogError("Error syncing views count data: {Message}", response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing views count data");
            }
        }
    }
}