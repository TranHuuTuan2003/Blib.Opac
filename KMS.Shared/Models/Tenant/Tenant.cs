namespace KMS.Shared.Models.Tenant
{
    public class Tenant
    {
        public string? tenant_code { get; set; }
        public string? tenant_name { get; set; }
        public string? tenant_name_en { get; set; }
        public int total_bib { get; set; }
    }
}