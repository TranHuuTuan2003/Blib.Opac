namespace KMS.Shared.DTOs.Auth.Login
{
    public class LoginResponse
    {
        public string id { get; set; }
        public string? loginname { get; set; }
        public string? name { get; set; }
        public string? avatar { get; set; }
        public DateTime? birth_date { get; set; }
        public string? ic_no { get; set; }
        public string? sex { get; set; }
        public string? tel { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public string? reader_digital_type_name { get; set; }
        public string? reader_digital_type_id { get; set; }
        public List<string> cards_no { get; set; } = new();
        public string? access_token { get; set; }
        public string? class_name { get; set; }
        public string? course_name { get; set; }
        public string? department_name { get; set; }
    }
}
