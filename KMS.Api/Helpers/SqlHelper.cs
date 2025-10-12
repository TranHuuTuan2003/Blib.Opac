namespace KMS.Api.Helpers
{
    public static class SqlHelper
    {
        public static string ToLikeParam(this string value) => string.IsNullOrWhiteSpace(value) ? "%" : $"%{value.Trim().ToLower()}%";
    }
}