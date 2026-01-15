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
        public StatisticsDto StatisticsDto { get; set; }
        public List<HomeLibrary> HomeLibraries { get; set; }
        public HomeViewModel()
        {
            ReachStatisticsDisplay = new ReachStatisticsDisplay();
            ContactInformation = new ContactInformation();
            MediaItems = new List<MediaItem>
            {
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/15.jpg"},
                new MediaItem { Url = "https://www.youtube.com/watch?v=6a9ZcotrVGQ&pp=ygUmdGjGsCB2aeG7h24gxJHhuqFpIGjhu41jIMSRaeG7h24gbOG7sWM%3D" , IsVideo = true},
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/14.jpg" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/13.jpg" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/11.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/12.jpg" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/9.png" },
                new MediaItem { Url = ConstLocation.value + "/img/home/media-block/1.png" }
            };
        }
    }
}