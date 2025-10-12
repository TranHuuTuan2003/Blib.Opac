using KMS.Shared.DTOs.FacetFilter;
using KMS.Shared.Models.Tenant;
using KMS.Web.Core;
using KMS.Web.Helpers;
using KMS.Web.ViewModels.Shared.Pages.SearchPage;

namespace KMS.Web.Services.FacetFilter
{
    public class Service : IService
    {
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly JsonConfigCacheService<TenantConfig> _jsonConfigService;

        public Service(
                ApiHelper apiHelper,
                AppConfigHelper appConfigHelper,
                JsonConfigCacheService<TenantConfig> jsonConfigService
            )
        {
            _apiHelper = apiHelper;
            _appConfigHelper = appConfigHelper;
            _jsonConfigService = jsonConfigService;
        }

        public async Task<FacetFilterViewModel> GetFacetFilterAsync(string type, FacetFilterRequest model)
        {
            var baseUrl = _appConfigHelper.GetApiApp();
            var url = baseUrl + $"Search/FacetFilterAsync/{type}";

            var response = await _apiHelper.PostApiResponseAsync<List<FacetFilterResponse>>(url, model);
            if (response.Data != null)
            {
                // var tenantConfig = await _document_count_service.GetDocumentCountByTenant();
                var tenantConfig = _jsonConfigService.GetConfig();
                var viewModel = new FacetFilterViewModel();
                viewModel.facetFilters = response.Data;
                var tenantFacetFilter = viewModel.facetFilters.FirstOrDefault(x => x.code == "tn");
                if (tenantConfig.tenants?.Any() == true)
                {
                    if (tenantFacetFilter != null && tenantFacetFilter.rs.Any())
                    {
                        foreach (var tenant in tenantConfig.tenants)
                        {
                            var match = tenantFacetFilter.rs.FirstOrDefault(x => x.value == tenant.tenant_code);
                            tenant.total_bib = match?.subcount ?? 0;
                        }
                    }
                    else
                    {
                        foreach (var tenant in tenantConfig.tenants)
                        {
                            tenant.total_bib = 0;
                        }
                    }
                }
                return viewModel;
            }

            return new();
        }
    }
}