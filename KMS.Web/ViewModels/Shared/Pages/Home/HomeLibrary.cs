using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KMS.Web.ViewModels.Shared.Components.Home
{
    public class HomeLibrary
    {
        public string Name { set; get; } = "";
        public string Description { set; get; } = "";
        public string Img { set; get; } = "";
        public int DocumentCount { set; get; } = 0;
        public string Logo { set; get; } = "";
        public bool isHighlightedLib { set; get; } = false;
        // public string DocumentCountDisplay => $"{DocumentCount / 1000000}M+";
    }
}