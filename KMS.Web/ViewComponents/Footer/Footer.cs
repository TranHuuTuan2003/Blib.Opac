using Microsoft.AspNetCore.Mvc;

using KMS.Shared.Helpers;
using KMS.Web.Core;
using KMS.Web.Helpers;
using KMS.Web.ViewModels.Shared.Components.Footer;

using Uc.Services.Dtos.Lms;
using Uc.Services.Services.Lms;

namespace KMS.Web.ViewComponents.Footer
{
    [ViewComponent]
    public class Footer : ViewComponent
    {
        private readonly Services.PLog.IService _plogService;
        private readonly JsonConfigCacheService<Shared.DTOs.Footer.Footer> _footerService;
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ILogger<Footer> _logger;

        public Footer(
            Services.PLog.IService plogService,
            JsonConfigCacheService<Shared.DTOs.Footer.Footer> footerService,
            ApiHelper apiHelper,
            AppConfigHelper appConfigHelper,
            ILogger<Footer> logger
            )
        {
            _footerService = footerService;
            _plogService = plogService;
            _apiHelper = apiHelper;
            _appConfigHelper = appConfigHelper;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool logoType = true)
        {
            Shared.DTOs.Footer.Footer savedConfig = _footerService.GetConfig();
            string address = _appConfigHelper.GetApiApp();

            if (savedConfig == null)
            {
                savedConfig = new();
            }

            if ((string.IsNullOrEmpty(savedConfig.footer_info?.content) && string.IsNullOrEmpty(savedConfig.footer_copyright_social?.content)) || (string.IsNullOrEmpty(savedConfig.footer_info?.content_en) && string.IsNullOrEmpty(savedConfig.footer_copyright_social?.content_en)))
            {
                try
                {
                    var footer = await _apiHelper.GetApiResponseAsync<Shared.DTOs.Footer.Footer>($"{address}PStaticPost/get-static-post-by-code?code=footer-info");
                    if (savedConfig.footer_info == null)
                    {
                        savedConfig.footer_info = new();
                        savedConfig.footer_info.content = footer?.Data?.footer_info?.content ?? string.Empty;
                        savedConfig.footer_info.content_en = footer?.Data?.footer_info?.content_en ?? string.Empty;
                        savedConfig.footer_info.is_active = footer?.Data?.footer_info?.is_active ?? false;
                    }
                    if (savedConfig.footer_copyright_social == null)
                    {
                        savedConfig.footer_copyright_social = new();
                        savedConfig.footer_copyright_social.content = footer?.Data?.footer_copyright_social?.content ?? string.Empty;
                        savedConfig.footer_copyright_social.content_en = footer?.Data?.footer_copyright_social?.content_en ?? string.Empty;
                        savedConfig.footer_copyright_social.is_active = footer?.Data?.footer_copyright_social?.is_active ?? false;
                    }
                    _footerService.SaveConfig(savedConfig);
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError(_logger, ex, ex.Message);
                    savedConfig = new();
                }
            }

            await _plogService.CreateLogDailyAsync(HttpContext);
            hit_count hitCount = await LogService.ReadHitCountAsync();
            var model = new FooterViewModel();
            model.footer = savedConfig;
            model.daily_access_count = hitCount.daily;
            model.total_access_count = hitCount.total;
            return View(model);
        }
    }
}