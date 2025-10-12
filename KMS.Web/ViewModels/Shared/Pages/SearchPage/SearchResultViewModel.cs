using KMS.Shared.DTOs.Search;

namespace KMS.Web.ViewModels.Shared.Components.SearchPage
{
    public class SearchResultViewModel : SearchResponse
    {
        public string type { get; set; } = "search";
        public string sortType { get; set; } = "desc";
    }
}