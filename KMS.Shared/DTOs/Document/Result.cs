namespace KMS.Shared.DTOs.Document
{
    public class Result
    {
        public Ext? item_ext { get; set; }
        public int id { get; set; }
        public string? bib_type { get; set; }
        public string? title { get; set; }
        public string? year_pub { get; set; }
        public string? cover_photo { get; set; }
        public int? view { get; set; }
        public int? download { get; set; }
        public string? db_type { get; set; }
        public int? mfn { get; set; }
        public string? is_attachment { get; set; }
        public int? did { get; set; }
        public string? slug { get; set; }
        public string? tenant_code { get; set; }
    }
}
