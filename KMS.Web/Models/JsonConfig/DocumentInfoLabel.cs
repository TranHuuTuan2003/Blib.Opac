namespace KMS.Web.Models.JsonConfig
{
    public class DocumentInfoLabel
    {
        public string key { get; set; }
        public string label { get; set; }
    }

    public class DocumentInfoLabelByLanguage
    {
        public List<DocumentInfoLabel> labels_vi { get; set; } = new();
        public List<DocumentInfoLabel> labels_en { get; set; } = new();
    }
}