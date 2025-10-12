using Microsoft.AspNetCore.Mvc;

using KMS.Api.Services;
using KMS.Shared.Helpers;

using UC.Core.Models;

namespace KMS.Api.Controllers.Publish
{
    [ApiController]
    [Route("api/[controller]")]
    public class PStaticPostController : ControllerBase
    {
        private readonly IServiceWrapper _service;
        private readonly ILogger<PStaticPostController> _logger;

        public PStaticPostController(
            IServiceWrapper service,
            ILogger<PStaticPostController> logger,
            IConfiguration configuration
            )
        {
            _service = service;
            _logger = logger;
        }

        // [HttpGet("get-static-post-by-code")]
        // public async Task<IActionResult> GetStaticPostByCode(string code)
        // {
        //     return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
        //     {
        //         var item = await _service.PStaticPost.GetStaticPostByCode(code);
        //         return ResponseMessage.Success(item);
        //     });
        // }
    }
}