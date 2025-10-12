namespace KMS.Shared.DTOs.FacetFilter
{
    public class FacetFilterPagingRequest
    {
        public string? code { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
    }
}