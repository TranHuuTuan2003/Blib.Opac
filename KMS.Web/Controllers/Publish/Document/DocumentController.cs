using Microsoft.AspNetCore.Mvc;

using KMS.Shared.DTOs.Document;
using KMS.Shared.Helpers;
using KMS.Web.Services.Document;
using KMS.Web.ViewModels.Shared.Components.DocumentDetail;

namespace KMS.Web.Controllers.Publish.Document
{
    public class DocumentController : Controller
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IService _service;

        public DocumentController(ILogger<DocumentController> logger, IService service)
        {
            _logger = logger;
            _service = service;
        }

        [Route("chi-tiet-tai-lieu/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            try
            {
                var document = _service.GetDocumentDetailAsync(slug);
                var document_lq = _service.GetRelatedDocumentsAsync(slug);
                var document_hot = _service.GetTop6BibHot();

                await Task.WhenAll([document, document_lq, document_hot]);

                var vm = new DocumentDetailViewModel
                {
                    Document = document.Result ?? new(),
                    RelatedDocuments = document_lq.Result,
                    HotDocuments = document_hot.Result
                };

                return View("~/Views/Document/Detail.cshtml", vm);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
                return NotFound();
            }
        }


        //[HttpPost("tai-lieu-lien-quan")]
        //public async Task<IActionResult> GetRelatedDocuments(string slug)
        //{
        //    try
        //    {
        //        var items = await _service.GetRelatedDocumentsAsync(slug);
        //        return PartialView("DocumentDetail/_RelatedDocuments", model: items);
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerHelper.LogError(_logger, ex, ex.Message);
        //        return PartialView("DocumentDetail/_RelatedDocuments", model: new List<Result>());
        //    }
        //}

        [Route("Document/Borrow")]
        public IActionResult Detail()
        {
            return View("~/Views/Document/Borrow.cshtml");
        }
    }
}
