// 檔案路徑：Items/CanvasItem.cs

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using LabelDesigner.Services;
using System.Collections.Generic;

namespace LabelDesigner.Items
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(TextItem), typeDiscriminator: "text")]
    [JsonDerivedType(typeof(ImageItem), typeDiscriminator: "image")]
    [JsonDerivedType(typeof(BarcodeItem), typeDiscriminator: "barcode")]
    [JsonDerivedType(typeof(RectangleItem), typeDiscriminator: "rectangle")]
    [JsonDerivedType(typeof(LineItem), typeDiscriminator: "line")]
    [JsonDerivedType(typeof(CircleItem), typeDiscriminator: "circle")]
    public abstract class CanvasItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Item";
        public RectangleF Bounds { get; set; } = new RectangleF(0, 0, 100, 40);
        public float Rotation { get; set; } = 0f;

        [JsonIgnore]
        public bool Selected { get; set; }

        public abstract void Draw(Graphics g, FieldResolver resolver);
        public abstract CanvasItem Clone();

        public virtual void DrawOutline(Graphics g)
        {
            using var pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        public virtual void DrawSelection(Graphics g)
        {
            using var pen = new Pen(Color.DarkRed, 1) { DashStyle = DashStyle.Dot };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

            foreach (var handle in GetResizeHandles())
            {
                g.FillRectangle(Brushes.White, handle);
                g.DrawRectangle(Pens.Black, handle.X, handle.Y, handle.Width, handle.Height);
            }
        }

        public virtual List<RectangleF> GetResizeHandles(float size = 10f)
        {
            var list = new List<RectangleF>();
            float half = size / 2f;

            list.Add(new RectangleF(Bounds.Left - half, Bounds.Top - half, size, size));
            list.Add(new RectangleF(Bounds.Left + Bounds.Width / 2 - half, Bounds.Top - half, size, size));
            list.Add(new RectangleF(Bounds.Right - half, Bounds.Top - half, size, size));
            list.Add(new RectangleF(Bounds.Right - half, Bounds.Top + Bounds.Height / 2 - half, size, size));
            list.Add(new RectangleF(Bounds.Right - half, Bounds.Bottom - half, size, size));
            list.Add(new RectangleF(Bounds.Left + Bounds.Width / 2 - half, Bounds.Bottom - half, size, size));
            list.Add(new RectangleF(Bounds.Left - half, Bounds.Bottom - half, size, size));
            list.Add(new RectangleF(Bounds.Left - half, Bounds.Top + Bounds.Height / 2 - half, size, size));

            return list;
        }

        public virtual bool HitTest(PointF p) => Bounds.Contains(p);
    }
}
