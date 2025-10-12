using Microsoft.AspNetCore.Mvc;


namespace KMS.Web.ViewComponents.UC
{
    [ViewComponent]
    public class UCAuth : ViewComponent
    {
        private readonly IConfiguration _configuration;
        public UCAuth(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}


