using KMS.Shared.DTOs.Search;
using KMS.Web.ViewModels.Shared.Components.SearchPage;

namespace KMS.Web.Services.Search
{
    public interface IService
    {
        Task<SearchResultViewModel> SearchDocsAsync(SearchRequest searchRequest);
    }
}