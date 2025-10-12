namespace KMS.Web.Common
{
	public class Enums
	{
		public enum AppConfigType
		{
			QG,
			DH,
			Tinh,
			Huyen
		}

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

		public enum AttachmentType
		{
			None = 0,
			OnlyDigitalFile = 1,
			OnlyAudioSummary = 2,
			HasBoth = 3
		}

		public enum StatisticType
		{
			VIEW_DDOC = 27,
			VIEW_PDOC = 28,
			VIEW_ADOC = 29,
			VIEW_ISBD = 31,
			VIEW_MARC21 = 33,
			VIEW_DUBLIN_CORE = 34,
			PREVIEW_DIGITAL_FILE = 35,
			READ_DIGITAL_FILE = 36,
			DOWNLOAD_DIGITAL_FILE = 37,
			QUICK_SEARCH = 20,
			BASIC_SEARCH = 21,
			ADVANCE_SEARCH = 22,
			UNKNOWN_SEARCH = 23,
			SUCCESSFULLY_LOGIN = 24,
			LIKE_DDOC = 40,
			LIKE_PDOC = 41,
			LIKE_ADOC = 42,
			DISLIKE_DDOC = 43,
			DISLIKE_PDOC = 44,
			DISLIKE_ADOC = 45
		}
	}

	public static class TenantTypeExtensions
	{
		public static string ToConfigKey(this Enums.TenantType tenant)
		{
			return tenant == Enums.TenantType.ucvn ? "default" : tenant.ToString();
		}
	}
}
