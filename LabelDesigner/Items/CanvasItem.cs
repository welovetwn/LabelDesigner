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
        /// 畫出物件的基本「設計框線」（灰色虛線，未選取時用）
        /// </summary>
        public virtual void DrawOutline(Graphics g)
        {
            using var pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        /// <summary>
        /// 當物件被選取時，畫出「紅色點狀框」
        /// </summary>
        public virtual void DrawSelection(Graphics g)
        {
            using var pen = new Pen(Color.DarkRed, 1) { DashStyle = DashStyle.Dot };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

            // 畫 8 個 resize handles
            foreach (var handle in GetResizeHandles())
            {
                using var brush = new SolidBrush(Color.White);
                g.FillRectangle(brush, handle);
                g.DrawRectangle(Pens.DarkRed, handle.X, handle.Y, handle.Width, handle.Height);
            }
        }

        public virtual bool HitTest(PointF p) => Bounds.Contains(p);

        /// <summary>
        /// 取得 8 個 resize 控制點
        /// </summary>
        public virtual List<RectangleF> GetResizeHandles(float handleSize = 6f)
        {
            var handles = new List<RectangleF>();

            float x = Bounds.X;
            float y = Bounds.Y;
            float w = Bounds.Width;
            float h = Bounds.Height;

            float hs = handleSize;

            // 八個控制點 (左上、上中、右上、右中、右下、下中、左下、左中)
            handles.Add(new RectangleF(x - hs / 2, y - hs / 2, hs, hs));             // 左上
            handles.Add(new RectangleF(x + w / 2 - hs / 2, y - hs / 2, hs, hs));     // 上中
            handles.Add(new RectangleF(x + w - hs / 2, y - hs / 2, hs, hs));         // 右上
            handles.Add(new RectangleF(x + w - hs / 2, y + h / 2 - hs / 2, hs, hs)); // 右中
            handles.Add(new RectangleF(x + w - hs / 2, y + h - hs / 2, hs, hs));     // 右下
            handles.Add(new RectangleF(x + w / 2 - hs / 2, y + h - hs / 2, hs, hs)); // 下中
            handles.Add(new RectangleF(x - hs / 2, y + h - hs / 2, hs, hs));         // 左下
            handles.Add(new RectangleF(x - hs / 2, y + h / 2 - hs / 2, hs, hs));     // 左中

            return handles;
        }
    }
}
