namespace KMS.Shared.DTOs.Document
{
    public class RegisteredCirculation
    {
        public int bibid { get; set; }
        public string statuscode { get; set; } = string.Empty;
        public string statusname { get; set; } = string.Empty;
        public string circulationid { get; set; } = string.Empty;
        public string circulationplace { get; set; } = string.Empty;
        public string storename { get; set; } = string.Empty;
        public DateTime duedate { get; set; }
        public string registername { get; set; } = string.Empty;
    }
}
