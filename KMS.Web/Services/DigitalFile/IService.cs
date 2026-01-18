using KMS.Shared.DTOs.DigitalFile;
using System.Threading.Tasks;

namespace KMS.Web.Services.DigitalFile
{
    public interface IService
    {
        Task<string> GetFile(string id);
        Task<Seclever> GetSecleverFile(string id);
    }
}