using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Resources;
using System.Resources.NetStandard;
using System.Text.RegularExpressions;
using KMS.Web.Common;
using UC.Core.Models;


namespace KMS.Web.Controllers.Language
{
    public class LanguageController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly LangResource _langResource;

        public LanguageController(IWebHostEnvironment hostingEnvironment, LangResource langResource)
        {
            _hostingEnvironment = hostingEnvironment;
            _langResource = langResource;
        }

        [HttpGet]
        public IActionResult GetLanguageResource()
        {
            Dictionary<string,Dictionary<string, string>> langResource = new Dictionary<string, Dictionary<string, string>>();

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            string culture = requestCulture?.RequestCulture.Culture.Name;

            var contentRootPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Resources");

            DirectoryInfo contentDirectoryInfo;
            try
            {
                contentDirectoryInfo = new DirectoryInfo(contentRootPath);
            }
            catch (DirectoryNotFoundException)
            {
                // Here you should handle "Resources" directory not found exception.
                throw;
            }

            var resoruceFilesInfo = contentDirectoryInfo.GetFiles("*.resx", SearchOption.AllDirectories);
            string strContain = "." + culture + ".resx";
            var resoruceFiles = resoruceFilesInfo.Where(o => o.Name.Contains(strContain)).ToList();

            Dictionary<string, string> resources = null;
            foreach (var resxFile in resoruceFiles)
            {
                resources = new Dictionary<string, string>();

                using (ResXResourceReader resxSet = new ResXResourceReader(resxFile.FullName))
                {
                    foreach (DictionaryEntry entry in resxSet)
                    {
                        resources.Add(entry.Key.ToString(), entry.Value.ToString());
                    }
                }
                langResource.Add(resxFile.Name.Replace(strContain, ""), resources);
            }
            _langResource.data = langResource;

            return Ok(langResource);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Json(new { returnUrl });
        }
    }
}
