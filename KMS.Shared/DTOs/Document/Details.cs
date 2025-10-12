using KMS.Shared.DTOs.DigitalFile;

namespace KMS.Shared.DTOs.Document
{
    public class Details : Result
    {
        public int? like { get; set; }
        public string? marc { get; set; } = string.Empty;
        public List<RegisteredCirculation>? register_circulation_place { get; set; }
        public List<MarcField>? marc_field_value_object { get; set; }
        public List<DublinCoreField>? dublin_core_object { get; set; }
        public List<ClassifiedFile> files { get; set; } = new();
    }
}
