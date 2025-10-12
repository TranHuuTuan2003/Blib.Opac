namespace KMS.Shared.DTOs.FacetFilter
{
    public class FacetFilterResponse
    {
        public string code { get; set; } = string.Empty;
        public int page { get; set; }
        public int pageSize { get; set; }
        public List<FacetFilterResult> rs { get; set; } = new();
        public bool forPaging { get; set; }
    }
}