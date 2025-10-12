namespace KMS.Web.ViewModels.Shared.Pages.DigitalFile
{
    public class DigitalFileViewModel : KMS.Shared.DTOs.DigitalFile.DigitalFile
    {
        public bool is_error_dborrow { get; set; }
        public string error_dborrow_message { get; set; } = string.Empty;
    }
}