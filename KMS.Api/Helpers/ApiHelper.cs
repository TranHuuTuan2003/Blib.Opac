using System.Text;

using KMS.Shared.DTOs.Api;

using Newtonsoft.Json;

namespace KMS.Api.Helpers
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiHelper> _logger;

        public ApiHelper(HttpClient httpClient, ILogger<ApiHelper> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> HeadAsync(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url); // Dùng GET thay vì HEAD
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 0); // Chỉ lấy 1 byte đầu (tránh tải toàn bộ file)

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"HEAD (GET) request failed: {ex.Message}");
                return false;
            }
        }

        public async Task<T> GetJsonStringAsync<T>(string url) where T : new()
        {
            try
            {
                // Kiểm tra URL hợp lệ
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new ArgumentException("URL cannot be null or empty", nameof(url));
                }

                // Sử dụng using để đảm bảo HttpResponseMessage được dispose đúng cách
                using (var response = await _httpClient.GetAsync(url))
                {
                    // Đảm bảo response thành công
                    response.EnsureSuccessStatusCode();

                    // Đọc nội dung response
                    var json = await response.Content.ReadAsStringAsync();

                    // Kiểm tra nếu json rỗng
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        _logger.LogWarning("Empty JSON response received from {Url}", url);
                        return new T();
                    }

                    // Deserialize JSON
                    var result = JsonConvert.DeserializeObject<T>(json);
                    return result ?? new T();
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request failed when calling {Url}. Error: {Message}", url, httpEx.Message);
                return new T();
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization failed for response from {Url}. Error: {Message}", url, jsonEx.Message);
                return new T();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching JSON from {Url}. Error: {Message}", url, ex.Message);
                return new T();
            }
        }

        public async Task<ApiResponse<T>> GetApiResponseAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }
        public async Task<ApiResponse<T>> PostApiResponseAsync<T>(string url, object data)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }
        public async Task<ApiResponse<T>> PutApiResponseAsync<T>(string url, object data)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }
        public async Task<ApiResponse<T>> DeleteApiResponseAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }
    }
}