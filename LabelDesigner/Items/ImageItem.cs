using System;
using System.Drawing;
using System.IO;
using LabelDesigner.Services;

namespace LabelDesigner.Items
{
    public class ImageItem : CanvasItem
    {
        public string ImagePath { get; set; } = string.Empty;
        public bool MaintainAspect { get; set; } = true;

        [System.Text.Json.Serialization.JsonIgnore]
        private Image? _cache;

        private Image? LoadImage()
        {
            try
            {
                if (_cache == null && File.Exists(ImagePath))
                {
                    _cache = Image.FromFile(ImagePath);
                }
            }
            catch { _cache = null; }
            return _cache;
        }

        public override void Draw(Graphics g, FieldResolver resolver)
        {
            var img = LoadImage();
            if (img == null) return;

            var dest = Bounds;
            if (MaintainAspect)
            {
                float ratio = Math.Min(dest.Width / img.Width, dest.Height / img.Height);
                var w = img.Width * ratio;
                var h = img.Height * ratio;
                var x = dest.X + (dest.Width - w) / 2f;
                var y = dest.Y + (dest.Height - h) / 2f;
                dest = new RectangleF(x, y, w, h);
            }

            g.DrawImage(img, dest);
        }
    }
}
