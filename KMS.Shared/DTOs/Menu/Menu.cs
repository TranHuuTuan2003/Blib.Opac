namespace KMS.Shared.DTOs.Menu
{
    public class Menu
    {
        public string id { get; set; }
        public string name { get; set; }
        public string name_eng { get; set; }
        public int order_index { get; set; }
        public bool is_active { get; set; }
        public string url { get; set; }
        public string target { get; set; }
        public string parent_id { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public List<Menu> childs { get; set; }
    }
}