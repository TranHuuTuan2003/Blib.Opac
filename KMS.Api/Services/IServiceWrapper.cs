namespace KMS.Api.Services
{
    public interface IServiceWrapper
    {
        UCExample.IService uc_sample { get; }
        Search.IService search_service { get; }
        Document.IService document { get; }
    }
}
