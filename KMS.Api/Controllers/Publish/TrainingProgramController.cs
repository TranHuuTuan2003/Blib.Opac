using KMS.Api.Filters;
using KMS.Api.Services;
using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.TrainingProgram;
using KMS.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using UC.Core.Models;

namespace KMS.Api.Controllers.Publish
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingProgramController : ControllerBase
    {
        private readonly IServiceWrapper _service;
        private readonly ILogger<TrainingProgramController> _logger;

        public TrainingProgramController(IServiceWrapper service, ILogger<TrainingProgramController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("get-list-training-program")]
        public async Task<IActionResult> GetTrainingProgram()
        {
            try
            {
                var items = await _service.training_program.GetTrainingProgram();
                return ResponseMessage.Success(items);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        [HttpGet("get-list-section-by-system-id")]
        public async Task<IActionResult> GetListSection(string id)
        {
            try
            {
                var model = new TrainingPrograms
                {
                    DetailSystem = await _service.training_program.GetDetailSystem(id),
                    ListSectionTrue = await _service.training_program.GetListSectionTrue(id),
                    ListSectionFalse = await _service.training_program.GetListSectionFalse(id)
                };

                return ResponseMessage.Success(model);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        [HttpGet("get-detail-section-by-id")]
        public async Task<IActionResult> GetDetailSection(string id)
        {
            try
            {
                var model = new TrainingSections
                {
                    DetailSection = await _service.training_program.GetDetailSection(id)
                };

                return ResponseMessage.Success(model);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }
    }
}