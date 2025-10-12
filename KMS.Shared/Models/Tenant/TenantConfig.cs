namespace KMS.Shared.Models.Tenant
{
    public class TenantConfig
    {
        public string? facet_filter_label { get; set; }
        public string? facet_filter_label_en { get; set; }
        public List<Tenant>? tenants { get; set; }
    }
}