using KMS.Shared.DTOs.Menu;

namespace KMS.Web.ViewModels.Shared.Components.SidebarMenu
{
    public class SidebarMenuViewModel
    {
        public bool is_logged_in { get; set; }
        public List<Menu> menu { get; set; } = new();
    }
}