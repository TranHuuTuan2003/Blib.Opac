using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;

using KMS.Shared.Helpers;
using KMS.Web.Helpers;

namespace KMS.Web.Controllers.Proxy
{
    [ApiController]
    public class ApiProxyController : ControllerBase
    {
        private readonly ILogger<ApiProxyController> _logger;
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;

        public ApiProxyController(ILogger<ApiProxyController> logger, ApiHelper apiHelper, AppConfigHelper appConfigHelper)
        {
            _logger = logger;
            _apiHelper = apiHelper;
            _appConfigHelper = appConfigHelper;
        }

        private string BuildUrl(string type, string path)
        {
            var query = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty;
            var sanitizedPath = path?.Split('?')[0].TrimStart('/');

            var baseUrl = type.ToLower() switch
            {
                "app" => _appConfigHelper.GetApiApp().TrimEnd('/'),
                "blib" => _appConfigHelper.GetApiBlib().TrimEnd('/'),
                "core" => _appConfigHelper.GetApiCore().TrimEnd('/'),
                "gateway" => _appConfigHelper.GetBaseUrlGateway().TrimEnd('/'),
                _ => throw new ArgumentException($"Invalid proxy type: {type}")
            };

            return $"{baseUrl}/{sanitizedPath}{query}";
        }

        private string GetTokenFromContext()
        {
            return Request.Headers["Authorization"].ToString();
        }

        [HttpGet("proxy/{type}/api/{**path}")]
        public async Task<IActionResult> ProxyGet(string type, string path)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var url = BuildUrl(type, path);
                var token = GetTokenFromContext();
                var result = await _apiHelper.RawGetAsync(url, token);
                return Content(result.Content, result.ContentType);
            });
        }

        [HttpPost("proxy/{type}/api/{**path}")]
        public async Task<IActionResult> ProxyPost(string type, string path)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var url = BuildUrl(type, path);
                var token = GetTokenFromContext();

                if (Request.HasFormContentType)
                {
                    var form = await Request.ReadFormAsync();
                    var multi = new MultipartFormDataContent();

                    foreach (var field in form)
                    {
                        multi.Add(new StringContent(field.Value!), field.Key);
                    }

                    foreach (var file in form.Files)
                    {
                        var stream = file.OpenReadStream();
                        var content = new StreamContent(stream);
                        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        multi.Add(content, file.Name, file.FileName);
                    }

                    var result = await _apiHelper.RawPostAsync(url, multi, token);
                    return Content(result.Content, result.ContentType);
                }
                else
                {
                    var result = await _apiHelper.RawPostAsync(url, Request.Body, Request.ContentType, token);
                    return Content(result.Content, result.ContentType);
                }
            });
        }

        [HttpPut("proxy/{type}/api/{**path}")]
        public async Task<IActionResult> ProxyPut(string type, string path)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var url = BuildUrl(type, path);
                var token = GetTokenFromContext();
                var result = await _apiHelper.RawPutAsync(url, Request.Body, Request.ContentType, token);
                return Content(result.Content, result.ContentType);
            });
        }

        [HttpDelete("proxy/{type}/api/{**path}")]
        public async Task<IActionResult> ProxyDelete(string type, string path)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var url = BuildUrl(type, path);
                var token = GetTokenFromContext();
                var result = await _apiHelper.RawDeleteAsync(url, token);
                return Content(result.Content, result.ContentType);
            });
        }

        [HttpGet("proxy/{type}/static/{**path}")]
        public async Task<IActionResult> ProxyStatic(string type, string path)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var url = BuildUrl(type, path);
                var token = GetTokenFromContext();

                if (string.IsNullOrEmpty(url))
                    return NotFound();

                var result = await _apiHelper.RawGetStreamAsync(url, token);

                if (!result.IsSuccessStatusCode)
                    return NotFound();

                return File(result.Stream, result.ContentType ?? "application/octet-stream");
            });
        }
    }
}