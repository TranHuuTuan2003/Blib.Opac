namespace KMS.Web.ViewModels.Shared.Admin
{
    public class CropperFrameViewModel
    {
        public string modalTitle { get; set; } = "Cắt ảnh";
        public string aspectRatio { get; set; } = "3 / 4";
        public string ratioPath { get; set; } = "img/media/media-3x4.webp";
        public int maxSize { get; set; } = 2;
        public string hiddenKeyCode { get; set; } = "";
        public string refType { get; set; } = "";
        public string folderCode { get; set; } = "";
        // Code của input hidden để lưu url được trả về sau khi tải lên
        public string hiddenUrlCode { get; set; } = "";
        // Sự kiện xử lý sau khi tải ảnh thành công, trả về url ảnh
        public string eventHandle { get; set; } = "";
        public bool needActiveButton { get; set; } = false;
        public bool needBtnDeleteImage { get; set; } = false;
    }
}