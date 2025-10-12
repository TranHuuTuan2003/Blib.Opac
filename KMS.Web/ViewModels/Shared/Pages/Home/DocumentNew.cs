using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KMS.Web.ViewModels.Shared.Components.Home
{
    public class DocumentNew
    {
        public string title { get; set; } = "";
        public string cover_photo { get; set; } = "";
        public string slug { get; set; } = "";
             public string bib_type { get; set; } = "";
    }
}