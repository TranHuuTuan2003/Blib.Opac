using System.Reflection;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using KMS.Web.Core;
using KMS.Web.Helpers;

using UC.Core.Common;
using UC.Core.Models;

using static KMS.Web.Common.Enums;using KMS.Shared.DTOs.Auth.Login;
using KMS.Shared.Helpers;
using KMS.Web.Common;

namespace KMS.Web.Controllers.Publish.Auth
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly Services.Auth.IService _service;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly Services.PLog.IService _plogService;

        public AuthController(ILogger<AuthController> logger, Services.Auth.IService service, AppConfigHelper appConfigHelper, Services.PLog.IService plogService)
        {
            _logger = logger;
            _service = service;
            _appConfigHelper = appConfigHelper;
            _plogService = plogService;
        }

        private async Task CreateAuthenticationClaim(LoginResponse model)
        {
            var claims = new List<Claim>();

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        private CookieOptions GetCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                IsEssential = true,
                Expires = DateTime.Now.AddMinutes(_appConfigHelper.GetSessionTimeout()),
                SameSite = SameSiteMode.Strict
            };
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _service.RequestLoginAsync(request);
                if (result != null)
                {
                    await CreateAuthenticationClaim(result);

                    var cookieOptions = GetCookieOptions();

                    var hash_key = _appConfigHelper.GetHashKey();
                    var tenant_code = _appConfigHelper.GetTenantCode();
                    string encrypted_token_name = _appConfigHelper.GenerateHMACHash(hash_key, $"{tenant_code}_token");
                    IdentityServerAuthentication _jwtTokenConfig = _appConfigHelper.GetIdentityServerAuthentication();
                    var encryptedToken = EncryptAndDecryptHelper.EncryptToken(result.access_token, (int)_jwtTokenConfig.AccessTokenExpiration);
                    HttpContext.Response.Cookies.Append(encrypted_token_name, encryptedToken, cookieOptions);
                    HttpContext.Session.SetInt32(SessionKeys.IsAuthencated, 1);
                    HttpContext.Session.SetString(SessionKeys.UserName, result.loginname);
                    HttpContext.Session.SetString(SessionKeys.AccessToken, result.access_token);
                    await _plogService.CreateLogDetailAsync(HttpContext, "publish", "r", (int)StatisticType.SUCCESSFULLY_LOGIN, "Đăng nhập thành công");
                    return ResponseMessage.Success("Đăng nhập thành công!");
                }
                else
                {
                    return ResponseMessage.Error("Tài khoản hoặc mật khẩu không đúng!");
                }
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return ResponseMessage.Error("Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin đăng nhập!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return ResponseMessage.Error("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau!");
            }
        }

        [Route("doi-mat-khau")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                string hash_key = _appConfigHelper.GetHashKey();
                string encrypted_token_name = _appConfigHelper.GenerateHMACHash(hash_key, "token");
                var cookieOptions = GetCookieOptions();
                HttpContext.Response.Cookies.Delete(encrypted_token_name, cookieOptions);
                HttpContext.Session.Clear();
                return ResponseMessage.Success("Đăng xuất thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return ResponseMessage.Error("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau!");
            }
        }

        [Route("het-phien-su-dung")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}