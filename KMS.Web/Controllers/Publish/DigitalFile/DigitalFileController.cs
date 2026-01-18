using KMS.Shared.DTOs.DigitalFile;
using KMS.Shared.Helpers;
using KMS.Web.Common;
using KMS.Web.Services.DigitalFile;
using KMS.Web.ViewModels.Shared.Components.DigitalFile;
using KMS.Web.ViewModels.Shared.Components.DocumentDetail;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace KMS.Web.Controllers.Publish.DigitalFile
{
    public class DigitalFileController : Controller
    {
        private readonly ILogger<DigitalFileController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IService _service;

        public DigitalFileController(
            ILogger<DigitalFileController> logger,
            IWebHostEnvironment webHostEnvironment,
            IService service)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _service = service;
        }

        [HttpGet]
        [Route("doc-tai-lieu/{id}")]
        public async Task<IActionResult> GetFile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            try
            {
                Task<string> fileTask = _service.GetFile(id);
                Task<Seclever> secleverTask = _service.GetSecleverFile(id);

                await Task.WhenAll(fileTask, secleverTask);

                var vm = new DigitalFileViewPDF
                {
                    file = fileTask.Result,
                    seclever = (Seclever)(secleverTask.Result ?? new object())
                };

                return View("~/Views/DigitalFile/ViewPdf.cshtml", vm);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
                return NotFound();
            }
        }


        [HttpGet("proxy-pdf")]
        public async Task<IActionResult> ProxyPdf(string url, int previewPages)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL is required");

            try
            {
                using var httpClient = new HttpClient();
                var fileBytes = await httpClient.GetByteArrayAsync(url);

                using var inputStream = new MemoryStream(fileBytes);
                using var outputStream = new MemoryStream();

                var sourcePdf = PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);
                var outputPdf = new PdfDocument();

                // Nếu previewPages = 0 thì xem toàn bộ
                int effectivePreviewPages = previewPages == 0
                    ? sourcePdf.PageCount
                    : previewPages;

                for (int i = 0; i < sourcePdf.PageCount; i++)
                {
                    if (i < effectivePreviewPages)
                    {
                        // Trang được xem
                        outputPdf.AddPage(sourcePdf.Pages[i]);
                    }
                    else
                    {
                        // Trang bị khóa
                        var page = outputPdf.AddPage();
                        page.Width = sourcePdf.Pages[i].Width;
                        page.Height = sourcePdf.Pages[i].Height;

                        using var gfx = XGraphics.FromPdfPage(page);

                        var titleFont = new XFont("Arial", 26, XFontStyle.Bold);
                        var textFont = new XFont("Arial", 17);

                        double centerY = page.Height / 2;

                        // ===== TIÊU ĐỀ =====
                        gfx.DrawString(
                            "HẾT SỐ TRANG XEM TRƯỚC",
                            titleFont,
                            XBrushes.DarkBlue,
                            new XRect(0, centerY - 60, page.Width, 40),
                            XStringFormats.Center
                        );

                        // ===== DÒNG 1 =====
                        gfx.DrawString(
                            "Các trang tài liệu tiếp theo",
                            textFont,
                            XBrushes.Gray,
                            new XRect(0, centerY - 20, page.Width, 30),
                            XStringFormats.Center
                        );

                        // ===== DÒNG 2 =====
                        gfx.DrawString(
                            "yêu cầu đăng nhập để được xem tiếp.",
                            textFont,
                            XBrushes.Gray,
                            new XRect(0, centerY + 5, page.Width, 40),
                            XStringFormats.Center
                        );
                    }
                }

                outputPdf.Save(outputStream, false);
                outputStream.Position = 0;

                return File(
                    outputStream.ToArray(),
                    "application/pdf",
                    "preview.pdf"
                );
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
