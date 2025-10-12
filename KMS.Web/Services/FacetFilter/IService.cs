using KMS.Shared.DTOs.FacetFilter;
using KMS.Web.ViewModels.Shared.Pages.SearchPage;

namespace KMS.Web.Services.FacetFilter
{
    public interface IService
    {
        Task<FacetFilterViewModel> GetFacetFilterAsync(string type, FacetFilterRequest model);
    }
}