using Microsoft.AspNetCore.Mvc;

using KMS.Shared.Helpers;
using KMS.Web.ViewModels.Shared.Pages.DigitalFile;
using KMS.Web.Common;

namespace KMS.Web.Controllers.Publish.DigitalFile
{
    public class DigitalFileController : Controller
    {
        private readonly ILogger<DigitalFileController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DigitalFileController(ILogger<DigitalFileController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool IsImageFile(string filePath)
        {
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var ext = Path.GetExtension(filePath);
            return extensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }

        private List<string> CreateFlipImagePathFromDirectory(string folderPath)
        {
            var images = new List<string>();

            if (Directory.Exists(folderPath))
            {
                var extensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

                var files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                    .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()));

                foreach (var file in files)
                {
                    // Trả về relative path so với wwwroot
                    var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, file)
                                           .Replace("\\", "/"); // chuẩn hóa cho web
                    images.Add(ConstLocation.value + "/" + relativePath);
                }
            }

            return images;
        }

        [Route("doc-tai-lieu")]
        public IActionResult ViewPdf()
        {
            var model = new DigitalFileViewModel();

            try
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", "flip-temp");
                var imageSources = CreateFlipImagePathFromDirectory(path);
                model.image_sources = imageSources;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
            }

            return View("~/Views/DigitalFile/ViewPdf.cshtml", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}