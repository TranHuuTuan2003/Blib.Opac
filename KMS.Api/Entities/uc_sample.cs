using UC.Core.Models;

namespace KMS.Api.Entities
{
	public class uc_sample : BaseEntity<string>
	{
		public string? name { get; set; }
		public int? number { get; set; }
		public DateTime? date { get; set; }
		public bool? boolean { get; set; }
	}
}
