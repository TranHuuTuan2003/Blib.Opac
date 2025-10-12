namespace KMS.Web.Services.UrlSwitcher
{
    public class Service : IService
    {
        private IConfiguration _configuration;

        public Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetSwitchedUrl(string serviceName, bool usePrivate)
        {
            try
            {
                string serviceUrl = _configuration.GetSection($"Services:{serviceName}").Value;
                string baseUrl = usePrivate ? _configuration.GetSection($"ConfigDNS:{serviceName}:Private").Value : _configuration.GetSection($"ConfigDNS:{serviceName}:Public").Value;
                if (baseUrl == null) return serviceUrl;

                string path = new Uri(serviceUrl).PathAndQuery;
                return baseUrl + path;
            }
            catch (Exception ex)
            {
                return _configuration.GetSection($"Services:{serviceName}").Value;
            }
        }

        public string GetPublicUrl(string originalUrl)
        {
            try
            {
                string url = _configuration.GetSection($"ConfigDNS:{originalUrl}:Public").Value;
                if (url == null) return "";
                return url;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public (string url, bool isHideLocation) GetPrivateUrl(string originalUrl)
        {
            try
            {
                bool isHideLocation = (_configuration.GetSection($"ConfigDNS:{originalUrl}:HiddenLocation").Value ?? "false") != "false";
                string url = _configuration.GetSection($"ConfigDNS:{originalUrl}:Private").Value;
                if (url == null) return ("", isHideLocation);
                return (url, isHideLocation);
            }
            catch (Exception ex)
            {
                return ("", false);
            }
        }
    }
}