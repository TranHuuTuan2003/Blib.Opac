using KMS.Shared.DTOs.Auth.Login;

namespace KMS.Web.Services.Auth
{
    public interface IService
    {
        Task<LoginResponse> RequestLoginAsync(LoginRequest model);
    }
}