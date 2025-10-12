using System.Text.Json.Serialization;

namespace KMS.Shared.DTOs.Document
{
    public class DublinCoreField
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("bib_id")]
        public string BibId { get; set; } = string.Empty;

        [JsonPropertyName("field")]
        public string? Field { get; set; } = string.Empty;

        [JsonPropertyName("indicator1")]
        public string? Indicator1 { get; set; } = string.Empty;

        [JsonPropertyName("indicator2")]
        public string? Indicator2 { get; set; } = string.Empty;

        [JsonPropertyName("field_index")]
        public string? FieldIndex { get; set; } = string.Empty;

        [JsonPropertyName("subFields")]
        public List<DublinCoreSubField> SubFields { get; set; } = new();
    }

    public class DublinCoreSubField
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("field_id")]
        public string FieldId { get; set; } = string.Empty;

        [JsonPropertyName("subfield")]
        public string Subfield { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("bib_id")]
        public string BibId { get; set; } = string.Empty;

        [JsonPropertyName("field")]
        public string Field { get; set; } = string.Empty;

        [JsonPropertyName("subfield_index")]
        public string SubfieldIndex { get; set; } = string.Empty;
    }
}
