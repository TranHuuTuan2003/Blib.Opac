using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.Search;
using KMS.Shared.DTOs.Tree;

namespace KMS.Api.Services.TrainingProgram
{
    public interface IService
    {
        Task<List<object>> GetTrainingProgram();
    }
}