using KMS.Shared.DTOs.FacetFilter;
using KMS.Web.ViewModels.Shared.Components.SearchPage;

namespace KMS.Web.ViewModels.Shared.Pages.Chatbot
{
    public class ChatbotViewModel
    {
        public string searchType { get; set; } = string.Empty;
        public string keyword { get; set; } = string.Empty;
        public bool hasFacetFilter { get; set; }
        public SearchResultViewModel searchResult { get; set; } = new();
        public List<FacetFilterResponse> facetFilters { get; set; } = new();
    }
}