namespace KMS.Shared.DTOs.DigitalFile
{
    public class ClassifiedFile
    {
        public string type { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public List<DigitalFile> files { get; set; } = new();
    }
}
