using Microsoft.AspNetCore.Mvc;

using KMS.Api.Filters;
using KMS.Api.Services;
using KMS.Shared.DTOs.Document;
using KMS.Shared.Helpers;

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

        
    }
}