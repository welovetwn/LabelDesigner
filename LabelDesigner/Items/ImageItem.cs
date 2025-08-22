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

        /// <summary>
        /// 建立這個物件的深複製
        /// </summary>
        public override CanvasItem Clone()
        {
            return new ImageItem
            {
                Id = Guid.NewGuid(),   // ⚡ 新的 Guid
                Name = this.Name,
                Bounds = this.Bounds,
                Rotation = this.Rotation,
                ImagePath = this.ImagePath,
                MaintainAspect = this.MaintainAspect
                // _cache 不複製，因為貼上後會自動重新載入
            };
        }
    }
}
