using KMS.Shared.DTOs.TrainingProgram;

namespace KMS.Web.Services.TrainingProgram
{
    public interface IService
    {
        Task<TrainingPrograms> GetDetailAsync(string id);
    }
}