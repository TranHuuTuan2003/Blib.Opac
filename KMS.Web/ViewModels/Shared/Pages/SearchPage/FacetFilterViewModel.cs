using KMS.Shared.DTOs.FacetFilter;
using KMS.Shared.Models.Tenant;

namespace KMS.Web.ViewModels.Shared.Pages.SearchPage
{
    public class FacetFilterViewModel
    {
        public List<FacetFilterResponse> facetFilters { get; set; } = new();
        public TenantConfig tenantConfig { get; set; } = new();
    }
}