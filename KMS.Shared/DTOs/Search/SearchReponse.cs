using KMS.Shared.DTOs.Document;

namespace KMS.Shared.DTOs.Search
{
    public class SearchResponse
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public int totalRecords { get; set; }
        public List<Result> results { get; set; } = new();
    }
}
