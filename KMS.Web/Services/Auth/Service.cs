using KMS.Shared.DTOs.Auth.Login;
using KMS.Web.Helpers;

namespace KMS.Web.Services.Auth
{
    public class Service : IService
    {
        private readonly ApiHelper _apiHelper;
        private readonly ILogger<Service> _logger;
        private readonly AppConfigHelper _appConfigHelper;

        public Service(ApiHelper apiHelper, ILogger<Service> logger, AppConfigHelper appConfigHelper)
        {
            _apiHelper = apiHelper;
            _logger = logger;
            _appConfigHelper = appConfigHelper;
        }

        public async Task<LoginResponse> RequestLoginAsync(LoginRequest model)
        {
            var apiApp = _appConfigHelper.GetApiApp();
            var url = apiApp + "Auth/Login";

            try
            {
                var response = await _apiHelper.PostApiResponseAsync<LoginResponse>(url, model);
                if (!string.IsNullOrEmpty(response.Data.id))
                {
                    return response.Data;
                }
                else
                {
                    _logger.LogWarning("Tài khoản hoặc mật khẩu không đúng!");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get account!: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}