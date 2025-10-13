using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.Search;
using KMS.Shared.DTOs.Tree;

namespace KMS.Api.Services.Collection
{
    public interface IService
    {
        Task<List<CollectionTree>> CollectionTreeDbTypeAsync(string dbType);
        Task<SearchResponse> SearchingAsync(string type, int page, int pageSize, SearchBody searchRequest);
    }
}