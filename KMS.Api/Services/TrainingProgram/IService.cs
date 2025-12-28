using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.Search;
using KMS.Shared.DTOs.TrainingProgram;
using KMS.Shared.DTOs.Tree;

namespace KMS.Api.Services.TrainingProgram
{
    public interface IService
    {
        Task<List<object>> GetTrainingProgram();
        Task<List<ListSection>> GetListSectionFalse(string id);
        Task<List<ListSection>> GetListSectionTrue(string id);
        Task<List<DetailSystem>> GetDetailSystem(string id);
    }
}