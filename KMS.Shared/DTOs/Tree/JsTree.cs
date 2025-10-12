namespace KMS.Shared.DTOs.Tree
{
    public class JsTree
    {
        public string id { get; set; } = string.Empty;
        public string text { get; set; } = string.Empty;
        public List<JsTree> children { get; set; } = new();
    }
}
