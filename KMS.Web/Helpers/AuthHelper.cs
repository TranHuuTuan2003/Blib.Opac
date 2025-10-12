using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

using KMS.Shared.Helpers;

namespace KMS.Web.Helpers
{
    public class AuthHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ILogger<AuthHelper> _logger;

        public AuthHelper(IHttpContextAccessor httpContextAccessor, AppConfigHelper appConfigHelper, ILogger<AuthHelper> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _appConfigHelper = appConfigHelper;
            _logger = logger;
        }

        public bool CheckAuthenticated()
        {
            var hash_key = _appConfigHelper.GetHashKey();
            var tenant_code = _appConfigHelper.GetTenantCode();
            string encrypted_token_name = _appConfigHelper.GenerateHMACHash(hash_key, $"{tenant_code}_token");

            var encryptedToken = _httpContextAccessor.HttpContext?.Request.Cookies[encrypted_token_name];
            string token = string.Empty;
            if (!string.IsNullOrEmpty(encryptedToken))
            {
                token = EncryptAndDecryptHelper.DecryptToken(encryptedToken) ?? string.Empty;
            }

            bool isAuthenticated = false;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    isAuthenticated = jwtToken.ValidTo > DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Invalid token: {ex.Message}");
                }
            }

            return isAuthenticated;
        }

        public string GetFullName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Name")?.Value ?? string.Empty;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Id")?.Value ?? string.Empty;
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Username")?.Value ?? string.Empty;
        }

        public string GetAvatar()
        {
            var avatar = _httpContextAccessor.HttpContext?.User.FindFirst("Avatar")?.Value ?? string.Empty;

            if (avatar.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                avatar.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return avatar;
            }

            return _appConfigHelper.GetBaseUrlFile() + avatar;
        }


        public string GetEmail()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Email")?.Value ?? string.Empty;
        }

        public string GetBirthDate()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Birth_Date")?.Value ?? string.Empty;
        }

        public string GetSex()
        {
            return (_httpContextAccessor.HttpContext?.User.FindFirst("Sex")?.Value == "m" ? "Nam" : "Nữ") ?? string.Empty;
        }

        public string GetIcNo()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Ic_No")?.Value ?? string.Empty;
        }

        public string GetTel()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Tel")?.Value ?? string.Empty;
        }

        public string GetAddress()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Address")?.Value ?? string.Empty;
        }

        public string GetDigitalReaderName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Digital_Reader_Name")?.Value ?? string.Empty;
        }

        public string GetDigitalReaderId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Digital_Reader_Id")?.Value ?? string.Empty;
        }

        public string GetClassName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Class_Name")?.Value ?? string.Empty;
        }

        public string GetCourseName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Course_Name")?.Value ?? string.Empty;
        }

        public string GetDepartmentName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("Department_Name")?.Value ?? string.Empty;
        }

        public List<string> GetCardsNo()
        {
            var cardsClaim = _httpContextAccessor.HttpContext?.User.FindFirst("Cards_No")?.Value;
            var cardsList = string.IsNullOrEmpty(cardsClaim)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(cardsClaim) ?? new List<string>();
            return cardsList;
        }
    }
}