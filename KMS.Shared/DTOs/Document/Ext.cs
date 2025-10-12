using System.Text.Json.Serialization;

using KMS.Shared.Converters;

namespace KMS.Shared.DTOs.Document
{
    public class Ext
    {
        [JsonPropertyName("Class_Name")]
        public string? class_name { get; set; }

        [JsonPropertyName("Summary")]
        public string? summary { get; set; }

        [JsonPropertyName("Publish_Info")]
        public string? publish_info { get; set; }

        [JsonPropertyName("Physical_Info")]
        public string? physical_info { get; set; }

        [JsonPropertyName("Author")]
        public string? author { get; set; }

        [JsonPropertyName("Keyword")]
        public string? keyword { get; set; }

        [JsonPropertyName("Subject")]
        public string? subject { get; set; }

        [JsonPropertyName("Regstr")]
        public string? regstr { get; set; }

        [JsonPropertyName("Isbd")]
        public string? isbd { get; set; }

        [JsonPropertyName("Cutter")]
        public string? cutter { get; set; }

        [JsonPropertyName("Language")]
        public string? language { get; set; }

        [JsonPropertyName("Isbn")]
        public string? isbn { get; set; }

        [JsonPropertyName("Cip")]
        public string? cip { get; set; }

        [JsonPropertyName("Bib_Price")]
        [JsonConverter(typeof(IntToStringConverter))]
        public string? bib_price { get; set; }
    }
}