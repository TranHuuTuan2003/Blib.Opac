using Microsoft.AspNetCore.Mvc;

namespace KMS.Web.ViewComponents.Tree
{
    [ViewComponent]
    public class ScriptTree : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
