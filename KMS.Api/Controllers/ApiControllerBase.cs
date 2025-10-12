using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using KMS.Api.Infrastructure.Authorization;
using KMS.Api.Services;
using KMS.Shared.Helpers;

using UC.Core.Common;
using UC.Core.Interfaces;
using UC.Core.Models;

namespace KMS.Api.Controllers
{
    [ApiController]
    [AuthorizeFilter]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase<TKeyId, TEntity> : ControllerBase
    {
        private readonly ServiceDecorator<TKeyId, TEntity> _serviceDecorator;
        private readonly ILogger _logger;
        private readonly IUserProvider _userProvider;
        private readonly IMemoryCache _cache;

        public ApiControllerBase(IServiceWrapper service, ILogger logger, IUserProvider userProvider, IMemoryCache cache)
        {
            _userProvider = userProvider;
            _logger = logger;
            _serviceDecorator = new ServiceDecorator<TKeyId, TEntity>(service);
            _cache = cache;
        }

        #region base actions
        [HttpGet("get-items")]
        [AuthorizeFilter]
        public virtual async Task<IActionResult> GetEntitiesAsync(bool? caching, bool? clearCache, string? columnsQuery, string? whereQuery, string? orderQuery)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                Type t = typeof(TEntity);
                List<TEntity> items = null;
                if (clearCache == true)
                {
                    _cache.Remove(t.Name);
                }
                if (_cache.TryGetValue(t.Name, out List<TEntity> data) && caching.HasValue && caching.Value)
                {
                    items = data;
                }
                else
                {
                    items = await _serviceDecorator.GetEntitiesAsync(columnsQuery, whereQuery, orderQuery);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(1));
                    _cache.Set(t.Name, items, cacheEntryOptions);
                }
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-item-by-id/{id}")]
        [AuthorizeFilter]
        public virtual async Task<IActionResult> GetEntityByIdAsync(TKeyId id)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var item = await _serviceDecorator.GetEntityByIdAsync(id);
                return ResponseMessage.Success(item);
            });
        }

        [HttpPost("insert-item")]
        [AuthorizeFilter]
        public virtual async Task<IActionResult> InsertEntityAsync([FromBody] TEntity model)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var item = await _serviceDecorator.InsertEntityAsync(model);
                Type t = typeof(TEntity);
                _cache.Remove(t.Name);
                return ResponseMessage.Success(item, Consts.Message.DATABASE_INSERT_SUCCESS);
            });
        }

        [HttpPut("update-item")]
        [AuthorizeFilter]
        public virtual async Task<IActionResult> UpdateEntityAsync([FromBody] TEntity model)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var item = await _serviceDecorator.UpdateEntityAsync(model);
                Type t = typeof(TEntity);
                _cache.Remove(t.Name);
                return ResponseMessage.Success(item, Consts.Message.DATABASE_UPDATE_SUCCESS);
            });
        }

        [HttpDelete("delete-item/{id}")]
        [AuthorizeFilter]
        public virtual async Task<IActionResult> DeleteEntityAsync(TKeyId id)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                await _serviceDecorator.DeleteEntityAsync(id);
                Type t = typeof(TEntity);
                _cache.Remove(t.Name);
                return ResponseMessage.Success(null, Consts.Message.DATABASE_DELETE_SUCCESS);
            });
        }
        #endregion
    }
}