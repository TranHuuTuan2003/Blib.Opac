using KMS.Shared.DTOs.Document;
using KMS.Web.ViewModels.Shared.Components.Home;

namespace KMS.Web.Services.Home
{
    public interface IService
    {
        Task<List<DocumentNew>> GetTopDocumentsNewAsync();
        Task<List<CollectionDto>> GetTopBibCollection();
    }
}