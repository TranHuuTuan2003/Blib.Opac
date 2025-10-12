

namespace KMS.Web.Common
{
    public class PublishViewPaths
    {
        public static string home = "Views/Publish/Home/";
        public static string search = "Views/Publish/Search/";
        public static string collection = "Views/Publish/Collection/";
        public static string detail = "Views/Publish/Detail/";
        public static string digital_file = "Views/Publish/DigitalFile/";
        public static string errors = "Views/Publish/Errors/";
        public static string reader = "Views/Publish/Reader/";
        public static string auth = "Views/Publish/Auth/";
        public static string register_borrowing = "Views/Publish/RegisterBorrowing/";
        public static string chatbot = "Views/Publish/Chatbot/";

        public static List<string> GetPathsAsList()
        {
            return new List<string>
            {
                home
            };
        }
    }
}




