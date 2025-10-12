namespace KMS.Shared.DTOs.Api
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
    }
}
