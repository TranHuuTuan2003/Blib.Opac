using System.Security.Cryptography;
using System.Text;

namespace KMS.Shared.Helpers
{
    public static class HashHelper
    {
        public static string ComputeMD5Hash(string rawData)
        {
            try
            {
                if (string.IsNullOrEmpty(rawData))
                {
                    return "empty_hash";
                }

                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] bytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                    return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-").TrimEnd('=');
                }
            }
            catch
            {
                return "error_hash";
            }
        }
    }
}