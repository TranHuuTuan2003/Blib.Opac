using KMS.Shared.DTOs.Document;
using KMS.Web.Common;

namespace KMS.Web.ViewModels.Shared.Components.Home
{
    public class HomeViewModel
    {
        public ReachStatisticsDisplay ReachStatisticsDisplay { get; set; }
        public ContactInformation ContactInformation { get; set; }
        public List<MediaItem> MediaItems { get; set; }
        public List<CollectionDto> Collections { get; set; }
        public List<DocumentNew> DocumentNews { get; set; }
        public List<HomeLibrary> HomeLibraries { get; set; }

        public HomeViewModel()
        {
            ReachStatisticsDisplay = new ReachStatisticsDisplay();
            ContactInformation = new ContactInformation();
            MediaItems = new List<MediaItem>
            {
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/1.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/2.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/3.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/4.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/5.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/6.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/7.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/8.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/9.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/10.png" }
            };
            HomeLibraries = new List<HomeLibrary>
            {

                new HomeLibrary{ Name = "Thư viện quân đội", Description ="Thư viện khoa học tổng hợp về quân sự cấp nhà nước, là cơ quan nghiệp vụ đầu ngành của hệ thống thư viện.", Img = ConstLocation.value + "/img/home/thu_vien_quan_doi.webp", DocumentCount = 10000000, isHighlightedLib = true, Logo = ConstLocation.value + "/img/home/logo/1.png" },
                new HomeLibrary{ Name = "Thư viện Quốc gia", Description ="Thư viện Quốc gia Việt Nam, thư viện trung tâm của cả nước, trực thuộc Bộ VHTTDL", Img = ConstLocation.value + "/img/home/thu_vien_quoc_gia.webp", DocumentCount = 10000000, isHighlightedLib = true, Logo = ConstLocation.value + "/img/home/logo/1.png" },
                new HomeLibrary{ Name = "Thư viện khoa học tổng hợp Đà Nẵng", Description ="Thư viện cung cấp nguồn tài liệu phong phú về khoa học, xã hội và tài liệu địa chí Đà Nẵng.", Img = ConstLocation.value + "/img/home/thu_vien_khoa_hoc_tong_hop_da_nang.webp", DocumentCount = 10000000, isHighlightedLib = true, Logo = ConstLocation.value + "/img/home/logo/1.png" },
                new HomeLibrary{ Name = "Thư viện Trường Đại học Ngoại Thương", Description ="Đơn vị trực thuộc trường, quản lý và cung cấp tài nguyên thông tin phục vụ giảng dạy, học tập và nghiên cứu.", Img = ConstLocation.value + "/img/home/thu_vien_truong_dai_hoc_ngoai_thuong.webp", DocumentCount = 10000000, isHighlightedLib = true, Logo = ConstLocation.value + "/img/home/logo/1.png" },
                new HomeLibrary{ Name = "Thư viện Đại học Cần Thơ", Description ="Thư viện ĐH Cần Thơ quản lý, phát triển và khai thác học liệu, đồng thời liên kết với các cơ sở giáo dục và thư viện địa phương để phục vụ hiệu quả.", Img = ConstLocation.value + "/img/home/thu_vien_dai_hoc_can_tho.webp", DocumentCount = 10000000, isHighlightedLib = true, Logo = ConstLocation.value + "/img/home/logo/1.png" },
                new HomeLibrary{ Name = "Thư viện Yên Bái", Description ="Thư viện tỉnh Yên Bái được thành lập từ tháng 5 năm 1959 và có vai trò quan trọng trong việc lưu giữ và phát triển tri thức cho tỉnh.", Img = ConstLocation.value + "/img/home/thu_vien_yen_bai.webp", DocumentCount = 10000000, isHighlightedLib = true, Logo = ConstLocation.value + "/img/home/logo/1.png" },
                // new HomeLibrary{ Name = "Thư viện khoa học tổng hợp Đà Nẵng", Description ="Thư viện Quốc gia Việt Nam, thư viện trung tâm của cả nước, trực thuộc Bộ VHTTDL", Img = ConstLocation.value + "/img/home/thu_vien_khoa_hoc_tong_hop_da_nang.png", DocumentCount = 10000000, isHighlightedLib = true, Logo = ConstLocation.value + "/img/home/logo/1.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/2.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/3.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/4.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/5.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/6.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/7.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/8.png" },
                new HomeLibrary{Logo = ConstLocation.value + "/img/home/logo/9.png" }
            };
        }
    }
}