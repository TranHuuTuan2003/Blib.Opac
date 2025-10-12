using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KMS.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent BookmarkIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("bookmark", isFilled);
        }
        public static IHtmlContent DownLoad2Icon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("download_2", isFilled);
        }
        public static IHtmlContent WhiteBookmarkIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("bookmark_white", isFilled);
        }
        public static IHtmlContent BorrowIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("borrow", isFilled);
        }
        public static IHtmlContent CollectionIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("collection", isFilled);
        }
        public static IHtmlContent DownloadIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("download", isFilled);
        }
        public static IHtmlContent HomeIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("home", isFilled);
        }
        public static IHtmlContent InputSearchIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("input_search", isFilled);
        }
        public static IHtmlContent AdvancedSearchIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("input_search_advance", isFilled);
        }
        public static IHtmlContent SearchInputIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("search_input", isFilled);
        }
        public static IHtmlContent MicroPhoneIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("micro_phone", isFilled);
        }
        public static IHtmlContent InfoIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("info", isFilled);
        }
        public static IHtmlContent QRCodeIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("qrcode", isFilled);
        }
        public static IHtmlContent SearchIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("search", isFilled);
        }
        public static IHtmlContent ShareIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("share", isFilled);
        }
        public static IHtmlContent SliderIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("slider", isFilled);
        }
        public static IHtmlContent FilterIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("filter", isFilled);
        }
        public static IHtmlContent SmileIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("smile", isFilled);
        }
        public static IHtmlContent EllipseIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("ellipse", isFilled);
        }
        public static IHtmlContent RefreshIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("refresh", isFilled);
        }

        public static IHtmlContent PdfIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("pdf", isFilled);
        }

        public static IHtmlContent MediaIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("media", isFilled);
        }
        public static IHtmlContent RegisterBorrowingIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("register_borrowing", isFilled);
        }
        public static IHtmlContent RegisterIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("register", isFilled);
        }
        public static IHtmlContent FingerPointingIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("finger_pointing", isFilled);
        }
        public static IHtmlContent PaginationFirstIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("pagination_first", isFilled);
        }
        public static IHtmlContent PaginationPrevIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("pagination_prev", isFilled);
        }
        public static IHtmlContent PaginationNextIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("pagination_next", isFilled);
        }
        public static IHtmlContent PaginationLastIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("pagination_last", isFilled);
        }
        public static IHtmlContent ArrowLeftIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("arrow_left", isFilled);
        }
        public static IHtmlContent ArrowRightIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("arrow_right", isFilled);
        }
        public static IHtmlContent MediumArrowLeftIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("medium_arrow_left", isFilled);
        }
        public static IHtmlContent MediumArrowRightIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("medium_arrow_right", isFilled);
        }
        public static IHtmlContent XMarkIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("xmark", isFilled);
        }
        public static IHtmlContent DigitalFileIcon(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("digital_file", isFilled);
        }
        public static IHtmlContent GoToBackSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("gotoback", isFilled);
        }
        public static IHtmlContent SaveSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("save", isFilled);
        }
        public static IHtmlContent QrSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("qr", isFilled);
        }
        public static IHtmlContent ShareSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("share", isFilled);
        }
        public static IHtmlContent FrendlySvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("frendly", isFilled);
        }
        public static IHtmlContent InfomationSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("infomation", isFilled);
        }

        public static IHtmlContent ReadSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("read", isFilled);
        }

        public static IHtmlContent BorrowSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("borrow", isFilled);
        }

        public static IHtmlContent OpenSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("open", isFilled);
        }
        public static IHtmlContent EnjoySvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("enjoy", isFilled);
        }
        public static IHtmlContent TopSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("top", isFilled);
        }
        public static IHtmlContent HatSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("hat", isFilled);
        }
        public static IHtmlContent MapSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("map", isFilled);
        }
        public static IHtmlContent StarSvg(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("star", isFilled);
        }
        public static IHtmlContent TopSite(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("topsite", isFilled);
        }
        public static IHtmlContent PointBackGround(this IHtmlHelper htmlHelper, bool isFilled = false)
        {
            return SvgHelper.GetSvgIcon("pointbackground", isFilled);
        }
        

    }
}
