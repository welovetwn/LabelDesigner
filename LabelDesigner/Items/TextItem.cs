using System;
using System.Drawing;
using System.Text.RegularExpressions;
using LabelDesigner.Services;

namespace LabelDesigner.Items
{
    public class TextItem : CanvasItem
    {
        public string Text { get; set; } = "Text";
        public string FontFamily { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 12f;
        public System.Drawing.FontStyle FontStyle { get; set; } = System.Drawing.FontStyle.Regular;
        public System.Drawing.Color Color { get; set; } = System.Drawing.Color.Black;
        public ContentAlignment Alignment { get; set; } = ContentAlignment.MiddleLeft;

        public override void Draw(Graphics g, FieldResolver resolver)
        {
            using var font = new Font(FontFamily, FontSize, FontStyle);
            using var brush = new SolidBrush(Color);
            var rect = Bounds;
            var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = Alignment switch
            {
                ContentAlignment.MiddleLeft or ContentAlignment.TopLeft or ContentAlignment.BottomLeft => StringAlignment.Near,
                ContentAlignment.MiddleCenter or ContentAlignment.TopCenter or ContentAlignment.BottomCenter => StringAlignment.Center,
                _ => StringAlignment.Far
            };

            string resolved = resolver.Resolve(Text);
            g.DrawString(resolved, font, brush, rect, sf);
        }
    }
}
