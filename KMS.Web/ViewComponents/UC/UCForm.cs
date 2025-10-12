using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UC.Core.Common;
using UC.Core.Helpers;
using UC.Core.Models;
using UC.Core.Models.UCFormSearchListConfig;
using KMS.Web.ViewModels.Shared.UC;
using UC.Core.Models.UCFormConfig;
using KMS.Web.Common;
using System.Text;
using KMS.Web.Services.UrlSwitcher;

namespace KMS.Web.ViewComponents.UC
{
    [ViewComponent]
    public class UCForm : ViewComponent
    {
        private readonly IService _service;

        public UCForm(IService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string FileName, bool? isFormSearch = false, string? type = "basic")
        {
            var client_site = Request.Cookies["client_site"];
            var lang = Request.Cookies["lang"];

            ClientRequestInfo clientRequestInfo = new ClientRequestInfo(string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host));
            HttpClientBuilder httpClientBuilder = new HttpClientBuilder(clientRequestInfo);
            string config_type = "";
            if (isFormSearch.HasValue && isFormSearch.Value)
            {
                config_type = "uc_form_search_list";
            }
            else
            {
                config_type = "uc_form";
            }

            (string webUrl, bool isHideLocation) = _service.GetPrivateUrl("WebUrl");
            ClientResponseInfo clientResponseInfo = httpClientBuilder.GetAsync(webUrl + (isHideLocation ? "" : ConstLocation.value) + "/" + "json" + "/" + client_site + "/" + lang + "/" + "config" + "/" + config_type + "/" + FileName).GetAwaiter().GetResult();
            UCFormViewModel model = new UCFormViewModel();
            if (clientResponseInfo.IsStatusCode)
            {
                if (isFormSearch.HasValue && isFormSearch.Value)
                {
                    UCFormSearchListConfig formSearchListConfig = JsonConvert.DeserializeObject<UCFormSearchListConfig>(clientResponseInfo.Content);
                    if (type == "basic")
                    {
                        if (formSearchListConfig.form != null)
                        {
                            model.html = UcHelper.DrawFormSearch(formSearchListConfig.form);
                        }
                    }
                    else if (type == "advance")
                    {
                        if (formSearchListConfig.form_advance != null)
                        {
                            model.html = UcHelper.DrawFormSearch(formSearchListConfig.form_advance);
                        }
                    }
                    else if (type == "search")
                    {
                        if (formSearchListConfig.form != null)
                        {
                            StringBuilder sb = new StringBuilder();

                            sb.AppendLine("<div class=\"row\">");
                            sb.AppendLine("    <div class=\"col-md-10\">");
                            sb.AppendLine(UcHelper.DrawFormSearch(formSearchListConfig.form));
                            sb.AppendLine("    </div>");
                            sb.AppendLine("    <div class=\"col-md-2 d-flex pb-2\">");
                            sb.AppendLine("        <div class=\"d-flex align-items-center\">");
                            sb.AppendLine("            <a id=\"btn-search\" class=\"btn btn-sm btn-primary w-100 me-2\">");
                            sb.AppendLine("                <i class=\"fa-solid fa-search\"></i>");
                            sb.AppendLine("            </a>");
                            sb.AppendLine("            <a id=\"btn-redo\" class=\"btn btn-sm btn-secondary w-100\">");
                            sb.AppendLine("                <i class=\"fa-solid fa-redo\"></i>");
                            sb.AppendLine("            </a>");
                            sb.AppendLine("        </div>");
                            if (formSearchListConfig.form_advance != null)
                            {
                                sb.AppendLine("        <div class=\"ms-2 d-flex align-items-center\" data-bs-toggle=\"modal\" data-bs-target=\"#modalSearchAdvance\">");
                                sb.AppendLine("            <a data-container=\"body\" data-bs-toggle=\"tooltip\" data-bs-placement=\"top\" title=\"\" data-bs-original-title=\"Tìm kiếm nâng cao\">");
                                sb.AppendLine("                <i class=\"fa-solid fa-sliders\"></i>");
                                sb.AppendLine("            </a>");
                                sb.AppendLine("        </div>");
                            }
                            sb.AppendLine("    </div>");
                            sb.AppendLine("</div>");
                            sb.AppendLine("<hr />");

                            model.html = sb.ToString();
                        }
                    }
                }
                else
                {
                    UCFormConfig formConfig = JsonConvert.DeserializeObject<UCFormConfig>(clientResponseInfo.Content);
                    model.html = UcHelper.DrawFormSearch(formConfig.form);
                }
            }

            return View(model);
        }
    }
}


