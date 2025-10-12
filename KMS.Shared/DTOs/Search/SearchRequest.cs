namespace KMS.Shared.DTOs.Search
{
    public record SearchRequest
    {
        public string? type { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public bool hasFacetFilter { get; set; }
        public SearchBody request { get; set; } = new();
    }
}
