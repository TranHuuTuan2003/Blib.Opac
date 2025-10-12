namespace KMS.Web.Common
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}