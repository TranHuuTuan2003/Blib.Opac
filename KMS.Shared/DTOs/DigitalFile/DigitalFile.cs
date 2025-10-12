namespace KMS.Shared.DTOs.DigitalFile
{
    public class DigitalFile
    {
        public string id { get; set; }
        public int? item_id { get; set; }
        public int? did { get; set; }
        public string? name { get; set; }
        public string? file_path { get; set; }
        public int? total_view { get; set; }
        public int? total_download { get; set; }
        public string? ext { get; set; }
        public bool has_config { get; set; }
        public int? page_count { get; set; }
        public bool is_read_preview { get; set; }
        public int? num_of_preview_pages { get; set; }
        public int? number_of_pages { get; set; }
        public bool is_read_all { get; set; }
        public bool is_download { get; set; }
        public string? ext_in_json { get; set; }
        public List<string>? image_sources { get; set; }
        public string? title { get; set; }
        public bool require_fee { get; set; }
        public bool require_borrow { get; set; }
    }
}
