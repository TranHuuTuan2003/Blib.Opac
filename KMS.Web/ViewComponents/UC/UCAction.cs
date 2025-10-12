using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UC.Core.Common;
using UC.Core.Helpers;
using UC.Core.Models;
using UC.Core.Models.UCFormSearchListConfig;
using KMS.Web.ViewModels.Shared.UC;
using UC.Core.Models.UCFormConfig;
using KMS.Web.Common;
using UC.Core.Models.UCActionConfig;
using KMS.Web.Services.UrlSwitcher;

namespace KMS.Web.ViewComponents.UC
{
    [ViewComponent]
    public class UCAction : ViewComponent
    {
        private readonly IService _service;

        public UCAction(IService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string FileName, string code, string type)
        {
            var client_site = Request.Cookies["client_site"];
            var lang = Request.Cookies["lang"];

            ClientRequestInfo clientRequestInfo = new ClientRequestInfo(string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host));
            HttpClientBuilder httpClientBuilder = new HttpClientBuilder(clientRequestInfo);
            string config_type = "";
            if (type == "uc_action")
            {
                config_type = "uc_action";
            }
            else if (type == "uc_form")
            {
                config_type = "uc_form";
            }
            else if (type == "uc_form_search_list")
            {
                config_type = "uc_form_search_list";
            }

            (string webUrl, bool isHideLocation) = _service.GetPrivateUrl("WebUrl");
            ClientResponseInfo clientResponseInfo = httpClientBuilder.GetAsync(webUrl + (isHideLocation ? "" : ConstLocation.value) + "/" + "json" + "/" + client_site + "/" + lang + "/" + "config" + "/" + config_type + "/" + FileName).GetAwaiter().GetResult();
            UCActionViewModel model = new UCActionViewModel();
            if (clientResponseInfo.IsStatusCode)
            {
                GroupAction? groupAction = null;
                if (config_type == "uc_action")
                {
                    UCActionConfig config = JsonConvert.DeserializeObject<UCActionConfig>(clientResponseInfo.Content);
                    groupAction = config.group_actions != null ? config.group_actions.FirstOrDefault(o => o.code == code) : null;
                }
                else if (config_type == "uc_form")
                {
                    UCFormConfig config = JsonConvert.DeserializeObject<UCFormConfig>(clientResponseInfo.Content);
                    groupAction = config.group_actions != null ? config.group_actions.FirstOrDefault(o => o.code == code) : null;
                }
                else if (config_type == "uc_form_search_list")
                {
                    UCFormSearchListConfig config = JsonConvert.DeserializeObject<UCFormSearchListConfig>(clientResponseInfo.Content);
                    groupAction = config.group_actions != null ? config.group_actions.FirstOrDefault(o => o.code == code) : null;
                }

                if (groupAction != null && groupAction.actions.Count > 0)
                {
                    model.html = UcHelper.DrawAction(groupAction.actions);
                }
            }
            return View(model);
        }
    }
}


