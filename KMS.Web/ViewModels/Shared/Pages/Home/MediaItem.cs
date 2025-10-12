using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KMS.Web.ViewModels.Shared.Components.Home
{
    public class MediaItem
    {
        public string Url { get; set; } = "";
        public bool IsVideo { get; set; } = false;
        public string? VideoLink { get; set; } = "";
    }

}