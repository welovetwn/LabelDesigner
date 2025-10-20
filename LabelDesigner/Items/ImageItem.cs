// 檔案路徑：Items/ImageItem.cs

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.Json.Serialization;
using LabelDesigner.Services;

namespace LabelDesigner.Items
{
    public class ImageItem : CanvasItem, INotifyPropertyChanged
    {
        // ✅ 僅設計階段使用，不序列化
        [Browsable(true)]
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("設計階段")]
        [Description("設計時載入圖片用的檔案路徑，不會被儲存。")]
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                if (File.Exists(value))
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(value);
                        ImageBase64 = Convert.ToBase64String(bytes);
                        _cache = null;
                        OnPropertyChanged(nameof(ImageBase64));
                    }
                    catch
                    {
                        // ignore
                    }
                }
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        private string _imagePath = string.Empty;

        [Browsable(false)]
        public string ImageBase64 { get; set; } = string.Empty;

        public bool MaintainAspect { get; set; } = true;

        [JsonIgnore]
        private Image? _cache;

        private Image? LoadImage()
        {
            if (_cache != null) return _cache;

            try
            {
                if (!string.IsNullOrEmpty(ImageBase64))
                {
                    byte[] bytes = Convert.FromBase64String(ImageBase64);
                    using var ms = new MemoryStream(bytes);
                    _cache = Image.FromStream(ms);
                }
            }
            catch
            {
                _cache = null;
            }

            return _cache;
        }

        public void LoadImageAndConvertToBase64(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("圖片檔案不存在", filePath);

            byte[] bytes = File.ReadAllBytes(filePath);
            ImageBase64 = Convert.ToBase64String(bytes);
            _cache = null;
        }

        public override void Draw(Graphics g, FieldResolver resolver)
        {
            var img = LoadImage();
            if (img == null) return;

            var dest = Bounds;
            if (MaintainAspect)
            {
                float ratio = Math.Min(dest.Width / img.Width, dest.Height / img.Height);
                float w = img.Width * ratio;
                float h = img.Height * ratio;
                float x = dest.X + (dest.Width - w) / 2f;
                float y = dest.Y + (dest.Height - h) / 2f;
                dest = new RectangleF(x, y, w, h);
            }

            g.DrawImage(img, dest);
        }

        public override CanvasItem Clone()
        {
            return new ImageItem
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Bounds = this.Bounds,
                Rotation = this.Rotation,
                MaintainAspect = this.MaintainAspect,
                ImageBase64 = this.ImageBase64
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
