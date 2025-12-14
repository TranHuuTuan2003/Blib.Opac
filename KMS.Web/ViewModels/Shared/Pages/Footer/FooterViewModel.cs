namespace KMS.Web.ViewModels.Shared.Components.Footer
{
    public class FooterViewModel
    {
        public KMS.Shared.DTOs.Footer.Footer? footer { get; set; }
        public int daily_access_count { get; set; }
        public int total_access_count { get; set; }
    }
}