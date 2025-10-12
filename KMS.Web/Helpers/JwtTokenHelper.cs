using System.Security.Claims;

using KMS.Shared.Helpers;

using UC.Core.Interfaces;
using UC.Core.Models;

namespace KMS.Web.Helpers
{
    public class JwtTokenHelper
    {
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ILogger<JwtTokenHelper> _logger;
        private readonly string username = "UCVN-OPAC";
        private readonly Claim[] claims = new[] {
            new Claim(ClaimTypes.Name, "UCVN-OPAC"),
            new Claim(ClaimTypes.Role, "User")
        };

        public JwtTokenHelper(IJwtAuthManager jwtAuthManager, ILogger<JwtTokenHelper> logger)
        {
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
        }

        public JwtAuthResult GenerateApiToken()
        {
            var now = DateTime.Now;
            return _jwtAuthManager.GenerateTokens(username, claims, now);
        }

        public JwtAuthResult RefreshApiToken(string refreshToken, string token)
        {
            var now = DateTime.Now;
            return _jwtAuthManager.Refresh(refreshToken, token, now);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var result = _jwtAuthManager.DecodeJwtToken(token);
                if (result.Item2 == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, "Error when validating jwt token: {error}.", ex.Message);
                return false;
            }
        }
    }
}