namespace KMS.Web.Common
{
    public static class QSBibType
    {
        public static readonly KeyValuePair<string, string> All = new KeyValuePair<string, string>("Tất cả", "qs");
        public static readonly KeyValuePair<string, string> Title = new KeyValuePair<string, string>("Nhan đề", "ti");
        public static readonly KeyValuePair<string, string> FtuTitle = new KeyValuePair<string, string>("Tên tài liệu", "ti");
        public static readonly KeyValuePair<string, string> Subject = new KeyValuePair<string, string>("Chủ đề", "su");
        public static readonly KeyValuePair<string, string> BibType = new KeyValuePair<string, string>("Loại tài liệu", "bt");
        public static readonly KeyValuePair<string, string> Author = new KeyValuePair<string, string>("Tác giả", "au");
        public static readonly KeyValuePair<string, string> YearPub = new KeyValuePair<string, string>("Năm xuất bản", "yr");
        public static readonly KeyValuePair<string, string> Keyword = new KeyValuePair<string, string>("Từ khóa", "kw");
        public static readonly KeyValuePair<string, string> ISBN = new KeyValuePair<string, string>("ISBN", "bn");
        public static readonly KeyValuePair<string, string> Regstr = new KeyValuePair<string, string>("Mã ĐKCB", "bc");
        public static readonly KeyValuePair<string, string> DDoc = new KeyValuePair<string, string>("Tài liệu số", "dbtype_ddoc");
        public static readonly KeyValuePair<string, string> PDoc = new KeyValuePair<string, string>("Tài liệu in", "dbtype_pdoc");
        public static readonly KeyValuePair<string, string> SYear = new KeyValuePair<string, string>("Từ năm", "s_yr");
        public static readonly KeyValuePair<string, string> EYear = new KeyValuePair<string, string>("Đến năm", "e_yr");
    }

    public static class DocDetailClassifiedFile
    {
        public static readonly Tuple<string, string, List<string>> document = new Tuple<string, string, List<string>>("File tài liệu", "document", new List<string> { ".docx", ".doc", ".docs", ".txt", ".pdf", ".rtf", ".odt", ".html", ".xml", ".epub", ".mobi" });
        public static readonly Tuple<string, string, List<string>> media = new Tuple<string, string, List<string>>("File âm thanh", "media", new List<string> { ".mp3", ".mp4", ".wav", ".avi", ".mkv", ".flac", ".mov" });
    }
}