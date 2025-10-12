using System.Net;
using System.Net.Http.Headers;
using System.Text;

using KMS.Shared.DTOs.Api;

using Newtonsoft.Json;

namespace KMS.Web.Helpers
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly string _secretApiKey;

        public ApiHelper(AppConfigHelper appConfigHelper)
        {
            _appConfigHelper = appConfigHelper;
            _secretApiKey = appConfigHelper.GetApiKey();

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = _cookieContainer
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task<ApiResponse<T>> GetApiResponseAsync<T>(string url, bool needToken = false, string token = "")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                // Thêm API Key vào request heade
                AddHeaders(request);
                if (needToken)
                {
                    AddAuthorizationHeader(request, token);
                }
                // Gửi Request
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }

        public async Task<ApiResponse<T>> PostApiResponseAsync<T>(string url, object data, bool needToken = false, string token = "")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                // Thêm API Key vào request heade
                AddHeaders(request);
                if (needToken)
                {
                    AddAuthorizationHeader(request, token);
                }
                // Chuyển dữ liệu thành JSON và gán vào request body
                var jsonContent = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None
                });
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Gửi request
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }

        public async Task<ApiResponse<T>> PutApiResponseAsync<T>(string url, object data, bool needToken = false, string token = "")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                AddHeaders(request);
                if (needToken)
                {
                    AddAuthorizationHeader(request, token);
                }
                // Chuyển dữ liệu thành JSON và gán vào request body
                var jsonContent = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Gửi request
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }

        public async Task<ApiResponse<T>> DeleteApiResponseAsync<T>(string url, bool needToken = false, string token = "")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                // Thêm API Key vào request heade
                AddHeaders(request);
                if (needToken)
                {
                    AddAuthorizationHeader(request, token);
                }
                // Gửi request
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString) ?? new ApiResponse<T>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching API response: {ex.Message}");
            }

            return new ApiResponse<T>();
        }

        public async Task<(string Content, string ContentType)> RawGetAsync(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddHeaders(request);
            if (!string.IsNullOrEmpty(token))
            {
                AddAuthorizationHeader(request, token);
            }
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return (content, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        public async Task<(string Content, string ContentType)> RawPostAsync(string url, Stream body, string? contentType, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            AddHeaders(request);
            if (!string.IsNullOrEmpty(token))
            {
                AddAuthorizationHeader(request, token);
            }
            request.Content = new StreamContent(body);

            if (contentType != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return (content, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        public async Task<(string Content, string ContentType)> RawPostAsync(string url, HttpContent content, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            AddHeaders(request);
            if (!string.IsNullOrEmpty(token))
            {
                AddAuthorizationHeader(request, token);
            }
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            return (responseContent, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        public async Task<(string Content, string ContentType)> RawPutAsync(string url, Stream body, string? contentType, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            AddHeaders(request);
            if (!string.IsNullOrEmpty(token))
            {
                AddAuthorizationHeader(request, token);
            }
            request.Content = new StreamContent(body);
            if (contentType != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return (content, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        public async Task<(string Content, string ContentType)> RawDeleteAsync(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            AddHeaders(request);
            if (!string.IsNullOrEmpty(token))
            {
                AddAuthorizationHeader(request, token);
            }
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return (content, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        public async Task<HttpResultStream> RawGetStreamAsync(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddHeaders(request);
            if (!string.IsNullOrEmpty(token))
            {
                AddAuthorizationHeader(request, token);
            }
            var response = await _httpClient.SendAsync(request);
            var stream = await response.Content.ReadAsStreamAsync();

            return new HttpResultStream
            {
                Stream = stream,
                ContentType = response.Content.Headers.ContentType?.ToString(),
                StatusCode = response.StatusCode,
                IsSuccessStatusCode = response.IsSuccessStatusCode
            };
        }

        public void AddHeader(string name, string value)
        {
            if (_httpClient.DefaultRequestHeaders.Contains(name))
                _httpClient.DefaultRequestHeaders.Remove(name);

            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        private void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("X-UC-SECRET", _secretApiKey);
            request.Headers.Add("UcSite", _appConfigHelper.GetTenantCode());
            request.Headers.Add("UcApp", _appConfigHelper.GetAppCode());
        }

        public void AddAuthorizationHeader(HttpRequestMessage request, string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim());
            }
        }
    }

    public class HttpResultStream
    {
        public Stream Stream { get; set; } = Stream.Null;
        public string? ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}