using KMS.Shared.Helpers;
using KMS.Web.Helpers;

namespace KMS.Web.Middlewares
{
    public class ApiKeySetupMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public ApiKeySetupMiddleware(RequestDelegate next, JwtTokenHelper jwtTokenHelper)
        {
            _next = next;
            _jwtTokenHelper = jwtTokenHelper;
        }

        private void AppendCookie(HttpContext httpContext, string token, string name)
        {
            httpContext.Response.Cookies.Append(name, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.Now.AddHours(24)
            });
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string accessTokenCookie = "X-API-KEY";
            string refreshTokenCookie = "KEY-API-X";

            string? encryptedAccessToken = context.Request.Cookies.TryGetValue(accessTokenCookie, out var accessToken) ? accessToken : null;
            string? encryptedRefreshToken = context.Request.Cookies.TryGetValue(refreshTokenCookie, out var refreshToken) ? refreshToken : null;

            bool needNewToken = string.IsNullOrEmpty(encryptedAccessToken) || string.IsNullOrEmpty(encryptedRefreshToken);

            if (!needNewToken)
            {
                try
                {
                    string? decryptedAccessToken = EncryptAndDecryptHelper.DecryptToken(encryptedAccessToken!);
                    bool isValid = _jwtTokenHelper.ValidateToken(decryptedAccessToken ?? "");

                    if (!isValid)
                    {
                        string? decryptedRefreshToken = EncryptAndDecryptHelper.DecryptToken(encryptedRefreshToken!);
                        var refreshed = _jwtTokenHelper.RefreshApiToken(decryptedRefreshToken ?? "", decryptedAccessToken ?? "");
                        SetTokenCookies(context, refreshed.AccessToken, refreshed.RefreshToken.TokenString);
                    }
                }
                catch
                {
                    needNewToken = true;
                }
            }

            if (needNewToken)
            {
                var newToken = _jwtTokenHelper.GenerateApiToken();
                SetTokenCookies(context, newToken.AccessToken, newToken.RefreshToken.TokenString);
            }

            await _next(context);
        }

        private void SetTokenCookies(HttpContext context, string accessToken, string refreshToken)
        {
            AppendCookie(context, EncryptAndDecryptHelper.EncryptToken(accessToken, 1440), "X-API-KEY");
            AppendCookie(context, EncryptAndDecryptHelper.EncryptToken(refreshToken, 1440), "KEY-API-X");
        }
    }
}