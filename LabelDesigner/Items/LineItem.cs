// 檔案路徑：Items/LineItem.cs

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using LabelDesigner.Services;

namespace LabelDesigner.Items
{
    /// <summary>
    /// 直線圖形物件
    /// </summary>
    public class LineItem : CanvasItem
    {
        private Color _lineColor = Color.Black;

        [Description("線條顏色")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color LineColor
        {
            get => _lineColor;
            set => _lineColor = value;
        }

        [Description("線寬粗細")]
        public float LineWidth { get; set; } = 2f;

        [Description("線條透明度 (0-255)")]
        public int LineAlpha
        {
            get => _lineColor.A;
            set => _lineColor = Color.FromArgb(
                Math.Clamp(value, 0, 255),
                _lineColor.R,
                _lineColor.G,
                _lineColor.B
            );
        }

        [Description("線條樣式")]
        public DashStyle DashStyle { get; set; } = DashStyle.Solid;

        public override void Draw(Graphics g, FieldResolver resolver)
        {
            if (LineWidth <= 0 || _lineColor.A == 0) return;

            using var pen = new Pen(_lineColor, LineWidth)
            {
                DashStyle = this.DashStyle
            };

            // 從左上角畫到右下角
            g.DrawLine(pen,
                Bounds.Left,
                Bounds.Top,
                Bounds.Right,
                Bounds.Bottom
            );
        }

        public override CanvasItem Clone()
        {
            return new LineItem
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Bounds = this.Bounds,
                Rotation = this.Rotation,
                LineColor = this.LineColor,
                LineWidth = this.LineWidth,
                DashStyle = this.DashStyle
            };
        }
    }
}