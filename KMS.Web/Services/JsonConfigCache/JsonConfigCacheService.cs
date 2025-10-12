using System.Text.Json;

using Microsoft.Extensions.Caching.Memory;

namespace KMS.Web.Services.JsonConfigCache
{
    public class JsonConfigCacheService<T> : IDisposable where T : class, new()
    {
        private readonly string _cacheKey;
        private readonly string _jsonPath;
        private readonly IMemoryCache _cache;
        private readonly FileSystemWatcher _watcher;
        private readonly ILogger _logger;
        private readonly object _lock = new();

        public JsonConfigCacheService(
            string cacheKey,
            string jsonPath,
            IMemoryCache cache,
            ILogger logger
        )
        {
            _cacheKey = cacheKey;
            _jsonPath = jsonPath;
            _cache = cache;
            _logger = logger;

            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);

            _watcher = new FileSystemWatcher(Path.GetDirectoryName(_jsonPath)!)
            {
                Filter = Path.GetFileName(_jsonPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };
            _watcher.Changed += OnFileChanged;
            _watcher.Deleted += OnFileDeleted;
            _watcher.Renamed += OnFileRenamed;
            _watcher.EnableRaisingEvents = true;

            LoadToCache();
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                _logger.LogWarning($"Config file deleted: {_jsonPath}. Recreating from cache...");

                if (_cache.TryGetValue(_cacheKey, out T config))
                {
                    SaveConfig(config); // tạo lại file
                    _logger.LogInformation($"Recreated config file from cache: {_jsonPath}");
                }
                else
                {
                    _logger.LogWarning($"No cache value available to recreate config file: {_cacheKey}");
                }

                // Sau khi tạo lại file, nạp lại vào cache
                LoadToCache();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recreating config file {_jsonPath}");
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                if (e.OldFullPath == _jsonPath)
                {
                    _logger.LogWarning($"Config file renamed: {e.OldFullPath} -> {e.FullPath}");

                    // Nếu đang có cache thì khôi phục lại file tên cũ
                    if (_cache.TryGetValue(_cacheKey, out T config))
                    {
                        File.WriteAllText(_jsonPath, JsonSerializer.Serialize(config, new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        }));

                        _logger.LogInformation($"Restored original config file: {_jsonPath}");
                    }
                    else
                    {
                        _logger.LogWarning($"Cannot restore {_jsonPath} because cache is empty");
                    }

                    LoadToCache();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring renamed config file: {_jsonPath}");
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                Thread.Sleep(300); // tránh lỗi file lock
                LoadToCache();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reloading config for {_jsonPath}");
            }
        }

        public void LoadToCache()
        {
            lock (_lock)
            {
                try
                {
                    var config = LoadFromFile();
                    if (config == null) return;

                    var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                        .RegisterPostEvictionCallback((key, value, reason, state) =>
                        {
                            if (reason == EvictionReason.Expired)
                            {
                                LoadToCache();
                            }
                        });

                    _cache.Set(_cacheKey, config, options);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to cache config for {_jsonPath}");
                }
            }
        }

        public T LoadFromFile()
        {
            if (!File.Exists(_jsonPath))
            {
                _logger.LogWarning($"Config file not found: {_jsonPath}");
                return new T();
            }

            try
            {
                var json = File.ReadAllText(_jsonPath);
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                }) ?? new T();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading config: {_jsonPath}");
                return new T();
            }
        }

        public T GetConfig()
        {
            if (!_cache.TryGetValue(_cacheKey, out T config))
            {
                LoadToCache();
                config = _cache.Get<T>(_cacheKey) ?? new T();
            }

            return config;
        }

        public void SaveConfig(T config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(_jsonPath, json);
            LoadToCache();
        }

        public void Dispose()
        {
            _watcher.Changed -= OnFileChanged;
            _watcher.Renamed -= OnFileRenamed;
            _watcher.Deleted -= OnFileDeleted;
            _watcher.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}