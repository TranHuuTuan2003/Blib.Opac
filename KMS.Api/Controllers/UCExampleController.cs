using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using KMS.Api.Services;
using KMS.Shared.Helpers;

using UC.Core.Interfaces;
using UC.Core.Models;

namespace KMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UCExample
    {
        private readonly IUserProvider _userProvider;
        private readonly IServiceWrapper _service;
        private readonly ILogger<UCExample> _logger;
        private readonly IMemoryCache _memoryCache;

        public UCExample(IServiceWrapper service, ILogger<UCExample> logger, IUserProvider userProvider, IMemoryCache memoryCache)
        {
            _service = service;
            _logger = logger;
            _userProvider = userProvider;
            _memoryCache = memoryCache;
        }

        [HttpPost("insert-item")]
        public virtual async Task<IActionResult> InsertItemAsync([FromBody] Dtos.UcExample model)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var item = await _service.uc_sample.InsertItemAsync(model);
                return ResponseMessage.Success(item);
            });
        }

        [HttpPut("update-item")]
        public virtual async Task<IActionResult> UpdateItemAsync([FromBody] Dtos.UcExample model)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var item = await _service.uc_sample.UpdateItemAsync(model);
                return ResponseMessage.Success(item);
            });
        }

        [HttpGet("get-items")]
        public virtual async Task<IActionResult> GetItemsAsync()
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.uc_sample.GetItemsAsync();
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-item-by-id")]
        public virtual async Task<IActionResult> GetItemByIdAsync(string id)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var item = await _service.uc_sample.GetItemByIdAsync(id);
                return ResponseMessage.Success(item);
            });
        }

        [HttpGet("delete-item")]
        public virtual async Task<IActionResult> DeleteItemAsync(string id)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                await _service.uc_sample.DeleteItemAsync(id);
                return ResponseMessage.Success();
            });
        }
    }
}