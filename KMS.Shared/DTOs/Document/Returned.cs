namespace KMS.Shared.DTOs.Document
{
    public class Returned
    {
        public string id { get; set; } = string.Empty;
        public string reg_id { get; set; } = string.Empty;
        public string card_no { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string borrow_date { get; set; } = string.Empty;
        public string return_date { get; set; } = string.Empty;
    }
}