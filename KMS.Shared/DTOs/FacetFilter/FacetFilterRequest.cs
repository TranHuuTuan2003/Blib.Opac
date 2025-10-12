using KMS.Shared.DTOs.Search;

namespace KMS.Shared.DTOs.FacetFilter
{
    public class FacetFilterRequest
    {
        public SearchBody searchRequest { get; set; }
        public string[] codes { get; set; }
        public FacetFilterPagingRequest? paging { get; set; }
        public List<FacetFilterPagingRequest>? pagings { get; set; }
    }
}