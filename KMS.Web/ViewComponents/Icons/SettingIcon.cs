using Microsoft.AspNetCore.Mvc;

namespace KMS.Web.ViewComponents.Icons
{
	[ViewComponent]
	public class SettingIcon : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}


