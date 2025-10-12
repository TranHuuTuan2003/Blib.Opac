using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;

using KMS.Web.Common.Lang;
using KMS.Web.Helpers;
using KMS.Web.Services.Menu;
using KMS.Web.ViewModels.Shared.Components.SidebarMenu;

namespace KMS.Web.ViewComponents.SidebarMenu
{
    [ViewComponent]
    public class SidebarMenu : ViewComponent
    {
        private readonly AuthHelper _authHelper;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly IService _service;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IUrlHelper _urlHelper;

        public SidebarMenu(AuthHelper authHelper, AppConfigHelper appConfigHelper, IService service, IStringLocalizer<SharedResource> localizer, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _authHelper = authHelper;
            _appConfigHelper = appConfigHelper;
            _service = service;
            _localizer = localizer;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext!);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tenant = _appConfigHelper.GetTenantCode();
            var model = new SidebarMenuViewModel();
            model.is_logged_in = _authHelper.CheckAuthenticated();
            model.menu = await _service.GetMenuAsync();
            model.menu = model.menu.Where(item => item.is_active).ToList();

            // if (model.is_logged_in)
            // {
            //     foreach (var readerMenu in ReaderMenu.GetMenus(tenant))
            //     {
            //         if (!readerMenu.IsActive) continue;
            //         if (readerMenu.IsLogout) continue;

            //         model.menu.Add(new Opac.Shared.DTOs.Menu.Menu
            //         {
            //             name = _localizer[readerMenu.TextKey],
            //             name_eng = _localizer[readerMenu.TextKey],
            //             url = _urlHelper.Action(readerMenu.Action, readerMenu.Controller) ?? string.Empty,
            //             controller = readerMenu.Controller,
            //             action = readerMenu.Action,
            //             is_active = readerMenu.IsActive,
            //         });
            //     }
            // }

            return View(model);
        }
    }
}