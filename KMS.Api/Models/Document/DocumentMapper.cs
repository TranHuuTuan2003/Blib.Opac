using KMS.Shared.DTOs.Document;

namespace KMS.Api.Models.Document
{
    public class DocumentMapper : Result
    {
        public string? marc_field_value { get; set; }
        public string? marc { get; set; }
        public string? dublin_core { get; set; }
    }
}