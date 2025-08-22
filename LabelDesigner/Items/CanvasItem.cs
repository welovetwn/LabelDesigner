using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using LabelDesigner.Services;

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
        /// �e�X���󪺰򥻡u�]�p�ؽu�v�]�Ǧ��u�A������ɥΡ^
        /// </summary>
        public virtual void DrawOutline(Graphics g)
        {
            using var pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        /// <summary>
        /// ����Q����ɡA�e�X�u�Ŧ��u�ءvColor.DeepSkyBlue-> Color.DarkRed
        /// </summary>
        public virtual void DrawSelection(Graphics g)
        {
            using var pen = new Pen(Color.DarkRed, 1) { DashStyle = DashStyle.Dot };
            g.DrawRectangle(pen, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        public virtual bool HitTest(PointF p) => Bounds.Contains(p);
    }
}
