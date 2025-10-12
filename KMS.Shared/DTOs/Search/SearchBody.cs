namespace KMS.Shared.DTOs.Search
{
    public record SearchBody
    {
        public string[][]? searchBy { get; set; }
        public string[][]? sortBy { get; set; }
        public string[][]? filterBy { get; set; }
        public string? advanceWhereClause { get; set; }
    }
}
