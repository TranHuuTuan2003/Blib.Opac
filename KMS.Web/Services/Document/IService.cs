using KMS.Shared.DTOs.Document;

namespace KMS.Web.Services.Document
{
    public interface IService
    {
        Task<Details?> GetDocumentDetailAsync(string slug);
        Task<List<Result>> GetRelatedDocumentsAsync(string slug);
        Task<List<Result>> GetTop6BibHot();
    }
}