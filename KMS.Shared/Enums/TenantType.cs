namespace KMS.Shared.Enums
{
    public enum TenantType
    {
        tvyb,
        tvftu,
        ftuqn,
        ucvn,
        tvbg,
        tvbn,
        nlv
    }

    public static class TenantTypeExtensions
    {
        public static string ToConfigKey(this TenantType tenant)
        {
            return tenant == TenantType.ucvn ? "default" : tenant.ToString();
        }
    }
}