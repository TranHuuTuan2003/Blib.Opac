namespace KMS.Web.Core
{
    public class IdentityServerAuthentication
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public double AccessTokenExpiration { get; set; }

        public double RefreshTokenExpiration { get; set; }

        public bool RequireHttpsMetadata { get; set; }
    }
}