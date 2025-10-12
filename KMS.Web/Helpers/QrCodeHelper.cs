using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace KMS.Web.Helpers
{
    public class QrCodeHelper : AbstractQRCode, IDisposable
    {
        public QrCodeHelper() { }

        public QrCodeHelper(QRCodeData data)
            : base(data)
        {
        }

        public Image<Rgba32> GetGraphicImageSharp(
            int pixelsPerModule,
            Rgba32 darkColor,
            Rgba32 lightColor,
            Image<Rgba32>? icon = null,
            int iconSizePercent = 15,
            int iconBorderWidth = 0,
            bool drawQuietZones = true,
            Rgba32? iconBackgroundColor = null)
        {
            var matrix = QrCodeData.ModuleMatrix;
            int size = (matrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            int offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var image = new Image<Rgba32>(size, size);
            image.Mutate(ctx =>
            {
                ctx.Fill(lightColor);

                // Vẽ các module đen nhanh hơn
                var rects = new List<Rectangle>();
                for (int y = 0; y < matrix.Count; y++)
                {
                    for (int x = 0; x < matrix[y].Count; x++)
                    {
                        if (matrix[y][x])
                        {
                            int px = (x * pixelsPerModule) - offset;
                            int py = (y * pixelsPerModule) - offset;
                            if (px >= 0 && py >= 0 && px < size && py < size)
                                rects.Add(new Rectangle(px, py, pixelsPerModule, pixelsPerModule));
                        }
                    }
                }

                var shapes = rects.Select(r => new RectangularPolygon(r.X, r.Y, r.Width, r.Height));
                ctx.Fill(darkColor, new PathCollection(shapes));

                // Vẽ icon nếu có
                if (icon != null && iconSizePercent > 0 && iconSizePercent <= 100)
                {
                    int iconSize = size * iconSizePercent / 100;
                    int iconX = (size - iconSize) / 2;
                    int iconY = (size - iconSize) / 2;

                    if (iconBorderWidth > 0)
                    {
                        int borderSize = iconSize + iconBorderWidth * 2;
                        var backgroundColor = iconBackgroundColor ?? lightColor;
                        var circle = new EllipsePolygon(iconX + iconSize / 2, iconY + iconSize / 2, borderSize / 2);
                        ctx.Fill(backgroundColor, circle);
                    }

                    using (var resizedIcon = icon.Clone(i => i.Resize(iconSize, iconSize)))
                    {
                        ctx.DrawImage(resizedIcon, new Point(iconX, iconY), 1f);
                    }
                }
            });

            return image;
        }
    }
}