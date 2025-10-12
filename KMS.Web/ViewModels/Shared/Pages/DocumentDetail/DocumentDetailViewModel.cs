using KMS.Shared.DTOs.Document;

namespace KMS.Web.ViewModels.Shared.Components.DocumentDetail
{
    public class DocumentDetailViewModel
    {
        public object Document { get; set; }
        public List<Result> RelatedDocuments { get; set; }
        public List<Result> HotDocuments { get; set; }
    }

}
