namespace KMS.Web.Common
{
    public class ConstLocation
    {
        public static string value { get; private set; }
        public static string source { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            value = configuration["AppConfig:Location"] ?? string.Empty;
            source = configuration["AppConfig:SourceType"] ?? string.Empty;
        }
    }
}
