namespace KMS.Web.Areas.Admin.Models.Auth
{
    public class LoginModel
    {
        public string? name { get; set; } = string.Empty;
        public string? description { get; set; } = string.Empty;
        public string? logo { get; set; } = string.Empty;
        public string? logo_login { get; set; } = string.Empty;
        public string? login_background { get; set; } = string.Empty;
        public string? sidebar_background { get; set; } = string.Empty;
    }
}