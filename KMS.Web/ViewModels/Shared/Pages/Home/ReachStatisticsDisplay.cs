using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KMS.Web.ViewModels.Shared.Components.Home
{
    public class ReachStatisticsDisplay
    {
        public int UserCount { get; set; } = 100000;
        public int SearchCount { get; set; } = 500000;
        public int MemberLibraryCount { get; set; } = 10000;
        public int DocumentCount { get; set; } = 20000;

        public string UserCountDisplay => $"{UserCount / 1000}K+";
        public string SearchCountDisplay => $"{SearchCount / 1000}K+";
        public string MemberLibraryCountDisplay => $"{MemberLibraryCount/1000}K+";
        public string DocumentCountDisplay => $"{DocumentCount / 1000}K+";
    }
}
