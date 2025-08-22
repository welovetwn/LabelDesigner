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
    public abstract class CanvasItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Item";
        public RectangleF Bounds { get; set; } = new RectangleF(0, 0, 100, 40);
        public float Rotation { get; set; } = 0f;

        [JsonIgnore]
        public bool Selected { get; set; }

        public abstract void Draw(Graphics g, FieldResolver resolver);

        /// <summary>
        /// 每個子類別都要實作「深複製」
        /// </summary>
        public abstract CanvasItem Clone();

        /// <summary>
        /// 灰色虛線框 (所有物件都會有)
        /// </summary>
        public virtual void DrawOutline(Graphics g)
        {
            using var pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        /// <summary>
        /// 當物件被選取時，畫紅色虛線框 + 8 個控制點
        /// </summary>
        public virtual void DrawSelection(Graphics g)
        {
            using var pen = new Pen(Color.DarkRed, 1) { DashStyle = DashStyle.Dot };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

            // 畫控制點
            foreach (var handle in GetResizeHandles())
            {
                g.FillRectangle(Brushes.White, handle);
                g.DrawRectangle(Pens.Black, handle.X, handle.Y, handle.Width, handle.Height);
            }
        }

        /// <summary>
        /// 取得 8 個縮放控制點 (四角 + 四邊中點)
        /// </summary>
        public virtual List<RectangleF> GetResizeHandles(float size = 10f)
        {
            var list = new List<RectangleF>();
            float half = size / 2f;

            // 四角 + 四邊
            list.Add(new RectangleF(Bounds.Left - half, Bounds.Top - half, size, size)); // 左上
            list.Add(new RectangleF(Bounds.Left + Bounds.Width / 2 - half, Bounds.Top - half, size, size)); // 上中
            list.Add(new RectangleF(Bounds.Right - half, Bounds.Top - half, size, size)); // 右上
            list.Add(new RectangleF(Bounds.Right - half, Bounds.Top + Bounds.Height / 2 - half, size, size)); // 右中
            list.Add(new RectangleF(Bounds.Right - half, Bounds.Bottom - half, size, size)); // 右下
            list.Add(new RectangleF(Bounds.Left + Bounds.Width / 2 - half, Bounds.Bottom - half, size, size)); // 下中
            list.Add(new RectangleF(Bounds.Left - half, Bounds.Bottom - half, size, size)); // 左下
            list.Add(new RectangleF(Bounds.Left - half, Bounds.Top + Bounds.Height / 2 - half, size, size)); // 左中

            return list;
        }

        public virtual bool HitTest(PointF p) => Bounds.Contains(p);
    }
}
