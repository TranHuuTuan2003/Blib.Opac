namespace KMS.Shared.DTOs.DigitalFile
{
    public class Seclever
    {
        public string id { get; set; }
        public bool is_read_preview { get; set; }
        public int? num_of_preview_pages { get; set; }
        public bool is_read_all { get; set; }
        public bool is_download { get; set; }
    }
}
