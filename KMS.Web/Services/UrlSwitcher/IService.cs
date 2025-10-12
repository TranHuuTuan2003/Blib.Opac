
namespace KMS.Web.Services.UrlSwitcher
{
    public interface IService
    {
        string GetSwitchedUrl(string serviceName, bool usePrivate = true);
        string GetPublicUrl(string originalUrl);
        (string url, bool isHideLocation) GetPrivateUrl(string originalUrl);
    }
}