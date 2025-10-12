using Microsoft.AspNetCore.Mvc;
using KMS.Web.ViewModels.Shared.UC;

namespace KMS.Web.ViewComponents.UC
{
	[ViewComponent]
	public class UCTable : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync(string id, bool is_reponsive = true)
		{
            UCTableViewModel model = new UCTableViewModel();
			model.id = id;
			model.is_reponsive = is_reponsive;
            return View(model);
		}
	}
}


