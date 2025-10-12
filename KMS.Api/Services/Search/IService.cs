using KMS.Shared.DTOs.FacetFilter;
using KMS.Shared.DTOs.Search;

namespace KMS.Api.Services.Search
{
	public interface IService
	{
		Task<SearchResponse> SearchingAsync(string type, int page, int pageSize, SearchBody searchRequest);
		Task<List<FacetFilterResponse>> FacetFilterAsync(string type, FacetFilterRequest facetFilterRequest);
	}
}
