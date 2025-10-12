namespace KMS.Web.Core
{
    public static class DefaultCoverPhoto
    {
        public static Dictionary<string, (string ImagePath, string[] Keywords)> BibTypeKeywords = new()
        {
            { "Sách", ("def-cp-bt/sach.webp", new[] { "sách", "tài liệu", "giáo trình", "ebook", "sách điện tử", "tư liệu" }) },
            { "Văn bản pháp luật", ("def-cp-bt/van_ban_phap_luat.webp", new[] { "pháp luật", "văn bản pháp luật", "luật", "nghị định", "thông tư", "quy định", "hướng dẫn", "chính sách" }) },
            { "Âm nhạc", ("def-cp-bt/am_nhac.webp", new[] { "âm nhạc", "bài hát", "giai điệu", "bản nhạc", "ca khúc", "hòa âm", "guitar", "piano", "karaoke" }) },
            { "Báo, Tạp chí số", ("def-cp-bt/bao_tap_chi_so.webp", new[] { "báo", "tạp chí", "tin tức", "thời sự", "báo mạng", "báo giấy", "bản tin", "tin nhanh" }) },
            { "Bản đồ", ("def-cp-bt/ban_do.webp", new[] { "bản đồ", "địa lý", "địa danh", "địa chính", "hành chính", "đường đi", "hướng dẫn đường", "atlas" }) },
            { "Sách Địa chí", ("def-cp-bt/dia_chi.webp", new[] { "địa chí", "sách địa chí", "địa phương", "đặc sản vùng miền", "địa danh nổi tiếng", "văn hóa địa phương" }) },
            { "Khác", ("def-cp-bt/khac.webp", new[] { "khác", "tổng hợp", "misc", "đa thể loại", "chưa phân loại" }) }
        };

        public static string GetImageForBibType(string bibType)
        {
            if (BibTypeKeywords.TryGetValue(bibType, out var exactMatch))
            {
                return exactMatch.ImagePath;
            }

            foreach (var (key, (imagePath, keywords)) in BibTypeKeywords)
            {
                if (keywords.Any(keyword => bibType.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                {
                    return imagePath;
                }
            }

            return BibTypeKeywords["Khác"].ImagePath;
        }
    }
}