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
    public class DocumentController : ControllerBase
    {
        private readonly IServiceWrapper _service;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(
            IServiceWrapper service,
            ILogger<DocumentController> logger
            )
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("get-detail")]
        public async Task<IActionResult> GetDetailAsync(string slug)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.document.GetDetailAsync(slug);
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-slug")]
        public async Task<IActionResult> GetSlug(int? mfn, int? did)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var slug = await _service.document.GetSlugAsync(mfn, did);
                if (string.IsNullOrEmpty(slug)) return ResponseMessage.Error();
                return ResponseMessage.Success(slug);
            });
        }

        [OnlyAppAuthorization]
        [HttpPut("update-document-views")]
        public async Task<IActionResult> UpdateDocumentView(string slug)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                await _service.document.UpdateDocumentView(slug);
                return ResponseMessage.Success();
            });
        }

        [OnlyAppAuthorization]
        [HttpPut("sync-document-views")]
        public async Task<IActionResult> SyncDocumentViews(List<SyncView> docViews)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                await _service.document.SyncDocumentViews(docViews);
                return ResponseMessage.Success();
            });
        }

        [HttpGet("related-documents")]
        public async Task<IActionResult> GetRelatedDocuments(string slug, int limit)
        {
            try
            {
                var items = await _service.document.GetRelatedDocuments(slug, limit);
                return ResponseMessage.Success(items);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
          
        }

        [HttpGet("get-borrowing-documents")]
        public async Task<IActionResult> GetDocsBorrowingAsync(string card_no)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.document.GetDocsBorrowingAsync(card_no);
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-extended-documents")]
        public async Task<IActionResult> GetDocsExtendAsync(string card_no)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.document.GetDocsExtendAsync(card_no);
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-returned-documents")]
        public async Task<IActionResult> GetDocsReturnedAsync(string card_no)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.document.GetDocsReturnedAsync(card_no);
                return ResponseMessage.Success(items);
            });
        }

        // [HttpGet("Marc21")]
        // public async Task<IActionResult> GetMarc21Async(string slug)
        // {
        //     if (string.IsNullOrEmpty(slug))
        //     {
        //         return ResponseMessage.Error();
        //     }

        //     _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
        //     try
        //     {
        //         var items = await _service.document.GetMarc21Async(slug);
        //         return ResponseMessage.Success(items);
        //     }
        //     catch (CustomException<DocMarc21> ex)
        //     {
        //         _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
        //         return ResponseMessage.Error(ex.ExtraValue, ex.Message);
        //     }
        // }

        // [HttpGet("DublinCore")]
        // public async Task<IActionResult> GetDublinCoreAsync(string slug)
        // {
        //     if (string.IsNullOrEmpty(slug))
        //     {
        //         return ResponseMessage.Error();
        //     }

        //     _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
        //     try
        //     {
        //         var items = await _service.document.GetDublinCoreAsync(slug);
        //         return ResponseMessage.Success(items);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
        //         return ResponseMessage.Error(ex.Message);
        //     }
        // }

        // [HttpGet("RegisterCir")]
        // public async Task<IActionResult> GetRegisterCirAsync(int id)
        // {
        //     return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
        //     {
        //         var items = await _service.document.GetRegisterCirAsync(id);
        //         return ResponseMessage.Success(items);
        //     });
        // }

        [HttpGet("get-marc")]
        public async Task<IActionResult> GetMarcByMfn(int mfn)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.document.GetMarcByMfn(mfn);
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-top-documet-new")]
        public async Task<IActionResult> GetTop12BibNew()
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.document.GetTop12BibNew();
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-top-documet-hot")]
        public async Task<IActionResult> GetTop6BibHot()
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.document.GetTop6BibHot();
                return ResponseMessage.Success(items);
            });
        }

        [HttpGet("get-top-bib-collection")]
        public async Task<IActionResult> GetTopBibCollection()
        {
            try
            {
                var items = await _service.document.GetTopBibCollection();
                return ResponseMessage.Success(items);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }

        [HttpGet("get-file-pdf")]
        public async Task<IActionResult> GetFile(string id)
        {
            try
            {
                var items = await _service.document.GetFile(id);
                return ResponseMessage.Success(items);
            }
            catch (Exception ex)
            {
                return ResponseMessage.Error(ex.Message);
            }
        }
    }
}