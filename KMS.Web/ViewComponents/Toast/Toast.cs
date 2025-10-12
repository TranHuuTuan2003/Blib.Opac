using Microsoft.AspNetCore.Mvc;

namespace KMS.Web.ViewComponents.Toast
{
	[ViewComponent]
	public class Toast : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
            return View();
		}
	}
}


