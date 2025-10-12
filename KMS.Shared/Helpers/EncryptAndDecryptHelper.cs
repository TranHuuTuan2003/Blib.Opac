using System.Text;

namespace KMS.Shared.Helpers
{
    public static class EncryptAndDecryptHelper
    {
        private static readonly string SecretKey = "V+H9D`fX26pR@T6+(P@{-)hCmjUFxY.h{UDtwUm@";

        public static string EncryptToken(string token, int expiryMinutes = -1)
        {
            long expiryTimestamp = expiryMinutes > 0 ? DateTimeOffset.UtcNow.AddMinutes(expiryMinutes).ToUnixTimeSeconds() : -1;
            string data = $"{token}|{expiryTimestamp}";
            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);

            byte[] encryptedBytes = XorBytes(plainBytes, keyBytes);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string? DecryptToken(string encryptedToken)
        {
            byte[] encryptedBytes;
            try
            {
                encryptedBytes = Convert.FromBase64String(encryptedToken);
            }
            catch
            {
                return null;
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);
            byte[] decryptedBytes = XorBytes(encryptedBytes, keyBytes);
            string decryptedData;

            try
            {
                decryptedData = Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return null;
            }

            var parts = decryptedData.Split('|');
            if (parts.Length != 2) return null;

            string token = parts[0];
            if (!long.TryParse(parts[1], out long expiryTimestamp)) return null;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (expiryTimestamp != -1)
            {
                if (now > expiryTimestamp) return null;
            }

            return token;
        }

        private static byte[] XorBytes(byte[] data, byte[] key)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key[i % key.Length]);
            }
            return result;
        }
    }
}
