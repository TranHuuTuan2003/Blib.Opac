using KMS.Shared.DTOs.Document;

namespace KMS.Web.Services.DigitalFile
{
    public interface IService
    {
        Task<string> GetFile(string id);
    }
}