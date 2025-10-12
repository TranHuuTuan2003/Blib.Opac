using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

using KMS.Shared.Helpers;
using KMS.Web.Common;
using KMS.Web.Helpers;

using UC.Core.Interfaces;

namespace KMS.Web.Middlewares
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<JwtBearerOptions> _jwtOptions;
        private readonly ILogger<JwtCookieMiddleware> _logger;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly IJwtAuthManager _jwtAuthManager;

        public JwtCookieMiddleware(RequestDelegate next, IOptions<JwtBearerOptions> jwtOptions, ILogger<JwtCookieMiddleware> logger, AppConfigHelper appConfigHelper, IJwtAuthManager jwtAuthManager)
        {
            _next = next;
            _jwtOptions = jwtOptions;
            _logger = logger;
            _appConfigHelper = appConfigHelper;
            _jwtAuthManager = jwtAuthManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // string hash_key = _appConfigHelper.GetHashKey();
            // string encrypted_token_name = _appConfigHelper.GenerateHMACHash(hash_key, "token");
            // var token = context.Request.Cookies[encrypted_token_name];

            // if (token != null)
            // {
            //     try
            //     {
            //         var claimsPrincipal = ValidateToken(token);
            //         if (claimsPrincipal != null)
            //         {
            //             context.User = claimsPrincipal;

            //             var authProperties = new AuthenticationProperties
            //             {
            //                 IsPersistent = true
            //             };
            //             await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         _logger.LogError(ex, "Token validation failed");
            //     }
            // }

            // await _next(context);

            var hash_key = _appConfigHelper.GetHashKey();
            var tenant_code = _appConfigHelper.GetTenantCode();
            string encrypted_token_name = _appConfigHelper.GenerateHMACHash(hash_key, $"{tenant_code}_token");

            try
            {
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    await _next(context);
                    return;
                }

                var encryptedToken = context.Request.Cookies[encrypted_token_name];

                if (string.IsNullOrEmpty(encryptedToken))
                {
                    await _next(context);  // Continue pipeline for anonymous requests
                    return;
                }

                if (!string.IsNullOrEmpty(encryptedToken))
                {
                    var decryptedToken = EncryptAndDecryptHelper.DecryptToken(encryptedToken);
                    var (_ClaimsPrincipal, _JwtSecurityToken) = _jwtAuthManager.DecodeJwtToken(decryptedToken ?? "");
                    if (_JwtSecurityToken == null)
                    {
                        throw new Exception("Invalid or expired token!");
                    }

                    context.User = _ClaimsPrincipal;
                    await _next(context);
                }
                else throw new Exception("Cookie session is empty!");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"JWT Cookie Middleware Error: {ex.Message}");

                // Clear invalid cookie
                context.Response.Cookies.Delete(encrypted_token_name);

                // Only redirect if not already going to error page
                if (!context.Request.Path.StartsWithSegments("/het-phien-su-dung"))
                {
                    context.Response.Redirect(ConstLocation.value + "/het-phien-su-dung");
                }
                else
                {
                    await _next(context);
                }
            }
        }

        // private ClaimsPrincipal ValidateToken(string token)
        // {
        //     var handler = new JwtSecurityTokenHandler();
        //     try
        //     {
        //         var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        //         var claims = jsonToken?.Claims.ToList();
        //         return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        //     }
        //     catch
        //     {
        //         return null;
        //     }
        // }
    }
}