namespace KMS.Shared.DTOs.Tree
{
	public class CollectionTree : CustomTree<CollectionTree>
	{
		public int? total_bib { get; set; }
		public bool? is_mobile { get; set; }
		public bool? is_home { get; set; }
	}

}
