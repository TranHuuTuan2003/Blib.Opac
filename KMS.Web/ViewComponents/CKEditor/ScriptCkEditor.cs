using Microsoft.AspNetCore.Mvc;

namespace KMS.Web.ViewComponents.CKEditor
{
    [ViewComponent]
    public class ScriptCkEditor : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
