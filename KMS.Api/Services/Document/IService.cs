using KMS.Shared.DTOs.Document;
using static KMS.Api.Services.Document.Service;

namespace KMS.Api.Services.Document
{
    public interface IService
    {
        Task<Details> GetDetailAsync(string slug);
        Task<string?> GetSlugAsync(int? mfn, int? did);
        Task UpdateDocumentView(string slug);
        Task SyncDocumentViews(List<SyncView> docViews);
        Task<List<Result>> GetRelatedDocuments(string slug, int limit);
        Task<List<Borrowing>> GetDocsBorrowingAsync(string card_no);
        Task<List<Extending>> GetDocsExtendAsync(string card_no);
        Task<List<Returned>> GetDocsReturnedAsync(string card_no);

        // Task<DocMarc21> GetMarc21Async(string slug);
        // Task<DocDublinCore> GetDublinCoreAsync(string slug);
        // Task<List<RegisteredCirculation>> GetRegisterCirAsync(int mfn);
        Task<string> GetMarcByMfn(int mfn);
        Task<List<object>> GetTop12BibNew();
        Task<List<object>> GetTop6BibHot();
        Task<List<CollectionDto>> GetTopBibCollection();
        Task<string> GetFile(string id);
        Task<List<object>> GetListDKCB(string slug);
        Task InsertRequestQueueAsync(RequestQueueDto model);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto model);
    }
}