using System.Web;

using KMS.Api.Helpers;
using KMS.Shared.Helpers;

using UC.Core.Interfaces;

namespace KMS.Api.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyMiddleware> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly string API_KEY_NAME = "X-API-KEY";
        private readonly string UC_APP_NAME = "UcApp";
        private readonly string API_KEY_SECRET_NAME = "X-UC-SECRET";
        private readonly string _secretApiKey;
        private readonly string _appCode = "opac";

        public ApiKeyMiddleware(
            RequestDelegate next,
            AppConfigHelper appConfigHelper,
            ILogger<ApiKeyMiddleware> logger,
            IJwtAuthManager jwtAuthManager
            )
        {
            _next = next;
            _logger = logger;
            _jwtAuthManager = jwtAuthManager;
            _secretApiKey = appConfigHelper.GetApiKey();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // var origin = context.Request.Headers["Origin"].ToString();
            // var referer = context.Request.Headers["Referer"].ToString();
            // var allowedOrigins = _appConfigHelper.GetAllowedOrigins();
            // bool isTrustedOrigin = allowedOrigins.Any(allowed =>
            //     origin.Equals(allowed, StringComparison.OrdinalIgnoreCase) ||
            //     referer.StartsWith(allowed, StringComparison.OrdinalIgnoreCase)
            // );

            // if (isTrustedOrigin)
            // {
            //     await _next(context);
            //     return;
            // }

            try
            {
                var path = context.Request.Path.Value?.ToLowerInvariant();
                var isSwagger = path.StartsWith("/swagger") || path.Contains("/swagger/index.html");

                if (isSwagger)
                {
                    await _next(context);
                    return;
                }

                if (context.Request.Headers.TryGetValue(UC_APP_NAME, out var appCode) && appCode == _appCode)
                {
                    if (context.Request.Headers.TryGetValue(API_KEY_SECRET_NAME, out var secretApiKey) && _secretApiKey == secretApiKey)
                    {
                        await _next(context);
                        return;
                    }
                }

                if (context.Request.Headers.TryGetValue(API_KEY_NAME, out var xApiKey))
                {
                    var decodeToken = HttpUtility.HtmlDecode(xApiKey);
                    var decryptedToken = EncryptAndDecryptHelper.DecryptToken(decodeToken);
                    try
                    {
                        var (_ClaimsPrincipal, _JwtSecurityToken) = _jwtAuthManager.DecodeJwtToken(decryptedToken ?? "");
                        if (_JwtSecurityToken != null)
                        {
                            await _next(context);
                            return;
                        }
                        throw new Exception("_JwtSecurityToken is null!");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error validating jwt token: {error}", ex.Message);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid key!");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Missing key!");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Unknown error!");
                return;
            }
        }
    }
}