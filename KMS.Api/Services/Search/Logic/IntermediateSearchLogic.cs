using Dapper;

using KMS.Api.Helpers;
using KMS.Shared.DTOs.Search;

namespace KMS.Api.Services.Search.Logic
{
    public class IntermediateSearchLogic : IIntermediateSearchLogic
    {
        private readonly IReadOnlyList<string> _tenantCodes;

        public IntermediateSearchLogic(AppConfigHelper appConfigHelper)
        {
            _tenantCodes = appConfigHelper.GetTenantCodes();
        }

        public (string, DynamicParameters) InitBuildQuerySearch(int lastId, SearchBody model)
        {
            return SearchQueryHelper.BuildInitQuerySearch(lastId, model, _tenantCodes);
        }

        public (string, DynamicParameters) QuickBuildQuerySearch(SearchBody model, string type = "search")
        {
            return SearchQueryHelper.BuildQuickQuerySearch(model, _tenantCodes, type);
        }

        public (string, DynamicParameters) BasicBuildQuerySearch(SearchBody model, string type = "search")
        {
            return SearchQueryHelper.BuildBasicQuerySearch(model, _tenantCodes, type);
        }

        public (string, DynamicParameters) AdvanceBuildQuerySearch(SearchBody searchRequest, string type = "search")
        {
            return SearchQueryHelper.BuildAdvanceQuerySearch(searchRequest, _tenantCodes, type);
        }
    }
}