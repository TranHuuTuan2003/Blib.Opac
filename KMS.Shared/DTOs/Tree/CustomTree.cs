namespace KMS.Shared.DTOs.Tree
{
    public abstract class CustomTree<TParent>
    {
        public string id { get; set; }
        public string value { get; set; }
        public string text { get; set; }
        public string? parent_id { get; set; }
        public int? order_index { get; set; }
        public List<TParent> children { get; set; } = new List<TParent>();
    }
}
