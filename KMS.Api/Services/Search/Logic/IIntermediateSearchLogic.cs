using Dapper;

using KMS.Shared.DTOs.Search;

namespace KMS.Api.Services.Search.Logic
{
    public interface IIntermediateSearchLogic
    {
        (string, DynamicParameters) InitBuildQuerySearch(int lastId, SearchBody model);
        (string, DynamicParameters) QuickBuildQuerySearch(SearchBody model, string type = "search");
        (string, DynamicParameters) BasicBuildQuerySearch(SearchBody model, string type = "search");
        (string, DynamicParameters) AdvanceBuildQuerySearch(SearchBody searchRequest, string type = "search");
    }
}