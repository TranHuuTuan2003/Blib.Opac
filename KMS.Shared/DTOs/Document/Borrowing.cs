namespace KMS.Shared.DTOs.Document
{
    public class Borrowing
    {
        public string id { get; set; } = string.Empty;
        public string reg_id { get; set; } = string.Empty;
        public string card_no { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string borrow_date { get; set; } = string.Empty;
        public string due_date { get; set; } = string.Empty;
        public string renew_status { get; set; } = string.Empty;
    }
}