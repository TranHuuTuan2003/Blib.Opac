namespace KMS.Api.Common
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}