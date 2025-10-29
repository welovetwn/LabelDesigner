// 檔案路徑：Items/RectangleItem.cs

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using LabelDesigner.Services;

namespace LabelDesigner.Items
{
    /// <summary>
    /// 矩形圖形物件
    /// </summary>
    public class RectangleItem : CanvasItem
    {
        private Color _borderColor = Color.Black;
        private Color _fillColor = Color.Transparent;
        
        [Description("外框顏色")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        [Description("填滿顏色 (Transparent = 不填滿)")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color FillColor
        {
            get => _fillColor;
            set => _fillColor = value;
        }

        [Description("線寬粗細")]
        public float LineWidth { get; set; } = 2f;

        [Description("外框透明度 (0-255)")]
        public int BorderAlpha
        {
            get => _borderColor.A;
            set => _borderColor = Color.FromArgb(
                Math.Clamp(value, 0, 255),
                _borderColor.R,
                _borderColor.G,
                _borderColor.B
            );
        }

        [Description("填滿透明度 (0-255)")]
        public int FillAlpha
        {
            get => _fillColor.A;
            set => _fillColor = Color.FromArgb(
                Math.Clamp(value, 0, 255),
                _fillColor.R,
                _fillColor.G,
                _fillColor.B
            );
        }

        [Description("是否填滿")]
        public bool IsFilled
        {
            get => _fillColor.A > 0 && _fillColor != Color.Transparent;
            set => _fillColor = value
                ? Color.FromArgb(128, _fillColor.R, _fillColor.G, _fillColor.B)
                : Color.Transparent;
        }

        public override void Draw(Graphics g, FieldResolver resolver)
        {
            var rect = Bounds;

            // 填滿
            if (IsFilled)
            {
                using var brush = new SolidBrush(_fillColor);
                g.FillRectangle(brush, rect);
            }

            // 外框
            if (LineWidth > 0 && _borderColor.A > 0)
            {
                using var pen = new Pen(_borderColor, LineWidth);
                // 調整矩形以避免線條超出邊界
                var drawRect = new RectangleF(
                    rect.X + LineWidth / 2,
                    rect.Y + LineWidth / 2,
                    rect.Width - LineWidth,
                    rect.Height - LineWidth
                );
                g.DrawRectangle(pen, drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height);
            }
        }

        public override CanvasItem Clone()
        {
            return new RectangleItem
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Bounds = this.Bounds,
                Rotation = this.Rotation,
                BorderColor = this.BorderColor,
                FillColor = this.FillColor,
                LineWidth = this.LineWidth
            };
        }
    }
}